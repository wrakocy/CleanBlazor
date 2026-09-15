---
description: PR review rules for the Web project — Blazor Server page/component structure, data-access boundaries, shared-component reuse, state-management consistency, user-facing behavior/accessibility, and component test coverage. Derived from docs/03-feature-development-guide.md §7 and the add-blazor-page skill. See code-review-standards.instructions.md for repo-wide rules.
applyTo: "*.Web/**/*.razor,*.Web/**/*.cs"
---

# Code Review Standards — Web / Blazor

Severity tags `[Required]`/`[Suggested]` follow the same meaning as in
`code-review-standards.instructions.md`. For all "compare to neighboring/sibling" rules below,
check the nearest existing page/component of the same kind (search page, detail page, shared
widget) before flagging — the point is consistency with what's already in the repo, not an
abstract ideal.

## Component structure

- **[Required]** Flag a new page's code-behind class that doesn't inherit `AppComponentBase` —
  this is where `_navManager`, `_userContext`, `_appState`, `_appBus`, `_dialogService`, and
  `_snackBarService` come from, and skipping it invites re-injecting those individually or
  reaching around them. (`add-blazor-page` skill)
- **[Suggested]** Flag non-trivial logic (more than simple parameter passthrough or a one-line
  computed value) written directly in a `.razor` file's `@code { }` block instead of in a paired
  `<Name>.razor.cs` code-behind class, when sibling pages in the same area use the code-behind
  pairing. (`add-blazor-page` skill)
- **[Required]** Flag a Blazor `[Inject]` property that isn't named `_camelCase` — this repo
  extends its private-field naming convention to injected fields by convention (`.editorconfig`
  only covers `private` fields, so this needs a human/reviewer check). (`add-blazor-page` skill)

## Business logic belongs in code-behind, not markup

- **[Required]** Flag non-trivial conditional or computed logic written inline in a `.razor`
  file's markup or inline expressions (multi-condition boolean logic, cross-field comparisons,
  formatting/derivation beyond simple display binding) when a paired code-behind class exists —
  that logic belongs on the code-behind as a `protected` property or method (see how
  `FiltersTitle`, `ResultsTitle`, and similar computed display strings are exposed as code-behind
  properties in this repo's existing search pages), not embedded in the `.razor` file.
  (docs/03-feature-development-guide.md §7; `add-blazor-page` skill)

## Data-access boundary

- **[Required]** Flag a component or its code-behind that injects a repository interface
  (`I<Entity>Repository`), an API client interface (`I<System>Client`), or `IMediator` directly
  instead of fetching/sending data exclusively through `_appBus.Send(...)`/`_appBus.Publish(...)`.
  (AGENTS.md MUST; `add-blazor-page` skill)

## Shared component reuse

- **[Suggested]** Flag a new component that duplicates the responsibility of an existing shared
  component (e.g., a loading indicator instead of `Progress`/`ProgressOverlay`, a page-title/
  breadcrumb block instead of `PageHeader`, a large-result-set warning instead of
  `LargeResultSetAlert`, a lookup-backed multiselect instead of the existing list-of-values-backed
  select component) — check `Components/Forms`/`Components/Widgets` (or wherever this repo's
  shared components live) for an existing match before approving a new one.
  (docs/03-feature-development-guide.md §7)

## State-management consistency

- **[Required]** Flag an async call through `_appBus` that hits the backing store without the
  working-spinner pattern (`_appState.Working = true` → `await ForceComponentRefreshAsync()` →
  await the call → `_appState.Working = false`) when neighboring pages performing a similar call
  use that pattern — an inconsistent page will not show a loading indicator where users expect
  one. (docs/03-feature-development-guide.md §7)
- **[Suggested]** Flag a search/list page that persists or restores its query state through a
  mechanism other than `ILocalStorageService` keyed by a page-specific string constant (e.g.,
  `"orders.search.query"`), restored in `OnAfterRenderAsync(firstRender: true)`, when sibling
  search pages in the repo use that mechanism. (docs/03-feature-development-guide.md §7)
- **[Suggested]** Flag a hardcoded breadcrumb, icon, page-size, or input-mask string when an
  equivalent constant already exists in `AppConstants`. (docs/03-feature-development-guide.md §7)

## User-facing behavior and accessibility

These rules are the primary remit of the `ui-reviewer` agent (invoked when a change materially
affects UI behavior, layout, interaction, accessibility, or design-system usage) but apply
whenever this file is in scope, including for a general code review of a Blazor change.

- **[Required]** Flag a page/component that performs an async `_appBus` call without visible
  loading feedback (the `_appState.Working` spinner pattern, or an equivalent already established
  on comparable pages) — see "State-management consistency" above; this is called out again here
  because a missing loading state is a user-facing defect, not just a convention slip.
- **[Required]** Flag a form/page where FluentValidation errors aren't surfaced to the user (no
  `Blazored.FluentValidation` integration or equivalent field-level message) — a validation rule
  that exists but is never shown provides no actual protection to the user.
- **[Required]** Flag a page/component with no handling for an empty result set (a search/list
  page that renders nothing rather than an explicit "no results" state) or an error from a failed
  `_appBus` call (silently swallowed instead of a `_snackBarService` notification or equivalent),
  when comparable existing pages do handle these states.
- **[Required]** Flag a non-decorative `<img>`/icon-only interactive control with no accessible
  name (missing `alt`, `aria-label`, or MudBlazor's equivalent prop), and a custom interactive
  element (a `<div>`/`<span>` with a click handler) that isn't keyboard-operable when a native
  `MudButton`/`<button>` would have worked.
- **[Suggested]** Flag a new layout that hard-codes a fixed pixel width or otherwise won't reflow
  at a narrow viewport, when comparable existing pages use MudBlazor's responsive grid/breakpoint
  props instead.
- **[Suggested]** Flag custom (non-MudBlazor-default) text/background color combinations that look
  low-contrast, or a dialog/modal that doesn't return focus sensibly after it closes.
- **[Suggested]** When the PR description or task links a design artifact (Figma/Zeplin), flag an
  implementation that visibly diverges from it (spacing, missing state such as hover/disabled) —
  the `ui-reviewer` agent should be given the artifact directly rather than asked to infer it.

## Component test coverage

- **[Suggested]** Flag a new or meaningfully changed component with non-trivial behavior
  (conditional rendering branches, an event handler containing logic, a computed display value
  derived from more than one input) that has no accompanying test under `UnitTests/Web/...`, when
  comparable existing components in the repo do have either a bUnit test or a plain code-behind
  logic test. The testing guide accepts either form of coverage for component behavior — this is
  a coverage-gap flag, not a requirement to specifically add bUnit if the repo's established
  pattern for similar components is a plain code-behind logic test instead.
  (docs/04-testing-guide.md — "Functional tests... don't assert on grid contents or exercise
  button clicks/interactivity — that's for component tests (bUnit) or manual/browser
  verification"; `add-blazor-page` skill step 8)
- **[Required]** Flag a `FunctionalTests` test that asserts on grid contents, simulates button
  clicks, or otherwise exercises component interactivity — functional tests in this repo are
  scoped to "does the route resolve and render," asserting only status code plus visible
  heading/title text. Interactivity belongs in a component test instead.
  (docs/04-testing-guide.md — "Scope of assertion")
