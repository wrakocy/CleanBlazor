using Wrak.CleanBlazor.Web;

namespace Wrak.CleanBlazor.UnitTests.Web;

public class AppIdentity_Name
{
    [Fact]
    public void IsCorrect()
    {
        Assert.Equal("App", new AppIdentity().Name);
    }
}
