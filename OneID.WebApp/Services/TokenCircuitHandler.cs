namespace OneID.WebApp.Services
{
    using Microsoft.AspNetCore.Components.Server.Circuits;

    public sealed class TokenCircuitHandler : CircuitHandler
    {
        private static readonly HashSet<Circuit> _activeCircuits = [];
        private static readonly PeriodicTimer _timer = new(TimeSpan.FromMinutes(4));
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<TokenCircuitHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private static bool _refreshLoopStarted = false;
        private static readonly object _lock = new();

        public TokenCircuitHandler(IHttpClientFactory httpClientFactory,
                                   ILogger<TokenCircuitHandler> logger,
                                   IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;

            lock (_lock)
            {
                if (!_refreshLoopStarted)
                {
                    _refreshLoopStarted = true;
                    _ = Task.Run(RefreshLoopAsync);
                }
            }

            _httpContextAccessor = httpContextAccessor;
        }

        public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            _activeCircuits.Add(circuit);
            return Task.CompletedTask;
        }

        public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            _activeCircuits.Remove(circuit);
            return Task.CompletedTask;
        }

        private async Task RefreshLoopAsync()
        {
            while (await _timer.WaitForNextTickAsync())
            {
                if (_activeCircuits.Count == 0)
                {
                    _logger.LogDebug("Nenhum circuito ativo, pulando refresh...");
                    continue;
                }

                try
                {
                    var refreshToken = _httpContextAccessor.HttpContext?.Request.Cookies["refresh_token"];
                    if (string.IsNullOrWhiteSpace(refreshToken))
                    {
                        _logger.LogWarning("Nenhum refresh_token presente nos cookies.");
                        continue;
                    }

                    var client = _httpClientFactory.CreateClient();
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7200/v1/auth/refresh-token");
                    request.Headers.Add("Cookie", $"refresh_token={refreshToken}");

                    _logger.LogInformation("Enviando requisição de refresh com cookie manual.");

                    var response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation("AccessToken renovado com sucesso.");
                    }
                    else
                    {
                        _logger.LogWarning("Falha ao renovar token. Código: {StatusCode}", response.StatusCode);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao tentar renovar token.");
                }
            }
        }

        public static bool IsCircuitActive(Circuit circuit)
        {
            return _activeCircuits.Contains(circuit);
        }
    }

}
