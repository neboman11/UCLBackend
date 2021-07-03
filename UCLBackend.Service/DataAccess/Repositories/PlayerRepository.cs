using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using UCLBackend.DataAccess.Models;
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
            // Check if the response was successful
            if (response.IsSuccessStatusCode)
            {
                var player = JsonConvert.DeserializeObject<GetPlayerIDRequest>(await response.Content.ReadAsStringAsync());
                // Return true if the response was successful
                return player.Data.MetaData.PlayerID;
            }

            throw new Exception("Failed to get player ID");
        }
    }
}