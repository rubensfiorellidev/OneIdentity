namespace OneID.WebApp.Services
{
    using Microsoft.AspNetCore.Components.Server.Circuits;
    using OneID.WebApp.Interfaces;
    using System.IdentityModel.Tokens.Jwt;
    using System.Net;

    public sealed class TokenCircuitHandler : CircuitHandler
    {
        private static readonly HashSet<Circuit> _activeCircuits = [];
        private static readonly PeriodicTimer _timer = new(TimeSpan.FromMinutes(4));
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<TokenCircuitHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenMemoryStore _tokenStore;

        private static bool _refreshLoopStarted = false;
        private static readonly object _lock = new();

        public TokenCircuitHandler(IHttpClientFactory httpClientFactory,
                                   ILogger<TokenCircuitHandler> logger,
                                   IHttpContextAccessor httpContextAccessor,
                                   ITokenMemoryStore tokenStore)
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
            _tokenStore = tokenStore;
        }

        public override async Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            _activeCircuits.Add(circuit);

            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext?.Request.Cookies.TryGetValue("refresh_token", out var refreshToken) == true &&
                httpContext.Request.Cookies.TryGetValue("access_token", out var accessToken))
            {
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var jwt = tokenHandler.ReadJwtToken(accessToken);

                    var expClaim = jwt.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;

                    if (!string.IsNullOrWhiteSpace(expClaim) && long.TryParse(expClaim, out var expUnix))
                    {
                        var exp = DateTimeOffset.FromUnixTimeSeconds(expUnix);
                        var scheduledTime = exp.AddMinutes(-4);

                        _tokenStore.SetToken(circuit.Id, refreshToken, scheduledTime);
                        _logger.LogInformation("Token armazenado na memória para o circuito {CircuitId}, expira em {Expiration}", circuit.Id, exp);
                    }
                    else
                    {
                        _logger.LogWarning("Claim 'exp' não encontrada ou inválida no access_token.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao decodificar access_token ou calcular expiração.");
                }
            }
            else
            {
                _logger.LogWarning("access_token ou refresh_token ausente nos cookies ao abrir o circuito {CircuitId}", circuit.Id);
            }

            await base.OnCircuitOpenedAsync(circuit, cancellationToken);
        }

        public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            _activeCircuits.Remove(circuit);
            _tokenStore.RemoveToken(circuit.Id);
            _logger.LogInformation("Token removido da memória para o circuito {CircuitId}", circuit.Id);

            return Task.CompletedTask;
        }

        private async Task RefreshLoopAsync()
        {
            while (await _timer.WaitForNextTickAsync())
            {
                var now = DateTimeOffset.UtcNow;

                foreach (var circuit in _activeCircuits.ToList())
                {
                    var tokenData = _tokenStore.GetToken(circuit.Id);

                    if (tokenData is null || string.IsNullOrWhiteSpace(tokenData.RefreshToken))
                        continue;

                    if (tokenData.ExpiresAt <= now)
                    {
                        _logger.LogInformation("Iniciando renovação para circuito {CircuitId}", circuit.Id);

                        try
                        {
                            var client = _httpClientFactory.CreateClient("RefreshClient");
                            var request = new HttpRequestMessage(HttpMethod.Post, "/v1/auth/refresh-token");
                            request.Headers.Add("Cookie", $"refresh_token={WebUtility.UrlEncode(tokenData.RefreshToken)}");

                            var response = await client.SendAsync(request);
                            if (response.IsSuccessStatusCode)
                            {
                                _logger.LogInformation("Token renovado com sucesso para circuito {CircuitId}", circuit.Id);
                            }
                            else
                            {
                                _logger.LogWarning("Falha ao renovar token para circuito {CircuitId}. Status: {StatusCode}", circuit.Id, response.StatusCode);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Erro ao renovar token para circuito {CircuitId}", circuit.Id);
                        }
                    }
                }
            }
        }

        public static bool IsCircuitActive(Circuit circuit)
        {
            return _activeCircuits.Contains(circuit);
        }
    }

}
