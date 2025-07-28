namespace OneID.WebApp.Services.AuthTokens
{
#nullable disable
    public record CircuitTokenData
    {
        public string RefreshToken { get; init; }
        public DateTimeOffset ExpiresAt { get; init; }
    }
}
