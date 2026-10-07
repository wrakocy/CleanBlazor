# Using This Repo as a `dotnet new` Template

This repo doubles as an installable `dotnet new` template (`.template.config/template.json`,
identity `Wrak.CleanBlazor.Template`, short name `blazor-clean`). Use this path when you
want a working, buildable solution under your own name in one command, instead of following
[02-bootstrap-guide.md](02-bootstrap-guide.md) step by step.

## Prerequisites

- .NET 10 SDK (`dotnet --version` → `10.x`) — every generated project targets `net10.0`.
- A local clone of this repo. `dotnet new install` needs a folder path, a `.nupkg`, or a NuGet
  package ID; it does not install directly from a git URL, so clone first.

## Install the template

From a clone of this repo:

```bash
dotnet new install .
```

Confirm it's registered:

```bash
dotnet new list blazor-clean
```

You should see one row: **ASP.NET Core with Blazor and Clean Architecture** / `blazor-clean`.

Installing by local path is a **live reference, not a snapshot** — `dotnet new` reads
`.template.config/template.json` and the working tree fresh on every invocation. If you edit this
repo (add a skill, change a bootstrap-guide section) the next `dotnet new blazor-clean` picks it
up immediately; you do not need to reinstall. This also means switching branches or having
uncommitted changes in this clone changes what the next generation produces.

## Generate a new solution

```bash
dotnet new blazor-clean -n <Company>.<Product> -o <path-to-new-solution>
```

- `-n`/`--name` is the **sourceName replacement** — every occurrence of the literal string
  `Wrak.CleanBlazor` becomes your `<Company>.<Product>` value, and every file/folder whose
  name contains `Wrak.CleanBlazor` is renamed to match (see below).
- `-o`/`--output` is optional and defaults to the current directory (`preferNameDirectory` is
  `false` in `template.json`) — `dotnet new` does **not** create a `<Company>.<Product>`
  subdirectory on its own. `cd` into (or create) the folder you want the new solution in first,
  then run the command with no `-o`; or pass `-o <path>` to target a different directory without
  `cd`-ing there.
- The target directory does not need to be empty, but an existing file with the same relative path
  as a generated one will cause the command to fail rather than overwrite it — generate into a
  fresh directory.

Example — generate into an existing empty directory:

```bash
mkdir ../Acme.Billing && cd ../Acme.Billing
dotnet new blazor-clean -n Acme.Billing
dotnet build Acme.Billing.slnx
```

Or in one step from anywhere, with `-o`:

```bash
dotnet new blazor-clean -n Acme.Billing -o ../Acme.Billing
```

The generated solution builds standalone — no reference back to this repo or its NuGet feed is
left behind.

## What gets renamed automatically

`dotnet new`'s default text-substitution behavior — **not** the `templateFileExtensions` entry in
`template.json`, which does not scope or limit which files get processed — applies to every file
in the template except the binary/verbatim types listed in `template.json`'s `copyOnly` modifier
(`*.dll`, `*.png`, `*.ico`, `*.jpg`, `*.jpeg`, `*.ps1`). In practice that means the
`Wrak.CleanBlazor` → `<Company>.<Product>` replacement runs across essentially every text
file in the repo, including ones you might not expect:

| Renamed automatically | Untouched |
| --- | --- |
| Project folder names (`Wrak.CleanBlazor.Core` → `<Company>.<Product>.Core`, etc.) | `Wrak.Extensions` / `Wrak.RandomData` NuGet package references — these are real published package IDs, not the template's sourceName, so they don't match and are left alone |
| `.csproj` / `.slnx` file names and their internal `<ProjectReference>`/`<File Path>` entries | `.template.config/` itself — excluded from `sources.include`, never present in generated output |
| C# namespaces, `global using` statements, and any other `Wrak.CleanBlazor` text inside `.cs`, `.razor`, `.yml` files | Anything under `[Bb]in/`, `[Oo]bj/`, `.vs/`, `.git/` — excluded by `sources.exclude` |
| Every `Wrak.CleanBlazor` occurrence inside `.md` files too — including `AGENTS.md` and `.github/copilot-instructions.md`'s "this describes the `Wrak.CleanBlazor` template" comment, project-map table, and `dotnet build`/`dotnet test` command examples | Prose that never contained the literal string in the first place — `README.md`'s current wording is already name-agnostic ("this template", "this solution"), so nothing changes there even though the substitution engine still runs over it |

Verify this yourself any time by generating into a scratch folder and diffing against this repo,
or `grep -r Wrak.CleanBlazor <generated-folder>` — a clean generation returns no matches.

## What still needs manual editing

The rename above is purely textual; it does not know anything about your actual domain. After
generating, still do this by hand — the HTML comments already sitting in `AGENTS.md` and
`.github/copilot-instructions.md` call out exactly this:

- **`AGENTS.md` / `.github/copilot-instructions.md`** — the name is already correct, but the
  persistence-model checkbox (`[ ]` API / DB / Both) and the one-line domain description are
  template placeholders, not sourceName tokens. Fill both in, in both files, and keep them in
  sync going forward.
- **`README.md`** — carried over verbatim, still written from the template's own point of view
  ("this solution has no business domain of its own yet", the `dotnet new install .` instructions
  you just followed). Replace it with a README that describes your actual product; nothing in the
  generation step does this for you.
- **Auth** (`AddAuthentication` in `Program.cs`/`DependencyInjectionExtensions.cs`) — generated
  with a `NotImplementedException` placeholder outside `Development`, by design. See
  [02-bootstrap-guide.md §6](02-bootstrap-guide.md).
- **`.template.config/`** is stripped from every generation, so the new solution is not itself
  reinstallable as a template — that's expected; re-templating a generated solution isn't a
  supported workflow here.
- Everything under `docs/`, `.claude/skills/`, `.github/instructions/` carries over as
  general-purpose guidance (with the name already substituted) and needs no edits unless you want
  to prune guidance that doesn't apply to your chosen persistence shape.

## Turning on deployments

A generated project ships with `.github/workflows/ci.yml` (active immediately) and
`.github/workflows/deploy.yml` (inert: every job requires the repository variable
`DEPLOY_ENABLED` to be `true`). Deployment targets Azure App Service using OIDC, so no Azure
password is stored. To turn it on:

1. **Create GitHub Environments** `Test` and `Production` (Settings → Environments). Give
   **both** a deployment branch policy limited to `main` — the federated credential subject
   (step 2) doesn't pin a branch or workflow file, so this policy is what stops a feature-branch
   workflow from requesting `environment: Test`/`Production` and getting an Azure token. (Automatic
   deploys run from `main`, so they're unaffected.) On `Production`, also add required reviewers
   and enable "Prevent self-review"; reviewers should check the CI run id being deployed.
2. **Create an Entra app registration** (or reuse one) per environment with access to that
   environment's Web App (e.g. Website Contributor: deploy + app settings) and Key Vault (secret
   read, e.g. Key Vault Secrets User), and add a **federated credential** to each,
   issuer `https://token.actions.githubusercontent.com`, with subject
   `repo:<owner>/<repo>:environment:Test` / `repo:<owner>/<repo>:environment:Production`.
3. **Create the Key Vault secrets** in each environment's vault: `azure-ad-tenant-id`,
   `azure-ad-client-id`, `azure-ad-client-secret`, and `app-insights-connection-string` (the full
   `InstrumentationKey=...;IngestionEndpoint=...` connection string).
4. **Set environment variables** `KEY_VAULT_NAME`, `WEBAPP_NAME`, `RESOURCE_GROUP` and
   **environment secrets** `AZURE_CLIENT_ID`, `AZURE_TENANT_ID`, `AZURE_SUBSCRIPTION_ID` on each
   environment.
5. **Set the repository variable** `DEPLOY_ENABLED` = `true` (Settings → Secrets and variables →
   Actions → Variables).
6. Recommended **rulesets** on `main`: require a pull request, require the `ci` status check
   (allow repository-admin bypass), and block deletion and non-fast-forward pushes. Also add a
   tag ruleset restricting creation of tags (at minimum one named `main`), since the deploy
   validation keys off the run's branch/tag name.

Also replace `AzureAd:Domain` (`your-domain.com`) in `appsettings.Test.json` /
`appsettings.Production.json`. Behavior and the config/secrets table are described in the README's
"Branching, CI and Deployment" section (replace the template's README, but keep that section's
content current for your project).

## Updating or removing the template

```bash
dotnet new uninstall .          # from the clone, or:
dotnet new uninstall Wrak.CleanBlazor.Template   # by identity, from anywhere
```

Because the install is a live path reference (see above), there is no separate "update" step for
local development — `git pull` in the clone is the update. `dotnet new uninstall` (with no
arguments) lists every installed template source, including the full local path, if you've lost
track of how it was installed.

## Troubleshooting

- **`dotnet new blazor-clean` fails with "No templates or subcommands found"** — the install
  didn't register, or you're running it from a shell that resolved a different `dotnet` (e.g. a
  different SDK version's template cache). Re-run `dotnet new install .` from the clone and check
  `dotnet new list blazor-clean` again.
- **Generation fails with a file-already-exists error** — you generated into a non-empty directory
  that already had a colliding path. Use an empty/fresh output directory.
- **Old content still appears after you edited this repo** — you're generating from a *different*
  installed copy (e.g. you have both a cloned-path install and an old `.nupkg` install
  registered). Run `dotnet new uninstall` with no arguments to see every installed source and
  identity, and remove the stale one.
- **A `.md`/`.yml`/config file you didn't expect to change got the name substituted** (or, less
  commonly, something you expected to change didn't) — re-read "What gets renamed automatically"
  above; the scope is "every non-`copyOnly` file," not the `templateFileExtensions` list. If you
  need a file's content to survive verbatim, add it to `template.json`'s `copyOnly` glob or
  `sources.exclude`, not to `templateFileExtensions`.
- **Generated solution won't restore/build** — check the .NET SDK version (`net10.0` target) and
  that `nuget.config` in the generated output still resolves `nuget.org` (it's copied verbatim;
  if your organization requires a private feed, edit it post-generation).
