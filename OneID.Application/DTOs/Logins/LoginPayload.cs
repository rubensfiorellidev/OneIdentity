namespace OneID.Application.DTOs.Logins
{
    public record LoginPayload
    {
        public string Login { get; init; }
        public string CorporateEmail { get; init; }

        public LoginPayload(string login, string corporateEmail)
        {
            Login = login;
            CorporateEmail = corporateEmail;
        }
    }
}
