using Wrak.CleanBlazor.Web.Extensions;
using Wrak.CleanBlazor.Web.Identity;

namespace Wrak.CleanBlazor.UnitTests.Web.Extensions;

public class ClaimsPrincipalExtensions_FullName
{
    [Fact]
    public void FromClaim()
    {
        var name = "Zinadine Zidane";
        Assert.Equal(name, TestClaimsPrincipal.FromClaim(Claims.FullName.Type, name).FullName());
    }
}
