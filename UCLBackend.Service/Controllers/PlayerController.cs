using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UCLBackend.Service.Data.Requests;
using UCLBackend.Service.Services.Interfaces;

namespace UCLBackend.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayerController : ControllerBase
    {
        private readonly ILogger<PlayerController> _logger;
        private readonly IPlayerService _playerService;

        public PlayerController(ILogger<PlayerController> logger, IPlayerService playerService)
        {
            _logger = logger;
            _playerService = playerService;
        }

        [HttpPost]
        [Route("AddPlayer")]
        public async Task<IActionResult> AddPlayer([FromBody] AddPlayerRequest player)
        {
            try
            {
                await _playerService.AddPlayer(player);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error adding player");
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("UpdateAllMMRs")]
        public async Task<IActionResult> UpdateAllMMRs()
        {
            try
            {
            await _playerService.UpdateAllMMRs();
            return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating all mmr");
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("SignPlayer")]
        public IActionResult SignPlayer([FromQuery] ulong discordID, [FromQuery] string franchiseName, [FromQuery] string league)
        {
            try
            {
                _playerService.SignPlayer(discordID, franchiseName, league);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error signing player");
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("ReleasePlayer")]
        public async Task<IActionResult> ReleasePlayer([FromQuery] ulong discordID)
        {
            try
            {
                await _playerService.ReleasePlayer(discordID);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error releasing player");
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("PlayerRankout")]
        public IActionResult PlayerRankout([FromQuery] ulong discordID)
        {
            try
            {
                _playerService.PlayerRankout(discordID);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error rankout player");
                return BadRequest();
            }
        }
    }
}
