using System.Net;

namespace Wrak.CleanBlazor.FunctionalTests.Components.Pages.Root.Error;

public class Error : WebApplicationTestFixtureBase
{
    public Error(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper) : base(factory, outputHelper) { }

    [Fact]
    public async Task DirectURLWithoutStatusParameter()
    {
        var rsp = await _client.GetAsync($"/error", TestContext.Current.CancellationToken);
        var stringRsp = await rsp.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        // Default to 500 when no status code is provided in the query string.
        Assert.Contains($"Error - {(int)HttpStatusCode.InternalServerError}", stringRsp);
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.Forbidden)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task DirectURLWithStatusParameter(HttpStatusCode statusCode)
    {
        var rsp = await _client.GetAsync($"/error?statusCode={(int)statusCode}", TestContext.Current.CancellationToken);
        var stringRsp = await rsp.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Contains($"Error - {(int)statusCode}", stringRsp);
    }

    [Fact]
    public async Task FallbackToErrorPage()
    {

        var rsp = await _client.GetAsync("/fizz-bang", TestContext.Current.CancellationToken); // Invalid URL
        var stringRsp = await rsp.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        // Fallback to error page when requested page not found.
        Assert.Contains($"Error - {(int)HttpStatusCode.NotFound}", stringRsp);
    }
}
