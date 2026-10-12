---
description: Add a unit test in the solution's UnitTests project. Use whenever adding or changing behavior in Core, Infrastructure, or Web that needs test coverage — covers this repo's naming, Moq, and test-data-builder conventions.
applyTo: "*.UnitTests/**/*.cs"
---

# Add a Unit Test

> Mirrors
> [.claude/skills/add-unit-test/SKILL.md](../../.claude/skills/add-unit-test/SKILL.md)
> for Claude Code — same guidance, two platforms. If you change one, change the other.

All unit tests live in the single `<Product>.UnitTests` project — never create a second,
per-source-project unit test project — mirroring the namespace of the code under test:
`Core/<Domain>/...`, `Infrastructure/...`, `Web/...`. See `docs/04-testing-guide.md`.

## Steps

1. **One test class per method under test**, named `<ClassUnderTest>_<MethodName>.cs`, e.g.
   `CreateOrderHandler_Handle.cs`, `OrderModelValidator_Constructor.cs`. Don't create one big
   `<ClassUnderTest>Tests.cs` file.
2. **Mock dependencies with Moq**, held as `readonly` fields initialized inline, and construct the
   subject in the test class's constructor:
   ```csharp
   private readonly Mock<IOrderRepository> _repo = new();
   private readonly CreateOrderHandler _handler;

   public CreateOrderHandler_Handle() => _handler = new(_repo.Object, ...);
   ```
3. **Use `[Fact]` for a single case, `[Theory]`/`[InlineData]` for parameterized cases.** Follow
   Arrange/Act/Assert comments for anything non-trivial. Verify both the return value and any
   important side-effect calls with `_mock.Verify(..., Times.Once)`.
4. **Always test the `Throw`-guard path** for public methods that guard their arguments:
   `await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(null!, ...));`
5. **Build test models with the `Builders/` classes**, not inline object initializers, when the
   model has any structure — `UnitTests/Builders/<Domain>/<Model>Builder.cs`, fluent `With...()`
   setters plus a `.WithRandomData()` convenience method that fills every field using
   `RandomData.As.X()` helpers (`Wrak.RandomData`). Add a new builder in the matching
   `Builders/<Domain>/` folder if one doesn't exist yet for a model.
6. **Validator tests** use `FluentValidation.TestHelper`: `await
   _validator.TestValidateAsync(model)`, then `result.ShouldHaveValidationErrorFor(x =>
   x.Field)` / `.WithErrorMessage(...)`, or `.ShouldNotHaveAnyValidationErrors()`. For rules on
   collection items, reference the indexed property by string:
   `$"{nameof(Model.Items)}[0].{nameof(Item.Field)}"`.
7. **Integration tests** (real dependencies) go in `IntegrationTests` instead, using the same
   `<Class>_<Method>.cs` naming — see `add-integration-test.instructions.md`.
   **Functional/full-stack tests** (`WebApplicationFactory`) go in `FunctionalTests` — see
   `add-functional-test.instructions.md`.
