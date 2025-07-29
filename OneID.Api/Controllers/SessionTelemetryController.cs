using Microsoft.AspNetCore.Mvc;
using OneID.Application.DTOs.ActiveSessions;
using OneID.Application.Interfaces.CQRS;
using OneID.Application.Queries.ActiveSessions;

namespace OneID.Api.Controllers
{
    [Route("internal/sessions")]
    public class SessionTelemetryController : MainController
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly ILogger<SessionTelemetryController> _logger;
        public SessionTelemetryController(ISender send,
                                          IQueryExecutor query,
                                          ILogger<SessionTelemetryController> logger) : base(send)
        {
            _queryExecutor = query;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetSessions(CancellationToken ct)
        {
            var query = new GetActiveSessionsQuery();
            var result = await _queryExecutor
                        .SendQueryAsync<GetActiveSessionsQuery, List<SessionTelemetry>>(query, ct);

            return Ok(result);
        }
    }
}
