namespace Wrak.CleanArchitecture.FunctionalTests.Endpoints;

public class Health : WebApplicationTestFixtureBase
{
    public Health(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper) : base(factory, outputHelper) { }

    [Fact]
    public async Task ReturnsViewWithCorrectMessage()
    {
        var rsp = await _client.GetAsync("/health");
        rsp.EnsureSuccessStatusCode();
        var stringRsp = await rsp.Content.ReadAsStringAsync();

        Assert.Contains("Healthy", stringRsp);
    }
}
