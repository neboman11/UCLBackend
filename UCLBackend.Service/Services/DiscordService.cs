using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using UCLBackend.Service.Data.Enums;
using UCLBackend.Service.Services.Interfaces;
using UCLBackend.Service.DataAccess.Interfaces;
using System;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using UCLBackend.Service.Data.Helpers;
using UCLBackend.Service.Data.Discord;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace UCLBackend.Services.Services
{
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

        #region League Roles
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

            Uri uri = new Uri($"{_discordUrl}/guilds/{_discordGuildId}/members/{discordId}/roles/{discordRoleId}");

            await SendWebRequest.PutAsync(uri, $"Bot {_discordToken}", null);
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

            Uri uri = new Uri($"{_discordUrl}/guilds/{_discordGuildId}/members/{discordId}/roles/{discordRoleId}");

            await SendWebRequest.DeleteAsync(uri, $"Bot {_discordToken}");
        }
        #endregion

        #region Franchise Roles
        public async Task AddFranchiseRolesToUser(ulong discordId, PlayerFranchise franchise, PlayerLeague league)
        {
            ulong discordRoleId = 0;
            switch (franchise)
            {
                case PlayerFranchise.Astros:
                    discordRoleId = ulong.Parse(_settingRepository.GetSetting("Franchise.Astros.RoleId"));
                    break;
                case PlayerFranchise.CMM:
                    discordRoleId = ulong.Parse(_settingRepository.GetSetting("Franchise.CMM.RoleId"));
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

            Uri uri = new Uri($"{_discordUrl}/guilds/{_discordGuildId}/members/{discordId}/roles/{discordRoleId}");

            await SendWebRequest.PutAsync(uri, $"Bot {_discordToken}", null);
        }

        public async Task RemoveFranchiseRoles(ulong discordId, PlayerFranchise franchise, PlayerLeague league)
        {
            ulong discordRoleId = 0;
            switch (franchise)
            {
                case PlayerFranchise.Astros:
                    discordRoleId = ulong.Parse(_settingRepository.GetSetting("Franchise.Astros.RoleId"));
                    break;
                case PlayerFranchise.CMM:
                    discordRoleId = ulong.Parse(_settingRepository.GetSetting("Franchise.CMM.RoleId"));
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

            Uri uri = new Uri($"{_discordUrl}/guilds/{_discordGuildId}/members/{discordId}/roles/{discordRoleId}");

            await SendWebRequest.DeleteAsync(uri, $"Bot {_discordToken}");
        }
        #endregion

        #region Nickname
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
        #endregion

        // A value of 1 for issuerDiscordID means the bot is operating autonomously
        public async Task LogTransaction(ulong issuerDiscordID, string message)
        {
            string discordMessage = $"{issuerDiscordID} performed: {message}";
            // Special case when bot operates automatically
            if (issuerDiscordID == 1)
            {
                discordMessage = $"Bot performed: {message}";
            }

            await SendMessage(_transactionChannelId, discordMessage);
        }

        public async Task SendMessage(ulong channelId, string message)
        {
            Uri uri = new Uri($"{_discordUrl}/channels/{channelId}/messages");

            var messageObj = new Message()
            {
                Content = message
            };

            var content = new StringContent(JsonConvert.SerializeObject(messageObj), Encoding.UTF8, "application/json");

            await SendWebRequest.PostAsync(uri, $"Bot {_discordToken}", content);
        }

        public async Task<Message> SendMessage(ulong channelId, Message message)
        {
            Uri uri = new Uri($"{_discordUrl}/channels/{channelId}/messages");

            var content = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json");

            return await SendWebRequest.PostAsync<Message>(uri, $"Bot {_discordToken}", content);
        }

        public async Task<Message> SendEmbed(ulong channelId, Embed embed)
        {
            Uri uri = new Uri($"{_discordUrl}/channels/{channelId}/messages");

            var message = new Message()
            {
                Embeds = new List<Embed>() { embed }
            };

            return await SendMessage(channelId, message);
        }

        public async Task DeleteMessage(ulong channelId, ulong messageId)
        {
            Uri uri = new Uri($"{_discordUrl}/channels/{channelId}/messages/{messageId}");

            await SendWebRequest.DeleteAsync(uri, $"Bot {_discordToken}");
        }

        #region Private Methods
        private async Task ChangeUserNickname(ulong discordId, string nickname)
        {
            Uri uri = new Uri($"{_discordUrl}/guilds/{_discordGuildId}/members/{discordId}");

            await SendWebRequest.PatchAsync(uri, $"Bot {_discordToken}", new StringContent($"{{\"nick\":\"{nickname}\"}}", Encoding.UTF8, "application/json"));
        }
        #endregion
    }
}
