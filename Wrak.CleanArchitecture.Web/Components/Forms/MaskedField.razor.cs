using System.Linq.Expressions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Wrak.CleanArchitecture.Web.Interfaces;

namespace Wrak.CleanArchitecture.Web.Components.Forms;

public abstract class MaskedFieldBase<T> : ComponentBase, IDisposable
{
    [Inject] protected IJSInteropService _jsService { get; set; } = default!;

    [Parameter] public string Label { get; set; } = string.Empty;
    [Parameter] public T Value { get; set; } = default!;
    [Parameter] public string? HelperText { get; set; }
    [Parameter] public string? Placeholder { get; set; }
    [Parameter] public Adornment Adornment { get; set; } = Adornment.None;
    [Parameter] public string? AdornmentIcon { get; set; }
    [Parameter] public Size IconSize { get; set; } = Size.Medium;
    [Parameter] public EventCallback<T> ValueChanged { get; set; }
    [Parameter] public Expression<Func<T>> For { get; set; } = default!;
    [Parameter] public bool Clearable { get; set; } = false;
    [Parameter] public bool Disabled { get; set; } = false;

    [CascadingParameter] private MudForm? _form { get; set; }

    protected readonly string _inputId = $"input-{Guid.NewGuid()}";
    protected Dictionary<string, object> _userAttributes = default!;
    protected DotNetObjectReference<MaskedFieldBase<T>> _dotNetRef = null!;

    protected override void OnInitialized()
    {
        EnsureSupportedType();

        _userAttributes = new()
        {
            { "id", _inputId },
            { "autocomplete", "off" },
            { "onfocus", EventCallback.Factory.Create<FocusEventArgs>(this, OnFocus) }
        };

        Initializing();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _dotNetRef = DotNetObjectReference.Create(this);
            await ApplyMask();
        }
    }

    protected async Task OnFocus(FocusEventArgs _)
    {
        if (_inputId is not null)
            await _jsService.SyncMaskForElementAndMoveCaret(_inputId);
    }

    protected virtual async Task OnValueUpdated(T newValue)
    {
        await ValueChanged.InvokeAsync(newValue);
    }

    protected async Task OnValueCleared()
    {
        await UpdateValue(string.Empty);
    }

    protected virtual void Initializing() { }

    protected virtual string? MaskPattern => null;

    protected virtual async Task ApplyMask()
    {
        var mask = MaskPattern ?? AppConstants.MaskPatterns.Wildcard;
        await _jsService.ApplyMaskToElement(_inputId, mask, _dotNetRef);
    }

    protected virtual T ConvertMaskedValue(string value)
    {
        return (T)Convert.ChangeType(value, typeof(T))!;
    }

    [JSInvokable]
    public async Task UpdateValue(string maskedValue)
    {
        Value = ConvertMaskedValue(maskedValue);
        await OnValueUpdated(Value);
        if (_form != null) await _form.ValidateAsync();

        StateHasChanged();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _dotNetRef?.Dispose();
    }

    private static void EnsureSupportedType()
    {
        var t = typeof(T);

        if (t != typeof(int) &&
            t != typeof(int?) &&
            t != typeof(long) &&
            t != typeof(long?) &&
            t != typeof(decimal) &&
            t != typeof(decimal?) &&
            t != typeof(string))
        {
            throw new NotSupportedException(
                $"Type '{t.FullName}' is not a supported masked types. " +
                "Supported types are: int, long, decimal and string.");
        }
    }
}
