---
name: add-integration-test
description: Add a test in the solution's IntegrationTests project that exercises a real dependency (a database, a file/byte-format writer, a real network call) rather than a mock. Use when mocking the dependency would leave the actual risk untested — not for ordinary business-logic coverage, which belongs in UnitTests.
---

# Add an Integration Test

`IntegrationTests` is for the minority of cases where a mock would hide the real risk: byte-level
file format correctness, a real database round-trip, timing/concurrency behavior a mock can't
reproduce. See `docs/04-testing-guide.md`. Most behavior should already be
covered by `UnitTests` with mocks — reach for this project only when that's not enough.

## Steps

1. **Same naming as unit tests**: one class per method under test,
   `<ClassUnderTest>_<MethodName>.cs`, in `IntegrationTests/<Namespace matching source>/`.
2. **No mocking framework** — construct the real subject with its real dependency:
   ```csharp
   public class ExcelExportService_ToExcelBytes
   {
       [Fact]
       public void HandlesTrickyDataTypes()
       {
           // Arrange: build real input data, including edge-case types the format has to handle
           // Act: call the real service, no mocks
           // Assert: the real output is well-formed (non-null, non-empty, parses back correctly)
       }
   }
   ```
3. **For a database-backed repository test**: point at a real (test/local/ephemeral) database
   instance — an EF Core in-memory provider is not a substitute for this, since it doesn't
   exercise real SQL translation, constraints, or transactions. Clean up any data the test writes,
   or run against a fixture that resets state between tests.
4. **Keep these tests few and targeted.** If a test here could pass with a mock instead, it
   belongs in `UnitTests`, not here.
5. **Configuration**: use a dedicated `appsettings.<ProjectName>.json` (or equivalent) for any
   connection strings/settings these tests need, separate from the app's own `appsettings.json`.
