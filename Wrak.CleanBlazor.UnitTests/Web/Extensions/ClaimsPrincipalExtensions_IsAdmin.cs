using System.Security.Claims;
using Wrak.CleanBlazor.Web.Extensions;

namespace Wrak.CleanBlazor.UnitTests.Web.Extensions;

public class ClaimsPrincipalExtensions_IsAdmin
{
    [Fact]
    public void IsAdmin()
    {
        Assert.True(TestClaimsPrincipal.Admin().IsAdmin());
    }

    [Fact]
    public void IsNotAdmin()
    {
        Assert.False(TestClaimsPrincipal.Admin("no").IsAdmin());
        Assert.False(TestClaimsPrincipal.User().IsAdmin());
        Assert.False(new ClaimsPrincipal().IsAdmin());
    }
}
