using Wrak.CleanBlazor.Web.Extensions;
using Wrak.CleanBlazor.Web.Identity;

namespace Wrak.CleanBlazor.UnitTests.Web.Extensions;

public class ClaimsPrincipalExtensions_FirstName
{
    [Fact]
    public void FromClaim()
    {
        var name = "Zinadine";
        Assert.Equal(name, TestClaimsPrincipal.FromClaim(Claims.FirstName.Type, name).FirstName());
    }
}
