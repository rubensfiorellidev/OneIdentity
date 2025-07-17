#nullable disable
using OneID.Domain.Interfaces;

namespace OneID.Domain.Results
{
    public class Result : IResult
    {
        public int? HttpCode { get; private set; }
        public string Message { get; private set; }
        public bool IsSuccess { get; private set; }
        public object Data { get; private set; }
        public object Hateoaes { get; private set; }
        public object AdditionalData { get; private set; }
        public string[] Errors { get; private set; }
        public string[] Warnings { get; private set; }
        public string JwtToken { get; private set; }


        public string FailureReason
        {
            get
            {
                if (!IsSuccess && Errors != null && Errors.Any())
                {
                    return Errors.First();
                }

                if (!IsSuccess)
                {
                    return Message;
                }

                return null;
            }
        }
        protected Result(int? httpCode,
                 string message,
                 bool isSuccess,
                 object data,
                 string[] errors = null,
                 string[] warnings = null,
                 object hateoaes = null,
                 object additionalData = null,
                 string token = null)
        {
            HttpCode = httpCode;
            Message = message;
            IsSuccess = isSuccess;
            Data = data ?? new object();
            Errors = errors ?? [];
            Warnings = warnings ?? [];
            Hateoaes = hateoaes ?? new object();
            AdditionalData = additionalData;
            JwtToken = token;
        }


        public static Result Success(string message = "Successful operation", object data = null, object additionalData = null)
        {
            var result = new Result(null, message, true, data);


            return result;
        }
        public static Result Success(int httpCode, string message = "Successful operation", object data = null, object additionalData = null)
        {
            var result = new Result(httpCode, message, true, data);


            return result;
        }
        public static Result TokenOnly(string token)
        {
            return new Result(null, null, true, null, token: token);
        }
        public static Result Failure(string message = "Failed to process the request", string[] errors = null, object additionalData = null)
        {
            return new Result(null, message, false, null,
                errors?.Length > 0 ? errors : [message],
                additionalData: additionalData);
        }
        public static Result Failure(int httpCode, string message = "Failed to process the request", string[] errors = null, object additionalData = null)
        {
            return new Result(httpCode, message, false, null,
                errors?.Length > 0 ? errors : [message],
                additionalData: additionalData);
        }
        public static Result Conflict(string message = "Conflict detected", string[] errors = null, object additionalData = null)
        {
            return Failure(409, message, errors, additionalData);
        }
        public static Result NotFound(string message = "Resource not found", object additionalData = null)
        {
            return Failure(404, message, additionalData: additionalData);
        }
        public static Result NotFound(int httpCode, string message = "Resource not found", object additionalData = null)
        {
            return Failure(httpCode, message, additionalData: additionalData);
        }

        public TResult Match<TResult>(
            Func<IResult, TResult> onSuccess,
            Func<IResult, TResult> onError)
        {
            return IsSuccess ? onSuccess(this) : onError(this);
        }

    }

}
