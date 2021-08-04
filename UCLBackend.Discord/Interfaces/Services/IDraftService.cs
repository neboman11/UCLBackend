using System.Threading.Tasks;
using UCLBackend.Service.Data.Enums;
using UCLBackend.Service.Data.Responses;

namespace UCLBackend.Discord.Interfaces.Services
{
    public interface IDraftService
    {
        Task StartDraft(ulong issuerDiscordID);
        Task Draft(ulong issuerDiscordID, ulong discordID, PlayerFranchise franchise);
        Task NextRound(ulong issuerDiscordID);
    }
}
