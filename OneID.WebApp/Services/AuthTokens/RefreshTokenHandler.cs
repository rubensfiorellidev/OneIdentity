using Microsoft.AspNetCore.Components;
using System.Net;

namespace OneID.WebApp.Services.AuthTokens;

public class RefreshTokenHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly NavigationManager _navigation;
    private readonly ILogger<RefreshTokenHandler> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public RefreshTokenHandler(
        IHttpContextAccessor httpContextAccessor,
        NavigationManager navigation,
        ILogger<RefreshTokenHandler> logger,
        IHttpClientFactory httpClientFactory)
    {
        _httpContextAccessor = httpContextAccessor;
        _navigation = navigation;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Tenta enviar a requisição normalmente
        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning("Token expirado ou inválido. Iniciando tentativa de refresh.");

            try
            {
                var refreshClient = _httpClientFactory.CreateClient();
                var refreshResponse = await refreshClient.PostAsync("https://localhost:7200/v1/auth/refresh-token", null, cancellationToken);

                if (refreshResponse.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Token atualizado com sucesso. Reenviando requisição original.");

                    // Clona a requisição original
                    var clonedRequest = await CloneHttpRequestMessageAsync(request);

                    return await base.SendAsync(clonedRequest, cancellationToken);
                }
                else
                {
                    _logger.LogWarning("Falha ao atualizar o token. Redirecionando para /request-token");
                    _navigation.NavigateTo("/request-token");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar atualizar o token.");
                _navigation.NavigateTo("/request-token");
            }
        }

        return response;
    }

    private async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage request)
    {
        var clone = new HttpRequestMessage(request.Method, request.RequestUri);

        // Copia o conteúdo (se tiver)
        if (request.Content != null)
        {
            var content = await request.Content.ReadAsByteArrayAsync();
            clone.Content = new ByteArrayContent(content);
            if (request.Content.Headers != null)
            {
                foreach (var h in request.Content.Headers)
                    clone.Content.Headers.Add(h.Key, h.Value);
            }
        }

        // Copia os headers
        foreach (var header in request.Headers)
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        clone.Version = request.Version;

        return clone;
    }
}
