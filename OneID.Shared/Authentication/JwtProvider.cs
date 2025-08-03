using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using OneID.Application.Commands;
using OneID.Application.Interfaces.SensitiveData;
using OneID.Application.Interfaces.Services;
using OneID.Data.DataContexts;
using OneID.Data.Interfaces;
using OneID.Domain.Contracts.Jwt;
using OneID.Domain.Entities.JwtWebTokens;
using OneID.Domain.Helpers;
using OneID.Domain.Interfaces;
using OneID.Domain.Results;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
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
        private readonly IHashService _hash;
        private readonly ISender _sender;

        public JwtProvider(IOptions<JwtOptions> jwtOptions,
                           IRefreshTokenService refreshTokenService,
                           IHttpContextAccessor httpContextAccessor,
                           ILogger<JwtProvider> logger,
                           IOneDbContextFactory contextFactory,
                           ISensitiveDataDecryptionServiceUserAccount decryptionService,
                           IHashService hash,
                           ISender sender)
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
            _hash = hash;
            _sender = sender;
        }
        public async Task<string> EnsureKeysAsync()
        {
            if (!File.Exists(_publicKeyPath) || !File.Exists(_privateKeyPath))
            {
                GenerateAndSaveKeys();
            }

            return await File.ReadAllTextAsync(_publicKeyPath);
        }

        public async Task<AuthResult> GenerateAuthenticatedAccessTokenAsync(Guid keycloakUserId,
                                                         string preferredUsername = null,
                                                         string email = null,
                                                         string name = null,
                                                         string circuitId = null,
                                                         string ipAddress = null,
                                                         string userAgent = null)
        {

            circuitId ??= GenerateCircuitId(preferredUsername ?? email ?? name ?? "anonymous");

            ipAddress = NormalizeIp(ipAddress);

            userAgent ??= _httpContextAccessor.HttpContext?.Request?.Headers.UserAgent.FirstOrDefault()
                ?? "unknown-client";


            var handler = new JsonWebTokenHandler();
            RsaSecurityKey key = GetRSAKey();

            string keyId = GenerateKeyId(key);
            var jti = Ulid.NewUlid().ToString();
            var jwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(key);
            jwk.KeyId = keyId;
            jwk.Alg = SecurityAlgorithms.RsaSha256;


            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);

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
                new("account_id", $"ONE-{userId.ToUpper()}"),
                new("circuit_id", circuitId)

            };

            if (!string.IsNullOrWhiteSpace(email))
                claims.Add(new Claim("email", email));

            if (!string.IsNullOrWhiteSpace(name))
                claims.Add(new Claim("name", name));

            claims.Add(new Claim("access_scope", "user_access"));

            var customClaims = await GetUserClaimsAsync(userId);

            claims.AddRange(customClaims);

            var expiresAt = DateTime.UtcNow.Add(JwtDefaults.AccessTokenLifetime);

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                NotBefore = DateTime.UtcNow,
                Expires = expiresAt,
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = signingCredentials
            };

            var jws = handler.CreateToken(descriptor);

            var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(
                user.LoginHash,
                jti,
                ipAddress,
                userAgent,
                circuitId
            );

            return new AuthResult
            {
                Jwtoken = jws,
                RefreshToken = refreshToken.TokenHash,
                Result = true,
                ExpiresAt = expiresAt
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
                if (ipAddresses.Length > 0 && !string.IsNullOrWhiteSpace(ipAddresses[0]))
                {
                    return ipAddresses[0].Trim();
                }
            }

            var remoteIp = httpContext.Connection.RemoteIpAddress?.ToString()?.Trim();

            return remoteIp is null || remoteIp == "::1"
                ? "127.0.0.1"
                : remoteIp;
        }

        public string CreateBootstrapToken(Dictionary<string, object> claims, TimeSpan? validFor)
        {
            var handler = new JsonWebTokenHandler();
            var key = GetRSAKey();

            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);

            if (!claims.ContainsKey("jti"))
                claims["jti"] = Ulid.NewUlid().ToString();

            claims["access_scope"] = "bootstrap_token";

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

        public bool ValidateTokenForLogin(string token, params string[] validScopes)
        {

            var principal = ValidateToken(token);
            if (principal == null)
                return false;

            var scopeClaim = principal.FindFirst("access_scope")?.Value.ToLowerInvariant();
            return validScopes.Contains(scopeClaim);
        }



        public async Task<(string NewJwt, string NewRefresh, bool Success)> RefreshTokenAsync(
            string userUpnHash,
            string refreshToken,
            string circuitId)
        {
            await using var db = _contextFactory.CreateDbContext();

            var httpContext = _httpContextAccessor.HttpContext;
            var ipAddress = GetClientIpAddress(httpContext);
            var userAgent = httpContext?.Request?.Headers.UserAgent.FirstOrDefault() ?? "unknown-client";

            var tokenCandidates = await db.RefreshWebTokens
                .Where(x =>
                    x.UserUpnHash == userUpnHash &&
                    !x.IsRevoked &&
                    !x.IsUsed &&
                    x.ExpiresAt > DateTimeOffset.UtcNow)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            RefreshWebToken existing = null;

            foreach (var candidate in tokenCandidates)
            {
                var hashed = await _hash.ComputeSha3HashAsync(refreshToken, candidate.TokenSalt);
                if (hashed == candidate.TokenHash)
                {
                    existing = candidate;
                    break;
                }
            }

            if (existing == null)
                return ("", "", false);

            var user = await db.UserAccounts
                .Where(u => u.LoginHash == userUpnHash)
                .OrderByDescending(u => u.ProvisioningAt)
                .FirstOrDefaultAsync();

            if (user == null)
                return ("", "", false);

            PatchUsed(db, existing);

            var excess = await db.RefreshWebTokens
                .Where(t => t.UserUpnHash == userUpnHash && !t.IsUsed && !t.IsRevoked)
                .OrderByDescending(t => t.CreatedAt)
                .Skip(4)
                .ToListAsync();

            foreach (var item in excess)
                PatchRevoked(db, item);

            var resolvedCircuitId = existing.CircuitId ?? circuitId ?? httpContext?.TraceIdentifier ?? Ulid.NewUlid().ToString();
            await _refreshTokenService.PatchCircuitIdIfMissingAsync(existing.Id, circuitId);

            var authResult = await GenerateAuthenticatedAccessTokenAsync(
                user.KeycloakUserId,
                user.Login,
                user.CorporateEmail,
                user.FullName,
                resolvedCircuitId,
                ipAddress,
                userAgent
            );

            await db.SaveChangesAsync();

            await _sender.Send(new RegisterSessionCommand
            {
                CircuitId = resolvedCircuitId,
                IpAddress = GetClientIpAddress(_httpContextAccessor.HttpContext),
                UpnOrName = user.FullName ?? user.CorporateEmail ?? user.Login,
                UserAgent = userAgent,
                LastActivity = DateTimeOffset.UtcNow,
                ExpiresAt = authResult.ExpiresAt
            });

            return (NewJwt: authResult.Jwtoken, NewRefresh: authResult.RefreshToken, Success: true);
        }

        private string GenerateScopedJwt(string username, Guid correlationId, string accessScope, TimeSpan? lifetime = null)
        {
            var claims = new Dictionary<string, object>
            {
                { JwtClaims.Jti, Ulid.NewUlid().ToString() },
                { JwtClaims.UniqueName, username },
                { JwtClaims.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds() },
                { "access_scope", accessScope },
                { "correlation_id", correlationId.ToString() }
            };

            return CreateBootstrapToken(claims, lifetime ?? TimeSpan.FromMinutes(2));
        }

        public string GenerateInitialRequestTokenAsync(string username, Guid correlationId)
            => GenerateScopedJwt(username, correlationId, "token_request_only");

        public string GenerateUserAccessTokenAsync(string username, Guid correlationId)
            => GenerateScopedJwt(username, correlationId, "user_access");

        public string GenerateBootstrapTokenAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            var ip = GetClientIpAddress(httpContext);
            var userAgent = httpContext?.Request?.Headers.TryGetValue("User-Agent", out var ua) == true
                ? ua.ToString()
                : "unknown";

            var claims = new Dictionary<string, object>
            {
                { JwtClaims.Jti, Ulid.NewUlid().ToString() },
                { JwtClaims.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds() },
                { "access_scope", "bootstrap_token" },
                { "ip", ip },
                { "user_agent", userAgent }
            };

            return CreateBootstrapToken(claims, TimeSpan.FromMinutes(5));
        }

        public string GenerateBootstrapToken(string username, Guid correlationId, TimeSpan? lifetime = null)
        {
            return GenerateScopedJwt(username, correlationId, "bootstrap_token", lifetime ?? TimeSpan.FromMinutes(2));
        }

        private static void PatchUsed(OneDbContext db, RefreshWebToken token)
        {
            var entry = db.Entry(token);
            entry.Property(x => x.IsUsed).CurrentValue = true;
            entry.Property(x => x.IsUsed).IsModified = true;
        }

        private static void PatchRevoked(OneDbContext db, RefreshWebToken token)
        {
            var entry = db.Entry(token);
            entry.Property(x => x.IsRevoked).CurrentValue = true;
            entry.Property(x => x.IsRevoked).IsModified = true;
        }

        private static string GenerateCircuitId(string userIdentifier)
        {
            var timestamp = DateTimeOffset.UtcNow.ToString("yyyyMMddHHmmss");
            var raw = $"{userIdentifier}_{timestamp}";
            var bytes = Encoding.UTF8.GetBytes(raw);
            var hash = SHA256.HashData(bytes);
            return Convert.ToHexString(hash)[..16];
        }

        private string NormalizeIp(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip) || ip == "::1")
            {
                return GetClientIpAddress(_httpContextAccessor.HttpContext);
            }

            return ip;
        }

    }

}
