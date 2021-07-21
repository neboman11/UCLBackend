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
        [Route("SignPlayer")]
        public async Task<IActionResult> SignPlayer([FromBody] SignPlayerRequest request)
        {
            try
            {
                await _playerService.SignPlayer(request);
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
                await _playerService.ReleasePlayer(request);
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
                await _playerService.PlayerRankout(request);
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
    }
}
