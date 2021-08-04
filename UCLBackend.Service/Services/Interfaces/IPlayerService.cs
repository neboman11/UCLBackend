using System.Threading.Tasks;
using UCLBackend.Service.Data.Requests;
using UCLBackend.Service.Data.Responses;

namespace UCLBackend.Service.Services.Interfaces
{
    public interface IPlayerService
    {
        Task AddPlayer(ulong issuerDiscordID, ulong discordID, string playerName, string rlTrackerLink, string[] altRLTrackerLinks);
        Task UpdateAllMMRs();
        Task UpdateSingleMMR(ulong discordID);
        Task SignPlayer(ulong issuerDiscordID, ulong discordID, string franchiseName);
        Task ReleasePlayer(ulong issuerDiscordID, ulong discordID);
        Task PlayerRankout(ulong issuerDiscordID, ulong discordID);
        PlayerInfoResponse GetPlayerInfo(ulong discordID);
    }
}
