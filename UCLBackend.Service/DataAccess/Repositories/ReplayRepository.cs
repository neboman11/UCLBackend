using System.Linq;
using UCLBackend.Service.DataAccess.Models;
using UCLBackend.Service.DataAccess.Interfaces;
using UCLBackend.Service.Data.DataModels;

namespace UCLBBackend.DataAccess.Repositories
{
    public class ReplayRepository : IReplayRepository
    {
        private readonly UCLContext _context;

        public ReplayRepository(UCLContext context)
        {
            _context = context;
        }

        public void StoreGroupPlayerStanding(Standing standing)
        {
            _context.Standings.Add(standing);
        }

        public Player GetPlayerByAccount(string platform, string accountName)
        {
            return _context.Accounts.FirstOrDefault(p => p.Platform == platform && p.AccountName == accountName).Player;
        }
    }
}
