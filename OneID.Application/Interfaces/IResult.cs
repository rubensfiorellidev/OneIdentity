namespace OneID.Application.Interfaces
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
        IResult WithAdditionalData(object additionalData);

    }
}
