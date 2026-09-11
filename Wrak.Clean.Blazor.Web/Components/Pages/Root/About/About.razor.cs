using Wrak.Clean.Blazor.Core.Shared.Interfaces;

namespace Wrak.Clean.Blazor.Web.Components.Pages.Root.About;

public partial class About
{
    [Inject] private IAppIdentity _appId { get; set; } = default!;
    [Inject] private IAppEnvironment _appEnv { get; set; } = default!;
}
