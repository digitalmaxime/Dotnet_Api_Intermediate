using System.Net;
using System.Text.Json;

namespace AgentFrameworkChat.Middlewares;

public class ExceptionMiddleware(ILogger<ExceptionMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            next(context);
        }
        catch (Exception exception)
        {
            await HandleException(context, exception);
        }
    }

    private async Task HandleException(HttpContext context, Exception exception)
    {
        logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

        var (statusCode, message) = GetStatusCodeAndMessage(exception);

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";
        var errorResponse = new ErrorResponse
        {
            StatusCode =  (int)statusCode,
            Message = message,
            Type = exception.GetType().Name,
            TraceId = Guid.NewGuid().ToString()
        };
        
        var json = JsonSerializer.Serialize(errorResponse);
        await context.Response.WriteAsJsonAsync(json);
    }

    private (HttpStatusCode statusCode, string message) GetStatusCodeAndMessage(Exception exception)
    {
        return exception switch
        {
            BadHttpRequestException badRequest => (HttpStatusCode.BadRequest, badRequest.Message),

            KeyNotFoundException => (HttpStatusCode.NotFound, "The request resource was not found"),

            UnauthorizedAccessException => (HttpStatusCode.Forbidden,
                "You do not have permission to access this resource"),

            ArgumentException argumentException => (HttpStatusCode.BadRequest, argumentException.Message),

            InvalidOperationException => (HttpStatusCode.BadRequest, exception.Message),

            _ => (HttpStatusCode.InternalServerError, exception.Message)
        };
    }

    private sealed class ErrorResponse
    {
        public int StatusCode { get; init; }
        public string Message { get; init; } = string.Empty;
        public string Type { get; init; } = string.Empty;
        public string TraceId { get; init; } = string.Empty;
    }
}