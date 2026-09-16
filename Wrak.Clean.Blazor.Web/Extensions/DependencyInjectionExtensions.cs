using System.Net;
using Ardalis.ListStartupServices;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using MudBlazor.Services;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog.Events;
using Wrak.Clean.Blazor.Core;
using Wrak.Clean.Blazor.Core.Shared.Interfaces;
using Wrak.Clean.Blazor.Infrastructure;
using Wrak.Clean.Blazor.Infrastructure.Filters;
using Wrak.Clean.Blazor.Web.Components.App;
using Wrak.Clean.Blazor.Web.Filters;
using Wrak.Clean.Blazor.Web.Interfaces;
using Wrak.Clean.Blazor.Web.Services;
using Wrak.ListComponentRoutes;

namespace Wrak.Clean.Blazor.Web.Extensions;

public static class DependencyInjectionExtensions
{
    public static void AddInteractiveBlazorServer(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorComponents().AddInteractiveServerComponents();
    }

    public static void AddAuthentication(this WebApplicationBuilder builder, ConfigurationManager config, IWebHostEnvironment env)
    {
        var services = builder.Services;

        if (!env.IsDevelopment())
        {
            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                    .AddMicrosoftIdentityWebApp(config.GetSection("AzureAd"));
        }
        else
        {
            // In development mode, a mock authentication handler.
            services.AddAuthentication("DevelopmentAuth")
                    .AddScheme<AuthenticationSchemeOptions, DevelopmentAuthHandler>("DevelopmentAuth", null);
        }
    }

    public static void AddAuthorization(this WebApplicationBuilder builder)
    {
        // TODO: Add policies as needed.
        // For now, just add the service with default options.
        builder.Services.AddAuthorization();
    }

    public static void AddMudBlazor(this WebApplicationBuilder builder)
    {
        var services = builder.Services;

        services.AddMudServices(config =>
        {
            var sb = config.SnackbarConfiguration;

            sb.PositionClass = Defaults.Classes.Position.TopCenter;
            sb.PreventDuplicates = false;
            sb.NewestOnTop = true;
            sb.ShowCloseIcon = true;
            sb.MaxDisplayedSnackbars = 3;
            sb.VisibleStateDuration = 4_000;
            sb.HideTransitionDuration = 50;
            sb.ShowTransitionDuration = 250;
            sb.SnackbarVariant = Variant.Filled;
            sb.MaximumOpacity = 96;
        });

        services.AddMudBlazorDialog();
    }

    public static void AddHealthChecks(this WebApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks();
    }

    public static void ClearDefaultLoggingProviders(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddFilter<OpenTelemetryLoggerProvider>("*", LogLevel.None); // suppress OTel log bridge — Serilog handles logs
    }

    public static void AddOpenTelemetry(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetApplicationInsightsConnectionString();
        if (string.IsNullOrEmpty(connectionString)) return;

        var otelBuilder = builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(
                    serviceName: AppConstants.ServiceName,
                    serviceVersion: $"{BuildInfo.Build}"));

        otelBuilder.UseAzureMonitor(options =>
        {
            options.ConnectionString = connectionString;
            options.SamplingRatio = 0.1f;
        });

        otelBuilder
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation(options =>
                {
                    options.Filter = ctx =>
                        !ctx.Request.Path.StartsWithSegments("/health") &&
                        !ctx.Request.Path.StartsWithSegments("/_framework");
                })
                .AddHttpClientInstrumentation())
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation());
    }

    public static void AddSerilog(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetApplicationInsightsConnectionString();
        var sp = builder.Services.BuildServiceProvider();
        var userEnricher = sp.GetRequiredService<UserContextLogService>();

        var loggerConfig = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Extensions.Localization", LogEventLevel.Error)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.WithProperty("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!)
            .Enrich.WithProperty("ApplicationName", AppConstants.ServiceName)
            .Enrich.WithProperty("Version", $"{BuildInfo.Build}")
            .Enrich.FromLogContext()
            .Enrich.With(userEnricher);

        if (!string.IsNullOrEmpty(connectionString))
            loggerConfig.WriteTo.ApplicationInsights(connectionString, TelemetryConverter.Traces);

        Log.Logger = loggerConfig.CreateLogger();

        // Use AddSerilog() instead of UseSerilog() — this ADDS Serilog as a logging
        // provider rather than REPLACING ILoggerFactory, so the OTel logging bridge
        // registered by UseAzureMonitor() is preserved and logs flow to Azure Monitor.
        builder.Logging.AddSerilog(Log.Logger, dispose: false);
    }

    public static void AddOptions(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var config = builder.Configuration;

        // TODO: bind and validate any Options types this solution needs, e.g.:
        // services.AddOptions<SomeApiOptions>().Bind(config.GetSection("SomeApiOptions")).ValidateDataAnnotations();
    }

    public static void AddMediatr(this WebApplicationBuilder builder)
    {
        var services = builder.Services;

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CoreMarker>());
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<InfrastructureMarker>());
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<WebMarker>());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(FeatureLoggingFilter<,>));
    }

    public static void AddCoreServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;

        services.AddTransient<IAppBus, AppBus>();

        // Register all FluentValidation validators in Core as transient
        services.AddValidatorsFromAssemblyContaining<CoreMarker>(ServiceLifetime.Transient);
    }

    public static void AddInfrastructureServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;

        // needed to allow Blazor ciruit-scoped services to
        // access services scoped to the circuit
        services.AddCircuitServicesAccessor();
    }

    public static void AddWebServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var envVar = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        services.AddSingleton<IAppIdentity>(x => new AppIdentity());
        services.AddSingleton<IAppEnvironment>(x => new AppEnvironment(envVar!));
        services.AddSingleton<UserContextLogService>();

        services.AddScoped<IAppState, AppState>();
        services.AddScoped<IUserContextService, UserContextService>();
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<Interfaces.IDialogService, Services.DialogService>();
        services.AddScoped<ISnackBarService, SnackBarService>();
        services.AddScoped<ILocalStorageService, LocalStorageService>();
        services.AddScoped<ISessionStorageService, SessionStorageService>();
        services.AddScoped<IJSInteropService, JSInteropService>();

        services.AddCircuitServicesAccessor(); // needed to access scoped services in non-scoped services
    }

    public static void AddListServices(this WebApplicationBuilder builder)
    {
        // add list services for diagnostic purposes
        // see https://github.com/ardalis/AspNetCoreStartupServices
        builder.Services.Configure<ServiceConfig>(config =>
        {
            config.Services = [.. builder.Services];
            config.Path = "/services";
        });
    }

    public static void AddListComponentRoutes(this WebApplicationBuilder builder)
    {
        builder.Services.AddListComponentRoutes(options =>
        {
            options.Assemblies = [typeof(WebMarker).Assembly];
        });
    }

    public static void Map404Fallback(this WebApplication app)
    {
        app.MapFallback(context =>
        {
            // NOTE: Blazor Web Apps (.NET 9 and up) don't use the NotFound
            // parameter (<NotFound>...</NotFound> markup in Route.cs), but
            // the parameter is still supported for backward compatibility. The
            // server-side ASP.NET Core middleware pipeline handles the requests.
            // Given this, for now, we must use server-side techniques to handle
            // requests for bad URLs. The downside here is that doing so
            // forces the app to reload in the browser, before the Not Found
            // dialog is displayed. This is may be slightly confusing to the user,
            // but is an acceptable trade off for now, barring user complaints.

            // For more information, see: https://github.com/dotnet/aspnetcore/issues/45654
            var status = (int)HttpStatusCode.NotFound;
            context.Response.ContentType = "text/html";
            context.Response.StatusCode = status;
            context.Response.Redirect($"/error?statusCode={status}", false);

            Log.Error($"Requested path '{context.Request.Path}' not found.");
            return Task.CompletedTask;
        });
    }

    public static void MapSignout(this WebApplication app, IWebHostEnvironment env)
    {
        app.MapGet("/signout", async (HttpContext context) =>
        {
            if (!env.IsDevelopment())
            {
                await context.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme,
                    new AuthenticationProperties
                    {
                        // Redirect back to home page,
                        // so that user can seamlessly log back in.
                        RedirectUri = "/"
                    });

                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
            else
            {
                context.Response.Redirect("/");
            }
        });
    }

    public static void MapRazorComponentsWithAuthorization(this WebApplication app)
    {
        app.MapRazorComponents<App>()
           .AddInteractiveServerRenderMode()
           .RequireAuthorization();
    }

    private static string? GetApplicationInsightsConnectionString(this ConfigurationManager config)
    {
        return config["ApplicationInsights:ConnectionString"];
    }
}
