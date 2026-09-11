using Wrak.Clean.Blazor.Web.Extensions;
using Wrak.Clean.Blazor.Web.Identity;

namespace Wrak.Clean.Blazor.UnitTests.Web.Extensions;

public class ClaimsPrincipalExtensions_Email
{
    [Fact]
    public void FromClaim()
    {
        var email = "zizou98@fifa.com";
        Assert.Equal(email, TestClaimsPrincipal.FromClaim(Claims.Email.Type, email).Email());
    }
}
