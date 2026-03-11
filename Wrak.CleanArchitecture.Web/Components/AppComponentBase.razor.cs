using Wrak.CleanArchitecture.Core.Shared.Interfaces;
using Wrak.CleanArchitecture.Web.Interfaces;

namespace Wrak.CleanArchitecture.Web.Components;

public partial class AppComponentBase : IDisposable
{
    [Inject] protected NavigationManager _navManager { get; set; } = default!;
    [Inject] protected IUserContext _userContext { get; set; } = default!;
    [Inject] protected IAppState _appState { get; set; } = default!;
    [Inject] protected IAppBus _appBus { get; set; } = default!;
    [Inject] protected Interfaces.IDialogService _dialogService { get; set; } = default!;
    [Inject] protected ISnackBarService _snackBarService { get; set; } = default!;

    private object? _modelRef;

    protected bool ReferenceHasChanged(object dataParam)
    {
        // HACH-ish: used by descendants to prevent uneccessary work 
        // in OnParametersSet when parameters are reset frequently
        var changed = _modelRef != dataParam;
        _modelRef = dataParam;
        return changed;
    }

    protected override void OnInitialized()
    {
        _appState.WorkingChanged += StateHasChanged;
    }

    protected async Task ForceComponentRefreshAsync()
    {
        // HACK-ish: In certain situations, it is
        // necessary for invoke state has changed
        // asynchronously and pause briefly to
        // ensure that the UI updates in a timely manner
        await InvokeAsync(StateHasChanged);
        await Task.Delay(5);
    }

    public void Dispose()
    {
        _modelRef = null;
        _appState.WorkingChanged -= StateHasChanged;
        GC.SuppressFinalize(this);
    }
}
