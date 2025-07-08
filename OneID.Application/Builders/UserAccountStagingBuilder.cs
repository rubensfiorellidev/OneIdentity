using OneID.Application.DTOs.Admission;
using OneID.Application.Interfaces.Builders;
using OneID.Application.Interfaces.Services;

namespace OneID.Application.Builders
{
    public sealed class UserAccountStagingBuilder : IUserAccountStagingBuilder
    {
        private readonly IHashService _hashService;
        public UserAccountStagingBuilder(IHashService hashService)
        {
            _hashService = hashService;
        }

        public async Task<AccountRequest> BuildAsync(AccountRequest request)
        {
            var correlationId = Guid.NewGuid();

            return request with
            {
                CorrelationId = correlationId,

                CpfHash = await _hashService.ComputeSha3HashAsync(request.Cpf),

                FiscalNumberIdentityHash = string.IsNullOrEmpty(request.FiscalNumberIdentity)
                    ? null : await _hashService.ComputeSha3HashAsync(request.FiscalNumberIdentity),

                LoginHash = string.IsNullOrEmpty(request.Login)
                    ? null : await _hashService.ComputeSha3HashAsync(request.Login),

                PersonalEmailHash = string.IsNullOrEmpty(request.PersonalEmail)
                    ? null : await _hashService.ComputeSha3HashAsync(request.PersonalEmail),

                CorporateEmailHash = string.IsNullOrEmpty(request.CorporateEmail)
                    ? null : await _hashService.ComputeSha3HashAsync(request.CorporateEmail)
            };
        }
    }
}
