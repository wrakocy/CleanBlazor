namespace Wrak.CleanArchitecture.Web.Components.Widgets;

public partial class JustifiedFlexGroup
{
    [Parameter] public Align Align { get; set; }
    [Parameter] public string? Class { get; set; }
    [Parameter] public string? Style { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }

    private string _class => $"d-flex flex-row gap-4 w-100 {AlignClass} {Class}";
    private string _style => $"background-color:transparent; {Style}";

    private string AlignClass => Align switch
    {
        Align.End or Align.Right => "justify-end",
        Align.Center => "justify-center",
        Align.Justify => "justify-between",
        _ => "justify-start" // default: left
    };
}
