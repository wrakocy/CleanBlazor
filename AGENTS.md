# AGENTS.md

<!-- This describes the Wrak.Clean.Blazor template itself. If you generated a new solution
     from it via `dotnet new blazor-clean -n <Company>.<Product>`, update the name/description
     below (and copilot-instructions.md to match) and check the persistence-model box once
     decided. -->

Wrak.Clean.Blazor is a Blazor Server application (.NET 10) — a starting point for a new
Clean Architecture solution, with no business domain of its own yet. It is built on Clean
Architecture: `Core` (CQRS use cases via MediatR, models, validators) is depended on by
`Infrastructure` (repositories and/or external API clients) and `Web` (the Blazor Server UI,
wired together with dependency injection). See [docs/](docs/) for the full architecture write-up.

> Working in Visual Studio with GitHub Copilot instead of Claude Code? This same guidance is
> mirrored in [.github/copilot-instructions.md](.github/copilot-instructions.md) and
> [.github/instructions/](.github/instructions/) — keep both in sync if you edit either side.

## Persistence model

<!-- Delete the two that don't apply once decided. This single choice determines whether the
     cache-service pattern below is in play. -->
- [ ] External API client(s) — `Infrastructure` implements `I<System>Client` interfaces declared
      in Core.
- [ ] Database via EF Core — `Infrastructure` implements `I<Entity>Repository` interfaces declared
      in Core.
- [ ] Both — decided per domain, not globally.

## Build & Test

```bash
dotnet format Wrak.Clean.Blazor.slnx
dotnet restore Wrak.Clean.Blazor.slnx
pwsh ./Verify-Package-Versions.ps1
dotnet build Wrak.Clean.Blazor.slnx
dotnet test Wrak.Clean.Blazor.slnx
```

While iterating, scope `dotnet test` to one project/class instead of the whole solution. See
[verify-and-review](.claude/skills/verify-and-review/SKILL.md) for the full pre-PR procedure,
including when to invoke which specialist reviewer.

## Project Map

| Path | Purpose |
| --- | --- |
| `Wrak.Clean.Blazor.Core` | Use cases (Commands/Queries/Handlers), domain events, models, validators, interfaces. No dependency on Infrastructure or Web. |
| `Wrak.Clean.Blazor.Infrastructure` | Implementations of Core interfaces: repositories and/or external API clients, `AppBus`, cross-cutting filters. |
| `Wrak.Clean.Blazor.Web` | The ASP.NET Core host and Blazor Server UI (MudBlazor). All DI wiring lives in `Extensions/DependencyInjectionExtensions.cs`, called from `Program.cs`. |
| `Wrak.Clean.Blazor.UnitTests` | xUnit/Moq tests for Core, Infrastructure, and Web, mirroring their namespaces, plus shared test-data `Builders/`. |
| `Wrak.Clean.Blazor.IntegrationTests` / `Wrak.Clean.Blazor.FunctionalTests` | Integration tests (real dependencies) and full-stack functional tests (`WebApplicationFactory`). |

## Available Skills

- `add-cqrs-feature` — add a Command and/or Query use case to Core.
- `add-domain-event` — add a domain event and handler published through `IAppBus`.
- `add-fluentvalidation-validator` — add a FluentValidation validator for a Core model.
- `add-persistence-integration` — add a new repository (EF Core) or external API integration to
  Infrastructure.
- `add-blazor-page` — add a Blazor page/component under `Components/Pages/Areas`.
- `add-unit-test` — add a test following this repo's one-class-per-method and Builder conventions.
- `add-integration-test` — add a real-dependency test in `IntegrationTests`.
- `add-functional-test` — add a full-stack functional test for a Blazor page.
- `verify-and-review` — the pre-PR verification ladder and risk-based specialist-reviewer
  selection; see [PR Workflow](#pr-workflow) below.

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

## PR Workflow

Default to one primary agent doing the implementation, writing/updating its own tests, and
running deterministic verification — not a fleet of subagents spawned by habit. Specialist
reviewers are independent-verification steps at specific risk boundaries, invoked only when the
change actually touches that boundary. Full procedure and reviewer-briefing shape:
[verify-and-review](.claude/skills/verify-and-review/SKILL.md).

1. Understand the task and the relevant slice of the architecture; inspect only what the task
   touches.
2. Implement the change, writing/updating the tests it needs as you go.
3. Run the [Build & Test](#build--test) ladder above.
4. Review your own diff (`git diff`) before asking anyone else to.
5. Invoke only the specialist reviewers this change actually warrants:

   | Reviewer | Invoke when | Skip when |
   | --- | --- | --- |
   | `code-reviewer` | Any meaningful implementation change (new/changed use case, handler, component, Infrastructure integration, cross-cutting wiring). Also covers Blazor UI behavior, states, accessibility, and design-system usage when the diff touches `Web` — brief it with the design artifact if one exists | Docs/comment-only edits, a single-constant change, a rename the compiler already verified |
   | `test-reviewer` | Behavior materially changed, or the change carries real regression risk (new/changed handler, validator, domain event, boundary conversion) | No behavior changed and no tests needed changing |
   | `security-reviewer` | Change touches auth, authorization, claims/identity, secrets/config, sensitive data, externally supplied input, file upload, crypto, or endpoint exposure | Ordinary CRUD with no security boundary crossed |

6. Address the findings you agree with; each reviewer reports independently and edits nothing
   itself.
7. Re-run only the deterministic checks affected by the fix. Re-invoke a reviewer only if the fix
   changed the risk area that reviewer examined — not as a matter of course.
8. Open the PR. Existing CI (`azure-pipelines-*.yml`: restore, `Verify-Package-Versions.ps1`,
   build, test) and human review apply unchanged — these subagents supplement that process, not
   replace it. If static/security analysis (e.g. SonarQube, CodeQL) is added to CI later, treat it
   the same way: CI re-verifies, it doesn't replace the local specialist review.
