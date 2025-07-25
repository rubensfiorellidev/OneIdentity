﻿#nullable disable
using OneID;

namespace OneID.Domain.Entities.JwtWebTokens
{
    public sealed class RefreshWebToken
    {
        public string Id { get; set; } = $"{Ulid.NewUlid()}";
        public string UserUpn { get; set; }
        public string Jti { get; set; }
        public string Token { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }
        public bool IsUsed { get; set; }

    }
}
