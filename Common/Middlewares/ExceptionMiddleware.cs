using System.Net;
using System.Text.Json;
using UrlShorter.Common.Exceptions;
using UrlShorter.Common.Responses;

namespace UrlShorter.Common.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(
        RequestDelegate next,
        IWebHostEnvironment env,
        ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _env = env;
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        // 🧠 log error
        _logger.LogError(ex, "Unhandled exception occurred");

        context.Response.ContentType = "application/json";

        int statusCode;
        string errorCode;

        // 🔥 handle custom exceptions
        if (ex is BaseException baseEx)
        {
            statusCode = baseEx.StatusCode;
            errorCode = baseEx.ErrorCode;
        }
        else
        {
            statusCode = (int)HttpStatusCode.InternalServerError;
            errorCode = "INTERNAL_SERVER_ERROR";
        }

        var response = new ApiResponse<object>
        {
            Success = false,
            Message = ex.Message,
        };

        // 🧪 dev فقط
        var errorDetails = _env.IsDevelopment() ? ex.StackTrace : null;

        context.Response.StatusCode = statusCode;

        var result = new
        {
            success = response.Success,
            message = response.Message,
            errorCode,
            error = errorDetails
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(result));
    }
}