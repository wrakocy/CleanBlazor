using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Wrak.CleanArchitecture.Web.Interfaces;

namespace Wrak.CleanArchitecture.Web.Services;

public class SessionStorageService : ISessionStorageService
{
    private readonly ProtectedSessionStorage _storage;

    public SessionStorageService(ProtectedSessionStorage storage)
    {
        _storage = storage.ThrowIfNull().Value;
    }

    public async Task SetAsync(string key, object value)
    {
        await _storage.SetAsync(key, value);
    }

    public async Task<T?> GetAsync<T>(string key, T? defaultVal = default)
    {
        try
        {
            var result = await _storage.GetAsync<T>(key);
            return result.Success ? result.Value : defaultVal;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error retrieving value from session storage for key {Key}", key);
            return defaultVal;
        }
    }
}
