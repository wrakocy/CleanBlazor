using System.Linq.Expressions;

namespace Wrak.Clean.Blazor.Web.Components.Forms;

public partial class EnumMultiSelect<TEnum> where TEnum : struct, Enum
{
    [Parameter] public string Label { get; set; } = string.Empty;
    [Parameter] public string SelectAllText { get; set; } = "Select all";
    [Parameter] public HashSet<TEnum> SelectedValues { get; set; } = default!;
    [Parameter] public EventCallback<HashSet<TEnum>> SelectedValuesChanged { get; set; }
    [Parameter] public Expression<Func<TEnum>> For { get; set; } = default!;
    [Parameter] public RenderFragment<TEnum>? ItemTemplate { get; set; }
    [Parameter] public Func<TEnum, string?>? ItemClassFunc { get; set; }
    [Parameter] public Func<TEnum, string?>? ItemStyleFunc { get; set; }
    [Parameter] public bool Disabled { get; set; } = false;
    [Parameter] public bool Clearable { get; set; } = false;
    [Parameter] public bool UseShortNames { get; set; } = false;

    [CascadingParameter] private MudForm? _form { get; set; }

    protected override void OnParametersSet()
    {
        if (SelectedValues is null)
            throw new ArgumentNullException(nameof(SelectedValues));
    }

    private async Task OnSelectedValuesChanged(IEnumerable<TEnum> values)
    {
        // IMPORTANT: An odd quirk of MudSelect's multi-select behavior
        // requires us to rehydrate SelectedValues to a HashSet. The component
        // will not respond to changes in the underlying collection without this.
        SelectedValues = values.ToHashSet();
        await SelectedValuesChanged.InvokeAsync(SelectedValues);

        // IMPORTANT: By default, MudSelect does not validate
        // immediately on blur or close the way
        // MudTextField does. To mimic that behavior,
        // we explicitly validate the entire form when the
        // selected values change.
        if (_form != null) await _form.ValidateAsync();
    }
}
