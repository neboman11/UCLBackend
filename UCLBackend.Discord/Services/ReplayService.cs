using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UCLBackend.Discord.Interfaces.Services;
using UCLBackend.Service.Data.Helpers;
using UCLBackend.Service.Data.Requests;

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
            Uri uri = new Uri($"{_backendUrl}/Replay/BeginUpload");

            // Set the request content
            var body = new BaseRequest
            {
                DiscordID = userId
            };

            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            await SendWebRequest.PutAsync(uri, null, content);
        }

        public async Task QueueReplay(ulong userId, string replayFileUrl)
        {
            Uri uri = new Uri($"{_backendUrl}/Replay/QueueReplay");

            // Set the request content
            var body = new QueueReplayRequest
            {
                DiscordID = userId,
                ReplayFileUrl = replayFileUrl
            };

            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            await SendWebRequest.PutAsync(uri, null, content);
        }

        public async Task EndUploadProcess(ulong userId)
        {
            Uri uri = new Uri($"{_backendUrl}/Replay/EndUpload");

            // Set the request content
            var body = new BaseRequest
            {
                DiscordID = userId
            };

            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            await SendWebRequest.PutAsync(uri, null, content);
        }
    }
}
