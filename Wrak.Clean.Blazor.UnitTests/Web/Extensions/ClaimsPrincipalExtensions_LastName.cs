using Wrak.Clean.Blazor.Web.Extensions;
using Wrak.Clean.Blazor.Web.Identity;

namespace Wrak.Clean.Blazor.UnitTests.Web.Extensions;

public class ClaimsPrincipalExtensions_LastName
{
    [Fact]
    public void FromClaim()
    {
        var name = "Zidane";
        Assert.Equal(name, TestClaimsPrincipal.FromClaim(Claims.LastName.Type, name).LastName());
    }
}
