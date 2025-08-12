namespace OneID.Api.Auth
{
    using Microsoft.AspNetCore.Http;
    using OneID.Application.Interfaces.CookiesOptions;

    public sealed class AuthCookieOptions : IAuthCookieOptions
    {
        public string AccessCookieName { get; init; } = "access_token";
        public string RefreshCookieName { get; init; } = "refresh_token";
        public string Domain { get; init; } = ".oneidsecure.cloud";
        public string Path { get; init; } = "/";
        public bool Secure { get; init; } = true;
        public bool HttpOnly { get; init; } = true;
        public SameSiteMode SameSite { get; init; } = SameSiteMode.Lax;
    }

}
