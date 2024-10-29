using Microsoft.AspNetCore.Mvc;
using ProjectIpInformation.Services.Interfaces;

namespace ProjectIpInformation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IpInfoController : ControllerBase
    {
        private readonly IIpInfoService _ipInfoService;

        public IpInfoController(IIpInfoService ipInfoService)
        {
            _ipInfoService = ipInfoService;
        }

        [HttpGet("{ip}")]
        public async Task<IActionResult> GetIpInfo(string ip)
        {
            var result = await _ipInfoService.GetIpInfoAsync(ip);
            if (result == null)
            {
                return NotFound("IP information not found");
            }
            return Ok(result);
        }

        [HttpGet("report")]
        public async Task<IActionResult> GetIpReport([FromQuery] string[] countryCodes)
        {
            var report = await _ipInfoService.GetIpReportAsync(countryCodes);
            return Ok(report);
        }
    }
}
