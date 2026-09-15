---
description: PR review rules for security-sensitive changes — authentication, authorization, identity/claims, secrets, sensitive data, externally supplied input, file upload, cryptography, and API/endpoint exposure. Applies only when a change touches one of these boundaries; most CRUD changes will not match. See code-review-standards.instructions.md for repo-wide rules.
applyTo: "*.Web/**/*.cs,*.Web/**/*.razor,*.Infrastructure/**/*.cs,*.Core/**/*.cs,*/appsettings*.json"
---

# Code Review Standards — Security

> Shared review standard — no `.claude/` counterpart. GitHub Copilot Code Review applies this
> file automatically via its `applyTo` globs, and the Claude Code reviewer agents in
> [.claude/agents/](../../.claude/agents/) are pointed at this same file. Edit it here only;
> don't fork a Claude-side copy.

Severity tags `[Required]`/`[Suggested]` follow the same meaning as in
`code-review-standards.instructions.md`. Unlike the other layer-specific files, this file's rules
apply only when the diff actually touches a security-sensitive concern — don't force-fit these
onto an ordinary CRUD change with no such boundary. This is the primary checklist for the
`security-reviewer` agent, invoked conditionally per AGENTS.md's PR Workflow.

This solution is a Blazor Server app authenticating via `Microsoft.Identity.Web`/OpenID Connect
with cookie-based auth, and uses ASP.NET Core `DataProtection` for anything requiring encryption
at rest in transit between server and client state.

## Authentication and authorization

- **[Required]** Flag a new page, route, or `IAppBus` operation exposing data or a state change
  that isn't gated the same way comparable existing ones are (an `[Authorize]` attribute or
  policy, or an explicit claim check via `ClaimsPrincipalExtensions` such as `IsAdmin`/`IsUser`).
- **[Required]** Flag an authorization decision made only in the UI (e.g., hiding a button) with
  no corresponding server-side check in the handler/component that performs the actual action —
  a hidden button is not an authorization boundary.
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
  per this repo's existing `azure-pipelines-*.yml` `AzureKeyVault@2`/`ReplaceTokens@6` pattern).
- **[Required]** Flag a fallback default for a secret-bearing configuration value that would
  silently function in a non-Development environment (masks a missing Key Vault binding instead of
  failing fast).
- **[Suggested]** Flag a secret or token value included in a log statement (Serilog) or exception
  message that could propagate to a sink/Application Insights.

## Sensitive data

- **[Suggested]** Flag PII or another sensitive field logged at a level enabled in
  Production/Test (check the relevant `appsettings.*.json` Serilog `MinimumLevel`), added to a
  DTO/API response beyond what the consuming feature needs, or cached in an in-memory
  `CacheServiceBase<TSnapshot>` longer than the feature requires.

## Externally supplied input

- **[Required]** Flag any raw SQL/dynamic query built via string concatenation or interpolation
  from user input, or a deserialization call (`Newtonsoft.Json` or equivalent) applied to
  untrusted external content without a constrained type/settings.
- **[Required]** Flag reliance on client-side/FluentValidation-only checks for something that also
  needs a server-side guard against a malicious (not just careless) caller — e.g., an ID used to
  fetch another user's record without an ownership/authorization check server-side.

## File upload

- **[Required]** Flag file-upload handling that trusts the client-supplied filename/extension or
  `Content-Type` for deciding what the file is, instead of validating actual content/size
  server-side, or that writes an uploaded file to a path built from unsanitized user input (path
  traversal).

## Cryptography

- **[Required]** Flag hand-rolled cryptography, a weak/deprecated algorithm or mode, or a
  hardcoded key/IV, where `System.Security.Cryptography` and this repo's `DataProtection` setup
  already provide a vetted path.

## API and endpoint exposure

- **[Required]** Flag a new minimal-API endpoint or a changed CORS/cookie/`DataProtection` setting
  that widens the app's attack surface (e.g., a permissive CORS policy, a cookie without
  `HttpOnly`/`Secure`/appropriate `SameSite`) without a stated reason.
- **[Suggested]** Flag an error path that returns a stack trace or other internal detail to the
  client outside `Development` (check `app.UseDeveloperExceptionPage()`/equivalent is properly
  gated by environment).
