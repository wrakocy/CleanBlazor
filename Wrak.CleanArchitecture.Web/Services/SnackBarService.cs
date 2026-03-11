using Wrak.CleanArchitecture.Web.Interfaces;

namespace Wrak.CleanArchitecture.Web.Services;

public class SnackBarService(ISnackbar snackbar) : ISnackBarService
{
    private readonly ISnackbar _snackbar = snackbar;

    public void Add(string message, Severity severity = Severity.Normal, int durationSeconds = 5)
    {
        void options(SnackbarOptions c)
        {
            c.VisibleStateDuration = durationSeconds * 1000;
            c.CloseAfterNavigation = true;
            c.SnackbarTypeClass = "pa-3 mud-typography-h6";
        }

        _snackbar.Add(message, severity, options);
    }

    public void Add(RenderFragment renderFragment, Severity severity = Severity.Normal, int durationSeconds = 5)
    {
        void options(SnackbarOptions c)
        {
            c.VisibleStateDuration = durationSeconds * 1000;
            c.CloseAfterNavigation = true;
        }

        _snackbar.Add(renderFragment, severity, options);
    }

    public void Clear() => _snackbar.Clear();
}
