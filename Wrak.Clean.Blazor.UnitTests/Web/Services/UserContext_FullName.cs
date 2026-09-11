using Wrak.Clean.Blazor.Web.Identity;
using Wrak.Clean.Blazor.Web.Services;

namespace Wrak.Clean.Blazor.UnitTests.Web.Services;

public class UserContext_FullName
{
    [Fact]
    public void FromGivenAndFamilyName()
    {
        var names = new[] { Claims.FirstName.Type, Claims.LastName.Type };
        var values = new[] { "Willie", "Brown" };
        var principal = TestClaimsPrincipal.FromClaims(names, values);
        var ctx = new UserContext();
        ctx.NotifyPrincipalChanged(principal);

        Assert.Equal("Willie Brown", ctx.FullName);
    }
}
