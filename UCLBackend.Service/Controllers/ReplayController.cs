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
    public class ReplayController : ControllerBase
    {
        private readonly IReplayService _replayService;
        private readonly ILogger _logger;

        public ReplayController(IReplayService replayService, ILogger<ReplayController> logger)
        {
            _replayService = replayService;
            _logger = logger;
        }

        [HttpPut]
        [Route("BeginUpload")]
        public IActionResult BeginUpload([FromBody] BaseRequest request)
        {
            try
            {
                _replayService.BeginUploadProcess(request.DiscordID);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error adding player");
                return BadRequest();
            }
        }

        [HttpPut]
        [Route("QueueReplay")]
        public IActionResult QueueReplay([FromBody] QueueReplayRequest request)
        {
            try
            {
                _replayService.QueueReplay(request.DiscordID, request.ReplayFileUrl);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error adding player");
                return BadRequest();
            }
        }

        [HttpPut]
        [Route("EndUpload")]
        public async Task<IActionResult> EndUpload([FromBody] BaseRequest request)
        {
            try
            {
                await _replayService.EndUploadProcess(request.DiscordID);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error adding player");
                return BadRequest();
            }
        }
    }
}
