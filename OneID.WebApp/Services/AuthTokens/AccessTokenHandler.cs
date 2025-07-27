using System.Net.Http.Headers;

namespace OneID.WebApp.Services.AuthTokens
{
    public class AccessTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccessTokenHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // 1. Prioridade: Cookie "access_token"
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["access_token"];

            // 2. Fallback: extrair do header (caso você já tenha usado JwtHandler como no dashboard)
            if (string.IsNullOrWhiteSpace(token))
            {
                var rawCookie = _httpContextAccessor.HttpContext?.Request.Headers["Cookie"].ToString();
                if (!string.IsNullOrEmpty(rawCookie))
                {
                    var cookies = rawCookie.Split(';')
                        .Select(c => c.Trim())
                        .Where(c => c.StartsWith("access_token="))
                        .Select(c => c["access_token=".Length..])
                        .FirstOrDefault();

                    token = cookies;
                }
            }

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }

}
