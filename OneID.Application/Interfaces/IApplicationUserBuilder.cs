using OneID.Domain.Entities;

namespace OneID.Application.Interfaces
{
    public interface IApplicationUserBuilder
    {
        IApplicationUserBuilder WithFullName(string fullName);
        IApplicationUserBuilder WithEmail(string email);
        IApplicationUserBuilder WithPhoneNumber(string phoneNumber);
        IApplicationUserBuilder WithCreatedBy(string createdBy);
        Task<ApplicationUser> BuildAsync(CancellationToken ct);
    }

}
