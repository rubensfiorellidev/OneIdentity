using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneID.Application.Commands;
using OneID.Application.DTOs.Admission;
using OneID.Application.Interfaces.CQRS;
using OneID.Application.Interfaces.Repositories;
using OneID.Application.Interfaces.Services;
using OtpNet;
using System.Security.Claims;

#nullable disable
namespace OneID.Api.Controllers
{
    [Route("v1/staging")]
    [ApiController]
    public class UserAccountController : MainController
    {
        private readonly ILogger<UserAccountController> _logger;
        private readonly IBus _bus;
        private readonly IHashService _hashService;
        private readonly IDeduplicationKeyRepository _keyRepository;
        private readonly IDeduplicationRepository _deduplicationRepository;
        private readonly IQueryAccountAdmissionStagingRepository _stagingRepository;

        private const string OperatorSecret = "JBSWY3DPEHPK3PXP";

        public UserAccountController(
            ISender send,
            ILogger<UserAccountController> logger,
            IHashService hashService,
            IDeduplicationKeyRepository keyRepository,
            IDeduplicationRepository deduplicationRepository,
            IQueryAccountAdmissionStagingRepository stagingRepository,
            IBus bus) : base(send)
        {
            _logger = logger;
            _hashService = hashService;
            _keyRepository = keyRepository;
            _deduplicationRepository = deduplicationRepository;
            _stagingRepository = stagingRepository;
            _bus = bus;
        }

        /// <summary>
        /// Etapa 1 - Inicia o provisionamento e grava na tabela de staging.
        /// </summary>
        [HttpPost("start")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> StartAsync([FromBody] SecureProvisioningRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            var correlationId = Guid.NewGuid();
            var cpfHash = await _hashService.ComputeSha3HashAsync(request.Cpf);

            if (await _keyRepository.ExistsAsync(cpfHash, "create-account", cancellationToken))
            {
                return Conflict(new
                {
                    Message = "Já existe um processo de admissão em andamento para esse CPF.",
                    cpfHash
                });
            }

            if (await _deduplicationRepository.ExistsAsync(correlationId, cancellationToken))
            {
                return Conflict(new
                {
                    Message = "Esse CorrelationId já existe.",
                    correlationId
                });
            }

            await _keyRepository.SaveAsync(cpfHash, "create-account-clt", cancellationToken);
            await _deduplicationRepository.SaveAsync(correlationId, "create-account-clt", cancellationToken);

            var loggedUser = User.FindFirst("preferred_username")?.Value
                          ?? User.FindFirst(ClaimTypes.Name)?.Value
                          ?? "unknown";

            var stagingCommand = new CreateAccountStagingCommand(request with { CorrelationId = correlationId }, loggedUser);
            var stagingResult = await Send.SendAsync(stagingCommand, cancellationToken);

            if (!stagingResult.IsSuccess)
                return Problem(detail: stagingResult.Message, statusCode: stagingResult.HttpCode ?? 500);

            _logger.LogInformation("Provisionamento iniciado - CorrelationId: {CorrelationId}", correlationId);

            return Accepted(new
            {
                Message = "Provisionamento iniciado. Confirme com o código TOTP.",
                CorrelationId = correlationId
            });
        }

        /// <summary>
        /// Etapa 2 - Confirma o provisionamento e publica o evento para a saga.
        /// </summary>
        [HttpPost("confirm")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ConfirmAsync([FromBody] ProvisioningConfirmationRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.TotpCode))
                return Unauthorized("Por favor, informe o código de verificação.");

            var totp = new Totp(Base32Encoding.ToBytes(OperatorSecret));
            if (!totp.VerifyTotp(request.TotpCode, out _, VerificationWindow.RfcSpecifiedNetworkDelay))
                return Unauthorized("Código de verificação inválido. Verifique no seu aplicativo autenticador.");

            var staging = await _stagingRepository.GetByCorrelationIdAsync(request.CorrelationId, cancellationToken);
            if (staging is null)
                return NotFound("Staging não encontrada.");

            var loggedUser = User.FindFirst("preferred_username")?.Value
                          ?? User.FindFirst(ClaimTypes.Name)?.Value
                          ?? "unknown";

            var command = new CreateAccountDatabaseCommand
            {
                CorrelationId = request.CorrelationId,
                CorporateEmail = staging.CorporateEmail,
                Login = staging.Login,
                KeycloakUserId = Guid.Parse(request.KeycloakUserId)
            };

            await _bus.Publish(command, cancellationToken);

            _logger.LogInformation("Provisionamento confirmado - CorrelationId: {CorrelationId}", request.CorrelationId);

            return Accepted(new
            {
                Message = "Provisionamento iniciado. Verifique seu app autenticador e informe o código de 6 dígitos para continuar.",
                request.CorrelationId
            });

        }
    }

}
