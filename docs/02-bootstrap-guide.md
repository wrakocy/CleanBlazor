# New Solution Bootstrap Guide

Follow this once, at the start of a new solution. It assumes an empty repo and a chosen product
name — substitute `<Company>.<Product>` throughout (e.g. `Acme.Billing`). It also assumes you've
already decided your persistence shape: **API-backed**, **DB-backed (EF Core)**, or **both**
(see [01-architecture-overview.md](01-architecture-overview.md)) — the steps below branch on that
choice where it matters, marked **[API]** / **[DB]** / **[Both]**.

## 1. Solution & folder layout

```
/.config/         .editorconfig, Directory.Build.props, Directory.Packages.props, nuget.config
/.docs/           README.md, architecture diagrams
/.scripts/        any one-off maintenance scripts (e.g. Clean-Solution.ps1)
/src/
  <Company>.<Product>.Core/
  <Company>.<Product>.Infrastructure/
  <Company>.<Product>.Web/
/tests/
  <Company>.<Product>.UnitTests/
  <Company>.<Product>.IntegrationTests/
  <Company>.<Product>.FunctionalTests/
<Company>.<Product>.slnx
AGENTS.md   (copy from this playbook's AGENTS.md, fill in the placeholders)
CLAUDE.md   (single line: `@AGENTS.md`)
```

Use solution folders (`.slnx` `<Folder>` entries, or `.sln` solution folders) that mirror this
physical layout — don't let the solution's logical view diverge from disk.

Do **not** create a second unit-test project per source project (e.g. a `Core.UnitTests`
alongside `UnitTests`) — this playbook's own source solution carries exactly this as dead weight
from an earlier attempt; don't repeat it. One `UnitTests` project, mirroring
`Core`/`Infrastructure`/`Web` namespaces internally, is canonical.

## 2. Repo-wide configuration

- `Directory.Packages.props` — `<ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>`,
  `<FileScopedNamespace>true</FileScopedNamespace>`, then one `<PackageVersion>` per package used
  anywhere in the solution (see §7 for the baseline package list).
- `Directory.Build.props` — solution-wide MSBuild switches, e.g.
  `<TreatWarningsAsErrors>false</TreatWarningsAsErrors>`, NuGet audit settings.
- `.editorconfig` — carry forward, unmodified in intent:
  - `csharp_style_namespace_declarations = file_scoped:warning`
  - private fields: `_camelCase` (prefix `_`, camelCase body)
  - 4-space indent, `utf-8-bom`, `end_of_line = crlf` (adjust for your platform)
  - **By convention** (not enforceable by `.editorconfig` alone), extend the `_camelCase` rule to
    Blazor `[Inject]` fields even though they're `protected` — document this explicitly in
    `AGENTS.md` since the linter won't catch a violation.

## 3. Core project

```bash
dotnet new classlib -n <Company>.<Product>.Core -f net10.0
```
- `<Nullable>enable</Nullable>`, `<ImplicitUsings>enable</ImplicitUsings>`.
- Packages: `MediatR`, `FluentValidation`, `Throw`, `Serilog` (for the static `Log` façade used
  throughout Core/Infrastructure), `Wrak.Extensions` (general-purpose extension methods — public
  package, safe to include).
- `CoreMarker.cs`:
  ```csharp
  namespace <Company>.<Product>.Core;
  public class CoreMarker { } // Marker class for assembly scanning.
  ```
- `GlobalUsings.cs`:
  ```csharp
  global using FluentValidation;
  global using MediatR;
  global using Serilog;
  global using <Company>.<Product>.Core.Shared.Extensions;
  global using Throw;
  ```
- `Shared/` foundational abstractions — write these **before** any feature:
  - `Features/CommandBase.cs`
    ```csharp
    public abstract class CommandBase : IRequest { }
    public abstract class CommandBase<TResponse> : IRequest<TResponse> { }
    ```
  - `Features/QueryBase.cs`
    ```csharp
    public abstract class QueryBase<TResponse> : IRequest<TResponse> { }
    ```
  - `Events/ApplicationEventBase.cs`
    ```csharp
    public abstract class ApplicationEventBase : INotification
    {
        public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
    }
    ```
  - `Validators/ValidatorBase.cs` — extends `AbstractValidator<TModel>`. If the UI is Blazor +
    MudBlazor (the default here), also define the `IModelValidator<TModel>` bridge interface that
    exposes `ModelPropertyValidationFunc` / `ValidateModelProperty`, so MudBlazor's per-property
    validation API can drive a FluentValidation validator. If your UI isn't Blazor+MudBlazor, skip
    the bridge and keep `ValidatorBase<TModel>` a thin pass-through.
  - `Interfaces/IAppBus.cs`
    ```csharp
    public interface IAppBus
    {
        Task Publish(ApplicationEventBase appEvent, CancellationToken cancellationToken = default);
        Task Send(CommandBase command, CancellationToken cancellationToken = default);
        Task<TResponse> Send<TResponse>(CommandBase<TResponse> command, CancellationToken cancellationToken = default);
        Task<TResponse> Send<TResponse>(QueryBase<TResponse> query, CancellationToken cancellationToken = default);
    }
    ```
  - `Exceptions/DomainExceptionBase.cs` — abstract base for custom domain exceptions.
  - **[API]** `Interfaces/ICacheService.cs` — `Task Initialize()`, `IDisposable`. Only add this if
    at least one domain is API-backed.
  - **[API]** `Interfaces/I<System>Client.cs` — the interface your generated/hand-written API
    client implements (name it for the real system, e.g. `IBillingApiClient`, not the generic
    `IApiClient` unless there's genuinely only one external system).
  - **[DB]** `Interfaces/I<Entity>Repository.cs` per aggregate, as features need them (don't
    pre-create empty repository interfaces speculatively).

## 4. Infrastructure project

```bash
dotnet new classlib -n <Company>.<Product>.Infrastructure -f net10.0
dotnet add <Company>.<Product>.Infrastructure reference <Company>.<Product>.Core
```
- `InfrastructureMarker.cs` (same pattern as `CoreMarker`).
- `AppBus.cs` — implements `IAppBus` by wrapping `IMediator`. This is the **only** class in the
  entire solution allowed to inject `IMediator` directly.
  ```csharp
  public class AppBus(IMediator mediator) : IAppBus
  {
      public Task Publish(ApplicationEventBase appEvent, CancellationToken ct = default) => mediator.Publish(appEvent, ct);
      public Task Send(CommandBase command, CancellationToken ct = default) => mediator.Send(command, ct);
      public Task<TResponse> Send<TResponse>(CommandBase<TResponse> command, CancellationToken ct = default) => mediator.Send(command, ct);
      public Task<TResponse> Send<TResponse>(QueryBase<TResponse> query, CancellationToken ct = default) => mediator.Send(query, ct);
  }
  ```
- **[API]** `Integrations/<System>/` — the generated or hand-written HTTP client behind
  `I<System>Client`. If generating from an OpenAPI spec (NSwag or similar), keep the generated
  file untouched and put hand-written glue in a `Partials/` file with a `partial class`.
  `Filters/` — `HttpMessageHandler`s for cross-cutting request concerns (auth headers, request
  logging), registered via `AddHttpMessageHandler<T>()` on the typed `HttpClient`.
- **[API]** `Services/CacheServiceBase.cs` — generic base class. The core shape: `Initialize()`
  loads the first snapshot synchronously, then starts a `Timer` that calls a protected `Refresh()`
  on an interval (default 15 minutes); `Refresh()` takes a `SemaphoreSlim` with a zero timeout so
  overlapping ticks are skipped rather than queued, calls an abstract `RefreshDataAsync(filters)`
  for the derived class to fetch new data, and — only if the fetch succeeded — swaps the snapshot
  in with `Volatile.Write`; readers get the snapshot via a `Volatile.Read` in a public property, so
  a read is always fast and never blocks on a refresh in progress:
  ```csharp
  public abstract class CacheServiceBase<TSnapshot> : IDisposable where TSnapshot : class
  {
      private readonly SemaphoreSlim _refreshLock = new(1, 1);
      private Timer _timer = default!;
      private TSnapshot _snapshot;

      protected CacheServiceBase() => _snapshot = EmptySnapshot;
      protected TSnapshot Snapshot => Volatile.Read(ref _snapshot);
      protected abstract TSnapshot EmptySnapshot { get; }
      protected virtual TimeSpan Interval => TimeSpan.FromMinutes(15);
      protected abstract Task<TSnapshot?> RefreshDataAsync(List<string>? filters);

      public async Task Initialize()
      {
          await Refresh();
          _timer = new Timer(_ => _ = Refresh(), null, Interval, Interval);
      }

      protected async Task Refresh(List<string>? filters = null)
      {
          if (!await _refreshLock.WaitAsync(0)) return; // a refresh is already in flight; skip
          try
          {
              var newSnapshot = await RefreshDataAsync(filters);
              if (newSnapshot is not null) Volatile.Write(ref _snapshot, newSnapshot);
          }
          finally { _refreshLock.Release(); }
      }

      public void Dispose() { _timer?.Dispose(); _refreshLock?.Dispose(); GC.SuppressFinalize(this); }
  }
  ```
  Then one `I<Domain>CacheService`/`<Domain>CacheService` pair per domain that needs fast,
  frequently-read data from the slow/rate-limited external API. **Do not add this for a DB-backed
  domain** — query the repository directly instead.
- **[DB]** `Persistence/<Product>DbContext.cs` (EF Core `DbContext`), `Persistence/Migrations/`,
  and one `<Entity>Repository : I<Entity>Repository` per aggregate under
  `Persistence/Repositories/`. Configure entities via `IEntityTypeConfiguration<T>` classes rather
  than data annotations, kept alongside the `DbContext`.
- `Services/` — anything else Infrastructure owns outright (file export, email/SMS senders, etc.),
  each behind a Core-declared interface.

## 5. Web project

```bash
dotnet new blazor -n <Company>.<Product>.Web -f net10.0 --interactivity Server --empty
dotnet add <Company>.<Product>.Web reference <Company>.<Product>.Infrastructure
```
- `WebMarker.cs` (same pattern).
- `Program.cs` — thin, ordered, wrapped in try/catch/finally for startup failures:
  ```csharp
  try
  {
      var builder = WebApplication.CreateBuilder(args);
      var env = builder.Environment;
      var config = builder.Configuration;

      builder.AddCoreServices();
      builder.AddInfrastructureServices();
      builder.AddWebServices();          // must be called before adding Serilog
      builder.ClearDefaultLoggingProviders(); // must be called before adding OTel or Serilog
      builder.AddOpenTelemetry();
      builder.AddSerilog();
      builder.AddInteractiveBlazorServer();
      builder.AddMudBlazor();
      builder.AddAuthentication(config, env);   // see §6 — placeholder provider
      builder.AddAuthorization();
      builder.AddHealthChecks();
      builder.AddOptions();
      builder.AddMediatr();
      // [API] builder.AddHttpClients();
      // [DB]  builder.AddPersistence();

      var app = builder.Build();

      if (env.IsDevelopment())
      {
          app.UseDeveloperExceptionPage();
          app.UseForwardedHeaders();
      }
      else
      {
          app.UseExceptionHandler("/error");
          app.UseForwardedHeaders();
          app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseAuthentication();
      app.UseAuthorization();
      app.UseAntiforgery();
      app.MapStaticAssets();
      app.MapRazorComponentsWithAuthorization();
      app.MapHealthChecks("/health");

      // [API] await app.InitializeCacheServices();
      // [DB]  await app.MigrateDatabaseAsync(); // or apply migrations via a separate release step

      Log.Information($"Starting application. Environment: {env.EnvironmentName}");
      app.Run();
  }
  catch (Exception ex) { Log.Fatal(ex, "Application terminated unexpectedly."); }
  finally { Log.Information("Flushing logs and shutting down..."); Log.CloseAndFlush(); }
  ```
- `Extensions/DependencyInjectionExtensions.cs` — one small static method per concern, in the same
  order as they're called above:
  - `AddCoreServices` — `IAppBus`, per-domain conversion services/factories (transient), and
    `services.AddValidatorsFromAssemblyContaining<CoreMarker>(ServiceLifetime.Transient)`.
  - `AddInfrastructureServices` — **[API]** cache services as `Singleton` (shared across all
    Blazor circuits — never Scoped/Transient), plus `AddCircuitServicesAccessor()`. **[DB]** the
    `DbContext` (`AddDbContext`, typically Scoped) and repository implementations.
  - `AddWebServices` — Web-only singletons/scoped services: app identity, app environment, app
    state, local/session storage wrappers, dialog/snackbar services.
  - `AddMediatr` — register **all three** marker assemblies (Core, Infrastructure, Web) even
    before Web has handlers of its own, plus any global `IPipelineBehavior<,>` (e.g. a
    feature-logging behavior with an `INoFeatureLogging` opt-out marker interface).
  - **[API]** `AddHttpClients` — typed `HttpClient` per external system, with delegating handlers
    for headers/logging.
  - **[DB]** `AddPersistence` — `AddDbContext<TContext>`, repository registrations.
- `Components/AppComponentBase.razor.cs` — shared base for every page code-behind: injects
  `NavigationManager`, `IUserContext`, `IAppState`, `IAppBus`, dialog service, snackbar service;
  exposes `ForceComponentRefreshAsync()` (forces a render before an awaited call, so a
  working-spinner actually shows) and a `Dispose()` that unsubscribes from `IAppState` events.
- `AppConstants.cs` — central home for breadcrumbs, icons, page-size defaults, mask patterns.

## 6. Auth (placeholder — fill in per project)

Wire the **swap pattern**, not a concrete provider:
```csharp
public static void AddAuthentication(this WebApplicationBuilder builder, ConfigurationManager config, IWebHostEnvironment env)
{
    if (!env.IsDevelopment())
    {
        // TODO: register your real identity provider here (OpenID Connect, Microsoft Identity Web,
        // Auth0, etc.) — this is intentionally left unimplemented by the bootstrap.
        throw new NotImplementedException("Configure a production authentication scheme.");
    }
    else
    {
        builder.Services.AddAuthentication("DevelopmentAuth")
                         .AddScheme<AuthenticationSchemeOptions, DevelopmentAuthenticationHandler>("DevelopmentAuth", null);
    }
}
```
Write `DevelopmentAuthenticationHandler` as a trivial always-authenticated scheme so local
development and `FunctionalTests` never depend on a real identity provider.

## 7. Observability (kept as the default stack)

- `Serilog` + `Serilog.AspNetCore` + `Serilog.Sinks.Console` + `Serilog.Sinks.ApplicationInsights`
  + `Serilog.Sinks.OpenTelemetry`, configured in `AddSerilog` with a `UserContextLogService`
  enricher.
- `Azure.Monitor.OpenTelemetry.AspNetCore` + `OpenTelemetry.Instrumentation.AspNetCore/Http/Runtime`
  for tracing/metrics, gated on an `ApplicationInsights:ConnectionString` config value being
  present (skip wiring OTel entirely if it's empty, so local dev doesn't need a real connection
  string).
- Use `builder.Logging.AddSerilog(Log.Logger, dispose: false)` (not `UseSerilog()`) so the OTel
  logging bridge registered by `UseAzureMonitor()` is preserved.

## 8. Test projects

```bash
dotnet new xunit -n <Company>.<Product>.UnitTests -f net10.0
dotnet add <Company>.<Product>.UnitTests reference <Company>.<Product>.Web

dotnet new xunit -n <Company>.<Product>.IntegrationTests -f net10.0
dotnet add <Company>.<Product>.IntegrationTests reference <Company>.<Product>.UnitTests

dotnet new xunit -n <Company>.<Product>.FunctionalTests -f net10.0
dotnet add <Company>.<Product>.FunctionalTests reference <Company>.<Product>.UnitTests
dotnet add <Company>.<Product>.FunctionalTests package Microsoft.AspNetCore.Mvc.Testing
```
- `UnitTests`: `Moq`, `Wrak.RandomData` (public package, used by `Builders/` for
  `.WithRandomData()`); `GlobalUsings.cs` globals `Moq`, the random-data package, and
  `Xunit`/your Core `Shared.Extensions`; empty `Builders/` folder (filled in per-feature); an
  `Extensions/AssertionExtensions.cs` for custom assertion helpers.
- `IntegrationTests`: only `Microsoft.NET.Test.Sdk`, `xunit`, `xunit.runner.visualstudio` — no
  mocking framework needed, these hit real dependencies.
- `FunctionalTests`:
  - `CustomWebApplicationFactory : WebApplicationFactory<WebMarker>` — override
    `ConfigureWebHost` to register an anonymous `IAuthorizationHandler`, and swap every external
    boundary interface for a fake registered as `Singleton` (`Mocks/Mock<System>Client.cs`,
    `Mocks/MockEmailSender.cs`, storage mocks, **[API]** mock cache services returning
    builder-generated data).
  - `WebApplicationTestFixtureBase : IClassFixture<CustomWebApplicationFactory>` exposing a
    `_client` (`HttpClient`).

## 9. Solution-level sanity checks before writing the first feature

- `dotnet build` succeeds with zero project references crossing the wrong direction
  (`Core.csproj` has no `<ProjectReference>` at all).
- `dotnet test` runs (even with zero tests) across all three test projects.
- The app starts (`dotnet run --project src/<Company>.<Product>.Web`) and `/health` returns 200.
- `IAppBus`, `AppComponentBase`, `CustomWebApplicationFactory`, and the three marker classes exist
  and compile — these are the foundational pieces every feature depends on.
