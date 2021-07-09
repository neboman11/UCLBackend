using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UCLBackend.DataAccess.Models;

namespace UCLBackend.Service.DataAccess.Interfaces
{
    public interface IPlayerRepository
    {
        void AddPlayer(Player player);
        void AddAccount(Account account);
        Task<string> RemoteGetPlayerID(string platform, string username);
        Task<List<(int, DateTime)>> RemoteGetPlayerMMRs(string playerID);
        void UpdatePlayerPeakMMR(string playerID, int peakMMR);
        void UpdatePlayerCurrentMMR(string playerID, int mmr);
        void UpdatePlayerSalary(string playerID, double salary);
        List<Player> GetAllPlayers();
        Player GetPlayer(string playerID);
        void UpdatePlayer(Player player);
    }
}