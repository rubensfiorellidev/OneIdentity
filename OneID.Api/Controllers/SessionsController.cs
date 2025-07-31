using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneID.Application.Queries.ActiveSessions;

namespace OneID.Api.Controllers
{
    [Route("internal/sessions")]
    public class SessionsController : MainController
    {
        private readonly ILogger<SessionsController> _logger;

        public SessionsController(ISender sender, ILogger<SessionsController> logger) : base(sender)
        {
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetSessions(CancellationToken ct)
        {
            var querySessions = new GetActiveSessionsQuery();

            var result = await Sender.Send(querySessions, ct);

            return Ok(result);
        }
    }
}
