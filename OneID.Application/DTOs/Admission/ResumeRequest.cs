using MassTransit;

#nullable disable
namespace OneID.Application.DTOs.Admission
{
    public record ResumeRequest : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
    }
}
