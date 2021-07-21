using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UCLBackend.Service.Data.Requests;
using UCLBackend.Discord.Interfaces.Services;
using UCLBackend.Service.Data.Responses;

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
            // Create a new HTTP client
            var client = new HttpClient();
            // Set the request content
            var content = new AddPlayerRequest
            {
                IssuerDiscordID = issuerDiscordID,
                DiscordID = discordID,
                PlayerName = playername,
                RLTrackerLink = rlTrackerLink,
                AltRLTrackerLinks = altRLTrackerLinks
            };

            var body = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

            // Send the request
            var response = await client.PostAsync($"{_backendUrl}/Player/AddPlayer", body);

            // Check the response
            if (!response.IsSuccessStatusCode)
            {
                var error = JsonConvert.DeserializeObject<BaseResponse>(await response.Content.ReadAsStringAsync());
                throw new Exception(error.ErrorMessage);
            }
        }

        public async Task SignPlayer(ulong issuerDiscordID, ulong discordID, string franchiseName)
        {
            // Create a new HTTP client
            var client = new HttpClient();
            // Set the request content
            var content = new SignPlayerRequest
            {
                IssuerDiscordID = issuerDiscordID,
                DiscordID = discordID,
                FranchiseName = franchiseName
            };

            var body = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

            // Send the request
            var response = await client.PutAsync($"{_backendUrl}/Player/SignPlayer", body);

            // Check the response
            if (!response.IsSuccessStatusCode)
            {
                var error = JsonConvert.DeserializeObject<BaseResponse>(await response.Content.ReadAsStringAsync());
                throw new Exception(error.ErrorMessage);
            }
        }

        public async Task ReleasePlayer(ulong issuerDiscordID, ulong discordID)
        {
            // Create a new HTTP client
            var client = new HttpClient();
            // Set the request content
            var content = new BaseRequest
            {
                IssuerDiscordID = issuerDiscordID,
                DiscordID = discordID
            };

            var body = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

            // Send the request
            var response = await client.PutAsync($"{_backendUrl}/Player/ReleasePlayer", body);

            // Check the response
            if (!response.IsSuccessStatusCode)
            {
                var error = JsonConvert.DeserializeObject<BaseResponse>(await response.Content.ReadAsStringAsync());
                throw new Exception(error.ErrorMessage);
            }
        }

        public async Task<PlayerInfoResponse> GetPlayerInfo(ulong discordID)
        {
            // Create a new HTTP client
            var client = new HttpClient();

            // Send the request
            var response = await client.GetAsync($"{_backendUrl}/Player/PlayerInfo?id={discordID}");

            // Check the response
            if (!response.IsSuccessStatusCode)
            {
                var error = JsonConvert.DeserializeObject<BaseResponse>(await response.Content.ReadAsStringAsync());
                throw new Exception(error.ErrorMessage);
            }
            
            return JsonConvert.DeserializeObject<PlayerInfoResponse>(await response.Content.ReadAsStringAsync());
        }
    }
}
