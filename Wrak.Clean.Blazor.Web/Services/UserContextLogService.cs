using Serilog.Core;
using Serilog.Events;
using Wrak.Clean.Blazor.Core.Shared.Interfaces;

namespace Wrak.Clean.Blazor.Web.Services;

public sealed class UserContextLogService(ICircuitServicesAccessor circuitAccessor) : ILogEventEnricher
{
    public const string UserPropertyName = "UserName";

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        // circuitAccessor.Services is null outside of an active Blazor circuit
        // (e.g. during startup, background services), so guard against that.
        var services = circuitAccessor.Services;
        if (services is null) return;

        var userCtx = services.GetService<IUserContext>();
        if (userCtx is null) return;

        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(UserPropertyName, userCtx.UserName));
    }
}
