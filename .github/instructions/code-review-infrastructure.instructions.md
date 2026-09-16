---
description: PR review rules for the Infrastructure project — persistence (EF Core and/or external API), the cache-service pattern, and cross-cutting filters. Derived from docs/01-architecture-overview.md, docs/02-bootstrap-guide.md §4, and the add-persistence-integration skill. See code-review-standards.instructions.md for repo-wide rules.
applyTo: "*.Infrastructure/**/*.cs"
---

# Code Review Standards — Infrastructure

> Shared review standard — no `.claude/` counterpart. GitHub Copilot Code Review applies this
> file automatically via its `applyTo` globs, and the Claude Code reviewer agents in
> [.claude/agents/](../../.claude/agents/) are pointed at this same file. Edit it here only;
> don't fork a Claude-side copy.

Severity tags `[Required]`/`[Suggested]` follow the same meaning as in
`code-review-standards.instructions.md`. Which sub-rules apply depends on this solution's
persistence model (declared in `AGENTS.md`/`copilot-instructions.md`'s "Persistence model"
section) — check that before flagging a Path A/B mismatch.

## AppBus exclusivity

- **[Required]** Flag any class in `Infrastructure` other than the `AppBus` implementation that
  injects `IMediator`. `AppBus` is the only class in the entire solution allowed to do so.
  (AGENTS.md MUST; docs/02-bootstrap-guide.md §4)

## Database-backed domains (EF Core)

- **[Required]** Flag a handler or any code outside `Infrastructure` that references an EF Core
  entity type or the `DbContext` directly — access must go through an `I<Entity>Repository`
  interface declared in Core, with the repository translating to/from Core `Model` types.
  (docs/03-feature-development-guide.md §6; `add-persistence-integration` skill)
- **[Suggested]** Flag a new generic `IRepository<T>` introduced for a single domain's needs when
  the established pattern in this repo is one purpose-built `I<Entity>Repository` per aggregate,
  named for the operations its handlers actually need. (`add-persistence-integration` skill)
- **[Suggested]** Flag entity configuration via data-annotation attributes on the entity class when
  the established pattern is `IEntityTypeConfiguration<T>` classes kept alongside the
  `DbContext`. (`add-persistence-integration` skill)
- **[Required]** Flag a repository registered as anything other than `Scoped` (it should match the
  `DbContext`'s lifetime), and flag manual `DbContext` registration outside the one
  `AddPersistence`/`AddInfrastructureServices` call. (docs/02-bootstrap-guide.md §5)
- **[Suggested]** Flag an EF Core migration whose diff touches substantially more than the schema
  change described in the PR — ask for a re-check that nothing unintended was regenerated.
  (`add-persistence-integration` skill, by analogy with the same guidance for generated API
  clients)

## API-backed domains

- **[Required]** Flag any hand-edit to a generated API client file or its generated DTOs (e.g., an
  NSwag-generated `ApiClient.cs` or its interface) — regenerate from the spec instead, and put
  hand-written glue in a separate `partial class` file under a `Partials/` folder.
  (AGENTS.md MUST; `add-persistence-integration` skill Path B)
- **[Required]** Flag a DTO or generated client type appearing in a handler's public signature or
  anywhere outside the immediate conversion-service/handler boundary call.
  (docs/01-architecture-overview.md; `add-persistence-integration` skill)
- **[Suggested]** Flag a new typed `HttpClient` registration for an external system that omits the
  cross-cutting `Filters/` delegating handlers (headers/logging) that sibling API clients in this
  solution register — compare against an existing `AddHttpClient<T>(...)` registration before
  flagging. (docs/02-bootstrap-guide.md §4–5)
- **[Required]** Flag introduction of the read-side cache-service pattern
  (`I<Domain>CacheService : ICacheService` / `<Domain>CacheService : CacheServiceBase<TSnapshot>`)
  for a domain that isn't backed by a slow/rate-limited external API. (AGENTS.md SHOULD;
  docs/01-architecture-overview.md; `add-persistence-integration` skill — "Gotchas")
- **[Required]** Flag a new `I<Domain>CacheService` implementation registered as anything other
  than `Singleton` — cache services are shared across every Blazor circuit and must never be
  `Scoped`/`Transient`. (AGENTS.md MUST; `add-persistence-integration` skill)
- **[Suggested]** Flag a new cache service that isn't wired into the app's cache-initialization
  step (called once from `Program.cs` after `app.Build()`), which would leave it empty until its
  first timer tick instead of populated before the app starts serving traffic.
  (docs/02-bootstrap-guide.md §5, §9)

## Mixing persistence shapes

- **[Suggested]** Flag a single domain whose handlers mix both a repository (Path A) and DTO
  conversion services (Path B) without a stated reason — pick one backing store per domain.
  (`add-persistence-integration` skill — "Gotchas"; docs/03-feature-development-guide.md §6 —
  "[Both]")

## Naming

- **[Suggested]** Flag an interface named generically (e.g., a solution-wide `IApiClient` when
  more than one external system exists) instead of being named for the real system it represents
  (e.g., `IBillingApiClient`). (docs/02-bootstrap-guide.md §3)
