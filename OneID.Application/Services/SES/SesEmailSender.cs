namespace OneID.Application.Services.SES
{
    using Amazon.SimpleEmail;
    using Amazon.SimpleEmail.Model;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using OneID.Application.Interfaces.SES;

    public class SesEmailSender : ISesEmailSender
    {
        private readonly IAmazonSimpleEmailService _sesClient;
        private readonly SesSettings _settings;
        private readonly ILogger<SesEmailSender> _logger;

        public SesEmailSender(IAmazonSimpleEmailService sesClient,
                              IOptions<SesSettings> options,
                              ILogger<SesEmailSender> logger)
        {
            _sesClient = sesClient;
            _settings = options.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(
            string to,
            string subject,
            string? htmlBody = null,
            string? textBody = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(htmlBody) && string.IsNullOrWhiteSpace(textBody))
            {
                throw new ArgumentException("É necessário fornecer htmlBody ou textBody.");
            }

            var body = new Body();

            if (!string.IsNullOrWhiteSpace(textBody))
                body.Text = new Content(textBody);

            if (!string.IsNullOrWhiteSpace(htmlBody))
                body.Html = new Content(htmlBody);

            var sendRequest = new SendEmailRequest
            {
                Source = _settings.SourceEmail,
                Destination = new Destination
                {
                    ToAddresses = [to]
                },
                Message = new Message
                {
                    Subject = new Content(subject),
                    Body = body
                }
            };

            try
            {
                var response = await _sesClient.SendEmailAsync(sendRequest, cancellationToken);
                _logger.LogInformation("E-mail enviado com sucesso. MessageId: {MessageId}", response.MessageId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar e-mail via SES");
                throw;
            }
        }
    }

}
