# Using This Repo as a `dotnet new` Template

This repo doubles as an installable `dotnet new` template (`.template.config/template.json`,
identity `Wrak.CleanArchitecture.Template`, short name `blazor-clean`). Use this path when you
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
  `Wrak.CleanArchitecture` becomes your `<Company>.<Product>` value, and every file/folder whose
  name contains `Wrak.CleanArchitecture` is renamed to match (see below).
- `-o`/`--output` is optional. Omit it and the template's `preferNameDirectory` setting kicks in:
  `dotnet new` creates a new subdirectory named `<Company>.<Product>` under the current directory
  and generates into that. Pass `-o .` to generate straight into the current directory instead.
- The target directory does not need to be empty, but an existing file with the same relative path
  as a generated one will cause the command to fail rather than overwrite it — generate into a
  fresh directory.

Example:

```bash
dotnet new blazor-clean -n Acme.Billing -o ../Acme.Billing
cd ../Acme.Billing
dotnet build Acme.Billing.slnx
```

The generated solution builds standalone — no reference back to this repo or its NuGet feed is
left behind.

## What gets renamed automatically

`dotnet new`'s default text-substitution behavior — **not** the `templateFileExtensions` entry in
`template.json`, which does not scope or limit which files get processed — applies to every file
in the template except the binary/verbatim types listed in `template.json`'s `copyOnly` modifier
(`*.dll`, `*.png`, `*.ico`, `*.jpg`, `*.jpeg`, `*.ps1`). In practice that means the
`Wrak.CleanArchitecture` → `<Company>.<Product>` replacement runs across essentially every text
file in the repo, including ones you might not expect:

| Renamed automatically | Untouched |
| --- | --- |
| Project folder names (`Wrak.CleanArchitecture.Core` → `<Company>.<Product>.Core`, etc.) | `Wrak.Extensions` / `Wrak.RandomData` NuGet package references — these are real published package IDs, not the template's sourceName, so they don't match and are left alone |
| `.csproj` / `.slnx` file names and their internal `<ProjectReference>`/`<File Path>` entries | `.template.config/` itself — excluded from `sources.include`, never present in generated output |
| C# namespaces, `global using` statements, and any other `Wrak.CleanArchitecture` text inside `.cs`, `.razor`, `.yml` files | Anything under `[Bb]in/`, `[Oo]bj/`, `.vs/`, `.git/` — excluded by `sources.exclude` |
| Every `Wrak.CleanArchitecture` occurrence inside `.md` files too — including `AGENTS.md` and `.github/copilot-instructions.md`'s "this describes the `Wrak.CleanArchitecture` template" comment, project-map table, and `dotnet build`/`dotnet test` command examples | Prose that never contained the literal string in the first place — `README.md`'s current wording is already name-agnostic ("this template", "this solution"), so nothing changes there even though the substitution engine still runs over it |

Verify this yourself any time by generating into a scratch folder and diffing against this repo,
or `grep -r Wrak.CleanArchitecture <generated-folder>` — a clean generation returns no matches.

## What still needs manual editing

The rename above is purely textual; it does not know anything about your actual domain. After
generating, still do this by hand (same list as `AGENTS.md`'s persistence-model checkbox and the
`create-solution` skill's step 7):

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

## Updating or removing the template

```bash
dotnet new uninstall .          # from the clone, or:
dotnet new uninstall Wrak.CleanArchitecture.Template   # by identity, from anywhere
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
