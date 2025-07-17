using MassTransit;
using Microsoft.Extensions.Logging;
using OneID.Application.Interfaces.Services;
using OneID.Application.Messaging.Sagas.Contracts.Events;

namespace OneID.Application.Messaging.Sagas.Consumers
{
    public sealed class CreateLoginConsumer : IConsumer<CreateLoginRequested>
    {
        private readonly ILogger<CreateLoginConsumer> _logger;
        private readonly IUserLoginGenerator _loginGenerator;

        public CreateLoginConsumer(
            ILogger<CreateLoginConsumer> logger, IUserLoginGenerator loginGenerator)
        {
            _logger = logger;
            _loginGenerator = loginGenerator;
        }

        public async Task Consume(ConsumeContext<CreateLoginRequested> context)
        {
            var message = context.Message;

            try
            {
                _logger.LogInformation(
                    "Iniciando provisionamento de login para {Firstname} {Lastname} - CorrelationId: {CorrelationId}",
                    message.FirstName, message.LastName, message.CorrelationId);

                var login = await _loginGenerator.GenerateLoginAsync(
                    $"{context.Message.FirstName} " +
                    $"{context.Message.LastName}",
                    context.CancellationToken);

                _logger.LogInformation(
                    "Login {Login} criado e usuário provisionado com sucesso - CorrelationId: {CorrelationId}",
                    login, message.CorrelationId);

                await context.Publish(new LoginCreated
                {
                    CorrelationId = message.CorrelationId,
                    Login = login,
                    CorporateEmail = $"{login}@oneidsecure.cloud"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Erro ao provisionar login - CorrelationId: {CorrelationId}",
                    message.CorrelationId);

                await context.Publish(new LoginFailed
                {
                    CorrelationId = message.CorrelationId,
                    FaultReason = ex.Message
                });

                throw;
            }
        }
    }
}
