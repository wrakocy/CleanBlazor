using Microsoft.AspNetCore.Components.Routing;

namespace Wrak.CleanBlazor.Web.Components.Layouts;

public partial class NavMenu : ComponentBase
{

    private List<MenuItem> _menuItems = new()
    {
        new MenuItem(new Link("Logout", AppConstants.Icons.Logout, NavLinkMatch.Prefix, "/signout"), null, false),
    };

    private record MenuItem(Link Link, List<MenuItem>? SubLinks = null, bool AddDivider = false);

    private record Link(string Name, string Icon, NavLinkMatch Match = NavLinkMatch.All, string Url = "");
}
