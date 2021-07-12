using UCLBackend.Service.DataAccess.Models;
using UCLBackend.Service.DataAccess.Interfaces;
using UCLBackend.Service.Services.Interfaces;
using UCLBackend.Service.Data.Requests;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using UCLBackend.Service.Data.Enums;
using System;
using UCLBackend.Service.Data.Responses;

namespace UCLBackend.Service.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly ISettingRepository _settingRepository;
        private readonly IDiscordService _discordService;

        public PlayerService(IPlayerRepository playerRepository, ISettingRepository settingRepository, IDiscordService discordService)
        {
            _playerRepository = playerRepository;
            _settingRepository = settingRepository;
            _discordService = discordService;
        }

        public async Task AddPlayer(AddPlayerRequest request)
        {
            // Grab the platform and account name from the tracker url
            var platform = request.RLTrackerLink.Split('/').ToList().TakeLast(3).First();
            var accountName = request.RLTrackerLink.Split('/').ToList().TakeLast(2).First();

            var playerID = await _playerRepository.RemoteGetPlayerID(platform, accountName);

            Player player = new Player
            {
                DiscordID = request.DiscordID,
                Name = request.PlayerName,
                PlayerID = playerID
            };

            var accounts = CreateAccountsList(request.AltRLTrackerLinks, playerID);
            accounts.Add(new Account{Platform = platform, AccountID = accountName, PlayerID = playerID, IsPrimary = true});

            player = await UpdatePlayerMMR(player);

            await _discordService.AddLeagueRolesToUser(player.DiscordID, GetPlayerLeague(player.Salary.Value));
            await _discordService.AddFranchiseRolesToUser(player.DiscordID, GetPlayerFranchise(null));

            // Save the player to the database last in case something goes wrong
            _playerRepository.AddPlayer(player);

            foreach (var account in accounts)
            {
                _playerRepository.AddAccount(account);
            }
        }

        public async Task UpdateAllMMRs()
        {
            // TODO: Update league roles
            var players = _playerRepository.GetAllPlayers();

            foreach (var player in players)
            {
                if (player.IsFreeAgent.Value)
                {
                    await UpdatePlayerMMR(player);
                }
            }
        }

        public void SignPlayer(ulong discordID, string franchiseName)
        {
            var player = _playerRepository.GetPlayerUsingDiscordID(discordID);

            if (player == null)
            {
                throw new ArgumentException("Player not found");
            }

            if (!player.IsFreeAgent.Value)
            {
                throw new ArgumentException("Player is already signed");
            }

            var team = _playerRepository.GetTeam(franchiseName, GetPlayerLeague(player.Salary.Value));

            if (team == null)
            {
                throw new Exception("Could not find team");
            }

            player.Team = team;
            player.IsFreeAgent = false;

            _discordService.AddFranchiseRolesToUser(player.DiscordID, GetPlayerFranchise(player.Team));

            _playerRepository.UpdatePlayer(player);
        }

        public async Task ReleasePlayer(ulong discordID)
        {
            var player = _playerRepository.GetPlayerUsingDiscordID(discordID);
            player.IsFreeAgent = true;
            player.TeamID = null;
            player = await UpdatePlayerMMR(player);

            _playerRepository.UpdatePlayer(player);
        }

        public async Task PlayerRankout(ulong discordID)
        {
            var player = _playerRepository.GetPlayerUsingDiscordID(discordID);

            var ultraMinSalary = double.Parse(_settingRepository.GetSetting("League.Ultra.MinSalary"));
            var eliteMinSalary = double.Parse(_settingRepository.GetSetting("League.Elite.MinSalary"));
            var superiorMinSalary = double.Parse(_settingRepository.GetSetting("League.Superior.MinSalary"));

            // Check if current mmr is above the min salary (not the frozen one)
            var mmrs = await _playerRepository.RemoteGetPlayerMMRs(player.PlayerID);

            var peakMMR = mmrs.Select(x => x.Item1).Max();
            var salary = ((peakMMR / 50) * 50) / 100.0;

            if (salary.Equals(ultraMinSalary - 0.5))
            {
                player.PeakMMR = (int)(ultraMinSalary * 100);
                player.Salary = ultraMinSalary;
            }
            else if (salary > ultraMinSalary - 0.5)
            {
                player.PeakMMR = peakMMR;
                player.Salary = salary;
            }
            else if (salary.Equals(eliteMinSalary - 0.5))
            {
                player.PeakMMR = (int)(eliteMinSalary * 100);
                player.Salary = eliteMinSalary;
            }
            else if (salary > eliteMinSalary - 0.5)
            {
                player.PeakMMR = peakMMR;
                player.Salary = salary;
            }
            else if (player.Salary.Value.Equals(superiorMinSalary - 0.5))
            {
                
                player.PeakMMR = (int)(eliteMinSalary * 100);
                player.Salary = eliteMinSalary;
            }
            else if (salary > superiorMinSalary - 0.5)
            {
                player.PeakMMR = peakMMR;
                player.Salary = salary;
            }

            _playerRepository.UpdatePlayer(player);
        }

        public PlayerInfoResponse GetPlayerInfo(ulong discordID)
        {
            var player = _playerRepository.GetPlayerUsingDiscordID(discordID);

            return new PlayerInfoResponse
            {
                Salary = player.Salary.Value,
                PeakMMR = player.PeakMMR.Value,
                CurrentMMR = player.CurrentMMR.Value
            };
        }

        #region Private Methods
        private List<Account> CreateAccountsList(string[] rlTrackerLinks, string PlayerID)
        {
            var accounts = new List<Account>();

            if (rlTrackerLinks != null)
            {
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
            }

            return accounts;
        }

        private async Task<Player> UpdatePlayerMMR(Player player)
        {
            // TODO: CurrentMMR is max of 2s and 3s latest mmr
            var mmrs = await _playerRepository.RemoteGetPlayerMMRs(player.PlayerID);

            player.PeakMMR = mmrs.Select(x => x.Item1).Max();
            player.CurrentMMR = mmrs.Find(x => x.Item2 == mmrs.Select(x => x.Item2).Max()).Item1;
            player.Salary = ((mmrs.Select(x => x.Item1).Max() / 50) * 50) / 100.0;

            return player;
        }

        private PlayerLeague GetPlayerLeague(double salary)
        {
            var ultraMinSalary = double.Parse(_settingRepository.GetSetting("League.Ultra.MinSalary"));
            var eliteMinSalary = double.Parse(_settingRepository.GetSetting("League.Elite.MinSalary"));
            var superiorMinSalary = double.Parse(_settingRepository.GetSetting("League.Superior.MinSalary"));

            if (salary < ultraMinSalary)
            {
                return PlayerLeague.Origins;
            }
            else if (salary < eliteMinSalary)
            {
                return PlayerLeague.Ultra;
            }
            else if (salary < superiorMinSalary)
            {
                return PlayerLeague.Elite;
            }
            else
            {
                return PlayerLeague.Superior;
            }
        }

        private PlayerFranchise GetPlayerFranchise(Team team)
        {
            if (team == null)
            {
                return PlayerFranchise.Free_Agents;
            }

            switch (team.TeamName)
            {
                case "Astros":
                    return PlayerFranchise.Astros;
                case "Atlantics":
                    return PlayerFranchise.Atlantics;
                case "Bison":
                    return PlayerFranchise.Bison;
                case "Cobras":
                    return PlayerFranchise.Cobras;
                case "Gators":
                    return PlayerFranchise.Gators;
                case "Knights":
                    return PlayerFranchise.Knights;
                case "Lightning":
                    return PlayerFranchise.Lightning;
                case "Raptors":
                    return PlayerFranchise.Raptors;
                case "Samurai":
                    return PlayerFranchise.Samurai;
                case "Spartans":
                    return PlayerFranchise.Spartans;
                case "XII Boost":
                    return PlayerFranchise.XII_Boost;
                case "Vikings":
                    return PlayerFranchise.Vikings;
                default:
                    return PlayerFranchise.Free_Agents;
            }
        }
        #endregion
    }
}
