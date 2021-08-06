using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UCLBackend.Service.Data.Requests;
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
        public async Task StartDraft([FromBody] StartDraftRequest request)
        {
            await _draftService.StartDraft(request.IssuerDiscordID, request.League);
        }

        [HttpPost]
        [Route("Draft")]
        public async Task Draft([FromBody] DraftRequest request)
        {
            await _draftService.Draft(request.IssuerDiscordID, request.DiscordID, request.Franchise);
        }

        [HttpPut]
        [Route("NextRound")]
        public async Task NextRound([FromBody] BaseRequest request)
        {
            await _draftService.NextRound(request.IssuerDiscordID);
        }

        [HttpPut]
        [Route("PickSkip")]
        public async Task PickSkip([FromBody] BaseRequest request)
        {
            await _draftService.PickSkip(request.IssuerDiscordID);
        }
    }
}
