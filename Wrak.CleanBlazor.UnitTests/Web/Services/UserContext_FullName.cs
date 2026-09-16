using Wrak.CleanBlazor.Web.Identity;
using Wrak.CleanBlazor.Web.Services;

namespace Wrak.CleanBlazor.UnitTests.Web.Services;

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
