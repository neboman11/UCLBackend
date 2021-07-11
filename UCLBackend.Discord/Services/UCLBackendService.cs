using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UCLBackend.Discord.Interfaces.Services;
using UCLBackend.Service.Data.Requests;

namespace UCLBackend.Discord.Services
{
    public class UCLBackendService : IUCLBackendService
    {
        private readonly string _backendUrl;

        public UCLBackendService()
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
    }
}