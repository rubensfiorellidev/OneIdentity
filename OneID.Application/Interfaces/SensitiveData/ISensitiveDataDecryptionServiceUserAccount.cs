using OneID.Domain.Entities.UserContext;

namespace OneID.Application.Interfaces.SensitiveData
{
    public interface ISensitiveDataDecryptionServiceUserAccount
    {
        Task<UserAccount> DecryptSensitiveDataAsync(UserAccount account);
    }

}
