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
        private readonly IPlayerService _playerService;
        private readonly Dictionary<string, ulong> _roleIds;
        private readonly List<string> _directorRoles = new List<string> {"DIRECTORS_ROLEID", "MARKETING_DIRECTOR_ROLEID", "ADMISSIONS_DIRECTOR_ROLEID", "STATS_DIRECTOR_ROLEID"};
        private readonly List<string> _leagueOperatorRoles = new List<string> {"LEAGUE_OPERATOR_ROLEID", "ORIGINS_LEAGUE_OPERATOR_ROLEID", "ULTRA_LEAGUE_OPERATOR_ROLEID", "ELITE_LEAGUE_OPERATOR_ROLEID", "SUPERIOR_LEAGUE_OPERATOR_ROLEID"};

        public PlayerModule(ILogger logger, IPlayerService playerService)
        {
            _logger = logger;
            _playerService = playerService;

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

                await _playerService.AddPlayer(user.Id, desiredName, region, rlTrackerLink, altRLTrackerLinks.Split(' '));
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
        public async Task AddPlayer(ulong user, string desiredName, string region, string rlTrackerLink, [Remainder] string altRLTrackerLinks)
        {
            try
            {
                var userRoles = Context.Guild.GetUser(user).Roles;

                if (!userRoles.ToList().Any(x => _roleIds.ContainsValue(x.Id)))
                {
                    await Context.Channel.SendMessageAsync("You do not have permission to perform this command.");
                    return;
                }

                await _playerService.AddPlayer(user, desiredName, region, rlTrackerLink, altRLTrackerLinks.Split(' '));
                await Context.Channel.SendMessageAsync($"{desiredName} ({Context.Guild.GetUser(user)}) has been registered as a player.");
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, $"An error occurred while adding player {user}/{desiredName}.");
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

                await _playerService.AddPlayer(user.Id, desiredName, region, rlTrackerLink, null);
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
        public async Task AddPlayer(ulong user, string desiredName, string region, string rlTrackerLink)
        {
            try
            {
                var userRoles = Context.Guild.GetUser(user).Roles;

                if (!userRoles.ToList().Any(x => _roleIds.ContainsValue(x.Id)))
                {
                    await Context.Channel.SendMessageAsync("You do not have permission to perform this command.");
                    return;
                }

                await _playerService.AddPlayer(user, desiredName, region, rlTrackerLink, null);
                await Context.Channel.SendMessageAsync($"{desiredName} ({Context.Guild.GetUser(user)}) has been registered as a player.");
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, $"An error occurred while adding player {user}/{desiredName}.");
                await Context.Channel.SendMessageAsync("Something went wrong, please try again.");
            }
        }

        [Command("dj")]
        [Summary("You know")]
        public async Task DJ()
        {
            await Context.Channel.SendMessageAsync($"is better than LightBrightD");
        }

        [Command("sign")]
        [Summary("Signs a player to a franchise")]
        public async Task Sign(IUser user, string franchiseName)
        {
            try
            {
                var userRoles = Context.Guild.GetUser(user.Id).Roles;
                // Allowed roles are Owner, Directors, and League Operators
                var allowedRoles = _roleIds.Where(x => _directorRoles.Contains(x.Key) || _leagueOperatorRoles.Contains(x.Key) || x.Key == "OWNER_ROLEID").Select(x => x.Value).ToList();
                if (!userRoles.ToList().Any(x => allowedRoles.Contains(x.Id)))
                {
                    await Context.Channel.SendMessageAsync("You do not have permission to perform this command.");
                    return;
                }

                await _playerService.SignPlayer(user.Id, franchiseName);
                await Context.Channel.SendMessageAsync($"{user.Username} has been signed to {franchiseName}");
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, $"An error occurred while signing player {user.Username} to {franchiseName}.");
                await Context.Channel.SendMessageAsync("Something went wrong, please try again.");
            }
        }

        [Command("sign")]
        [Summary("Signs a player to a franchise")]
        public async Task Sign(IUser user, IRole franchiseRole)
        {
            try
            {
                var userRoles = Context.Guild.GetUser(user.Id).Roles;
                // Allowed roles are Owner, Directors, and League Operators
                var allowedRoles = _roleIds.Where(x => _directorRoles.Contains(x.Key) || _leagueOperatorRoles.Contains(x.Key) || x.Key == "OWNER_ROLEID").Select(x => x.Value).ToList();
                if (!userRoles.ToList().Any(x => allowedRoles.Contains(x.Id)))
                {
                    await Context.Channel.SendMessageAsync("You do not have permission to perform this command.");
                    return;
                }

                await _playerService.SignPlayer(user.Id, franchiseRole.Name);
                await Context.Channel.SendMessageAsync($"{user.Username} has been signed to {franchiseRole.Name}");
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, $"An error occurred while signing player {user.Username} to {franchiseRole.Name}.");
                await Context.Channel.SendMessageAsync("Something went wrong, please try again.");
            }
        }

        [Command("release")]
        [Summary("Releases a player from a franchise")]
        public async Task Release(IUser user)
        {
            try
            {
                var userRoles = Context.Guild.GetUser(user.Id).Roles;
                // Allowed roles are Owner, Directors, and League Operators
                var allowedRoles = _roleIds.Where(x => _directorRoles.Contains(x.Key) || _leagueOperatorRoles.Contains(x.Key) || x.Key == "OWNER_ROLEID").Select(x => x.Value).ToList();
                if (!userRoles.ToList().Any(x => allowedRoles.Contains(x.Id)))
                {
                    await Context.Channel.SendMessageAsync("You do not have permission to perform this command.");
                    return;
                }

                await _playerService.ReleasePlayer(user.Id);
                await Context.Channel.SendMessageAsync($"{user.Username} has been released");
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, $"An error occurred while releasing player {user.Username}.");
                await Context.Channel.SendMessageAsync("Something went wrong, please try again.");
            }
        }

        [Command("info")]
        [Summary("Gets information about a player")]
        public async Task GetPlayerInfo()
        {
            try
            {
                var player = Context.Message.Author.Id;
                // var info = await _playerService.GetPlayerInfo(player);
                // await Context.Channel.SendMessageAsync($"{info.Name} is a {info.Position} playing for {info.Franchise}");
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, $"An error occurred while getting player info for {Context.Message.Author.Username}.");
                await Context.Channel.SendMessageAsync("Something went wrong, please try again.");
            }
        }

        [Command("info")]
        [Summary("Gets information about a player")]
        public async Task GetPlayerInfo(IUser user)
        {
            try
            {
                var player = user.Id;
                // var info = await _playerService.GetPlayerInfo(player);
                // await Context.Channel.SendMessageAsync($"{info.Name} is a {info.Position} playing for {info.Franchise}");
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, $"An error occurred while getting player info for {Context.Message.Author.Username}.");
                await Context.Channel.SendMessageAsync("Something went wrong, please try again.");
            }
        }
    }
}