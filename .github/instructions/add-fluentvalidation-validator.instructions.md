---
description: Add or extend a FluentValidation validator for a Core model. Use when a model needs field-level or cross-field business-rule validation, as opposed to the null-guard checks in command constructors.
applyTo: "*.Core/**/Models/**/*Validator.cs"
---

# Add a FluentValidation Validator

Validators live alongside their model, e.g. `Core/Orders/Models/Order/OrderModelValidator.cs`. See
`docs/03-feature-development-guide.md` §5.

## Steps

1. **Extend `ValidatorBase<TModel>`** (`Shared/Validators/ValidatorBase.cs`), not
   `AbstractValidator<TModel>` directly. If the UI is Blazor + MudBlazor, `ValidatorBase` bridges
   FluentValidation into MudBlazor's per-property validation API via `IModelValidator<TModel>`.
2. **Inject child validators** for nested models via constructor (`IValidator<TChild>`), guard
   with `.ThrowIfNull().Value`, and wire them with `RuleFor(x => x.Child).SetValidator(...)` or
   `RuleForEach(x => x.Children).SetValidator(...)` for collections.
3. **Cross-field/collection rules** use `.Must(...)` with `.WithMessage(...)`, or `.Custom((value,
   ctx) => ctx.AddFailure(...))` for rules that need to report multiple, dynamic error messages
   per item. Use `.Cascade(CascadeMode.Stop)` when a later check assumes an earlier one passed.
4. **Extension-method rules** (e.g. `.MustNotConflict()`, `.MustBeContinuous()`) are custom
   FluentValidation extensions defined in `Shared/Extensions/ValidationExtensions.cs` — check
   there before writing a new inline `.Must(...)` that duplicates one.
5. **No DI registration needed** — `AddCoreServices` registers every validator in the Core
   assembly automatically via `AddValidatorsFromAssemblyContaining<CoreMarker>(ServiceLifetime.Transient)`.
6. **Test it** with `FluentValidation.TestHelper`: build a valid model via its `Builder` (see
   `add-unit-test.instructions.md`), mutate the specific field under test, then assert with
   `result.ShouldHaveValidationErrorFor(...)` / `.ShouldNotHaveValidationErrorFor(...)` /
   `.ShouldNotHaveAnyValidationErrors()`. Write one `[Fact]`/`[Theory]` per rule, including
   "smoke test" cases that confirm a child validator is actually invoked.
