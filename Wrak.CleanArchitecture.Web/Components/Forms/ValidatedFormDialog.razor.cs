namespace Wrak.CleanArchitecture.Web.Components.Forms;

public partial class ValidatedFormDialog<TModel> where TModel : class
{
    [CascadingParameter] protected IMudDialogInstance _dialogInstance { get; set; } = default!;

    [Parameter] public TModel Model { get; set; } = default!;
    [Parameter] public string? Class { get; set; }
    [Parameter] public string? Style { get; set; }
    [Parameter] public string SubmitText { get; set; } = "Apply";
    [Parameter] public string CancelText { get; set; } = "Cancel";
    [Parameter] public string SubmitIcon { get; set; } = AppConstants.Icons.OK;
    [Parameter] public Variant SubmitVariant { get; set; } = Variant.Outlined;
    [Parameter] public bool SubmitDisabled { get; set; } = false;
    [Parameter] public RenderFragment? ChildContent { get; set; }

    protected override void OnParametersSet()
    {
        if (Model is null)
            throw new ArgumentNullException(nameof(Model), $"{nameof(Model)} must be provided.");
    }

    protected void Cancel() => _dialogInstance.Close(DialogResult.Cancel());

    protected void Submit() => _dialogInstance.Close(DialogResult.Ok(Model));
}
