using System.Linq.Expressions;

namespace Wrak.CleanArchitecture.Web.Components.Forms;

public partial class NullableDatePicker
{
    [Parameter] public string Label { get; set; } = string.Empty;
    [Parameter] public DateTime? Value { get; set; }
    [Parameter] public DateTime? MinDate { get; set; } = DateTime.MinValue;
    [Parameter] public DateTime? MaxDate { get; set; } = DateTime.MaxValue;
    [Parameter] public EventCallback<DateTime?> ValueChanged { get; set; }
    [Parameter] public Expression<Func<DateTime?>> For { get; set; } = default!;
    [Parameter] public bool Disabled { get; set; } = false;

    private Task OnValueChanged(DateTime? value)
    {
        Value = value;
        return ValueChanged.InvokeAsync(value);
    }
}
