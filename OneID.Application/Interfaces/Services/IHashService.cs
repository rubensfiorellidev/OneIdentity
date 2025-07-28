namespace OneID.Application.Interfaces.Services
{
    public interface IHashService
    {
        ValueTask<string> ComputeSha3HashAsync(string input);
        ValueTask<string> ComputeSha3HashAsync(string input, string salt);
    }

}
