using Microsoft.Extensions.Logging;
using OneID.Application.Interfaces.AesCryptoService;
using System.Security.Cryptography;

namespace OneID.Application.Services.AesCryptoServices
{
    public sealed class AesCryptoService : ICryptoService, ISecureCryptoService
    {
        private readonly byte[] _encryptionKey;
        private readonly string _crypt;
        private readonly ILogger<AesCryptoService> _logger;

        public AesCryptoService(string key, string crypt, ILogger<AesCryptoService> logger)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrWhiteSpace(crypt)) throw new ArgumentNullException(nameof(crypt));

            _encryptionKey = Convert.FromBase64String(key);
            if (_encryptionKey.Length != 32)
                throw new ArgumentException("Invalid AES key length. Expected 32 bytes for AES-256.");

            _crypt = crypt;
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

                ms.Write(aes.IV, 0, aes.IV.Length);

                using (var cryptoStream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var writer = new StreamWriter(cryptoStream))
                {
                    writer.Write(plainText);
                }

                var encryptedBytes = ms.ToArray();
                return _crypt + Convert.ToBase64String(encryptedBytes);
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

            if (!cipherText.StartsWith(_crypt)) return cipherText;

            cipherText = cipherText[_crypt.Length..];

            try
            {
                if (!IsBase64String(cipherText))
                {
                    _logger.LogWarning("Decrypt received invalid base64 string.");
                    throw new FormatException("Cipher text is not valid base64.");
                }

                var fullCipher = Convert.FromBase64String(cipherText);

                using Aes aes = Aes.Create();
                aes.Key = _encryptionKey;

                var iv = new byte[16];
                Array.Copy(fullCipher, 0, iv, 0, iv.Length);
                aes.IV = iv;
                aes.Padding = PaddingMode.PKCS7;

                using var decryptor = aes.CreateDecryptor();
                using var ms = new MemoryStream(fullCipher, 16, fullCipher.Length - 16);
                using var cryptoStream = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                using var reader = new StreamReader(cryptoStream);

                var result = reader.ReadToEnd();

                Array.Clear(fullCipher, 0, fullCipher.Length); // Clean sensitive data

                return result;
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "Format error during {Operation}", nameof(Decrypt));
                throw;
            }
            catch (CryptographicException ex)
            {
                _logger.LogError(ex, "Cryptographic error during {Operation}", nameof(Decrypt));
                throw;
            }
        }

        public char[] DecryptToCharArray(string cipherText)
        {
            var decrypted = Decrypt(cipherText);
            return string.IsNullOrEmpty(decrypted) ? Array.Empty<char>() : decrypted.ToCharArray();
        }

        private bool IsBase64String(string input)
        {
            Span<byte> buffer = new Span<byte>(new byte[input.Length]);
            return Convert.TryFromBase64String(input, buffer, out _);
        }
    }

}
