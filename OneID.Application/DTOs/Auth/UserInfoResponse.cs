namespace OneID.Application.DTOs.Auth
{
    public readonly record struct UserInfoResponse
    {
        public string Name { get; init; }
        public string Email { get; init; }
        public string AccountId { get; init; }
        public List<string> Roles { get; init; }
    }

}
