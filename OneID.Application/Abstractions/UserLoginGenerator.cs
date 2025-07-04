using OneID.Application.Interfaces;

namespace OneID.Application.Abstractions
{
    public class UserLoginGenerator : IUserLoginGenerator
    {
        private readonly IKeycloakUserChecker _userChecker;

        public UserLoginGenerator(IKeycloakUserChecker userChecker)
        {
            _userChecker = userChecker;
        }

        public async Task<string> GenerateLoginAsync(string fullName, CancellationToken cancellationToken)
        {
            var baseLogin = CreateBaseLogin(fullName);
            var candidate = baseLogin;
            var counter = 1;

            while (true)
            {
                if (!await _userChecker.UsernameExistsAsync(candidate, cancellationToken))
                {
                    return candidate;
                }

                candidate = Truncate($"{baseLogin}{counter}");
                counter++;
            }
        }

        private static string CreateBaseLogin(string fullName)
        {
            var parts = fullName.Trim().ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 2)
                throw new InvalidOperationException("Full name must include at least first and last names.");

            var firstName = parts[0];
            var lastName = parts[^1];

            return Truncate($"{firstName}.{lastName}");
        }

        private static string Truncate(string login)
        {
            return login.Length <= 12 ? login : login[..12];
        }
    }

}
