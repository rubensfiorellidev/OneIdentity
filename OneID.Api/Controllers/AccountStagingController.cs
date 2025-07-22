using Microsoft.AspNetCore.Mvc;
using OneID.Application.DTOs.Admission;
using OneID.Application.Interfaces.CQRS;
using OneID.Application.Queries.AdmissionQueries;

namespace OneID.Api.Controllers
{
    [Route("v1/pendings")]
    public class AccountStagingController : MainController
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly ILogger<AccountStagingController> _logger;
        public AccountStagingController(ISender send, IQueryExecutor queryDispatcher, ILogger<AccountStagingController> logger) : base(send)
        {
            _queryExecutor = queryDispatcher;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetPendingsAsync(CancellationToken cancellationToken)
        {
            var query = new GetPendingStagingQuery();
            var result = await _queryExecutor
                .SendQueryAsync<GetPendingStagingQuery, List<PendingProcessDto>>(query, cancellationToken);

            _logger.LogInformation("Query retornou {Count} processos pendentes", result?.Count ?? 0);

            foreach (var item in result ?? [])
            {
                _logger.LogDebug(" - {CorrelationId} | {FullName} | {CreatedAt}",
                    item.CorrelationId, item.FullName, item.CreatedAt);
            }

            return Ok(result);
        }

    }
}
