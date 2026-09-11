using Wrak.Clean.Blazor.Web;

namespace Wrak.Clean.Blazor.UnitTests.Web;

public class AppIdentity_Name
{
    [Fact]
    public void IsCorrect()
    {
        Assert.Equal("App", new AppIdentity().Name);
    }
}
