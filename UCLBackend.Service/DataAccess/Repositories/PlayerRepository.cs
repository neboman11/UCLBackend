using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using UCLBackend.DataAccess.Models;
using UCLBackend.DataAccess.Models.Responses;
using UCLBackend.Service.Data.Enums;
using UCLBackend.Service.DataAccess.Interfaces;

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
            // Create a new HTTP client
            var client = new HttpClient();
            Uri uri = new Uri($"https://api.tracker.gg/api/v2/rocket-league/standard/profile/{platform}/{username}");

            // Send the request
            var response = await client.GetAsync(uri.ToString());
            response.EnsureSuccessStatusCode();

            var player = JsonConvert.DeserializeObject<GetPlayerIDResponse>(await response.Content.ReadAsStringAsync());
            
            return player.Data.MetaData.PlayerID;
        }

        public async Task<List<(int, DateTime)>> RemoteGetPlayerMMRs(string playerID)
        {
            // Create a new HTTP client
            var client = new HttpClient();
            Uri uri = new Uri($"https://api.tracker.gg/api/v1/rocket-league/player-history/mmr/{playerID}");

            // Send the request
            var response = await client.GetAsync(uri.ToString());
            response.EnsureSuccessStatusCode();

            var player = JsonConvert.DeserializeObject<GetPlayerMMRsResponse>(await response.Content.ReadAsStringAsync());
            
            List<(int, DateTime)> mmrs = player.Data[11].ToList().Select(x => (x.Rating, x.CollectDate)).ToList();
            mmrs.AddRange(player.Data[13].ToList().Select(x => (x.Rating, x.CollectDate)).ToList());
            return mmrs;
        }

        public void UpdatePlayerPeakMMR(string playerID, int peakMMR)
        {
            var player = _context.Players.FirstOrDefault(x => x.PlayerID == playerID);
            if (player != null)
            {
                player.PeakMMR = peakMMR;
                _context.SaveChanges();
            }
        }

        public void UpdatePlayerCurrentMMR(string playerID, int mmr)
        {
            var player = _context.Players.FirstOrDefault(x => x.PlayerID == playerID);
            if (player != null)
            {
                player.CurrentMMR = mmr;
                _context.SaveChanges();
            }
        }

        public void UpdatePlayerSalary(string playerID, double salary)
        {
            var player = _context.Players.FirstOrDefault(x => x.PlayerID == playerID);
            if (player != null)
            {
                player.Salary = salary;
                _context.SaveChanges();
            }
        }

        public List<Player> GetAllPlayers()
        {
            return _context.Players.ToList();
        }

        public Player GetPlayer(string playerID)
        {
            return _context.Players.FirstOrDefault(x => x.PlayerID == playerID);
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
            return _context.Players.FirstOrDefault(x => x.DiscordID == discordID);
        }
    }
}
