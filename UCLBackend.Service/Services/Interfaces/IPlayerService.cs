using System.Threading.Tasks;
using UCLBackend.Service.Data.Requests;
using UCLBackend.Service.Data.Responses;

namespace UCLBackend.Service.Services.Interfaces
{
    public interface IPlayerService
    {
        Task AddPlayer(AddPlayerRequest request);
        Task UpdateAllMMRs();
        void SignPlayer(ulong discordID, string franchiseName);
        Task ReleasePlayer(ulong discordID);
        Task PlayerRankout(ulong discordID);
        PlayerInfoResponse GetPlayerInfo(ulong discordID);
    }
}
