using System.Security.Claims;
using Wrak.Clean.Blazor.Web.Identity;

namespace Wrak.Clean.Blazor.UnitTests.Web;

public class TestClaimsPrincipal
{
    public static ClaimsPrincipal User(string claimVal = Claims.User.Value) => PrincipalWithAllClaims(claimVal, "no");
    public static ClaimsPrincipal Admin(string claimVal = Claims.Administrator.Value) => PrincipalWithAllClaims("no", claimVal);

    public static ClaimsPrincipal FromClaim(string name, string value)
    {
        var claims = new Claim[] { new Claim(name, value) };
        var claimsIdentity = new ClaimsIdentity(claims, "Basic");
        return new ClaimsPrincipal(claimsIdentity);
    }

    public static ClaimsPrincipal FromClaims(string[] names, string[] values)
    {
        var claims = new Claim[names.Length];

        for (var i = 0; i < names.Length; i++)
        {
            var name = names[i];
            var value = values[i];

            claims[i] = new Claim(name, value);
        }

        var claimsIdentity = new ClaimsIdentity(claims, "Basic");
        return new ClaimsPrincipal(claimsIdentity);
    }

    private static ClaimsPrincipal PrincipalWithAllClaims(string isUser, string isAdmin)
    {
        var firstName = "Fake";
        var lastName = "User";
        var userName = "fake_user@fake.com";

        string[] names = { "given_name", "family_name", "preferred_username", Claims.User.Type, Claims.Administrator.Type };
        string[] values = { firstName, lastName, userName, isUser, isAdmin };

        return FromClaims(names, values);
    }
}

