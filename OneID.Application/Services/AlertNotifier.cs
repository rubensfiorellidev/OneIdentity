using Microsoft.Extensions.Logging;
using OneID.Application.Interfaces.Repositories;
using OneID.Application.Interfaces.Services;
using OneID.Application.Interfaces.SES;

#nullable disable
namespace OneID.Application.Services
{
    public class AlertNotifier : IAlertNotifier
    {
        private readonly ISesEmailSender _emailSender;
        private readonly IAlertSettingsRepository _alertSettingsRepository;
        private readonly ILogger<AlertNotifier> _logger;

        public AlertNotifier(
            ISesEmailSender emailSender,
            ILogger<AlertNotifier> logger,
            IAlertSettingsRepository alertSettingsRepository)
        {
            _emailSender = emailSender;
            _logger = logger;
            _alertSettingsRepository = alertSettingsRepository;
        }

        public async Task SendCriticalAlertAsync(string subject, string message, IEnumerable<string> recipients = null)
        {
            var settings = await _alertSettingsRepository.GetAsync();
            await SendAsync(subject, message, recipients ?? settings.CriticalRecipients, "CRITICAL");
        }

        public async Task SendWarningAlertAsync(string subject, string message, IEnumerable<string> recipients = null)
        {
            var settings = await _alertSettingsRepository.GetAsync();
            await SendAsync(subject, message, recipients ?? settings.WarningRecipients, "WARNING");
        }

        public async Task SendInfoAlertAsync(string subject, string message, IEnumerable<string> recipients = null)
        {
            var settings = await _alertSettingsRepository.GetAsync();
            await SendAsync(subject, message, recipients ?? settings.InfoRecipients, "INFO");
        }

        private async Task SendAsync(string subject, string message, IEnumerable<string> recipients, string level)
        {
            foreach (var recipient in recipients.Distinct())
            {
                try
                {
                    await _emailSender.SendEmailAsync(recipient, $"[{level}] {subject}", htmlBody: message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao enviar alerta {Level} para {Recipient}", level, recipient);
                }
            }
        }
    }

}
