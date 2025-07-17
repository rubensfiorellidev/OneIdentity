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
using OneID.Data.Interfaces;
using OneID.Domain.Contracts.Jwt;
using OtpNet;
using System.IdentityModel.Tokens.Jwt;
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

        private const string BootstraperUserId = "01JZTZXJSC1WY70TPPB1SRVQYZ";
        private const string OperatorSecret = "JBSWY3DPEHPK3PXP";
        public AuthController(ISender send,
                              IJwtProvider jwtProvider,
                              ILogger<AuthController> logger,
                              IOneDbContextFactory contextFactory,
                              IKeycloakAuthService authService,
                              IHashService hashService,
                              ISensitiveDataDecryptionServiceUserAccount decryptionService,
                              ICurrentUserService currentUser) : base(send)
        {
            _jwtProvider = jwtProvider;
            _logger = logger;
            _contextFactory = contextFactory;
            _authService = authService;
            _hashService = hashService;
            _decryptionService = decryptionService;
            _currentUser = currentUser;
        }

        [HttpPost("request-token")]
        public async Task<IActionResult> RequestTokenAsync([FromBody] AuthRequest request)
        {
            var totp = new Totp(Base32Encoding.ToBytes(OperatorSecret));
            if (!totp.VerifyTotp(request.TotpCode, out _, VerificationWindow.RfcSpecifiedNetworkDelay))
            {
                _logger.LogWarning("TOTP inválido");
                return Unauthorized("Código TOTP inválido");
            }

            var claims = await GetServiceUserClaimsAsync(BootstraperUserId);
            var token = _jwtProvider.GenerateAcceptanceToken(claims, TimeSpan.FromMinutes(2));
            return Ok(new { token });
        }


        [HttpPost("login")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
        {
            var accessScope = _currentUser.GetClaim("access_scope");
            if (accessScope != "token_request_only")
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

            var authResult = await _jwtProvider.GenerateTokenAsync(
                decryptedUser.KeycloakUserId,
                preferredUsername,
                email,
                name
            );

            return Ok(new
            {
                token = authResult.Jwtoken,
                refreshToken = authResult.RefreshToken
            });
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
