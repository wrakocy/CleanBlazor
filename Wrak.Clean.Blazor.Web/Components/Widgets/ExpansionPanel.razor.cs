namespace Wrak.Clean.Blazor.Web.Components.Widgets;

public class ExpansionPanelBase : ComponentBase
{
    [Parameter] public string? Title { get; set; }
    [Parameter] public string? Icon { get; set; }
    [Parameter] public bool Expanded { get; set; }
    [Parameter] public EventCallback<bool> ExpandedChanged { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
}
