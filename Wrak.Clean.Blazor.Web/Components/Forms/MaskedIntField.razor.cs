namespace Wrak.Clean.Blazor.Web.Components.Forms;

public class MaskedIntFieldBase : MaskedFieldBase<int?>
{
    [Parameter] public object? Validation { get; set; }
    [Parameter] public int MaxLength { get; set; } = 6;

    protected override string MaskPattern => new('0', MaxLength);

    private const int MaxLengthOfInt = 10; // int.MaxValue = 2147483647, which has 10 digits

    protected override void OnParametersSet()
    {
        if (MaxLengthOfInt > 10)
            throw new ArgumentOutOfRangeException(nameof(MaxLength),
                "MaxLength cannot exceed 10");
    }

    protected override int? ConvertMaskedValue(string value)
    {
        if (string.IsNullOrEmpty(value))
            return null;
        return int.Parse(value);
    }
}
