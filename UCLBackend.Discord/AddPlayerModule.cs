using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using UCLBackend.Discord.Interfaces.Services;

namespace UCLBackend.Discord
{
    public class AddPlayerModule : ModuleBase<SocketCommandContext>
    {
        private readonly IUCLBackendService _uclBackendService;

        public AddPlayerModule(IUCLBackendService uclBackendService)
        {
            _uclBackendService = uclBackendService;
        }

        [Command("addplayer")]
        [Summary("Adds a player to the database")]
        public async Task AddPlayer(IUser user, string desiredName, string region, string rlTrackerLink, [Remainder] string altRLTrackerLinks)
        {
            if (altRLTrackerLinks == null)
            {
                altRLTrackerLinks = "";
            }
            await _uclBackendService.AddPlayer(user.Id, desiredName, region, rlTrackerLink, altRLTrackerLinks.Split(' '));
            await Context.Channel.SendMessageAsync($"{user.Id} {desiredName} {region} {rlTrackerLink} {string.Join(", ", altRLTrackerLinks.Split(" "))} has been added to the database.");
        }

        [Command("addplayer")]
        [Summary("Adds a player to the database")]
        public async Task AddPlayer(IUser user, string desiredName, string region, string rlTrackerLink)
        {
            string altRLTrackerLinks = "";
            
            await _uclBackendService.AddPlayer(user.Id, desiredName, region, rlTrackerLink, altRLTrackerLinks.Split(' '));
            await Context.Channel.SendMessageAsync($"{user.Id} {desiredName} {region} {rlTrackerLink} {string.Join(", ", altRLTrackerLinks.Split(" "))} has been added to the database.");
        }

        [Command("dj")]
        [Summary("You know")]
        public async Task DJ()
        {
            await Context.Channel.SendMessageAsync($"is better than LightBrightD");
        }
    }
}