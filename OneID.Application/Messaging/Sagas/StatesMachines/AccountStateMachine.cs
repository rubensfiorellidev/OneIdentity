using MassTransit;
using Microsoft.Extensions.Logging;
using OneID.Application.Messaging.Sagas.Contracts;
using OneID.Application.Messaging.Sagas.Contracts.Events;
using OneID.Domain.Entities;
using OneID.Domain.Helpers;

#nullable disable
namespace OneID.Application.Messaging.Sagas.StatesMachines
{
    public sealed class AccountStateMachine : MassTransitStateMachine<AccountSagaState>
    {
        private readonly ILogger<AccountStateMachine> _logger;

        public State WaitingLoginResult { get; private set; }
        public State WaitingKeycloakCreation { get; private set; }
        public State Completed { get; private set; }
        public State Faulted { get; private set; }

        public Event<StartCreateAccountSaga> StartSaga { get; private set; }
        public Event<LoginCreated> LoginCreated { get; private set; }
        public Event<LoginFailed> LoginFailed { get; private set; }
        public Event<KeycloakUserCreated> KeycloakUserCreated { get; private set; }
        public Event<KeycloakUserFailed> KeycloakUserFailed { get; private set; }
        public Event<AccountSagaFailed> SagaFailed { get; private set; }



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



            Initially(
                When(StartSaga)
                    .Then(context =>
                    {
                        context.Saga.CorrelationId = context.Message.CorrelationId;
                        context.Saga.Payload = context.Message.Payload with { };
                        context.Saga.CreatedAt = DateTimeOffset.UtcNow;
                    })
                    .Publish(context => new CreateLoginRequested
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        FirstName = context.Saga.Payload.Firstname,
                        LastName = context.Saga.Payload.Lastname

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
                    .TransitionTo(WaitingLoginResult)

            );

            During(WaitingLoginResult,
                When(LoginCreated)
                    .Then(context =>
                    {
                        var password = PasswordTempGenerator.GenerateTemporaryPassword();

                        context.Saga.Payload = context.Saga.Payload with
                        {
                            Username = context.Message.Login,
                            Password = password,
                            Email = $"{context.Message.Login}@company.com"
                        };

                        _logger.LogInformation("Saga {CorrelationId}: Login {Login} criado, senha temporária gerada.", context.Saga.CorrelationId, context.Message.Login);
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
                    .TransitionTo(WaitingKeycloakCreation)
            );

            DuringAny(
                When(LoginFailed)
                    .Then(context =>
                    {
                        _logger.LogError("Saga {CorrelationId}: Falha ao criar login: {Reason}", context.Saga.CorrelationId, context.Message.FaultReason);
                        context.Saga.FaultReason = context.Message.FaultReason;
                    })
                    .Publish(context => new AdmissionAudit
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        Firstname = context.Saga.Payload.Firstname,
                        Lastname = context.Saga.Payload.Lastname,
                        CurrentState = nameof(Faulted),
                        EventName = "Falha na geração do login",
                        Description = $"Erro ao gerar o login: {context.Message.FaultReason}",
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
                        _logger.LogInformation("Saga {CorrelationId}: Usuário criado no Keycloak com sucesso.", context.Saga.CorrelationId);
                    })
                    .Publish(context => new AdmissionAudit
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        Firstname = context.Saga.Payload.Firstname,
                        Lastname = context.Saga.Payload.Lastname,
                        CurrentState = nameof(Completed),
                        EventName = "Usuário criado no Keycloak",
                        Description = "Usuário criado no Keycloak com sucesso.",
                        ProvisioningDate = DateTimeOffset.UtcNow,
                        Login = context.Saga.Payload.Username,
                        DatabaseId = "NotAvailable"
                    })
                    .TransitionTo(Completed)
            );

            During(WaitingKeycloakCreation,
                When(KeycloakUserFailed)
                    .Then(context =>
                    {
                        _logger.LogError("Saga {CorrelationId}: Falha ao criar usuário no Keycloak: {Reason}", context.Saga.CorrelationId, context.Message.FaultReason);
                        context.Saga.FaultReason = context.Message.FaultReason;
                    })
                    .Publish(context => new AdmissionAudit
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        Firstname = context.Saga.Payload.Firstname,
                        Lastname = context.Saga.Payload.Lastname,
                        CurrentState = nameof(Faulted),
                        EventName = "Falha no provisionamento do usuário no Keycloak",
                        Description = $"Erro ao provisionar usuário no Keycloak: {context.Message.FaultReason}",
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
