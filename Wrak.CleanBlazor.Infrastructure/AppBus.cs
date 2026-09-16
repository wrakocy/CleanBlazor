using Wrak.CleanBlazor.Core.Shared.Events;
using Wrak.CleanBlazor.Core.Shared.Features;
using Wrak.CleanBlazor.Core.Shared.Interfaces;

namespace Wrak.CleanBlazor.Infrastructure;

public class AppBus : IAppBus
{
    private readonly IMediator _mediator;

    public AppBus(IMediator mediator) => _mediator = mediator;

    public async Task Publish(ApplicationEventBase appEvent, CancellationToken cancellationToken = default)
    {
        await _mediator.Publish(appEvent, cancellationToken);
    }

    public async Task Send(CommandBase command, CancellationToken cancellationToken = default)
    {
        await _mediator.Send(command, cancellationToken);
    }

    public async Task<TResponse> Send<TResponse>(CommandBase<TResponse> command, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    public async Task<TResponse> Send<TResponse>(QueryBase<TResponse> query, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(query, cancellationToken);
    }
}
