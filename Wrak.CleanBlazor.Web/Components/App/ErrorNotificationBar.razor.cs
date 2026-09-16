using Wrak.CleanBlazor.Core.Shared.Interfaces;

namespace Wrak.CleanBlazor.Web.Components.App;

public partial class ErrorNotificationBar
{
    [Inject] private IAppEnvironment _appEnv { get; set; } = default!;
}
