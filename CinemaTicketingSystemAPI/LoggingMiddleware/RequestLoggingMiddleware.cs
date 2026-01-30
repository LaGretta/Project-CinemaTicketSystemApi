namespace CinemaTicketingSystemAPI.LoggingMiddleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        var method = context.Request.Method;
        var path = context.Request.Path;
        
        Console.WriteLine($"[{timestamp}] Запит: {method} {path}");
        await _next(context);
    }
}