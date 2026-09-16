namespace Wrak.Clean.Blazor.FunctionalTests.Components.Pages.Root.Palette;

public class Palette : WebApplicationTestFixtureBase
{
    public Palette(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper) : base(factory, outputHelper) { }

    [Fact]
    public async Task ReturnsViewWithCorrectMessage()
    {
        var rsp = await _client.GetAsync("/palette", TestContext.Current.CancellationToken);
        rsp.EnsureSuccessStatusCode();
        var stringRsp = await rsp.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Contains("Palette", stringRsp);
    }
}
