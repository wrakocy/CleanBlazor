using Wrak.CleanArchitecture.Web.Extensions;
using Wrak.CleanArchitecture.Web.Identity;

namespace Wrak.CleanArchitecture.UnitTests.Web.Extensions;

public class ClaimsPrincipalExtensions_UserName
{
    [Fact]
    public void FromClaim()
    {
        var name = "zizou98";
        Assert.Equal(name, TestClaimsPrincipal.FromClaim(Claims.UserName.Type, name).UserName());
    }
}
