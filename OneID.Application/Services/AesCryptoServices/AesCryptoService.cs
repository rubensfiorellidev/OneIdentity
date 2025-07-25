using Microsoft.Extensions.Logging;
using OneID.Application.Interfaces.AesCryptoService;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using System.Security.Cryptography;

namespace OneID.Application.Services.AesCryptoServices
{
    public sealed class AesCryptoService : ICryptoService, ISecureCryptoService
    {
        private readonly byte[] _encryptionKey;
        private readonly ILogger<AesCryptoService> _logger;

        private const byte PrefixMarker = 0xA1;
        public AesCryptoService(string key, ILogger<AesCryptoService> logger)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));
            _encryptionKey = Convert.FromBase64String(key);
            if (_encryptionKey.Length != 32)
                throw new ArgumentException("Invalid AES key length. Expected 32 bytes for AES-256.");

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return plainText;

            try
            {
                using Aes aes = Aes.Create();
                aes.Key = _encryptionKey;
                aes.GenerateIV();
                aes.Padding = PaddingMode.PKCS7;

                using var encryptor = aes.CreateEncryptor();
                using var ms = new MemoryStream();

                ms.WriteByte(PrefixMarker);
                ms.Write(aes.IV, 0, aes.IV.Length);

                using (var cryptoStream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var writer = new StreamWriter(cryptoStream))
                {
                    writer.Write(plainText);
                }

                var cipherBytes = ms.ToArray();

                var hmac = GenerateHmac(cipherBytes);

                var finalPayload = cipherBytes.Concat(hmac).ToArray();
                return Convert.ToBase64String(finalPayload);

            }
            catch (CryptographicException ex)
            {
                _logger.LogError(ex, "Cryptographic error during {Operation}", nameof(Encrypt));
                throw;
            }
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return string.Empty;

            try
            {
                if (!IsBase64String(cipherText))
                    throw new FormatException("Cipher text is not valid base64.");

                var fullPayload = Convert.FromBase64String(cipherText);

                const int prefixSize = 1;
                const int ivSize = 16;
                const int hmacSize = 48;

                if (fullPayload.Length <= (prefixSize + ivSize + hmacSize))
                    throw new CryptographicException("Invalid payload length.");

                // Valida prefixo binário
                if (fullPayload[0] != PrefixMarker)
                    throw new CryptographicException("Invalid data prefix marker.");

                var cipherDataLength = fullPayload.Length - hmacSize;
                var cipherData = fullPayload[..cipherDataLength];
                var receivedHmac = fullPayload[cipherDataLength..];

                var expectedHmac = GenerateHmac(cipherData);
                if (!CryptographicOperations.FixedTimeEquals(receivedHmac, expectedHmac))
                    throw new CryptographicException("Invalid HMAC. Data integrity check failed.");

                var iv = cipherData[prefixSize..(prefixSize + ivSize)];
                var encrypted = cipherData[(prefixSize + ivSize)..];

                using Aes aes = Aes.Create();
                aes.Key = _encryptionKey;
                aes.IV = iv;
                aes.Padding = PaddingMode.PKCS7;

                using var decryptor = aes.CreateDecryptor();
                using var ms = new MemoryStream(encrypted);
                using var cryptoStream = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                using var reader = new StreamReader(cryptoStream);

                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Decryption failed.");
                throw;
            }
        }

        public char[] DecryptToCharArray(string cipherText)
        {
            var decrypted = Decrypt(cipherText);
            return string.IsNullOrEmpty(decrypted) ? [] : decrypted.ToCharArray();
        }

        private byte[] GenerateHmac(byte[] data)
        {
            var hmac = new HMac(new Sha3Digest(384));
            hmac.Init(new KeyParameter(_encryptionKey));
            hmac.BlockUpdate(data, 0, data.Length);

            var result = new byte[hmac.GetMacSize()];
            hmac.DoFinal(result, 0);
            return result;
        }

        private bool IsBase64String(string input)
        {
            Span<byte> buffer = new Span<byte>(new byte[input.Length]);
            return Convert.TryFromBase64String(input, buffer, out _);
        }
    }

}
