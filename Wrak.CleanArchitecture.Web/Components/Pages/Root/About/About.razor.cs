using Wrak.CleanArchitecture.Core.Shared.Interfaces;

namespace Wrak.CleanArchitecture.Web.Components.Pages.Root.About;

public partial class About
{
    [Inject] private IAppIdentity _appId { get; set; } = default!;
    [Inject] private IAppEnvironment _appEnv { get; set; } = default!;
}
