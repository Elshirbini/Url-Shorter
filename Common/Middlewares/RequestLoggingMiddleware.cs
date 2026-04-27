using System.Diagnostics;

namespace UrlShorter.Common.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        var method = context.Request.Method;
        var path = context.Request.Path;

        _logger.LogInformation("➡️ Incoming Request: {Method} {Path}", method, path);

        await _next(context);

        stopwatch.Stop();

        var statusCode = context.Response.StatusCode;

        _logger.LogInformation("⬅️ Response: {Method} {Path} → {StatusCode} in {Elapsed} ms",
            method,
            path,
            statusCode,
            stopwatch.ElapsedMilliseconds);
    }
}