# Architecture Overview

Clean Architecture, three projects, dependencies point inward only:

```
Web  →  Infrastructure  →  Core
(Core references nothing else in the solution)
```

- **Core** — CQRS use cases (MediatR requests + handlers), domain models, validators, domain
  events, and the interfaces for everything that lives outside Core (a database, an external API,
  email/SMS, etc.). No ASP.NET, no HTTP, no UI, no EF Core package reference.
- **Infrastructure** — implements Core's interfaces: repositories (EF Core), external API clients,
  the `IAppBus`/MediatR bridge, cross-cutting filters, file export services, and — only if the
  solution talks to an external API — in-memory read caches. Only project-references Core.
- **Web** — the ASP.NET Core host and Blazor Server UI. All dependency-injection wiring lives in
  one file, called in explicit order from `Program.cs`. Only project-references Infrastructure
  (which transitively brings in Core).
- **Tests** — `UnitTests` (mirrors the `Core`/`Infrastructure`/`Web` namespaces, one test project
  total — do not create a second, per-project unit test project), `IntegrationTests` (real
  dependencies, no mocks), `FunctionalTests` (full-stack via `WebApplicationFactory`, mirrors the
  Blazor page folder structure).

## Request flow

```
Blazor component (code-behind)
      │  _appBus.Send(command | query)  /  _appBus.Publish(event)
      ▼
IAppBus  (Infrastructure: AppBus, the ONLY class allowed to touch IMediator directly)
      │
      ▼
MediatR  →  Core handler (IRequestHandler<TRequest, TResponse>)
      │
      ├─ guards request + dependencies (Throw)
      ├─ calls Core interface for the backing store (repository and/or API client)
      ├─ on a successful state change, publishes a domain event via _appBus.Publish(...)
      └─ returns a Core model (never a DTO/EF entity) to the caller
      ▼
INotificationHandler<TEvent>  (reacts — typically invalidates/updates an in-memory cache
                                if the solution has one)
```

Nothing outside `Infrastructure` ever references `IMediator`, an `EF Core DbContext`, or a
generated API client type. Nothing outside `Core`/`Infrastructure` ever sees a DTO or EF entity —
Web only ever works with Core `Model` types.

## Persistence shape (decide once, per solution)

Core declares interfaces named for what they represent, not for the mechanism:

- API-backed: `I<System>Client` (e.g. `IBillingClient`), implemented in Infrastructure by a
  generated or hand-written HTTP client.
- DB-backed: `I<Entity>Repository` (e.g. `IOrderRepository`), implemented in Infrastructure by an
  EF Core repository over a `DbContext`.
- Both: some domains are API-backed, others are DB-backed — pick per domain, not globally. A
  single domain should not normally be split across both without a clear reason.

The in-memory cache-service pattern (`CacheServiceBase<TSnapshot>` + singleton per-domain cache
services, refreshed on a timer) exists specifically to compensate for a slow/rate-limited external
API. **It only appears in Infrastructure when the solution has an API-backed domain.** A pure
DB-backed domain reads through its repository directly — EF Core's own tracking/query
capabilities are the caching story there, and a background-refreshed in-memory snapshot is not a
default expectation.

## Cross-cutting conventions

- **Guard clauses**: the `Throw` package (`x.ThrowIfNull().Value`) at constructors and the top of
  every `Handle` method — never hand-rolled `if (x == null) throw ...`.
- **DI composition root**: one `DependencyInjectionExtensions.cs` in Web, one small named
  extension method per concern (`AddCoreServices`, `AddInfrastructureServices`, `AddWebServices`,
  `AddMediatr`, `AddPersistence`, …), called in explicit, order-dependent sequence from
  `Program.cs`.
- **Assembly markers**: an empty `CoreMarker`/`InfrastructureMarker`/`WebMarker` class per project,
  used only for assembly scanning (MediatR handler discovery, FluentValidation validator
  discovery, route discovery). Register all three markers with MediatR even before Web has any
  handlers of its own, for forward-compatibility.
- **Naming for conversion services** (API-backed domains only): a service is named for the type it
  *takes*, not the type it produces — `I<Entity>ModelConversionService.ToDto(model)` converts a
  Model to a Dto; `I<Entity>DtoConversionService.ToModel(dto)` converts a Dto to a Model. This
  reads backwards at a glance; document it so nobody "fixes" it later.
