using Wrak.CleanArchitecture.Web.Extensions;
using Wrak.CleanArchitecture.Web.Identity;

namespace Wrak.CleanArchitecture.UnitTests.Web.Extensions;

public class ClaimsPrincipalExtensions_FirstName
{
    [Fact]
    public void FromClaim()
    {
        var name = "Zinadine";
        Assert.Equal(name, TestClaimsPrincipal.FromClaim(Claims.FirstName.Type, name).FirstName());
    }
}
