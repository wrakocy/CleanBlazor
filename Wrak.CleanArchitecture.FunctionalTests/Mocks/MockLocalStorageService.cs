using Wrak.CleanArchitecture.Web.Interfaces;

namespace Wrak.CleanArchitecture.FunctionalTests.Mocks;

public class MockLocalStorageService : ILocalStorageService
{
    public Task<T?> GetAsync<T>(string key, T? defaultVal = default)
    {
        return Task.FromResult(defaultVal);
    }

    public Task SetAsync(string key, object value)
    {
        // Do nothing
        return Task.CompletedTask;
    }
}
