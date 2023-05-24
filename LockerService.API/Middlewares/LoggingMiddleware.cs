using System.Text;

namespace LockerService.API.Middlewares;

public class LoggingMiddleware
{
    private readonly ILogger _logger;
    private readonly RequestDelegate _next;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var startAt = DateTime.Now;
        
        try
        {
            await _next(context);
        }
        finally
        {
            var endAt = DateTime.Now;

            var duration = endAt - startAt;

            _logger.LogInformation("Response Time: {0} ms", duration.Milliseconds);
        }
    }
}