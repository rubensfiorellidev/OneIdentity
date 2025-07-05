using OneID.Application.Interfaces;
using Org.BouncyCastle.Crypto.Digests;
using System.Text;

#nullable disable
namespace OneID.Application.Services
{
    public sealed class HashService : IHashService
    {
        public async Task<string> ComputeSha3HashAsync(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            await Task.Delay(1);

            var shaDigest = new Sha3Digest(bitLength: 384);
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            shaDigest.BlockUpdate(inputBytes, 0, inputBytes.Length);

            byte[] result = new byte[shaDigest.GetDigestSize()];
            shaDigest.DoFinal(result, 0);


            StringBuilder hashString = new();

            foreach (byte b in result)
            {
                hashString.Append(b.ToString("x2"));
            }

            return hashString.ToString();
        }

    }
}
