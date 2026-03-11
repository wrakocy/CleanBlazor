using Ardalis.ListStartupServices;
using Wrak.CleanArchitecture.Web.Extensions;
using Wrak.ListComponentRoutes;

try
{
    // Configure app services.
    var builder = WebApplication.CreateBuilder(args);
    var env = builder.Environment;
    var config = builder.Configuration;

    builder.AddInteractiveBlazorServer();
    builder.AddMudBlazor();
    builder.AddAuthentication(config, env);
    builder.AddAuthorization();
    builder.AddHealthChecks();
    builder.AddApplicationInsightsTelemetry();
    builder.AddSerilog(env);
    builder.AddOptions();
    builder.AddMediatr();
    builder.AddCoreServices();
    builder.AddInfrastructureServices();
    builder.AddWebServices();
    builder.AddListServices();
    builder.AddListComponentRoutes();

    // Configure app pipeline and run.
    var app = builder.Build();

    if (env.IsDevelopment())
    {
        app.UseListComponentRoutes();
        app.UseShowAllServicesMiddleware();
        app.UseDeveloperExceptionPage();
        app.UseForwardedHeaders();
    }
    else
    {
        app.UseExceptionHandler("/error");
        app.UseForwardedHeaders();
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseAntiforgery();
    app.MapStaticAssets();
    app.MapRazorComponentsWithAuthorization();
    app.MapHealthChecks("/health");
    app.Map404Fallback();
    app.MapSignout(env);

    Log.Information($"Starting application. Environment: {env.EnvironmentName}");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly.");
}
finally
{
    Log.Information($"Flushing logs and shutting down...");
    Log.CloseAndFlush();
}
