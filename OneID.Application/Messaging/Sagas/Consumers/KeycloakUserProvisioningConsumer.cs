using MassTransit;
using Microsoft.Extensions.Logging;
using OneID.Application.Interfaces;
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
                await _creator.CreateUserAsync(
                    msg.Payload.Username,
                    msg.Payload.Password,
                    msg.Payload.Email,
                    msg.Payload.Firstname,
                    msg.Payload.Lastname,
                    context.CancellationToken
                );

                await context.Publish(new KeycloakUserCreated
                {
                    CorrelationId = msg.CorrelationId
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
