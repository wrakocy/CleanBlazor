---
name: verify-and-review
description: The pre-PR procedure for this repo — deterministic verification ladder, self-diff review, and which specialist reviewer (code, test, security) to invoke for a given change. Use once an implementation (and its tests) are locally complete, before opening a PR.
---

# Verify and Review

This is the orchestration layer for AGENTS.md's PR Workflow. It exists so that specialist
reviewers are invoked deliberately, at real risk boundaries, instead of by default — see
"Risk-based reviewer selection" below before spawning anything.

## 1. Deterministic verification

Run these yourself; none of them need an LLM:

```bash
dotnet format Wrak.Clean.Blazor.slnx
dotnet restore Wrak.Clean.Blazor.slnx
pwsh ./Verify-Package-Versions.ps1
dotnet build Wrak.Clean.Blazor.slnx
dotnet test Wrak.Clean.Blazor.slnx
```

- While iterating, scope `dotnet test` to the project or class you're touching
  (`dotnet test Wrak.Clean.Blazor.UnitTests --filter ...`) rather than the whole solution; run the
  full solution-wide `dotnet test` once before opening the PR.
- `Verify-Package-Versions.ps1` enforces the pinned `MediatR` 12.x version — CI runs it too, so
  catching a violation locally saves a CI round-trip.
- If `dotnet format` changes anything, re-run `dotnet build`/`dotnet test` after — don't assume a
  formatting pass is a no-op for compilation.

## 2. Review your own diff

Before invoking any specialist agent, read `git diff` (against `main`'s merge-base, or `HEAD` for
uncommitted work) yourself. Catching the obvious issues here is cheaper than paying a reviewer
agent to find them. This also determines which reviewers below are actually warranted — you can't
route correctly without having looked at what changed.

## 3. Risk-based reviewer selection

Invoke only the reviewers whose trigger condition the diff actually meets. Do not invoke a
reviewer "to be safe" — an unconditional review on every change is exactly the cost/quality
tradeoff this skill exists to avoid.

| Reviewer | Invoke when the diff includes... | Skip when |
| --- | --- | --- |
| `code-reviewer` | A new/changed use case, handler, component, Infrastructure integration, or cross-cutting wiring — any change where a correctness or architecture-boundary mistake is plausible | Docs/comment-only edits, a single-constant change, a rename already verified by the compiler |
| `test-reviewer` | Behavior materially changed, or the change carries real regression risk (new/changed handler, validator, domain event, boundary conversion) | No behavior changed and no tests needed changing |
| `security-reviewer` | Auth, authorization, claims/identity, secrets/security-sensitive config, sensitive data, externally supplied input, file upload, cryptography, or endpoint/permission exposure | Ordinary CRUD with no security boundary crossed |

**UI review is part of `code-reviewer`, not a separate agent.** When the diff touches Blazor UI
behavior, states, validation presentation, accessibility, responsive layout, or design-system
usage, `code-reviewer` applies its "when the diff touches Blazor UI" section — so say so in the
briefing, and attach any design artifact (Figma/Zeplin link, screenshot) it should compare
against. A second agent re-reading the same diff for the UI slice would duplicate most of the
first one's context for little added independence.

A single change can warrant more than one reviewer (e.g., a new authenticated Blazor page warrants
`code-reviewer` — with the UI section in play — plus `security-reviewer`). It's also normal for a
change to warrant none of them (a pure refactor with full test coverage and no behavior change
might only need steps 1–2 above).

`code-reviewer` and `test-reviewer` are the common case for any non-trivial implementation change;
`security-reviewer` is boundary-specific and should be the exception, not the default.

## 4. Brief the reviewer — don't hand it the repo

Each reviewer agent already knows this repo's architecture and conventions from its own
system prompt and the `.github/instructions/code-review-*.instructions.md` files it's told to
read — don't restate those. What the reviewer needs *from you* is the specific change:

- The task/acceptance criteria in a sentence or two.
- The diff range (`git diff <merge-base>...HEAD`, or note if it's uncommitted).
- The list of changed files.
- Any of the small set of supporting files that give necessary context but aren't in the diff
  (e.g., the interface a new handler implements, a sibling page being mirrored) — only if the
  reviewer would otherwise have to guess or search blindly for them.

Do not tell a reviewer to "review the whole repo" or re-derive architecture that's already
documented — that's wasted context on both sides.

## 5. Address findings and re-verify

- Resolve the findings you agree with. Reviewers are read-only and never edit anything themselves.
- Re-run only the deterministic checks affected by the fix (e.g., `dotnet test` on the touched
  project, not necessarily the full solution).
- Re-invoke a reviewer only if the fix changed the risk area that reviewer examined — a trivial
  follow-up fix (renaming a variable the reviewer flagged, adding one missing test) doesn't need a
  fresh full review pass.

## 6. Open the PR

CI (`azure-pipelines-*.yml`: restore, `Verify-Package-Versions.ps1`, build, test) and human review
apply unchanged — this skill's local verification and specialist reviews supplement that
pipeline, they don't replace it. This repo's CI does not currently run static/security analysis
(no SonarQube/CodeQL step configured); if one is added later, it re-verifies what the local
`security-reviewer` pass already checked, it doesn't substitute for it.
