using SecurityBase.Core.DTOs;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace SecurityBase.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception has occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = new ApiResponse<object>
        {
            Success = false,
            Message = "Internal Server Error",
            Errors = new List<string> { exception.Message }
        };

        var json = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(json);
    }
}
 public static class ExceptionMiddlewareExtensions
 {
     public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
     {
         return builder.UseMiddleware<ExceptionMiddleware>();
     }
 }
