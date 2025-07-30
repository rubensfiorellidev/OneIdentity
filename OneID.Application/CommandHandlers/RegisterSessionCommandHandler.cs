using Microsoft.Extensions.Logging;
using OneID.Application.Commands;
using OneID.Application.DTOs.ActiveSessions;
using OneID.Application.Interfaces.CQRS;
using OneID.Application.Interfaces.Repositories;
using OneID.Domain.Interfaces;
using OneID.Domain.Results;

namespace OneID.Application.CommandHandlers
{
    public class RegisterSessionCommandHandler : ICommandHandler<RegisterSessionCommand, IOperationResult>
    {
        private readonly IRedisRepository _redis;
        private readonly ILogger<RegisterSessionCommandHandler> _logger;

        public RegisterSessionCommandHandler(
            IRedisRepository redis,
            ILogger<RegisterSessionCommandHandler> logger)
        {
            _redis = redis;
            _logger = logger;
        }

        public async Task<IOperationResult> Handle(RegisterSessionCommand command, CancellationToken cancellationToken)
        {

            var session = new ActiveSessionInfo(
                command.CircuitId,
                command.IpAddress,
                command.UpnOrName,
                command.UserAgent,
                command.LastActivity,
                command.ExpiresAt
            );

            var key = $"session:{command.CircuitId}";
            var ttl = session.ExpiresAt - session.LastActivity;

            await _redis.SetAsync(key, session, ttl);

            _logger.LogInformation("Sessão registrada no Redis: {CircuitId}", session.CircuitId);

            return Result.Success();
        }
    }

}
