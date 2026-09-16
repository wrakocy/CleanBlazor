using Wrak.CleanBlazor.Core.Shared.Interfaces;

namespace Wrak.CleanBlazor.Web.Interfaces;

public interface IUserContextService
{
    Task<IUserContext> GetUserContextAsync();
}
