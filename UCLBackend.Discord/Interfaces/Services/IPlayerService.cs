using System.Threading.Tasks;
using UCLBackend.Service.Data.Responses;

namespace UCLBackend.Discord.Interfaces.Services
{
    public interface IPlayerService
    {
        Task AddPlayer(ulong issuerDiscordID, ulong discordID, string playername, string rlTrackerLink, string[] altRLTrackerLinks);
        Task SignPlayer(ulong issuerDiscordID, ulong discordID, string franchiseName);
        Task ReleasePlayer(ulong issuerDiscordID, ulong discordID);
        Task<PlayerInfoResponse> GetPlayerInfo(ulong discordID);
    }
}