#nullable disable
namespace OneID.Application.DTOs.Admission
{
    public record PendingProcessDto
    {
        public Guid CorrelationId { get; init; }
        public string FullName { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
    }

}
