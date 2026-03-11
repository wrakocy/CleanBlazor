namespace Wrak.CleanArchitecture.Web.Components.Pages.Shared;

public partial class PageHeader
{
    [Parameter] public Color Color { get; set; } = Color.Default;
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public List<BreadcrumbItem>? BreadcrumbItems { get; set; }
}
