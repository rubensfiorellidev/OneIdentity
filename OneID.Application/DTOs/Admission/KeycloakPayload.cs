﻿namespace OneID.Application.DTOs.Admission
{
#nullable disable
    public record KeycloakPayload
    {
        public Guid CorrelationId { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid KeycloakUserId { get; set; }

    }
}
