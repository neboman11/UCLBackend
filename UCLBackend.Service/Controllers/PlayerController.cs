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
                return BadRequest(new BaseResponse { HasError = true, ErrorMessage = e.Message });
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
                return BadRequest(new BaseResponse { HasError = true, ErrorMessage = e.Message });
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
                return BadRequest(new BaseResponse { HasError = true, ErrorMessage = e.Message });
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
                return BadRequest(new BaseResponse { HasError = true, ErrorMessage = e.Message });
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
                return BadRequest(new BaseResponse { HasError = true, ErrorMessage = e.Message });
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
                return BadRequest(new BaseResponse { HasError = true, ErrorMessage = e.Message });
            }
        }

        [HttpGet]
        [Route("PlayerInfo")]
        public IActionResult PlayerInfo([FromQuery] ulong id)
        {
            try
            {
                var playerInfo = _playerService.GetPlayerInfo(id);
                return Ok(playerInfo);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting player info");
                return BadRequest(new BaseResponse { HasError = true, ErrorMessage = e.Message });
            }
        }

        [HttpPost]
        [Route("AltAccount")]
        public async Task<IActionResult> AddAltAccount([FromBody] AccountRequest request)
        {
            try
            {
                await _playerService.AddAltAccount(request.IssuerDiscordID, request.DiscordID, request.RLTrackerLink);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating alt account");
                return BadRequest(new BaseResponse { HasError = true, ErrorMessage = e.Message });
            }
        }

        [HttpPut]
        [Route("MainAccount")]
        public async Task<IActionResult> ChangeMainAccount([FromBody] AccountRequest request)
        {
            try
            {
                await _playerService.ChangeMainAccount(request.IssuerDiscordID, request.DiscordID, request.RLTrackerLink);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error changing main account");
                return BadRequest(new BaseResponse { HasError = true, ErrorMessage = e.Message });
            }
        }

        [HttpPut]
        [Route("PlayerName")]
        public async Task<IActionResult> ChangePlayerName([FromBody] ChangePlayerNameRequest request)
        {
            try
            {
                await _playerService.ChangePlayerName(request.IssuerDiscordID, request.DiscordID, request.NewName);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error changing player name");
                return BadRequest(new BaseResponse { HasError = true, ErrorMessage = e.Message });
            }
        }

        [HttpDelete]
        [Route("MissingServerPlayers")]
        public async Task<IActionResult> RemoveMissingServerPlayers()
        {
            try
            {
                await _playerService.RemoveMissingServerPlayers();
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error removing missing server players");
                return BadRequest(new BaseResponse { HasError = true, ErrorMessage = e.Message });
            }
        }

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
                return BadRequest(new BaseResponse { HasError = true, ErrorMessage = e.Message });
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
                return BadRequest(new BaseResponse { HasError = true, ErrorMessage = e.Message });
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
                return BadRequest(new BaseResponse { HasError = true, ErrorMessage = e.Message });
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
                return BadRequest(new BaseResponse { HasError = true, ErrorMessage = e.Message });
            }
        }
    }
}
