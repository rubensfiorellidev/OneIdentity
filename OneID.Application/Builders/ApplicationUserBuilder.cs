using OneID.Application.Interfaces;
using OneID.Domain.Entities;

#nullable disable
namespace OneID.Application.Builders
{
    public class ApplicationUserBuilder : IApplicationUserBuilder
    {
        private string _fullName;
        private string _firstName;
        private string _lastName;
        private string _email;
        private string _phoneNumber;
        private string _createdBy;
        private string _loginHash;
        private string _loginCrypt;
        private string _keycloakUserId;


        public IApplicationUserBuilder WithFullName(string fullName)
        {
            _fullName = fullName;
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

        public IApplicationUserBuilder WithFirstName(string firstName)
        {
            _firstName = firstName;
            return this;
        }

        public IApplicationUserBuilder WithLastName(string lastName)
        {
            _lastName = lastName;
            return this;
        }

        public IApplicationUserBuilder WithKeycloakUserId(string keycloakUserId)
        {
            _keycloakUserId = keycloakUserId;
            return this;
        }

        public ApplicationUser Build()
        {
            if (string.IsNullOrWhiteSpace(_fullName))
                throw new InvalidOperationException("FullName must be provided.");
            if (string.IsNullOrWhiteSpace(_firstName))
                throw new InvalidOperationException("FirstName must be provided.");
            if (string.IsNullOrWhiteSpace(_lastName))
                throw new InvalidOperationException("LastName must be provided.");
            if (string.IsNullOrWhiteSpace(_email))
                throw new InvalidOperationException("Email must be provided.");


            return new ApplicationUser(
                _fullName,
                _firstName,
                _lastName,
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
