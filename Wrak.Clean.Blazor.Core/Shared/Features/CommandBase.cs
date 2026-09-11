namespace Wrak.Clean.Blazor.Core.Shared.Features;

public abstract class CommandBase : IRequest
{
}

public abstract class CommandBase<TResponse> : IRequest<TResponse>
{
}

