using System.Net.Http.Headers;

public class RefreshTokenHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<RefreshTokenHandler> _logger;

    public RefreshTokenHandler(IHttpContextAccessor httpContextAccessor, ILogger<RefreshTokenHandler> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var context = _httpContextAccessor.HttpContext;

        if (context != null)
        {
            foreach (var cookie in context.Request.Cookies)
            {
                _logger.LogInformation("Cookie disponível: {Key} = {Value}", cookie.Key, cookie.Value);
            }

            if (context.Request.Cookies.TryGetValue("refresh_token", out var refreshToken) &&
                !string.IsNullOrWhiteSpace(refreshToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshToken);
                _logger.LogInformation("Refresh token injetado no header.");
            }
            else
            {
                _logger.LogWarning("refresh_token não encontrado no cookie. A requisição seguirá sem Authorization header.");
            }
        }
        else
        {
            _logger.LogWarning("HttpContext está nulo no RefreshTokenHandler.");
        }

        return base.SendAsync(request, cancellationToken);


        //var context = _httpContextAccessor.HttpContext;

        //if (context != null && context.Request.Cookies.TryGetValue("refresh_token", out var refreshToken) &&
        //    !string.IsNullOrWhiteSpace(refreshToken))
        //{
        //    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshToken);
        //}
        //else
        //{
        //    _logger.LogWarning("refresh_token não encontrado no cookie. A requisição seguirá sem Authorization header.");
        //}

        //return base.SendAsync(request, cancellationToken);
    }
}
