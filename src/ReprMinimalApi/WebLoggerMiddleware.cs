using System.Diagnostics;
using System.Net;
using System.Text.Json;

namespace ReprMinimalApi;

public class WebLoggerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly WebLoggerMiddlewareSettings _settings;

    public WebLoggerMiddleware(RequestDelegate next, WebLoggerMiddlewareSettings settings)
    {
        _next = next;
        _settings = settings;
    }

    public async Task Invoke(HttpContext context, ILogger<WebLoggerMiddleware> logger)
    {
        long start=0;
        if (logger.IsEnabled(_settings.LogLevel))
        {
            logger.Log(_settings.LogLevel, await WebLoggerHelper.GetRequestDataAsync(context.Request, _settings.IncludeBody).ConfigureAwait(false));
            start = Stopwatch.GetTimestamp();
        }

        try
        {
            await _next(context);
            if (logger.IsEnabled(_settings.LogLevel))
            {
                var stop = Stopwatch.GetTimestamp();
                var elapsed = new TimeSpan(stop - start);
                logger.Log(_settings.LogLevel, "RESPONSE: {guid} took {seconds}ms",WebLoggerHelper.GetRequestUid(context), elapsed.TotalMilliseconds);
                logger.Log(_settings.LogLevel, WebLoggerHelper.GetResponseDataAsync(context.Response));
            }
        }
        catch (Exception exception)
        {
            if (!logger.IsEnabled(_settings.LogLevel))
            {
                logger.LogError(exception, await WebLoggerHelper.GetRequestDataAsync(context.Request, true));
            }
            else
            {
                logger.LogError(exception, "RESPONSE: {guid} throw exception", WebLoggerHelper.GetRequestUid(context));
            }
            
            if (!_settings.HandleExceptionResponse)
            {
                throw;
            }

            var jsonException = new ExceptionJsonReturn(exception);
            var result = JsonSerializer.Serialize(jsonException);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            logger.LogInformation("RESPONSE: {guid} Returning {statusCode} response: {response}", WebLoggerHelper.GetRequestUid(context), context.Response.StatusCode,
                jsonException);
            await context.Response.WriteAsync(result);
        }
    }
}
public record WebLoggerMiddlewareSettings
{
    /// <summary>
    /// livello che se attivato vengono emessi i log
    /// indica anche il livello a cui sono emessi i log
    /// </summary>
    public LogLevel LogLevel { get; set; } = LogLevel.Debug;
    /// <summary>
    /// includi il body delle richieste nei log
    /// </summary>
    public bool IncludeBody { get; set; }
    /// <summary>
    /// se ci sono eccezioni durante la richiesta le incapsula in <see cref="ExceptionJsonReturn"/>
    /// </summary>
    public bool HandleExceptionResponse { get; set; }
}