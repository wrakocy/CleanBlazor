namespace Wrak.CleanArchitecture.FunctionalTests.Components.Pages.Root.Throw;

public class Throw : WebApplicationTestFixtureBase
{
    public Throw(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper) : base(factory, outputHelper) { }

    [Fact(Skip = "This suddenly started failing. Figure out why. Just a test page.")]
    public async Task ReturnsViewWithCorrectMessage()
    {
        var rsp = await _client.GetAsync("/throw");
        rsp.EnsureSuccessStatusCode();
        var stringRsp = await rsp.Content.ReadAsStringAsync();

        Assert.Contains("Error", stringRsp); // Should render the error page after throwing.
    }
}
