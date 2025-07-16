using MassTransit;

#nullable disable
namespace OneID.Application.Messaging.Sagas.Contracts.Events
{
    public record AccountCpfValidated : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; init; }
        public string CpfHash { get; init; }
    }

}
