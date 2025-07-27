using Microsoft.AspNetCore.Http;
using OneID.Application.DTOs.Auth;
using OneID.Application.Interfaces.CQRS;
using OneID.Application.Queries.Auth;
using System.Security.Claims;

#nullable disable
namespace OneID.Application.QueryHandlers.Auth
{
    public sealed class GetCurrentUserQueryHandler : IQueryHandler<GetCurrentUserQuery, UserInfoResponse>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetCurrentUserQueryHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<UserInfoResponse> Handle(GetCurrentUserQuery query, CancellationToken cancellationToken)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity.IsAuthenticated)
                throw new UnauthorizedAccessException();

            var response = new UserInfoResponse
            {
                Name = user.FindFirst("name")?.Value ?? "Desconhecido",
                Email = user.FindFirst("email")?.Value ?? "sem@email.com",
                AccountId = user.FindFirst("account_id")?.Value ?? "N/A",
                Roles = [.. user.FindAll(ClaimTypes.Role).Select(r => r.Value)]
            };

            return Task.FromResult(response);
        }
    }

}
