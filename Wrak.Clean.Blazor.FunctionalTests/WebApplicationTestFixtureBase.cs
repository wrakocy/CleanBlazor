namespace Wrak.Clean.Blazor.FunctionalTests;

public abstract class WebApplicationTestFixtureBase : IClassFixture<CustomWebApplicationFactory>
{
    protected ITestOutputHelper _outputHelper;
    protected readonly HttpClient _client;

    public WebApplicationTestFixtureBase(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        _client = factory.CreateClient();
    }
}
