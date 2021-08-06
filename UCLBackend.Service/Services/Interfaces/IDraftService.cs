using System.Threading.Tasks;
using UCLBackend.Service.Data.Enums;

namespace UCLBackend.Service.Services.Interfaces
{
    public interface IDraftService
    {
        Task StartDraft(ulong issuerDiscordID, PlayerLeague league);
        Task Draft(ulong issuerDiscordID, ulong discordID, PlayerFranchise franchise);
        Task NextRound(ulong issuerDiscordID);
        Task SkipPick(ulong issuerDiscordID);
    }
}
