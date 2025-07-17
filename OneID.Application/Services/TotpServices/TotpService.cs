using Microsoft.Extensions.Options;
using OneID.Application.Interfaces.TotpServices;
using OneID.Domain.Entities.TotpOptions;
using OtpNet;

namespace OneID.Application.Services.TotpServices
{
    public sealed class TotpService : ITotpService
    {
        private readonly TotpOptions _options;

        public TotpService(IOptions<TotpOptions> options)
        {
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public bool ValidateCode(string code)
        {
            var totp = new Totp(Base32Encoding.ToBytes(_options.OperatorSecret));
            return totp.VerifyTotp(code, out _, VerificationWindow.RfcSpecifiedNetworkDelay);
        }
    }

}
