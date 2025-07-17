namespace OneID.Application.Interfaces.Graph
{
    public interface IAccessPackageGroupService
    {
        Task<IReadOnlyCollection<string>> ResolveGroupsForUserAsync(Guid correlationId, CancellationToken cancellationToken);
    }

}
