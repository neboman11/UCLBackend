using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using UCLBackend.Discord.Interfaces.Services;

namespace UCLBackend.Discord.Modules
{
    public class PlayerModule : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger _logger;
        private readonly IUCLBackendService _uclBackendService;
        private readonly Dictionary<string, ulong> _roleIds;

        public PlayerModule(ILogger logger, IUCLBackendService uclBackendService)
        {
            _logger = logger;
            _uclBackendService = uclBackendService;

            _roleIds = new Dictionary<string, ulong>();
            _roleIds.Add("ADMISSIONS_AGENT_ROLEID", ulong.Parse(Environment.GetEnvironmentVariable("ADMISSIONS_AGENT_ROLEID")));
            _roleIds.Add("LEAGUE_OPERATOR_ROLEID", ulong.Parse(Environment.GetEnvironmentVariable("LEAGUE_OPERATOR_ROLEID")));
            _roleIds.Add("SUPERIOR_LEAGUE_OPERATOR_ROLEID", ulong.Parse(Environment.GetEnvironmentVariable("SUPERIOR_LEAGUE_OPERATOR_ROLEID")));
            _roleIds.Add("ELITE_LEAGUE_OPERATOR_ROLEID", ulong.Parse(Environment.GetEnvironmentVariable("ELITE_LEAGUE_OPERATOR_ROLEID")));
            _roleIds.Add("ULTRA_LEAGUE_OPERATOR_ROLEID", ulong.Parse(Environment.GetEnvironmentVariable("ULTRA_LEAGUE_OPERATOR_ROLEID")));
            _roleIds.Add("ORIGINS_LEAGUE_OPERATOR_ROLEID", ulong.Parse(Environment.GetEnvironmentVariable("ORIGINS_LEAGUE_OPERATOR_ROLEID")));
            _roleIds.Add("DIRECTORS_ROLEID", ulong.Parse(Environment.GetEnvironmentVariable("DIRECTORS_ROLEID")));
            _roleIds.Add("MARKETING_DIRECTOR_ROLEID", ulong.Parse(Environment.GetEnvironmentVariable("MARKETING_DIRECTOR_ROLEID")));
            _roleIds.Add("ADMISSIONS_DIRECTOR_ROLEID", ulong.Parse(Environment.GetEnvironmentVariable("ADMISSIONS_DIRECTOR_ROLEID")));
            _roleIds.Add("STATS_DIRECTOR_ROLEID", ulong.Parse(Environment.GetEnvironmentVariable("STATS_DIRECTOR_ROLEID")));
            _roleIds.Add("OWNER_ROLEID", ulong.Parse(Environment.GetEnvironmentVariable("OWNER_ROLEID")));
        }

        [Command("addplayer")]
        [Summary("Adds a player to the database")]
        public async Task AddPlayer(IUser user, string desiredName, string region, string rlTrackerLink, [Remainder] string altRLTrackerLinks)
        {
            try
            {
                var userRoles = Context.Guild.GetUser(user.Id).Roles;

                if (!userRoles.ToList().Any(x => _roleIds.ContainsValue(x.Id)))
                {
                    await Context.Channel.SendMessageAsync("You do not have permission to perform this command.");
                    return;
                }

                await _uclBackendService.AddPlayer(user.Id, desiredName, region, rlTrackerLink, altRLTrackerLinks.Split(' '));
                await Context.Channel.SendMessageAsync($"{desiredName} ({user.Username}) has been registered as a player.");
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, $"An error occurred while adding player {user.Username}/{desiredName}.");
                await Context.Channel.SendMessageAsync("Something went wrong, please try again.");
            }
        }

        [Command("addplayer")]
        [Summary("Adds a player to the database")]
        public async Task AddPlayer(IUser user, string desiredName, string region, string rlTrackerLink)
        {
            try
            {
                var userRoles = Context.Guild.GetUser(user.Id).Roles;

                if (!userRoles.ToList().Any(x => _roleIds.ContainsValue(x.Id)))
                {
                    await Context.Channel.SendMessageAsync("You do not have permission to perform this command.");
                    return;
                }
                
                await _uclBackendService.AddPlayer(user.Id, desiredName, region, rlTrackerLink, null);
                await Context.Channel.SendMessageAsync($"{desiredName} ({user.Username}) has been registered as a player.");
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, $"An error occurred while adding player {user.Username}/{desiredName}.");
                await Context.Channel.SendMessageAsync("Something went wrong, please try again.");
            }
        }

        [Command("dj")]
        [Summary("You know")]
        public async Task DJ()
        {
            await Context.Channel.SendMessageAsync($"is better than LightBrightD");
        }
    }
}