using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Wrak.CleanBlazor.FunctionalTests.Mocks;
using Wrak.CleanBlazor.Web;
using Wrak.CleanBlazor.Web.Interfaces;

namespace Wrak.CleanBlazor.FunctionalTests;

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
