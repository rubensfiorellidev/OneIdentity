using OneID.Application.DTOs.Admission;

namespace OneID.Application.Interfaces.Builders
{
    public interface IUserAccountStagingBuilder
    {
        Task<AccountRequest> BuildAsync(AccountRequest request);

    }
}
