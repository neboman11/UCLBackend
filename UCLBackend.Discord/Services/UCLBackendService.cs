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
        public async Task<bool> AddPlayer(ulong discordID, string username, string platform, string region, string rlTrackerLink, string[] altRLTrackerLinks)
        {
            // Create a new HTTP client
            var client = new HttpClient();
            // Set the request content
            var content = new AddUserRequest
            {
                DiscordID = discordID,
                Username = username,
                Platform = platform,
                Region = region,
                RLTrackerLink = rlTrackerLink,
                AltRLTrackerLinks = altRLTrackerLinks
            };

            var body = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

            // Send the request
            var response = await client.PostAsync("http://localhost:5000/User/AddUser", body);
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