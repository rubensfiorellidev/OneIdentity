using OneID.Domain.Entities.Logins;

namespace OneID.Application.Interfaces.Repositories
{
    public interface IMfaChallengeRepository
    {
        Task<MfaChallengeEntity> CreateAsync(MfaChallengeEntity challenge, CancellationToken ct);
        Task<MfaChallengeEntity> GetByIdAsync(string jti, CancellationToken ct);
        Task<MfaChallengeEntity> GetLatestActiveAsync(string userId, CancellationToken ct);
        Task<bool> MarkUsedAsync(string jti, bool success, CancellationToken ct);
        Task ExpireAsync(string jti, CancellationToken ct);
    }
}
