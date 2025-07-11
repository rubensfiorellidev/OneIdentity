using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneID.Application.DTOs.Auth;
using OneID.Application.Interfaces.Keycloak;
using OneID.Data.Interfaces;
using OneID.Shared.Authentication;
using OtpNet;
using JwtClaims = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;


#nullable disable
namespace OneID.Api.Controllers
{
    [Route("v1/auth")]
    public class AuthController : MainController
    {
        private readonly JwtProvider _jwtProvider;
        private readonly ILogger<AuthController> _logger;
        private readonly IOneDbContextFactory _contextFactory;
        private readonly IKeycloakAuthService _authService;

        private const string BootstraperUserId = "01JZTZXJSC1WY70TPPB1SRVQYZ";
        private const string OperatorSecret = "JBSWY3DPEHPK3PXP";
        public AuthController(ISender sender,
                              JwtProvider jwtProvider,
                              ILogger<AuthController> logger,
                              IOneDbContextFactory contextFactory,
                              IKeycloakAuthService authService) : base(sender)
        {
            _jwtProvider = jwtProvider;
            _logger = logger;
            _contextFactory = contextFactory;
            _authService = authService;
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
            // Valida escopo mínimo do token TOTP
            var accessScope = HttpContext.User.FindFirst("access_scope")?.Value;
            if (accessScope != "token_request_only")
                return Forbid("Token não autorizado para login.");

            // 🔐 Autentica no Keycloak
            var keycloakToken = await _authService.AuthenticateAsync(request.Login, request.Password);
            if (keycloakToken == null)
                return Unauthorized("Login ou senha inválidos.");

            var keycloakUserId = keycloakToken.Sub; // ou preferred_username dependendo do seu mapeamento

            // 🎯 Busca claims do seu banco
            await using var db = _contextFactory.CreateDbContext();
            var user = await db.UserAccounts.FirstOrDefaultAsync(u => u.Login == request.Login);

            if (user is null)
                return Unauthorized("Usuário autenticado, mas não registrado no sistema.");

            var claims = await db.UserClaims
                .Where(c => c.UserAccountId == user.Id)
                .ToDictionaryAsync(c => c.Type, c => (object)c.Value);

            claims[JwtClaims.Sub] = user.Id;
            claims[JwtClaims.UniqueName] = user.Login;

            var token = await _jwtProvider.GenerateTokenAsync(user.Login);
            return Ok(token);
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
