namespace Wrak.Clean.Blazor.Web.Components.Widgets;

public partial class ButtonTooltip
{
    [Parameter] public string Text { get; set; } = string.Empty;
    [Parameter] public RenderFragment? ChildContent { get; set; }
}
