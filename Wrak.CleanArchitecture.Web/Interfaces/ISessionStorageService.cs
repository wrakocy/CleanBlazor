namespace Wrak.CleanArchitecture.Web.Interfaces;

public interface ISessionStorageService
{
    Task<T?> GetAsync<T>(string key, T? defaultVal = default);
    Task SetAsync(string key, object value);
}
