namespace Wrak.CleanBlazor.Core.Shared.Features;

public abstract class CommandBase : IRequest
{
}

public abstract class CommandBase<TResponse> : IRequest<TResponse>
{
}

