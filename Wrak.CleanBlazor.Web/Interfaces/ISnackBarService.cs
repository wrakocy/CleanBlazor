namespace Wrak.CleanBlazor.Web.Interfaces;

public interface ISnackBarService
{
    void Add(string message, Severity severity = Severity.Normal, int durationSeconds = 5);
    void Add(RenderFragment renderFragment, Severity severity = Severity.Normal, int durationSeconds = 5);
    void Clear();
}
