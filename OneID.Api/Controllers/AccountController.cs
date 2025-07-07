using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneID.Application.DTOs.Admission;
using OneID.Application.Messaging.Sagas.Contracts.Events;

namespace OneID.Api.Controllers
{
    [Route("v1/accounts")]
    public class AccountController : MainController
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IBus _bus;

        public AccountController(
            ISender sender,
            ILogger<AccountController> logger,
            IBus bus) : base(sender)
        {
            _logger = logger;
            _bus = bus;
        }

        [HttpPost("start-provisioning")]
        [Authorize]
        public async Task<IActionResult> StartProvisioningAsync([FromBody] AccountRequest request, CancellationToken cancellationToken)
        {
            try
            {
                if (!ModelState.IsValid)
                    return UnprocessableEntity(ModelState);

                var correlationId = request.CorrelationId == Guid.Empty ? Guid.NewGuid() : request.CorrelationId;

                var payload = new KeycloakPayload
                {
                    CorrelationId = correlationId,
                    Firstname = request.Firstname,
                    Lastname = request.Lastname

                };

                await _bus.Publish(new StartCreateAccountSaga
                {
                    CorrelationId = correlationId,
                    Payload = payload
                }, cancellationToken);

                _logger.LogInformation("Saga de criação de conta iniciada - CorrelationId: {CorrelationId}", correlationId);

                return Accepted(new
                {
                    Message = "Processamento iniciado. Acompanhe os eventos de auditoria.",
                    CorrelationId = correlationId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao iniciar o processo de provisionamento");
                return Problem(detail: "Erro inesperado ao iniciar o processo", statusCode: 500);
            }
        }
    }

}
