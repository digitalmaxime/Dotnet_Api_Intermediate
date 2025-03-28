using System.Net;
using System.Text.Json;

namespace Api.Middleware;

public class ExceptionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        Console.WriteLine("ExceptionMiddleware executing - Before the request");
        try
        {
            await next(context);
            Console.WriteLine("ExceptionMiddleware executing - After the request");
        }
        catch (Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            await context.Response.WriteAsync(new ErrorDetails()
            {
                StatusCode = context.Response.StatusCode,
                Message = "Internal Server Error from the custom middleware.",
                ExceptionMessage = ex.Message
            }.ToString());
        }
    }
}

public class ErrorDetails
{
    public int StatusCode { get; set; }
    public required string Message { get; set; }
    public required string ExceptionMessage { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}