namespace OneID.Application.DTOs.Admission
{
#nullable disable
    public record KeycloakPayload
    {
        public Guid CorrelationId { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

    }
}
