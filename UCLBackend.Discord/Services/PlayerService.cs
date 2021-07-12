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

        public async Task AddPlayer(ulong discordID, string playername, string region, string rlTrackerLink, string[] altRLTrackerLinks)
        {
            // Create a new HTTP client
            var client = new HttpClient();
            // Set the request content
            var content = new AddPlayerRequest
            {
                DiscordID = discordID,
                PlayerName = playername,
                Region = region,
                RLTrackerLink = rlTrackerLink,
                AltRLTrackerLinks = altRLTrackerLinks
            };

            var body = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

            // Send the request
            var response = await client.PostAsync($"{_backendUrl}/Player/AddPlayer", body);
            response.EnsureSuccessStatusCode();
        }

        public async Task SignPlayer(ulong discordID, string franchiseName)
        {
            // Create a new HTTP client
            var client = new HttpClient();
            // Set the request content
            var content = new SignPlayerRequest
            {
                DiscordID = discordID,
                FranchiseName = franchiseName
            };

            var body = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

            // Send the request
            var response = await client.PutAsync($"{_backendUrl}/Player/SignPlayer", body);
            response.EnsureSuccessStatusCode();
        }

        public async Task ReleasePlayer(ulong discordID)
        {
            // Create a new HTTP client
            var client = new HttpClient();
            // Set the request content
            var content = new BaseRequest
            {
                DiscordID = discordID
            };

            var body = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

            // Send the request
            var response = await client.PutAsync($"{_backendUrl}/Player/ReleasePlayer", body);
            response.EnsureSuccessStatusCode();
        }

        public async Task<PlayerInfoResponse> GetPlayerInfo(ulong discordID)
        {
            // Create a new HTTP client
            var client = new HttpClient();

            // Send the request
            var response = await client.GetAsync($"{_backendUrl}/Player/PlayerInfo?id={discordID}");
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<PlayerInfoResponse>(await response.Content.ReadAsStringAsync());
        }
    }
}
