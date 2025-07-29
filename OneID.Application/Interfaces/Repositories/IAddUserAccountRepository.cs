using OneID.Domain.Entities.UserContext;
using OneID.Domain.Interfaces;

namespace OneID.Application.Interfaces.Repositories
{
    public interface IAddUserAccountRepository
    {
        Task<IOperationResult> AddAsync(UserAccount entity, CancellationToken cancellationToken);

    }
}
