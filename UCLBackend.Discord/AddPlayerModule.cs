using System.Threading.Tasks;
using Discord.Commands;

namespace UCLBackend.Discord
{
    public class AddPlayerModule : ModuleBase<SocketCommandContext>
    {
        [Command("addplayer")]
        [Summary("Adds a player to the database")]
        public async Task AddPlayer([Remainder] string name)
        {
            await Context.Channel.SendMessageAsync($"{name} has been added to the database.");
        }
    }
}