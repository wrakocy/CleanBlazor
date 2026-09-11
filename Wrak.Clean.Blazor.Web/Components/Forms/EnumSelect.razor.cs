using System.Linq.Expressions;

namespace Wrak.Clean.Blazor.Web.Components.Forms;

public partial class EnumSelect<TEnum> where TEnum : struct, Enum
{
    [Parameter] public string Label { get; set; } = string.Empty;
    [Parameter] public TEnum Value { get; set; }
    [Parameter] public EventCallback<TEnum> ValueChanged { get; set; }
    [Parameter] public Expression<Func<TEnum>> For { get; set; } = default!;
    [Parameter] public RenderFragment<TEnum>? ItemTemplate { get; set; }
    [Parameter] public Func<TEnum, string?>? ItemClassFunc { get; set; }
    [Parameter] public Func<TEnum, string?>? ItemStyleFunc { get; set; }
    [Parameter] public bool Disabled { get; set; } = false;
    [Parameter] public bool UseShortNames { get; set; } = false;

    protected async Task OnValueChanged(TEnum value)
    {
        Value = value;
        await ValueChanged.InvokeAsync(value);
    }
}
