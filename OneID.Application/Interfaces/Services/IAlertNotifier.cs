namespace OneID.Application.Interfaces.Services
{
    public interface IAlertNotifier
    {
        Task SendCriticalAlertAsync(string subject, string message, IEnumerable<string>? recipients = null);
        Task SendWarningAlertAsync(string subject, string message, IEnumerable<string>? recipients = null);
        Task SendInfoAlertAsync(string subject, string message, IEnumerable<string>? recipients = null);
    }
}
