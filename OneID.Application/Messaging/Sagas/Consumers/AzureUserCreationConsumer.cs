using MassTransit;
using Microsoft.Extensions.Logging;
using OneID.Application.Interfaces.Graph;
using OneID.Application.Messaging.Sagas.Contracts.Events;

namespace OneID.Application.Messaging.Sagas.Consumers
{
    public sealed class AzureUserCreationConsumer : IConsumer<AzureUserCreationRequested>
    {
        private readonly IAzureGraphUserSyncService _graphUserSync;
        private readonly ILogger<AzureUserCreationConsumer> _logger;

        public AzureUserCreationConsumer(
            IAzureGraphUserSyncService graphUserSync,
            ILogger<AzureUserCreationConsumer> logger)
        {
            _graphUserSync = graphUserSync;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<AzureUserCreationRequested> context)
        {
            var message = context.Message;

            _logger.LogInformation("Iniciando provisionamento do usuário {Login} no Entra ID...", message.Login);

            var result = await _graphUserSync.CreateUserAsync(message, context.CancellationToken);

            if (result.IsSuccess)
            {
                _logger.LogInformation("Usuário {Login} provisionado com sucesso no Entra ID.", message.Login);

                await context.Publish(new AzureUserCreated
                {
                    CorrelationId = message.CorrelationId
                });
            }
            else
            {
                _logger.LogError("Erro ao provisionar usuário {Login} no Entra ID: {Error}", message.Login, result.FailureReason);

                await context.Publish(new AzureUserCreationFailed
                {
                    CorrelationId = message.CorrelationId,
                    FaultReason = result.FailureReason
                });
            }
        }
    }

}
