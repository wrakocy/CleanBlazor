using Wrak.CleanBlazor.Web.Extensions;
using Wrak.CleanBlazor.Web.Identity;

namespace Wrak.CleanBlazor.UnitTests.Web.Extensions;

public class ClaimsPrincipalExtensions_UserName
{
    [Fact]
    public void FromClaim()
    {
        var name = "zizou98";
        Assert.Equal(name, TestClaimsPrincipal.FromClaim(Claims.UserName.Type, name).UserName());
    }
}
