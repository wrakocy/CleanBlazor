using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Wrak.Clean.Blazor.FunctionalTests.Mocks;
using Wrak.Clean.Blazor.Web;
using Wrak.Clean.Blazor.Web.Interfaces;

namespace Wrak.Clean.Blazor.FunctionalTests;

public class CustomWebApplicationFactory : WebApplicationFactory<WebMarker>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Add anonymous authentication
            services.AddSingleton<IAuthorizationHandler, AnonymousAuthorizationHandler>();

            // Add mock services
            services.AddSingleton<ISessionStorageService, MockSessionStorageService>();
            services.AddSingleton<ILocalStorageService, MockLocalStorageService>();
        });
    }
}
