using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UCLBackend.Service.Data.DataModels;
using UCLBackend.Service.Data.Helpers;

namespace UCLFrontend.Services
{
    public class PlayerService
    {
        public async Task<List<Player>> GetPlayersAsync()
        {
            return await SendWebRequest.GetAsync<List<Player>>(new Uri("http://localhost:5100/Player/GetPlayers"), "");
        }
    }
}
