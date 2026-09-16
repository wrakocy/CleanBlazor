namespace Wrak.CleanBlazor.Web.Interfaces;

public interface IAppState
{
    bool Working { get; set; }
    event Action? WorkingChanged;
}
