using System.Security.Claims;

namespace Wrak.Clean.Blazor.Core.Shared.Interfaces;

public interface IUserContext
{
    string? FirstName { get; }
    string? FullName { get; }
    bool IsAdmin { get; }
    bool IsAuthenticated { get; }
    bool IsUser { get; }
    string? LastName { get; }
    string? UserName { get; }

    void NotifyPrincipalChanged(ClaimsPrincipal principal);
}
