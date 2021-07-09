using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using UCLBackend.Discord.Interfaces.Services;

namespace UCLBackend.Discord
{
    public class AddPlayerModule : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger<AddPlayerModule> _logger;
        private readonly IUCLBackendService _uclBackendService;

        public AddPlayerModule(ILogger<AddPlayerModule> logger, IUCLBackendService uclBackendService)
        {
            _logger = logger;
            _uclBackendService = uclBackendService;
        }

        [Command("addplayer")]
        [Summary("Adds a player to the database")]
        public async Task AddPlayer(IUser user, string desiredName, string region, string rlTrackerLink, [Remainder] string altRLTrackerLinks)
        {
            try
            {
                if (altRLTrackerLinks == null)
                {
                    altRLTrackerLinks = "";
                }
                await _uclBackendService.AddPlayer(user.Id, desiredName, region, rlTrackerLink, altRLTrackerLinks.Split(' '));
                await Context.Channel.SendMessageAsync($"{desiredName} ({user.Username}) has been added to the database.");
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
                await _uclBackendService.AddPlayer(user.Id, desiredName, region, rlTrackerLink, null);
                await Context.Channel.SendMessageAsync($"{user.Id} {desiredName} {region} {rlTrackerLink} has been added to the database.");
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