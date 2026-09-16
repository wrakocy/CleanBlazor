using Wrak.CleanBlazor.Web.Identity;
using Wrak.CleanBlazor.Web.Services;

namespace Wrak.CleanBlazor.UnitTests.Web.Services;

public class UserContext_NotifyPrincipalChanged
{
    [Theory]
    [InlineData(Claims.User.Value, "no", true, false)]
    [InlineData("no", Claims.Administrator.Value, false, true)]
    public void Construct(string userVal, string adminVal, bool shouldBeUser, bool shouldBeAdmin)
    {
        // Arrange
        var firstName = "Ted";
        var lastName = "Danson";
        var userName = "tdanson@cheersbar.com";
        var names = new[] { Claims.FirstName.Type, Claims.LastName.Type, Claims.UserName.Type, Claims.User.Type, Claims.Administrator.Type };
        var values = new[] { firstName, lastName, userName, userVal, adminVal };
        var principal = TestClaimsPrincipal.FromClaims(names, values);

        // Act
        var ctx = new UserContext();
        ctx.NotifyPrincipalChanged(principal);

        // Assert
        Assert.Equal(firstName, ctx.FirstName);
        Assert.Equal(lastName, ctx.LastName);
        Assert.Equal(userName, ctx.UserName);
        Assert.Equal(shouldBeAdmin, ctx.IsAdmin);
        Assert.Equal(shouldBeUser, ctx.IsUser);
    }
}
