using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OneID.Application.Interfaces.Keycloak;
using OneID.Domain.Entities.KeycloakOptions;

#nullable disable
namespace OneID.Application.Services.KeyCloakServices
{
    public class KeycloakUserChecker : IKeycloakUserChecker
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IKeycloakTokenService _tokenService;
        private readonly KeycloakOptions _options;
        private readonly ILogger<KeycloakUserChecker> _logger;

        public KeycloakUserChecker(
            IHttpClientFactory httpClientFactory,
            IKeycloakTokenService tokenService,
            IOptions<KeycloakOptions> options,
            ILogger<KeycloakUserChecker> logger)
        {
            _httpClientFactory = httpClientFactory;
            _tokenService = tokenService;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken)
        {
            var token = await _tokenService.GetAccessTokenAsync(cancellationToken);
            var client = _httpClientFactory.CreateClient("KeycloakAdmin");

            var request = new HttpRequestMessage(HttpMethod.Get,
                $"admin/realms/{_options.Realm}/users?username={Uri.EscapeDataString(username)}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new Exception($"Keycloak query failed: {response.StatusCode} - {content}");
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var users = JsonConvert.DeserializeObject<List<KeycloakUserRepresentation>>(json);

            _logger.LogDebug("Consulta Keycloak: {Count} usuários encontrados para username {Username}",
                users?.Count ?? 0, username);

            return users != null && users.Any();
        }
    }

    public class KeycloakUserRepresentation
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }
    }

}
