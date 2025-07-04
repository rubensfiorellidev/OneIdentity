using OneID.Domain.Abstractions.Events;

namespace OneID.Domain.Contracts.UserProfileContext
{
    public interface IAccount
    {
        string Id { get; }
        IReadOnlyCollection<Event> Events { get; }
        void ClearEvents();
    }
}

