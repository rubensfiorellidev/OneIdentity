using OneID.Application.Interfaces;
using OneID.Domain.Entities.UserContext;

#nullable disable
namespace OneID.Application.Builders
{
    public class ApplicationUserBuilder : IApplicationUserBuilder
    {
        private string _userName;
        private string _email;
        private string _phoneNumber;
        private string _createdBy;
        private string _loginHash;
        private string _loginCrypt;
        private string _keycloakUserId;

        public IApplicationUserBuilder WithUserName(string userName)
        {
            _userName = userName;
            return this;
        }

        public IApplicationUserBuilder WithEmail(string email)
        {
            _email = email;
            return this;
        }

        public IApplicationUserBuilder WithPhoneNumber(string phoneNumber)
        {
            _phoneNumber = phoneNumber;
            return this;
        }

        public IApplicationUserBuilder WithCreatedBy(string createdBy)
        {
            _createdBy = createdBy;
            return this;
        }

        public IApplicationUserBuilder WithLoginHash(string loginHash)
        {
            _loginHash = loginHash;
            return this;
        }

        public IApplicationUserBuilder WithLoginCrypt(string loginCrypt)
        {
            _loginCrypt = loginCrypt;
            return this;
        }

        public IApplicationUserBuilder WithKeycloakUserId(string keycloakUserId)
        {
            _keycloakUserId = keycloakUserId;
            return this;
        }

        public ApplicationUser Build()
        {
            return new ApplicationUser(
                _userName,
                _email,
                _phoneNumber,
                _createdBy,
                _loginHash,
                _loginCrypt,
                _keycloakUserId
            );
        }
    }

}
