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
        public async Task AddPlayer(IUser user, string desiredName, string platform, string region, string rlTrackerLink, [Remainder] string altRLTrackerLinks)
        {
            await _uclBackendService.AddPlayer(user.Id, desiredName, platform, region, rlTrackerLink, altRLTrackerLinks.Split(' '));
            await Context.Channel.SendMessageAsync($"{user.Id} {desiredName} {platform} {region} {rlTrackerLink} {string.Join(", ", altRLTrackerLinks.Split(" "))} has been added to the database.");
        }

        [Command("dj")]
        [Summary("You know")]
        public async Task DJ()
        {
            await Context.Channel.SendMessageAsync($"is better than LightBrightD");
        }
    }
}