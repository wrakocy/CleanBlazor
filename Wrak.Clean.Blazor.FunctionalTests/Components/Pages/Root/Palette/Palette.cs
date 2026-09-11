namespace Wrak.Clean.Blazor.FunctionalTests.Components.Pages.Root.Palette;

public class Palette : WebApplicationTestFixtureBase
{
    public Palette(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper) : base(factory, outputHelper) { }

    [Fact]
    public async Task ReturnsViewWithCorrectMessage()
    {
        var rsp = await _client.GetAsync("/palette");
        rsp.EnsureSuccessStatusCode();
        var stringRsp = await rsp.Content.ReadAsStringAsync();

        Assert.Contains("Palette", stringRsp);
    }
}
