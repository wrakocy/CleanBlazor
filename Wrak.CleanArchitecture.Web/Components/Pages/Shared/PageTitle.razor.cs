using Wrak.CleanArchitecture.Core.Shared.Interfaces;

namespace Wrak.CleanArchitecture.Web.Components.Pages.Shared;

public partial class PageTitle
{
    [Parameter] public RenderFragment? ChildContent { get; set; }

    [Inject] private IAppIdentity _appId { get; set; } = default!;
    [Inject] private IAppEnvironment _appEnv { get; set; } = default!;
}
