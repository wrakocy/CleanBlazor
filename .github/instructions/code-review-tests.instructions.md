---
description: PR review rules for the UnitTests, IntegrationTests, and FunctionalTests projects — naming, structure, and scope of each test kind. Derived from docs/04-testing-guide.md and the add-unit-test/add-integration-test/add-functional-test skills. See code-review-standards.instructions.md for the cross-cutting "does this PR need a test" rules.
applyTo: "*.UnitTests/**/*.cs,*.IntegrationTests/**/*.cs,*.FunctionalTests/**/*.cs"
---

# Code Review Standards — Tests

> Repository-specific review rules. GitHub Copilot Code Review applies this file automatically via
> its `applyTo` globs, and the AgentKit reviewer agents read it too. The organization-wide rules
> (severity tags, general conventions) are in the synced `code-review-kit-*.instructions.md` files
> beside it; change those in AgentKit, not here.

Severity tags `[Required]`/`[Suggested]` follow the same meaning as in
`code-review-kit-general.instructions.md`. This file covers the *shape* of tests already being
added; see `code-review-standards.instructions.md` for whether a PR needed a test at all. The
general xUnit/Moq rules (one test file per method, `readonly` mocks, Fact vs. Theory,
FluentValidation.TestHelper, unit-vs-integration placement) are in the synced
`code-review-kit-dotnet-tests` and `code-review-kit-general` files; this file keeps the rules that
name this repository's types and projects.

## Project boundaries

- **[Required]** Flag the creation of any test project other than the solution's single
  `UnitTests`, `IntegrationTests`, and `FunctionalTests` projects (e.g., a new per-source-project
  unit test project) — this repo has exactly one unit-test project mirroring source namespaces.
  (AGENTS.md MUST NOT; docs/04-testing-guide.md — "What NOT to do")

## Unit test structure

- **[Required]** Flag a new public method on a Core/Infrastructure class that guards its arguments
  (via `Throw`) with no corresponding test asserting the guard throws
  (`Assert.ThrowsAsync<ArgumentNullException>(...)` or equivalent). (docs/04-testing-guide.md —
  "Always test the Throw-guard path"; `add-unit-test` skill)
- **[Required]** Flag a test building a structured model via an inline object initializer
  (`new OrderModel { ... }` with several properties set) instead of via a
  `Builders/<Domain>/<Model>Builder.cs`, once that model has any real structure. If no builder
  exists yet for the model, flag the gap rather than the inline initializer being the fix — the
  correct fix is adding the builder. (docs/04-testing-guide.md; `add-unit-test` skill)

## Functional test structure

- **[Required]** Flag a functional test class named after its containing folder rather than the
  page it tests (e.g., a `Details` class inside a `Search` folder) — name the class after the
  page. This is a known slip called out explicitly in the playbook to not repeat.
  (AGENTS.md MUST NOT; docs/04-testing-guide.md; `add-functional-test` skill)
- **[Required]** Flag a functional test file that doesn't mirror the Blazor page's own folder path
  under `Web/Components/Pages/Areas/...` (functional tests use this mirrored-path convention, not
  the `<Class>_<Method>.cs` naming used elsewhere in `UnitTests`/`IntegrationTests`).
  (docs/04-testing-guide.md; `add-functional-test` skill)
- **[Required]** Flag a functional test that asserts on an internal route constant or a DI
  registration instead of the page's visible heading/title text (what a user would actually see).
  (docs/04-testing-guide.md — "Scope of assertion")
- **[Required]** Flag a new page with both a parameterized and parameterless route (e.g.,
  `/orders/details/{Id:int}` and `/orders/details`) that has only one functional test instead of
  one per route, using the `WithId`/`WithoutId` naming suffix. (`add-functional-test` skill)
- **[Suggested]** Flag a functional test that adds page-specific mocking instead of extending the
  shared `CustomWebApplicationFactory`/mock classes that already back every other functional test
  — per-test mocking isn't the established pattern here. (`add-functional-test` skill)
