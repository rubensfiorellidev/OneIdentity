namespace OneID.Application.Interfaces.SES
{

    public interface ISesEmailSender
    {
        Task SendEmailAsync(
            string to,
            string subject,
            string? htmlBody = null,
            string? textBody = null,
            CancellationToken cancellationToken = default);
    }


}
