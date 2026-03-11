namespace Wrak.CleanArchitecture.UnitTests.Web;

public class AppIdentity_ClientId
{
    [Fact]
    public void IsCorrect()
    {
        Assert.Equal("app-v1", new CleanArchitecture.Web.AppIdentity().ClientId);
    }
}
