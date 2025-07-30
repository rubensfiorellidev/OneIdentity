using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneID.Application.DTOs.ActiveSessions;
using OneID.Application.Interfaces.CQRS;
using OneID.Application.Queries.ActiveSessions;

namespace OneID.Api.Controllers
{
    [Route("internal/sessions")]
    public class SessionsController : MainController
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly ILogger<SessionsController> _logger;
        public SessionsController(ISender send,
                                          IQueryExecutor query,
                                          ILogger<SessionsController> logger) : base(send)
        {
            _queryExecutor = query;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetSessions(CancellationToken ct)
        {
            var query = new GetActiveSessionsQuery();
            var result = await _queryExecutor
                        .SendQueryAsync<GetActiveSessionsQuery, List<ActiveSessionInfo>>(query, ct);

            return Ok(result);
        }
    }
}
