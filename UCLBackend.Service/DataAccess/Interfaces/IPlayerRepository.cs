using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UCLBackend.Service.DataAccess.Models;
using UCLBackend.Service.Data.Enums;

namespace UCLBackend.Service.DataAccess.Interfaces
{
    public interface IPlayerRepository
    {
        void AddPlayer(Player player);
        void AddAccount(Account account);
        Task<string> RemoteGetPlayerID(string platform, string username);
        Task<(List<(int, DateTime)>, List<(int, DateTime)>)> RemoteGetPlayerMMRs(string playerID);
        List<Player> GetAllPlayers();
        Player GetPlayer(string playerID);
        void UpdatePlayer(Player player);
        Team GetTeam(string teamName, PlayerLeague league);
        Player GetPlayerUsingDiscordID(ulong discordID);
    }
}