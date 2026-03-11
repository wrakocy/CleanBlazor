namespace Wrak.CleanArchitecture.UnitTests.Web;

public class AppIdentity_Name
{
    [Fact]
    public void IsCorrect()
    {
        Assert.Equal("App", new CleanArchitecture.Web.AppIdentity().Name);
    }
}
