using Wrak.Clean.Blazor.Web;

namespace Wrak.Clean.Blazor.UnitTests.Web;

public class AppIdentity_ClientId
{
    [Fact]
    public void IsCorrect()
    {
        Assert.Equal("app-v1", new AppIdentity().ClientId);
    }
}
