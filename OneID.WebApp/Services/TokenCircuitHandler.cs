namespace OneID.WebApp.Services
{
    using Microsoft.AspNetCore.Components.Server.Circuits;
    using OneID.WebApp.Interfaces;
    using System.Collections.Concurrent;
    using System.IdentityModel.Tokens.Jwt;
    using System.Net;

    public sealed class TokenCircuitHandler : CircuitHandler
    {
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _scheduledRenewals = new();
        private static readonly HashSet<Circuit> _activeCircuits = [];
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<TokenCircuitHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenMemoryStore _tokenStore;

        public TokenCircuitHandler(IHttpClientFactory httpClientFactory,
                                   ILogger<TokenCircuitHandler> logger,
                                   IHttpContextAccessor httpContextAccessor,
                                   ITokenMemoryStore tokenStore)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
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

                        ScheduleTokenRefresh(circuit, refreshToken, scheduledTime);
                    }
                    else
                    {
                        _logger.LogWarning("Claim 'exp' não encontrada ou inválida no access_token.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao decodificar o access_token ou calcular a expiração.");
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

            if (_scheduledRenewals.TryRemove(circuit.Id, out var cts))
            {
                cts.Cancel();
                cts.Dispose();
            }

            _logger.LogInformation("Circuito encerrado: {CircuitId}. Token removido da memória e agendamento cancelado.", circuit.Id);
            return Task.CompletedTask;
        }

        private void ScheduleTokenRefresh(Circuit circuit, string refreshToken, DateTimeOffset scheduledTime)
        {
            var delay = scheduledTime - DateTimeOffset.UtcNow;
            if (delay <= TimeSpan.Zero) delay = TimeSpan.FromSeconds(1); // fallback seguro

            var cts = new CancellationTokenSource();
            _scheduledRenewals[circuit.Id] = cts;

            _ = Task.Delay(delay, cts.Token).ContinueWith(async task =>
            {
                if (task.IsCanceled || !_activeCircuits.Contains(circuit)) return;

                var tokenData = _tokenStore.GetToken(circuit.Id);
                if (tokenData is null || string.IsNullOrWhiteSpace(tokenData.RefreshToken)) return;

                _logger.LogInformation("Iniciando renovação do token para o circuito {CircuitId}", circuit.Id);

                try
                {
                    var client = _httpClientFactory.CreateClient("RefreshClient");
                    var request = new HttpRequestMessage(HttpMethod.Post, "/v1/auth/refresh-token");
                    request.Headers.Add("Cookie", $"refresh_token={WebUtility.UrlEncode(tokenData.RefreshToken)}");

                    var response = await client.SendAsync(request, cts.Token);
                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation("Token renovado com sucesso para o circuito {CircuitId}", circuit.Id);
                    }
                    else
                    {
                        _logger.LogWarning("Falha ao renovar o token para o circuito {CircuitId}. Status: {StatusCode}", circuit.Id, response.StatusCode);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao tentar renovar o token para o circuito {CircuitId}", circuit.Id);
                }
            }, cts.Token);
        }

        public static bool IsCircuitActive(Circuit circuit)
        {
            return _activeCircuits.Contains(circuit);
        }
    }

}
