using OneID.Application.Interfaces;

namespace OneID.Application.Abstractions
{
    public class UserLoginGenerator : IUserLoginGenerator
    {
        private readonly IUserRepository _userRepository;
        private readonly IHashService _hashService;

        public UserLoginGenerator(IUserRepository userRepository, IHashService hashService)
        {
            _userRepository = userRepository;
            _hashService = hashService;
        }

        public async Task<string> GenerateLoginAsync(string fullName, CancellationToken cancellationToken)
        {
            var baseLogin = CreateBaseLogin(fullName);
            var candidate = baseLogin;
            var counter = 1;

            while (true)
            {
                var loginHash = await _hashService.ComputeSha3HashAsync(candidate);

                if (!await _userRepository.LoginExistsAsync(loginHash, cancellationToken))
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

            if (parts.Length == 1)
            {
                return Truncate(parts[0]);
            }

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
