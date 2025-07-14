using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneID.Application.Commands;
using OneID.Application.DTOs.Admission;
using OneID.Application.Interfaces.CQRS;
using OneID.Application.Interfaces.Repositories;
using OneID.Application.Interfaces.Services;
using OneID.Application.Messaging.Sagas.Contracts.Events;
using OneID.Domain.ValueObjects;
using System.Security.Claims;

#nullable disable
namespace OneID.Api.Controllers
{
    [Route("v1/staging")]
    public class UserAccountController : MainController
    {
        private readonly ILogger<UserAccountController> _logger;
        private readonly IBus _bus;
        private readonly IHashService _hashService;
        private readonly IDeduplicationKeyRepository _keyRepository;
        private readonly IDeduplicationRepository _deduplicationRepository;

        private const string OperatorSecret = "JBSWY3DPEHPK3PXP";



        public UserAccountController(
            ISender send,
            ILogger<UserAccountController> logger,
            IHashService hashService,
            IDeduplicationKeyRepository keyRepository,
            IDeduplicationRepository deduplicationRepository,
            IBus bus) : base(send)
        {
            _logger = logger;
            _hashService = hashService;
            _keyRepository = keyRepository;
            _deduplicationRepository = deduplicationRepository;
            _bus = bus;
        }

        [HttpPost("start-provisioning")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> StartProvisioningAsync([FromBody] SecureProvisioningRequest request, CancellationToken cancellationToken)
        {
            try
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

                var stagingCommand = new CreateAccountStagingCommand(
                    request with { CorrelationId = correlationId },
                    loggedUser
                );

                var stagingResult = await Send.SendAsync(stagingCommand, cancellationToken);

                if (!stagingResult.IsSuccess)
                    return Problem(detail: stagingResult.Message, statusCode: stagingResult.HttpCode ?? 500);

                var enrichedRequest = stagingResult.Data as AccountRequest;

                await _bus.Publish(new AccountDatabaseCreationRequested
                {
                    CorrelationId = correlationId,
                    DatabasePayload = new UserAccountPayload
                    {
                        FirstName = enrichedRequest.FirstName,
                        LastName = enrichedRequest.LastName,
                        FullName = $"{enrichedRequest.FirstName} {enrichedRequest.LastName}",
                        SocialName = enrichedRequest.SocialName,
                        Email = enrichedRequest.PersonalEmail,
                        Password = null,
                        Cpf = enrichedRequest.Cpf,
                        BirthDate = enrichedRequest.BirthDate,
                        StartDate = enrichedRequest.StartDate,
                        Registry = enrichedRequest.Registry,
                        MotherName = enrichedRequest.MotherName,
                        Company = enrichedRequest.Company,
                        Login = enrichedRequest.Login,
                        CorporateEmail = enrichedRequest.CorporateEmail,
                        PersonalEmail = enrichedRequest.PersonalEmail,
                        PhoneNumber = enrichedRequest.PhoneNumber,
                        StatusUserAccount = UserAccountStatus.Inactive,
                        TypeUserAccount = enrichedRequest.TypeUserAccount,
                        LoginManager = enrichedRequest.LoginManager,
                        JobTitleId = enrichedRequest.JobTitleId,
                        FiscalNumberIdentity = enrichedRequest.FiscalNumberIdentity,
                        ContractorCnpj = enrichedRequest.ContractorCnpj,
                        ContractorName = enrichedRequest.ContractorName,
                        CreatedBy = loggedUser
                    }

                }, cancellationToken);

                _logger.LogInformation("Saga de criação de conta iniciada - CorrelationId: {CorrelationId}", correlationId);

                return stagingResult.Match(
                     success => Accepted(new
                     {
                         Message = "Processo iniciado. Acompanhe os eventos de auditoria.",
                         CorrelationId = correlationId
                     }),
                     error => Problem(detail: error.Message, statusCode: error.HttpCode ?? 500)
                );

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao iniciar o processo de provisionamento");
                return Problem(detail: "Erro inesperado ao iniciar o processo", statusCode: 500);
            }
        }
    }

}
