using Wrak.CleanArchitecture.Core.Shared.Interfaces;
using Wrak.CleanArchitecture.Web;

namespace Wrak.CleanArchitecture.UnitTests.Web;

public class AppEnvironment_SetAndGet
{
    [Theory]
    [InlineData("", EnvironmentType.Development)]
    [InlineData("Development", EnvironmentType.Development)]
    [InlineData("Production", EnvironmentType.Production)]
    [InlineData("production", EnvironmentType.Development)]
    [InlineData("Production ", EnvironmentType.Development)]
    public void SetAndGet(string envString, EnvironmentType expectedEnvType)
    {
        // Arrange
        var appEnv = new AppEnvironment(envString);

        // Act and Assert
        Assert.Equal(appEnv.GetEnvironment(), expectedEnvType);
    }
}
