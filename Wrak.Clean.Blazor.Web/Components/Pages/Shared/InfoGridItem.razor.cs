namespace Wrak.Clean.Blazor.Web.Components.Pages.Shared;

public partial class InfoGridItem
{
    [Parameter] public string Label { get; set; } = default!;
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public string? ContentStyle { get; set; }
    [Parameter] public int xs { get; set; } = 6;
    [Parameter] public int sm { get; set; } = 3;
    [Parameter] public int md { get; set; } = 2;
    [Parameter] public int lg { get; set; } = 2;
    [Parameter] public int xl { get; set; } = 2;
}
