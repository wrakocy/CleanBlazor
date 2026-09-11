using Wrak.Clean.Blazor.Web.Extensions;
using Wrak.Clean.Blazor.Web.Identity;

namespace Wrak.Clean.Blazor.UnitTests.Web.Extensions;

public class ClaimsPrincipalExtensions_UserName
{
    [Fact]
    public void FromClaim()
    {
        var name = "zizou98";
        Assert.Equal(name, TestClaimsPrincipal.FromClaim(Claims.UserName.Type, name).UserName());
    }
}
