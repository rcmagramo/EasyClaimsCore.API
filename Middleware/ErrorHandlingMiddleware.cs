using EasyClaimsCore.API.Models.Responses;
using System.Net;
using System.Text.Json;

namespace EasyClaimsCore.API.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = ApiResponse<object>.CreateError(
                message: exception.Message,
                errorCode: "INTERNAL_SERVER_ERROR",
                statusCode: HttpStatusCode.InternalServerError
            );

            switch (exception)
            {
                case ArgumentNullException:
                    response = ApiResponse<object>.CreateError(
                        message: exception.Message,
                        errorCode: "BAD_REQUEST",
                        statusCode: HttpStatusCode.BadRequest
                    );
                    break;
                case UnauthorizedAccessException:
                    response = ApiResponse<object>.CreateError(
                        message: exception.Message,
                        errorCode: "UNAUTHORIZED",
                        statusCode: HttpStatusCode.Unauthorized
                    );
                    break;
                case KeyNotFoundException:
                    response = ApiResponse<object>.CreateError(
                        message: exception.Message,
                        errorCode: "NOT_FOUND",
                        statusCode: HttpStatusCode.NotFound
                    );
                    break;
                case ArgumentException:
                    response = ApiResponse<object>.CreateError(
                        message: exception.Message,
                        errorCode: "BAD_REQUEST",
                        statusCode: HttpStatusCode.BadRequest
                    );
                    break;
            }

            context.Response.StatusCode = (int)response.StatusCode;

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}