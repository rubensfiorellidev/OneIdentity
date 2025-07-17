namespace OneID.Application.Interfaces.TotpServices
{
    public interface ITotpService
    {
        bool ValidateCode(string code);
    }

}
