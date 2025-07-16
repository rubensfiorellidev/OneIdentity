using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OneID.Application.Interfaces.Keycloak;
using OneID.Domain.Entities.KeycloakOptions;

#nullable disable
namespace OneID.Application.Services.KeyCloakServices
{
    public class KeycloakTokenService : IKeycloakTokenService
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly KeycloakOptions _options;

        public KeycloakTokenService(IHttpClientFactory httpClientFactory, IOptions<KeycloakOptions> options)
        {
            _httpClientFactory = httpClientFactory;
            _options = options.Value;
        }

        public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
        {
            var client = _httpClientFactory.CreateClient("KeycloakClient");

            var content = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("client_id", _options.ClientId),
            new KeyValuePair<string, string>("client_secret", _options.ClientSecret),
            new KeyValuePair<string, string>("grant_type", "client_credentials")
            ]);

            var response = await client.PostAsync(
                $"realms/{_options.Realm}/protocol/openid-connect/token",
                content,
                cancellationToken);

            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Erro ao obter token do Keycloak: {json}");

            var tokenResponse = JsonConvert.DeserializeObject<KeycloakTokenResponse>(json);

            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
                throw new InvalidOperationException("Token de acesso não pôde ser obtido.");

            return tokenResponse.AccessToken;
        }
    }

    public class KeycloakTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }

}
