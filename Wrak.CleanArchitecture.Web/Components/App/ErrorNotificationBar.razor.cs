using Wrak.CleanArchitecture.Core.Shared.Interfaces;

namespace Wrak.CleanArchitecture.Web.Components.App;

public partial class ErrorNotificationBar
{
    [Inject] private IAppEnvironment _appEnv { get; set; } = default!;
}
