using Wrak.CleanArchitecture.Web.Extensions;
using Wrak.CleanArchitecture.Web.Identity;

namespace Wrak.CleanArchitecture.UnitTests.Web.Extensions;

public class ClaimsPrincipalExtensions_LastName
{
    [Fact]
    public void FromClaim()
    {
        var name = "Zidane";
        Assert.Equal(name, TestClaimsPrincipal.FromClaim(Claims.LastName.Type, name).LastName());
    }
}
