using OneID.Application.Interfaces;
using OneID.Domain.Entities;

#nullable disable
namespace OneID.Application.Builders
{
    public class ApplicationUserBuilder : IApplicationUserBuilder
    {
        private readonly IUserLoginGenerator _loginGenerator;

        private string _fullName;
        private string _email;
        private string _phoneNumber;
        private string _createdBy;

        public ApplicationUserBuilder(
            IUserLoginGenerator loginGenerator)
        {
            _loginGenerator = loginGenerator;
        }

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

        public async Task<ApplicationUser> BuildAsync(CancellationToken ct)
        {
            var login = await _loginGenerator.GenerateLoginAsync(_fullName, ct);

            var user = new ApplicationUser(
                login,
                _fullName,
                _email,
                _phoneNumber,
                _createdBy
            );

            return user;
        }

    }

}
