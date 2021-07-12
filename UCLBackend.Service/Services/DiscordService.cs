using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using UCLBackend.Service.Data.Enums;
using UCLBackend.Service.Services.Interfaces;
using UCLBackend.Service.DataAccess.Interfaces;
using System.Net.Http;
using System;

namespace UCLBackend.Services.Services
{
    public class DiscordService : IDiscordService
    {
        private string _discordUrl;
        private string _discordToken;
        private ulong _discordGuildId;
        private readonly ISettingRepository _settingRepository;

        public DiscordService(IConfiguration configuration, ISettingRepository settingRepository)
        {
            _discordUrl = configuration.GetSection("Discord").GetValue<string>("Endpoint");
            _discordToken = configuration.GetSection("Discord").GetValue<string>("Token");
            _discordGuildId = configuration.GetSection("Discord").GetValue<ulong>("GuildId");
            _settingRepository = settingRepository;
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
            response.EnsureSuccessStatusCode();
        }

        public async Task AddFranchiseRolesToUser(ulong discordId, PlayerFranchise franchise)
        {
            ulong discordRoleId = 860638221892976653;
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
                    discordRoleId = ulong.Parse(_settingRepository.GetSetting("Franchise.Free_Agent.RoleId"));
                    break;
            }

            // Create a new HTTP client
            var client = new HttpClient();
            Uri uri = new Uri($"{_discordUrl}/guilds/{_discordGuildId}/members/{discordId}/roles/{discordRoleId}");
            client.DefaultRequestHeaders.Add("Authorization", $"Bot {_discordToken}");

            // Send the request
            var response = await client.PutAsync(uri.ToString(), null);
            response.EnsureSuccessStatusCode();
        }
    }
}
