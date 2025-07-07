using OneID.Domain.Abstractions.EventsContext;

namespace OneID.Application.Interfaces.Repositories
{
    public interface IStoredEventRepository
    {
        Task AddAsync(StoredEvent storedEvent, CancellationToken cancellationToken);
    }
}
