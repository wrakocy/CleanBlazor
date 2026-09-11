namespace Wrak.Clean.Blazor.Web.Interfaces;

public interface IAppState
{
    bool Working { get; set; }
    event Action? WorkingChanged;
}
