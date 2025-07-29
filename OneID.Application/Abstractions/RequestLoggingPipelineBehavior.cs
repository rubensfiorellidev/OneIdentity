using Microsoft.Extensions.Logging;
using OneID.Application.Interfaces.CQRS;
using OneID.Domain.Interfaces;

namespace OneID.Application.Abstractions
{
    public class LoggingCommandHandlerDecorator<TCommand, TResponse> : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        private readonly ICommandHandler<TCommand, TResponse> _inner;
        private readonly ILogger<LoggingCommandHandlerDecorator<TCommand, TResponse>> _logger;

        public LoggingCommandHandlerDecorator(
            ICommandHandler<TCommand, TResponse> inner,
            ILogger<LoggingCommandHandlerDecorator<TCommand, TResponse>> logger)
        {
            _inner = inner;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken)
        {
            var commandName = typeof(TCommand).Name;
            _logger.LogInformation("🔍 Executando comando: {CommandName}", commandName);

            var response = await _inner.Handle(command, cancellationToken);

            if (response is IOperationResult result)
            {
                if (result.IsSuccess)
                    _logger.LogInformation("✅ Comando {CommandName} finalizado com sucesso", commandName);
                else
                    _logger.LogWarning("⚠️ Comando {CommandName} falhou: {Message}", commandName, result.Message);
            }

            return response;
        }
    }

}
