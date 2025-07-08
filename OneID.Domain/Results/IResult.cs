namespace OneID.Domain.Results
{
    public interface IResult
    {
        int? HttpCode { get; }
        string Message { get; }
        bool IsSuccess { get; }
        object Data { get; }
        string FailureReason { get; }
        string[] Errors { get; }
        string[] Warnings { get; }
        string JwtToken { get; }

    }
}
