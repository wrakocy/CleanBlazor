using FluentValidation.Results;

namespace Wrak.CleanBlazor.Web.Components.Pages.Shared;

public partial class ValidationFailuresModal
{
    [Parameter] public List<ValidationFailure> Failures { get; set; } = default!;

    [CascadingParameter] protected IMudDialogInstance _dialogInstance { get; set; } = default!;

    protected override void OnParametersSet()
    {
        if (Failures is null)
            throw new ArgumentNullException(nameof(Failures), $"{nameof(Failures)} must be provided.");
    }

    protected void Done() => _dialogInstance.Close(DialogResult.Cancel());
}
