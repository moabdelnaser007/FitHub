using FitHubBackendAPI.Entities.Enums;
using FitHubBackendAPI.ViewModels;
using FluentValidation;
using System.Net;
using System.Text.Json;

namespace FitHubBackendAPI.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;

            try
            {
                await _next(context);

                if (context.Response.StatusCode == 401 || context.Response.StatusCode == 403)
                {
                    string? existingBody = null;

                    using (var newBody = new MemoryStream())
                    {
                        context.Response.Body = newBody;

                        var message = context.Response.StatusCode == 401
                            ? "Unauthorized access. Please login."
                            : "Forbidden. You don't have permission to access this resource.";

                        var errorCode = context.Response.StatusCode == 401
                            ? ErrorCode.Unauthorized
                            : ErrorCode.Forbidden;

                        var response = ResponseViewModel<string>.Fail(message, errorCode);

                        context.Response.ContentType = "application/json";

                        var json = JsonSerializer.Serialize(response);
                        await context.Response.WriteAsync(json);

                        newBody.Seek(0, SeekOrigin.Begin);
                        await newBody.CopyToAsync(originalBodyStream);
                    }

                    return;
                }
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, originalBodyStream);
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception, Stream originalBodyStream)
        {
            _logger.LogError(exception, "Unhandled exception occurred");

            var errorCode = ErrorCode.UnknownError;
            var statusCode = (int)HttpStatusCode.InternalServerError;
            var message = "An unexpected error has occurred.";

            switch (exception)
            {
                case ValidationException validationEx:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    errorCode = ErrorCode.ValidationError;
                    message = validationEx.Message;
                    break;

                case KeyNotFoundException:
                    statusCode = (int)HttpStatusCode.NotFound;
                    errorCode = ErrorCode.NotFound;
                    message = exception.Message;
                    break;

                case UnauthorizedAccessException:
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    errorCode = ErrorCode.Unauthorized;
                    message = exception.Message;
                    break;

                case InvalidOperationException invalidOp:
                    statusCode = (int)HttpStatusCode.Conflict;
                    errorCode = ErrorCode.Conflict;
                    message = invalidOp.Message;
                    break;

                default:
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    errorCode = ErrorCode.ServerError;
                    message = exception.Message;
                    break;
            }

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var response = ResponseViewModel<string>.Fail(message, errorCode);
            var json = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);
        }
    }
}
