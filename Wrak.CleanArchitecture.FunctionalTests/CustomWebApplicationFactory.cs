using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Wrak.CleanArchitecture.FunctionalTests.Mocks;
using Wrak.CleanArchitecture.Web;
using Wrak.CleanArchitecture.Web.Interfaces;

namespace Wrak.CleanArchitecture.FunctionalTests;

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
