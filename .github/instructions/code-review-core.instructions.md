---
description: PR review rules for the Core project — CQRS use cases, domain events, validators, and models. Derived from docs/03-feature-development-guide.md and the add-cqrs-feature/add-domain-event/add-fluentvalidation-validator skills. See code-review-standards.instructions.md for repo-wide rules (dependency direction, DTO leakage, etc.).
applyTo: "*.Core/**/*.cs"
---

# Code Review Standards — Core

Severity tags `[Required]`/`[Suggested]` follow the same meaning as in
`code-review-standards.instructions.md`.

## Vertical-slice structure

- **[Required]** Flag a new use case that isn't organized as
  `Core/<Domain>/Features/<UseCaseName>/` containing exactly a request class and a handler for
  that one use case. Flag a `Features/` folder containing more than one use case's worth of
  request/handler pairs, or a shared "God handler" servicing multiple commands.
  (docs/03-feature-development-guide.md — "Domain folder shape"; `add-cqrs-feature` skill)
- **[Suggested]** Flag inconsistent placement when a similar domain elsewhere in the repo already
  established a folder shape (e.g., `Events/<EventName>/`, `Models/<ModelName>/`) that the new
  domain doesn't follow without reason.

## Commands and Queries

- **[Required]** Flag a command/query request class whose constructor does anything other than
  null-guard its arguments via `Throw` (`.ThrowIfNull().Value`) — no business-rule checks, no
  derived-value computation beyond simple assignment. (AGENTS.md MUST; `add-cqrs-feature` skill)
- **[Required]** Flag a handler that doesn't guard its constructor-injected dependencies with
  `.ThrowIfNull().Value`, or that doesn't guard the incoming request and its required members at
  the top of `Handle`. (AGENTS.md MUST)
- **[Suggested]** Flag a command that returns no data using `CommandBase<TResponse>` when
  `CommandBase` (no return value) would do, or vice versa — a create/update should generally
  return the saved model via `CommandBase<TResponse>`. (docs/03-feature-development-guide.md §2)
- **[Suggested]** Flag a query with required constructor arguments when the established pattern
  for filter/search queries in this repo is plain settable properties (optionally with a
  `MaxItems` constant) and no required constructor. (docs/03-feature-development-guide.md §3)

## Domain events

- **[Required]** Flag a domain event published *before* the state-changing call it represents has
  succeeded, rather than after. (AGENTS.md MUST; docs/03-feature-development-guide.md §4)
- **[Suggested]** Flag a new `INotificationHandler<TEvent>` added purely for symmetry with other
  domains when nothing in the handler body actually reacts to anything (e.g., an empty handler, or
  one that only logs). A DB-backed domain with no in-memory cache and no other reactive concern
  often doesn't need an event handler at all. (docs/03-feature-development-guide.md §4 —
  "Only write a handler if something actually needs to react")
- **[Required]** Flag manual DI registration of an `INotificationHandler<T>` or
  `IRequestHandler<TRequest, TResponse>` — these are auto-discovered by MediatR from the
  `CoreMarker` assembly and registering them manually is redundant. (`add-domain-event`,
  `add-cqrs-feature` skills)

## Validation

- **[Required]** Flag a validator that extends `AbstractValidator<TModel>` directly instead of
  this repo's `ValidatorBase<TModel>`. (`add-fluentvalidation-validator` skill)
- **[Required]** Flag manual DI registration of a `FluentValidation` validator — validators in the
  Core assembly are auto-registered via `AddValidatorsFromAssemblyContaining<CoreMarker>`.
  (`add-fluentvalidation-validator` skill)
- **[Suggested]** Flag a model with clear cross-field or business-rule constraints (e.g., a
  required-if-X field, a max-count collection rule, a uniqueness rule) that has no validator at
  all, when sibling models of similar complexity in the same domain area do have one. Compare
  against the nearest existing model before flagging — not every model needs a validator.
  (docs/03-feature-development-guide.md §5)
- **[Suggested]** Flag a validator missing `.Cascade(CascadeMode.Stop)` where a later rule clearly
  assumes an earlier one already passed (e.g., indexing into a collection a prior rule should have
  guaranteed is non-empty). (`add-fluentvalidation-validator` skill)

## Models

- **[Required]** Flag a Core model with business-logic methods (calculations, side-effecting
  behavior) on it — models in this codebase are intentionally anemic; behavior belongs in
  handlers, factories, conversion services, and validators. (docs/03-feature-development-guide.md
  §1 — "Models are anemic POCOs")
- **[Suggested]** Flag a "create new" code path that constructs a model with `new SomeModel()`
  directly instead of through an `ISomeModelFactory.BuildWithDefaultValues()` when a factory
  already exists for that model. (docs/03-feature-development-guide.md §1)
- **[Suggested]** Flag a hand-computed "has this changed" diff (comparing current vs. original
  values) written in a handler or in Web, when the model itself could expose it as a computed
  property comparing to an `Original...` value captured at load time.
  (docs/03-feature-development-guide.md §1)

## Naming (API-backed domains)

- **[Suggested]** Flag conversion-service naming/direction that doesn't match this repo's
  convention: `I<Entity>ModelConversionService.ToDto(model)` converts a Model to a Dto (named for
  its input); `I<Entity>DtoConversionService.ToModel(dto)` converts a Dto to a Model (also named
  for its input). A conversion service whose name and direction are reversed from this is worth a
  comment even though it may still compile and work correctly.
  (docs/01-architecture-overview.md — "Naming for conversion services")
