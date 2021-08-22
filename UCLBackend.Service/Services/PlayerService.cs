using UCLBackend.Service.DataAccess.Interfaces;
using UCLBackend.Service.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using UCLBackend.Service.Data.Enums;
using System;
using UCLBackend.Service.Data.Responses;
using Microsoft.Extensions.Logging;
using System.Text;
using UCLBackend.Service.Data.Discord;
using UCLBackend.Service.Data.Exceptions;
using UCLBackend.Service.Data.DataModels;
using UCLBackend.Service.Data.Helpers;
using UCLBackend.Service.Data.MMR;

namespace UCLBackend.Service.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly ISettingRepository _settingRepository;
        private readonly IDiscordService _discordService;
        private readonly IRedisService _redisService;
        private readonly ILogger<PlayerService> _logger;
        private Dictionary<PlayerLeague, ulong> _leagueChannelIds;
        private Dictionary<PlayerLeague, double> _leagueMinSalaries;
        private ulong _freeAgentRosterChannelId;

        public PlayerService(IPlayerRepository playerRepository, ISettingRepository settingRepository, IDiscordService discordService, IRedisService redisService, ILogger<PlayerService> logger)
        {
            _playerRepository = playerRepository;
            _settingRepository = settingRepository;
            _discordService = discordService;
            _redisService = redisService;
            _logger = logger;

            _leagueChannelIds = new Dictionary<PlayerLeague, ulong>()
            {
                {PlayerLeague.Origins, ulong.Parse(_settingRepository.GetSetting("Roster.Origins.ChannelId"))},
                {PlayerLeague.Ultra, ulong.Parse(_settingRepository.GetSetting("Roster.Ultra.ChannelId"))},
                {PlayerLeague.Elite, ulong.Parse(_settingRepository.GetSetting("Roster.Elite.ChannelId"))},
                {PlayerLeague.Superior, ulong.Parse(_settingRepository.GetSetting("Roster.Superior.ChannelId"))}
            };
            _leagueMinSalaries = _settingRepository.GetMinSalaries();
            _freeAgentRosterChannelId = ulong.Parse(_settingRepository.GetSetting("Roster.FreeAgent.ChannelId"));
        }

        public async Task AddPlayer(ulong issuerDiscordID, ulong discordID, string playerName, string rlTrackerLink, string[] altRLTrackerLinks)
        {
            // Grab the platform and account name from the tracker url
            var accountParts = GetAccountParts(rlTrackerLink);
            var platform = accountParts.Item1;
            var accountName = accountParts.Item2;

            // TODO: Change to checking if given accounts are already in the database
            // if (_playerRepository.GetPlayer(playerID) != null)
            // {
            //     throw new Exception("Player is already in database");
            // }

            Player player = new Player
            {
                DiscordID = discordID,
                Name = playerName
            };

            var accounts = CreateAccountsList(altRLTrackerLinks);
            accounts.Add(new Account { Platform = platform, AccountName = accountName, IsPrimary = true });
            // Add the accounts to the player so the MMRs can be fetched
            player.Accounts = accounts;

            player = await UpdatePlayerMMR(player);

            await _discordService.AddLeagueRolesToUser(player.DiscordID, PlayerHelpers.GetPlayerLeague(player.Salary, _leagueMinSalaries));
            await _discordService.AddFranchiseRolesToUser(player.DiscordID, GetPlayerFranchise(null), PlayerHelpers.GetPlayerLeague(player.Salary, _leagueMinSalaries));
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
            var players = _playerRepository.GetAllPlayersWithAccounts();

            foreach (var player in players)
            {
                if (player.IsFreeAgent.Value)
                {
                    try
                    {
                        var oldLeague = PlayerHelpers.GetPlayerLeague(player.Salary, _leagueMinSalaries);

                        var newPlayer = await UpdatePlayerMMR(player);
                        var newLeague = PlayerHelpers.GetPlayerLeague(newPlayer.Salary, _leagueMinSalaries);

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

            var oldLeague = PlayerHelpers.GetPlayerLeague(player.Salary, _leagueMinSalaries);

            var newPlayer = await UpdatePlayerMMR(player);
            var newLeague = PlayerHelpers.GetPlayerLeague(newPlayer.Salary, _leagueMinSalaries);

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

            var team = _playerRepository.GetTeam(franchiseName, PlayerHelpers.GetPlayerLeague(player.Salary, _leagueMinSalaries));

            if (team == null)
            {
                throw new Exception("Could not find team");
            }

            player.Team = team;
            player.IsFreeAgent = false;

            // Update Discord
            await _discordService.AddFranchiseRolesToUser(player.DiscordID, GetPlayerFranchise(player.Team), PlayerHelpers.GetPlayerLeague(player.Salary, _leagueMinSalaries));
            await _discordService.RemoveFranchiseRoles(player.DiscordID, GetPlayerFranchise(null), PlayerHelpers.GetPlayerLeague(player.Salary, _leagueMinSalaries));
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
            await _discordService.RemoveFranchiseRoles(player.DiscordID, GetPlayerFranchise(player.Team), PlayerHelpers.GetPlayerLeague(player.Salary, _leagueMinSalaries));

            player.IsFreeAgent = true;
            player.TeamID = null;
            player = await UpdatePlayerMMR(player);

            // Update Discord
            await _discordService.AddFranchiseRolesToUser(player.DiscordID, GetPlayerFranchise(null), PlayerHelpers.GetPlayerLeague(player.Salary, _leagueMinSalaries));
            await _discordService.SetFreeAgentNickname(player.DiscordID, player.Name);

            _playerRepository.UpdatePlayer(player);

            await LogTransaction(issuerDiscordID, $"Released player {player.Name}");
        }

        // TODO: Check logic here
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
            var mmrStats = await GetMMRStats(player);
            var playerLeague = PlayerHelpers.GetPlayerLeague(player.Salary, _leagueMinSalaries);

            if (mmrStats.Salary >= superiorMinSalary - 0.5 && playerLeague != PlayerLeague.Superior)
            {
                await _discordService.AddLeagueRolesToUser(discordID, PlayerLeague.Superior);
                await _discordService.RemoveLeagueRoles(discordID, playerLeague);

                player.PeakMMR = mmrStats.PeakMMR;
                player.Salary = superiorMinSalary;
                player.CurrentMMR = mmrStats.CurrentMMR;
            }
            else if (mmrStats.Salary >= eliteMinSalary - 0.5 && (playerLeague != PlayerLeague.Elite || playerLeague != PlayerLeague.Superior))
            {
                await _discordService.AddLeagueRolesToUser(discordID, PlayerLeague.Elite);
                await _discordService.RemoveLeagueRoles(discordID, playerLeague);

                player.PeakMMR = mmrStats.PeakMMR;
                player.Salary = eliteMinSalary;
            }
            else if (mmrStats.Salary >= ultraMinSalary - 0.5 && (playerLeague != PlayerLeague.Ultra || playerLeague != PlayerLeague.Elite || playerLeague != PlayerLeague.Superior))
            {
                await _discordService.AddLeagueRolesToUser(discordID, PlayerLeague.Ultra);
                await _discordService.RemoveLeagueRoles(discordID, playerLeague);

                player.PeakMMR = mmrStats.PeakMMR;
                player.Salary = ultraMinSalary;
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
                throw new UCLException("Player not found");
            }

            return new PlayerInfoResponse
            {
                Salary = player.Salary,
                PeakMMR = player.PeakMMR,
                CurrentMMR = player.CurrentMMR,
                Name = player.Name
            };
        }

        public async Task FreeAgentsList()
        {
            var freeAgents = _playerRepository.GetFreeAgents().OrderBy(x => x.Salary).ThenBy(x => x.Name).ToList();
            var playersByLeague = SeparatePlayersByLeague(freeAgents);

            foreach (var league in Enum.GetValues(typeof(PlayerLeague)).Cast<PlayerLeague>())
            {
                // Remove old message
                ulong oldMessageId = 0;
                string redisKey = $"{league}.FreeAgent.MessageId";
                if (ulong.TryParse(await _redisService.RetrieveValue(redisKey), out oldMessageId))
                {
                    await _discordService.DeleteMessage(_freeAgentRosterChannelId, oldMessageId);
                }

                var discordMessage = new StringBuilder();
                discordMessage.AppendLine("```");

                foreach (var player in playersByLeague[league])
                {
                    discordMessage.AppendLine(player.Name.PadRight(18) + player.Salary.ToString().PadLeft(4));
                }
                discordMessage.AppendLine("```");

                var message = await _discordService.SendEmbed(_freeAgentRosterChannelId, new Embed() { Title = $"{league}: Free Agents", Description = discordMessage.ToString() });
                await _redisService.StoreValue(redisKey, message.Id.ToString());
                _logger.LogInformation($"Sent {league} free agent list");
            }
        }

        public async Task Roster()
        {
            foreach (var league in Enum.GetValues(typeof(PlayerLeague)).Cast<PlayerLeague>())
            {
                var teamsInLeague = _playerRepository.TeamsByLeague(league.ToString()).OrderBy(x => x.TeamName);

                foreach (var team in teamsInLeague)
                {
                    // Remove old message
                    ulong oldMessageId = 0;
                    string redisKey = $"{league}.{team.TeamName}.MessageId";
                    if (ulong.TryParse(await _redisService.RetrieveValue(redisKey), out oldMessageId))
                    {
                        try
                        {
                            await _discordService.DeleteMessage(_freeAgentRosterChannelId, oldMessageId);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Failed to delete message {oldMessageId}");
                        }
                    }

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

                    var message = await _discordService.SendEmbed(_leagueChannelIds[league], new Embed() { Title = $"{league}: {team.TeamName}", Description = discordMessage.ToString() });
                    await _redisService.StoreValue(redisKey, message.Id.ToString());
                    _logger.LogInformation($"Sent {league} {team.TeamName} roster");
                }
            }
        }

        public async Task<List<Player>> GetPlayers()
        {
            var players = await _playerRepository.GetAllPlayersWithTeams();

            return players;
        }

        public async Task<List<Player>> GetPlayersByLeague(string league)
        {
            var players = await _playerRepository.GetAllPlayersWithTeams();

            return players.Where(x => PlayerHelpers.GetPlayerLeague(x.Salary, _leagueMinSalaries) == Enum.Parse<PlayerLeague>(league)).ToList();
        }

        public async Task AddAltAccount(ulong issuerDiscordID, ulong discordID, string rlTrackerLink)
        {
            var player = _playerRepository.GetPlayerUsingDiscordID(discordID);
            if (player == null)
            {
                throw new UCLException("Player not found");
            }

            if (player.Accounts.Count >= 4)
            {
                throw new UCLException("Player already has 3 alt accounts");
            }

            var accountParts = GetAccountParts(rlTrackerLink);

            player.Accounts.Add(new Account
            {
                Platform = accountParts.Item1,
                AccountName = accountParts.Item2,
                PlayerID = player.PlayerID,
                IsPrimary = false
            });

            player = await UpdatePlayerMMR(player);

            _playerRepository.UpdatePlayer(player);

            await LogTransaction(issuerDiscordID, $"Added alt account to {player.Name}");
        }

        public async Task ChangeMainAccount(ulong issuerDiscordID, ulong discordID, string rlTrackerLink)
        {
            var player = _playerRepository.GetPlayerUsingDiscordID(discordID);
            if (player == null)
            {
                throw new UCLException("Player not found");
            }

            var accountParts = GetAccountParts(rlTrackerLink);

            var mainAccount = player.Accounts.FirstOrDefault(x => x.IsPrimary);

            mainAccount.Platform = accountParts.Item1;
            mainAccount.AccountName = accountParts.Item2;

            player = await UpdatePlayerMMR(player);

            _playerRepository.UpdatePlayer(player);

            await LogTransaction(issuerDiscordID, $"Changed main account for {player.Name}");
        }

        public async Task ChangePlayerName(ulong issuerDiscordID, ulong discordID, string newName)
        {
            var player = _playerRepository.GetPlayerUsingDiscordIDWithTeam(discordID);
            if (player == null)
            {
                throw new UCLException("Player not found");
            }

            player.Name = newName;

            if (player.Team == null)
            {
                await _discordService.SetFreeAgentNickname(discordID, newName);
            }
            else
            {
                await _discordService.SetFranchiseNickname(discordID, GetPlayerFranchise(player.Team), newName);
            }

            _playerRepository.UpdatePlayer(player);

            await LogTransaction(issuerDiscordID, $"Changed player name for {player.Name}");
        }

        public async Task RemoveMissingServerPlayers()
        {
            var dbPlayers = _playerRepository.GetAllPlayers();
            var srvPlayers = await _discordService.GetServerMembers();

            var missingPlayers = dbPlayers.Where(x => !srvPlayers.Select(y => y.User.Id).Contains(x.DiscordID)).ToList();

            foreach (var player in missingPlayers)
            {
                _logger.LogInformation($"Removing player {player.Name} from database");
                
                await LogTransaction(1, $"Removed {player.Name} from the database");
            }
        }

        #region Private Methods
        private List<Account> CreateAccountsList(string[] rlTrackerLinks)
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
                            IsPrimary = false
                        });
                    }
                }
            }

            return accounts;
        }

        private async Task<Player> UpdatePlayerMMR(Player player)
        {
            var mmrStats = await GetMMRStats(player);
            player.PeakMMR = mmrStats.PeakMMR;
            player.Salary = mmrStats.Salary;
            player.CurrentMMR = mmrStats.CurrentMMR;

            return player;
        }

        private async Task<MMRStats> GetMMRStats(Player player)
        {
            var mainAccount = player.Accounts.FirstOrDefault(x => x.IsPrimary);
            var mmrs = await GetMMRHistory(mainAccount.Platform, mainAccount.AccountName);
            var doublesMMRs = mmrs.Item1;
            var triplesMMRs = mmrs.Item2;
            var peakMMR = doublesMMRs.Select(x => x.Item1).Max() > triplesMMRs.Select(x => x.Item1).Max() ? doublesMMRs.Select(x => x.Item1).Max() : triplesMMRs.Select(x => x.Item1).Max();
            var currentMMR = doublesMMRs.Find(x => x.Item2 == doublesMMRs.Select(x => x.Item2).Max()).Item1 > triplesMMRs.Find(x => x.Item2 == triplesMMRs.Select(x => x.Item2).Max()).Item1 ? doublesMMRs.Find(x => x.Item2 == doublesMMRs.Select(x => x.Item2).Max()).Item1 : triplesMMRs.Find(x => x.Item2 == triplesMMRs.Select(x => x.Item2).Max()).Item1;

            foreach (var account in player.Accounts)
            {
                mmrs = await GetMMRHistory(account.Platform, account.AccountName);
                doublesMMRs = mmrs.Item1;
                triplesMMRs = mmrs.Item2;

                var tempPeakMMR = doublesMMRs.Select(x => x.Item1).Max() > triplesMMRs.Select(x => x.Item1).Max() ? doublesMMRs.Select(x => x.Item1).Max() : triplesMMRs.Select(x => x.Item1).Max();

                if (tempPeakMMR > peakMMR)
                {
                    peakMMR = tempPeakMMR;
                    currentMMR = doublesMMRs.Find(x => x.Item2 == doublesMMRs.Select(x => x.Item2).Max()).Item1 > triplesMMRs.Find(x => x.Item2 == triplesMMRs.Select(x => x.Item2).Max()).Item1 ? doublesMMRs.Find(x => x.Item2 == doublesMMRs.Select(x => x.Item2).Max()).Item1 : triplesMMRs.Find(x => x.Item2 == triplesMMRs.Select(x => x.Item2).Max()).Item1;
                }
            }

            return new MMRStats
            {
                PeakMMR = peakMMR,
                CurrentMMR = currentMMR,
                Salary = ((peakMMR / 50) * 50) / 100.0
            };
        }

        private async Task<(List<(int, DateTime)>, List<(int, DateTime)>)> GetMMRHistory(string platform, string accountName)
        {
            
            var playerID = await _playerRepository.RemoteGetPlayerID(platform, accountName);

            if (string.IsNullOrEmpty(playerID))
            {
                throw new UCLException("Failed fetching player ID for main account");
            }

            return await _playerRepository.RemoteGetPlayerMMRs(playerID);
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
                var league = PlayerHelpers.GetPlayerLeague(player.Salary, _leagueMinSalaries);
                playerDict[league].Add(player);
            }

            return playerDict;
        }
        #endregion
    }
}
