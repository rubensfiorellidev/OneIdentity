using Microsoft.Extensions.Options;
using OneID.Application.Interfaces.Keycloak;
using OneID.Domain.Entities.KeycloakOptions;
using OneID.Domain.Entities.TokenResults;
using System.Text.Json;
#nullable disable
namespace OneID.Application.Services.KeyCloakServices
{
    public class KeycloakAuthService : IKeycloakAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly KeycloakOptions _options;

        public KeycloakAuthService(HttpClient httpClient, IOptions<KeycloakOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
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
            if (!response.IsSuccessStatusCode) return null;

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
