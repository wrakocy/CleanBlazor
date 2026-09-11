# Testing Guide

| Kind | Project | Covers | Naming & pattern |
| --- | --- | --- | --- |
| Unit | `UnitTests` | Every public method with logic: request constructors (null-guard), handlers (happy path + null-guard), validators (one rule per test), conversion services/repositories, factories, Web code-behind logic. | One test class per method under test: `<ClassUnderTest>_<MethodName>.cs` (e.g. `CreateOrderHandler_Handle.cs`). Never one big `<Class>Tests.cs`. |
| Integration | `IntegrationTests` | Code with a *real* external dependency worth exercising for real — a file/byte-format writer, a real (test) database round-trip, anything where mocking would hide the actual risk. | Same `<Class>_<Method>.cs` naming; no mocking framework. |
| Functional | `FunctionalTests` | Every Blazor route resolves and renders through the real, DI-wired app. | Mirrors the **Web page's folder path**, not the unit-test naming scheme; one `[Fact]` per route. |

## Unit tests

- Mock dependencies with Moq, held as `readonly` fields initialized inline; construct the subject
  in the test class's constructor:
  ```csharp
  public class CreateOrderHandler_Handle
  {
      private readonly Mock<IOrderRepository> _repo = new();
      private readonly Mock<IAppBus> _appBus = new();
      private readonly CreateOrderHandler _handler;

      public CreateOrderHandler_Handle() => _handler = new(_repo.Object, _appBus.Object);

      [Fact]
      public async Task WhenCommandIsValid() { /* Arrange / Act / Assert */ }

      [Fact]
      public async Task WhenCommandIsNull() =>
          await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(null!, CancellationToken.None));
  }
  ```
- `[Fact]` for a single case, `[Theory]`/`[InlineData]` for parameterized cases.
- Verify both the return value and important side-effect calls: `_mock.Verify(x => x.Method(...), Times.Once)`.
- **Always** test the `Throw`-guard path for any public method that guards its arguments.
- Build test models via `Builders/<Domain>/<Model>Builder.cs`, not inline object initializers, once
  a model has any structure:
  ```csharp
  public class OrderModelBuilder
  {
      private CustomerModel _customer = default!;

      public OrderModelBuilder WithRandomData()
      {
          _customer = new CustomerModelBuilder().WithRandomData().Build();
          return this;
      }

      public OrderModelBuilder WithCustomer(CustomerModel customer) { _customer = customer; return this; }

      public OrderModel Build() => new() { Id = RandomData.As.Int(), Customer = _customer };
  }
  ```
  Add a builder in the matching `Builders/<Domain>/` folder if one doesn't exist yet for a model.
- Validator tests use `FluentValidation.TestHelper`:
  ```csharp
  var result = await _validator.TestValidateAsync(model);
  result.ShouldHaveValidationErrorFor(x => x.Field).WithErrorMessage("...");
  result.ShouldNotHaveAnyValidationErrors();
  ```
  For rules on collection items, reference the indexed property by string:
  `$"{nameof(Model.Items)}[0].{nameof(Item.Field)}"`. Write one `[Fact]`/`[Theory]` per rule,
  including a "smoke test" confirming a child validator is actually invoked.

## Integration tests

- No per-test mocking — these exist specifically to exercise a real dependency.
- Same `<Class>_<Method>.cs` naming as unit tests, in `IntegrationTests` instead.
- Keep these few and targeted: a byte-format export, a real filesystem/network round-trip. Most
  behavior belongs in `UnitTests` with mocks; reach for `IntegrationTests` only when mocking the
  dependency would leave the real risk untested.

## Functional tests

- File location mirrors the page's path in Web: a page at
  `Web/Components/Pages/Areas/Orders/Search/Search.razor` gets
  `FunctionalTests/Components/Pages/Areas/Orders/Search/Search.cs`.
- **Name the class after the page**, not the containing folder.
- Inherit `WebApplicationTestFixtureBase`, which exposes `_client`:
  ```csharp
  public class Search(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
      : WebApplicationTestFixtureBase(factory, outputHelper)
  {
      [Fact]
      public async Task ReturnsViewWithCorrectMessage()
      {
          var rsp = await _client.GetAsync("/orders");
          rsp.EnsureSuccessStatusCode();
          var body = await rsp.Content.ReadAsStringAsync();
          Assert.Contains("Order Search", body);
      }
  }
  ```
- One `[Fact]` per route the page responds to, named `ReturnsViewWithCorrectMessage` (add a
  `WithId`/`WithoutId` suffix when the page has both a parameterized and parameterless route).
  Assert success status plus the page's visible heading/title text — what a user would actually
  see — not an internal route or constant name.
- No per-test mocking: `CustomWebApplicationFactory` registers every mock boundary
  (`Mocks/Mock<System>Client.cs` etc.) once for the whole test run. If a page needs a method that
  isn't on the mock yet, add it there following the existing methods' pattern, rather than mocking
  per-test.
- Scope of assertion: these tests only prove the route resolves and the page renders. They don't
  assert on grid contents or exercise button clicks/interactivity — that's for component tests
  (bUnit) or manual/browser verification.

## What NOT to do

- Don't create a second unit-test project per source project — one `UnitTests` project, mirroring
  namespaces, is canonical (see `02-bootstrap-guide.md` §1).
- Don't name a functional test class after its containing folder when it doesn't match the page it
  tests — name it after the page.
- Don't assert on internal implementation details (route constants, DI registrations) in a
  functional test — assert on what a user sees.
