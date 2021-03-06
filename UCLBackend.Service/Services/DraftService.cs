using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UCLBackend.Service.Data.Enums;
using UCLBackend.Service.Data.Exceptions;
using UCLBackend.Service.Services.Interfaces;

namespace UCLBackend.Service.Services
{
    public class DraftService : IDraftService
    {
        private readonly ILogger<DraftService> _logger;

        private Queue<PlayerFranchise> _draftOrder;
        const ulong _draftChannelId = 820892786064359445;
        private PlayerFranchise _currentFranchise;
        private PlayerLeague _currentLeague;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private int _roundCount;
        private bool _draftOccuring = false;

        private Timer _timer;

        public DraftService(ILogger<DraftService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = scopeFactory;
        }

        public async Task StartDraft(ulong issuerDiscordID, PlayerLeague league)
        {
            // Create a timer with a two minute interval.
            _timer = new System.Timers.Timer(120000);
            // Hook up the Elapsed event for the timer. 
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;

            switch (league)
            {
                case PlayerLeague.Origins:
                    _currentLeague = PlayerLeague.Origins;
                    ResetDraftOrder(league);
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var _discordService = scope.ServiceProvider.GetService<IDiscordService>();

                        await _discordService.SendMessage(_draftChannelId, "Starting Origins Draft.");
                    }
                    _currentFranchise = _draftOrder.Dequeue();
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var _discordService = scope.ServiceProvider.GetService<IDiscordService>();

                        await _discordService.SendMessage(_draftChannelId, $"{_currentFranchise} are on the clock, you have 120 seconds to make a selection.");
                    }
                    _timer.Start();
                    break;
                case PlayerLeague.Ultra:
                    _currentLeague = PlayerLeague.Ultra;
                    ResetDraftOrder(league);
                    _roundCount = 2;
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var _discordService = scope.ServiceProvider.GetService<IDiscordService>();

                        await _discordService.SendMessage(_draftChannelId, "Starting Ultra Draft.");
                    }
                    _currentFranchise = _draftOrder.Dequeue();
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var _discordService = scope.ServiceProvider.GetService<IDiscordService>();

                        await _discordService.SendMessage(_draftChannelId, $"{_currentFranchise} are on the clock, you have 120 seconds to make a selection.");
                    }
                    _timer.Start();
                    break;
                case PlayerLeague.Elite:
                    _currentLeague = PlayerLeague.Elite;
                    ResetDraftOrder(league);
                    _roundCount = 2;
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var _discordService = scope.ServiceProvider.GetService<IDiscordService>();

                        await _discordService.SendMessage(_draftChannelId, "Starting Elite Draft.");
                    }
                    _currentFranchise = _draftOrder.Dequeue();
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var _discordService = scope.ServiceProvider.GetService<IDiscordService>();

                        await _discordService.SendMessage(_draftChannelId, $"{_currentFranchise} are on the clock, you have 120 seconds to make a selection.");
                    }
                    _timer.Start();
                    break;
                default:
                    throw new Exception();
            }

            _draftOccuring = true;

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _discordService = scope.ServiceProvider.GetService<IDiscordService>();

                await _discordService.LogTransaction(issuerDiscordID, "Started season draft");
            }
        }

        public async Task Draft(ulong issuerDiscordID, ulong discordID, PlayerFranchise franchise)
        {
            if (!_draftOccuring)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var _discordService = scope.ServiceProvider.GetService<IDiscordService>();

                    await _discordService.SendMessage(_draftChannelId, $"Draft is not currently happening.");
                }
                return;
            }

            if (_currentFranchise != franchise)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var _discordService = scope.ServiceProvider.GetService<IDiscordService>();

                    await _discordService.SendMessage(_draftChannelId, $"{franchise} is not on the clock.");
                }
                return;
            }

            _timer.Stop();

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _playerService = scope.ServiceProvider.GetService<IPlayerService>();

                try
                {
                    _playerService.GetPlayerInfo(discordID);

                    await _playerService.SignPlayer(issuerDiscordID, discordID, franchise.ToString());
                }
                catch (UCLException e)
                {
                    var _discordService = scope.ServiceProvider.GetService<IDiscordService>();

                    await _discordService.SendMessage(_draftChannelId, $"Unable to sign player: {e.Message}.");
                    return;
                }
            }

            switch (_currentLeague)
            {
                case PlayerLeague.Origins:
                    if (_draftOrder.Count == 0)
                    {
                        using (var scope = _serviceScopeFactory.CreateScope())
                        {
                            var _discordService = scope.ServiceProvider.GetService<IDiscordService>();

                            _draftOccuring = false;

                            await _discordService.SendMessage(_draftChannelId, "Origins Draft is over.");
                        }
                        return;
                    }
                    _currentFranchise = _draftOrder.Dequeue();
                    break;
                case PlayerLeague.Ultra:
                    if (_draftOrder.Count == 0)
                    {
                        if (_roundCount == 1)
                        {
                            using (var scope = _serviceScopeFactory.CreateScope())
                            {
                                var _discordService = scope.ServiceProvider.GetService<IDiscordService>();

                                _draftOccuring = false;

                                await _discordService.SendMessage(_draftChannelId, "Ultra Draft is over.");
                            }
                            _roundCount = 0;
                        }
                        else
                        {
                            using (var scope = _serviceScopeFactory.CreateScope())
                            {
                                var _discordService = scope.ServiceProvider.GetService<IDiscordService>();


                                _draftOccuring = false;
                                await _discordService.SendMessage(_draftChannelId, $"Ultra Draft Round 1 is over.");
                            }
                            _roundCount -= 1;
                        }
                        return;
                    }
                    _currentFranchise = _draftOrder.Dequeue();
                    break;
                case PlayerLeague.Elite:
                    if (_draftOrder.Count == 0)
                    {
                        if (_roundCount == 1)
                        {
                            using (var scope = _serviceScopeFactory.CreateScope())
                            {
                                var _discordService = scope.ServiceProvider.GetService<IDiscordService>();

                                _draftOccuring = false;

                                await _discordService.SendMessage(_draftChannelId, "Elite Draft is over.");
                            }
                            _roundCount = 0;
                        }
                        else
                        {
                            using (var scope = _serviceScopeFactory.CreateScope())
                            {
                                var _discordService = scope.ServiceProvider.GetService<IDiscordService>();

                                _draftOccuring = false;

                                await _discordService.SendMessage(_draftChannelId, $"Elite Draft Round 1 is over.");
                            }
                            _roundCount -= 1;
                        }
                        return;
                    }
                    _currentFranchise = _draftOrder.Dequeue();
                    break;
                default:
                    throw new Exception();
            }

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _discordService = scope.ServiceProvider.GetService<IDiscordService>();

                await _discordService.SendMessage(_draftChannelId, $"{_currentFranchise} are on the clock, you have 120 seconds to make a selection.");
            }
            _timer.Start();
        }

        public async Task NextRound(ulong issuerDiscordID)
        {
            if (_roundCount == 0)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var _discordService = scope.ServiceProvider.GetService<IDiscordService>();

                    await _discordService.SendMessage(_draftChannelId, "No draft is awaiting another round.");
                    return;
                }
            }

            switch (_currentLeague)
            {
                case PlayerLeague.Origins:
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var _discordService = scope.ServiceProvider.GetService<IDiscordService>();

                        await _discordService.SendMessage(_draftChannelId, "Origins draft is only 1 round.");
                    }
                    return;
                case PlayerLeague.Ultra:
                    ResetDraftOrder(_currentLeague);
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var _discordService = scope.ServiceProvider.GetService<IDiscordService>();

                        await _discordService.SendMessage(_draftChannelId, "Starting next round of Ultra Draft.");
                    }
                    _currentFranchise = _draftOrder.Dequeue();
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var _discordService = scope.ServiceProvider.GetService<IDiscordService>();

                        await _discordService.SendMessage(_draftChannelId, $"{_currentFranchise} are on the clock, you have 120 seconds to make a selection.");
                    }
                    _timer.Start();
                    break;
                case PlayerLeague.Elite:
                    ResetDraftOrder(_currentLeague);
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var _discordService = scope.ServiceProvider.GetService<IDiscordService>();

                        await _discordService.SendMessage(_draftChannelId, "Starting next round of Elite Draft.");
                    }
                    _currentFranchise = _draftOrder.Dequeue();
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var _discordService = scope.ServiceProvider.GetService<IDiscordService>();

                        await _discordService.SendMessage(_draftChannelId, $"{_currentFranchise} are on the clock, you have 120 seconds to make a selection.");
                    }
                    _timer.Start();
                    break;
                default:
                    throw new Exception();
            }

            _draftOccuring = true;
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _discordService = scope.ServiceProvider.GetService<IDiscordService>();

                _discordService.SendMessage(_draftChannelId, $"{_currentFranchise}, you are out of time for the pick. You will be moved to the end of the round.");
            }
            switch (_currentLeague)
            {
                case PlayerLeague.Origins:
                    _draftOrder.Enqueue(_currentFranchise);
                    _currentFranchise = _draftOrder.Dequeue();
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var _discordService = scope.ServiceProvider.GetService<IDiscordService>();

                        _discordService.SendMessage(_draftChannelId, $"{_currentFranchise} are on the clock, you have 120 seconds to make a selection.");
                    }
                    _timer.Start();
                    break;
                case PlayerLeague.Ultra:
                    _draftOrder.Enqueue(_currentFranchise);
                    _currentFranchise = _draftOrder.Dequeue();
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var _discordService = scope.ServiceProvider.GetService<IDiscordService>();

                        _discordService.SendMessage(_draftChannelId, $"{_currentFranchise} are on the clock, you have 120 seconds to make a selection.");
                    }
                    _timer.Start();
                    break;
                case PlayerLeague.Elite:
                    _draftOrder.Enqueue(_currentFranchise);
                    _currentFranchise = _draftOrder.Dequeue();
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var _discordService = scope.ServiceProvider.GetService<IDiscordService>();

                        _discordService.SendMessage(_draftChannelId, $"{_currentFranchise} are on the clock, you have 120 seconds to make a selection.");
                    }
                    _timer.Start();
                    break;
                default:
                    throw new Exception();
            }
        }

        private void ResetDraftOrder(PlayerLeague league)
        {
            switch (league)
            {
                case PlayerLeague.Origins:
                    _draftOrder = new Queue<PlayerFranchise>(new[] { PlayerFranchise.Bison, PlayerFranchise.Gators, PlayerFranchise.Samurai, PlayerFranchise.Astros, PlayerFranchise.Knights, PlayerFranchise.Vikings, PlayerFranchise.CMM, PlayerFranchise.Spartans });
                    break;
                case PlayerLeague.Ultra:
                    _draftOrder = new Queue<PlayerFranchise>(new[] { PlayerFranchise.Cobras, PlayerFranchise.CMM, PlayerFranchise.Lightning, PlayerFranchise.Samurai, PlayerFranchise.Spartans, PlayerFranchise.Gators, PlayerFranchise.Bison, PlayerFranchise.Astros });
                    break;
                case PlayerLeague.Elite:
                    _draftOrder = new Queue<PlayerFranchise>(new[] { PlayerFranchise.Knights, PlayerFranchise.Cobras, PlayerFranchise.Vikings, PlayerFranchise.Samurai, PlayerFranchise.Gators, PlayerFranchise.Spartans, PlayerFranchise.Lightning, PlayerFranchise.CMM, PlayerFranchise.Bison, PlayerFranchise.Astros });
                    break;
                default:
                    throw new Exception();
            }
        }

        public async Task PickSkip(ulong issuerDiscordID)
        {
            if (!_draftOccuring)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var _discordService = scope.ServiceProvider.GetService<IDiscordService>();

                    await _discordService.SendMessage(_draftChannelId, $"Draft is not currently happening.");
                }
                return;
            }

            _timer.Stop();
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _discordService = scope.ServiceProvider.GetService<IDiscordService>();

                await _discordService.SendMessage(_draftChannelId, $"Skipping pick for {_currentFranchise}.");
            }
            switch (_currentLeague)
            {
                case PlayerLeague.Origins:
                    if (_draftOrder.Count == 0)
                    {
                        using (var scope = _serviceScopeFactory.CreateScope())
                        {
                            var _discordService = scope.ServiceProvider.GetService<IDiscordService>();

                            _draftOccuring = false;

                            await _discordService.SendMessage(_draftChannelId, "Origins Draft is over.");
                        }
                        return;
                    }
                    _currentFranchise = _draftOrder.Dequeue();
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var _discordService = scope.ServiceProvider.GetService<IDiscordService>();

                        await _discordService.SendMessage(_draftChannelId, $"{_currentFranchise} are on the clock, you have 120 seconds to make a selection.");
                    }
                    _timer.Start();
                    break;
                case PlayerLeague.Ultra:
                    if (_draftOrder.Count == 0)
                    {
                        if (_roundCount == 1)
                        {
                            using (var scope = _serviceScopeFactory.CreateScope())
                            {
                                var _discordService = scope.ServiceProvider.GetService<IDiscordService>();

                                _draftOccuring = false;

                                await _discordService.SendMessage(_draftChannelId, "Ultra Draft is over.");
                            }
                            _roundCount = 0;
                        }
                        else
                        {
                            using (var scope = _serviceScopeFactory.CreateScope())
                            {
                                var _discordService = scope.ServiceProvider.GetService<IDiscordService>();

                                _draftOccuring = false;

                                await _discordService.SendMessage(_draftChannelId, $"Ultra Draft Round 1 is over.");
                            }
                            _roundCount -= 1;
                        }
                        return;
                    }
                    _currentFranchise = _draftOrder.Dequeue();
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var _discordService = scope.ServiceProvider.GetService<IDiscordService>();

                        await _discordService.SendMessage(_draftChannelId, $"{_currentFranchise} are on the clock, you have 120 seconds to make a selection.");
                    }
                    _timer.Start();
                    break;
                case PlayerLeague.Elite:
                    if (_draftOrder.Count == 0)
                    {
                        if (_roundCount == 1)
                        {
                            using (var scope = _serviceScopeFactory.CreateScope())
                            {
                                var _discordService = scope.ServiceProvider.GetService<IDiscordService>();

                                _draftOccuring = false;

                                await _discordService.SendMessage(_draftChannelId, "Elite Draft is over.");
                            }
                            _roundCount = 0;
                        }
                        else
                        {
                            using (var scope = _serviceScopeFactory.CreateScope())
                            {
                                var _discordService = scope.ServiceProvider.GetService<IDiscordService>();

                                _draftOccuring = false;

                                await _discordService.SendMessage(_draftChannelId, $"Elite Draft Round 1 is over.");
                            }
                            _roundCount -= 1;
                        }
                        return;
                    }
                    _currentFranchise = _draftOrder.Dequeue();
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var _discordService = scope.ServiceProvider.GetService<IDiscordService>();

                        await _discordService.SendMessage(_draftChannelId, $"{_currentFranchise} are on the clock, you have 120 seconds to make a selection.");
                    }
                    _timer.Start();
                    break;
                default:
                    throw new Exception();
            }
        }
    }
}
