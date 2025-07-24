using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

public class RefreshTokenHandler : DelegatingHandler
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<RefreshTokenHandler> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    private string? _cachedAccessToken;

    public RefreshTokenHandler(
        IHttpClientFactory httpClientFactory,
        ILogger<RefreshTokenHandler> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {

        if (!string.IsNullOrWhiteSpace(_cachedAccessToken))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _cachedAccessToken);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning("Access token expirado. Tentando refresh automático...");

            try
            {
                var refreshClient = _httpClientFactory.CreateClient();
                var refreshRequest = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7200/v1/auth/refresh-token");

                var context = _httpContextAccessor.HttpContext;

                if (context?.Request.Cookies.TryGetValue("refresh_token", out var refreshToken) == true)
                {
                    refreshRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshToken);
                }

                var refreshResponse = await refreshClient.SendAsync(refreshRequest, cancellationToken);

                if (refreshResponse.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Token renovado. Reenviando requisição original...");

                    var json = await refreshResponse.Content.ReadFromJsonAsync<JsonElement>();
                    _cachedAccessToken = json.GetProperty("token").GetString();

                    var clonedRequest = await CloneHttpRequestMessageAsync(request);

                    // Aplica novo access_token
                    if (!string.IsNullOrWhiteSpace(_cachedAccessToken))
                        clonedRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _cachedAccessToken);

                    return await base.SendAsync(clonedRequest, cancellationToken);
                }

                _logger.LogWarning("Refresh token falhou. Token expirado.");
                response.Headers.Add("X-Token-Expired", "true");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar renovar o token.");
                response.Headers.Add("X-Token-Expired", "true");
            }
        }

        return response;
    }

    private async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage request)
    {
        var clone = new HttpRequestMessage(request.Method, request.RequestUri);

        if (request.Content != null)
        {
            var content = await request.Content.ReadAsByteArrayAsync();
            clone.Content = new ByteArrayContent(content);

            foreach (var h in request.Content.Headers)
                clone.Content.Headers.Add(h.Key, h.Value);
        }

        foreach (var header in request.Headers)
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

        clone.Version = request.Version;
        return clone;
    }
}
