using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using UCLBackend.DataAccess.Models.Responses;
using UCLBackend.Discord.Requests;
using UCLBackend.Service.Data.Enums;
using UCLBackend.Service.DataAccess.Interfaces;
using UCLBackend.Service.DataAccess.Models;
using UCLBackend.Service.Models.Responses;
using UCLBackend.Service.Services.Interfaces;

// TODO: Change to usings for all HttpClients and responses (in UCLBackend.Service as well)

namespace UCLBackend.Service.Services
{
    public class ReplayService : IReplayService
    {
        private readonly string _ballchasingApiKey;
        private readonly IRedisService _redisService;
        private readonly IReplayRepository _replayRepository;
        private readonly ISettingRepository _settingRepository;

        public ReplayService(IRedisService redisService, IReplayRepository replayRepository, IConfiguration configuration, ISettingRepository settingRepository)
        {
            _redisService = redisService;
            _replayRepository = replayRepository;
            _ballchasingApiKey = configuration.GetSection("Ballchasing").GetValue<string>("ApiKey");
            _settingRepository = settingRepository;
        }

        public async Task BeginUploadProcess(ulong userId)
        {
            await _redisService.StoreValue($"upload_{userId}", $"replay_{userId}");
        }

        public async Task QueueReplay(ulong userId, string replayFileUrl)
        {
            var urls = _redisService.RetrieveValue($"replay_{userId}");

            if (urls == null)
            {
                await _redisService.StoreValue($"replay_{userId}", replayFileUrl);
            }
            else
            {
                await _redisService.StoreValue($"replay_{userId}", urls + "," + replayFileUrl);
            }
        }

        public async Task EndUploadProcess(ulong userId)
        {
            var urlsKey = await _redisService.RetrieveValue($"upload_{userId}");
            var urls = await _redisService.RetrieveValue(urlsKey);

            // TODO: Figure out how to get league
            var groupId = await CreateBallchasingGroup($"UCL-{DateTime.Now.ToString(CultureInfo.InvariantCulture)}", PlayerLeague.Origins);

            // Split urls then post them to ballchasing.com
            var urlsArray = urls.Split(',');
            foreach (var url in urlsArray)
            {
                // Create a new HTTP client
                var client = new HttpClient();
                Uri uri = new Uri(url);

                // Send the request
                var response = await client.GetAsync(uri.ToString());
                response.EnsureSuccessStatusCode();

                await SendFileToBallchasing(await response.Content.ReadAsByteArrayAsync(), url.Split('/').Last(), groupId);
            }

            var stats = await FetchGroupStats(groupId);
            StoreGroupPlayerStandings(stats);

            // Delete the key
            await _redisService.RemoveValue($"upload_{userId}");
            await _redisService.RemoveValue($"replay_{userId}");
        }

        #region Private Methods
        private async Task SendFileToBallchasing(byte[] file, string fileName, string groupId)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", _ballchasingApiKey);

            var content = new MultipartFormDataContent("----" + DateTime.Now.ToString(CultureInfo.InvariantCulture));
            content.Headers.ContentType.MediaType = "multipart/form-data";
            content.Add(new ByteArrayContent(file), "file", fileName);

            // Send the request
            var response = await client.PostAsync($"https://ballchasing.com/api/v2/upload?group={groupId}", content);
            response.EnsureSuccessStatusCode();
        }

        private async Task<string> CreateBallchasingGroup(string groupName, PlayerLeague league)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", _ballchasingApiKey);

            var content = new CreateBallchasingGroupRequest
            {
                Name = groupName,
                PlayerIdentification = "by-id",
                TeamIdentification = "by-distinct-players"
            };

            var body = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

            string parentGroup = "";
            switch (league)
            {
                case PlayerLeague.Origins:
                    parentGroup = _settingRepository.GetSetting("Ballchasing.Origins.Group");
                    break;
                case PlayerLeague.Ultra:
                    parentGroup = _settingRepository.GetSetting("Ballchasing.Ultra.Group");
                    break;
                case PlayerLeague.Elite:
                    parentGroup = _settingRepository.GetSetting("Ballchasing.Elite.Group");
                    break;
                case PlayerLeague.Superior:
                    parentGroup = _settingRepository.GetSetting("Ballchasing.Superior.Group");
                    break;
            }

            // Send the request
            var response = await client.PostAsync($"https://ballchasing.com/api/groups?parent={parentGroup}", body);
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<CreateBallchasingGroupResponse>(await response.Content.ReadAsStringAsync()).Id;
        }

        private async Task<GetBallchasingGroupResponse> FetchGroupStats(string groupId)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", _ballchasingApiKey);

            var response = await client.GetAsync($"https://ballchasing.com/api/groups/{groupId}");
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<GetBallchasingGroupResponse>(await response.Content.ReadAsStringAsync());
        }

        private void StoreGroupPlayerStandings(GetBallchasingGroupResponse groupStats)
        {
            foreach (var groupPlayer in groupStats.Players)
            {
                var player = _replayRepository.GetPlayerByAccount(groupPlayer.Platform, groupPlayer.Id);
                var standing = new Standing
                {
                    Goals = groupPlayer.Cumulative.Core.Goals,
                    Assists = groupPlayer.Cumulative.Core.Assists,
                    Saves = groupPlayer.Cumulative.Core.Saves,
                    Score = groupPlayer.Cumulative.Core.Score,
                    Shots = groupPlayer.Cumulative.Core.Shots,
                    Player = player
                };

                _replayRepository.StoreGroupPlayerStanding(standing);
            }
        }
        #endregion
    }
}
