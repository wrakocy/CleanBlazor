using Wrak.Clean.Blazor.Core.Shared.Interfaces;
using Wrak.Clean.Blazor.Web;

namespace Wrak.Clean.Blazor.UnitTests.Web;

public class AppEnvironment_IsNot
{
    [Theory]
    [InlineData("", EnvironmentType.Production)]
    [InlineData("Development", EnvironmentType.Production)]
    [InlineData("Production", EnvironmentType.Development)]
    [InlineData("production", EnvironmentType.Production)]
    [InlineData("Production ", EnvironmentType.Production)]
    public void IsNot(string envString, EnvironmentType not)
    {
        // Arrange
        var appEnv = new AppEnvironment(envString);

        // Act and Assert
        Assert.True(appEnv.IsNot(not));
    }
}

