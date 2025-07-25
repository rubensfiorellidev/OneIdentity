using OneID.Application.DTOs.Admission;
using OneID.Application.Interfaces.AesCryptoService;
using OneID.Application.Interfaces.Builders;

#nullable disable
namespace OneID.Application.Builders
{
    public sealed class UserAccountStagingBuilder : IUserAccountStagingBuilder
    {
        private readonly ICryptoService _cryptoService;

        public UserAccountStagingBuilder(ICryptoService cryptoService)
        {
            _cryptoService = cryptoService;
        }

        public Task<AccountRequest> BuildAsync(AccountRequest request)
        {
            var correlationId = Guid.NewGuid();

            var (cpf, cpfHash) = EncryptField(request.Cpf);
            var (fiscal, fiscalHash) = EncryptField(request.FiscalNumberIdentity);
            var (login, loginHash) = EncryptField(request.Login);
            var (personalEmail, personalEmailHash) = EncryptField(request.PersonalEmail);
            var (corpEmail, corpEmailHash) = EncryptField(request.CorporateEmail);

            return Task.FromResult(request with
            {
                CorrelationId = correlationId,

                Cpf = cpf,
                CpfHash = cpfHash,

                FiscalNumberIdentity = fiscal,
                FiscalNumberIdentityHash = fiscalHash,

                Login = login,
                LoginHash = loginHash,

                PersonalEmail = personalEmail,
                PersonalEmailHash = personalEmailHash,

                CorporateEmail = corpEmail,
                CorporateEmailHash = corpEmailHash
            });
        }

        private (string Encrypted, string Hash) EncryptField(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? (null, null)
                : _cryptoService.EncryptWithHash(value);
        }
    }
}
