using Microsoft.AspNetCore.Mvc;
using OneID.Data.DataContexts;
using StackExchange.Redis;

namespace OneID.Api.Controllers
{
    [Route("")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : ControllerBase
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly OneDbContext _db;

        public HomeController(IConnectionMultiplexer redis, OneDbContext db)
        {
            _redis = redis;
            _db = db;
        }

        [HttpGet("")]
        public IActionResult Home()
        {
            return Ok(CreateSystemInfoResponse());
        }

        [HttpGet("health", Name = "CheckHealth")]
        public async Task<IActionResult> HealthCheck(CancellationToken ct)
        {
            var redisOk = false;
            var dbOk = false;

            try
            {
                var dbRedis = _redis.GetDatabase();
                var ping = await dbRedis.PingAsync();
                redisOk = ping.TotalMilliseconds < 100;
            }
            catch { /* redisOk continua false */ }

            try
            {
                dbOk = await _db.Database.CanConnectAsync(ct);
            }
            catch { /* dbOk continua false */ }

            var healthy = redisOk && dbOk;

            return healthy
                ? Ok(new { status = "Healthy", redis = "Up", database = "Up" })
                : StatusCode(503, new
                {
                    status = "Degraded",
                    redis = redisOk ? "Up" : "Down",
                    database = dbOk ? "Up" : "Down"
                });
        }

        private static object CreateSystemInfoResponse()
        {
            return new
            {
                Environment.Is64BitOperatingSystem,
                Environment.MachineName,
                OS = Environment.OSVersion.VersionString,
                Uptime = (DateTime.UtcNow - System.Diagnostics.Process.GetCurrentProcess().StartTime.ToUniversalTime()).ToString("g")
            };
        }
    }
}
