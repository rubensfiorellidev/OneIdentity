using System.Net;
using System.Net.Http.Headers;

public class RefreshTokenHandler : DelegatingHandler
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<RefreshTokenHandler> _logger;

    public RefreshTokenHandler(
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        ILogger<RefreshTokenHandler> logger)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning("Token expirado. Tentando refresh...");

            var refreshToken = _httpContextAccessor.HttpContext?.Request.Cookies["refresh_token"];
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                _logger.LogWarning("Refresh token não encontrado no cookie.");
                return response;
            }

            var refreshClient = _httpClientFactory.CreateClient();
            refreshClient.BaseAddress = new Uri("https://localhost:7200");

            var refreshRequest = new HttpRequestMessage(HttpMethod.Post, "/v1/auth/refresh-token");
            refreshRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshToken);

            var refreshResponse = await refreshClient.SendAsync(refreshRequest, cancellationToken);

            if (refreshResponse.IsSuccessStatusCode)
            {
                _logger.LogInformation("Refresh token bem-sucedido. Reenviando a requisição original...");

                var newRequest = await CloneRequestAsync(request);
                return await base.SendAsync(newRequest, cancellationToken);
            }

            _logger.LogWarning("Refresh token falhou com status {StatusCode}.", refreshResponse.StatusCode);
        }

        return response;
    }

    private async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage request)
    {
        var clone = new HttpRequestMessage(request.Method, request.RequestUri)
        {
            Version = request.Version
        };

        if (request.Content != null)
        {
            var content = await request.Content.ReadAsByteArrayAsync();
            clone.Content = new ByteArrayContent(content);

            foreach (var header in request.Content.Headers)
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        foreach (var header in request.Headers)
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

        return clone;
    }
}
