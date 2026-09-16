namespace Wrak.CleanBlazor.Web.Interfaces;

public interface IBreakpointService : IBrowserViewportObserver, IAsyncDisposable
{
    Breakpoint Current { get; }
    event Action<Breakpoint>? BreakpointChanging;
}
