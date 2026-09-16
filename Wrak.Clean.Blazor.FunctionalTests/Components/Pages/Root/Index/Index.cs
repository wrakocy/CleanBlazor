namespace Wrak.Clean.Blazor.FunctionalTests.Components.Pages.Root.Index;

public class Index : WebApplicationTestFixtureBase
{
    public Index(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper) : base(factory, outputHelper) { }

    [Fact]
    public async Task ReturnsViewWithCorrectMessage()
    {
        var rsp = await _client.GetAsync("/", TestContext.Current.CancellationToken);
        rsp.EnsureSuccessStatusCode();
        var stringRsp = await rsp.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Contains("Welcome", stringRsp);
    }
}
