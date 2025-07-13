using MassTransit;
using Microsoft.Extensions.Logging;
using OneID.Application.Commands;
using OneID.Application.Interfaces.Builders;
using OneID.Application.Interfaces.Repositories;
using OneID.Application.Interfaces.SensitiveData;
using OneID.Application.Interfaces.Services;
using OneID.Application.Messaging.Sagas.Contracts.Events;
using OneID.Domain.Entities.UserContext;
using OneID.Domain.EventsFromDomain;
using OneID.Domain.Interfaces;

namespace OneID.Application.Messaging.Sagas.Consumers
{
    public sealed class CreateAccountDatabaseConsumer : IConsumer<CreateAccountDatabaseCommand>
    {
        private readonly IUserAccountBuilder _builder;
        private readonly IAddUserAccountRepository _repository;
        private readonly ILogger<CreateAccountDatabaseConsumer> _logger;
        private readonly IHashService _hashService;
        private readonly ISensitiveDataEncryptionServiceUserAccount _encryptionService;
        private readonly IEventDispatcher _dispatcher;
        private readonly IQueryAccountAdmissionStagingRepository _stagingRepository;
        private readonly IAccessPackageClaimService _claimService;
        private readonly IUserClaimWriterRepository _claimWriter;


        public CreateAccountDatabaseConsumer(
            IUserAccountBuilder builder,
            IAddUserAccountRepository repository,
            ILogger<CreateAccountDatabaseConsumer> logger,
            IHashService hashService,
            ISensitiveDataEncryptionServiceUserAccount encryptionService,
            IEventDispatcher dispatcher,
            IQueryAccountAdmissionStagingRepository stagingRepository,
            IAccessPackageClaimService claimService,
            IUserClaimWriterRepository claimWriter)
        {
            _builder = builder;
            _repository = repository;
            _logger = logger;
            _hashService = hashService;
            _encryptionService = encryptionService;
            _dispatcher = dispatcher;
            _stagingRepository = stagingRepository;
            _claimService = claimService;
            _claimWriter = claimWriter;
        }

        public async Task Consume(ConsumeContext<CreateAccountDatabaseCommand> context)
        {
            var correlationId = context.Message.CorrelationId;

            try
            {
                var staging = await _stagingRepository.GetByCorrelationIdAsync(correlationId, context.CancellationToken);
                if (staging is null)
                {
                    await context.Publish(new AccountDatabaseCreationFailed
                    {
                        CorrelationId = correlationId,
                        FaultReason = "Staging não encontrada"

                    });
                    return;
                }

                var userAccount = _builder
                    .WithCorrelationId(correlationId)
                    .WithFirstName(staging.FirstName)
                    .WithLastName(staging.LastName)
                    .WithFullName(staging.FullName)
                    .WithSocialName(staging.SocialName)
                    .WithCpf(staging.Cpf)
                    .WithBirthDate(staging.BirthDate)
                    .WithDateOfHire(staging.StartDate)
                    .WithRegistry(staging.Registry)
                    .WithMotherName(staging.MotherName)
                    .WithCompany(staging.Company)
                    .WithLogin(context.Message.Login)
                    .WithCorporateEmail(context.Message.CorporateEmail)
                    .WithPersonalEmail(staging.PersonalEmail)
                    .WithPhoneNumber(staging.PhoneNumber)
                    .WithStatusUserAccount(staging.StatusUserAccount)
                    .WithTypeUserAccount(staging.TypeUserAccount)
                    .WithLoginManager(staging.LoginManager)
                    .WithJobTitleId(staging.JobTitleId)
                    .WithJobTitle(staging.JobTitleName)
                    .WithDepartmentId(staging.DepartmentId)
                    .WithDepartment(staging.DepartmentName)
                    .WithFiscalNumberIdentity(staging.FiscalNumberIdentity)
                    .WithContractor(staging.ContractorCnpj, staging.ContractorName)
                    .WithCreatedBy(staging.CreatedBy)
                    .WithKeycloakUserId(context.Message.KeycloakUserId)
                    .Build();



                if (!userAccount.IsValid())
                {
                    await context.Publish(new AccountDatabaseCreationFailed
                    {
                        CorrelationId = correlationId,
                        FaultReason = "Dados inválidos"

                    });
                    return;
                }

                var cpfHash = await _hashService.ComputeSha3HashAsync(userAccount.Cpf);
                var emailHash = await _hashService.ComputeSha3HashAsync(userAccount.CorporateEmail);
                var personalEmailHash = await _hashService.ComputeSha3HashAsync(userAccount.PersonalEmail);
                var loginHash = await _hashService.ComputeSha3HashAsync(userAccount.Login);
                var fiscalHash = await _hashService.ComputeSha3HashAsync(userAccount.FiscalNumberIdentity);
                var contractorHash = await _hashService.ComputeSha3HashAsync(userAccount.ContractorCnpj);

                userAccount.ApplyHashes(cpfHash, emailHash, personalEmailHash, loginHash, fiscalHash, contractorHash);

                userAccount = await _encryptionService.EncryptSensitiveDataAsync(userAccount);

                var result = await _repository.AddAsync(userAccount, context.CancellationToken);

                if (!result.IsSuccess)
                {
                    return;
                }

                var resolvedClaims = await _claimService.ResolveClaimsForUserAsync(userAccount, context.CancellationToken);

                if (resolvedClaims.Any())
                {
                    await _claimWriter.AddRangeAsync(resolvedClaims, context.CancellationToken);
                }

                if (!result.IsSuccess)
                {
                    var failedEvent = new UserAccountCreationFailedEvent(
                        DateTimeOffset.UtcNow,
                        userAccount.Id,
                        nameof(UserAccount),
                        userAccount.CreatedBy
                    );

                    userAccount.AddEvent(failedEvent);
                    await _dispatcher.HandleAsync(userAccount.Events, context.CancellationToken);
                    userAccount.ClearEvents();

                    await context.Publish(new AccountDatabaseCreationFailed
                    {
                        CorrelationId = correlationId,
                        FaultReason = result.Message ?? "Falha ao persistir"

                    });
                    return;
                }

                var createdEvent = new UserAccountCreatedEvent(
                    DateTimeOffset.UtcNow,
                    userAccount.Id,
                    nameof(UserAccount),
                    userAccount.CreatedBy
                );

                userAccount.AddEvent(createdEvent);
                await _dispatcher.HandleAsync(userAccount.Events, context.CancellationToken);

                userAccount.ClearEvents();

                await context.Publish(new AccountDatabaseCreated
                {
                    CorrelationId = correlationId,
                    DatabaseId = userAccount.Id,
                    CreatedAt = userAccount.ProvisioningAt
                });

                _logger.LogInformation("UserAccount criado com sucesso - CorrelationId: {CorrelationId}", correlationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao persistir UserAccount - CorrelationId: {CorrelationId}", correlationId);

                await context.Publish(new AccountDatabaseCreationFailed
                {
                    CorrelationId = correlationId,
                    FaultReason = "Erro inesperado"
                });

                throw;
            }
        }
    }

}
