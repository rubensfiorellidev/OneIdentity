using OneID.Domain.Entities.UserContext;

namespace OneID.Application.Interfaces.Repositories
{
    public interface IAddUserAccountStagingRepository
    {
        Task SaveAsync(AccountPjAdmissionStaging entity, CancellationToken cancellationToken);
    }
}
