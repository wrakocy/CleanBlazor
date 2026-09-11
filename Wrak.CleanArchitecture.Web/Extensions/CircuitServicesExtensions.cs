using Microsoft.AspNetCore.Components.Server.Circuits;
using Wrak.CleanArchitecture.Core.Shared.Interfaces;

namespace Wrak.CleanArchitecture.Web.Extensions;

// This class allows access to scoped services (e.g., UserContext)
// from outside of Blazor components (e.g., ApiRequestHeadersFilter)
// See: https://learn.microsoft.com/en-us/aspnet/core/blazor/fundamentals/dependency-injection?view=aspnetcore-9.0#access-server-side-blazor-services-from-a-different-di-scope

public class CircuitServicesAccessor : ICircuitServicesAccessor
{
    static readonly AsyncLocal<IServiceProvider> blazorServices = new();

    public IServiceProvider? Services
    {
        get => blazorServices.Value;
        set => blazorServices.Value = value!;
    }
}

public class ServicesAccessorCircuitHandler(
    IServiceProvider services, ICircuitServicesAccessor servicesAccessor)
    : CircuitHandler
{
    public override Func<CircuitInboundActivityContext, Task> CreateInboundActivityHandler(
        Func<CircuitInboundActivityContext, Task> next) =>
            async context =>
            {
                servicesAccessor.Services = services;
                await next(context);
                servicesAccessor.Services = null;
            };
}

public static class CircuitServicesServiceCollectionExtensions
{
    public static IServiceCollection AddCircuitServicesAccessor(
        this IServiceCollection services)
    {
        services.AddSingleton<ICircuitServicesAccessor, CircuitServicesAccessor>();
        services.AddScoped<CircuitHandler, ServicesAccessorCircuitHandler>();

        return services;
    }
}
