using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Wrak.CleanBlazor.Core.Shared.Interfaces;
using Wrak.CleanBlazor.Web.Interfaces;

namespace Wrak.CleanBlazor.Web.Services;

public sealed class UserContextService : IUserContextService
{
    #region Fields

    private readonly SemaphoreSlim _lock;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly IUserContext _userCtx;

    #endregion

    #region Constructor

    public UserContextService(IServiceProvider sp)
    {
        sp.ThrowIfNull();

        _lock = new SemaphoreSlim(1, 1);
        _authStateProvider = sp.GetRequiredService<AuthenticationStateProvider>().ThrowIfNull().Value;
        _userCtx = sp.GetRequiredService<IUserContext>().ThrowIfNull().Value;
    }

    #endregion

    #region Public members

    public async Task<IUserContext> GetUserContextAsync()
    {
        try
        {
            await _lock.WaitAsync();

            if (!_userCtx.IsAuthenticated)
            {
                var principal = (await _authStateProvider.GetAuthenticationStateAsync()).User;
                _userCtx.NotifyPrincipalChanged(principal);
            }

            return _userCtx;
        }
        finally
        {
            _lock.Release();
        }
    }

    #endregion
}

public class UserContext : IUserContext
{
    private ClaimsPrincipal? _principal;

    public string? FirstName => _principal?.FirstName();
    public string? LastName => _principal?.LastName();
    public string? FullName => $"{FirstName} {LastName}";
    public string? UserName => _principal?.UserName();
    public bool IsAuthenticated => _principal?.IsAuthenticated() ?? false;
    public bool IsAdmin => _principal?.IsAdmin() ?? false;
    public bool IsUser => _principal?.IsUser() ?? false;

    public void NotifyPrincipalChanged(ClaimsPrincipal principal)
    {
        _principal = principal.ThrowIfNull().Value;
    }
}
