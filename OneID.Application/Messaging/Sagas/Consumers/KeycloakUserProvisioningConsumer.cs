using MassTransit;
using Microsoft.Extensions.Logging;
using OneID.Application.DTOs.Admission;
using OneID.Application.Interfaces.Keycloak;
using OneID.Application.Messaging.Sagas.Contracts.Events;

namespace OneID.Application.Messaging.Sagas.Consumers
{
    public sealed class KeycloakUserProvisioningConsumer : IConsumer<KeycloakUserCreationRequested>
    {
        private readonly ILogger<KeycloakUserProvisioningConsumer> _logger;
        private readonly IKeycloakUserCreator _creator;

        public KeycloakUserProvisioningConsumer(
            ILogger<KeycloakUserProvisioningConsumer> logger,
            IKeycloakUserCreator creator)
        {
            _logger = logger;
            _creator = creator;
        }

        public async Task Consume(ConsumeContext<KeycloakUserCreationRequested> context)
        {
            var msg = context.Message;

            try
            {
                var keycloakUserId = await _creator.CreateUserAsync(
                    msg.Payload.Username,
                    msg.Payload.Password,
                    msg.Payload.Email,
                    msg.Payload.FirstName,
                    msg.Payload.LastName,
                    context.CancellationToken
                );

                await context.Publish(new KeycloakUserCreated
                {
                    CorrelationId = msg.CorrelationId,
                    KeycloakPayload = new KeycloakPayload
                    {
                        Username = msg.Payload.Username,
                        Password = msg.Payload.Password,
                        Email = msg.Payload.Email,
                        KeycloakUserId = keycloakUserId
                    },
                    Cpf = msg.Cpf,
                    FullName = msg.FullName,
                    JobTitleId = msg.JobTitleId
                });

                _logger.LogInformation("Usuário criado no Keycloak - CorrelationId: {CorrelationId}", msg.CorrelationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar usuário no Keycloak - CorrelationId: {CorrelationId}", msg.CorrelationId);

                await context.Publish(new KeycloakUserFailed
                {
                    CorrelationId = msg.CorrelationId,
                    FaultReason = ex.Message
                });

                throw;
            }
        }
    }

}
