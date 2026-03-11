namespace Wrak.CleanArchitecture.Web.Components.Pages.Root.Index;

public partial class IndexCard
{
    [Inject] public NavigationManager NavManager { get; set; } = default!;

    [Parameter] public string? Title { get; set; }
    [Parameter] public string? HeaderIcon { get; set; }
    [Parameter] public string Url { get; set; } = "/";
    [Parameter] public RenderFragment? ChildContent { get; set; }
}
