namespace Wrak.CleanArchitecture.Core.Shared.Events;

public abstract class ApplicationEventBase : INotification
{
    public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
}
