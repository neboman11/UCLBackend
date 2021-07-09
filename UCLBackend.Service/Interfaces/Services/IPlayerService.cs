using System.Threading.Tasks;
using UCLBackend.Service.Data.Requests;

namespace UCLBackend.Service.Interfaces.Services
{
    public interface IPlayerService
    {
        Task AddPlayer(AddPlayerRequest request);
        Task UpdateAllMMRs();
        void SignPlayer(string playerID, int teamID);
        Task ReleasePlayer(string playerID);
    }
}
