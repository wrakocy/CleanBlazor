---
description: Bootstrap a brand-new solution from an empty repo following this Clean Architecture/CQRS playbook. Use once, at the very start of a new project — not for adding features to an existing solution.
applyTo: "**"
---

<!-- No natural file-glob trigger exists for a one-time bootstrap action — this instructions file
     applies broadly. Invoke it explicitly (ask Copilot Chat to "create the solution following
     create-solution.instructions.md") rather than expecting it to auto-trigger on an edit. -->

# Create a New Solution

Full detail lives in `docs/02-bootstrap-guide.md` — this file is the condensed checklist. Before
starting, confirm with the user (or infer from their request) three decisions that change what
gets built:

1. **Persistence**: external API client(s), a database via EF Core, or both.
2. **Company/Product name** — used for every namespace/project (`<Company>.<Product>.Core`, etc.).
3. Anything else in `README.md`'s "Decisions already made" section the user wants to override
   (auth provider, observability stack, UI framework).

## Steps

1. **Create the solution and folder layout** — `/src/` with `Core`/`Infrastructure`/`Web`
   projects, `/tests/` with `UnitTests`/`IntegrationTests`/`FunctionalTests`, plus `/.config/` for
   `Directory.Build.props`, `Directory.Packages.props`, `.editorconfig`, `nuget.config`. See
   bootstrap guide §1–2 for the exact layout and `.editorconfig` settings (file-scoped namespaces,
   `_camelCase` private fields).
2. **Core project**: `MediatR`, `FluentValidation`, `Throw`, `Serilog`, `Wrak.Extensions`. Write
   `CoreMarker.cs`, `GlobalUsings.cs`, and the foundational `Shared/` abstractions verbatim from
   bootstrap guide §3 — `CommandBase`/`CommandBase<T>`, `QueryBase<T>`, `ApplicationEventBase`,
   `ValidatorBase<T>`, `IAppBus`, `DomainExceptionBase`. If persistence includes an API, also add
   `ICacheService` and the relevant `I<System>Client` interface.
3. **Infrastructure project**: reference Core only. Write `InfrastructureMarker.cs` and `AppBus.cs`
   (the *only* class allowed to inject `IMediator`). Add the persistence-specific pieces from
   bootstrap guide §4 (EF Core `DbContext`/repositories, and/or an API client + `Filters/` +
   `CacheServiceBase<TSnapshot>` if API-backed).
4. **Web project**: reference Infrastructure only. Write `WebMarker.cs`, `Program.cs`, and
   `Extensions/DependencyInjectionExtensions.cs` following bootstrap guide §5 exactly, including
   the call order in `Program.cs` (it's order-dependent — Serilog after `AddWebServices`, OTel
   before Serilog, etc.). Wire auth as the swap-pattern placeholder from §6 (real scheme
   outside Development, a `DevelopmentAuthenticationHandler` inside it) — do not pick a concrete
   identity provider unless the user specifies one. Wire observability per §7
   (Serilog + OpenTelemetry + Azure Monitor, gated on a connection string being present).
5. **Test projects**: set up per bootstrap guide §8 — one `UnitTests` project (not one per source
   project), `IntegrationTests` and `FunctionalTests` both referencing it,
   `CustomWebApplicationFactory` + `WebApplicationTestFixtureBase` + a `Mocks/` folder for every
   external boundary.
6. **Verify** per bootstrap guide §9: `dotnet build`, `dotnet test`, `dotnet run` + `/health`
   returns 200, and confirm `Core.csproj` has zero `<ProjectReference>` entries.
7. **Fill in the new repo's agent instructions.** This playbook repo's own `AGENTS.md`,
   `CLAUDE.md`, `.github/copilot-instructions.md`, `.claude/skills/`, and `.github/instructions/`
   are already written to be copied as-is — just fill in the placeholders in `AGENTS.md` and
   `.github/copilot-instructions.md` (product name, persistence checkbox, one-line domain
   description) once, in both files, and keep them in sync going forward.

## Gotchas

- Don't create a `Core.UnitTests` or any other per-project test project — one `UnitTests` project
  mirroring namespaces is canonical; a second one is dead weight from day one.
- Don't add the cache-service pattern unless the solution actually includes an API-backed domain.
- Leave auth unimplemented (throw `NotImplementedException` in the production branch) rather than
  guessing a provider — that's an explicit placeholder, not an oversight.
