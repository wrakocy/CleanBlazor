namespace Wrak.CleanBlazor.FunctionalTests.Components.Pages.Root.Throw;

public class Throw : WebApplicationTestFixtureBase
{
    public Throw(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper) : base(factory, outputHelper) { }

    [Fact]
    public async Task ReturnsViewWithCorrectMessage()
    {
        var rsp = await _client.GetAsync("/throw", TestContext.Current.CancellationToken);
        rsp.EnsureSuccessStatusCode();
        var stringRsp = await rsp.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Contains("Error", stringRsp); // Should render the error page after throwing.
    }
}
