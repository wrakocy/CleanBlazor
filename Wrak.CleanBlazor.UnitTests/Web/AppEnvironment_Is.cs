using Wrak.CleanBlazor.Core.Shared.Interfaces;
using Wrak.CleanBlazor.Web;

namespace Wrak.CleanBlazor.UnitTests.Web;

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
