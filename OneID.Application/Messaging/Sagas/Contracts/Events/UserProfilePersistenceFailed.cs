using MassTransit;
#nullable disable
namespace OneID.Application.Messaging.Sagas.Contracts.Events
{
    public record UserProfilePersistenceFailed : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; init; }
        public string FaultReason { get; init; }
    }

}
