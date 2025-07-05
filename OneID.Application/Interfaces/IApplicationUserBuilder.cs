using OneID.Domain.Entities.UserContext;

namespace OneID.Application.Interfaces
{
    public interface IApplicationUserBuilder
    {
        IApplicationUserBuilder WithFullName(string fullName);
        IApplicationUserBuilder WithEmail(string email);
        IApplicationUserBuilder WithPhoneNumber(string phoneNumber);
        IApplicationUserBuilder WithCreatedBy(string createdBy);
        IApplicationUserBuilder WithLoginHash(string loginHash);
        IApplicationUserBuilder WithLoginCrypt(string loginCrypt);
        ApplicationUser Build();
    }

}
