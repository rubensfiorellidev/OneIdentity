using OneID.Shared.Authentication;

#nullable disable
namespace OneID.Api.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using System.Text.Json;

    [ApiController]
    [Route("external-auth")]
    public class ExternalAuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly JwtProvider _jwtProvider;
        private readonly IHttpClientFactory _httpClientFactory;

        public ExternalAuthController(
            IConfiguration configuration,
            JwtProvider jwtProvider,
            IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _jwtProvider = jwtProvider;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("signin-oidc")]
        public async Task<IActionResult> SignInOidc([FromQuery] string code, [FromQuery] string state)
        {
            if (string.IsNullOrEmpty(code))
                return BadRequest("Código não fornecido.");

            var tokenResponse = await ExchangeCodeForTokenAsync(code);
            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.Value.AccessToken))
                return Unauthorized("Não foi possível obter token da Microsoft.");

            var userInfo = await GetUserInfoAsync(tokenResponse.Value.AccessToken);
            if (userInfo == null || string.IsNullOrEmpty(userInfo.Value.Email))
                return Unauthorized("Não foi possível obter informações do usuário.");

            // ✅ Corrigido aqui 👇
            var keycloakUserId = await GetKeycloakUserIdByEmail(userInfo.Value.Email);
            if (keycloakUserId is null)
                return Unauthorized("Usuário não encontrado no Keycloak.");

            var jwt = await _jwtProvider.GenerateTokenAsync(
                keycloakUserId.Value,
                preferredUsername: userInfo.Value.Username,
                email: userInfo.Value.Email,
                name: userInfo.Value.Name
            );

            return Ok(new
            {
                access_token = jwt.Jwtoken,
                refresh_token = jwt.RefreshToken,
                expires_in = 900
            });
        }

        private async Task<(string AccessToken, string IdToken)?> ExchangeCodeForTokenAsync(string code)
        {
            var tenantId = _configuration["AzureAd:TenantId"];
            var clientId = _configuration["AzureAd:ClientId"];
            var clientSecret = _configuration["AzureAd:ClientSecret"];
            var redirectUri = _configuration["AzureAd:CallbackPath"];

            var tokenUrl = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";

            var form = new Dictionary<string, string>
            {
                ["client_id"] = clientId,
                ["scope"] = "openid profile email",
                ["code"] = code,
                ["redirect_uri"] = redirectUri,
                ["grant_type"] = "authorization_code",
                ["client_secret"] = clientSecret
            };

            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsync(tokenUrl, new FormUrlEncodedContent(form));
            if (!response.IsSuccessStatusCode) return default;

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content).RootElement;

            return (
                AccessToken: json.GetProperty("access_token").GetString(),
                IdToken: json.GetProperty("id_token").GetString()
            );
        }

        private async Task<(string Email, string Name, string Username)?> GetUserInfoAsync(string accessToken)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.GetAsync("https://graph.microsoft.com/v1.0/me");
            if (!response.IsSuccessStatusCode) return default;

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content).RootElement;

            return (
                Email: json.GetProperty("mail").GetString() ?? json.GetProperty("userPrincipalName").GetString(),
                Name: json.GetProperty("displayName").GetString(),
                Username: json.GetProperty("userPrincipalName").GetString()
            );
        }

        private async Task<Guid?> GetKeycloakUserIdByEmail(string email)
        {
            var keycloakBaseUrl = _configuration["Keycloak:BaseUrl"];
            var realm = _configuration["Keycloak:Realm"];
            var clientId = _configuration["Keycloak:ClientId"];
            var clientSecret = _configuration["Keycloak:ClientSecret"];

            var client = _httpClientFactory.CreateClient();

            // Primeiro, obter token de acesso do cliente Keycloak
            var tokenResponse = await client.PostAsync($"{keycloakBaseUrl}/realms/{realm}/protocol/openid-connect/token",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["grant_type"] = "client_credentials",
                    ["client_id"] = clientId,
                    ["client_secret"] = clientSecret
                }));

            if (!tokenResponse.IsSuccessStatusCode)
                return null;

            var tokenJson = JsonDocument.Parse(await tokenResponse.Content.ReadAsStringAsync()).RootElement;
            var kcAccessToken = tokenJson.GetProperty("access_token").GetString();

            // Buscar usuário pelo email
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", kcAccessToken);

            var usersResponse = await client.GetAsync($"{keycloakBaseUrl}/admin/realms/{realm}/users?email={email}");
            if (!usersResponse.IsSuccessStatusCode)
                return null;

            var usersJson = JsonDocument.Parse(await usersResponse.Content.ReadAsStringAsync()).RootElement;

            if (usersJson.GetArrayLength() == 0)
                return null;

            var idStr = usersJson[0].GetProperty("id").GetString();
            return Guid.TryParse(idStr, out var guid) ? guid : null;
        }
    }
}
