using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UCLBackend.Service.Data.DataModels;
using UCLBackend.Service.Data.Enums;
using UCLBackend.Service.Data.Helpers;

namespace UCLFrontend.Services
{
    public class PlayerService
    {
        public async Task<List<Player>> GetPlayersAsync()
        {
            return (await SendWebRequest.GetAsync<List<Player>>(new Uri("http://localhost:5000/Player/GetPlayers"), "")).OrderBy(x => x.Name).ToList();
        }

        public async Task<List<Player>> GetPlayersByLeagueAsync(PlayerLeague league)
        {
            return (await SendWebRequest.GetAsync<List<Player>>(new Uri($"http://localhost:5000/Player/GetPlayersByLeague?league={league}"), "")).OrderBy(x => x.Name).ToList();
        }

        public async Task<Dictionary<PlayerLeague, double>> GetLeagueMinSalariesAsync()
        {
            return (await SendWebRequest.GetAsync<Dictionary<PlayerLeague, double>>(new Uri("http://localhost:5000/Setting/MinSalaries"), ""));
        }
    }
}
