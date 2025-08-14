namespace OneID.Application.Interfaces.Logins
{
    public interface ILoginReservationRepository
    {
        Task<string> ReserveAsync(string preferredLogin, Guid correlationId, CancellationToken ct);
        Task CommitAsync(Guid correlationId, CancellationToken ct);
        Task ReleaseAsync(Guid correlationId, CancellationToken ct);
    }
}
