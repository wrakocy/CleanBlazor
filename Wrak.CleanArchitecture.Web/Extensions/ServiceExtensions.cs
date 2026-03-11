using System.Net;
using Ardalis.ListStartupServices;
using FluentValidation;
using MediatR;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using MudBlazor.Services;
using Serilog.Events;
using Wrak.CleanArchitecture.Core;
using Wrak.CleanArchitecture.Core.Shared.Interfaces;
using Wrak.CleanArchitecture.Infrastructure;
using Wrak.CleanArchitecture.Infrastructure.Filters;
using Wrak.CleanArchitecture.Web.Components.App;
using Wrak.CleanArchitecture.Web.Extensions;
using Wrak.CleanArchitecture.Web.Filters;
using Wrak.CleanArchitecture.Web.Interfaces;
using Wrak.CleanArchitecture.Web.Services;
using Wrak.ListComponentRoutes;

namespace Wrak.CleanArchitecture.Web.Extensions;

public static class ServiceExtensions
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

    public static void AddApplicationInsightsTelemetry(this WebApplicationBuilder builder)
    {
        builder.Services.AddApplicationInsightsTelemetry();
    }

    public static void AddSerilog(this WebApplicationBuilder builder, IWebHostEnvironment env)
    {
        builder.Host.UseSerilog((_, services, config) =>
        {
            var logConfig = config.ReadFrom.Configuration(builder.Configuration);
            var telemetryConfig = services.GetRequiredService<TelemetryConfiguration>();

            logConfig.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning);

            logConfig.Enrich.WithProperty("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!)
                     .Enrich.WithProperty("ApplicationName", "Wrak.CleanArchitecture.Web")
                     .Enrich.WithProperty("Product", "Wrak.CleanArchitecture.Web")
                     .Enrich.WithProperty("Version", $"{BuildInfo.Build}")
                     .Enrich.FromLogContext()
                     .Enrich.WithEnvironmentUserName()
                     .Enrich.WithMachineName();

            logConfig.WriteTo.ApplicationInsights(
                    telemetryConfig,
                    TelemetryConverter.Traces);
        });
    }

    public static void AddOptions(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var config = builder.Configuration;

        // TODO
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

        // allow Blazor ciruit-scoped services to
        // access services not scoped to the circuit
        services.AddCircuitServicesAccessor();
    }

    public static void AddWebServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var envVar = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        services.AddSingleton<IAppIdentity>(x => new AppIdentity());
        services.AddSingleton<IAppEnvironment>(x => new AppEnvironment(envVar!));

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
        var services = builder.Services;

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
        var services = builder.Services;

        services.AddListComponentRoutes(options =>
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
           .RequireAuthorization(); // Require authorization for all server-rendered components.
    }
}
