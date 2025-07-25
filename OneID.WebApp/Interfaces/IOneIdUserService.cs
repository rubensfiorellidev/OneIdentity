using static OneID.WebApp.Components.Pages.ActiveUsers;

namespace OneID.WebApp.Interfaces
{
    public interface IOneIdUserService
    {
        Task<List<ActiveUserViewModel>> GetActiveUsersAsync();
    }

}
