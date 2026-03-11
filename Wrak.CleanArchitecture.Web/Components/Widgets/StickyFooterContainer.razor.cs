namespace Wrak.CleanArchitecture.Web.Components.Widgets;

public partial class StickyFooterContainer
{
    [Parameter] public RenderFragment? Content { get; set; }
    [Parameter] public RenderFragment? Footer { get; set; }
    [Parameter] public string? ContentStyle { get; set; }
    [Parameter] public string? FooterStyle { get; set; }
}
