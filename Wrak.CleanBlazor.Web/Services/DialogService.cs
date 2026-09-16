namespace Wrak.CleanBlazor.Web.Services;

public class DialogService : Interfaces.IDialogService
{
    private readonly MudBlazor.IDialogService _dialogSvc;
    private readonly IBrowserViewportService _viewportSvc;

    public DialogService(MudBlazor.IDialogService dialogSvc, IBrowserViewportService viewportSvc)
    {
        _dialogSvc = dialogSvc.ThrowIfNull().Value;
        _viewportSvc = viewportSvc.ThrowIfNull().Value;
    }

    public async Task<DialogResult> Show<T>(MaxWidth maximumWidth = MaxWidth.Medium) where T : IComponent
    {
        var options = await BuildDialogOptions(null, false, maximumWidth);
        var dialogRef = await _dialogSvc.ShowAsync<T>();
        var result = await dialogRef.Result;

        return result ?? DialogResult.Cancel();
    }

    public async Task<DialogResult> Show<T>(string? title, MaxWidth maximumWidth = MaxWidth.Medium) where T : IComponent
    {
        var options = await BuildDialogOptions(title, false, maximumWidth);
        var dialogRef = await _dialogSvc.ShowAsync<T>(title);
        var result = await dialogRef.Result;

        return result ?? DialogResult.Cancel();
    }

    public async Task<DialogResult> Show<T>(string? title, DialogParameters parameters, MaxWidth maximumWidth = MaxWidth.Medium, bool closeOnBackdropClick = false) where T : IComponent
    {
        var options = await BuildDialogOptions(title, closeOnBackdropClick, maximumWidth);
        var dialogRef = await _dialogSvc.ShowAsync<T>(title, parameters, options);
        var result = await dialogRef.Result;

        return result ?? DialogResult.Cancel();
    }

    public async Task<bool?> ShowMessageBox(string title, string msg, string yesText, string cancelText)
    {
        var options = await BuildDialogOptions(title, false);
        return await _dialogSvc.ShowMessageBoxAsync(title, msg, yesText, null, cancelText, options);
    }

    public async Task<bool?> ShowUnsavedChangesMessageBox()
    {
        return await ShowMessageBox("Warning!", "You have unsaved changes. Are you sure you want to leave this page?", "Yes, Leave!", "No, Stay");
    }

    public async Task<bool?> ShowConfirmDeleteMessageBox()
    {
        return await ShowMessageBox("Warning!", "Deleted records cannot be recovered. Are you sure you want to do this?", "Yes, Delete!", "No, Cancel");
    }

    private async Task<DialogOptions> BuildDialogOptions(string? title, bool closeOnBackdropClick, MaxWidth maximumWidth = MaxWidth.Large)
    {
        var bp = await _viewportSvc.GetCurrentBreakpointAsync();
        var fullScreen = bp <= Breakpoint.Sm;
        var maxWidth = fullScreen ? MaxWidth.ExtraExtraLarge : maximumWidth;

        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            BackdropClick = closeOnBackdropClick,
            NoHeader = string.IsNullOrEmpty(title), // Only show header if title is not null
            CloseButton = false,
            FullScreen = fullScreen,
            FullWidth = fullScreen,
            MaxWidth = maxWidth,
            Position = DialogPosition.Center
        };

        return options;
    }
}
