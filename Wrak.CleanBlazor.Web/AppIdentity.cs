using Wrak.CleanBlazor.Core.Shared.Interfaces;

namespace Wrak.CleanBlazor.Web;

public class AppIdentity : IAppIdentity
{
    public string ClientId => "app-v1";
    public string Name => "App";
}
