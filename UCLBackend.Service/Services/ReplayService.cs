using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UCLBackend.Discord.Requests;
using UCLBackend.Service.Models.Responses;
using UCLBackend.Service.Services.Interfaces;

// TODO: Change to usings for all HttpClients and responses (in UCLBackend.Service as well)

namespace UCLBackend.Service.Services
{
    public class ReplayService : IReplayService
    {
        private readonly IRedisService _redisService;

        public ReplayService(IRedisService redisService)
        {
            _redisService = redisService;
        }

        public void BeginUploadProcess(ulong userId)
        {
            _redisService.StoreValue($"upload_{userId}", $"replay_{userId}");
        }

        public void QueueReplay(ulong userId, string replayFileUrl)
        {
            var urls = _redisService.RetrieveValue($"replay_{userId}");

            if (urls == null)
            {
                _redisService.StoreValue($"replay_{userId}", replayFileUrl);
            }
            else
            {
                _redisService.StoreValue($"replay_{userId}", urls + "," + replayFileUrl);
            }
        }

        public async Task EndUploadProcess(ulong userId)
        {
            var urlsKey = _redisService.RetrieveValue($"upload_{userId}");
            var urls = _redisService.RetrieveValue(urlsKey);

            var groupId = await CreateBallchasingGroup($"UCL-{DateTime.Now.ToString(CultureInfo.InvariantCulture)}");

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

            // Delete the key
            _redisService.RemoveValue($"upload_{userId}");
            _redisService.RemoveValue($"replay_{userId}");
        }

        #region Private Methods
        private async Task SendFileToBallchasing(byte[] file, string fileName, string groupId)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Z8ucsdFkmM0dHa7eovEosSSm5c3gpHY5LPd4kypf");

            var content = new MultipartFormDataContent("----" + DateTime.Now.ToString(CultureInfo.InvariantCulture));
            content.Headers.ContentType.MediaType = "multipart/form-data";
            content.Add(new ByteArrayContent(file), "file", fileName);

            // Send the request
            var response = await client.PostAsync($"https://ballchasing.com/api/v2/upload?group={groupId}", content);
            response.EnsureSuccessStatusCode();
        }

        private async Task<string> CreateBallchasingGroup(string groupName)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Z8ucsdFkmM0dHa7eovEosSSm5c3gpHY5LPd4kypf");

            var content = new CreateBallchasingGroupRequest
            {
                Name = groupName,
                PlayerIdentification = "by-id",
                TeamIdentification = "by-distinct-players"
            };

            var body = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

            // Send the request
            var response = await client.PostAsync("https://ballchasing.com/api/groups", body);
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<CreateBallchasingGroupResponse>(await response.Content.ReadAsStringAsync()).Id;
        }
        #endregion
    }
}
