using Wrak.CleanArchitecture.Core.Shared.Interfaces;
using Wrak.CleanArchitecture.Web;

namespace Wrak.CleanArchitecture.UnitTests.Web;

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

