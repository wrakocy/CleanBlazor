using Wrak.Clean.Blazor.Web.Extensions;
using Wrak.Clean.Blazor.Web.Identity;

namespace Wrak.Clean.Blazor.UnitTests.Web.Extensions;

public class ClaimsPrincipalExtensions_FirstName
{
    [Fact]
    public void FromClaim()
    {
        var name = "Zinadine";
        Assert.Equal(name, TestClaimsPrincipal.FromClaim(Claims.FirstName.Type, name).FirstName());
    }
}
