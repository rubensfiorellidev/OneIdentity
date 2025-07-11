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
        private readonly IDeduplicationRepository _deduplicationRepository;

        public CreateAccountStagingCommandHandler(
            IAddUserAccountStagingRepository repository,
            ILogger<CreateAccountStagingCommandHandler> logger,
            IUserAccountStagingBuilder userAccountStagingBuilder,
            IDeduplicationKeyRepository keyRepository,
            IDeduplicationRepository deduplicationRepository,
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
            var request = await _builder.BuildAsync(command.Request);

            if (await _keyRepository.ExistsAsync(request.CpfHash, "create-account", cancellationToken))
            {
                return Result.Conflict("Já existe um processo de admissão em andamento para esse CPF.");
            }

            if (await _deduplicationRepository.ExistsAsync(request.CorrelationId, cancellationToken))
            {
                return Result.Conflict("Esse CorrelationId já existe.");
            }

            var staging = new AccountAdmissionStaging
            {
                CorrelationId = request.CorrelationId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                FullName = request.FullName,
                SocialName = request.SocialName,
                Cpf = request.Cpf,
                CpfHash = request.CpfHash,
                FiscalNumberIdentity = request.FiscalNumberIdentity,
                FiscalNumberIdentityHash = request.FiscalNumberIdentityHash,
                StartDate = request.StartDate,
                ContractorCnpj = request.ContractorCnpj,
                ContractorName = request.ContractorName,
                JobTitleId = request.JobTitleId,
                Login = request.Login,
                LoginHash = request.LoginHash,
                PersonalEmail = request.PersonalEmail,
                PersonalEmailHash = request.PersonalEmailHash,
                CorporateEmail = request.CorporateEmail,
                CorporateEmailHash = request.CorporateEmailHash,
                BirthDate = request.BirthDate,
                Company = request.Company,
                MotherName = request.MotherName,
                Registry = request.Registry,
                LoginManager = request.LoginManager,

                CreatedBy = request.CreatedBy,
                CreatedAt = DateTimeOffset.UtcNow,
                Status = "Pending"
            };

            await _repository.SaveAsync(staging, cancellationToken);
            await _keyRepository.SaveAsync(request.CpfHash, "create-account", cancellationToken);
            await _deduplicationRepository.SaveAsync(request.CorrelationId, "create-account", cancellationToken);

            await _publishEndpoint.Publish(new StartCreateAccountSaga
            {
                CorrelationId = request.CorrelationId,
                KeycloakPayload = new KeycloakPayload
                {
                    CorrelationId = request.CorrelationId,
                    FirstName = request.FirstName,
                    LastName = request.LastName
                }
            }, cancellationToken);

            _logger.LogInformation("Dados salvos em staging e saga iniciada - CorrelationId: {CorrelationId}", request.CorrelationId);
            return Result.Success("Staging salvo com sucesso.", request);
        }
    }

}
