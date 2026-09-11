using Wrak.Clean.Blazor.Core.Shared.Interfaces;

namespace Wrak.Clean.Blazor.Web.Interfaces;

public interface IUserContextService
{
    Task<IUserContext> GetUserContextAsync();
}
