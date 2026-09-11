# Copilot Instructions

<!-- This describes the Wrak.CleanArchitecture template itself. If you generated a new solution
     from it via `dotnet new blazor-clean -n <Company>.<Product>`, update the name/description
     below (and AGENTS.md to match) and check the persistence-model box once decided. -->

Wrak.CleanArchitecture is a Blazor Server application (.NET 10) — a starting point for a new
Clean Architecture solution, with no business domain of its own yet. It is built on Clean
Architecture: `Core` (CQRS use cases via MediatR, models, validators) is depended on by
`Infrastructure` (repositories and/or external API clients) and `Web` (the Blazor Server UI,
wired together with dependency injection). See `docs/` for the full architecture write-up.

> Working in Claude Code instead of Visual Studio? This same guidance is mirrored in
> [AGENTS.md](../AGENTS.md) and [.claude/skills/](../.claude/skills/) — keep both in sync if you
> edit either side.

## Persistence model

<!-- Delete the two that don't apply once decided. This single choice determines whether the
     cache-service pattern below is in play. -->
- [ ] External API client(s) — `Infrastructure` implements `I<System>Client` interfaces declared
      in Core.
- [ ] Database via EF Core — `Infrastructure` implements `I<Entity>Repository` interfaces declared
      in Core.
- [ ] Both — decided per domain, not globally.

## Build & Test

Primary workflow is Visual Studio: open `Wrak.CleanArchitecture.slnx`, set
`Wrak.CleanArchitecture.Web` as the startup project with the `Development` environment, hit F6,
and run tests from Test Explorer (xUnit + Moq).

From the CLI:

```bash
dotnet build Wrak.CleanArchitecture.slnx
dotnet test Wrak.CleanArchitecture.slnx
```

## Project Map

| Path | Purpose |
| --- | --- |
| `Wrak.CleanArchitecture.Core` | Use cases (Commands/Queries/Handlers), domain events, models, validators, interfaces. No dependency on Infrastructure or Web. |
| `Wrak.CleanArchitecture.Infrastructure` | Implementations of Core interfaces: repositories and/or external API clients, `AppBus`, cross-cutting filters. |
| `Wrak.CleanArchitecture.Web` | The ASP.NET Core host and Blazor Server UI (MudBlazor). All DI wiring lives in `Extensions/DependencyInjectionExtensions.cs`, called from `Program.cs`. |
| `Wrak.CleanArchitecture.UnitTests` | xUnit/Moq tests for Core, Infrastructure, and Web, mirroring their namespaces, plus shared test-data `Builders/`. |
| `Wrak.CleanArchitecture.IntegrationTests` / `Wrak.CleanArchitecture.FunctionalTests` | Integration tests (real dependencies) and full-stack functional tests (`WebApplicationFactory`). |

## Task-Specific Guidance

Detailed, per-task instructions live in `.github/instructions/*.instructions.md` and apply
automatically based on the files being edited:

- `create-solution.instructions.md` — bootstrapping a brand-new solution following this
  architecture (invoke this one explicitly — there's no file to edit yet for it to auto-apply to).
- `add-cqrs-feature.instructions.md` — adding a Command/Query use case to Core.
- `add-domain-event.instructions.md` — adding a domain event and handler published through
  `IAppBus`.
- `add-fluentvalidation-validator.instructions.md` — adding a FluentValidation validator for a
  Core model.
- `add-persistence-integration.instructions.md` — adding a new repository (EF Core) or external
  API integration to Infrastructure.
- `add-blazor-page.instructions.md` — adding a Blazor page/component under
  `Components/Pages/Areas`.
- `add-unit-test.instructions.md` — adding a test in `Wrak.CleanArchitecture.UnitTests`.
- `add-integration-test.instructions.md` — adding a real-dependency test in
  `Wrak.CleanArchitecture.IntegrationTests`.
- `add-functional-test.instructions.md` — adding a full-stack functional test in
  `Wrak.CleanArchitecture.FunctionalTests`.
- `code-review-*.instructions.md` — PR review rules for GitHub Copilot Code Review, derived from
  the same conventions (repo-wide, Core, Infrastructure, Web/Blazor, and Tests).

## Load-Bearing Conventions

### MUST
- Core MUST NOT reference Infrastructure or Web.
- All use-case dispatch MUST go through `IAppBus`; only `AppBus` (in Infrastructure) may inject
  `IMediator` directly.
- Every command/query request class MUST null-guard its constructor args via `Throw`
  (`.ThrowIfNull().Value`); it MUST NOT contain business-rule validation.
- Every handler MUST guard its injected dependencies in the constructor, and its request (and
  required members) at the top of `Handle`.
- A command handler that changes state MUST publish a domain event via `_appBus.Publish(...)`
  after the underlying write succeeds, if anything else in the app caches or reacts to that state.
- Business-rule validation MUST live in a `ValidatorBase<TModel>` colocated with its model — never
  inline in a handler or a Blazor component.
- All cross-cutting DI registration MUST happen in the one `DependencyInjectionExtensions.cs`,
  called in explicit order from `Program.cs`.
- Any service shared across Blazor circuits (e.g. an in-memory cache) MUST be registered
  `Singleton`; anything session/user-scoped MUST be `Scoped`.
- Generated client/DTO code MUST NOT be hand-edited — regenerate it and fix the hand-written call
  sites instead.
- A Blazor component MUST fetch/send data only via `_appBus` — never a repository, an API client,
  or `IMediator` directly.

### SHOULD
- One folder per use case under `Core/<Domain>/Features/<UseCase>/`, containing exactly the
  request + handler for that use case.
- One test class per method under test (`<Class>_<Method>.cs`), not one file per class.
- Test models SHOULD be built via `Builders/<Domain>/<Model>Builder.cs` with a `.WithRandomData()`
  convenience method once a model has any structure.
- Functional tests SHOULD assert only route success + visible page text, not internal state or
  interactivity.
- New shared validation rules SHOULD be added as `ValidationExtensions` methods before writing a
  new inline `.Must(...)` that might duplicate one.
- The in-memory cache-service pattern (`CacheServiceBase<TSnapshot>`) SHOULD only be introduced for
  a domain backed by an external API — not for a DB-backed domain reading through a repository.

### MUST NOT
- MUST NOT inject `IMediator` outside `AppBus`.
- MUST NOT put DTO/wire-shape or EF-entity concerns into a handler's public signature — convert
  to/from them only immediately around the boundary call.
- MUST NOT create a second, parallel unit-test project "just in case" — one `UnitTests` project
  only, mirroring source namespaces.
- MUST NOT name a functional test class after its containing folder when it doesn't match the page
  it tests — name it after the page.

## Enabling this file

Custom instructions are off by default in Visual Studio. Enable them under **Tools → Options →
GitHub → Copilot → Copilot Chat → "Enable custom instructions to be loaded from
.github/copilot-instructions.md files and added to requests."**
