using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneID.Application.DTOs.Auth;
using OneID.Application.Interfaces.CQRS;
using OneID.Application.Interfaces.Interceptor;
using OneID.Application.Interfaces.Keycloak;
using OneID.Application.Interfaces.SensitiveData;
using OneID.Application.Interfaces.Services;
using OneID.Application.Interfaces.TotpServices;
using OneID.Data.Interfaces;
using OneID.Domain.Contracts.Jwt;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using JwtClaims = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;


#nullable disable
namespace OneID.Api.Controllers
{
    [Route("v1/auth")]
    public class AuthController : MainController
    {
        private readonly IJwtProvider _jwtProvider;
        private readonly ILogger<AuthController> _logger;
        private readonly IOneDbContextFactory _contextFactory;
        private readonly IKeycloakAuthService _authService;
        private readonly IHashService _hashService;
        private readonly ISensitiveDataDecryptionServiceUserAccount _decryptionService;
        private readonly ICurrentUserService _currentUser;
        private readonly ITotpService _totpService;

        private const string BootstraperUserId = "01JZTZXJSC1WY70TPPB1SRVQYZ";
        public AuthController(ISender send,
                              IJwtProvider jwtProvider,
                              ILogger<AuthController> logger,
                              IOneDbContextFactory contextFactory,
                              IKeycloakAuthService authService,
                              IHashService hashService,
                              ISensitiveDataDecryptionServiceUserAccount decryptionService,
                              ICurrentUserService currentUser,
                              ITotpService totpService) : base(send)
        {
            _jwtProvider = jwtProvider;
            _logger = logger;
            _contextFactory = contextFactory;
            _authService = authService;
            _hashService = hashService;
            _decryptionService = decryptionService;
            _currentUser = currentUser;
            _totpService = totpService;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("bootstrap-token")]
        public IActionResult GenerateBootstrapToken([FromBody] Guid correlationId)
        {
            if (correlationId == Guid.Empty)
                return BadRequest("correlationId inválido.");

            var username = User.Identity?.Name
                        ?? User.FindFirst("preferred_username")?.Value
                        ?? User.FindFirst(JwtClaims.Sub)?.Value
                        ?? "unknown";

            var token = _jwtProvider.GenerateBootstrapToken(username, correlationId);

            _logger.LogInformation("Bootstrap token gerado para {Username} com correlationId {CorrelationId}", username, correlationId);

            return Ok(new { token });
        }


        [HttpPost("request-token")]
        public async Task<IActionResult> RequestTokenAsync([FromBody] AuthRequest request)
        {
            if (!_totpService.ValidateCode(request.TotpCode))
            {
                _logger.LogWarning("TOTP inválido");
                return Unauthorized("Código TOTP inválido");
            }

            var claims = await GetServiceUserClaimsAsync(BootstraperUserId);
            var token = _jwtProvider.CreateBootstrapToken(claims, TimeSpan.FromMinutes(2));
            return Ok(new { token });
        }


        [HttpPost("login")]
        [Authorize(AuthenticationSchemes = "RequestToken")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
        {
            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");

            if (!_jwtProvider.ValidateTokenForLogin(token, "bootstrap_token", "token_request_only"))
                return Unauthorized("Token de requisição inválido ou expirado.");

            var accessScope = _currentUser.GetClaim("access_scope");
            if (accessScope is not ("token_request_only" or "bootstrap_token"))
                return Forbid("Token não autorizado para login.");


            var keycloakToken = await _authService.AuthenticateAsync(request.Login, request.Password);
            if (keycloakToken == null)
                return Unauthorized("Login ou senha inválidos.");

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(keycloakToken.AccessToken);

            var keycloakUserId = jwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            var preferredUsername = jwt.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
            var email = jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
            var name = jwt.Claims.FirstOrDefault(c => c.Type == "name")?.Value;

            if (string.IsNullOrWhiteSpace(keycloakUserId))
                return Unauthorized("Token inválido. Sub não encontrado.");

            var loginHash = await _hashService.ComputeSha3HashAsync(request.Login);

            await using var db = _contextFactory.CreateDbContext();

            if (!Guid.TryParse(keycloakUserId, out var parsedId))
                throw new InvalidOperationException("KeycloakUserId inválido.");

            var user = await db.UserAccounts
                .Where(u => u.KeycloakUserId == parsedId)
                .OrderByDescending(u => u.ProvisioningAt)
                .FirstOrDefaultAsync();

            if (user is null)
                return Unauthorized("Usuário autenticado, mas não registrado no sistema.");

            var decryptedUser = await _decryptionService.DecryptSensitiveDataAsync(user);

            var authResult = await _jwtProvider.GenerateAuthenticatedAccessTokenAsync(
                decryptedUser.KeycloakUserId,
                preferredUsername,
                email,
                name
            );

            Response.Cookies.Append("access_token", authResult.Jwtoken, new CookieOptions
            {
                HttpOnly = true,                     // protege contra JS malicioso
                Secure = true,                       // só HTTPS
                SameSite = SameSiteMode.Strict,      // bloqueia CSRF cruzado
                Expires = DateTimeOffset.UtcNow.AddHours(1)  // tempo do token
            });

            Response.Cookies.Append("refresh_token", authResult.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });


            return Ok(new
            {
                token = authResult.Jwtoken,
                refreshToken = authResult.RefreshToken
            });
        }

        [HttpPost("refresh-token")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> RefreshTokenAsync()
        {
            var refreshToken = Request.Cookies["refresh_token"];

            if (string.IsNullOrWhiteSpace(refreshToken))
                return Unauthorized("Refresh token ausente");

            var userUpn = User.FindFirstValue(JwtClaims.Sub);

            var (newJwt, newRefresh, success) = await _jwtProvider.RefreshTokenAsync(userUpn, refreshToken);

            if (!success)
                return Unauthorized("Refresh token inválido ou expirado");

            Response.Cookies.Append("access_token", newJwt, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(5)
            });

            Response.Cookies.Append("refresh_token", newRefresh, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            return Ok(new { success = true });
        }

        private async Task<Dictionary<string, object>> GetServiceUserClaimsAsync(string serviceUserId)
        {
            await using var db = _contextFactory.CreateDbContext();

            var claims = await db.ServiceUserClaims
                .Where(c => c.ServiceUserId == serviceUserId)
                .ToDictionaryAsync(c => c.Type, c => (object)c.Value);

            claims[JwtClaims.Sub] = serviceUserId;

            return claims;
        }

    }
    public class AuthRequest
    {
        public string TotpCode { get; set; }

    }
}
