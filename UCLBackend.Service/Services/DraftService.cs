using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UCLBackend.Service.Data.Enums;
using UCLBackend.Service.Services.Interfaces;

namespace UCLBackend.Service.Services
{
    public class DraftService : IDraftService
    {
        private readonly IDiscordService _discordService;
        private readonly IRedisService _redisService;
        private readonly IPlayerService _playerService;
        private readonly ILogger<DraftService> _logger;

        public DraftService(IDiscordService discordService, IRedisService redisService, IPlayerService playerService, ILogger<DraftService> logger)
        {
            _discordService = discordService;
            _redisService = redisService;
            _playerService = playerService;
            _logger = logger;
        }

        // TODO: If a team doesn't make a pick in time, move them to the back of the round (maybe use a queue for the franchise ordering?)

        public async Task StartDraft(ulong issuerDiscordID)
        {
            await _redisService.StoreValue("draft-remaining-rounds", "6");
            await _redisService.StoreValue("draft-remaining-picks", "12");

            // TODO: Prompt first team to make a pick
            // TODO: Start timer

            await _discordService.LogTransaction(issuerDiscordID, "Started season draft");
        }

        public async Task Draft(ulong issuerDiscordID, ulong discordID, PlayerFranchise franchise)
        {
            // TODO: Make check that given franchise is next to pick
            await _playerService.SignPlayer(issuerDiscordID, discordID, franchise.ToString());

            var remainingPicks = int.Parse(await _redisService.RetrieveValue("draft-remaining-picks"));
            await _redisService.StoreValue("draft-remaining-picks", $"{remainingPicks - 1}");

            // TODO: Prompt next team to make a pick
            // TODO: Start timer
        }

        public async Task NextRound(ulong issuerDiscordID)
        {
            var remainingRounds = int.Parse(await _redisService.RetrieveValue("draft-remaining-rounds"));
            await _redisService.StoreValue("draft-remaining-rounds", $"{remainingRounds - 1}");
            await _redisService.StoreValue("draft-remaining-picks", "12");

            // TODO: Prompt next team to make a pick
            // TODO: Start timer

            await _discordService.LogTransaction(issuerDiscordID, $"Moved to round {7 - remainingRounds}");
        }
    }
}
