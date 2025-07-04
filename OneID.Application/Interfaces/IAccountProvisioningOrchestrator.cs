namespace OneID.Application.Interfaces
{
    public interface IAccountProvisioningOrchestrator
    {
        Task<string> ProvisionLoginAsync(string firstName, string lastName, CancellationToken cancellationToken);
    }

}
