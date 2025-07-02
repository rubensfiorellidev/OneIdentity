using OneID.Application.Interfaces;
using OneID.Domain.Entities;

#nullable disable
namespace OneID.Application.Builders
{
    public class ApplicationUserBuilder : IApplicationUserBuilder
    {
        private string _fullName;
        private string _email;
        private string _phoneNumber;
        private string _createdBy;
        private string _loginHash;
        private string _loginCrypt;

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

        public ApplicationUser Build()
        {
            if (string.IsNullOrWhiteSpace(_loginHash) || string.IsNullOrWhiteSpace(_loginCrypt))
                throw new InvalidOperationException("LoginHash and LoginCrypt must be provided.");

            return new ApplicationUser(
                _fullName,
                _email,
                _phoneNumber,
                _createdBy,
                _loginHash,
                _loginCrypt
            );
        }
    }

}
