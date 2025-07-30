using MediatR;
using Microsoft.AspNetCore.Mvc;
using OneID.Application.Queries.AdmissionQueries;

namespace OneID.Api.Controllers
{
    [Route("v1/pendings")]
    public class AccountStagingController : MainController
    {
        private readonly ILogger<AccountStagingController> _logger;
        public AccountStagingController(ISender sender, ILogger<AccountStagingController> logger) : base(sender)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetPendingsAsync(CancellationToken cancellationToken)
        {
            var query = new GetPendingStagingQuery();
            var result = await Sender.Send(query, cancellationToken);

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
