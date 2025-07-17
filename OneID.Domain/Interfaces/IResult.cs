namespace OneID.Domain.Interfaces
{
    public interface IResult
    {
        int? HttpCode { get; }
        string Message { get; }
        bool IsSuccess { get; }
        object Data { get; }
        string FailureReason { get; }
        string[] Errors { get; }
        string JwtToken { get; }
        object AdditionalData { get; }

        TResult Match<TResult>(
            Func<IResult, TResult> onSuccess,
            Func<IResult, TResult> onError);

    }
}
