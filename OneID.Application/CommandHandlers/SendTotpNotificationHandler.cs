using Microsoft.Extensions.Options;
using OneID.Application.Commands;
using OneID.Application.Interfaces.CQRS;
using OneID.Application.Interfaces.SES;
using OneID.Application.Interfaces.SMSService;
using OneID.Domain.Contracts.Jwt;
using OneID.Domain.Entities.ApiOptions;
using OneID.Domain.Interfaces;
using OneID.Domain.Results;
using System.IdentityModel.Tokens.Jwt;

namespace OneID.Application.CommandHandlers
{
    public sealed class SendTotpNotificationHandler : ICommandHandler<SendTotpNotificationCommand, IResult>
    {
        private readonly ISesEmailSender _sesEmailSender;
        private readonly ISmsService _smsService;
        private readonly IJwtProvider _jwtProvider;
        private readonly ApiUrlSettings _urlBase;

        public SendTotpNotificationHandler(
            ISmsService smsService,
            IOptions<ApiUrlSettings> options,
            IJwtProvider jwtProvider,
            ISesEmailSender sesEmailSender)
        {
            _smsService = smsService;
            _jwtProvider = jwtProvider;
            _urlBase = options.Value;
            _sesEmailSender = sesEmailSender;
        }

        public async Task<IResult> Handle(SendTotpNotificationCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var now = DateTimeOffset.UtcNow;
                var jwtClaims = new Dictionary<string, object>
                {
                    { JwtRegisteredClaimNames.Sub, command.OperatorEmail },
                    { JwtRegisteredClaimNames.Email, command.OperatorEmail },
                    { JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString() },
                    { JwtRegisteredClaimNames.Iat, now.ToUnixTimeSeconds().ToString() },
                    { JwtRegisteredClaimNames.Nbf, now.ToUnixTimeSeconds().ToString() },
                    { JwtRegisteredClaimNames.Exp, now.AddMinutes(5).ToUnixTimeSeconds().ToString() },
                    { JwtRegisteredClaimNames.Aud, "totp_confirmation" },
                    { JwtRegisteredClaimNames.Iss, "oneid.api" },
                    { "correlation_id", command.CorrelationId },
                    { "purpose", "totp_confirm" },
                    { "scope", "confirm:provisioning" }
                };

                var token = _jwtProvider.GenerateAcceptanceToken(jwtClaims, TimeSpan.FromMinutes(5));
                var callbackUrl = $"{_urlBase.BaseUrl.TrimEnd('/')}/v1/staging/confirm?token={token}";

                var subject = "Confirmação de Provisionamento";
                var textBody = $"""
                Olá {command.OperatorName},

                    Para continuar o provisionamento da conta, acesse o link abaixo e informe o código TOTP:

                    {callbackUrl}

                    Esse link expira em 5 minutos.

                    Se você não iniciou este processo, por favor ignore esta mensagem.
                """;

                await _sesEmailSender.SendEmailAsync(
                    to: command.OperatorEmail,
                    subject: subject,
                    textBody: textBody,
                    cancellationToken: cancellationToken);

                var smsBody = $"Olá {command.OperatorName}, acesse para confirmar o provisionamento: {callbackUrl}";
                await _smsService.SendTotpConfirmationSmsAsync(
                    phoneNumber: command.OperatorPhone,
                    callbackUrl: callbackUrl,
                    recipientName: command.OperatorName);

                return Result.Success("Notificação TOTP enviada com sucesso.");
            }
            catch (Exception ex)
            {
                return Result.Failure($"Falha ao notificar operador: {ex.Message}");
            }
        }
    }

}
