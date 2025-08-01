#nullable disable
namespace OneID.Application.DTOs.Users
{
    public record UserResponse
    {
        public string Id { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Company { get; init; }
        public string StatusUserAccount { get; init; }
        public string TypeUserAccount { get; init; }
        public string JobTitleName { get; init; }
        public string CreatedBy { get; init; }
    }
}
