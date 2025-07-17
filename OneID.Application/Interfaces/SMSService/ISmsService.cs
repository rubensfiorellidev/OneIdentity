namespace OneID.Application.Interfaces.SMSService
{
    public interface ISmsService
    {
        Task SendTotpConfirmationSmsAsync(string phoneNumber, string callbackUrl, string recipientName);
    }

}
