namespace OneID.Domain.Interfaces
{
    public interface IOperationResult
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
            Func<IOperationResult, TResult> onSuccess,
            Func<IOperationResult, TResult> onError);

    }
}
