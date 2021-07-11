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
                case PlayerLeague.Origin:
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
            var temp = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
        }
    }
}
