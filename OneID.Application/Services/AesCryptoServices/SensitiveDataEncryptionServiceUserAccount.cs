using OneID.Application.Interfaces.AesCryptoService;
using OneID.Application.Interfaces.SensitiveData;
using OneID.Domain.Entities.UserContext;

namespace OneID.Application.Services.AesCryptoServices
{
    public class SensitiveDataEncryptionServiceUserAccount : ISensitiveDataEncryptionServiceUserAccount
    {
        private readonly ICryptoService _cryptoService;
        public SensitiveDataEncryptionServiceUserAccount(ICryptoService cryptoService)
        {
            _cryptoService = cryptoService ?? throw new ArgumentNullException(nameof(cryptoService));
        }

        public Task<UserAccount> EncryptSensitiveDataAsync(UserAccount account)
        {
            ArgumentNullException.ThrowIfNull(account);

            var encryptedCpf = _cryptoService.Encrypt(account.Cpf);

            var encryptedCorporateEmail = _cryptoService.Encrypt(account.CorporateEmail);

            var personalEmail = !string.IsNullOrEmpty(account.PersonalEmail)
                ? _cryptoService.Encrypt(account.PersonalEmail)
                : account.PersonalEmail;

            var encryptedLogin = _cryptoService.Encrypt(account.Login);

            var encryptedRegistry = _cryptoService.Encrypt(account.Registry);

            var encryptedFiscalNumber = !string.IsNullOrEmpty(account.FiscalNumberIdentity)
                ? _cryptoService.Encrypt(account.FiscalNumberIdentity)
                : account.FiscalNumberIdentity;

            var encryptedContractorCnpj = !string.IsNullOrEmpty(account.ContractorCnpj)
                ? _cryptoService.Encrypt(account.ContractorCnpj)
                : account.ContractorCnpj;

            var encryptedLoginManager = !string.IsNullOrEmpty(account.LoginManager)
               ? _cryptoService.Encrypt(account.LoginManager)
               : account.LoginManager;

            account.ApplyCrytos(
                encryptedCpf,
                encryptedCorporateEmail,
                personalEmail,
                encryptedLogin,
                encryptedRegistry,
                encryptedFiscalNumber,
                encryptedContractorCnpj,
                encryptedLoginManager
            );

            return Task.FromResult(account);
        }
    }

}
