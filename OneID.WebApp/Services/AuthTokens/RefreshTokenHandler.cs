using System.Net;

public class RefreshTokenHandler : DelegatingHandler
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<RefreshTokenHandler> _logger;

    public RefreshTokenHandler(
        IHttpClientFactory httpClientFactory,
        ILogger<RefreshTokenHandler> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning("Token expirado. Tentando fazer refresh...");

            try
            {
                var refreshClient = _httpClientFactory.CreateClient();
                var refreshResponse = await refreshClient.PostAsync("https://localhost:7200/v1/auth/refresh-token", null, cancellationToken);

                if (refreshResponse.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Token renovado com sucesso. Reenviando requisição original...");

                    var clonedRequest = await CloneHttpRequestMessageAsync(request);
                    return await base.SendAsync(clonedRequest, cancellationToken);
                }
                else
                {
                    _logger.LogWarning("Refresh token falhou. Sinalizando para UI tomar ação.");

                    response.Headers.Add("X-Token-Expired", "true");
                }
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
