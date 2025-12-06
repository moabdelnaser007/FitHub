using FitHubBackendAPI.Entities.Enums;

namespace FitHubBackendAPI.ViewModels
{
    public class ResponseViewModel<T>
    {
        public T? Data { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public ErrorCode ErrorCode { get; set; } = ErrorCode.NoError;

        public static ResponseViewModel<T> Success(T data, string message = "")
        {
            return new ResponseViewModel<T>
            {
                Data = data,
                IsSuccess = true,
                Message = message,
                ErrorCode = ErrorCode.NoError,
            };
        }

        public static ResponseViewModel<T> Fail(string message, ErrorCode errorCode = ErrorCode.NoError, T? data = default)
        {
            return new ResponseViewModel<T>
            {
                Data = data,
                IsSuccess = false,
                Message = message,
                ErrorCode = errorCode
            };
        }
    }
}
