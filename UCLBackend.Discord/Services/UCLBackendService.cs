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
        public async Task<bool> AddPlayer(ulong discordID, string playername, string region, string rlTrackerLink, string[] altRLTrackerLinks)
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
            // TODO: Replace with env var
            var response = await client.PostAsync("http://localhost:5000/Player/AddPlayer", body);
            // Check if the response was successful
            if (response.IsSuccessStatusCode)
            {
                // Return true if the response was successful
                return true;
            }
            // Return false if the response was not successful
            return false;
        }
    }
}