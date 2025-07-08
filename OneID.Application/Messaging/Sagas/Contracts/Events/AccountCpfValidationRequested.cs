using MassTransit;

#nullable disable
namespace OneID.Application.Messaging.Sagas.Contracts.Events
{
    public record AccountCpfValidationRequested : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; init; }
        public string Cpf { get; init; }
        public string FullName { get; init; }
        public string PositionHeldId { get; init; }
    }
}
