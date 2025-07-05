using MassTransit;

#nullable disable
namespace OneID.Application.Messaging.Sagas.Contracts.Events
{
    public record LoginCreated : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; init; }
        public string Login { get; init; }
    }
}
