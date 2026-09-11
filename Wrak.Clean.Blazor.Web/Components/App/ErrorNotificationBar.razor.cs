using Wrak.Clean.Blazor.Core.Shared.Interfaces;

namespace Wrak.Clean.Blazor.Web.Components.App;

public partial class ErrorNotificationBar
{
    [Inject] private IAppEnvironment _appEnv { get; set; } = default!;
}
