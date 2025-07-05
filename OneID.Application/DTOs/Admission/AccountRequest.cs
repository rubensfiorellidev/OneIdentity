namespace OneID.Application.DTOs.Admission
{
#nullable disable
    public record AccountRequest
    {
        public Guid CorrelationId { get; init; }
        public string Firstname { get; init; }
        public string Lastname { get; init; }
    }
}
