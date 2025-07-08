namespace OneID.Application.Interfaces
{
    public interface IHashService
    {
        Task<string> ComputeSha3HashAsync(string input);
    }

}
