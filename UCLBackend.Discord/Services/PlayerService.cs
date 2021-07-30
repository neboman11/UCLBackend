using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UCLBackend.Service.Data.Requests;
using UCLBackend.Discord.Interfaces.Services;
using UCLBackend.Service.Data.Responses;
using UCLBackend.Service.Data.Helpers;

namespace UCLBackend.Discord.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly string _backendUrl;

        public PlayerService()
        {
            _backendUrl = Environment.GetEnvironmentVariable("BACKEND_URL");
        }

        public async Task AddPlayer(ulong issuerDiscordID, ulong discordID, string playername, string rlTrackerLink, string[] altRLTrackerLinks)
        {
            Uri uri = new Uri($"{_backendUrl}/Player/AddPlayer");
            // Set the request content
            var body = new AddPlayerRequest
            {
                IssuerDiscordID = issuerDiscordID,
                DiscordID = discordID,
                PlayerName = playername,
                RLTrackerLink = rlTrackerLink,
                AltRLTrackerLinks = altRLTrackerLinks
            };

            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            var response = await SendWebRequest.PostAsync<BaseResponse>(uri, null, content);
        }

        public async Task SignPlayer(ulong issuerDiscordID, ulong discordID, string franchiseName)
        {
            Uri uri = new Uri($"{_backendUrl}/Player/SignPlayer");

            // Set the request content
            var body = new SignPlayerRequest
            {
                IssuerDiscordID = issuerDiscordID,
                DiscordID = discordID,
                FranchiseName = franchiseName
            };

            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            await SendWebRequest.PutAsync(uri, null, content);
        }

        public async Task ReleasePlayer(ulong issuerDiscordID, ulong discordID)
        {
            Uri uri = new Uri($"{_backendUrl}/Player/ReleasePlayer");

            // Set the request content
            var body = new BaseRequest
            {
                IssuerDiscordID = issuerDiscordID,
                DiscordID = discordID
            };

            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            await SendWebRequest.PutAsync(uri, null, content);
        }

        public async Task<PlayerInfoResponse> GetPlayerInfo(ulong discordID)
        {
            Uri uri = new Uri($"{_backendUrl}/Player/PlayerInfo?id={discordID}");

            var response = await SendWebRequest.GetAsync<PlayerInfoResponse>(uri, null);
            
            return response;
        }

        public async Task PlayerRankout(ulong issuerDiscordID, ulong discordID)
        {
            Uri uri = new Uri($"{_backendUrl}/Player/PlayerRankout");

            // Set the request content
            var body = new BaseRequest
            {
                IssuerDiscordID = issuerDiscordID,
                DiscordID = discordID
            };

            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            await SendWebRequest.PutAsync(uri, null, content);
        }
    }
}
