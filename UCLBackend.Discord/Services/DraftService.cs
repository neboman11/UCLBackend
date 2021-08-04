using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UCLBackend.Discord.Interfaces.Services;
using UCLBackend.Service.Data.Enums;
using UCLBackend.Service.Data.Helpers;
using UCLBackend.Service.Data.Requests;

namespace UCLBackend.Discord.Services
{
    public class DraftService : IDraftService
    {
        private readonly string _backendUrl;

        public DraftService()
        {
            _backendUrl = Environment.GetEnvironmentVariable("BACKEND_URL");
        }

        public async Task StartDraft(ulong issuerDiscordID)
        {
            Uri uri = new Uri($"{_backendUrl}/Draft/StartDraft");

            // Set the request content
            var body = new BaseRequest
            {
                IssuerDiscordID = issuerDiscordID
            };

            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            await SendWebRequest.PostAsync(uri, null, content);
        }

        public async Task Draft(ulong issuerDiscordID, ulong discordID, PlayerFranchise franchise)
        {
            Uri uri = new Uri($"{_backendUrl}/Draft/Draft");

            // Set the request content
            var body = new DraftRequest
            {
                IssuerDiscordID = issuerDiscordID,
                DiscordID = discordID,
                Franchise = franchise
            };

            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            await SendWebRequest.PostAsync(uri, null, content);
        }

        public async Task NextRound(ulong issuerDiscordID)
        {
            Uri uri = new Uri($"{_backendUrl}/Draft/NextRound");

            // Set the request content
            var body = new BaseRequest
            {
                IssuerDiscordID = issuerDiscordID
            };

            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            await SendWebRequest.PutAsync(uri, null, content);
        }
    }
}
