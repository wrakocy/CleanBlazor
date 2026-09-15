# Copilot Instructions

<!-- This describes the Wrak.Clean.Blazor template itself. If you generated a new solution
     from it via `dotnet new blazor-clean -n <Company>.<Product>`, update the name/description
     below (and AGENTS.md to match) and check the persistence-model box once decided. -->

Wrak.Clean.Blazor is a Blazor Server application (.NET 10) — a starting point for a new
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

Primary workflow is Visual Studio: open `Wrak.Clean.Blazor.slnx` and hit F6 — `Web` is
already the default startup project and every launch profile runs with
`ASPNETCORE_ENVIRONMENT=Development` — then run tests from Test Explorer (xUnit + Moq).

From the CLI, the full verification ladder (also what CI runs):

```bash
dotnet format Wrak.Clean.Blazor.slnx
dotnet restore Wrak.Clean.Blazor.slnx
pwsh ./Verify-Package-Versions.ps1
dotnet build Wrak.Clean.Blazor.slnx
dotnet test Wrak.Clean.Blazor.slnx
```

While iterating, scope `dotnet test` to one project/class instead of the whole solution.

## Project Map

| Path | Purpose |
| --- | --- |
| `Wrak.Clean.Blazor.Core` | Use cases (Commands/Queries/Handlers), domain events, models, validators, interfaces. No dependency on Infrastructure or Web. |
| `Wrak.Clean.Blazor.Infrastructure` | Implementations of Core interfaces: repositories and/or external API clients, `AppBus`, cross-cutting filters. |
| `Wrak.Clean.Blazor.Web` | The ASP.NET Core host and Blazor Server UI (MudBlazor). All DI wiring lives in `Extensions/DependencyInjectionExtensions.cs`, called from `Program.cs`. |
| `Wrak.Clean.Blazor.UnitTests` | xUnit/Moq tests for Core, Infrastructure, and Web, mirroring their namespaces, plus shared test-data `Builders/`. |
| `Wrak.Clean.Blazor.IntegrationTests` / `Wrak.Clean.Blazor.FunctionalTests` | Integration tests (real dependencies) and full-stack functional tests (`WebApplicationFactory`). |

## Task-Specific Guidance

Detailed, per-task instructions live in `.github/instructions/*.instructions.md` and apply
automatically based on the files being edited:

- `add-cqrs-feature.instructions.md` — adding a Command/Query use case to Core.
- `add-domain-event.instructions.md` — adding a domain event and handler published through
  `IAppBus`.
- `add-fluentvalidation-validator.instructions.md` — adding a FluentValidation validator for a
  Core model.
- `add-persistence-integration.instructions.md` — adding a new repository (EF Core) or external
  API integration to Infrastructure.
- `add-blazor-page.instructions.md` — adding a Blazor page/component under
  `Components/Pages/Areas`.
- `add-unit-test.instructions.md` — adding a test in `Wrak.Clean.Blazor.UnitTests`.
- `add-integration-test.instructions.md` — adding a real-dependency test in
  `Wrak.Clean.Blazor.IntegrationTests`.
- `add-functional-test.instructions.md` — adding a full-stack functional test in
  `Wrak.Clean.Blazor.FunctionalTests`.
- `code-review-*.instructions.md` — PR review rules for GitHub Copilot Code Review, derived from
  the same conventions (repo-wide, Core, Infrastructure, Web/Blazor, Tests, and Security). The
  Security file applies only when a change touches an auth/secrets/input-handling boundary.

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

## Development & Review Workflow

Default to one primary pass doing the implementation, writing/updating its own tests, and running
the deterministic verification ladder above — not reviewing every change with every available
agent by habit. The `.github/agents/*.agent.md` reviewers are independent-verification steps at
specific risk boundaries, invoked only when a change actually touches that boundary:

1. Understand the task and the relevant slice of the architecture; inspect only what the task
   touches.
2. Implement the change, writing/updating the tests it needs as you go.
3. Run the Build & Test ladder above.
4. Review your own diff before asking for another review.
5. Invoke only the reviewers this change actually warrants:

   | Reviewer | Invoke when | Skip when |
   | --- | --- | --- |
   | `code-reviewer` | Any meaningful implementation change (new/changed use case, handler, component, Infrastructure integration, cross-cutting wiring) | Docs/comment-only edits, a single-constant change, a compiler-verified rename |
   | `test-reviewer` | Behavior materially changed, or the change carries real regression risk (new/changed handler, validator, domain event, boundary conversion) | No behavior changed and no tests needed changing |
   | `ui-reviewer` | Change affects Blazor UI behavior, state transitions, validation presentation, accessibility, responsive layout, MudBlazor usage, or implements a supplied design artifact | Backend-only change; a label/text swap; mechanical markup moves |
   | `security-reviewer` | Change touches auth, authorization, claims/identity, secrets/config, sensitive data, externally supplied input, file upload, crypto, or endpoint exposure | Ordinary CRUD with no security boundary crossed |

6. Address the findings you agree with; each reviewer reports independently and edits nothing
   itself.
7. Re-run only the deterministic checks affected by the fix. Re-invoke a reviewer only if the fix
   changed the risk area that reviewer examined.
8. Open the PR. Existing CI (`azure-pipelines-*.yml`: restore, `Verify-Package-Versions.ps1`,
   build, test) and human review apply unchanged — this workflow supplements that pipeline, it
   doesn't replace it. This repo's CI does not currently run static/security analysis
   (no SonarQube/CodeQL step configured); if one is added later, it re-verifies what
   `security-reviewer` already checked, it doesn't substitute for it.

## Enabling this file

Custom instructions are off by default in Visual Studio. Enable them under **Tools → Options →
GitHub → Copilot → Copilot Chat → "Enable custom instructions to be loaded from
.github/copilot-instructions.md files and added to requests."**
