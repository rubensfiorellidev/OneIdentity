using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OneID.Application.Interfaces.SMSService;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace OneID.Application.Services.SmsServices
{
    public sealed class SmsService : ISmsService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmsService> _logger;

        public SmsService(IConfiguration configuration, ILogger<SmsService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendTotpConfirmationSmsAsync(string phoneNumber, string callbackUrl, string recipientName)
        {
            var accountSid = _configuration["Twilio:AccountSid"];
            var authToken = _configuration["Twilio:AuthToken"];
            var fromNumber = _configuration["Twilio:From"];

            TwilioClient.Init(accountSid, authToken);

            var body = $"Olá {recipientName}, para continuar o provisionamento acesse:\n{callbackUrl}";

            var message = await MessageResource.CreateAsync(
                to: new PhoneNumber(phoneNumber),
                from: new PhoneNumber(fromNumber),
                body: body);

            _logger.LogInformation("SMS enviado para {Recipient}: SID {Sid}", phoneNumber, message.Sid);
        }
    }

}
