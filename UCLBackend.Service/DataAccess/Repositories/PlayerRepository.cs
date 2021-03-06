using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using UCLBackend.Service.DataAccess.Models;
using UCLBackend.DataAccess.Models.Responses;
using UCLBackend.Service.Data.Enums;
using UCLBackend.Service.DataAccess.Interfaces;
using System.Net;
using System.Threading;
using UCLBackend.Service.Data.Helpers;
using UCLBackend.Service.Data.DataModels;

namespace UCLBackend.Service.DataAccess.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly UCLContext _context;

        public PlayerRepository(UCLContext context)
        {
            _context = context;
        }

        public void AddPlayer(Player player)
        {
            _context.Players.Add(player);
            _context.SaveChanges();
        }

        public void AddAccount(Account account)
        {
            _context.Accounts.Add(account);
            _context.SaveChanges();
        }

        public async Task<string> RemoteGetPlayerID(string platform, string username)
        {
            Uri uri = new Uri($"https://api.tracker.gg/api/v2/rocket-league/standard/profile/{platform}/{username}");

            var player = await SendWebRequest.GetAsync<GetPlayerIDResponse>(uri, null);

            return player.Data.MetaData.PlayerID;
        }

        public async Task<(List<(int, DateTime)>, List<(int, DateTime)>)> RemoteGetPlayerMMRs(string playerID)
        {
            Uri uri = new Uri($"https://api.tracker.gg/api/v1/rocket-league/player-history/mmr/{playerID}");

            var player = await SendWebRequest.GetAsync<GetPlayerMMRsResponse>(uri, null);

            var doublesMMRs = new List<(int, DateTime)>();
            doublesMMRs.Add((0, DateTime.MinValue));
            if (player.Data.ContainsKey(11))
            {
                doublesMMRs = player.Data[11].ToList().Select(x => (x.Rating, x.CollectDate)).ToList();
            }

            var triplesMMRs = new List<(int, DateTime)>();
            triplesMMRs.Add((0, DateTime.MinValue));
            if (player.Data.ContainsKey(13))
            {
                triplesMMRs = player.Data[13].ToList().Select(x => (x.Rating, x.CollectDate)).ToList();
            }

            return (doublesMMRs, triplesMMRs);
        }

        public List<Player> GetAllPlayers()
        {
            return _context.Players.ToList();
        }

        public List<Player> GetAllPlayersWithAccounts()
        {
            return _context.Players.Include(p => p.Accounts).ToList();
        }

        public async Task<List<Player>> GetAllPlayersWithTeams()
        {
            return await _context.Players.Include(p => p.Team).ToListAsync();
        }

        public Player GetPlayer(int playerID)
        {
            return _context.Players.Include(p => p.Accounts).FirstOrDefault(x => x.PlayerID == playerID);
        }

        public void UpdatePlayer(Player player)
        {
            _context.Players.Update(player);
            _context.SaveChanges();
        }

        public Team GetTeam(string teamName, PlayerLeague league)
        {
            return _context.Roster.FirstOrDefault(x => x.TeamName == teamName && x.League == league.ToString());
        }

        public Player GetPlayerUsingDiscordID(ulong discordID)
        {
            return _context.Players.Include(p => p.Accounts).FirstOrDefault(x => x.DiscordID == discordID);
        }

        public Player GetPlayerUsingDiscordIDWithTeam(ulong discordID)
        {
            return _context.Players.Include(p => p.Accounts).Include(p => p.Team).FirstOrDefault(x => x.DiscordID == discordID);
        }

        public List<Player> GetFreeAgents()
        {
            return _context.Players.Where(x => x.IsFreeAgent.Value).ToList();
        }

        public List<Team> TeamsByLeague(string league)
        {
            return _context.Roster.Include(x => x.Players).Where(x => x.League == league).ToList();
        }

        public void DeletePlayer(Player player)
        {
            _context.Players.Remove(player);
            _context.SaveChanges();
        }

        public Account GetAccount(string platform, string accountName)
        {
            return _context.Accounts.FirstOrDefault(x => x.Platform == platform && x.AccountName == accountName);
        }
    }
}
