namespace Wrak.CleanBlazor.Web.Components.Pages.Root.Index;

public class IndexBase : AppComponentBase
{
    [Parameter, SupplyParameterFromQuery] public bool? Reloaded { get; set; }
}
