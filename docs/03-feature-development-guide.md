# Feature Development Guide

Canonical patterns for adding functionality once the solution exists (see
[02-bootstrap-guide.md](02-bootstrap-guide.md) for the one-time setup). Examples use a placeholder
domain **`Order`** under an `Orders` area. Persistence steps branch on whether your solution is
**[API]**-backed, **[DB]**-backed, or **[Both]** — see §1 of the architecture overview.

## Domain folder shape

Every domain gets its own top-level folder in Core, containing only what it needs:

```
Core/Orders/
  Enums/
  Events/<EventName>/
  Exceptions/
  Extensions/
  Features/<UseCaseName>/
  Interfaces/
  Models/<ModelName>/
  Services/
  ValueObjects/
  OrderConstants.cs         (only if the domain needs shared magic numbers/strings)
```
Don't create empty subfolders speculatively — add `Enums/`, `ValueObjects/`, etc. only when a
feature actually needs one.

## 1. New domain entity/model

```
Core/Orders/Models/Order/
  OrderModel.cs
  OrderListItemModel.cs        (a lighter shape for list/search results, if the full model is heavy)
  OrderModelValidator.cs       // : ValidatorBase<OrderModel>
  OrderModelFactory.cs         // : IOrderModelFactory
Core/Orders/Interfaces/
  IOrderModelFactory.cs
```
- Models are **anemic POCOs** — mutable properties, no business-logic methods. Behavior lives in
  handlers, factories, conversion services, and validators, not on the model.
- `IOrderModelFactory.BuildWithDefaultValues()` returns a new model with every default value a
  "new" record should start with — this is what a "create new" Blazor page calls instead of
  `new OrderModel()` directly, so defaults live in one place.
- If a model tracks its own "did this change" state (e.g. a `NameHasChanged` flag used by a domain
  event handler to decide whether to invalidate a lookup cache), expose it as a computed property
  comparing a current value to an `Original...` value captured at load time — don't compute the
  diff in the handler or the UI.

## 2. New command

```
Core/Orders/Features/CreateOrder/
  CreateOrderCommand.cs
  CreateOrderHandler.cs
```
- Pick the base: `CommandBase<TResponse>` if it returns data (most creates/updates do — return the
  saved model), plain `CommandBase` if it truly returns nothing.
- Request class only null-guards constructor args via `Throw` — it does **not** validate business
  rules (that's the validator, run by the Blazor page before `Save()` — see §6):
  ```csharp
  public class CreateOrderCommand : CommandBase<OrderModel>
  {
      public CreateOrderCommand(OrderModel model) => Model = model.ThrowIfNull().Value;
      public OrderModel Model { get; }
  }
  ```
- Handler constructor-injects only what it needs, guards each dependency, guards the request and
  its required members at the top of `Handle`, performs the write, publishes a domain event on
  success, converts and returns:
  ```csharp
  public class CreateOrderHandler(IOrderRepository repo, IAppBus appBus) : IRequestHandler<CreateOrderCommand, OrderModel>
  {
      private readonly IOrderRepository _repo = repo.ThrowIfNull().Value;
      private readonly IAppBus _appBus = appBus.ThrowIfNull().Value;

      public async Task<OrderModel> Handle(CreateOrderCommand cmd, CancellationToken token)
      {
          var model = cmd.ThrowIfNull().Value.Model.ThrowIfNull().Value;
          var saved = await _repo.AddAsync(model, token);          // [DB]
          // var saved = await _api.CreateOrderAsync(dto, token);  // [API] — convert to/from Dto immediately around this call

          await _appBus.Publish(new OrderChangedEvent(saved), token);
          return saved;
      }
  }
  ```

## 3. New query

```
Core/Orders/Features/GetOrders/
  GetOrdersQuery.cs
  GetOrdersHandler.cs
```
- Queries extend `QueryBase<TResponse>`. A search/filter query typically has no required
  constructor args — plain settable properties plus a `MaxItems` constant:
  ```csharp
  public class GetOrdersQuery : QueryBase<List<OrderListItemModel>>
  {
      public static int MaxItems => 500;
      public HashSet<string> OrderIds { get; set; } = [];
      public string? CustomerName { get; set; }
  }
  ```
- Handler guards the query, calls the repository/API, and returns Core models — never DTOs or EF
  entities.

## 4. New domain event

```
Core/Orders/Events/OrderChanged/
  OrderChangedEvent.cs
  OrderChangedHandler.cs
```
```csharp
public class OrderChangedEvent(OrderModel model) : ApplicationEventBase
{
    public OrderModel Model { get; } = model;
}

public class OrderChangedHandler(IOrdersCacheService cacheSvc) : INotificationHandler<OrderChangedEvent>
{
    private readonly IOrdersCacheService _cacheSvc = cacheSvc.ThrowIfNull().Value;

    public Task Handle(OrderChangedEvent evt, CancellationToken ct)
    {
        var model = evt.ThrowIfNull().Value.Model.ThrowIfNull().Value;
        _cacheSvc.NotifyOrderChanged(model.Id);   // [API]-backed domains: invalidate/refresh the cache
        return Task.CompletedTask;                // [DB]-backed domains often don't need a handler at all
                                                    // unless another in-process concern reacts to the change
    }
}
```
- Publish from the command handler, **after** the write succeeds, never before.
- Only write a handler if something actually needs to react. A DB-backed domain with no in-memory
  cache and no other reactive concern doesn't need an event for every write — don't publish events
  "just in case."
- No DI registration needed — `INotificationHandler<T>` implementations are auto-discovered from
  the `CoreMarker` assembly.

## 5. Validation

```
Core/Orders/Models/Order/OrderModelValidator.cs
```
- Extend `ValidatorBase<OrderModel>`, not `AbstractValidator<OrderModel>` directly.
- Inject child validators for nested models via constructor (`IValidator<TChild>`), wire with
  `RuleFor(x => x.Child).SetValidator(...)` / `RuleForEach(x => x.Children).SetValidator(...)`.
- Cross-field/collection rules: `.Must(...).WithMessage(...)`, or `.Custom((value, ctx) =>
  ctx.AddFailure(...))` when you need multiple, dynamic messages per item. Use
  `.Cascade(CascadeMode.Stop)` when a later check assumes an earlier one passed.
- Check `Shared/Extensions/ValidationExtensions.cs` for an existing extension-method rule (e.g.
  `.MustNotConflict()`) before writing a new inline `.Must(...)` that duplicates one.
- No DI registration needed — `AddCoreServices` registers every validator in the Core assembly via
  `AddValidatorsFromAssemblyContaining<CoreMarker>(ServiceLifetime.Transient)`.

## 6. Persistence

**[DB]** — repository behind a Core interface:
```
Core/Orders/Interfaces/IOrderRepository.cs
Infrastructure/Persistence/Repositories/OrderRepository.cs
Infrastructure/Persistence/Configurations/OrderEntityConfiguration.cs
```
Handlers work exclusively in terms of Core `Model` types; the repository is responsible for
translating to/from EF entities internally if the EF entity shape differs from the Core model.
Register the repository (typically `Scoped`, matching the `DbContext` lifetime) in
`AddInfrastructureServices`.

**[API]** — conversion services behind Core interfaces:
```
Core/Orders/Interfaces/IOrderDtoConversionService.cs     // Dto → Model
Core/Orders/Interfaces/IOrderModelConversionService.cs   // Model → Dto
Core/Orders/Services/OrderDtoConversionService.cs
Core/Orders/Services/OrderModelConversionService.cs
```
Remember the naming direction: `OrderModelConversionService.ToDto(model)` (named for its *input*),
`OrderDtoConversionService.ToModel(dto)` (also named for its *input*). Handlers call the API
client only immediately around the boundary — convert to a `Dto` right before the call, convert
back to a `Model` right after. Register both as `Transient` in `AddCoreServices`. If a domain's
data is read frequently and the API is slow, add an `I<Domain>CacheService` (see the
`add-persistence-integration` skill) and have handlers read through it instead of hitting the API
on every query.

**[Both]** — apply whichever of the two above matches that particular domain; don't mix DTO
conversion and repository patterns within a single domain's handlers.

## 7. Blazor page

```
Web/Components/Pages/Areas/Orders/Search/Search.razor(.cs)
Web/Components/Pages/Areas/Orders/Details/Details.razor(.cs)
```
- Code-behind class (`SearchBase`/`DetailsBase`) in the `.razor.cs` file, inheriting
  `AppComponentBase`; the `.razor` file declares `@inherits <Name>Base`.
- `[Inject]` fields follow the `_camelCase` convention even though `protected`, matching the rest
  of the codebase.
- Fetch/send data **only** through `_appBus` — never `IApiClient`/`IOrderRepository`/`IMediator`
  directly from a component.
- Wrap any async call that hits the backing store in the working-spinner pattern:
  ```csharp
  _appState.Working = true;
  await ForceComponentRefreshAsync();
  var results = await _appBus.Send(_qry);
  _appState.Working = false;
  ```
- Breadcrumbs, icons, page-size defaults, mask patterns: pull from `AppConstants` — check there
  before hardcoding a string.
- Search/list pages persist their query to `ILocalStorageService`, keyed by a page-specific string
  constant (e.g. `"orders.search.query"`), restored in `OnAfterRenderAsync(firstRender: true)`.
- Prefer existing shared components (`Progress`, `ProgressOverlay`, `PageHeader`,
  `LargeResultSetAlert`, a list-of-values-backed select) and `MudDataGrid`/`MudExpansionPanels`/
  `MudGrid` for layout before building something new.

## 8. Tests

See [04-testing-guide.md](04-testing-guide.md) for the full breakdown. At minimum for a new
feature: a constructor test for the request (null-arg guard), a `Handle` test for the handler
(happy path + null-arg guard), a validator test per rule, and — if a new page/route was added — a
functional test proving the route resolves and renders.

## What NOT to do

- Don't put DTO/EF-entity shape concerns into a handler's public signature — convert immediately
  around the boundary call, work in Core `Model` types everywhere else.
- Don't add an in-memory cache service for a DB-backed domain "for consistency" with an API-backed
  one elsewhere in the same solution — it's a compensating structure for a specific problem
  (slow/rate-limited external API), not a default.
- Don't leave large commented-out chunks of a half-built feature in a component's code-behind —
  delete unused code; git history is where "we tried this before" lives.
- Don't validate business rules inside a command/query constructor — that's the validator's job,
  run explicitly by the Blazor page before `Save()`.
