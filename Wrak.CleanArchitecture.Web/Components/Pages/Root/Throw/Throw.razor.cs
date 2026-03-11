using Wrak.RandomData;

namespace Wrak.CleanArchitecture.Web.Components.Pages.Root.Throw;

public partial class Throw
{
    protected override void OnInitialized()
    {
        var statusCode = RandomData.RandomData.As.Int(400, 500); // Http status codes 400 through 500.
        throw new ApplicationException("Test application exception.");
    }
}
