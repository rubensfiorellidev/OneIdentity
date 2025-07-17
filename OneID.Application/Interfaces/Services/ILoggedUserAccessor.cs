namespace OneID.Application.Interfaces.Services
{
    public interface ILoggedUserAccessor
    {
        string GetEmail();
        string GetName();
        string GetPhone();
    }

}
