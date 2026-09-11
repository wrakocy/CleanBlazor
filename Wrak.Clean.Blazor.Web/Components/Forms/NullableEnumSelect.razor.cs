using System.Linq.Expressions;

namespace Wrak.Clean.Blazor.Web.Components.Forms;

public partial class NullableEnumSelect<TEnum> where TEnum : struct, Enum
{
    [Parameter] public string Label { get; set; } = string.Empty;
    [Parameter] public string NullText { get; set; } = "".ToStringOrEmpty();
    [Parameter] public TEnum? Value { get; set; }
    [Parameter] public EventCallback<TEnum?> ValueChanged { get; set; }
    [Parameter] public Expression<Func<TEnum?>> For { get; set; } = default!;
    [Parameter] public RenderFragment<TEnum>? ItemTemplate { get; set; }
    [Parameter] public Func<TEnum?, string?>? ItemClassFunc { get; set; }
    [Parameter] public Func<TEnum?, string?>? ItemStyleFunc { get; set; }
    [Parameter] public bool Disabled { get; set; } = false;
    [Parameter] public bool UseShortNames { get; set; } = false;
    [Parameter] public bool Clearable { get; set; } = true;

    protected async Task OnValueChanged(TEnum? value)
    {
        Value = value;
        await ValueChanged.InvokeAsync(value);
    }

    protected static IEnumerable<TEnum> GetOrderedEnumValues()
    {
        return Enum.GetValues<TEnum>()
            .OrderBy(e =>
            {
                var member = typeof(TEnum).GetMember(e.ToString()).FirstOrDefault();
                var displayAttr = member?.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.DisplayAttribute), false)
                    .Cast<System.ComponentModel.DataAnnotations.DisplayAttribute>()
                    .FirstOrDefault();
                return displayAttr?.GetOrder() ?? int.MaxValue;
            })
            .ThenBy(e => Convert.ToInt64(e));
    }
}
