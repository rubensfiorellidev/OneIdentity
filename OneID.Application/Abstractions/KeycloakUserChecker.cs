using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OneID.Application.Interfaces;

#nullable disable
namespace OneID.Application.Abstractions
{
    public class KeycloakUserChecker : IKeycloakUserChecker
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IKeycloakTokenService _tokenService;
        private readonly KeycloakOptions _options;

        public KeycloakUserChecker(
            IHttpClientFactory httpClientFactory,
            IKeycloakTokenService tokenService,
            IOptions<KeycloakOptions> options)
        {
            _httpClientFactory = httpClientFactory;
            _tokenService = tokenService;
            _options = options.Value;
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
            var users = JsonConvert.DeserializeObject<List<object>>(json);

            return users != null && users.Any();
        }
    }

}
