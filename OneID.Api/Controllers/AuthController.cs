using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneID.Application.Commands;
using OneID.Application.DTOs.Auth;
using OneID.Application.Interfaces.Interceptor;
using OneID.Application.Interfaces.Keycloak;
using OneID.Application.Interfaces.SensitiveData;
using OneID.Application.Interfaces.Services;
using OneID.Application.Interfaces.TotpServices;
using OneID.Application.Queries.Auth;
using OneID.Data.Interfaces;
using OneID.Domain.Contracts.Jwt;
using OneID.Domain.Helpers;
using OneID.Domain.Interfaces;
using OneID.Domain.OpenTelemetryEntity;
using System.Diagnostics;
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
        private readonly ITotpService _totpService;
        private readonly IRefreshTokenService _refreshTokenService;

        private const string BootstraperUserId = "01JZTZXJSC1WY70TPPB1SRVQYZ";
        public AuthController(ISender sender,
                              IJwtProvider jwtProvider,
                              ILogger<AuthController> logger,
                              IOneDbContextFactory contextFactory,
                              IKeycloakAuthService authService,
                              IHashService hashService,
                              ISensitiveDataDecryptionServiceUserAccount decryptionService,
                              ICurrentUserService currentUser,
                              ITotpService totpService,
                              IRefreshTokenService refreshTokenService) : base(sender)
        {
            _jwtProvider = jwtProvider;
            _logger = logger;
            _contextFactory = contextFactory;
            _authService = authService;
            _hashService = hashService;
            _decryptionService = decryptionService;
            _currentUser = currentUser;
            _totpService = totpService;
            _refreshTokenService = refreshTokenService;
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me(CancellationToken cancellationToken)
        {
            var query = new GetCurrentUserQuery();
            var result = await Sender.Send(query, cancellationToken);

            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("bootstrap-token")]
        public IActionResult GenerateBootstrapToken(Guid correlationId)
        {
            using var activity = Telemetry.Source.StartActivity(
                "Geração de token bootstrap",
                ActivityKind.Server);

            activity?.SetTag("rota", "v1/auth/bootstrap-token");
            activity?.SetTag("method", "POST");
            activity?.SetTag("correlation_id", correlationId.ToString());
            activity?.SetTag("bootstrap.iniciado_em", DateTimeOffset.UtcNow);

            if (correlationId == Guid.Empty)
            {
                activity?.SetTag("erro", "correlation_id_invalido");
                return BadRequest("correlationId inválido.");
            }

            var token = _jwtProvider.GenerateBootstrapToken("bootstrap_user", correlationId);

            _logger.LogInformation("Bootstrap token gerado com correlationId {CorrelationId}", correlationId);

            activity?.SetTag("bootstrap.sucesso", true);
            activity?.SetTag("bootstrap.finalizado_em", DateTimeOffset.UtcNow);

            return Ok(new { token });
        }


        [HttpPost("request-token")]
        public async Task<IActionResult> RequestTokenAsync([FromBody] AuthRequest request)
        {
            using var activity = Telemetry.Source.StartActivity(
                "TOTP request para acessar o login",
                ActivityKind.Server);

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
        public async Task<IActionResult> LoginAsync(LoginRequest request)
        {
            using var activity = Telemetry.Source.StartActivity(
                "Login do usuário via token de requisição",
                ActivityKind.Server);

            activity?.SetTag("rota", "v1/auth/login");
            activity?.SetTag("method", "POST");
            activity?.SetTag("login.iniciado_em", DateTimeOffset.UtcNow);

            var token = Request.Headers.Authorization.ToString();
            if (string.IsNullOrWhiteSpace(token) || !token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = Request.Cookies["request_token"];
            }
            else
            {
                token = token["Bearer ".Length..].Trim();
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                activity?.SetTag("erro", "token_ausente");
                return Unauthorized("Token ausente. Tente novamente após confirmar o TOTP.");
            }

            if (!_jwtProvider.ValidateTokenForLogin(token, "bootstrap_token", "token_request_only"))
            {
                activity?.SetTag("erro", "token_invalido_ou_expirado");
                return Unauthorized("Token de requisição inválido ou expirado.");
            }

            var accessScope = _currentUser.GetClaim("access_scope");
            activity?.SetTag("access_scope", accessScope);

            if (accessScope is not ("token_request_only" or "bootstrap_token"))
            {
                activity?.SetTag("erro", "token_nao_autorizado");
                return Forbid("Token não autorizado para login.");
            }

            var keycloakToken = await _authService.AuthenticateAsync(request.Login, request.Password);
            if (keycloakToken == null)
            {
                activity?.SetTag("erro", "keycloak_falhou");
                return Unauthorized("Login ou senha inválidos.");
            }

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(keycloakToken.AccessToken);

            var keycloakUserId = jwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            var preferredUsername = jwt.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
            var email = jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
            var name = jwt.Claims.FirstOrDefault(c => c.Type == "name")?.Value;

            if (!string.IsNullOrWhiteSpace(preferredUsername))
                activity?.SetTag("usuario.username", preferredUsername);

            if (!string.IsNullOrWhiteSpace(email))
                activity?.SetTag("usuario.email", email);

            if (!string.IsNullOrWhiteSpace(keycloakUserId))
                activity?.SetTag("usuario.keycloak_user_id", keycloakUserId);
            else
            {
                activity?.SetTag("erro", "sub_nao_encontrado");
                return Unauthorized("Token inválido. Sub não encontrado.");
            }

            var loginHash = await _hashService.ComputeSha3HashAsync(request.Login);
            activity?.SetTag("usuario.login_hash", loginHash);

            await using var db = _contextFactory.CreateDbContext();

            if (!Guid.TryParse(keycloakUserId, out var parsedId))
            {
                activity?.SetTag("erro", "keycloak_id_invalido");
                throw new InvalidOperationException("KeycloakUserId inválido.");
            }

            var user = await db.UserAccounts
                .Where(u => u.KeycloakUserId == parsedId)
                .OrderByDescending(u => u.ProvisioningAt)
                .FirstOrDefaultAsync();

            if (user is null)
            {
                activity?.SetTag("erro", "usuario_nao_registrado");
                return Unauthorized("Usuário autenticado, mas não registrado no sistema.");
            }

            var httpContext = HttpContext;
            var ip = GetClientIpAddress(httpContext);
            var userAgent = httpContext.Request.Headers.UserAgent.ToString();
            var circuitId = httpContext.TraceIdentifier ?? Ulid.NewUlid().ToString();

            var authResult = await _jwtProvider.GenerateAuthenticatedAccessTokenAsync(
                user.KeycloakUserId,
                preferredUsername,
                email,
                name,
                circuitId,
                ip,
                userAgent
            );

            await Sender.Send(new RegisterSessionCommand
            {
                CircuitId = circuitId,
                IpAddress = ip,
                UpnOrName = name ?? email ?? preferredUsername ?? user.Id,
                UserAgent = userAgent,
                LastActivity = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.Add(JwtDefaults.AccessTokenLifetime)

            });

            SetAuthCookies(authResult.Jwtoken, authResult.RefreshToken,
                           circuitId,
                           JwtDefaults.AccessTokenLifetime,
                           JwtDefaults.RefreshTokenLifetime
            );

            activity?.SetTag("login.sucesso", true);
            activity?.SetTag("login.finalizado_em", DateTimeOffset.UtcNow);

            return Ok(new
            {
                token = authResult.Jwtoken,
                refreshToken = authResult.RefreshToken
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync()
        {
            using var activity = Telemetry.Source.StartActivity(
                "Refresh do token de acesso",
                ActivityKind.Server
            );

            activity?.SetTag("rota", "v1/auth/refresh-token");
            activity?.SetTag("method", "POST");
            activity?.SetTag("refresh.iniciado_em", DateTimeOffset.UtcNow);

            _logger.LogInformation("Token de refresh solicitado às {Now}", DateTimeOffset.Now);

            foreach (var cookie in Request.Cookies)
            {
                _logger.LogInformation("Cookie recebido: {Key} = {Value}", cookie.Key, cookie.Value);
            }

            var refreshToken = Request.Cookies["refresh_token"];

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                var authHeader = Request.Headers.Authorization.ToString();
                if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    refreshToken = authHeader["Bearer ".Length..].Trim();
                }
            }

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                activity?.SetTag("erro", "refresh_token_ausente");
                _logger.LogWarning("Refresh token ausente.");
                return Unauthorized("Refresh token inválido.");
            }

            activity?.SetTag("refresh_token.recebido", true);

            var refreshInfo = await _refreshTokenService.GetRefreshTokenAsync(refreshToken);
            if (refreshInfo is null)
            {
                activity?.SetTag("erro", "refresh_token_nao_encontrado");
                _logger.LogWarning("Refresh token não encontrado.");
                return Unauthorized("Refresh token inválido.");
            }

            activity?.SetTag("user.upn_hash", refreshInfo.UserUpnHash);

            var circuitId = HttpContext.TraceIdentifier;

            var (newJwt, newRefresh, success) =
                await _jwtProvider.RefreshTokenAsync(refreshInfo.UserUpnHash, refreshToken, circuitId);

            if (!success)
            {
                activity?.SetTag("erro", "refresh_token_expirado_ou_invalido");
                _logger.LogWarning("Falha no refresh. Token inválido ou expirado.");
                return Unauthorized("Refresh token inválido ou expirado.");
            }

            SetAuthCookies(newJwt, newRefresh,
                           circuitId,
                           JwtDefaults.AccessTokenLifetime,
                           JwtDefaults.RefreshTokenLifetime
            );

            activity?.SetTag("refresh.sucesso", true);
            activity?.SetTag("refresh.finalizado_em", DateTimeOffset.UtcNow);

            return Ok(new
            {
                success = true,
                token = newJwt,
                refreshToken = newRefresh
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

        private void SetAuthCookies(string accessToken,
                                    string refreshToken,
                                    string circuitId,
                                    TimeSpan accessTokenLifetime,
                                    TimeSpan refreshTokenLifetime)
        {
            Response.Cookies.Append("access_token", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.Add(accessTokenLifetime)
            });

            Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.Add(refreshTokenLifetime)
            });

            Response.Cookies.Append("circuit_id", circuitId, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                MaxAge = TimeSpan.FromDays(7)
            });

        }

        private static string GetClientIpAddress(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
            {
                var ipList = forwardedFor.ToString().Split(',');
                if (ipList.Length > 0 && !string.IsNullOrWhiteSpace(ipList[0]))
                    return ipList[0].Trim();
            }

            var remoteIp = context.Connection.RemoteIpAddress?.ToString();

            return remoteIp switch
            {
                "::1" or null => "127.0.0.1",
                _ => remoteIp
            };
        }


    }
    public class AuthRequest
    {
        public string TotpCode { get; set; }

    }


}
