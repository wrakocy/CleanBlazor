---
description: PR review rules for security-sensitive changes — authentication, authorization, identity/claims, secrets, sensitive data, externally supplied input, file upload, cryptography, and API/endpoint exposure. Applies only when a change touches one of these boundaries; most CRUD changes will not match. See code-review-standards.instructions.md for repo-wide rules.
applyTo: "*.Web/**/*.cs,*.Web/**/*.razor,*.Infrastructure/**/*.cs,*.Core/**/*.cs,*/appsettings*.json"
---

# Code Review Standards — Security

> Repository-specific review rules. GitHub Copilot Code Review applies this file automatically via
> its `applyTo` globs, and the AgentKit reviewer agents read it too. The organization-wide rules
> (severity tags, general conventions) are in the synced `code-review-kit-*.instructions.md` files
> beside it; change those in AgentKit, not here.

Severity tags `[Required]`/`[Suggested]` follow the same meaning as in
`code-review-kit-general.instructions.md`. Unlike the other layer-specific files, this file's rules
apply only when the diff actually touches a security-sensitive concern — don't force-fit these
onto an ordinary CRUD change with no such boundary. This file holds only this repository's own
security rules. The generic and ASP.NET Core forms of the other categories (externally supplied
input, file upload, cryptography, cookies/CORS/antiforgery, error details, secret fallbacks, UI-only
authorization) are in the synced `code-review-kit-security` and `code-review-kit-dotnet-security`
files. Together they are the checklist for the `security-reviewer` agent, invoked conditionally per
AGENTS.md's PR Workflow.

This solution is a Blazor Server app authenticating via `Microsoft.Identity.Web`/OpenID Connect
with cookie-based auth, and uses ASP.NET Core `DataProtection` for anything requiring encryption
at rest in transit between server and client state.

## Authentication and authorization

- **[Required]** Flag a new page, route, or `IAppBus` operation exposing data or a state change
  that isn't gated the same way comparable existing ones are (an `[Authorize]` attribute or
  policy, or an explicit claim check via `ClaimsPrincipalExtensions` such as `IsAdmin`/`IsUser`).
- **[Required]** Flag any code path that could make `Filters/DevelopmentAuthHandler.cs` (or an
  equivalent development-only auth bypass) reachable outside the `Development` environment — check
  the registration in `DependencyInjectionExtensions.cs` is still conditioned on
  `IsDevelopment()`/`AppEnvironment.Is(...)`.

## Identity and claims

- **[Required]** Flag a claim read (`ClaimsPrincipalExtensions`, `AppIdentity`,
  `IUserContextService`) with no handling for the claim being absent, when the result feeds an
  authorization decision or a data query.
- **[Required]** Flag any code that derives "who is this request for" from client-supplied input
  (a query parameter, hidden form field, request body) instead of the authenticated
  `ClaimsPrincipal`/`IUserContextService`.

## Secrets and sensitive configuration

- **[Required]** Flag any secret, connection string, API key, or credential committed as a literal
  in source or in any `appsettings*.json` file (as opposed to Key Vault/environment/user-secrets,
  per this repo's `.github/workflows/deploy.yml`, which reads Key Vault secrets at deploy time and
  applies them as App Service app settings — no `#{token}#` placeholders in `appsettings*.json`).

## Sensitive data

- **[Suggested]** Flag PII or another sensitive field logged at a level enabled in
  Production/Test (check the relevant `appsettings.*.json` Serilog `MinimumLevel`), added to a
  DTO/API response beyond what the consuming feature needs, or cached in an in-memory
  `CacheServiceBase<TSnapshot>` longer than the feature requires.
