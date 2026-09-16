using System.Linq.Expressions;

namespace Wrak.CleanBlazor.Web.Components.Forms;

public partial class CurrencyField
{
    [Parameter] public string Label { get; set; } = string.Empty;
    [Parameter] public decimal? Value { get; set; }
    [Parameter] public EventCallback<decimal?> ValueChanged { get; set; }
    [Parameter] public Expression<Func<decimal?>> For { get; set; } = default!;
    [Parameter] public decimal Min { get; set; } = 0.0m;
    [Parameter] public decimal Max { get; set; } = 10.0m;
    [Parameter] public decimal Step { get; set; } = 0.25m;
    [Parameter] public string? HelperText { get; set; }
    [Parameter] public bool Disabled { get; set; } = false;

    private Task OnValueChanged(decimal? value)
    {
        Value = value;
        return ValueChanged.InvokeAsync(value);
    }
}
