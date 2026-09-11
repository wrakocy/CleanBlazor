---
name: add-cqrs-feature
description: Add a new Command or Query use case to Core (e.g. "add a create/update/get for X"). Use when the task is a new CRUD-style operation, not a UI-only or validation-only change.
---

# Add a CQRS Feature

Features live under `Core/<Domain>/Features/<UseCaseName>/`, one folder per use case, each with a
request type and a handler. See `docs/03-feature-development-guide.md`
§2–3 for full generic examples.

## Steps

1. **Pick the request base class.**
   - A command that returns data: `CommandBase<TResponse>`.
   - A command with no return value: `CommandBase`.
   - A query: `QueryBase<TResponse>`.
2. **Write the request class** in `<Domain>/Features/<UseCaseName>/<UseCaseName>Command.cs` (or
   `Query.cs`). Guard constructor args with `Throw`:
   ```csharp
   public class CreateOrderCommand : CommandBase<OrderModel>
   {
       public CreateOrderCommand(OrderModel model) => Model = model.ThrowIfNull().Value;
       public OrderModel Model { get; }
   }
   ```
   A query with only filter properties (no required constructor args) is plain settable
   properties, often with a `MaxItems` constant.
3. **Write the handler** in `<UseCaseName>Handler.cs`, implementing
   `IRequestHandler<TRequest, TResponse>`. Constructor-inject only what's needed (the repository
   or API client for this domain, conversion services, `IAppBus`), guard each with
   `.ThrowIfNull().Value`, and guard the incoming request/its required members at the top of
   `Handle`. If the operation changes state that other parts of the app cache or care about,
   publish a domain event afterward via `_appBus.Publish(...)` (see the `add-domain-event` skill)
   — do not call `IMediator` directly.
4. **Register any new services** the handler depends on (conversion services, factories,
   repositories, cache services) in `Web/Extensions/DependencyInjectionExtensions.cs`
   (`AddCoreServices` for Core services, `AddInfrastructureServices` for
   Infrastructure/persistence/cache services). The handler itself needs no registration — MediatR
   auto-discovers it from the `CoreMarker` assembly.
5. **Call it from Blazor** via `_appBus.Send(new CreateOrderCommand(model))` from a component's
   code-behind (see `add-blazor-page`), never by injecting `IMediator`.
6. **Add tests** — see `add-unit-test`. At minimum: a constructor test for the request (null-arg
   guard behavior) and a `Handle` test for the handler (happy path + null-arg guard).

## Gotchas

- Validators are separate from the command/query — see `add-fluentvalidation-validator`; a
  command constructor only null-guards, it doesn't validate business rules.
- Don't put DTO/EF-entity shape concerns in the request/handler signature beyond what the
  conversion services (API-backed) or repository (DB-backed) already handle — handlers work in
  terms of Core `Model` types, converting only immediately around the persistence-boundary call.
- Keep commands and queries in this one skill/workflow rather than splitting into separate
  "add-command"/"add-query" steps — they share the same folder-per-use-case shape and mostly
  differ only in base class.
