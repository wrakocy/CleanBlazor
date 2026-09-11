namespace Wrak.Clean.Blazor.Web.Interfaces;

public interface IBreakpointService : IBrowserViewportObserver, IAsyncDisposable
{
    Breakpoint Current { get; }
    event Action<Breakpoint>? BreakpointChanging;
}
