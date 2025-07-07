namespace OneID.Application.Interfaces.AesCryptoService
{
    public interface ISecureCryptoService
    {
        char[] DecryptToCharArray(string cipherText);

    }
}
