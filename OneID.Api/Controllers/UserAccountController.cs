using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneID.Application.Commands;
using OneID.Application.DTOs.Admission;
using OneID.Application.Interfaces.CQRS;
using OneID.Application.Interfaces.Interceptor;
using OneID.Application.Interfaces.Repositories;
using OneID.Application.Interfaces.Services;
using OneID.Application.Interfaces.TotpServices;
using OneID.Application.Messaging.Sagas.Contracts.Events;
using OneID.Application.Queries.AdmissionQueries;
using OneID.Domain.Contracts.Jwt;
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
        private readonly ITotpService _otpService;
        private readonly ICurrentUserService _currentUser;
        private readonly IJwtProvider _jwtProvider;
        private readonly IQueryExecutor _queryExecutor;


        public UserAccountController(
            ISender send,
            ILogger<UserAccountController> logger,
            IHashService hashService,
            IDeduplicationKeyRepository keyRepository,
            IDeduplicationRepository deduplicationRepository,
            IQueryAccountAdmissionStagingRepository stagingRepository,
            IBus bus,
            ITotpService otpService,
            ICurrentUserService currentUser,
            IJwtProvider jwtProvider,
            IQueryExecutor queryExecutor) : base(send)
        {
            _logger = logger;
            _hashService = hashService;
            _keyRepository = keyRepository;
            _deduplicationRepository = deduplicationRepository;
            _stagingRepository = stagingRepository;
            _bus = bus;
            _otpService = otpService;
            _currentUser = currentUser;
            _jwtProvider = jwtProvider;
            _queryExecutor = queryExecutor;
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

            var loggedUser = _currentUser.GetUsername();

            var stagingCommand = new CreateAccountStagingCommand(request with { CorrelationId = correlationId }, loggedUser);
            var stagingResult = await Send.SendAsync(stagingCommand, cancellationToken);

            if (!stagingResult.IsSuccess)
                return Problem(detail: stagingResult.Message, statusCode: stagingResult.HttpCode ?? 500);

            _logger.LogInformation("Provisionamento iniciado - CorrelationId: {CorrelationId}", correlationId);

            var requestToken = _jwtProvider.GenerateInitialRequestTokenAsync(loggedUser, correlationId);

            Response.Cookies.Append("request_token", requestToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(5)
            });

            return Accepted(new
            {
                Message = "Provisionamento iniciado. Confirme com o código TOTP.",
                CorrelationId = correlationId,
            });

        }

        [Authorize(AuthenticationSchemes = "RequestToken")]
        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmAsync([FromBody] ProvisioningConfirmationRequest request, CancellationToken cancellationToken)
        {

            var accessScope = User.FindFirst("access_scope")?.Value ?? "unknown";

            if (accessScope is not ("bootstrap_token" or "user_access"))
            {
                _logger.LogWarning("Token rejeitado. Escopo não autorizado para confirmação: {Scope}", accessScope);
                return Forbid("RequestToken");
            }


            if (string.IsNullOrWhiteSpace(request.TotpCode))
                return Unauthorized("Por favor, informe o código de verificação.");

            if (!_otpService.ValidateCode(request.TotpCode))
                return Unauthorized("Código de verificação inválido. Verifique no seu aplicativo autenticador.");


            var staging = await _stagingRepository.GetByCorrelationIdAsync(request.CorrelationId, cancellationToken);
            if (staging is null)
                return NotFound("Staging não encontrada.");

            var loggedUser = User.FindFirst("preferred_username")?.Value
                          ?? User.FindFirst(ClaimTypes.Name)?.Value
                          ?? "unknown";

            await _bus.Publish(new StartCreateAccountSaga
            {
                CorrelationId = request.CorrelationId,
                KeycloakPayload = new KeycloakPayload
                {
                    CorrelationId = request.CorrelationId,
                    FirstName = staging.FirstName,
                    LastName = staging.LastName
                }
            }, cancellationToken);


            _logger.LogInformation("Provisionamento confirmado - CorrelationId: {CorrelationId}", request.CorrelationId);

            var totpToken = _jwtProvider.GenerateUserAccessTokenAsync(loggedUser, request.CorrelationId);

            return Accepted(new
            {
                Message = "Provisionamento iniciado. Verifique seu app autenticador e informe o código de 6 dígitos para continuar.",
                request.CorrelationId
            });

        }

        [HttpPost("resume")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ResumeAsync([FromBody] ResumeRequest request)
        {
            var staging = await _stagingRepository.GetByCorrelationIdAsync(request.CorrelationId);
            if (staging is null)
                return NotFound("Processo não encontrado.");

            if (staging.Status != "PENDING")
                return BadRequest("Esse processo não pode mais ser retomado.");

            var claims = new Dictionary<string, object>
            {
                { "sub", User.Identity?.Name ?? "unknown" },
                { "preferred_username", User.Identity?.Name ?? "unknown" },
                { "correlation_id", request.CorrelationId },
            };

            var token = _jwtProvider.CreateBootstrapToken(claims, TimeSpan.FromMinutes(5));

            return Ok(new
            {
                Message = "Token gerado com sucesso.",
                Token = token,
                request.CorrelationId
            });
        }

        [HttpGet("admissions-recents")]
        public async Task<IActionResult> GetRecentAdmissionsAsync([FromQuery] int limit = 10, CancellationToken cancellationToken = default)
        {
            var query = new GetRecentAdmissionsQuery(limit);
            var result = await _queryExecutor
                .SendQueryAsync<GetRecentAdmissionsQuery, List<RecentAdmissionDto>>(query, cancellationToken);

            _logger.LogInformation("Query retornou {Count} admissões recentes", result?.Count ?? 0);

            foreach (var item in result ?? [])
            {
                _logger.LogDebug(" - {AccountId} | {FullName} | {Department} | {JobTitle} | {ProvisioningAt}",
                    item.AccountId, item.FullName, item.DepartmentName, item.JobTitleName, item.ProvisioningAt);
            }

            return Ok(result);
        }

    }

}
