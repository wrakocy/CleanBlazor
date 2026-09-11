namespace Wrak.Clean.Blazor.Core.Shared.Events;

public abstract class ApplicationEventBase : INotification
{
    public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
}
