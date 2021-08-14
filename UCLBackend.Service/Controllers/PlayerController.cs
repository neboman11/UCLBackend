using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UCLBackend.Service.Data.Requests;
using UCLBackend.Service.Data.Responses;
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
        public async Task<IActionResult> AddPlayer([FromBody] AddPlayerRequest request)
        {
            try
            {
                await _playerService.AddPlayer(request.IssuerDiscordID, request.DiscordID, request.PlayerName, request.RLTrackerLink, request.AltRLTrackerLinks);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error adding player");
                return BadRequest(new BaseResponse{HasError = true, ErrorMessage = e.Message});
            }
        }

        [HttpPut]
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
                _logger.LogError(e, "Error updating all MMRs");
                return BadRequest(new BaseResponse{HasError = true, ErrorMessage = e.Message});
            }
        }

        [HttpPut]
        [Route("UpdateSingleMMR")]
        public async Task<IActionResult> UpdateSingleMMR([FromQuery] ulong discordID)
        {
            try
            {
                await _playerService.UpdateSingleMMR(discordID);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error single MMR");
                return BadRequest(new BaseResponse{HasError = true, ErrorMessage = e.Message});
            }
        }

        [HttpPut]
        [Route("SignPlayer")]
        public async Task<IActionResult> SignPlayer([FromBody] SignPlayerRequest request)
        {
            try
            {
                await _playerService.SignPlayer(request.IssuerDiscordID, request.DiscordID, request.FranchiseName);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error signing player");
                return BadRequest(new BaseResponse{HasError = true, ErrorMessage = e.Message});
            }
        }

        [HttpPut]
        [Route("ReleasePlayer")]
        public async Task<IActionResult> ReleasePlayer([FromBody] BaseRequest request)
        {
            try
            {
                await _playerService.ReleasePlayer(request.IssuerDiscordID, request.DiscordID);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error releasing player");
                return BadRequest(new BaseResponse{HasError = true, ErrorMessage = e.Message});
            }
        }

        [HttpPut]
        [Route("PlayerRankout")]
        public async Task<IActionResult> PlayerRankout([FromBody] BaseRequest request)
        {
            try
            {
                await _playerService.PlayerRankout(request.IssuerDiscordID, request.DiscordID);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error rankout player");
                return BadRequest(new BaseResponse{HasError = true, ErrorMessage = e.Message});
            }
        }

        [HttpGet]
        [Route("PlayerInfo")]
        public IActionResult PlayerInfo([FromQuery] ulong id)
        {
            try
            {
                var playerInfo = _playerService.GetPlayerInfo(id);
                return new OkObjectResult(playerInfo);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting player info");
                return BadRequest(new BaseResponse{HasError = true, ErrorMessage = e.Message});
            }
        }

        // TODO: Create route to add alt accounts to a user
        // TODO: Create route to modify main account of user
        // TODO: Create route to change people's name
        // TODO: Create route to remove players from the database who are not in the discord server

        [HttpGet]
        [Route("FreeAgentsList")]
        public async Task<IActionResult> FreeAgentsList()
        {
            try
            {
                await _playerService.FreeAgentsList();
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting free agents list");
                return BadRequest(new BaseResponse{HasError = true, ErrorMessage = e.Message});
            }
        }

        [HttpGet]
        [Route("Roster")]
        public async Task<IActionResult> Roster()
        {
            try
            {
                await _playerService.Roster();
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting roster");
                return BadRequest(new BaseResponse{HasError = true, ErrorMessage = e.Message});
            }
        }

        [HttpGet]
        [Route("GetPlayers")]
        public async Task<IActionResult> GetPlayers()
        {
            try
            {
                var players = await _playerService.GetPlayers();
                return Ok(players);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting all players");
                return BadRequest(new BaseResponse{HasError = true, ErrorMessage = e.Message});
            }
        }

        [HttpGet]
        [Route("GetPlayersByLeague")]
        public async Task<IActionResult> GetPlayersByLeague([FromQuery] string league)
        {
            try
            {
                var players = await _playerService.GetPlayersByLeague(league);
                return Ok(players);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting all players");
                return BadRequest(new BaseResponse{HasError = true, ErrorMessage = e.Message});
            }
        }
    }
}
