using System.Security.Claims;
using Wrak.CleanBlazor.Web.Extensions;

namespace Wrak.CleanBlazor.UnitTests.Web.Extensions;

public class ClaimsPrincipalExtensions_IsUser
{
    [Fact]
    public void Is()
    {
        Assert.True(TestClaimsPrincipal.User().IsUser());
    }

    [Fact]
    public void IsNot()
    {
        Assert.False(TestClaimsPrincipal.User("no").IsUser());
        Assert.False(TestClaimsPrincipal.Admin().IsUser());
        Assert.False(new ClaimsPrincipal().IsUser());
    }
}
