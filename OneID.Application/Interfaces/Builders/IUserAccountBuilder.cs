using OneID.Domain.Entities.UserContext;
using OneID.Domain.Enums;

namespace OneID.Application.Interfaces.Builders
{
    public interface IUserAccountBuilder
    {
        IUserAccountBuilder WithCorrelationId(Guid correlationId);
        IUserAccountBuilder WithFirstName(string firstName);
        IUserAccountBuilder WithLastName(string lastName);
        IUserAccountBuilder WithFullName(string fullName);
        IUserAccountBuilder WithSocialName(string socialName);
        IUserAccountBuilder WithCpf(string cpf);
        IUserAccountBuilder WithBirthDate(DateTime birthDate);
        IUserAccountBuilder WithDateOfHire(DateTime dateOfHire);
        IUserAccountBuilder WithRegistry(string registry);
        IUserAccountBuilder WithMotherName(string motherName);
        IUserAccountBuilder WithCompany(string company);
        IUserAccountBuilder WithLogin(string login);
        IUserAccountBuilder WithCorporateEmail(string corporateEmail);
        IUserAccountBuilder WithPersonalEmail(string personalEmail);
        IUserAccountBuilder WithStatusUserProfile(EnumStatusUserAccount status);
        IUserAccountBuilder WithTypeUserProfile(EnumTypeUserAccount type);
        IUserAccountBuilder WithLoginManager(string loginManager);
        IUserAccountBuilder WithPositionHeldId(string positionHeldId);
        IUserAccountBuilder WithFiscalNumberIdentity(string fiscalNumberIdentity);
        IUserAccountBuilder WithContractor(string contractorCnpj, string contractorName);
        IUserAccountBuilder WithCreatedBy(string createdBy);

        UserAccount Build();
    }

}
