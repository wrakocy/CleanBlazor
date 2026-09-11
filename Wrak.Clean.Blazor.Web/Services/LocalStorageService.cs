using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Wrak.Clean.Blazor.Web.Interfaces;

namespace Wrak.Clean.Blazor.Web.Services;

public class LocalStorageService : ILocalStorageService
{
    private readonly ProtectedLocalStorage _storage;

    public LocalStorageService(ProtectedLocalStorage storage)
    {
        _storage = storage.ThrowIfNull().Value;
    }

    public async Task SetAsync(string key, object value)
    {
        await _storage.SetAsync(key, value);
    }

    public async Task<T?> GetAsync<T>(string key, T? defaultVal)
    {
        try
        {
            var result = await _storage.GetAsync<T>(key);
            return result.Success ? result.Value : defaultVal;
        }
        catch (Exception ex)
        {
            Log.Debug(ex, "Error retrieving value from local storage for key {Key}", key);
            return defaultVal;
        }
    }
}
