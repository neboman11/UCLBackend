using UCLBackend.DataAccess.Models;
using UCLBackend.Service.DataAccess.Interfaces;
using UCLBackend.Service.Interfaces.Services;
using UCLBackend.Service.Data.Requests;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace UCLBackend.Service.Services
{
    public class PlayerService : IPlayerService
    {
        // TODO: Add service to talk to discord
        private readonly IPlayerRepository _playerRepository;

        public PlayerService(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public async Task AddPlayer(AddPlayerRequest request)
        {
            // Grab the platform and account name from the tracker url
            var platform = request.RLTrackerLink.Split('/').ToList().TakeLast(3).First();
            var accountName = request.RLTrackerLink.Split('/').ToList().TakeLast(2).First();

            var playerID = await _playerRepository.RemoteGetPlayerID(platform, accountName);

            Player player = new Player
            {
                DiscordID = request.DiscordID.ToString(),
                Name = request.PlayerName,
                PlayerID = playerID
            };

            _playerRepository.AddPlayer(player);

            var accounts = GetAccounts(request.AltRLTrackerLinks, playerID);
            accounts.Add(new Account{Platform = platform, AccountID = accountName, PlayerID = playerID, IsPrimary = true});

            foreach (var account in accounts)
            {
                _playerRepository.AddAccount(account);
            }

            await UpdatePlayerMMR(playerID);
        }

        public async Task UpdateAllMMRs()
        {
            var players = _playerRepository.GetAllPlayers();

            foreach (var player in players)
            {
                if (player.IsFreeAgent.Value)
                {
                    await UpdatePlayerMMR(player.PlayerID);
                }
            }
        }

        public void SignPlayer(string discordID, string franchiseName, string league)
        {
            var player = _playerRepository.GetPlayerUsingDiscordID(discordID);
            var team = _playerRepository.GetTeam(franchiseName, league);
            player.Team = team;
            player.IsFreeAgent = false;

            _playerRepository.UpdatePlayer(player);
        }

        public async Task ReleasePlayer(string discordID)
        {
            var player = _playerRepository.GetPlayerUsingDiscordID(discordID);
            player.IsFreeAgent = true;
            player.TeamID = -1;
            _playerRepository.UpdatePlayer(player);

            await UpdatePlayerMMR(player.PlayerID);
        }

        public void PlayerRankout(string discordID)
        {
            var player = _playerRepository.GetPlayerUsingDiscordID(discordID);

            var ultraMinSalary = double.Parse(_playerRepository.GetSetting("League.Ultra.MinSalary"));
            var eliteMinSalary = double.Parse(_playerRepository.GetSetting("League.Elite.MinSalary"));
            var superiorMinSalary = double.Parse(_playerRepository.GetSetting("League.Superior.MinSalary"));

            // TODO: Check if current mmr is above the min salary (not the frozen one)
            if (player.Salary.Value >= ultraMinSalary - 0.5)
            {
                player.PeakMMR = (int)(ultraMinSalary * 100);
                player.Salary = ultraMinSalary;
            }
            else if (player.Salary.Value.Equals(eliteMinSalary - 0.5))
            {
                
                player.PeakMMR = (int)(eliteMinSalary * 100);
                player.Salary = eliteMinSalary;
            }
            else if (player.Salary.Value.Equals(superiorMinSalary - 0.5))
            {
                
                player.PeakMMR = (int)(eliteMinSalary * 100);
                player.Salary = eliteMinSalary;
            }

            _playerRepository.UpdatePlayer(player);
        }

        #region Private Methods
        private List<Account> GetAccounts(string[] rlTrackerLinks, string PlayerID)
        {
            var accounts = new List<Account>();

            foreach (var rlTrackerLink in rlTrackerLinks)
            {
                if (!(rlTrackerLink == "" || rlTrackerLink == null))
                {
                    var platform = rlTrackerLink.Split('/').ToList().TakeLast(2).First();
                    var accountName = rlTrackerLink.Split('/').ToList().Last();

                    accounts.Add(new Account
                    {
                        Platform = platform,
                        AccountID = accountName,
                        PlayerID = PlayerID,
                        IsPrimary = false
                    });
                }
            }

            return accounts;
        }

        private async Task UpdatePlayerMMR(string playerID)
        {
            var mmrs = await _playerRepository.RemoteGetPlayerMMRs(playerID);

            _playerRepository.UpdatePlayerPeakMMR(playerID, mmrs.Select(x => x.Item1).Max());
            _playerRepository.UpdatePlayerCurrentMMR(playerID, mmrs.Find(x => x.Item2 == mmrs.Select(x => x.Item2).Max()).Item1);

            // Salary is PeakMMR rounded to the nearest 50 then divided by 100
            // Example: PeakMMR = 1277, Salary = 12.5
            _playerRepository.UpdatePlayerSalary(playerID, ((mmrs.Select(x => x.Item1).Max() / 50) * 50) / 100.0 );
        }
        #endregion
    }
}
