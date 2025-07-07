using OneID.Domain.Entities.UserContext;

namespace OneID.Application.Interfaces.SensitiveData
{
    public interface ISensitiveDataEncryptionServiceUserAccount
    {
        Task<UserAccount> EncryptSensitiveDataAsync(UserAccount account);
    }

}
