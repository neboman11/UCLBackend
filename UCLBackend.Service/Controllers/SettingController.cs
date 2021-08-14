using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UCLBackend.Service.Data.Responses;
using UCLBackend.Service.DataAccess.Interfaces;

namespace UCLBackend.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SettingController : ControllerBase
    {
        private readonly ILogger<SettingController> _logger;
        private readonly ISettingRepository _settingRepository;

        public SettingController(ILogger<SettingController> logger, ISettingRepository settingRepository)
        {
            _logger = logger;
            _settingRepository = settingRepository;
        }

        [HttpGet]
        [Route("MinSalaries")]
        public async Task<IActionResult> MinSalaries()
        {
            try
            {
                var minSalaries = await Task.Run(() => _settingRepository.GetMinSalaries());
                return Ok(minSalaries);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting min salaries");
                return BadRequest(new BaseResponse { HasError = true, ErrorMessage = e.Message });
            }
        }
    }
}
