using Wrak.Clean.Blazor.Core.Shared.Interfaces;

namespace Wrak.Clean.Blazor.Web;

public class AppIdentity : IAppIdentity
{
    public string ClientId => "app-v1";
    public string Name => "App";
}
