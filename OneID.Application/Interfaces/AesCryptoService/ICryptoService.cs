namespace OneID.Application.Interfaces.AesCryptoService
{
    public interface ICryptoService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);

    }
}
