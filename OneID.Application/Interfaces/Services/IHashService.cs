namespace OneID.Application.Interfaces.Services
{
    public interface IHashService
    {
        Task<string> ComputeSha3HashAsync(string input);
    }

}
