using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UCLBackend.Service.Data.Requests;
using UCLBackend.Service.Interfaces.Services;

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
            await _playerService.AddPlayer(player);
            return Ok();
        }

        [HttpGet]
        [Route("UpdateAllMMRs")]
        public async Task<IActionResult> UpdateAllMMRs()
        {
            await _playerService.UpdateAllMMRs();
            return Ok();
        }
    }
}
