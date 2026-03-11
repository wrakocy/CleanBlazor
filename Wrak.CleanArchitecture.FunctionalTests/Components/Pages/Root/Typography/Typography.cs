namespace Wrak.CleanArchitecture.FunctionalTests.Components.Pages.Root.Typography;

public class Typography : WebApplicationTestFixtureBase
{
    public Typography(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper) : base(factory, outputHelper) { }

    [Fact]
    public async Task ReturnsViewWithCorrectMessage()
    {
        var rsp = await _client.GetAsync("/typography");
        rsp.EnsureSuccessStatusCode();
        var stringRsp = await rsp.Content.ReadAsStringAsync();

        Assert.Contains("Typography", stringRsp);
    }
}
