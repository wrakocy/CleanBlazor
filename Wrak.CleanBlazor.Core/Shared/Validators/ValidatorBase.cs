namespace Wrak.CleanBlazor.Core.Shared.Validators;

public abstract class ValidatorBase<TModel> : AbstractValidator<TModel>, IValidateModel<TModel> where TModel : class
{
    public Func<object, string, Task<IEnumerable<string>>> ValidateModel => async (obj, propertyName) =>
    {
        var model = (TModel)obj;
        var result = await ValidateAsync(model);

        if (result.IsValid) return [];

        return result.Errors
                     .Where(e => e.PropertyName == propertyName)
                     .Select(e => e.ErrorMessage);
    };

    public IEnumerable<string> ValidateProperty(TModel model, string propertyName)
    {
        Log.Debug($"Validating property: {propertyName}");

        var context = ValidationContext<TModel>.CreateWithOptions(model, options =>
        {
            options.IncludeProperties(propertyName);
        });

        var result = Validate(context);
        if (result.IsValid) return [];

        return result.Errors
                     .Where(e => e.PropertyName == propertyName)
                     .Select(e => e.ErrorMessage);
    }
}

public interface IValidateModel<TModel>
{
    Func<object, string, Task<IEnumerable<string>>> ValidateModel { get; }
    IEnumerable<string> ValidateProperty(TModel model, string propertyName);
}
