using OneID.Application.Interfaces.Services;
using Org.BouncyCastle.Crypto.Digests;
using System.Text;

#nullable disable
namespace OneID.Application.Services
{
    public sealed class Sha3HashService : IHashService
    {
        public ValueTask<string> ComputeSha3HashAsync(string input)
        {
            if (string.IsNullOrEmpty(input))
                return ValueTask.FromResult<string>(null);

            return ValueTask.FromResult(ComputeInternal(input));
        }

        public ValueTask<string> ComputeSha3HashAsync(string input, string salt)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(salt))
                return ValueTask.FromResult<string>(null);

            return ValueTask.FromResult(ComputeInternal(input + salt));
        }

        private static string ComputeInternal(string input)
        {
            var shaDigest = new Sha3Digest(384);
            var bytes = Encoding.UTF8.GetBytes(input);

            shaDigest.BlockUpdate(bytes, 0, bytes.Length);

            var result = new byte[shaDigest.GetDigestSize()];
            shaDigest.DoFinal(result, 0);

            var sb = new StringBuilder(result.Length * 2);
            foreach (var b in result)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }
    }
}
