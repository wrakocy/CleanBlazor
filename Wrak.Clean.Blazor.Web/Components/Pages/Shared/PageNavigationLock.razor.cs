using Microsoft.AspNetCore.Components.Routing;

namespace Wrak.Clean.Blazor.Web.Components.Pages.Shared;

public partial class PageNavigationLock
{
    [Parameter] public EventCallback<LocationChangingContext> OnBeforeNavigation { get; set; }
}
