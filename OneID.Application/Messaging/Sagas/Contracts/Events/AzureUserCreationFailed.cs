#nullable disable
using MassTransit;

namespace OneID.Application.Messaging.Sagas.Contracts.Events
{
    public record AzureUserCreationFailed : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; init; }
        public string FaultReason { get; init; }
    }
}
