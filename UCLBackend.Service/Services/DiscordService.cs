using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using UCLBackend.Service.Data.Enums;
using UCLBackend.Service.Services.Interfaces;
using UCLBackend.Service.DataAccess.Interfaces;
using System.Net.Http;
using System;
using System.Text;
using Microsoft.Extensions.Logging;

namespace UCLBackend.Services.Services
{
    // TODO: While adding user to database, set their nickame to FA | <given username>
    // TODO: When signed to a team, replace FA with team abbr
    public class DiscordService : IDiscordService
    {
        private string _discordUrl;
        private string _discordToken;
        private ulong _discordGuildId;
        private readonly ISettingRepository _settingRepository;
        private readonly ILogger<DiscordService> _logger;
        private ulong _transactionChannelId;

        public DiscordService(IConfiguration configuration, ISettingRepository settingRepository, ILogger<DiscordService> logger)
        {
            _discordUrl = configuration.GetSection("Discord").GetValue<string>("Endpoint");
            _discordToken = configuration.GetSection("Discord").GetValue<string>("Token");
            _discordGuildId = configuration.GetSection("Discord").GetValue<ulong>("GuildId");
            _settingRepository = settingRepository;
            _logger = logger;

            _transactionChannelId = ulong.Parse(_settingRepository.GetSetting("Transaction.ChannelId"));
        }

        public async Task AddLeagueRolesToUser(ulong discordId, PlayerLeague league)
        {
            ulong discordRoleId = 0;
            switch (league)
            {
                case PlayerLeague.Origins:
                    discordRoleId = ulong.Parse(_settingRepository.GetSetting("League.Origin.RoleId"));
                    break;
                case PlayerLeague.Ultra:
                    discordRoleId = ulong.Parse(_settingRepository.GetSetting("League.Ultra.RoleId"));
                    break;
                case PlayerLeague.Elite:
                    discordRoleId = ulong.Parse(_settingRepository.GetSetting("League.Elite.RoleId"));
                    break;
                case PlayerLeague.Superior:
                    discordRoleId = ulong.Parse(_settingRepository.GetSetting("League.Superior.RoleId"));
                    break;
            }

            // Create a new HTTP client
            var client = new HttpClient();
            Uri uri = new Uri($"{_discordUrl}/guilds/{_discordGuildId}/members/{discordId}/roles/{discordRoleId}");
            client.DefaultRequestHeaders.Add("Authorization", $"Bot {_discordToken}");

            // Send the request
            var response = await client.PutAsync(uri.ToString(), null);
            await ValidateResponseCode(response);
        }

        // TODO: Remove Free Agent role from user when they sign to a team
        public async Task AddFranchiseRolesToUser(ulong discordId, PlayerFranchise franchise, PlayerLeague league)
        {
            ulong discordRoleId = 0;
            switch (franchise)
            {
                case PlayerFranchise.Astros:
                    discordRoleId = ulong.Parse(_settingRepository.GetSetting("Franchise.Astros.RoleId"));
                    break;
                case PlayerFranchise.Atlantics:
                    discordRoleId = ulong.Parse(_settingRepository.GetSetting("Franchise.Atlantics.RoleId"));
                    break;
                case PlayerFranchise.Bison:
                    discordRoleId = ulong.Parse(_settingRepository.GetSetting("Franchise.Bison.RoleId"));
                    break;
                case PlayerFranchise.Cobras:
                    discordRoleId = ulong.Parse(_settingRepository.GetSetting("Franchise.Cobras.RoleId"));
                    break;
                case PlayerFranchise.Gators:
                    discordRoleId = ulong.Parse(_settingRepository.GetSetting("Franchise.Gators.RoleId"));
                    break;
                case PlayerFranchise.Knights:
                    discordRoleId = ulong.Parse(_settingRepository.GetSetting("Franchise.Knights.RoleId"));
                    break;
                case PlayerFranchise.Lightning:
                    discordRoleId = ulong.Parse(_settingRepository.GetSetting("Franchise.Lightning.RoleId"));
                    break;
                case PlayerFranchise.Raptors:
                    discordRoleId = ulong.Parse(_settingRepository.GetSetting("Franchise.Raptors.RoleId"));
                    break;
                case PlayerFranchise.Samurai:
                    discordRoleId = ulong.Parse(_settingRepository.GetSetting("Franchise.Samurai.RoleId"));
                    break;
                case PlayerFranchise.Spartans:
                    discordRoleId = ulong.Parse(_settingRepository.GetSetting("Franchise.Spartans.RoleId"));
                    break;
                case PlayerFranchise.XII_Boost:
                    discordRoleId = ulong.Parse(_settingRepository.GetSetting("Franchise.XII_Boost.RoleId"));
                    break;
                case PlayerFranchise.Vikings:
                    discordRoleId = ulong.Parse(_settingRepository.GetSetting("Franchise.Vikings.RoleId"));
                    break;
                case PlayerFranchise.Free_Agents:
                    switch (league)
                    {
                        case PlayerLeague.Origins:
                            discordRoleId = ulong.Parse(_settingRepository.GetSetting("Franchise.Free_Agent.Origins.RoleId"));
                            break;
                        case PlayerLeague.Ultra:
                            discordRoleId = ulong.Parse(_settingRepository.GetSetting("Franchise.Free_Agent.Ultra.RoleId"));
                            break;
                        case PlayerLeague.Elite:
                            discordRoleId = ulong.Parse(_settingRepository.GetSetting("Franchise.Free_Agent.Elite.RoleId"));
                            break;
                        case PlayerLeague.Superior:
                            discordRoleId = ulong.Parse(_settingRepository.GetSetting("Franchise.Free_Agent.Superior.RoleId"));
                            break;
                    }
                    break;
            }

            // Create a new HTTP client
            var client = new HttpClient();
            Uri uri = new Uri($"{_discordUrl}/guilds/{_discordGuildId}/members/{discordId}/roles/{discordRoleId}");
            client.DefaultRequestHeaders.Add("Authorization", $"Bot {_discordToken}");

            // Send the request
            var response = await client.PutAsync(uri.ToString(), null);
            await ValidateResponseCode(response);
        }

        public async Task SetFreeAgentNickname(ulong discordId, string nickname)
        {
            var discordNickname = $"FA | {nickname}";

            await ChangeUserNickname(discordId, discordNickname);
        }

        public async Task SetFranchiseNickname(ulong discordId, PlayerFranchise franchise, string nickname)
        {
            var discordNickname = $"{franchise.ToString().ToUpper().Substring(0, 3)} | {nickname}";

            await ChangeUserNickname(discordId, discordNickname);
        }

        public async Task RemoveFranchiseRoles(ulong discordId, PlayerFranchise franchise, PlayerLeague league)
        {
            ulong discordRoleId = 0;
            switch (franchise)
            {
                case PlayerFranchise.Astros:
                    discordRoleId = ulong.Parse(_settingRepository.GetSetting("Franchise.Astros.RoleId"));
                    break;
                case PlayerFranchise.Atlantics:
                    discordRoleId = ulong.Parse(_settingRepository.GetSetting("Franchise.Atlantics.RoleId"));
                    break;
                case PlayerFranchise.Bison:
                    discordRoleId = ulong.Parse(_settingRepository.GetSetting("Franchise.Bison.RoleId"));
                    break;
                case PlayerFranchise.Cobras:
                    discordRoleId = ulong.Parse(_settingRepository.GetSetting("Franchise.Cobras.RoleId"));
                    break;
                case PlayerFranchise.Gators:
                    discordRoleId = ulong.Parse(_settingRepository.GetSetting("Franchise.Gators.RoleId"));
                    break;
                case PlayerFranchise.Knights:
                    discordRoleId = ulong.Parse(_settingRepository.GetSetting("Franchise.Knights.RoleId"));
                    break;
                case PlayerFranchise.Lightning:
                    discordRoleId = ulong.Parse(_settingRepository.GetSetting("Franchise.Lightning.RoleId"));
                    break;
                case PlayerFranchise.Raptors:
                    discordRoleId = ulong.Parse(_settingRepository.GetSetting("Franchise.Raptors.RoleId"));
                    break;
                case PlayerFranchise.Samurai:
                    discordRoleId = ulong.Parse(_settingRepository.GetSetting("Franchise.Samurai.RoleId"));
                    break;
                case PlayerFranchise.Spartans:
                    discordRoleId = ulong.Parse(_settingRepository.GetSetting("Franchise.Spartans.RoleId"));
                    break;
                case PlayerFranchise.XII_Boost:
                    discordRoleId = ulong.Parse(_settingRepository.GetSetting("Franchise.XII_Boost.RoleId"));
                    break;
                case PlayerFranchise.Vikings:
                    discordRoleId = ulong.Parse(_settingRepository.GetSetting("Franchise.Vikings.RoleId"));
                    break;
                case PlayerFranchise.Free_Agents:
                    switch (league)
                    {
                        case PlayerLeague.Origins:
                            discordRoleId = ulong.Parse(_settingRepository.GetSetting("Franchise.Free_Agent.Origins.RoleId"));
                            break;
                        case PlayerLeague.Ultra:
                            discordRoleId = ulong.Parse(_settingRepository.GetSetting("Franchise.Free_Agent.Ultra.RoleId"));
                            break;
                        case PlayerLeague.Elite:
                            discordRoleId = ulong.Parse(_settingRepository.GetSetting("Franchise.Free_Agent.Elite.RoleId"));
                            break;
                        case PlayerLeague.Superior:
                            discordRoleId = ulong.Parse(_settingRepository.GetSetting("Franchise.Free_Agent.Superior.RoleId"));
                            break;
                    }
                    break;
            }

            // Create a new HTTP client
            var client = new HttpClient();
            Uri uri = new Uri($"{_discordUrl}/guilds/{_discordGuildId}/members/{discordId}/roles/{discordRoleId}");
            client.DefaultRequestHeaders.Add("Authorization", $"Bot {_discordToken}");

            // Send the request
            var response = await client.DeleteAsync(uri.ToString());
            await ValidateResponseCode(response);
        }

        public async Task RemoveLeagueRoles(ulong discordId, PlayerLeague league)
        {
            ulong discordRoleId = 0;
            switch (league)
            {
                case PlayerLeague.Origins:
                    discordRoleId = ulong.Parse(_settingRepository.GetSetting("League.Origin.RoleId"));
                    break;
                case PlayerLeague.Ultra:
                    discordRoleId = ulong.Parse(_settingRepository.GetSetting("League.Ultra.RoleId"));
                    break;
                case PlayerLeague.Elite:
                    discordRoleId = ulong.Parse(_settingRepository.GetSetting("League.Elite.RoleId"));
                    break;
                case PlayerLeague.Superior:
                    discordRoleId = ulong.Parse(_settingRepository.GetSetting("League.Superior.RoleId"));
                    break;
            }

            // Create a new HTTP client
            var client = new HttpClient();
            Uri uri = new Uri($"{_discordUrl}/guilds/{_discordGuildId}/members/{discordId}/roles/{discordRoleId}");
            client.DefaultRequestHeaders.Add("Authorization", $"Bot {_discordToken}");

            // Send the request
            var response = await client.DeleteAsync(uri.ToString());
            await ValidateResponseCode(response);
        }

        public async Task LogTransaction(ulong issuerDiscordID, string message)
        {
            // Create a new HTTP client
            var client = new HttpClient();
            Uri uri = new Uri($"{_discordUrl}/channels/{_transactionChannelId}/messages");
            client.DefaultRequestHeaders.Add("Authorization", $"Bot {_discordToken}");

            var content = new StringContent($"{{\"content\":\"{issuerDiscordID} performed: {message}\"}}");

            // Send the request
            var response = await client.PostAsync(uri.ToString(), content);
            await ValidateResponseCode(response);
        }

        #region Private Methods
        private async Task ChangeUserNickname(ulong discordId, string nickname)
        {
            // Create a new HTTP client
            var client = new HttpClient();
            Uri uri = new Uri($"{_discordUrl}/guilds/{_discordGuildId}/members/{discordId}");
            client.DefaultRequestHeaders.Add("Authorization", $"Bot {_discordToken}");

            // Send the request
            var response = await client.PatchAsync(uri.ToString(), new StringContent($"{{\"nick\":\"{nickname}\"}}", Encoding.UTF8, "application/json"));
            await ValidateResponseCode(response);
        }

        private async Task ValidateResponseCode(HttpResponseMessage response)
        {
            if (((int)response.StatusCode) >= 400)
            {
                var responseMessage = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"{response.StatusCode} {response.ReasonPhrase}: {responseMessage}");
            }
        }
        #endregion
    }
}
