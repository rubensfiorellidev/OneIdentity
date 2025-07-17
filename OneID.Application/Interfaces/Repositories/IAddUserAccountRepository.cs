using OneID.Domain.Entities.UserContext;
using OneID.Domain.Interfaces;

namespace OneID.Application.Interfaces.Repositories
{
    public interface IAddUserAccountRepository
    {
        Task<IResult> AddAsync(UserAccount entity, CancellationToken cancellationToken);

    }
}
