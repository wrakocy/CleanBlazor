using System.Diagnostics;
using FluentValidation;
using Wrak.Clean.Blazor.Core.Shared.Validators;

namespace Wrak.Clean.Blazor.Web.Components.Forms;

public partial class ValidatedForm<TModel> where TModel : class
{
    [Parameter] public TModel Model { get; set; } = null!;
    [Parameter] public string? Class { get; set; }
    [Parameter] public string? Style { get; set; }
    [Parameter] public string SubmitText { get; set; } = "Apply";
    [Parameter] public string CancelText { get; set; } = "Cancel";
    [Parameter] public string SubmitIcon { get; set; } = AppConstants.Icons.OK;
    [Parameter] public Variant SubmitVariant { get; set; } = Variant.Outlined;
    [Parameter] public bool SuppressImplicitSubmission { get; set; } = false;
    [Parameter] public bool SubmitDisabled { get; set; } = false;
    [Parameter] public EventCallback Submitted { get; set; }
    [Parameter] public EventCallback Cancelled { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }

    [Inject] private IServiceProvider _sp { get; set; } = default!;

    private MudForm _form = default!;
    private IValidateModel<TModel> _validator = default!;

    protected override void OnParametersSet()
    {
        EnsureModelNotNull();
        EnsureValidatorNotNull();
    }

    private async Task Cancel() => await Cancelled.InvokeAsync();

    protected async Task Submit()
    {
        await _form.ValidateAsync();

        if (_form.IsValid)
            await Submitted.InvokeAsync();
        else
        {
            // TODO: if possible, set focus to the first invalid field
            // Not sure how to do this with MudBlazor yet. Might
            // need to use JavaScript interop.

            // TODO: log validation errors to app insights?
            return;
        }
    }

    private void EnsureModelNotNull()
    {
        if (Model is null)
            throw new ArgumentNullException(nameof(Model));
    }

    private void EnsureValidatorNotNull()
    {
        Debug.WriteLine($"TModel = {typeof(TModel).AssemblyQualifiedName}");

        if (_sp.GetService<IValidator<TModel>>() is not IValidateModel<TModel> validator)
        {
            throw new ArgumentException($"No validator found for " +
                        $"model type: {Model.GetType().FullName}");
        }

        _validator = validator;
    }
}
