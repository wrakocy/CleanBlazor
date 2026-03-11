namespace Wrak.CleanArchitecture.Web.Interfaces;

public interface IBreakpointService : IBrowserViewportObserver, IAsyncDisposable
{
    Breakpoint Current { get; }
    event Action<Breakpoint>? BreakpointChanging;
}
