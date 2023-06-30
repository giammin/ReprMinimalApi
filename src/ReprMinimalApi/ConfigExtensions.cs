using FluentValidation;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Rewrite;

namespace ReprMinimalApi;

public static class ConfigExtensions
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<ValidationPipeline>();
        builder.Services.AddTransient<CreatePostHandler>();
        builder.Services.AddTransient<IValidator<CreatePostCommand>, CreatePostCommandValidator>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        //easy way to log responsebody. the rest is logged with WebLoggerMiddleware
        builder.Services.AddHttpLogging(c => c.LoggingFields = HttpLoggingFields.ResponseBody);
        return builder;
    }

    public static WebApplication MapHandlers(this WebApplication app)
    {
        //HEALTHCHECK
        app.MapGet("", () => Results.Ok());

        app.MapPost("/createpost", (
            CreatePostCommand command,
            CreatePostHandler handler,
            ValidationPipeline pipeline,
            CancellationToken cancellationToken
        ) => pipeline.Exec(command, handler.Handle, cancellationToken));

        return app;
    }

    public static WebApplicationBuilder ConfigureCors(this WebApplicationBuilder builder)
    {
        var allowedOriginsConfig = builder.Configuration.GetSection("AllowedOrigins").Value;


        if (string.IsNullOrWhiteSpace(allowedOriginsConfig))
        {
            throw new Exception("AllowedOrigins config missing");
        }
        var allowedOrigins = allowedOriginsConfig.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);

        builder.Services.AddCors(o =>
        {
            o.AddDefaultPolicy(
                p => p.WithOrigins(allowedOrigins)
                    .WithMethods("GET", "POST")
                    .AllowCredentials()
                    .AllowAnyHeader());
        });
        return builder;
    }
    public static IApplicationBuilder UseLogging(this IApplicationBuilder app)
    {
        app.UseHttpLogging();
        
        app.UseWebLoggerMiddleware(c =>
        {
            c.LogLevel = LogLevel.Information;
            c.IncludeBody = true;
            c.HandleExceptionResponse = true;
        });
        return app;
    }
    /// <summary>
    /// bot goes to youtube
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseHappyBot(this IApplicationBuilder app)
    {
        var options = new RewriteOptions()
            .Add(context =>
            {
                var path = context.HttpContext.Request.Path;
                if (path.HasValue && Configuration.BotRequestRegex.IsMatch(path.Value))
                {
                    var redirectUrl = Configuration.TenHoursOfFun[Random.Shared.Next(0, Configuration.TenHoursOfFun.Length)];
                    var request = WebLoggerHelper.GetRequestDataAsync(context.HttpContext.Request).GetAwaiter().GetResult();
                    context.Logger.LogDebug("Bot found redirecting to {redirectUrl} request: {request}", redirectUrl, request);
                    context.Result = RuleResult.EndResponse;
                    context.HttpContext.Response.StatusCode = 301;
                    context.HttpContext.Response.Headers.Location = redirectUrl;
                }
            });
        app.UseRewriter(options);
        return app;
    }
    public static IApplicationBuilder UseWebLoggerMiddleware(this IApplicationBuilder builder, Action<WebLoggerMiddlewareSettings>? action = null)
    {
        var config = new WebLoggerMiddlewareSettings();
        action?.Invoke(config);
        return builder.UseMiddleware<WebLoggerMiddleware>(config);
    }
}