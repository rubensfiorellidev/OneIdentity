namespace OneID.Domain.Helpers
{
    public static class JwtDefaults
    {
        public static readonly TimeSpan AccessTokenLifetime = TimeSpan.FromMinutes(5);
        public static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(7);
    }

}
