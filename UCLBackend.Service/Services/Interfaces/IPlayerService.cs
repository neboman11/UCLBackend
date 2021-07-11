using System.Threading.Tasks;
using UCLBackend.Service.Data.Requests;

namespace UCLBackend.Service.Services.Interfaces
{
    public interface IPlayerService
    {
        Task AddPlayer(AddPlayerRequest request);
        Task UpdateAllMMRs();
        void SignPlayer(ulong discordID, string franchiseName);
        Task ReleasePlayer(ulong discordID);
        Task PlayerRankout(ulong discordID);
    }
}
