using UCLBackend.Service.DataAccess.Models;

namespace UCLBackend.Service.DataAccess.Interfaces
{
    public interface IReplayRepository
    {
        void StoreGroupPlayerStanding(Standing standing);
        Player GetPlayerByAccount(string platform, string accountId);
    }
}