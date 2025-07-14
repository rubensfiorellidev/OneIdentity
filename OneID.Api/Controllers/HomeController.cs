using Microsoft.AspNetCore.Mvc;

namespace OneID.Api.Controllers
{
    [Route("")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : ControllerBase
    {

        [HttpGet("")]
        public IActionResult Home()
        {
            return Ok(CreateSystemInfoResponse());
        }

        [HttpGet("health", Name = "CheckHealth")]
        public IActionResult HealthCheck()
        {
            return Ok(new { status = "Healthy" });
        }
        private static object CreateSystemInfoResponse()
        {
            return new { Environment.Is64BitOperatingSystem };
        }

    }
}
