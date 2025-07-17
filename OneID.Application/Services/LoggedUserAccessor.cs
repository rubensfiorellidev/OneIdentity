using Microsoft.AspNetCore.Http;
using OneID.Application.Interfaces.Services;

namespace OneID.Application.Services
{
    public sealed class LoggedUserAccessor : ILoggedUserAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoggedUserAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetEmail()
            => _httpContextAccessor.HttpContext?.User.FindFirst("email")?.Value ?? "unknown@domain.com";

        public string GetName()
            => _httpContextAccessor.HttpContext?.User.FindFirst("name")?.Value ?? "Operador Desconhecido";

        public string GetPhone()
            => "11948571713";
    }

}
