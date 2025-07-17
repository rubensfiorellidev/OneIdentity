using MassTransit;
using Microsoft.Extensions.Logging;
using OneID.Application.Commands;
using OneID.Application.DTOs.Admission;
using OneID.Application.Messaging.Sagas.Contracts;
using OneID.Application.Messaging.Sagas.Contracts.Events;
using OneID.Domain.Helpers;

#nullable disable
namespace OneID.Application.Messaging.Sagas.StatesMachines
{
    public sealed class AccountStateMachine : MassTransitStateMachine<AccountSagaState>
    {
        private readonly ILogger<AccountStateMachine> _logger;

        public State WaitingUserAccountPersistence { get; private set; }
        public State WaitingLoginResult { get; private set; }
        public State WaitingCpfValidation { get; private set; }
        public State WaitingKeycloakCreation { get; private set; }
        public State WaitingDatabaseResult { get; private set; }
        public State WaitingAzureResult { get; private set; }
        public State Completed { get; private set; }
        public State Faulted { get; private set; }

        public Event<StartCreateAccountSaga> StartSaga { get; private set; }
        public Event<LoginCreated> LoginCreated { get; private set; }
        public Event<LoginFailed> LoginFailed { get; private set; }
        public Event<AccountCpfValidated> CpfValidated { get; private set; }
        public Event<AccountCpfValidationFailed> CpfValidationFailed { get; private set; }
        public Event<AccountDatabaseCreated> DatabaseCreated { get; private set; }
        public Event<AccountDatabaseCreationFailed> DatabaseFailed { get; private set; }
        public Event<KeycloakUserCreated> KeycloakUserCreated { get; private set; }
        public Event<KeycloakUserFailed> KeycloakUserFailed { get; private set; }
        public Event<AccountSagaFailed> SagaFailed { get; private set; }
        public Event<UserProfilePersisted> UserProfilePersisted { get; private set; }
        public Event<UserProfilePersistenceFailed> UserProfilePersistenceFailed { get; private set; }
        public Event<AzureUserCreated> AzureUserCreated { get; private set; }
        public Event<AzureUserCreationFailed> AzureUserCreationFailed { get; private set; }
        public Event<AzureUserCreationRequested> AzureUserCreationRequested { get; private set; }



        public AccountStateMachine(ILogger<AccountStateMachine> logger)
        {
            _logger = logger;

            InstanceState(x => x.CurrentState);

            Event(() => StartSaga, x => x.CorrelateById(m => m.Message.CorrelationId).SelectId(m => m.Message.CorrelationId));
            Event(() => LoginCreated, x => x.CorrelateById(m => m.Message.CorrelationId));
            Event(() => LoginFailed, x => x.CorrelateById(m => m.Message.CorrelationId));
            Event(() => KeycloakUserCreated, x => x.CorrelateById(m => m.Message.CorrelationId));
            Event(() => KeycloakUserFailed, x => x.CorrelateById(m => m.Message.CorrelationId));
            Event(() => SagaFailed, x => x.CorrelateById(m => m.Message.CorrelationId));
            Event(() => UserProfilePersisted, x => x.CorrelateById(m => m.Message.CorrelationId));
            Event(() => UserProfilePersistenceFailed, x => x.CorrelateById(m => m.Message.CorrelationId));
            Event(() => CpfValidated, x => x.CorrelateById(m => m.Message.CorrelationId));
            Event(() => CpfValidationFailed, x => x.CorrelateById(m => m.Message.CorrelationId));
            Event(() => DatabaseCreated, x => x.CorrelateById(m => m.Message.CorrelationId));
            Event(() => DatabaseFailed, x => x.CorrelateById(m => m.Message.CorrelationId));
            Event(() => AzureUserCreationRequested, x => x.CorrelateById(m => m.Message.CorrelationId));
            Event(() => AzureUserCreated, x => x.CorrelateById(m => m.Message.CorrelationId));
            Event(() => AzureUserCreationFailed, x => x.CorrelateById(m => m.Message.CorrelationId));


            Initially(
                When(StartSaga)
                    .Then(context =>
                    {
                        context.Saga.CorrelationId = context.Message.CorrelationId;
                        context.Saga.CreatedAt = DateTimeOffset.UtcNow;
                        context.Saga.KeycloakData = context.Message.KeycloakPayload;
                    })
                    .Publish(context => new AdmissionAuditRequested
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        FirstName = context.Saga.KeycloakData.FirstName,
                        LastName = context.Saga.KeycloakData.LastName,
                        CurrentState = nameof(WaitingLoginResult),
                        EventName = "Admissão iniciada",
                        Description = "Processo iniciado. Aguardando criação do login.",
                        ProvisioningDate = DateTimeOffset.UtcNow,
                        Login = "NotAvailable",
                        DatabaseId = "NotAvailable"
                    })
                    .Publish(context => new CreateLoginRequested
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        FirstName = context.Saga.KeycloakData.FirstName,
                        LastName = context.Saga.KeycloakData.LastName

                    })
                    .TransitionTo(WaitingLoginResult)

            );

            During(WaitingLoginResult,
                When(LoginCreated)
                    .Then(context =>
                    {
                        var pwd = PasswordTempGenerator.GenerateTemporaryPassword();
                        var corporateEmail = $"{context.Message.Login}@oneidsecure.com";

                        context.Saga.KeycloakData = context.Saga.KeycloakData with
                        {
                            Username = context.Message.Login,
                            Password = pwd,
                            Email = corporateEmail

                        };

                        context.Saga.AccountData = new UserAccountPayload
                        {
                            Login = context.Message.Login,
                            CorporateEmail = corporateEmail
                        };

                    })
                    .Publish(context => new AdmissionAuditRequested
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        FirstName = context.Saga.KeycloakData.FirstName,
                        LastName = context.Saga.KeycloakData.LastName,
                        CurrentState = nameof(WaitingLoginResult),
                        EventName = "Login provisionado",
                        Description = "Login foi provisionado no sistema",
                        ProvisioningDate = DateTimeOffset.UtcNow,
                        Login = context.Message.Login ?? "NotAvailable",
                        DatabaseId = "NotAvailable"
                    })
                   .Publish(context => new KeycloakUserCreationRequested
                   {
                       CorrelationId = context.Saga.CorrelationId,
                       Payload = context.Saga.KeycloakData
                   })
                   .TransitionTo(WaitingKeycloakCreation),

                When(LoginFailed)
                    .Then(context =>
                    {
                        context.Saga.FaultReason = context.Message.FaultReason;
                        _logger.LogError("Falha ao gerar login: {Reason}", context.Message.FaultReason);
                    })
                    .Publish(context => new AdmissionAuditRequested
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        FirstName = context.Saga.KeycloakData.FirstName,
                        LastName = context.Saga.KeycloakData.LastName,
                        CurrentState = nameof(Faulted),
                        EventName = "Falha na geração do login",
                        Description = $"Erro: {context.Message.FaultReason}",
                        ProvisioningDate = DateTimeOffset.UtcNow,
                        Login = "NotAvailable",
                        DatabaseId = "NotAvailable"
                    })
                    .TransitionTo(Faulted)
            );

            During(WaitingKeycloakCreation,
                When(KeycloakUserCreated)
                    .Then(context =>
                    {
                        context.Saga.KeycloakData.Username = context.Message.KeycloakPayload.Username;
                        context.Saga.KeycloakData.Password = context.Message.KeycloakPayload.Password;
                        context.Saga.KeycloakData.Email = context.Message.KeycloakPayload.Email;
                        context.Saga.KeycloakData.KeycloakUserId = context.Message.KeycloakPayload.KeycloakUserId;

                    })
                    .Publish(context => new AdmissionAuditRequested
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        FirstName = context.Saga.KeycloakData.FirstName,
                        LastName = context.Saga.KeycloakData.LastName,
                        CurrentState = nameof(WaitingCpfValidation),
                        EventName = "Usuário provisionado no Keycloak",
                        Description = "Usuário provisionado com sucesso no Keycloak. Iniciando validação do CPF.",
                        ProvisioningDate = DateTimeOffset.UtcNow,
                        Login = context.Saga.KeycloakData.Username ?? "NotAvailable",
                        DatabaseId = "NotAvailable"
                    })
                    .Publish(context => new AccountCpfValidationRequested
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        Cpf = context.Message.Cpf,
                        FullName = context.Message.FullName,
                        JobTitleId = context.Message.JobTitleId
                    })
                    .TransitionTo(WaitingCpfValidation),

                    When(KeycloakUserFailed)
                          .Then(context =>
                          {
                              context.Saga.FaultReason = context.Message.FaultReason;
                          })
                          .Publish(context => new AdmissionAuditRequested
                          {
                              CorrelationId = context.Saga.CorrelationId,
                              FirstName = context.Saga.KeycloakData.FirstName,
                              LastName = context.Saga.KeycloakData.LastName,
                              CurrentState = nameof(Faulted),
                              EventName = "Falha no Keycloak",
                              Description = $"Erro: {context.Message.FaultReason}",
                              ProvisioningDate = DateTimeOffset.UtcNow,
                              Login = context.Saga.KeycloakData.Username,
                              DatabaseId = "NotAvailable"
                          })
                          .TransitionTo(Faulted)
            );

            During(WaitingCpfValidation,
                When(CpfValidated)
                    .Publish(context => new AdmissionAuditRequested
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        FirstName = context.Saga.KeycloakData.FirstName,
                        LastName = context.Saga.KeycloakData.LastName,
                        CurrentState = nameof(WaitingDatabaseResult),
                        EventName = "CPF validado",
                        Description = "CPF validado com sucesso. Iniciando criação da conta no sistema.",
                        ProvisioningDate = DateTimeOffset.UtcNow,
                        Login = context.Saga.KeycloakData.Username,
                        DatabaseId = "InProcess"
                    })
                    .Publish(context => new CreateAccountDatabaseCommand
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        Login = context.Saga.KeycloakData.Username,
                        KeycloakUserId = context.Saga.KeycloakData.KeycloakUserId,
                        CorporateEmail = context.Saga.AccountData.CorporateEmail
                    })
                    .TransitionTo(WaitingDatabaseResult),

                When(CpfValidationFailed)
                    .Then(context => context.Saga.FaultReason = context.Message.FaultReason)
                    .Publish(context => new AdmissionAuditRequested
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        CurrentState = nameof(Faulted),
                        EventName = "Falha na validação do CPF",
                        Description = $"Não foi possível validar o CPF: {context.Message.FaultReason}",
                        ProvisioningDate = DateTimeOffset.UtcNow,
                        Login = context.Saga.KeycloakData.Username,
                        DatabaseId = "NotAvailable"
                    })
                    .TransitionTo(Faulted)
            );

            During(WaitingDatabaseResult,
                When(DatabaseCreated)
                    .Then(context => context.Saga.DatabaseId = context.Message.DatabaseId)
                    .Publish(context => new AdmissionAuditRequested
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        FirstName = context.Saga.KeycloakData.FirstName,
                        LastName = context.Saga.KeycloakData.LastName,
                        CurrentState = nameof(WaitingAzureResult),
                        EventName = "UserAccount provisionado",
                        Description = $"User account provisionado - ID {context.Message.DatabaseId}, Iniciando o provisionamento no Entra ID",
                        ProvisioningDate = DateTimeOffset.UtcNow,
                        Login = context.Saga.KeycloakData.Username,
                        DatabaseId = context.Message.DatabaseId
                    })
                    .Publish(context => new AzureUserCreationRequested
                    {

                        CorrelationId = context.Saga.CorrelationId,
                        FirstName = context.Saga.KeycloakData.FirstName,
                        LastName = context.Saga.KeycloakData.LastName,
                        Login = context.Saga.KeycloakData.Username,
                        Email = context.Saga.AccountData.CorporateEmail,
                        Password = context.Saga.AccountData.Password,
                        ManagerLogin = context.Saga.AccountData.LoginManager

                    })
                    .TransitionTo(WaitingAzureResult),

                When(DatabaseFailed)
                    .Then(context => context.Saga.FaultReason = context.Message.FaultReason)
                    .Publish(context => new AdmissionAuditRequested
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        FirstName = context.Saga.KeycloakData.FirstName,
                        LastName = context.Saga.KeycloakData.LastName,
                        CurrentState = nameof(Faulted),
                        EventName = "Falha na criação da conta",
                        Description = $"Erro: {context.Message.FaultReason}",
                        ProvisioningDate = DateTimeOffset.UtcNow,
                        Login = context.Saga.KeycloakData.Username,
                        DatabaseId = "NotAvailable"
                    })
                    .TransitionTo(Faulted)
            );

            During(WaitingAzureResult,
                When(AzureUserCreated)
                    .Then(context =>
                    {
                        _logger.LogInformation("Usuário {Username} criado com sucesso no Entra ID", context.Saga.KeycloakData.Username);
                    })
                    .Publish(context => new AdmissionAuditRequested
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        FirstName = context.Saga.KeycloakData.FirstName,
                        LastName = context.Saga.KeycloakData.LastName,
                        CurrentState = nameof(Completed),
                        EventName = "Usuário criado no Entra ID",
                        Description = "Usuário sincronizado com sucesso no Entra ID.",
                        ProvisioningDate = DateTimeOffset.UtcNow,
                        Login = context.Saga.KeycloakData.Username,
                        DatabaseId = context.Saga.DatabaseId
                    })
                    .TransitionTo(Completed),

                When(AzureUserCreationFailed)
                    .Then(context =>
                    {
                        context.Saga.FaultReason = context.Message.FaultReason;
                        _logger.LogError("Falha ao criar usuário no Entra ID: {Reason}", context.Message.FaultReason);
                    })
                    .Publish(context => new AdmissionAuditRequested
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        FirstName = context.Saga.KeycloakData.FirstName,
                        LastName = context.Saga.KeycloakData.LastName,
                        CurrentState = nameof(Faulted),
                        EventName = "Erro no Entra ID",
                        Description = $"Falha ao provisionar usuário no Entra ID: {context.Message.FaultReason}",
                        ProvisioningDate = DateTimeOffset.UtcNow,
                        Login = context.Saga.KeycloakData.Username,
                        DatabaseId = context.Saga.DatabaseId
                    })
                    .TransitionTo(Faulted)
            );

            SetCompletedWhenFinalized();

            During(Completed,
                When(SagaFailed)
                    .Then(context =>
                    {
                        _logger.LogWarning("Saga {CorrelationId} já estava finalizada, mas recebeu evento SagaFailed.", context.Saga.CorrelationId);
                    })
            );

            DuringAny(
                When(LoginFailed)
                    .Then(context =>
                    {
                        _logger.LogError("Saga {CorrelationId}: Falha ao criar login: {Reason}", context.Saga.CorrelationId, context.Message.FaultReason);
                        context.Saga.FaultReason = context.Message.FaultReason;
                    })
                    .TransitionTo(Faulted),
                When(KeycloakUserFailed)
                    .Then(context =>
                    {
                        _logger.LogError("Saga {CorrelationId}: Falha ao criar usuário no Keycloak: {Reason}", context.Saga.CorrelationId, context.Message.FaultReason);
                        context.Saga.FaultReason = context.Message.FaultReason;
                    })
                    .TransitionTo(Faulted)
            );





        }
    }
}
