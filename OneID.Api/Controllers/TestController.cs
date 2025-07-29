using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OneID.Application.DTOs.ActiveSessions;
using OneID.Application.Interfaces.CQRS;
using OneID.Application.Interfaces.SES;
using StackExchange.Redis;

#nullable disable
namespace OneID.Api.Controllers
{
    [Route("v1/tests")]
    [ApiController]
    public class TestController : MainController
    {
        private readonly ISesEmailSender _emailSender;
        private readonly IConnectionMultiplexer _redis;

        public TestController(ISender dispatcher, ISesEmailSender emailSender, IConnectionMultiplexer redis) : base(dispatcher)
        {
            _emailSender = emailSender;
            _redis = redis;
        }

        [HttpPost("test")]
        public async Task<IActionResult> SendTestEmail([FromQuery] string to)
        {

            await _emailSender.SendEmailAsync(
            to,
            "Usuário pronto",
            textBody: "Provisionamento no Keycloak concluído. Acesse: https://oneidsecure.com/continuar"
            );


            return Ok("Email enviado com sucesso.");
        }

        [HttpGet("ping-redis")]
        public async Task<IActionResult> PingRedis()
        {
            var db = _redis.GetDatabase();
            var pong = await db.PingAsync(); // usa a versão async

            return Ok($"Redis respondeu em {pong.TotalMilliseconds}ms");
        }

        [HttpPost("seed-session")]
        public async Task<IActionResult> SeedSession()
        {
            var db = _redis.GetDatabase();

            var now = DateTime.UtcNow;
            var sessions = new List<SessionTelemetry>
            {
                new(Ulid.NewUlid().ToString(), "192.168.0.1", "rubens@oneid.cloud", now, now.AddMinutes(15)),
                new(Ulid.NewUlid().ToString(), "192.168.0.2", "maria@oneid.cloud", now.AddMinutes(-2), now.AddMinutes(10)),
                new(Ulid.NewUlid().ToString(), "192.168.0.3", "carlos@oneid.cloud", now.AddMinutes(-5), now.AddMinutes(5))
            };

            foreach (var session in sessions)
            {
                var key = $"session:{session.CircuitId}";
                var value = JsonConvert.SerializeObject(session);
                await db.StringSetAsync(key, value, TimeSpan.FromMinutes(20));
            }

            return Ok("3 mock sessions criadas com sucesso.");
        }


    }
}
