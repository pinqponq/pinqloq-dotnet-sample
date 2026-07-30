using Pinqloq;

namespace Pinqloq.Sample.Api.Middleware;

/// <summary>
/// Catches unhandled exceptions and demonstrates pinqloq's manual Error-level logging
/// (Detail carries the exception type, message, and stack trace).
/// </summary>
public class PinqloqExceptionLoggingMiddleware
{
    private const string DemoIdentifier = "demo-user";

    private readonly RequestDelegate _next;

    public PinqloqExceptionLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IPinqloqLogger pinqloqLogger)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            pinqloqLogger.Enqueue(new PinqloqLogEntry
            {
                Event = "UnhandledException",
                Identifier = DemoIdentifier,
                LogLevel = PinqloqLogLevel.Error,
                CollectionName = PinqloqCollections.Jobs,
                Detail = new Dictionary<string, string>
                {
                    ["exceptionType"] = exception.GetType().Name,
                    ["message"] = exception.Message,
                    ["stackTrace"] = exception.StackTrace ?? string.Empty
                }
            });

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { error = "Something went wrong. See the pinqloq panel for details." });
        }
    }
}
