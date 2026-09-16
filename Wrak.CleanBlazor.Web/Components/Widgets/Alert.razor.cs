namespace Wrak.CleanBlazor.Web.Components.Widgets;

public partial class Alert
{
    [Parameter] public RenderFragment ChildContent { get; set; } = default!;
    [Parameter] public Severity Severity { get; set; } = Severity.Normal;
    [Parameter] public HorizontalAlignment ContentAlignment { get; set; } = HorizontalAlignment.Left;
    [Parameter] public bool ShowCloseIcon { get; set; } = false;
    [Parameter] public bool NoIcon { get; set; } = false;
    [Parameter] public bool Visible { get; set; }
    [Parameter] public string Class { get; set; } = "mb-5";
}
