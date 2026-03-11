using Wrak.CleanArchitecture.Web.Extensions;
using Wrak.CleanArchitecture.Web.Identity;

namespace Wrak.CleanArchitecture.UnitTests.Web.Extensions;

public class ClaimsPrincipalExtensions_FullName
{
    [Fact]
    public void FromClaim()
    {
        var name = "Zinadine Zidane";
        Assert.Equal(name, TestClaimsPrincipal.FromClaim(Claims.FullName.Type, name).FullName());
    }
}
