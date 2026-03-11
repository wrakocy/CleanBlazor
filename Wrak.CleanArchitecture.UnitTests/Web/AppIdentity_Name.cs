using Wrak.CleanArchitecture.Web;

namespace Wrak.CleanArchitecture.UnitTests.Web;

public class AppIdentity_Name
{
    [Fact]
    public void IsCorrect()
    {
        Assert.Equal("App", new AppIdentity().Name);
    }
}
