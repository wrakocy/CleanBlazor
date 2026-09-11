---
description: Add a full-stack functional test in FunctionalTests for a Blazor page. Use whenever a new page/route is added under Components/Pages/Areas, to prove the route resolves and renders through the real DI-wired app.
applyTo: "*.FunctionalTests/**/*.cs"
---

# Add a Functional Test

Functional tests in `FunctionalTests` boot the whole app via `WebApplicationFactory`
(`CustomWebApplicationFactory`), with every external boundary (API clients, repositories, storage,
email/SMS) replaced by a mock, and issue real HTTP `GET` requests against page routes. Unlike unit
tests, these mirror the **Blazor page's own folder structure** in `Web`, not the
`<Class>_<Method>.cs` naming used for unit tests. See `docs/04-testing-guide.md`.

## Steps

1. **File location mirrors the page's path in the Web project.** For a page at
   `Web/Components/Pages/Areas/<Domain>/<PageName>/<PageName>.razor`, add
   `FunctionalTests/Components/Pages/Areas/<Domain>/<PageName>/<PageName>.cs`.
2. **Name the class after the page**, not the containing folder.
3. **Inherit `WebApplicationTestFixtureBase`**, which exposes a `_client` (`HttpClient`) wired to
   `CustomWebApplicationFactory`:
   ```csharp
   public class Archived(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
       : WebApplicationTestFixtureBase(factory, outputHelper) { }
   ```
4. **One `[Fact]` per route the page responds to**, named `ReturnsViewWithCorrectMessage` (add a
   `WithId` / `WithoutId` suffix when the page has both a parameterized and parameterless route,
   e.g. `@page "/orders/details/{Id:int}"` and `@page "/orders/details"`). `GET` the route, assert
   success, and assert the response body contains the page's visible heading/title text (from its
   `<PageHeader>`/`<PageTitle>`, i.e. what a user would actually see) — not an internal route or
   constant name:
   ```csharp
   [Fact]
   public async Task ReturnsViewWithCorrectMessage()
   {
       var rsp = await _client.GetAsync("/orders");
       rsp.EnsureSuccessStatusCode();
       var stringRsp = await rsp.Content.ReadAsStringAsync();
       Assert.Contains("Order Search", stringRsp);
   }
   ```
5. **No per-test mocking is needed.** `CustomWebApplicationFactory` already registers a mock for
   every external boundary interface (`FunctionalTests/Mocks/`) for the whole test run. If the
   page depends on a repository/API-client method or a cache-service interface that isn't
   implemented in the relevant mock or registered in `CustomWebApplicationFactory.cs`, add it
   there — following the existing methods' pattern — rather than mocking per-test.
6. **Scope of assertion**: these tests only prove the route resolves and the page renders — they
   don't assert on grid contents or exercise button clicks/interactivity (that's for bUnit/component
   tests or manual browser verification). Keep assertions to status code plus the page's
   title/heading text.
