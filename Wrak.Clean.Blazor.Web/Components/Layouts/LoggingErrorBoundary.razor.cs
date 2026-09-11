using Microsoft.AspNetCore.Components.Web;
using Wrak.Clean.Blazor.Core.Shared.Interfaces;
using Wrak.Clean.Blazor.Web.Interfaces;

namespace Wrak.Clean.Blazor.Web.Components.Layouts;

public class LoggingErrorBoundaryBase : ErrorBoundary
{
    [Inject] private IAppEnvironment _appEnv { get; set; } = default!;
    [Inject] private IAppState _appState { get; set; } = default!;

    protected override void OnParametersSet() => Recover();

    protected override Task OnErrorAsync(Exception ex)
    {
        var descr = $"Caught unhandled exception.";
        Log.Error(ex, descr);

        if (_appEnv.Is(EnvironmentType.Test) || _appEnv.Is(EnvironmentType.Production))
        {
            // In Test and Prod, also log the error to the
            // Traces table in Application Insights.
            Log.ForContext("ExceptionType", ex.GetType().FullName)
               .ForContext("StackTrace", ex.StackTrace)
               .Error(descr);
        }

        // Set working to false to ensure that the
        // progress spinner does not remain active.
        _appState.Working = false;

        return Task.CompletedTask;
    }
}
