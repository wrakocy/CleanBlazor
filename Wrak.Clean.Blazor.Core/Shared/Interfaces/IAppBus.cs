using Wrak.Clean.Blazor.Core.Shared.Events;
using Wrak.Clean.Blazor.Core.Shared.Features;

namespace Wrak.Clean.Blazor.Core.Shared.Interfaces;

public interface IAppBus
{
    Task Publish(ApplicationEventBase appEvent, CancellationToken cancellationToken = default);
    Task Send(CommandBase command, CancellationToken cancellationToken = default);
    Task<TResponse> Send<TResponse>(CommandBase<TResponse> command, CancellationToken cancellationToken = default);
    Task<TResponse> Send<TResponse>(QueryBase<TResponse> query, CancellationToken cancellationToken = default);
}
