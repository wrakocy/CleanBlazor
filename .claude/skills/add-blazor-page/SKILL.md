---
name: add-blazor-page
description: Add or modify a Blazor Server page/component under Web/Components/Pages/Areas. Use for UI work — search/list pages, detail/edit forms, widgets — not for Core business logic.
---

# Add a Blazor Page or Component

Pages live under `Web/Components/Pages/Areas/<Domain>/...`, each as a `.razor` + `.razor.cs`
code-behind pair. See `docs/03-feature-development-guide.md` §7. Model a
search/list page and a detail/edit page after any existing pair in this solution once one exists.

## Steps

1. **Code-behind class** (`<Name>Base`) goes in `<Name>.razor.cs`, e.g. `SearchBase`, and inherits
   `AppComponentBase` (`Web/Components/AppComponentBase.razor.cs`), which already provides
   `_navManager`, `_userContext`, `_appState`, `_appBus`, `_dialogService`, `_snackBarService`. The
   `.razor` file then declares `@inherits <Name>Base`.
2. **Follow the `_camelCase` convention for `[Inject]` fields** even though they're `protected`,
   matching the rest of the codebase (`.editorconfig` enforces this for `private` fields; the repo
   extends it by convention to injected fields too):
   ```csharp
   [Inject] private ILocalStorageService _storage { get; set; } = default!;
   protected GetOrdersQuery _qry = new();
   ```
3. **Fetch/send data only through `_appBus`** (`await _appBus.Send(_qry)` /
   `await _appBus.Send(new CreateOrderCommand(model))`), never through a repository, an API
   client, or `IMediator` directly from a component.
4. **Use the working-spinner pattern** around any async call that hits the backing store:
   ```csharp
   _appState.Working = true;
   await ForceComponentRefreshAsync();
   var results = await _appBus.Send(_qry);
   _appState.Working = false;
   ```
   `ForceComponentRefreshAsync()` (on `AppComponentBase`) forces a render before an awaited call so
   the spinner actually shows.
5. **Breadcrumbs, icons, page-size defaults, etc.** come from `AppConstants` — check
   `Web/AppConstants.cs` for an existing constant before hardcoding a string.
6. **Use existing shared components/MudBlazor** before building new ones — e.g. `Progress`,
   `ProgressOverlay`, `PageHeader`, `LargeResultSetAlert`, a list-of-values-backed select
   component — and `MudDataGrid`/`MudExpansionPanels`/`MudGrid` for layout.
7. **Persisted search state**: list/search pages persist their query to local storage
   (`ILocalStorageService`) keyed by a page-specific string constant (e.g.
   `"orders.search.query"`) and restore it in `OnAfterRenderAsync(firstRender: true)`.
8. **Tests**: Blazor component tests (bUnit) or plain code-behind logic tests go in
   `UnitTests/Web/...` — see `add-unit-test`. Route-level rendering goes in `FunctionalTests` —
   see `add-functional-test`.
