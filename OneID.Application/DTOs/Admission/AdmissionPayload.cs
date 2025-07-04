namespace OneID.Application.DTOs.Admission
{
#nullable disable
    public record AdmissionPayload
    {
        public Guid CorrelationId { get; init; }

        public string Username { get; init; }
        public string Password { get; init; }
        public string Email { get; init; }
        public string Firstname { get; init; }
        public string Lastname { get; init; }

    }
}
