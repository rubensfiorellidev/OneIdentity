using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OneID.Application.Interfaces.Keycloak;
using OneID.Domain.Entities.KeycloakOptions;
using OneID.Domain.Entities.Tokens;
using System.Text.Json;
#nullable disable
namespace OneID.Application.Services.KeyCloakServices
{
    public class KeycloakAuthService : IKeycloakAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly KeycloakOptions _options;
        private readonly ILogger<KeycloakAuthService> _logger;

        public KeycloakAuthService(HttpClient httpClient, IOptions<KeycloakOptions> options, ILogger<KeycloakAuthService> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<KeycloakTokenResult> AuthenticateAsync(string username, string password)
        {
            var url = $"realms/{_options.Realm}/protocol/openid-connect/token";

            var parameters = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "client_id", _options.ClientId },
                { "client_secret", _options.ClientSecret },
                { "username", username },
                { "password", password }
            };

            var response = await _httpClient.PostAsync(url, new FormUrlEncodedContent(parameters));
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Erro ao autenticar no Keycloak. Status: {Status}, Detalhes: {Detalhes}", response.StatusCode, error);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var payload = JsonDocument.Parse(json).RootElement;

            return new KeycloakTokenResult
            {
                AccessToken = payload.GetProperty("access_token").GetString(),
                RefreshToken = payload.GetProperty("refresh_token").GetString(),
                ExpiresIn = payload.GetProperty("expires_in").GetInt32(),
                RefreshExpiresIn = payload.GetProperty("refresh_expires_in").GetInt32()
            };
        }
    }

}
