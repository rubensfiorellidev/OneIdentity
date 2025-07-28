using System.Net.Http.Headers;

namespace OneID.WebApp.Services.AuthTokens
{
    public class AccessTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AccessTokenHandler> _logger;

        public AccessTokenHandler(IHttpContextAccessor httpContextAccessor, ILogger<AccessTokenHandler> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var context = _httpContextAccessor.HttpContext;

            if (context != null && context.Request.Cookies.TryGetValue("access_token", out var accessToken) &&
                !string.IsNullOrWhiteSpace(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            else
            {
                _logger.LogWarning("access_token não encontrado no cookie. A requisição seguirá sem Authorization header.");
            }

            return base.SendAsync(request, cancellationToken);
        }
    }

}
