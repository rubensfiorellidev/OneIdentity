using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OneID.Application.Abstractions;
using OneID.Application.Interfaces;

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


            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException($"Erro ao obter token do Keycloak: {error}");
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var tokenResponse = JsonConvert.DeserializeObject<KeycloakTokenResponse>(json);

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
