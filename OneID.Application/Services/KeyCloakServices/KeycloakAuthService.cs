using Microsoft.Extensions.Options;
using OneID.Application.Interfaces.Keycloak;
using OneID.Domain.Entities.KeycloakOptions;
using OneID.Domain.Entities.TokenResults;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
#nullable disable
namespace OneID.Application.Services.KeyCloakServices
{
    public class KeycloakAuthService : IKeycloakAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly KeycloakOptions _options;

        public KeycloakAuthService(IOptions<KeycloakOptions> options)
        {
            _options = options.Value;
            _httpClient = new HttpClient();
        }

        public async Task<KeycloakTokenResult> AuthenticateAsync(string username, string password)
        {
            var url = $"{_options.BaseUrl}/realms/{_options.Realm}/protocol/openid-connect/token";

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

            var accessToken = payload.GetProperty("access_token").GetString();
            var refreshToken = payload.GetProperty("refresh_token").GetString();

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(accessToken);


            return new KeycloakTokenResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = payload.GetProperty("expires_in").GetInt32(),
                RefreshExpiresIn = payload.GetProperty("refresh_expires_in").GetInt32()
            };

        }
    }

}
