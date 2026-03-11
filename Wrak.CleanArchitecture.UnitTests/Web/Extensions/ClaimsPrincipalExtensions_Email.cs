using Wrak.CleanArchitecture.Web.Extensions;
using Wrak.CleanArchitecture.Web.Identity;

namespace Wrak.CleanArchitecture.UnitTests.Web.Extensions;

public class ClaimsPrincipalExtensions_Email
{
    [Fact]
    public void FromClaim()
    {
        var email = "zizou98@fifa.com";
        Assert.Equal(email, TestClaimsPrincipal.FromClaim(Claims.Email.Type, email).Email());
    }
}
