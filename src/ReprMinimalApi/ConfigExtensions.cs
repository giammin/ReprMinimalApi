using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Rewrite;
using ReprMinimalApi.Filters;
using ReprMinimalApi.Middleware;
using ReprMinimalApi.Posts;
using ReprMinimalApi.Utils;
using System.Text.RegularExpressions;

namespace ReprMinimalApi;

public static class ConfigExtensions
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(t=>t.Name.EndsWith("Handler") && !t.IsAbstract && !t.IsInterface))
        {
            builder.Services.AddScoped(type);
        }
        builder.Services.AddScoped<GenericFluentValidationFilter<CreatePostCommand>>();
        builder.Services.AddValidatorsFromAssemblyContaining<CreatePostCommandValidator>();

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
        app.MapPostEndpoints();
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
                if (path.HasValue && BotRequestRegex.IsMatch(path.Value))
                {
                    var redirectUrl = TenHoursOfFun[Random.Shared.Next(0, TenHoursOfFun.Length)];
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
    public static bool Debug
#if DEBUG
        => true;
#else
        => false;
#endif
    public static readonly Regex BotRequestRegex = new("^.*(\\.yaml|\\.old|\\.bak|\\.xml|\\.cfg|\\.js|\\.ini|phpinfo|\\.txt|\\.php[\\d]?|\\.?env(\\.|\\/)?\\d?(old|prod|staging)?)$", RegexOptions.Compiled & RegexOptions.IgnoreCase & RegexOptions.Singleline, TimeSpan.FromSeconds(1));

    public static readonly string[] TenHoursOfFun =
    {
        "https://www.youtube.com/watch?v=wbby9coDRCk",
        "https://www.youtube.com/watch?v=nb2evY0kmpQ",
        "https://www.youtube.com/watch?v=z9Uz1icjwrM",
        "https://www.youtube.com/watch?v=Sagg08DrO5U",
        "https://www.youtube.com/watch?v=5XmjJvJTyx0",
        "https://www.youtube.com/watch?v=jScuYd3_xdQ",
        "https://www.youtube.com/watch?v=S5PvBzDlZGs",
        "https://www.youtube.com/watch?v=9UZbGgXvCCA",
        "https://www.youtube.com/watch?v=O-dNDXUt1fg",
        "https://www.youtube.com/watch?v=MJ5JEhDy8nE",
        "https://www.youtube.com/watch?v=VnnWp_akOrE",
        "https://www.youtube.com/watch?v=jwGfwbsF4c4",
        "https://www.youtube.com/watch?v=hGlyFc79BUE",
        "https://www.youtube.com/watch?v=xA8-6X8aR3o",
        "https://www.youtube.com/watch?v=7R1nRxcICeE",
        "https://www.youtube.com/watch?v=bIoe_IR9MB8",
        "https://www.youtube.com/watch?v=sCNrK-n68CM"
    };
}