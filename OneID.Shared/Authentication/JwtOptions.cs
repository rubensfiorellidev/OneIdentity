﻿namespace OneID.Shared.Authentication
{
    public sealed class JwtOptions
    {
        public string? Issuer { get; init; }
        public string? Audience { get; init; }
        public string? PrivateKey { get; init; }
        public TimeSpan AccessTokenExpires { get; set; } = TimeSpan.FromMinutes(15);
        public TimeSpan AccessTokenTotpExpires { get; set; } = TimeSpan.FromMinutes(2);
        public TimeSpan RefreshTokenExpires { get; set; }
        public string? PrivateKeyPath { get; set; }

    }
}
