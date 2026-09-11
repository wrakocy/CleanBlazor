using Wrak.Clean.Blazor.Web.Extensions;
using Wrak.Clean.Blazor.Web.Identity;

namespace Wrak.Clean.Blazor.UnitTests.Web.Extensions;

public class ClaimsPrincipalExtensions_FullName
{
    [Fact]
    public void FromClaim()
    {
        var name = "Zinadine Zidane";
        Assert.Equal(name, TestClaimsPrincipal.FromClaim(Claims.FullName.Type, name).FullName());
    }
}
