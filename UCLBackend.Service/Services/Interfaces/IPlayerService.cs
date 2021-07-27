using System.Threading.Tasks;
using UCLBackend.Service.Data.Requests;
using UCLBackend.Service.Data.Responses;

namespace UCLBackend.Service.Services.Interfaces
{
    public interface IPlayerService
    {
        Task AddPlayer(AddPlayerRequest request);
        Task UpdateAllMMRs();
        Task TempUpdateAllMMRs();
        Task SignPlayer(SignPlayerRequest request);
        Task ReleasePlayer(BaseRequest request);
        Task PlayerRankout(BaseRequest request);
        PlayerInfoResponse GetPlayerInfo(ulong discordID);
    }
}
