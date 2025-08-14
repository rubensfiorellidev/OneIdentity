using MassTransit;
using Microsoft.Extensions.Logging;
using OneID.Application.Interfaces.Logins;
using OneID.Application.Messaging.Sagas.Contracts.Events;

namespace OneID.Application.Messaging.Sagas.Consumers
{
    public sealed class CommitLoginConsumer : IConsumer<CommitLoginRequested>
    {
        private readonly ILogger<CommitLoginConsumer> _logger;
        private readonly ILoginReservationRepository _repo;

        public CommitLoginConsumer(ILogger<CommitLoginConsumer> logger, ILoginReservationRepository repo)
        { _logger = logger; _repo = repo; }

        public async Task Consume(ConsumeContext<CommitLoginRequested> context)
        {
            await _repo.CommitAsync(context.Message.CorrelationId, context.CancellationToken);
            _logger.LogInformation("Login commit concluído (Cid: {Cid})", context.Message.CorrelationId);
        }
    }
}
