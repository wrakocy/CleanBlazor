# Copilot Instructions

<!-- This describes the Wrak.CleanBlazor template itself. If you generated a new solution
     from it via `dotnet new blazor-clean -n <Company>.<Product>`, update the name/description
     below (and AGENTS.md to match) and check the persistence-model box once decided. -->

Wrak.CleanBlazor is a Blazor Server application (.NET 10) — a starting point for a new
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

## CI & Deployment

GitHub Actions around a single long-lived `main` branch (details: README "Branching, CI and
Deployment"):

- **CI** (`.github/workflows/ci.yml`, job `ci`) runs on push/PR to `main`: stamp `BuildInfo.cs` →
  restore → `Verify-Package-Versions.ps1` → `dotnet format --verify-no-changes --no-restore` →
  build → test. Only a passing push to `main` publishes the `drop` artifact.
- **Deploy** (`.github/workflows/deploy.yml`) never builds. It deploys a CI run's `drop` to Test
  automatically, or to Test/Production manually, after a `validate` job rejects any run that
  isn't a successful push to `main`. It is inert unless repo variable `DEPLOY_ENABLED` is `true`.
  Secrets come from Key Vault at deploy time and become App Service app settings; no secrets or
  `#{token}#` placeholders belong in any `appsettings*.json`.

## Build & Test

Primary workflow is Visual Studio: open `Wrak.CleanBlazor.slnx` and hit F6 — `Web` is
already the default startup project and every launch profile runs with
`ASPNETCORE_ENVIRONMENT=Development` — then run tests from Test Explorer (xUnit + Moq).

From the CLI, the full verification ladder (also what CI runs):

```bash
dotnet format Wrak.CleanBlazor.slnx
dotnet restore Wrak.CleanBlazor.slnx
pwsh ./Verify-Package-Versions.ps1
dotnet build Wrak.CleanBlazor.slnx
dotnet test Wrak.CleanBlazor.slnx
```

While iterating, scope `dotnet test` to one project/class instead of the whole solution.

## Project Map

| Path | Purpose |
| --- | --- |
| `Wrak.CleanBlazor.Core` | Use cases (Commands/Queries/Handlers), domain events, models, validators, interfaces. No dependency on Infrastructure or Web. |
| `Wrak.CleanBlazor.Infrastructure` | Implementations of Core interfaces: repositories and/or external API clients, `AppBus`, cross-cutting filters. |
| `Wrak.CleanBlazor.Web` | The ASP.NET Core host and Blazor Server UI (MudBlazor). All DI wiring lives in `Extensions/DependencyInjectionExtensions.cs`, called from `Program.cs`. |
| `Wrak.CleanBlazor.UnitTests` | xUnit/Moq tests for Core, Infrastructure, and Web, mirroring their namespaces, plus shared test-data `Builders/`. |
| `Wrak.CleanBlazor.IntegrationTests` / `Wrak.CleanBlazor.FunctionalTests` | Integration tests (real dependencies) and full-stack functional tests (`WebApplicationFactory`). |

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
- `add-unit-test.instructions.md` — adding a test in `Wrak.CleanBlazor.UnitTests`.
- `add-integration-test.instructions.md` — adding a real-dependency test in
  `Wrak.CleanBlazor.IntegrationTests`.
- `add-functional-test.instructions.md` — adding a full-stack functional test in
  `Wrak.CleanBlazor.FunctionalTests`.
- `code-review-*.instructions.md` — PR review rules for GitHub Copilot Code Review, derived from
  the same conventions (repo-wide, Core, Infrastructure, Web/Blazor, Tests, and Security). The
  Security file applies only when a change touches an auth/secrets/input-handling boundary.
- `code-review-kit-*.instructions.md` and `agentkit-workflow.instructions.md` — organization-wide
  review rules and the agent workflow policy, synced from
  [AgentKit](https://github.com/wrakocy/AgentKit). The `code-review-*` files above hold only this
  repository's specific rules; don't edit the synced files here.

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

The agent workflow (one primary pass, deterministic verification, specialist reviewers only at risk
boundaries, and the rule to confirm explicitly before any commit, push or pull request) comes from
[AgentKit](https://github.com/wrakocy/AgentKit): see `agentkit-workflow.instructions.md`, the
`verify-and-review` skill in `.github/skills/`, and the `code-reviewer`, `test-reviewer` and
`security-reviewer` agents in `.github/agents/`. These are synced files; don't edit them here. The
version is pinned in `.github/agentkit.json`. The commands the workflow runs are the Build & Test
ladder above.

This repository's own examples of when each reviewer applies:

- **`code-reviewer`**: any new or changed use case (Command/Query/Handler), component,
  Infrastructure integration (repository, API client, cache service), or cross-cutting wiring in
  `DependencyInjectionExtensions.cs`. It also covers Blazor UI behavior, states, accessibility and
  design-system usage when the diff touches `Web`; brief it with the design artifact if one exists.
  Skip it for docs/comment-only edits, a single-constant change, or a compiler-verified rename.
- **`test-reviewer`**: a new or changed handler, validator, domain event, or boundary conversion
  (model to/from DTO or entity), or any change with real regression risk. Skip it when no behavior
  changed and no tests needed changing.
- **`security-reviewer`**: anything touching auth, authorization, claims/identity, secrets or
  configuration, sensitive data, externally supplied input, file upload, cryptography, or endpoint
  exposure. Skip it for ordinary CRUD that crosses no security boundary.

Once a PR is open, CI (`.github/workflows/ci.yml`: restore, `Verify-Package-Versions.ps1`,
`dotnet format --verify-no-changes`, build, test) and human review apply unchanged; the local
reviews supplement that pipeline, they don't replace it. This repo's CI does not currently run
static/security analysis (no SonarQube/CodeQL step configured); if one is added later, it
re-verifies what `security-reviewer` already checked, it doesn't substitute for it.

## Enabling this file

Custom instructions are off by default in Visual Studio. Enable them under **Tools → Options →
GitHub → Copilot → Copilot Chat → "Enable custom instructions to be loaded from
.github/copilot-instructions.md files and added to requests."**
