using Wrak.CleanArchitecture.Web;

namespace Wrak.CleanArchitecture.UnitTests.Web;

public class AppIdentity_ClientId
{
    [Fact]
    public void IsCorrect()
    {
        Assert.Equal("app-v1", new AppIdentity().ClientId);
    }
}
