using System.Threading.Tasks;
using UCLBackend.Service.Data.Enums;

namespace UCLBackend.Service.Services.Interfaces
{
    public interface IDiscordService
    {
        Task AddLeagueRolesToUser(ulong discordId, PlayerLeague league);
        Task AddFranchiseRolesToUser(ulong discordId, PlayerFranchise franchise, PlayerLeague league);
        Task SetFreeAgentNickname(ulong discordId, string nickname);
        Task SetFranchiseNickname(ulong discordId, PlayerFranchise franchise, string nickname);
        Task RemoveFranchiseRoles(ulong discordId, PlayerFranchise franchise, PlayerLeague league);
        Task RemoveLeagueRoles(ulong discordId, PlayerLeague league);
        Task LogTransaction(ulong issuerDiscordID, string message);
        Task SendMessage(ulong channelId, string message);
    }
}
