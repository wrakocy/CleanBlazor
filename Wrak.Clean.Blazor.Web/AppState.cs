using Wrak.Clean.Blazor.Web.Interfaces;

namespace Wrak.Clean.Blazor.Web;

public class AppState : IAppState
{
    private bool _working;

    public bool Working
    {
        get => _working;
        set
        {
            if (_working != value)
            {
                _working = value;
                OnWorkingChanged();
            }
        }
    }

    public event Action? WorkingChanged;

    private void OnWorkingChanged() => WorkingChanged?.Invoke();
}
