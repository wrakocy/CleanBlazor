namespace Wrak.Clean.Blazor.FunctionalTests.Components.Pages.Root.Index;

public class Index : WebApplicationTestFixtureBase
{
    public Index(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper) : base(factory, outputHelper) { }

    [Fact]
    public async Task ReturnsViewWithCorrectMessage()
    {
        var rsp = await _client.GetAsync("/");
        rsp.EnsureSuccessStatusCode();
        var stringRsp = await rsp.Content.ReadAsStringAsync();

        Assert.Contains("Welcome", stringRsp);
    }
}
