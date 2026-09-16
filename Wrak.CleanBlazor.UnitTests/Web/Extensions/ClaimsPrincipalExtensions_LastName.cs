using Wrak.CleanBlazor.Web.Extensions;
using Wrak.CleanBlazor.Web.Identity;

namespace Wrak.CleanBlazor.UnitTests.Web.Extensions;

public class ClaimsPrincipalExtensions_LastName
{
    [Fact]
    public void FromClaim()
    {
        var name = "Zidane";
        Assert.Equal(name, TestClaimsPrincipal.FromClaim(Claims.LastName.Type, name).LastName());
    }
}
