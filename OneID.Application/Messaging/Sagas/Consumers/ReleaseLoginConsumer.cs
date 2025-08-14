using MassTransit;
using Microsoft.Extensions.Logging;
using OneID.Application.Interfaces.Logins;
using OneID.Application.Messaging.Sagas.Contracts.Events;

namespace OneID.Application.Messaging.Sagas.Consumers
{
    public sealed class ReleaseLoginConsumer : IConsumer<ReleaseLoginRequested>
    {
        private readonly ILogger<ReleaseLoginConsumer> _logger;
        private readonly ILoginReservationRepository _repo;

        public ReleaseLoginConsumer(ILogger<ReleaseLoginConsumer> logger, ILoginReservationRepository repo)
        { _logger = logger; _repo = repo; }

        public async Task Consume(ConsumeContext<ReleaseLoginRequested> context)
        {
            await _repo.ReleaseAsync(context.Message.CorrelationId, context.CancellationToken);
            _logger.LogInformation("Login release concluído (Cid: {Cid})", context.Message.CorrelationId);
        }
    }
}
