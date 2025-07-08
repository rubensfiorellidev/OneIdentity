using MassTransit;
using Microsoft.Extensions.Logging;
using OneID.Application.Messaging.Sagas.Contracts;
using OneID.Application.Messaging.Sagas.Contracts.Events;
using OneID.Domain.Entities.AuditSagas;
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




            Initially(
                When(StartSaga)
                    .Then(context =>
                    {
                        context.Saga.CorrelationId = context.Message.CorrelationId;
                        context.Saga.Payload = context.Message.Payload;
                        context.Saga.CreatedAt = DateTimeOffset.UtcNow;
                    })
                    .Publish(context => new AdmissionAudit
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        Firstname = context.Saga.Payload.Firstname,
                        Lastname = context.Saga.Payload.Lastname,
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
                        FirstName = context.Saga.Payload.Firstname,
                        LastName = context.Saga.Payload.Lastname

                    })
                    .TransitionTo(WaitingLoginResult)

            );

            During(WaitingLoginResult,
                When(LoginCreated)
                    .Then(context =>
                    {
                        var pwd = PasswordTempGenerator.GenerateTemporaryPassword();
                        context.Saga.Payload = context.Saga.Payload with
                        {
                            Username = context.Message.Login,
                            Password = pwd,
                            Email = $"{context.Message.Login}@company.com"
                        };
                    })
                    .Publish(context => new AdmissionAudit
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        Firstname = context.Saga.Payload.Firstname,
                        Lastname = context.Saga.Payload.Lastname,
                        CurrentState = nameof(WaitingLoginResult),
                        EventName = "Login provisionado",
                        Description = "Login foi provisionado no sistema",
                        ProvisioningDate = DateTimeOffset.UtcNow,
                        Login = context.Message.Login,
                        DatabaseId = "NotAvailable"
                    })
                   .Publish(context => new KeycloakUserCreationRequested
                   {
                       CorrelationId = context.Saga.CorrelationId,
                       Payload = context.Saga.Payload
                   })
                   .TransitionTo(WaitingKeycloakCreation),

                When(LoginFailed)
                    .Then(context =>
                    {
                        context.Saga.FaultReason = context.Message.FaultReason;
                        _logger.LogError("Falha ao gerar login: {Reason}", context.Message.FaultReason);
                    })
                    .Publish(context => new AdmissionAudit
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        Firstname = context.Saga.Payload.Firstname,
                        Lastname = context.Saga.Payload.Lastname,
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
                        context.Saga.Payload.Username = context.Message.KeycloakPayload.Username;
                        context.Saga.Payload.Password = context.Message.KeycloakPayload.Password;
                        context.Saga.Payload.Email = context.Message.KeycloakPayload.Email;
                    })
                    .Publish(context => new AdmissionAudit
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        Firstname = context.Saga.Payload.Firstname,
                        Lastname = context.Saga.Payload.Lastname,
                        CurrentState = nameof(Completed),
                        EventName = "Usuário provisionado no Keycloak",
                        Description = "Usuário provisionado com sucesso no Keycloak. Iniciando validação do CPF.",
                        ProvisioningDate = DateTimeOffset.UtcNow,
                        Login = context.Saga.Payload.Username,
                        DatabaseId = "NotAvailable"
                    })
                    .Publish(context => new AccountCpfValidationRequested
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        Cpf = context.Saga.DatabasePayload.Cpf,
                        FullName = context.Saga.DatabasePayload.FullName,
                        PositionHeldId = context.Saga.DatabasePayload.PositionHeldId
                    })
                    .TransitionTo(WaitingCpfValidation),

                    When(KeycloakUserFailed)
                            .Then(context =>
                            {
                                context.Saga.FaultReason = context.Message.FaultReason;
                            })
                            .Publish(context => new AdmissionAudit
                            {
                                CorrelationId = context.Saga.CorrelationId,
                                Firstname = context.Saga.Payload.Firstname,
                                Lastname = context.Saga.Payload.Lastname,
                                CurrentState = nameof(Faulted),
                                EventName = "Falha no Keycloak",
                                Description = $"Erro: {context.Message.FaultReason}",
                                ProvisioningDate = DateTimeOffset.UtcNow,
                                Login = context.Saga.Payload.Username,
                                DatabaseId = "NotAvailable"
                            })
                            .TransitionTo(Faulted)
            );

            During(WaitingCpfValidation,
                When(CpfValidated)
                    .Publish(context => new AdmissionAudit
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        CurrentState = nameof(WaitingDatabaseResult),
                        EventName = "CPF validado",
                        Description = "CPF validado com sucesso. Iniciando criação da conta no sistema.",
                        ProvisioningDate = DateTimeOffset.UtcNow,
                        Login = context.Saga.Login,
                        DatabaseId = "InProcess"
                    })
                    .Publish(context => new CreateAccountDatabaseCommand
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        Login = context.Saga.Login
                    })
                    .TransitionTo(WaitingDatabaseResult),

                When(CpfValidationFailed)
                    .Then(context => context.Saga.FaultReason = context.Message.FaultReason)
                    .Publish(context => new AdmissionAudit
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        CurrentState = nameof(Faulted),
                        EventName = "Falha na validação do CPF",
                        Description = $"Não foi possível validar o CPF: {context.Message.FaultReason}",
                        ProvisioningDate = DateTimeOffset.UtcNow,
                        Login = context.Saga.Login,
                        DatabaseId = "NotAvailable"
                    })
                    .TransitionTo(Faulted)
            );


            During(WaitingDatabaseResult,
                When(DatabaseCreated)
                    .Then(context => context.Saga.DatabaseId = context.Message.DatabaseId)
                    .Publish(context => new AdmissionAudit
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        Firstname = context.Saga.Payload.Firstname,
                        Lastname = context.Saga.Payload.Lastname,
                        CurrentState = nameof(Completed),
                        EventName = "UserAccount provisionado",
                        Description = $"User account provisionado na base de dados com o ID {context.Message.DatabaseId}",
                        ProvisioningDate = DateTimeOffset.UtcNow,
                        Login = context.Saga.Payload.Username,
                        DatabaseId = context.Message.DatabaseId
                    })
                    .TransitionTo(Completed),

                When(DatabaseFailed)
                    .Then(context => context.Saga.FaultReason = context.Message.FaultReason)
                    .Publish(context => new AdmissionAudit
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        Firstname = context.Saga.Payload.Firstname,
                        Lastname = context.Saga.Payload.Lastname,
                        CurrentState = nameof(Faulted),
                        EventName = "Falha na criação da conta",
                        Description = $"Erro: {context.Message.FaultReason}",
                        ProvisioningDate = DateTimeOffset.UtcNow,
                        Login = context.Saga.Payload.Username,
                        DatabaseId = "NotAvailable"
                    })
                    .TransitionTo(Faulted)
            );

            SetCompletedWhenFinalized();


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
