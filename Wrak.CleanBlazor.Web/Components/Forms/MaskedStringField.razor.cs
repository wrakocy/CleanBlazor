namespace Wrak.CleanBlazor.Web.Components.Forms;

public class MaskedStringFieldBase : MaskedFieldBase<string>
{
    [Parameter] public string Mask { get; set; } = string.Empty;

    protected override string MaskPattern => Mask;
}
