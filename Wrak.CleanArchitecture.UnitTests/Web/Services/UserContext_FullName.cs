using Wrak.CleanArchitecture.Web.Identity;
using Wrak.CleanArchitecture.Web.Services;

namespace Wrak.CleanArchitecture.UnitTests.Web.Services;

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
