using System.Net;

namespace EasyClaimsCore.API.Models.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public string ErrorCode { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

        public static ApiResponse<T> CreateSuccess(T data, string message = "")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message,
                StatusCode = HttpStatusCode.OK
            };
        }

        public static ApiResponse<T> CreateError(string message, string errorCode = "", HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode,
                StatusCode = statusCode
            };
        }
    }
}