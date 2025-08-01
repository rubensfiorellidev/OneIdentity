namespace OneID.WebApp.ViewModels
{
    public sealed record PaginatedUsersViewModel(
     IReadOnlyList<AllUserViewModel> Users,
     int TotalCount
 );

}
