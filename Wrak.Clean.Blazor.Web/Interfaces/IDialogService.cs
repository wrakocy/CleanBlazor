namespace Wrak.Clean.Blazor.Web.Interfaces;

public interface IDialogService
{
    Task<DialogResult> Show<T>(MaxWidth maximumWidth = MaxWidth.Large) where T : IComponent;
    Task<DialogResult> Show<T>(string? title, MaxWidth maximumWidth = MaxWidth.Large) where T : IComponent;
    Task<DialogResult> Show<T>(string? title, DialogParameters parameters, MaxWidth maximumWidth = MaxWidth.Medium, bool closeOnBackdropClick = false) where T : IComponent;
    Task<bool?> ShowMessageBox(string title, string msg, string yesText, string cancelText);
    Task<bool?> ShowUnsavedChangesMessageBox();
    Task<bool?> ShowConfirmDeleteMessageBox();
}
