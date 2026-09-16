using Wrak.CleanBlazor.Web.Extensions;
using Wrak.CleanBlazor.Web.Identity;

namespace Wrak.CleanBlazor.UnitTests.Web.Extensions;

public class ClaimsPrincipalExtensions_Email
{
    [Fact]
    public void FromClaim()
    {
        var email = "zizou98@fifa.com";
        Assert.Equal(email, TestClaimsPrincipal.FromClaim(Claims.Email.Type, email).Email());
    }
}
