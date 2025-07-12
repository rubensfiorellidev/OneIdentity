namespace OneID.Domain.Contracts
{
    public interface IAuditableEntity
    {
        DateTimeOffset ProvisioningAt { get; }
        DateTimeOffset? UpdatedAt { get; }

        string CreatedBy { get; }
        string UpdatedBy { get; }

        void PrepareForUpdate(string updatedBy);
    }

}
