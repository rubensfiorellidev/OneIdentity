using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using OneID.Application.Messaging.Sagas.Contracts.Events;

namespace OneID.Application.Messaging.Sagas.Consumers
{
    public sealed class CreateLoginConsumer : IConsumer<CreateLoginRequested>
    {
        private readonly ILogger<CreateLoginConsumer> _logger;
        private readonly ISender _sender;

        public CreateLoginConsumer(ILogger<CreateLoginConsumer> logger, ISender sender)
        {
            _logger = logger;
            _sender = sender;
        }
        public async Task Consume(ConsumeContext<CreateLoginRequested> context)
        {
            var message = context.Message;

            try
            {
                _logger.LogInformation(
                    "Iniciando criação de login para {Firstname}, {Lastname} - CorrelationId: {CorrelationId}",
                    message.FirstName, message.LastName, message.CorrelationId);

                var command = new CreateLoginCommand
                {
                    CorrelationId = message.CorrelationId,
                    FullName = message.FirstName,
                    LastName = message.LastName

                };

                var result = await _sender.Send(command, context.CancellationToken);

                if (result.IsSuccess && result.Data is string login)
                {
                    _logger.LogInformation(
                        "Login {Login} criado com sucesso - CorrelationId: {CorrelationId}",
                        login, message.CorrelationId);

                    await context.Publish(new LoginCreated
                    {
                        CorrelationId = message.CorrelationId,
                        Login = login
                    });
                }
                else
                {
                    var reason = result.Message ?? "Falha ao criar login";

                    _logger.LogWarning(
                        "Falha na criação do login - CorrelationId: {CorrelationId} - Motivo: {Reason}",
                        message.CorrelationId, reason);

                    await context.Publish(new LoginFailed
                    {
                        CorrelationId = message.CorrelationId,
                        FaultReason = "Falha ao criar login"

                    });
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Erro inesperado ao criar login - CorrelationId: {CorrelationId}",
                    message.CorrelationId);

                await context.Publish(new AccountPjLoginCreationFailed
                {
                    CorrelationId = message.CorrelationId,
                    Reason = "Erro inesperado",
                    Errors = [ex.Message]
                });

                throw;
            }

        }
    }
}
