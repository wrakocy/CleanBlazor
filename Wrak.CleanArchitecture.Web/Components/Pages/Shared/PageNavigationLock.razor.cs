using Microsoft.AspNetCore.Components.Routing;

namespace Wrak.CleanArchitecture.Web.Components.Pages.Shared;

public partial class PageNavigationLock
{
    [Parameter] public EventCallback<LocationChangingContext> OnBeforeNavigation { get; set; }
}
