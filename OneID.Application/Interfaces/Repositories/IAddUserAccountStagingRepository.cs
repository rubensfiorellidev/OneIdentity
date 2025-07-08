using OneID.Domain.Entities.UserContext;

namespace OneID.Application.Interfaces.Repositories
{
    public interface IAddUserAccountStagingRepository
    {
        Task SaveAsync(AccountAdmissionStaging entity, CancellationToken cancellationToken);
    }
}
