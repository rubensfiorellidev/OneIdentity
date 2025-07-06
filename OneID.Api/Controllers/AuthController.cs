using MediatR;
using Microsoft.AspNetCore.Mvc;
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

        private const string OperatorSecret = "JBSWY3DPEHPK3PXP";
        public AuthController(ISender sender,
                              JwtProvider jwtProvider,
                              ILogger<AuthController> logger) : base(sender)
        {
            _jwtProvider = jwtProvider;
            _logger = logger;
        }

        [HttpPost("request-token")]
        public IActionResult RequestToken([FromBody] AuthRequest request)
        {
            var totp = new Totp(Base32Encoding.ToBytes(OperatorSecret));
            if (!totp.VerifyTotp(request.TotpCode, out _, VerificationWindow.RfcSpecifiedNetworkDelay))
            {
                _logger.LogWarning("TOTP inválido");
                return Unauthorized("Código TOTP inválido");
            }

            var claims = new Dictionary<string, object>
        {
            { JwtClaims.Sub, "operator" },
            { "role", "adm-operator" }
        };

            var token = _jwtProvider.GenerateAcceptanceToken(claims, TimeSpan.FromMinutes(2));
            return Ok(new { token });
        }
    }
    public class AuthRequest
    {
        public string TotpCode { get; set; }
    }
}
