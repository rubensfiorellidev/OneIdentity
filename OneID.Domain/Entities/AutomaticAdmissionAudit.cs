#nullable disable
namespace OneID.Domain.Entities
{
    public sealed class AutomaticAdmissionAudit
    {
        public Guid CorrelationId { get; set; }

        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string DatabaseId { get; set; }
        public string CurrentState { get; set; }
        public string EventName { get; set; }
        public DateTimeOffset ProvisioningDate { get; set; }
        public string Description { get; set; }
        public string Login { get; set; }

    }
}
