using Microsoft.AspNetCore.Http;

namespace OneID.Application.Interfaces.CookiesOptions
{
    public interface IAuthCookieOptions
    {
        public string AccessCookieName { get; }
        public string RefreshCookieName { get; }
        public string Domain { get; }
        public string Path { get; }
        public bool Secure { get; }
        public bool HttpOnly { get; }
        public SameSiteMode SameSite { get; }
    }
}
