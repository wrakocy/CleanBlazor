---
description: Add or update how Infrastructure reaches a backing store for a domain — an EF Core repository, an external API client operation, or (for an API) the in-memory cache service that sits in front of it. Use when a handler needs a persistence capability that doesn't exist yet, not for hand-editing generated client code.
applyTo: "src/*.Infrastructure/**/*.cs"
---

# Add a Persistence Integration

Which path applies depends on this solution's persistence model (check `copilot-instructions.md`'s
"Persistence model" section). A single solution may use both, decided per domain.

## Path A — Database via EF Core

1. **Declare the interface in Core**: `Core/<Domain>/Interfaces/I<Entity>Repository.cs`, named for
   the operations the domain's handlers actually need (`AddAsync`, `GetByIdAsync`,
   `GetByFilterAsync`, …) — not a generic `IRepository<T>` unless every domain genuinely needs the
   same shape.
2. **Implement it in Infrastructure**: `Infrastructure/Persistence/Repositories/<Entity>Repository.cs`,
   working against the shared `<Product>DbContext`. Add an `IEntityTypeConfiguration<T>` under
   `Infrastructure/Persistence/Configurations/` if the entity needs relationship/index/constraint
   configuration. If the EF entity shape differs from the Core `Model`, the repository is
   responsible for translating between them — handlers never see the EF entity type.
3. **Add a migration**: `dotnet ef migrations add <Name> --project Infrastructure --startup-project Web`.
   Diff the generated migration before committing — check it only touches what you expect.
4. **Register** the repository in `AddInfrastructureServices` (typically `Scoped`, matching the
   `DbContext`'s lifetime). No DI registration is needed for the `DbContext` itself beyond the one
   `AddDbContext<T>()` call already wired at bootstrap.
5. **Test it** — see `add-integration-test.instructions.md` for a real-database round-trip test,
   and `add-unit-test.instructions.md` for handler tests that mock `I<Entity>Repository`.

## Path B — External API client

1. If the client is generated from an OpenAPI spec (NSwag or similar), **never hand-edit** the
   generated file or its interface. Refresh the spec, regenerate, then diff the result before
   committing anything — a single new endpoint often reformats large unrelated sections of the
   generated file; confirm the diff is limited to what you expect.
2. **Fix hand-written call sites** broken by DTO shape changes — conversion services and handlers
   are hand-written and won't be touched by regeneration.
3. If a new DTO needs a corresponding Core `Model`, add/update the conversion service
   (`I<Domain>DtoConversionService` / `I<Domain>ModelConversionService`) rather than using the DTO
   directly outside Infrastructure/handler boundaries.
4. If the client is hand-written instead of generated, implement the new operation on
   `I<System>Client` in Core and its Infrastructure implementation directly, following the
   existing methods' shape (typed `HttpClient`, cross-cutting concerns via the registered
   `Filters/` delegating handlers).

## Path B, continued — the read-side cache (only if this domain needs fast reads from a slow API)

1. **Declare** `I<Domain>CacheService : ICacheService` in Core, exposing read methods
   (`GetAll<Entity>s()`) and `Notify...Changed(...)` methods for domain-event handlers to call.
2. **Implement** `<Domain>CacheService : CacheServiceBase<TSnapshot>` in Infrastructure,
   implementing `EmptySnapshot` and `RefreshDataAsync(filters)` — the base class handles the
   timer, the lock against overlapping refreshes, and the `Volatile`-swapped snapshot.
3. **Register** it as a `Singleton` in `AddInfrastructureServices` — cache services are shared
   across every Blazor circuit and must never be `Scoped`/`Transient`.
4. **Initialize** it at startup: add a call in the app's cache-initialization step (invoked once
   from `Program.cs` after `app.Build()`), so the first snapshot loads before the app starts
   serving traffic.
5. Have `add-domain-event.instructions.md` handlers call its `Notify...Changed` methods after a
   write, and have query handlers for this domain read through the cache instead of hitting the
   API per request.

## Gotchas

- Don't introduce Path B's cache service for a domain that's DB-backed — read through the
  repository directly instead.
- Don't mix Path A and Path B within a single domain's handlers without a clear reason — pick one
  backing store per domain.
- A generated API client and its DTOs are off-limits for manual edits under all circumstances —
  fix the spec/generator inputs, then regenerate.
