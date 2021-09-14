using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using UCLBackend.Discord.Interfaces.Services;
using UCLBackend.Service.Data.Enums;

namespace UCLBackend.Discord.Modules
{
    public class DraftModule : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger _logger;
        private readonly IDraftService _draftService;
        private readonly Dictionary<string, ulong> _roleIds;
        private static readonly Dictionary<string, PlayerFranchise> _roleToFranchiseMap = new Dictionary<string, PlayerFranchise> {
            {"ASTROS_ROLEID", PlayerFranchise.Astros},
            {"CMM_ROLEID", PlayerFranchise.CMM},
            {"BISON_ROLEID", PlayerFranchise.Bison},
            {"COBRAS_ROLEID", PlayerFranchise.Cobras},
            {"GATORS_ROLEID", PlayerFranchise.Gators},
            {"KNIGHTS_ROLEID", PlayerFranchise.Knights},
            {"LIGHTNING_ROLEID", PlayerFranchise.Lightning},
            {"RAPTORS_ROLEID", PlayerFranchise.Raptors},
            {"SAMURAI_ROLEID", PlayerFranchise.Samurai},
            {"SPARTANS_ROLEID", PlayerFranchise.Spartans},
            {"VIKINGS_ROLEID", PlayerFranchise.Vikings}
        };

        public DraftModule(ILogger logger, IDraftService draftService)
        {
            _logger = logger;
            _draftService = draftService;

            _roleIds = new Dictionary<string, ulong>();
            _roleIds.Add("OWNER_ROLEID", ulong.Parse(Environment.GetEnvironmentVariable("OWNER_ROLEID")));
            _roleIds.Add("GM_ROLEID", ulong.Parse(Environment.GetEnvironmentVariable("GM_ROLEID")));
            _roleIds.Add("AGM_ROLEID", ulong.Parse(Environment.GetEnvironmentVariable("AGM_ROLEID")));
            _roleIds.Add("FRANCHISE_OWNER_ROLEID", ulong.Parse(Environment.GetEnvironmentVariable("FRANCHISE_OWNER_ROLEID")));
            _roleIds.Add("ASTROS_ROLEID", ulong.Parse(Environment.GetEnvironmentVariable("ASTROS_ROLEID")));
            _roleIds.Add("CMM_ROLEID", ulong.Parse(Environment.GetEnvironmentVariable("CMM_ROLEID")));
            _roleIds.Add("BISON_ROLEID", ulong.Parse(Environment.GetEnvironmentVariable("BISON_ROLEID")));
            _roleIds.Add("COBRAS_ROLEID", ulong.Parse(Environment.GetEnvironmentVariable("COBRAS_ROLEID")));
            _roleIds.Add("GATORS_ROLEID", ulong.Parse(Environment.GetEnvironmentVariable("GATORS_ROLEID")));
            _roleIds.Add("KNIGHTS_ROLEID", ulong.Parse(Environment.GetEnvironmentVariable("KNIGHTS_ROLEID")));
            _roleIds.Add("LIGHTNING_ROLEID", ulong.Parse(Environment.GetEnvironmentVariable("LIGHTNING_ROLEID")));
            _roleIds.Add("RAPTORS_ROLEID", ulong.Parse(Environment.GetEnvironmentVariable("RAPTORS_ROLEID")));
            _roleIds.Add("SAMURAI_ROLEID", ulong.Parse(Environment.GetEnvironmentVariable("SAMURAI_ROLEID")));
            _roleIds.Add("SPARTANS_ROLEID", ulong.Parse(Environment.GetEnvironmentVariable("SPARTANS_ROLEID")));
            _roleIds.Add("VIKINGS_ROLEID", ulong.Parse(Environment.GetEnvironmentVariable("VIKINGS_ROLEID")));
        }

        [Command("startdraft")]
        [Summary("Starts the draft process for a season")]
        public async Task StartDraft(string league)
        {
            try
            {
                var userRoles = Context.Guild.GetUser(Context.Message.Author.Id)?.Roles;

                if (userRoles == null)
                {
                    throw new Exception($"Unable to fetch roles for {Context.Message.Author.Username}");
                }

                if (!userRoles.ToList().Any(x => x.Id == _roleIds["OWNER_ROLEID"]))
                {
                    await Context.Channel.SendMessageAsync("You do not have permission to perform this command.");
                    return;
                }

                await _draftService.StartDraft(Context.Message.Author.Id, league);
                await Context.Channel.SendMessageAsync($"Season 3 Draft has begun");
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, $"An error occurred while starting the draft.");
                await Context.Channel.SendMessageAsync(e.Message);
            }
        }

        [Command("draft")]
        [Summary("Drafts a player to a franchise")]
        public async Task Draft(ulong discordID)
        {
            try
            {
                var userRoles = Context.Guild.GetUser(Context.Message.Author.Id)?.Roles;

                if (userRoles == null)
                {
                    throw new Exception($"Unable to fetch roles for {Context.Message.Author.Username}");
                }

                if (!userRoles.ToList().Any(x => _roleIds.ContainsValue(x.Id)))
                {
                    await Context.Channel.SendMessageAsync("You do not have permission to perform this command.");
                    return;
                }

                // This method of determining the league sucks
                var notableRoles = _roleIds.Where(x => userRoles.Select(y => y.Id).Contains(x.Value)).Select(x => x.Key);
                var leagueRoles = _roleToFranchiseMap.Where(x => notableRoles.Contains(x.Key));
                await _draftService.Draft(Context.Message.Author.Id, discordID, leagueRoles.First().Value);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, $"An error occurred while drafting player {discordID}.");
                await Context.Channel.SendMessageAsync(e.Message);
            }
        }

        [Command("draft")]
        [Summary("Drafts a player to a franchise")]
        public async Task Draft(IUser player)
        {
            try
            {
                var userRoles = Context.Guild.GetUser(Context.Message.Author.Id)?.Roles;

                if (userRoles == null)
                {
                    throw new Exception($"Unable to fetch roles for {Context.Message.Author.Username}");
                }

                if (!userRoles.ToList().Any(x => _roleIds.ContainsValue(x.Id)))
                {
                    await Context.Channel.SendMessageAsync("You do not have permission to perform this command.");
                    return;
                }

                await _draftService.Draft(Context.Message.Author.Id, player.Id, _roleToFranchiseMap[_roleIds.Where(x => _roleToFranchiseMap.ContainsKey(x.Key)).First().Key]);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, $"An error occurred while drafting player {player.Id}.");
                await Context.Channel.SendMessageAsync(e.Message);
            }
        }

        [Command("nextround")]
        [Summary("Starts the next round of draft process")]
        public async Task NextRound()
        {
            try
            {
                var userRoles = Context.Guild.GetUser(Context.Message.Author.Id)?.Roles;

                if (userRoles == null)
                {
                    throw new Exception($"Unable to fetch roles for {Context.Message.Author.Username}");
                }

                if (!userRoles.ToList().Any(x => x.Id == _roleIds["OWNER_ROLEID"]))
                {
                    await Context.Channel.SendMessageAsync("You do not have permission to perform this command.");
                    return;
                }

                await _draftService.NextRound(Context.Message.Author.Id);
                await Context.Channel.SendMessageAsync($"Starting the next round of the draft");
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, $"An error occurred while starting the next round of the draft.");
                await Context.Channel.SendMessageAsync(e.Message);
            }
        }

        [Command("pickskip")]
        [Summary("Skips the pick for a franchise")]
        public async Task PickSkip()
        {
            try
            {
                var userRoles = Context.Guild.GetUser(Context.Message.Author.Id)?.Roles;

                if (userRoles == null)
                {
                    throw new Exception($"Unable to fetch roles for {Context.Message.Author.Username}");
                }

                if (!userRoles.ToList().Any(x => x.Id == _roleIds["OWNER_ROLEID"]))
                {
                    await Context.Channel.SendMessageAsync("You do not have permission to perform this command.");
                    return;
                }

                await _draftService.PickSkip(Context.Message.Author.Id);
                await Context.Channel.SendMessageAsync($"Skipping current pick");
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, $"An error occurred while skipping current pick.");
                await Context.Channel.SendMessageAsync(e.Message);
            }
        }
    }
}
