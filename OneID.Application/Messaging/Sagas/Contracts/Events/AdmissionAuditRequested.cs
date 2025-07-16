using MassTransit;

#nullable disable
namespace OneID.Application.Messaging.Sagas.Contracts.Events
{
    public record AdmissionAuditRequested : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }

        public string DatabaseId { get; set; }
        public string CurrentState { get; set; }
        public string EventName { get; set; }
        public DateTimeOffset ProvisioningDate { get; set; }
        public string Description { get; set; }
        public string Login { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

}
