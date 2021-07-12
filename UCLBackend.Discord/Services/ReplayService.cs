using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UCLBackend.Discord.Interfaces.Services;
using UCLBackend.Service.Data.Requests;

// TODO: Change to usings for all HttpClients and responses (in UCLBackend.Service as well)

namespace UCLBackend.Discord.Services
{
    public class ReplayService : IReplayService
    {
        private readonly string _backendUrl;

        public ReplayService()
        {
            _backendUrl = Environment.GetEnvironmentVariable("BACKEND_URL");
        }

        public async Task BeginUploadProcess(ulong userId)
        {
            // Create a new HTTP client
            var client = new HttpClient();
            // Set the request content
            var content = new BaseRequest
            {
                DiscordID = userId
            };

            var body = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

            // Send the request
            var response = await client.PutAsync($"{_backendUrl}/Replay/BeginUpload", body);
            response.EnsureSuccessStatusCode();
        }

        public async Task QueueReplay(ulong userId, string replayFileUrl)
        {
            // Create a new HTTP client
            var client = new HttpClient();
            // Set the request content
            var content = new QueueReplayRequest
            {
                DiscordID = userId,
                ReplayFileUrl = replayFileUrl
            };

            var body = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

            // Send the request
            var response = await client.PutAsync($"{_backendUrl}/Replay/QueueReplay", body);
            response.EnsureSuccessStatusCode();
        }

        public async Task EndUploadProcess(ulong userId)
        {
            // Create a new HTTP client
            var client = new HttpClient();
            // Set the request content
            var content = new BaseRequest
            {
                DiscordID = userId
            };

            var body = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

            // Send the request
            var response = await client.PutAsync($"{_backendUrl}/Replay/EndUpload", body);
            response.EnsureSuccessStatusCode();
        }
    }
}
