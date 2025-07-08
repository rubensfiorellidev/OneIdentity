using MassTransit;
using Microsoft.Extensions.Logging;
using OneID.Application.Commands;
using OneID.Application.DTOs.Admission;
using OneID.Application.Interfaces.Builders;
using OneID.Application.Interfaces.CQRS;
using OneID.Application.Interfaces.Repositories;
using OneID.Application.Interfaces.Services;
using OneID.Application.Messaging.Sagas.Contracts.Events;
using OneID.Domain.Entities.UserContext;
using OneID.Domain.Results;

namespace OneID.Application.CommandHandlers
{
    public class CreateAccountStagingCommandHandler : ICommandHandler<CreateAccountStagingCommand, IResult>
    {
        private readonly IAddUserAccountStagingRepository _repository;
        private readonly IBus _publishEndpoint;
        private readonly ILogger<CreateAccountStagingCommandHandler> _logger;
        private readonly IUserAccountStagingBuilder _builder;
        private readonly IDeduplicationKeyRepository _keyRepository;
        private readonly ISagaDeduplicationRepository _deduplicationRepository;



        public CreateAccountStagingCommandHandler(
            IAddUserAccountStagingRepository repository,
            ILogger<CreateAccountStagingCommandHandler> logger,
            IUserAccountStagingBuilder userAccountStagingBuilder,
            IDeduplicationKeyRepository keyRepository,
            ISagaDeduplicationRepository deduplicationRepository,
            IBus publishEndpoint)
        {
            _repository = repository;
            _logger = logger;
            _builder = userAccountStagingBuilder;
            _keyRepository = keyRepository;
            _deduplicationRepository = deduplicationRepository;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<IResult> Handle(CreateAccountStagingCommand command, CancellationToken cancellationToken)
        {
            var payload = await _builder.BuildAsync(command.Request);

            if (await _keyRepository.ExistsAsync(payload.CpfHash, "create-account", cancellationToken))
            {
                return Result.Conflict("Já existe um processo de admissão em andamento para esse CPF.");
            }

            if (await _deduplicationRepository.ExistsAsync(payload.CorrelationId, cancellationToken))
            {
                return Result.Conflict("Esse CorrelationId já existe.");
            }

            var staging = new AccountPjAdmissionStaging
            {
                CorrelationId = payload.CorrelationId,
                FullName = payload.FullName,
                SocialName = payload.SocialName,
                Cpf = payload.Cpf,
                CpfHash = payload.CpfHash,
                FiscalNumberIdentity = payload.FiscalNumberIdentity,
                FiscalNumberIdentityHash = payload.FiscalNumberIdentityHash,
                StartDate = payload.StartDate,
                ContractorCnpj = payload.ContractorCnpj,
                ContractorName = payload.ContractorName,
                PositionHeldId = payload.PositionHeldId,
                Login = payload.Login,
                LoginHash = payload.LoginHash,
                PersonalEmail = payload.PersonalEmail,
                PersonalEmailHash = payload.PersonalEmailHash,
                CorporateEmail = payload.CorporateEmail,
                CorporateEmailHash = payload.CorporateEmailHash,
                CreatedBy = payload.CreatedBy,
                CreatedAt = DateTimeOffset.UtcNow,
                Status = "Pending"
            };

            await _repository.SaveAsync(staging, cancellationToken);
            await _keyRepository.SaveAsync(payload.CpfHash, "create-account", cancellationToken);
            await _deduplicationRepository.SaveAsync(payload.CorrelationId, "create-account", cancellationToken);

            await _publishEndpoint.Publish(new StartCreateAccountSaga
            {
                CorrelationId = payload.CorrelationId,
                Payload = new KeycloakPayload
                {
                    CorrelationId = payload.CorrelationId,
                    Firstname = payload.FirstName,
                    Lastname = payload.LastName
                }
            }, cancellationToken);

            _logger.LogInformation("Dados salvos em staging e saga iniciada - CorrelationId: {CorrelationId}", payload.CorrelationId);
            return Result.Success("Staging salvo com sucesso.", new { payload.CorrelationId });
        }
    }

}
