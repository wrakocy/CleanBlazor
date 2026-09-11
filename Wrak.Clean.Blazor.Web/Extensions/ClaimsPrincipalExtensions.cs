using System.Security.Claims;
using Wrak.Clean.Blazor.Web.Identity;

namespace Wrak.Clean.Blazor.Web.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? FirstName(this ClaimsPrincipal user)
    {
        return GetClaim(user, Claims.FirstName.Type);
    }

    public static string? LastName(this ClaimsPrincipal user)
    {
        return GetClaim(user, Claims.LastName.Type);

    }
    public static string? FullName(this ClaimsPrincipal user)
    {
        return GetClaim(user, Claims.FullName.Type);
    }

    public static string? UserName(this ClaimsPrincipal user)
    {
        return GetClaim(user, Claims.UserName.Type);
    }

    public static string? Email(this ClaimsPrincipal user)
    {
        return GetClaim(user, Claims.Email.Type);
    }

    public static bool IsAuthenticated(this ClaimsPrincipal user)
    {
        return user?.Identity?.IsAuthenticated ?? false;
    }

    public static bool IsAdmin(this ClaimsPrincipal user)
    {
        return user.HasClaim(x => x.Type == Claims.Administrator.Type && x.Value == Claims.Administrator.Value) ||
            user.HasClaim(x => x.Type == Claims.DevUser.Type && x.Value == Claims.DevUser.Value);
    }

    public static bool IsUser(this ClaimsPrincipal user)
    {
        return user.HasClaim(x => x.Type == Claims.User.Type && x.Value == Claims.User.Value);
    }

    private static string? GetClaim(ClaimsPrincipal principal, string claim)
    {
        return principal?.FindAll(claim)?.FirstOrDefault()?.Value;
    }
}

