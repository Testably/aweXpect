# Testing

## Test projects

| Project | Runner | Targets | Covers |
| --- | --- | --- | --- |
| `Tests/aweXpect.Tests` | TUnit, `dotnet run` | `net48;net8.0;net10.0` | The expectations of `aweXpect`, through the public API |
| `Tests/aweXpect.Internal.Tests` | TUnit, `dotnet run` | `net48;net8.0;net10.0` | Internals of `aweXpect` (`InternalsVisibleTo`) |
| `Tests/aweXpect.Core.Tests` | TUnit, `dotnet run` | `net48;net8.0;net10.0` | `aweXpect.Core`, including internals |
| `Tests/aweXpect.Generators.Tests` | TUnit, `dotnet run` | `net8.0` | All source generators, and parity of generated metadata with the reflection fallback |
| `Tests/aweXpect.Analyzers.Tests` | TUnit, `dotnet run` | `net8.0` | Analyzers, code fixers and the `IsNotNullSuppressor` |
| `Tests/aweXpect.Docs.Tests` | TUnit, `dotnet run` | `net48;net8.0;net10.0` | Compiles the code blocks in `Docs/pages`, checks that the API is documented |
| `Tests/aweXpect.Api.Tests` | TUnit, `dotnet run` | `net8.0` | Public API snapshots of `aweXpect` and `aweXpect.Core` per target framework |
| `Tests/Frameworks/*` | per framework | `net10.0` (NUnit: `net8.0`) | Integration with MSTest, NUnit, xUnit, TUnit and the fallback without a framework |
| `Tests/Aot/*` | executables | `net10.0` | Smoke checks under Native AOT (TUnit, xUnit v3) |

`aweXpect.Tests` alone is not the whole suite. Pick the projects by what you changed:

- `Source/aweXpect`: `aweXpect.Tests`, `aweXpect.Internal.Tests`, and `aweXpect.Docs.Tests` (slow tests) when a public
  expectation is added or renamed.
- `Source/aweXpect.Core`: `aweXpect.Core.Tests`, plus `aweXpect.Tests` if the behaviour shows there (in `Debug` only,
  see the package split in [AGENTS.md](../../AGENTS.md#project-constraints)).
- `[GuaranteesNotNull]` markings: `aweXpect.Analyzers.Tests` (`IsNotNullSuppressorTests` asserts which expectations
  suppress CS8602) and `GuaranteesNotNullTests` in `aweXpect.Tests`.
- Generators: `aweXpect.Generators.Tests`.
- Public API: `aweXpect.Api.Tests`.
- Reflection, metadata registration or trimming: `./build.ps1 AotSmokeTests`.
- `Docs/**`: the slow tests of `aweXpect.Docs.Tests`.

## Slow tests

Tests marked `[Explicit]` and `[Category(TestCategories.Slow)]` (`Tests/Shared/TestCategories.cs`) are skipped by a
plain run and only selected with `--treenode-filter "/*/*/*/*[Category=Slow]"`. A plain run of `aweXpect.Docs.Tests`
therefore runs almost nothing. The pipeline runs them in a step of its own for the projects in `ProjectsWithSlowTests`
(`Pipeline/Build.UnitTest.cs`).

## Public API snapshots

`ApiApprovalTests` reads the build of every target framework of `aweXpect` and `aweXpect.Core` and compares its public
API with the `*.awexpect-snapshot.txt` files next to it (aweXpect.Snapshots.PublicApi). The `Debug` test build reads
the `Debug` library builds.

When the API changed on purpose, accept the new surface in one of these ways and commit the updated snapshots:

- run with `AWEXPECT_ACCEPT_SNAPSHOTS=all` (ignored on a build server):
  `$env:AWEXPECT_ACCEPT_SNAPSHOTS = 'all'; dotnet run --project Tests/aweXpect.Api.Tests`
- run the explicit test `AcceptApiChanges`, which a plain run skips:
  `dotnet run --project Tests/aweXpect.Api.Tests -- --treenode-filter "/*/*/ApiApprovalTests/AcceptApiChanges"`.
  TUnit runs an explicit test only when the filter matches nothing but explicit tests.
- rename the `*.awexpect-snapshot.pending.txt` file that a failed run left next to the snapshot

Never commit a pending file. CI runs the snapshots in the `ApiChecks` target.

## Documentation tests

The slow tests of `aweXpect.Docs.Tests`:

- `CodeBlocks_ShouldCompile` compiles every ```` ```csharp ```` block of each page in `Docs/pages` together with
  `Scaffold/Scaffold.cs` and an optional `Scaffold/<page path>.cs`. A block must be complete code (a fragment of a
  method fails); mark a block that cannot compile with ```` ```csharp no-compile ````.
- `ExtensionCode_ShouldCompileAgainstCoreOnly` compiles the pages in `11-extending` against `aweXpect.Core` alone.
- `DocsApiCoverageTests` fails when a public `IThat<T>` extension of `aweXpect`, or a member of `Expect`, `Fail`,
  `Skip` or `Signaler`, is not mentioned on any page.

## Writing tests

- Tests use aweXpect itself (`await That(...)`). A class per expectation, nested in a partial class per subject, e.g.
  `ThatString.IsEqualTo.AsPrefixTests` in `Tests/aweXpect.Tests/Strings/ThatString.IsEqualTo.AsPrefixTests.cs`.
- Method names follow `When<Condition>_Should<Outcome>`; methods are sorted alphabetically within their class.
- No comments or XML docs on tests: put the reasoning into `.Because(...)` on the assertion. Helpers may keep an XML
  remark when it is important.
- Never a bare `Throws<T>()`: pin the message with `.WithMessage(...)`. Failure messages are usually compared as raw
  string literals, which carry the file's CRLF line endings.
- Data sources: `[Arguments]`, `[MethodDataSource]` and `[AutoArguments]` (`Tests/Shared/AutoArgumentsAttribute.cs`,
  fills the remaining parameters with AutoFixture). `Catch.Exception`/`Catch.ExceptionAsync` capture an exception
  for later inspection.
- Timing: use `.WithTimeSystem(new VirtualTimeSystem())` (`aweXpect.Core.Internal`;
  `Tests/aweXpect.Core.Tests/TestHelpers/VirtualTimeSystem.cs`, linked into `aweXpect.Tests`) and advance the clock
  instead of real delays or wider margins. Only tests about real time itself stay on the real clock.
- `aweXpect.Tests` targets `net48` without the `IsExternalInit` or `Index` polyfill: no records or list patterns there.
  Run `-f net48` first when a test uses a newer language feature, because `net8.0`/`net10.0` pass without it.
