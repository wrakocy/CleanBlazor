using Wrak.Clean.Blazor.Core.Shared.Interfaces;

namespace Wrak.Clean.Blazor.Web.Components.Pages.Shared;

public partial class PageTitle
{
    [Parameter] public RenderFragment? ChildContent { get; set; }

    [Inject] private IAppIdentity _appId { get; set; } = default!;
    [Inject] private IAppEnvironment _appEnv { get; set; } = default!;
}
