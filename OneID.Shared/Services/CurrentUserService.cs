using Microsoft.AspNetCore.Http;
using OneID.Application.Interfaces.Interceptor;
using System.Security.Claims;

#nullable disable
namespace OneID.Shared.Services
{
    public sealed class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetClaim(string claimType)
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(claimType)?.Value;
        }

        public string GetUserId()
        {
            return _httpContextAccessor.HttpContext?.User?
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public string GetUsername()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.FindFirst("preferred_username")?.Value
                ?? user?.FindFirst(ClaimTypes.Name)?.Value;

        }
    }
}
