namespace Wrak.CleanBlazor.FunctionalTests.Endpoints;

public class Health : WebApplicationTestFixtureBase
{
    public Health(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper) : base(factory, outputHelper) { }

    [Fact]
    public async Task ReturnsViewWithCorrectMessage()
    {
        var rsp = await _client.GetAsync("/health", TestContext.Current.CancellationToken);
        rsp.EnsureSuccessStatusCode();
        var stringRsp = await rsp.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Contains("Healthy", stringRsp);
    }
}
