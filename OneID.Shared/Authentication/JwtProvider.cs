using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using OneID.Application.Interfaces.SensitiveData;
using OneID.Data.Interfaces;
using OneID.Domain.Contracts.Jwt;
using OneID.Domain.Interfaces;
using OneID.Domain.Results;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using JwtClaims = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;


#nullable disable
namespace OneID.Shared.Authentication
{
    public sealed class JwtProvider : IJwtProvider
    {
        private readonly JwtOptions _jwtOptions;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<JwtProvider> _logger;
        private readonly string _keyDirectoryPath;
        private readonly string _privateKeyPath;
        private readonly string _publicKeyPath;
        private readonly string _metadataPath;
        private readonly JwtSecurityTokenHandler _tokenHandler = new();
        private readonly IOneDbContextFactory _contextFactory;
        private readonly ISensitiveDataDecryptionServiceUserAccount _decryptionService;

        public JwtProvider(IOptions<JwtOptions> jwtOptions,
                           IRefreshTokenService refreshTokenService,
                           IHttpContextAccessor httpContextAccessor,
                           ILogger<JwtProvider> logger,
                           IOneDbContextFactory contextFactory,
                           ISensitiveDataDecryptionServiceUserAccount decryptionService)
        {
            _jwtOptions = jwtOptions.Value;
            _refreshTokenService = refreshTokenService;
            _httpContextAccessor = httpContextAccessor;

            _keyDirectoryPath = Path.Combine(Environment.CurrentDirectory, "Keys");
            _privateKeyPath = Path.Combine(_keyDirectoryPath, "PrivateRsaKey.xml");
            _publicKeyPath = Path.Combine(_keyDirectoryPath, "PublicRsaKey.xml");
            _metadataPath = Path.Combine(_keyDirectoryPath, "key-metadata.json");

            try
            {
                EnsureKeys();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao garantir chaves RSA: {ex.Message}");
                throw;
            }

            _logger = logger;
            _contextFactory = contextFactory;
            _decryptionService = decryptionService;
        }
        public async Task<string> EnsureKeysAsync()
        {
            if (!File.Exists(_publicKeyPath) || !File.Exists(_privateKeyPath))
            {
                GenerateAndSaveKeys();
            }

            return await File.ReadAllTextAsync(_publicKeyPath);
        }
        public async Task<AuthResult> GenerateTokenAsync(Guid keycloakUserId,
                                                         string preferredUsername = null,
                                                         string email = null,
                                                         string name = null)
        {
            var handler = new JsonWebTokenHandler();
            RsaSecurityKey key = GetRSAKey();

            string keyId = GenerateKeyId(key);
            var jti = Ulid.NewUlid().ToString();
            var jwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(key);
            jwk.KeyId = keyId;
            jwk.Alg = SecurityAlgorithms.RsaSha256;


            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);

            var httpContext = _httpContextAccessor.HttpContext;
            var ipAddress = GetClientIpAddress(httpContext);
            var userAgent = httpContext?.Request.Headers.UserAgent.ToString() ?? "unknown";

            await using var db = _contextFactory.CreateDbContext();

            var user = await db.UserAccounts
                .FirstOrDefaultAsync(u => u.KeycloakUserId == keycloakUserId)
                ?? throw new SecurityTokenException("Usuário não encontrado com base no KeycloakUserId");

            var userId = user.Id;

            var claims = new List<Claim>
            {
                new(JwtClaims.Jti, jti),
                new(JwtClaims.Sub, userId),
                new(JwtClaims.UniqueName, preferredUsername ?? userId),
                new(JwtClaims.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new("ip", ipAddress),
                new("user_agent", userAgent),
                new("account_id", $"ONE-{userId.ToUpper()}")

            };

            if (!string.IsNullOrWhiteSpace(email))
                claims.Add(new Claim("email", email));

            if (!string.IsNullOrWhiteSpace(name))
                claims.Add(new Claim("name", name));

            var customClaims = await GetUserClaimsAsync(userId);
            claims.AddRange(customClaims);

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.Add(_jwtOptions.AccessTokenExpires),
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = signingCredentials
            };

            var jws = handler.CreateToken(descriptor);

            var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(userId, jws);

            return new AuthResult
            {
                Jwtoken = jws,
                RefreshToken = refreshToken.Token,
                Result = true
            };
        }

        private RsaSecurityKey GetRSAKey()
        {
            EnsureKeys();

            string xmlKey = File.ReadAllText(_privateKeyPath);

            var rsa = RSA.Create();
            rsa.FromXmlString(xmlKey);

            return new RsaSecurityKey(rsa);
        }

        private void EnsureKeys()
        {
            try
            {
                if (!File.Exists(_privateKeyPath) || ShouldRotateKeys())
                {
                    GenerateAndSaveKeys();
                    EnsureMetadata();
                }
                else if (!File.Exists(_metadataPath))
                {
                    EnsureMetadata();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void GenerateAndSaveKeys()
        {
            if (!Directory.Exists(_keyDirectoryPath))
            {
                Directory.CreateDirectory(_keyDirectoryPath);
            }

            using var rsa = RSA.Create(2048);
            string privateRsaKeyXml = rsa.ToXmlString(true);
            string publicRsaKeyXml = rsa.ToXmlString(false);

            File.WriteAllText(_privateKeyPath, privateRsaKeyXml);
            File.WriteAllText(_publicKeyPath, publicRsaKeyXml);

            var metadata = new { CreatedAt = DateTime.UtcNow };
            File.WriteAllText(_metadataPath, JsonConvert.SerializeObject(metadata));

        }

        private bool ShouldRotateKeys()
        {
            if (!File.Exists(_metadataPath))
            {
                return true;
            }

            try
            {
                var metadata = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(_metadataPath));
                DateTime createdAt = metadata?.CreatedAt ?? DateTime.MinValue;

                return (DateTime.UtcNow - createdAt).TotalDays >= 90;
            }
            catch
            {
                return true;
            }
        }

        private string GenerateKeyId(RsaSecurityKey rsaKey)
        {
            var keyBytes = rsaKey.Rsa.ExportSubjectPublicKeyInfo();
            var hash = SHA256.HashData(keyBytes);
            return Convert.ToBase64String(hash);
        }

        private string GetClientIpAddress(HttpContext httpContext)
        {
            if (httpContext.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
            {
                var ipAddresses = forwardedFor.ToString().Split(',');
                if (ipAddresses.Length > 0)
                {
                    return ipAddresses[0];
                }
            }

            return httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
        public string GenerateAcceptanceToken(Dictionary<string, object> claims, TimeSpan? validFor)
        {
            var handler = new JsonWebTokenHandler();
            var key = GetRSAKey();

            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);

            if (!claims.ContainsKey("jti"))
                claims["jti"] = Ulid.NewUlid().ToString();

            claims["access_scope"] = "token_request_only";

            var claimsIdentity = new ClaimsIdentity(claims.Select(c =>
                new Claim(c.Key, c.Value?.ToString() ?? string.Empty)));

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.Add(validFor ?? _jwtOptions.AccessTokenTotpExpires),
                Subject = claimsIdentity,
                SigningCredentials = signingCredentials
            };

            return handler.CreateToken(descriptor);
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = GetRSAKey();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidAudience = _jwtOptions.Audience,
                IssuerSigningKey = key,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Erro ao validar token de aceite.");
                return null;
            }
        }

        public RsaSecurityKey GetPublicKey()
        {
            string xmlKey = File.ReadAllText(_publicKeyPath);

            var rsa = RSA.Create();
            rsa.FromXmlString(xmlKey);

            return new RsaSecurityKey(rsa);
        }

        public IDictionary<string, object> DecodeToken(string token)
        {
            var jwtToken = _tokenHandler.ReadJwtToken(token);
            return jwtToken.Claims.ToDictionary(c => c.Type, c => (object)c.Value);
        }

        private void EnsureMetadata()
        {
            if (!File.Exists(_metadataPath))
            {
                var metadata = new { CreatedAt = DateTime.UtcNow };
                File.WriteAllText(_metadataPath, JsonConvert.SerializeObject(metadata));
                _logger.LogInformation("Metadata do par de chaves criado em {CreatedAt}", metadata.CreatedAt);
            }
        }

        private async Task<List<Claim>> GetUserClaimsAsync(string userId)
        {
            await using var db = _contextFactory.CreateDbContext();

            var claims = await db.UserClaims
                .Where(c => c.UserAccountId == userId)
                .Select(c => new Claim(c.Type, c.Value))
                .ToListAsync();

            var roles = await db.UserRoles
                .Where(ur => ur.UserAccountId == userId)
                .Select(ur => ur.Role.Name)
                .ToListAsync();

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            if (roles.Any())
            {
                claims.Add(new Claim("roles", JsonConvert.SerializeObject(roles)));
            }

            return claims;
        }

        public bool ValidateRequestToken(string token)
        {
            var principal = ValidateToken(token);

            if (principal == null)
                return false;

            var scopeClaim = principal.FindFirst("access_scope")?.Value;
            return scopeClaim == "token_request_only";
        }

        public async Task<(string Token, string RefreshToken, bool Success)> RefreshTokenAsync(string userUpn, string refreshToken)
        {
            if (!Guid.TryParse(userUpn, out var userGuid))
                return ("", "", false);

            await using var db = _contextFactory.CreateDbContext();

            var user = await db.UserAccounts
                .Where(u => u.KeycloakUserId == userGuid)
                .OrderByDescending(u => u.ProvisioningAt)
                .FirstOrDefaultAsync();

            if (user == null)
                return ("", "", false);

            var existing = await db.RefreshWebTokens
                .Where(x =>
                    x.UserUpn == userUpn &&
                    x.Token == refreshToken &&
                    !x.IsRevoked &&
                    !x.IsUsed &&
                    x.ExpiresAt > DateTimeOffset.UtcNow)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();

            if (existing == null)
                return ("", "", false);

            existing.IsUsed = true;

            var decrypted = await _decryptionService.DecryptSensitiveDataAsync(user);

            var authResult = await GenerateTokenAsync(
                decrypted.KeycloakUserId,
                decrypted.Login,
                decrypted.CorporateEmail,
                decrypted.FullName
            );

            var newJwt = authResult.Jwtoken;
            var newRefreshToken = authResult.RefreshToken;


            await db.SaveChangesAsync();

            return (newJwt, newRefreshToken, true);
        }

        public string GenerateRequestToken(string username, Guid correlationId)
        {
            var claims = new Dictionary<string, object>
            {
                { JwtClaims.Jti, Ulid.NewUlid().ToString() },
                { JwtClaims.UniqueName, username },
                { JwtClaims.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds() },
                { "access_scope", "token_request_only" },
                { "correlation_id", correlationId.ToString() }
            };

            return GenerateAcceptanceToken(claims, TimeSpan.FromMinutes(5));
        }

    }



}
