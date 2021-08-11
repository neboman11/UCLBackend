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
using Microsoft.Extensions.Logging;
using System.Text;
using UCLBackend.Service.Data.Discord;

namespace UCLBackend.Service.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly ISettingRepository _settingRepository;
        private readonly IDiscordService _discordService;
        private readonly ILogger<PlayerService> _logger;
        private Dictionary<PlayerLeague, ulong> _leagueChannelIds;
        private ulong _freeAgentRosterChannelId;

        public PlayerService(IPlayerRepository playerRepository, ISettingRepository settingRepository, IDiscordService discordService, ILogger<PlayerService> logger)
        {
            _playerRepository = playerRepository;
            _settingRepository = settingRepository;
            _discordService = discordService;
            _logger = logger;

            _leagueChannelIds = new Dictionary<PlayerLeague, ulong>()
            {
                {PlayerLeague.Origins, ulong.Parse(_settingRepository.GetSetting("Roster.Origins.ChannelId"))},
                {PlayerLeague.Ultra, ulong.Parse(_settingRepository.GetSetting("Roster.Ultra.ChannelId"))},
                {PlayerLeague.Elite, ulong.Parse(_settingRepository.GetSetting("Roster.Elite.ChannelId"))},
                {PlayerLeague.Superior, ulong.Parse(_settingRepository.GetSetting("Roster.Superior.ChannelId"))}
            };
            _freeAgentRosterChannelId = ulong.Parse(_settingRepository.GetSetting("Roster.FreeAgent.ChannelId"));
        }

        public async Task AddPlayer(ulong issuerDiscordID, ulong discordID, string playerName, string rlTrackerLink, string[] altRLTrackerLinks)
        {
            // Grab the platform and account name from the tracker url
            var accountParts = GetAccountParts(rlTrackerLink);
            var platform = accountParts.Item1;
            var accountName = accountParts.Item2;

            var playerID = await _playerRepository.RemoteGetPlayerID(platform, accountName);

            if (_playerRepository.GetPlayer(playerID) != null)
            {
                throw new Exception("Player is already in database");
            }

            Player player = new Player
            {
                DiscordID = discordID,
                Name = playerName,
                PlayerID = playerID
            };

            var accounts = CreateAccountsList(altRLTrackerLinks, playerID);
            accounts.Add(new Account { Platform = platform, AccountName = accountName, PlayerID = playerID, IsPrimary = true });
            // Add the accounts to the player so the MMRs can be fetched
            player.Accounts = accounts;

            player = await UpdatePlayerMMR(player);

            await _discordService.AddLeagueRolesToUser(player.DiscordID, GetPlayerLeague(player.Salary.Value));
            await _discordService.AddFranchiseRolesToUser(player.DiscordID, GetPlayerFranchise(null), GetPlayerLeague(player.Salary.Value));
            await _discordService.SetFreeAgentNickname(player.DiscordID, player.Name);

            // Remove the accounts from the player so they can be saved to the database
            player.Accounts = null;

            // Save the player to the database last in case something goes wrong
            _playerRepository.AddPlayer(player);

            foreach (var account in accounts)
            {
                _playerRepository.AddAccount(account);
            }

            await LogTransaction(issuerDiscordID, $"Added player {player.Name}");
        }

        public async Task UpdateAllMMRs()
        {
            var players = _playerRepository.GetAllPlayers();

            foreach (var player in players)
            {
                if (player.IsFreeAgent.Value)
                {
                    try
                    {
                        var oldLeague = GetPlayerLeague(player.Salary.Value);

                        var newPlayer = await UpdatePlayerMMR(player);
                        var newLeague = GetPlayerLeague(newPlayer.Salary.Value);

                        if (oldLeague != newLeague)
                        {
                            await _discordService.AddFranchiseRolesToUser(player.DiscordID, GetPlayerFranchise(null), newLeague);
                            await _discordService.AddLeagueRolesToUser(player.DiscordID, newLeague);
                            await _discordService.RemoveFranchiseRoles(player.DiscordID, GetPlayerFranchise(null), oldLeague);
                            await _discordService.RemoveLeagueRoles(player.DiscordID, oldLeague);

                            await _discordService.LogTransaction(1, $"Player {player.Name} was moved from {oldLeague} to {newLeague}");
                        }

                        _playerRepository.UpdatePlayer(newPlayer);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"Error updating MMR for player {player.Name} ({player.PlayerID})");
                    }
                }
            }

            _logger.LogInformation("Updated MMRs for all players");
        }

        public async Task UpdateSingleMMR(ulong discordID)
        {
            var player = _playerRepository.GetPlayerUsingDiscordID(discordID);

            var oldLeague = GetPlayerLeague(player.Salary.Value);

            var newPlayer = await UpdatePlayerMMR(player);
            var newLeague = GetPlayerLeague(newPlayer.Salary.Value);

            try
            {
                await _discordService.RemoveLeagueRoles(player.DiscordID, oldLeague);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error removing league role player {player.Name} ({player.PlayerID})");
            }

            await _discordService.AddLeagueRolesToUser(player.DiscordID, newLeague);


            if (oldLeague != newLeague)
            {
                await _discordService.LogTransaction(1, $"Player {player.Name} was moved from {oldLeague} to {newLeague}");
            }

            _logger.LogTrace($"Old peak MMR: {player.PeakMMR}, New peak MMR: {newPlayer.PeakMMR}");

            _playerRepository.UpdatePlayer(newPlayer);
        }

        public async Task SignPlayer(ulong issuerDiscordID, ulong discordID, string franchiseName)
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

            // Update Discord
            await _discordService.AddFranchiseRolesToUser(player.DiscordID, GetPlayerFranchise(player.Team), GetPlayerLeague(player.Salary.Value));
            await _discordService.RemoveFranchiseRoles(player.DiscordID, GetPlayerFranchise(null), GetPlayerLeague(player.Salary.Value));
            await _discordService.SetFranchiseNickname(player.DiscordID, GetPlayerFranchise(player.Team), player.Name);

            _playerRepository.UpdatePlayer(player);

            await LogTransaction(issuerDiscordID, $"Signed player {player.Name} to {franchiseName}");
        }

        public async Task ReleasePlayer(ulong issuerDiscordID, ulong discordID)
        {
            var player = _playerRepository.GetPlayerUsingDiscordID(discordID);

            if (player == null)
            {
                throw new ArgumentException("Player not found");
            }

            if (player.IsFreeAgent.Value)
            {
                throw new ArgumentException("Player is not signed to a team");
            }

            // Need to remove the old franchise role before clearing the team
            await _discordService.RemoveFranchiseRoles(player.DiscordID, GetPlayerFranchise(player.Team), GetPlayerLeague(player.Salary.Value));

            player.IsFreeAgent = true;
            player.TeamID = null;
            player = await UpdatePlayerMMR(player);

            // Update Discord
            await _discordService.AddFranchiseRolesToUser(player.DiscordID, GetPlayerFranchise(null), GetPlayerLeague(player.Salary.Value));
            await _discordService.SetFreeAgentNickname(player.DiscordID, player.Name);

            _playerRepository.UpdatePlayer(player);

            await LogTransaction(issuerDiscordID, $"Released player {player.Name}");
        }

        public async Task PlayerRankout(ulong issuerDiscordID, ulong discordID)
        {
            var player = _playerRepository.GetPlayerUsingDiscordID(discordID);

            if (player == null)
            {
                throw new ArgumentException("Player not found");
            }

            var ultraMinSalary = double.Parse(_settingRepository.GetSetting("League.Ultra.MinSalary"));
            var eliteMinSalary = double.Parse(_settingRepository.GetSetting("League.Elite.MinSalary"));
            var superiorMinSalary = double.Parse(_settingRepository.GetSetting("League.Superior.MinSalary"));

            // Check if current mmr is above the min salary (not the frozen one)
            var mmrs = await _playerRepository.RemoteGetPlayerMMRs(player.PlayerID);
            var doublesMMRs = mmrs.Item1;
            var triplesMMRs = mmrs.Item2;

            var newPeakMMR = doublesMMRs.Select(x => x.Item1).Max() > triplesMMRs.Select(x => x.Item1).Max() ? doublesMMRs.Select(x => x.Item1).Max() : triplesMMRs.Select(x => x.Item1).Max();
            var newSalary = ((newPeakMMR / 50) * 50) / 100.0;

            if (newSalary >= superiorMinSalary - 0.5 && GetPlayerLeague(player.Salary.Value) != PlayerLeague.Superior)
            {
                await _discordService.AddLeagueRolesToUser(discordID, PlayerLeague.Superior);
                await _discordService.RemoveLeagueRoles(discordID, GetPlayerLeague(player.Salary.Value));

                player.PeakMMR = newPeakMMR;
                player.Salary = newSalary;
            }
            else if (newSalary >= eliteMinSalary - 0.5 && (GetPlayerLeague(player.Salary.Value) != PlayerLeague.Elite || GetPlayerLeague(player.Salary.Value) != PlayerLeague.Superior))
            {
                await _discordService.AddLeagueRolesToUser(discordID, PlayerLeague.Elite);
                await _discordService.RemoveLeagueRoles(discordID, GetPlayerLeague(player.Salary.Value));

                player.PeakMMR = newPeakMMR;
                player.Salary = newSalary;
            }
            else if (newSalary >= ultraMinSalary - 0.5 && (GetPlayerLeague(player.Salary.Value) != PlayerLeague.Ultra || GetPlayerLeague(player.Salary.Value) != PlayerLeague.Elite || GetPlayerLeague(player.Salary.Value) != PlayerLeague.Superior))
            {
                await _discordService.AddLeagueRolesToUser(discordID, PlayerLeague.Ultra);
                await _discordService.RemoveLeagueRoles(discordID, GetPlayerLeague(player.Salary.Value));

                player.PeakMMR = newPeakMMR;
                player.Salary = newSalary;
            }
            else
            {
                throw new Exception("Player cannot rankout");
            }

            _playerRepository.UpdatePlayer(player);

            await LogTransaction(issuerDiscordID, $"Player {player.Name} ranked out");
        }

        public PlayerInfoResponse GetPlayerInfo(ulong discordID)
        {
            var player = _playerRepository.GetPlayerUsingDiscordID(discordID);

            if (player == null)
            {
                throw new ArgumentException("Player not found");
            }

            return new PlayerInfoResponse
            {
                Salary = player.Salary.Value,
                PeakMMR = player.PeakMMR.Value,
                CurrentMMR = player.CurrentMMR.Value
            };
        }

        public async Task FreeAgentsList()
        {
            var freeAgents = _playerRepository.GetFreeAgents().OrderBy(x => x.Salary).ThenBy(x => x.Name).ToList();
            var playersByLeague = SeparatePlayersByLeague(freeAgents);

            foreach (var league in Enum.GetValues(typeof(PlayerLeague)).Cast<PlayerLeague>())
            {
                var discordMessage = new StringBuilder();
                discordMessage.AppendLine("```");

                foreach (var player in playersByLeague[league])
                {
                    discordMessage.AppendLine(player.Name.PadRight(18) + player.Salary.ToString().PadLeft(4));
                }
                discordMessage.AppendLine("```");

                await _discordService.SendEmbed(_freeAgentRosterChannelId, new Embed() { Title = $"{league}: Free Agents", Description = discordMessage.ToString() });
            }
        }

        public async Task Roster()
        {
            foreach (var league in Enum.GetValues(typeof(PlayerLeague)).Cast<PlayerLeague>())
            {
                var teamsInLeague = _playerRepository.TeamsByLeague(league.ToString()).OrderBy(x => x.TeamName);

                foreach (var team in teamsInLeague)
                {
                    var discordMessage = new StringBuilder();
                    discordMessage.AppendLine("```");

                    foreach (var player in team.Players)
                    {
                        discordMessage.AppendLine(player.Name.PadRight(15) + player.Salary.ToString().PadLeft(4));
                    }

                    if (league != PlayerLeague.Superior)
                    {
                        var temp = $"League.{league}.CapSpace";
                        var maxSalary = double.Parse(_settingRepository.GetSetting($"League.{league}.CapSpace"));
                        var currentTotalSalary = team.Players.Sum(x => x.Salary);
                        discordMessage.AppendLine($"Salary Remaining: {maxSalary - currentTotalSalary}");
                    }

                    discordMessage.AppendLine("```");

                    await _discordService.SendEmbed(_leagueChannelIds[league], new Embed() { Title = $"{league}: {team.TeamName}", Description = discordMessage.ToString() });
                }
            }
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
                        var accountParts = GetAccountParts(rlTrackerLink);

                        accounts.Add(new Account
                        {
                            Platform = accountParts.Item1,
                            AccountName = accountParts.Item2,
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
            var mmrs = await _playerRepository.RemoteGetPlayerMMRs(player.PlayerID);
            var doublesMMRs = mmrs.Item1;
            var triplesMMRs = mmrs.Item2;
            var peakMMR = doublesMMRs.Select(x => x.Item1).Max() > triplesMMRs.Select(x => x.Item1).Max() ? doublesMMRs.Select(x => x.Item1).Max() : triplesMMRs.Select(x => x.Item1).Max();
            var currentMMR = doublesMMRs.Find(x => x.Item2 == doublesMMRs.Select(x => x.Item2).Max()).Item1 > triplesMMRs.Find(x => x.Item2 == triplesMMRs.Select(x => x.Item2).Max()).Item1 ? doublesMMRs.Find(x => x.Item2 == doublesMMRs.Select(x => x.Item2).Max()).Item1 : triplesMMRs.Find(x => x.Item2 == triplesMMRs.Select(x => x.Item2).Max()).Item1;

            foreach (var account in player.Accounts)
            {
                var playerID = await _playerRepository.RemoteGetPlayerID(account.Platform, account.AccountName);

                if (string.IsNullOrEmpty(playerID))
                {
                    throw new Exception("Failed fetching player ID for alt account");
                }

                mmrs = await _playerRepository.RemoteGetPlayerMMRs(playerID);
                doublesMMRs = mmrs.Item1;
                triplesMMRs = mmrs.Item2;

                var tempPeakMMR = doublesMMRs.Select(x => x.Item1).Max() > triplesMMRs.Select(x => x.Item1).Max() ? doublesMMRs.Select(x => x.Item1).Max() : triplesMMRs.Select(x => x.Item1).Max();

                if (tempPeakMMR > peakMMR)
                {
                    peakMMR = tempPeakMMR;
                    currentMMR = doublesMMRs.Find(x => x.Item2 == doublesMMRs.Select(x => x.Item2).Max()).Item1 > triplesMMRs.Find(x => x.Item2 == triplesMMRs.Select(x => x.Item2).Max()).Item1 ? doublesMMRs.Find(x => x.Item2 == doublesMMRs.Select(x => x.Item2).Max()).Item1 : triplesMMRs.Find(x => x.Item2 == triplesMMRs.Select(x => x.Item2).Max()).Item1;
                }
            }

            player.PeakMMR = peakMMR;
            player.Salary = ((player.PeakMMR / 50) * 50) / 100.0;
            player.CurrentMMR = currentMMR;

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
                case "CMM":
                    return PlayerFranchise.CMM;
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
                    throw new ArgumentException("Invalid team name");
            }
        }

        private async Task LogTransaction(ulong issuerDiscordID, string message, string discordMessage = null)
        {
            _logger.LogInformation($"{issuerDiscordID}: {message}");

            if (discordMessage == null)
            {
                discordMessage = message;
            }

            await _discordService.LogTransaction(issuerDiscordID, discordMessage);
        }

        // Return: First is platform, second is account name
        private (string, string) GetAccountParts(string trackerLink)
        {
            var platform = trackerLink.Split('/').ToList().TakeLast(3).First();
            var accountName = trackerLink.Split('/').ToList().TakeLast(2).First();

            if (string.IsNullOrEmpty(platform) || string.IsNullOrEmpty(accountName))
            {
                throw new ArgumentException($"Alt tracker link is malformed: {trackerLink}");
            }

            // TODO: Move this into an enum or something
            if (platform != "xbl" && platform != "psn" && platform != "epic" && platform != "steam")
            {
                throw new ArgumentException($"Invalid platform: {platform} (in {trackerLink})");
            }

            return (platform, accountName);
        }

        private Dictionary<PlayerLeague, List<Player>> SeparatePlayersByLeague(List<Player> players)
        {
            var playerDict = new Dictionary<PlayerLeague, List<Player>>();

            // Initialize dictionary
            foreach (var league in Enum.GetValues(typeof(PlayerLeague)).Cast<PlayerLeague>())
            {
                playerDict[league] = new List<Player>();
            }

            foreach (var player in players)
            {
                var league = GetPlayerLeague(player.Salary.Value);
                playerDict[league].Add(player);
            }

            return playerDict;
        }
        #endregion
    }
}
