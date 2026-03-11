using Wrak.CleanArchitecture.Core.Shared.Interfaces;

namespace Wrak.CleanArchitecture.Web.Interfaces;

public interface IUserContextService
{
    Task<IUserContext> GetUserContextAsync();
}
