using OneID.Application.Interfaces.Builders;
using OneID.Domain.Entities.UserContext;
using OneID.Domain.ValueObjects;

#nullable disable
namespace OneID.Application.Builders
{
    public class UserAccountBuilder : IUserAccountBuilder
    {
        private Guid _correlationId;

        private string _id = $"{Ulid.NewUlid()}";
        private string _firstName;
        private string _lastName;
        private string _fullName;
        private string _socialName;
        private string _cpf;
        private DateTime _birthDate;
        private DateTime _dateOfHire;
        private string _registry;
        private string _motherName;
        private string _company;
        private string _login;
        private string _corporateEmail;
        private string _personalEmail;
        private UserAccountStatus _statusUserProfile;
        private TypeUserAccount _typeUserProfile;
        private string _loginManager;
        private string _jobTitleId;
        private string _jobTitle;
        private string _departmentId;
        private string _department;
        private string _fiscalNumberIdentity;
        private string _contractorCnpj;
        private string _contractorName;
        private string _createdBy;


        public IUserAccountBuilder WithCorrelationId(Guid correlationId)
        {
            _correlationId = correlationId;
            return this;
        }
        public IUserAccountBuilder WithFirstName(string firstName)
        {
            _firstName = firstName;
            return this;
        }

        public IUserAccountBuilder WithLastName(string lastName)
        {
            _lastName = lastName;
            return this;
        }

        public IUserAccountBuilder WithFullName(string fullName)
        {
            _fullName = fullName;
            return this;
        }

        public IUserAccountBuilder WithSocialName(string socialName)
        {
            _socialName = socialName;
            return this;
        }

        public IUserAccountBuilder WithCpf(string cpf)
        {
            _cpf = cpf;
            return this;
        }

        public IUserAccountBuilder WithBirthDate(DateTime birthDate)
        {
            _birthDate = birthDate;
            return this;
        }

        public IUserAccountBuilder WithDateOfHire(DateTime dateOfHire)
        {
            _dateOfHire = dateOfHire;
            return this;
        }

        public IUserAccountBuilder WithRegistry(string registry)
        {
            _registry = registry;
            return this;
        }

        public IUserAccountBuilder WithMotherName(string motherName)
        {
            _motherName = motherName;
            return this;
        }

        public IUserAccountBuilder WithCompany(string company)
        {
            _company = company;
            return this;
        }

        public IUserAccountBuilder WithLogin(string login)
        {
            _login = login;
            return this;
        }

        public IUserAccountBuilder WithCorporateEmail(string corporateEmail)
        {
            _corporateEmail = corporateEmail;
            return this;
        }

        public IUserAccountBuilder WithPersonalEmail(string personalEmail)
        {
            _personalEmail = personalEmail;
            return this;
        }

        public IUserAccountBuilder WithStatusUserProfile(UserAccountStatus status)
        {
            _statusUserProfile = status;
            return this;
        }

        public IUserAccountBuilder WithTypeUserProfile(TypeUserAccount type)
        {
            _typeUserProfile = type;
            return this;
        }

        public IUserAccountBuilder WithLoginManager(string loginManager)
        {
            _loginManager = loginManager;
            return this;
        }

        public IUserAccountBuilder WithJobTitleId(string jobTitleId)
        {
            _jobTitleId = jobTitleId;
            return this;
        }
        public IUserAccountBuilder WithJobTitle(string jobTitle)
        {
            _jobTitle = jobTitle;
            return this;
        }
        public IUserAccountBuilder WithDepartmentId(string departmentId)
        {
            _departmentId = departmentId;
            return this;
        }

        public IUserAccountBuilder WithDepartment(string department)
        {
            _department = department;
            return this;
        }

        public IUserAccountBuilder WithFiscalNumberIdentity(string fiscalNumberIdentity)
        {
            _fiscalNumberIdentity = fiscalNumberIdentity;
            return this;
        }

        public IUserAccountBuilder WithContractor(string contractorCnpj, string contractorName)
        {
            _contractorCnpj = contractorCnpj;
            _contractorName = contractorName;
            return this;
        }

        public IUserAccountBuilder WithCreatedBy(string createdBy)
        {
            _createdBy = createdBy;
            return this;
        }

        public UserAccount Build()
        {
            var user = new UserAccount(_id, _createdBy);

            user.SetCorrelationId(_correlationId);
            user.SetFirstName(_firstName);
            user.SetLastname(_lastName);
            user.SetFullName(_fullName);
            user.SetSocialName(_socialName);
            user.SetCpf(_cpf);
            user.SetBirthDate(_birthDate);
            user.SetDateOfHire(_dateOfHire);
            user.SetRegistry(_registry);
            user.SetMotherName(_motherName);
            user.SetCompany(_company);
            user.SetLogin(_login);
            user.SetCorporateEmail(_corporateEmail);
            user.SetPersonalEmail(_personalEmail);
            user.SetStatusUserAccount(_statusUserProfile);
            user.SetTypeUserAccount(_typeUserProfile);
            user.SetLoginManager(_loginManager);
            user.SetJobTitleId(_jobTitleId);
            user.SetJobTitle(_jobTitle);
            user.SetDepartmentId(_departmentId);
            user.SetDepartment(_department);
            user.SetFiscalNumberIdentity(_fiscalNumberIdentity);
            user.SetContractor(_contractorCnpj, _contractorName);

            return user;
        }


    }

}
