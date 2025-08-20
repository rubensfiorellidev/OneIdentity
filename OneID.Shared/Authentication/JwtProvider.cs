using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using OneID.Application.Commands;
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
        private readonly JwtSecurityTokenHandler _tokenHandler = new();
        private readonly IOneDbContextFactory _contextFactory;
        private readonly IHashService _hash;
        private readonly ISender _sender;

        private readonly string _keyDirectoryPath;
        private readonly string _privateKeyPath;
        private readonly string _publicKeyPath;
        private readonly string _metadataPath;

        private static readonly object _keyLock = new();

        public JwtProvider(IOptions<JwtOptions> jwtOptions,
                           IRefreshTokenService refreshTokenService,
                           IHttpContextAccessor httpContextAccessor,
                           ILogger<JwtProvider> logger,
                           IOneDbContextFactory contextFactory,
                           IHashService hash,
                           ISender sender)
        {
            _jwtOptions = jwtOptions.Value;
            _refreshTokenService = refreshTokenService;
            _httpContextAccessor = httpContextAccessor;

            _keyDirectoryPath = Path.Combine(Environment.CurrentDirectory, "Keys");
            _privateKeyPath = Path.Combine(Environment.CurrentDirectory, _jwtOptions.PrivateKeyPath);
            _publicKeyPath = Path.Combine(Environment.CurrentDirectory, _jwtOptions.PublicKeyPath);
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
            _hash = hash;
            _sender = sender;
        }

        private string GetPemPath(string originalPath) =>
            Path.ChangeExtension(originalPath, ".pem");

        private static string Base64UrlEncode(byte[] data)
        {
            return Convert.ToBase64String(data)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        private KeyMetadata LoadMetadataSafe()
        {
            try
            {
                if (File.Exists(_metadataPath))
                {
                    return JsonConvert.DeserializeObject<KeyMetadata>(File.ReadAllText(_metadataPath));
                }
            }
            catch { /* ignore */ }
            return null;
        }

        private string GetActiveKeyId()
        {
            var meta = LoadMetadataSafe();
            if (!string.IsNullOrWhiteSpace(meta?.KeyId))
                return meta.KeyId;

            var publicPem = File.ReadAllText(GetPemPath(_publicKeyPath));
            using var rsa = RSA.Create();
            rsa.ImportFromPem(publicPem.ToCharArray());
            var spki = rsa.ExportSubjectPublicKeyInfo();
            var hash = SHA256.HashData(spki);
            return Base64UrlEncode(hash);
        }

        private char[] GetPrivateKeyPasswordOrThrow(out bool fromEnv)
        {
            fromEnv = false;
            if (!string.IsNullOrWhiteSpace(_jwtOptions.PrivateKeyPassword))
                return _jwtOptions.PrivateKeyPassword.ToCharArray();

            var env = Environment.GetEnvironmentVariable("JWT_PRIVATE_KEY_PASSWORD");
            if (!string.IsNullOrWhiteSpace(env))
            {
                fromEnv = true;
                return env.ToCharArray();
            }

            throw new InvalidOperationException(
                "Senha da chave privada não configurada. Defina Jwt:PrivateKeyPassword ou a env JWT_PRIVATE_KEY_PASSWORD."
            );
        }

        public async Task<string> EnsureKeysAsync()
        {
            if (!File.Exists(GetPemPath(_publicKeyPath)) || !File.Exists(GetPemPath(_privateKeyPath)))
            {
                GenerateAndSaveKeys();
            }

            return await File.ReadAllTextAsync(GetPemPath(_publicKeyPath));
        }

        public async Task<AuthResult> GenerateAuthenticatedAccessTokenAsync(
                                                         Guid keycloakUserId,
                                                         string preferredUsername = null,
                                                         string email = null,
                                                         string name = null,
                                                         string ipAddress = null,
                                                         string userAgent = null,
                                                         string circuitId = null,
                                                         TimeSpan? accessTokenLifetime = null,
                                                         TimeSpan? refreshTokenLifetime = null)
        {
            circuitId ??= _httpContextAccessor.HttpContext?.Items["circuit_id"]?.ToString()
                            ?? Guid.NewGuid().ToString();

            ipAddress = NormalizeIp(ipAddress);

            userAgent ??= _httpContextAccessor.HttpContext?.Request?.Headers.UserAgent.FirstOrDefault()
                        ?? "unknown-client";

            var handler = new JsonWebTokenHandler();
            var key = GetRSAKey();
            key.KeyId = GetActiveKeyId();

            var accessJti = Ulid.NewUlid().ToString();
            var refreshJti = Ulid.NewUlid().ToString();

            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);

            await using var db = _contextFactory.CreateDbContext();
            var user = await db.UserAccounts
                .FirstOrDefaultAsync(u => u.KeycloakUserId == keycloakUserId)
                ?? throw new SecurityTokenException("Usuário não encontrado com base no KeycloakUserId");

            var userId = user.Id;

            var claims = new List<Claim>
            {
                new(JwtClaims.Jti, accessJti),
                new(JwtClaims.Sub, userId),
                new(JwtClaims.UniqueName, preferredUsername ?? userId),
                new(JwtClaims.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new("ip", ipAddress),
                new("user_agent", userAgent),
                new("account_id", $"ONE-{userId.ToUpper()}"),
                new("circuit_id", circuitId),
                new("access_scope", "user_access")
            };

            if (!string.IsNullOrWhiteSpace(email)) claims.Add(new Claim("email", email));
            if (!string.IsNullOrWhiteSpace(name)) claims.Add(new Claim("name", name));

            var customClaims = await GetUserClaimsAsync(userId);
            claims.AddRange(customClaims);

            var issuedAt = DateTime.UtcNow;
            var expiresAt = issuedAt.Add(accessTokenLifetime ?? JwtDefaults.AccessTokenLifetime);

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                IssuedAt = issuedAt,
                NotBefore = issuedAt,
                Expires = expiresAt,
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = signingCredentials
            };

            var jws = handler.CreateToken(descriptor);

            var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(
                user.LoginHash,
                refreshJti,
                ipAddress,
                userAgent,
                circuitId,
                refreshTokenLifetime ?? JwtDefaults.RefreshTokenLifetime
            );

            await _sender.Send(new RegisterSessionCommand
            {
                CircuitId = circuitId,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                LastActivity = DateTime.UtcNow,
                ExpiresAt = expiresAt,
                UpnOrName = user.FullName ?? preferredUsername ?? user.Login,
            });

            return new AuthResult
            {
                Jwtoken = jws,
                RefreshToken = refreshToken.RawToken!,
                Result = true,
                ExpiresAt = expiresAt,
                CircuitId = circuitId
            };
        }

        private RsaSecurityKey GetRSAKey()
        {
            EnsureKeys();

            var privatePem = File.ReadAllText(GetPemPath(_privateKeyPath));
            var rsa = RSA.Create();

            if (privatePem.Contains("BEGIN ENCRYPTED PRIVATE KEY", StringComparison.OrdinalIgnoreCase))
            {
                bool fromEnv;
                var pwd = GetPrivateKeyPasswordOrThrow(out fromEnv);
                try
                {
                    rsa.ImportFromEncryptedPem(privatePem, pwd);
                }
                finally
                {
                    Array.Clear(pwd, 0, pwd.Length);
                    if (fromEnv) Environment.SetEnvironmentVariable("JWT_PRIVATE_KEY_PASSWORD", null);
                }
            }
            else
            {
                rsa.ImportFromPem(privatePem.ToCharArray());
            }

            return new RsaSecurityKey(rsa);
        }

        private void EnsureKeys()
        {
            try
            {
                lock (_keyLock)
                {
                    var privPem = GetPemPath(_privateKeyPath);
                    var pubPem = GetPemPath(_publicKeyPath);

                    if (!File.Exists(privPem) || !File.Exists(pubPem) || ShouldRotateKeys())
                    {
                        GenerateAndSaveKeys();
                        EnsureMetadata();
                    }
                    else if (!File.Exists(_metadataPath))
                    {
                        EnsureMetadata();
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        private void GenerateAndSaveKeys()
        {
            if (!Directory.Exists(_keyDirectoryPath))
                Directory.CreateDirectory(_keyDirectoryPath);

            using var rsa = RSA.Create(Math.Max(_jwtOptions.KeySize, 3072));

            // pública (sempre plaintext)
            var publicKeyPem = ExportPublicKeyAsPem(rsa);
            File.WriteAllText(GetPemPath(_publicKeyPath), publicKeyPem);

            // privada (PKCS#8 criptografada com senha)
            bool fromEnv;
            var pwd = GetPrivateKeyPasswordOrThrow(out fromEnv);
            try
            {
                var privateKeyPemEncrypted = ExportEncryptedPrivateKeyAsPem(rsa, pwd);
                File.WriteAllText(GetPemPath(_privateKeyPath), privateKeyPemEncrypted);
            }
            finally
            {
                Array.Clear(pwd, 0, pwd.Length);
                if (fromEnv) Environment.SetEnvironmentVariable("JWT_PRIVATE_KEY_PASSWORD", null);
            }

            // ===== metadata forte: ULID + salt + kid = b64url(sha256(ULID || salt || SPKI))
            using var rsaPub = RSA.Create();
            rsaPub.ImportFromPem(publicKeyPem.ToCharArray());
            var spki = rsaPub.ExportSubjectPublicKeyInfo();

            var keyVersionUlid = Ulid.NewUlid().ToString();
            var saltBytes = RandomNumberGenerator.GetBytes(32);
            var kvBytes = Encoding.UTF8.GetBytes(keyVersionUlid);

            var raw = new byte[kvBytes.Length + saltBytes.Length + spki.Length];
            Buffer.BlockCopy(kvBytes, 0, raw, 0, kvBytes.Length);
            Buffer.BlockCopy(saltBytes, 0, raw, kvBytes.Length, saltBytes.Length);
            Buffer.BlockCopy(spki, 0, raw, kvBytes.Length + saltBytes.Length, spki.Length);

            var kid = Base64UrlEncode(SHA256.HashData(raw));

            var metadata = new KeyMetadata(
                CreatedAt: DateTimeOffset.UtcNow,
                KeySize: Math.Max(_jwtOptions.KeySize, 3072),
                KeyId: kid,
                KeyVersion: keyVersionUlid,
                Salt: Base64UrlEncode(saltBytes)
            );

            File.WriteAllText(_metadataPath, JsonConvert.SerializeObject(metadata));
        }

        private bool ShouldRotateKeys()
        {
            if (!File.Exists(_metadataPath))
                return true;

            try
            {
                var meta = JsonConvert.DeserializeObject<KeyMetadata>(File.ReadAllText(_metadataPath));
                var createdAt = meta?.CreatedAt ?? DateTimeOffset.MinValue;
                return (DateTimeOffset.UtcNow - createdAt).TotalDays >= 90;
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
            return Base64UrlEncode(hash);
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
            key.KeyId = GetActiveKeyId();

            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);

            if (!claims.ContainsKey("jti"))
                claims["jti"] = Ulid.NewUlid().ToString();

            claims["access_scope"] = "bootstrap_token";

            var claimsIdentity = new ClaimsIdentity(claims.Select(c =>
                new Claim(c.Key, c.Value?.ToString() ?? string.Empty)));

            var now = DateTime.UtcNow;

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                IssuedAt = now,
                NotBefore = now,
                Expires = now.Add(validFor ?? _jwtOptions.AccessTokenTotpExpires),
                Subject = claimsIdentity,
                SigningCredentials = signingCredentials
            };

            return handler.CreateToken(descriptor);
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidAudience = _jwtOptions.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = GetPublicKey()
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
            var publicPem = File.ReadAllText(GetPemPath(_publicKeyPath));
            var rsa = RSA.Create();
            rsa.ImportFromPem(publicPem.ToCharArray());
            var key = new RsaSecurityKey(rsa)
            {
                KeyId = GetActiveKeyId()
            };
            return key;
        }

        public IDictionary<string, object> DecodeToken(string token)
        {
            var jwtToken = _tokenHandler.ReadJwtToken(token);
            return jwtToken.Claims.ToDictionary(c => c.Type, c => (object)c.Value);
        }

        private void EnsureMetadata()
        {
            if (File.Exists(_metadataPath)) return;

            try
            {
                var publicPem = File.ReadAllText(GetPemPath(_publicKeyPath));
                using var rsa = RSA.Create();
                rsa.ImportFromPem(publicPem.ToCharArray());
                var spki = rsa.ExportSubjectPublicKeyInfo();

                var keyVersionUlid = Ulid.NewUlid().ToString();
                var saltBytes = RandomNumberGenerator.GetBytes(32);
                var kvBytes = Encoding.UTF8.GetBytes(keyVersionUlid);

                var raw = new byte[kvBytes.Length + saltBytes.Length + spki.Length];
                Buffer.BlockCopy(kvBytes, 0, raw, 0, kvBytes.Length);
                Buffer.BlockCopy(saltBytes, 0, raw, kvBytes.Length, saltBytes.Length);
                Buffer.BlockCopy(spki, 0, raw, kvBytes.Length + saltBytes.Length, spki.Length);

                var kid = Base64UrlEncode(SHA256.HashData(raw));

                var metadata = new KeyMetadata(
                    CreatedAt: DateTimeOffset.UtcNow,
                    KeySize: Math.Max(_jwtOptions.KeySize, 3072),
                    KeyId: kid,
                    KeyVersion: keyVersionUlid,
                    Salt: Base64UrlEncode(saltBytes)
                );

                File.WriteAllText(_metadataPath, JsonConvert.SerializeObject(metadata));
                _logger.LogInformation("Metadata do par de chaves criado em {CreatedAt}", metadata.CreatedAt);
            }
            catch
            {
                var fallback = new { CreatedAt = DateTimeOffset.UtcNow };
                File.WriteAllText(_metadataPath, JsonConvert.SerializeObject(fallback));
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
            string existingCircuitId)
        {
            await using var db = _contextFactory.CreateDbContext();

            var accountUser = await db.UserAccounts
                .FirstOrDefaultAsync(x => x.LoginHash == userUpnHash);

            if (accountUser == null)
                return (null, null, false);

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
            await db.SaveChangesAsync();

            var excess = await db.RefreshWebTokens
                .Where(t => t.UserUpnHash == userUpnHash && !t.IsUsed && !t.IsRevoked)
                .OrderByDescending(t => t.CreatedAt)
                .Skip(4)
                .ToListAsync();

            foreach (var item in excess)
                PatchRevoked(db, item);

            var resolvedCircuitId =
                _httpContextAccessor.HttpContext?.Items["circuit_id"]?.ToString()
                ?? existing.CircuitId
                ?? Guid.NewGuid().ToString();

            _httpContextAccessor.HttpContext?.Items.TryAdd("circuit_id", resolvedCircuitId);

            if (string.IsNullOrWhiteSpace(resolvedCircuitId))
            {
                if (!string.IsNullOrWhiteSpace(existingCircuitId))
                {
                    resolvedCircuitId = existingCircuitId;
                    await _refreshTokenService.PatchCircuitIdIfMissingAsync(existing.Id, resolvedCircuitId);
                }
                else
                {
                    _logger.LogError("CircuitId ausente no RefreshToken e também não foi fornecido pelo frontend.");
                    throw new InvalidOperationException("CircuitId não pode ser nulo durante o refresh.");
                }
            }

            await _refreshTokenService.PatchCircuitIdIfMissingAsync(existing.Id, resolvedCircuitId);

            _httpContextAccessor.HttpContext.Items["circuit_id"] = resolvedCircuitId;

            var authResult = await GenerateAuthenticatedAccessTokenAsync(
                user.KeycloakUserId,
                user.Login,
                user.CorporateEmail,
                user.FullName,
                ipAddress,
                userAgent,
                resolvedCircuitId
            );

            authResult.CircuitId = existingCircuitId;

            await db.SaveChangesAsync();

            await _sender.Send(new RegisterSessionCommand
            {
                CircuitId = authResult.CircuitId,
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

        private string NormalizeIp(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip) || ip == "::1")
            {
                return GetClientIpAddress(_httpContextAccessor.HttpContext);
            }

            return ip;
        }

        private static string ExportPrivateKeyAsPem(RSA rsa)
        {
            var privateKeyBytes = rsa.ExportPkcs8PrivateKey();
            var b64 = Convert.ToBase64String(privateKeyBytes, Base64FormattingOptions.InsertLineBreaks);
            return $"-----BEGIN PRIVATE KEY-----\n{b64}\n-----END PRIVATE KEY-----\n";
        }

        private static string ExportPublicKeyAsPem(RSA rsa)
        {
            var publicKeyBytes = rsa.ExportSubjectPublicKeyInfo();
            var b64 = Convert.ToBase64String(publicKeyBytes, Base64FormattingOptions.InsertLineBreaks);
            return $"-----BEGIN PUBLIC KEY-----\n{b64}\n-----END PUBLIC KEY-----\n";
        }

        private static string ExportEncryptedPrivateKeyAsPem(RSA rsa, ReadOnlySpan<char> password)
        {
            var pbe = new PbeParameters(
                PbeEncryptionAlgorithm.Aes256Cbc,
                HashAlgorithmName.SHA256,
                iterationCount: 200_000
            );

            var encrypted = rsa.ExportEncryptedPkcs8PrivateKey(password, pbe);
            var b64 = Convert.ToBase64String(encrypted, Base64FormattingOptions.InsertLineBreaks);
            return $"-----BEGIN ENCRYPTED PRIVATE KEY-----\n{b64}\n-----END ENCRYPTED PRIVATE KEY-----\n";
        }

    }

}
