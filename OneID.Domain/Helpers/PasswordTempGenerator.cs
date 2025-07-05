using System.Security.Cryptography;
using System.Text;

namespace OneID.Domain.Helpers
{
    public class PasswordTempGenerator
    {
        public static string GenerateTemporaryPassword()
        {
            var passwordBuilder = new StringBuilder();
            using var randomNumberGenerator = RandomNumberGenerator.Create();

            void AddRandomChar(int count, Func<byte, char> charSelector)
            {
                for (int i = 0; i < count; i++)
                {
                    byte[] randomBytes = new byte[1];
                    randomNumberGenerator.GetBytes(randomBytes);
                    passwordBuilder.Append(charSelector(randomBytes[0]));
                }
            }

            // Adiciona 3 letras minúsculas
            AddRandomChar(3, b => (char)('a' + (b % 26)));

            // Adiciona 3 letras maiúsculas
            AddRandomChar(3, b => (char)('A' + (b % 26)));

            // Adiciona 3 dígitos
            AddRandomChar(3, b => (char)('0' + (b % 10)));

            // Adiciona 3 caracteres especiais variados
            char[] specialChars = "!@#$%^&*()-_=+[]{}".ToCharArray();
            AddRandomChar(3, b => specialChars[b % specialChars.Length]);

            // Embaralha os caracteres
            var passwordChars = passwordBuilder.ToString().ToCharArray();
            Shuffle(passwordChars);
            return new string([.. passwordChars.Take(12)]);
        }

        private static void Shuffle(char[] array)
        {
            using var rng = RandomNumberGenerator.Create();
            for (int i = array.Length - 1; i > 0; i--)
            {
                byte[] box = new byte[1];
                rng.GetBytes(box);
                int j = box[0] % (i + 1);
                (array[i], array[j]) = (array[j], array[i]);
            }
        }
    }

}
