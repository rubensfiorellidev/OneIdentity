namespace OneID.Api.Auth
{
    using Microsoft.AspNetCore.Mvc;
    using OneID.Application.Interfaces.CookiesOptions;

    public sealed class AuthCookiesResult : IActionResult
    {
        private readonly string _accessToken;
        private readonly TimeSpan _accessTtl;
        private readonly string _refreshToken;
        private readonly TimeSpan _refreshTtl;
        private readonly IAuthCookieOptions _opts;
        private readonly bool _includeBody;
        private readonly object? _body;

        public AuthCookiesResult(
            string accessToken,
            TimeSpan accessTtl,
            string refreshToken,
            TimeSpan refreshTtl,
            IAuthCookieOptions options,
            bool includeBody = false,
            object? body = null)
        {
            _accessToken = accessToken;
            _accessTtl = accessTtl;
            _refreshToken = refreshToken;
            _refreshTtl = refreshTtl;
            _opts = options;
            _includeBody = includeBody;
            _body = body;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var resp = context.HttpContext.Response;

            // Evita cache de resposta com credenciais
            resp.Headers.CacheControl = "no-store";
            resp.Headers.Pragma = "no-cache";
            resp.Headers.Expires = "0";

            // Remoção prévia
            var del = new CookieOptions { Domain = _opts.Domain, Path = _opts.Path };
            resp.Cookies.Delete(_opts.AccessCookieName, del);
            resp.Cookies.Delete(_opts.RefreshCookieName, del);

            resp.Cookies.Append(_opts.AccessCookieName, _accessToken, new CookieOptions
            {
                HttpOnly = _opts.HttpOnly,
                Secure = _opts.Secure,
                SameSite = _opts.SameSite,
                Path = _opts.Path,
                Domain = _opts.Domain,
                Expires = DateTimeOffset.UtcNow.Add(_accessTtl),
                MaxAge = _accessTtl
            });

            resp.Cookies.Append(_opts.RefreshCookieName, _refreshToken, new CookieOptions
            {
                HttpOnly = _opts.HttpOnly,
                Secure = _opts.Secure,
                SameSite = _opts.SameSite,
                Path = _opts.Path,
                Domain = _opts.Domain,
                Expires = DateTimeOffset.UtcNow.Add(_refreshTtl),
                MaxAge = _refreshTtl
            });

            if (_includeBody && _body is not null)
            {
                var objectResult = new OkObjectResult(_body);
                await objectResult.ExecuteResultAsync(context);
            }
            else
            {
                resp.StatusCode = StatusCodes.Status204NoContent;
            }
        }
    }
}
