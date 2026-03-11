namespace Wrak.CleanArchitecture.Web.Components.Pages.Shared;

public partial class FormGridItem
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public string? ContentStyle { get; set; }
    [Parameter] public int xs { get; set; } = 12;
    [Parameter] public int sm { get; set; } = 6;
    [Parameter] public int md { get; set; } = 4;
    [Parameter] public int lg { get; set; } = 3;
    [Parameter] public int xl { get; set; } = 3;
}
