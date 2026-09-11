namespace Wrak.Clean.Blazor.FunctionalTests.Components.Pages.Root.About;

public class About : WebApplicationTestFixtureBase
{
    public About(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper) : base(factory, outputHelper) { }

    [Fact]
    public async Task ReturnsViewWithCorrectMessage()
    {
        var rsp = await _client.GetAsync("/about");
        rsp.EnsureSuccessStatusCode();
        var stringRsp = await rsp.Content.ReadAsStringAsync();

        Assert.Contains("About", stringRsp);
    }
}
