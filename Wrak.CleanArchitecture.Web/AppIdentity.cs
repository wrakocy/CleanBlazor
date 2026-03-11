using Wrak.CleanArchitecture.Core.Shared.Interfaces;

namespace Wrak.CleanArchitecture.Web;

public class AppIdentity : IAppIdentity
{
    public string ClientId => "app-v1";
    public string Name => "App";
}
