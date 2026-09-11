using Wrak.Clean.Blazor.Core.Shared.Interfaces;
using Wrak.Clean.Blazor.Web;

namespace Wrak.Clean.Blazor.UnitTests.Web;

public class AppEnvironment_Is
{
    [Theory]
    [InlineData("", EnvironmentType.Development)]
    [InlineData("Development", EnvironmentType.Development)]
    [InlineData("Test", EnvironmentType.Test)]
    [InlineData("Production", EnvironmentType.Production)]
    [InlineData("test", EnvironmentType.Development)]
    [InlineData("production", EnvironmentType.Development)]
    [InlineData("Production ", EnvironmentType.Development)]
    public void Is(string envString, EnvironmentType expectedEnvType)
    {
        // Arrange
        var appEnv = new AppEnvironment(envString);

        // Act and Assert
        Assert.True(appEnv.Is(expectedEnvType));
    }
}
