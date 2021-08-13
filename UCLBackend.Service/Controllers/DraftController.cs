using System;
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
    public class DraftController : ControllerBase
    {
        private readonly ILogger<DraftController> _logger;
        private readonly IDraftService _draftService;

        public DraftController(ILogger<DraftController> logger, IDraftService draftService)
        {
            _logger = logger;
            _draftService = draftService;
        }

        [HttpPost]
        [Route("StartDraft")]
        public async Task<IActionResult> StartDraft([FromBody] StartDraftRequest request)
        {
            try
            {
                await _draftService.StartDraft(request.IssuerDiscordID, request.League);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting roster");
                return BadRequest(new BaseResponse { HasError = true, ErrorMessage = e.Message });
            }
        }

        [HttpPost]
        [Route("Draft")]
        public async Task<IActionResult> Draft([FromBody] DraftRequest request)
        {
            try
            {
                await _draftService.Draft(request.IssuerDiscordID, request.DiscordID, request.Franchise);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting roster");
                return BadRequest(new BaseResponse { HasError = true, ErrorMessage = e.Message });
            }
        }

        [HttpPut]
        [Route("NextRound")]
        public async Task<IActionResult> NextRound([FromBody] BaseRequest request)
        {
            try
            {
                await _draftService.NextRound(request.IssuerDiscordID);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting roster");
                return BadRequest(new BaseResponse { HasError = true, ErrorMessage = e.Message });
            }
        }

        [HttpPut]
        [Route("PickSkip")]
        public async Task<IActionResult> PickSkip([FromBody] BaseRequest request)
        {
            try
            {
                await _draftService.PickSkip(request.IssuerDiscordID);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting roster");
                return BadRequest(new BaseResponse { HasError = true, ErrorMessage = e.Message });
            }
        }
    }
}
