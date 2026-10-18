# Workflows

Commands are run from the repository root. `./build.ps1` (Windows) and `./build.sh` (Linux, macOS) run the Fallout
build in `Pipeline/`; plain `dotnet` commands are faster for a local edit-test loop.

## Build

```shell
dotnet build aweXpect.slnx
dotnet build Source/aweXpect.Core
```

- The default configuration is `Debug`. Only `Debug` compiles `Source/aweXpect` against the `aweXpect.Core` project;
  every other configuration uses the released package pinned in `Directory.Packages.props`.
- Building `Source/aweXpect` also builds the analyzers, code fixers and generators it packs.
- `GeneratePackageOnBuild` is on, so every library build writes a `.nupkg` into its `bin` folder.

## Test

The unit test projects are TUnit executables on the Microsoft.Testing.Platform, so they start with `dotnet run`.
Arguments after `--` go to the test application.

```shell
dotnet run --project Tests/aweXpect.Tests -f net10.0
dotnet run --project Tests/aweXpect.Tests -f net10.0 -- --treenode-filter "/*/*/ThatString+IsEqualTo+AsPrefixTests/*"
dotnet run --project Tests/aweXpect.Docs.Tests -f net10.0 -- --treenode-filter "/*/*/*/*[Category=Slow]"
dotnet test Tests/aweXpect.Api.Tests
```

- The tree-node filter is `/<assembly>/<namespace>/<class>/<test>`; nested classes are joined with `+`.
- `net48` runs only on Windows.
- `aweXpect.Api.Tests` and the VSTest projects under `Tests/Frameworks` (MSTest, NUnit, xUnit v2, Fallback) run with
  `dotnet test`; the TUnit and xUnit v3 projects under `Tests/Frameworks` run with `dotnet run`.
- See [testing](testing.md) for which projects to run and how to accept snapshot changes.

## Benchmarks

`Benchmarks/aweXpect.Benchmarks` compares aweXpect with FluentAssertions and TUnit.Assertions in happy-case scenarios
with BenchmarkDotNet.

```shell
dotnet run -c Release --project Benchmarks/aweXpect.Benchmarks -- --filter "*String*"
dotnet run -c Debug -p:Optimize=true --project Benchmarks/aweXpect.Benchmarks -- --filter "*String*"
```

- A Release build measures the released `aweXpect.Core` package. To measure a local Core change, use the optimized
  Debug build from the second line.
- Compare before and after in ABBA order (after, before, before, after) on an idle machine. Time differences below
  about 20 ns are noise; allocations are stable.
- CI runs `Benchmarks` on every pull request and posts the result as a PR comment; on `main` it appends to the
  history on the `benchmarks` branch that the benchmark page of the docs shows.

## Mutation tests

Stryker.NET mutates `aweXpect` (`MutationTestsMain`, tested by `aweXpect.Tests` and `aweXpect.Internal.Tests`) and
`aweXpect.Core` (`MutationTestsCore`, tested by `aweXpect.Core.Tests` and the framework projects).

- They are currently switched off by `DisableMutationTests = true` in `Pipeline/Build.MutationTests.cs`, because
  Stryker's runner for the Microsoft.Testing.Platform cannot capture coverage within the job time limit. The CI and
  nightly jobs still exist and skip the targets.
- The source is split into disjoint slices (`MainMutationSlices`, `CoreMutationSlices`), selected with the build
  parameter `MutationSlice`; without it the whole project is mutated. The nightly workflow runs one job per slice.
- A full run takes hours. Do not start one locally unless asked.

## Documentation

- The user docs are Markdown pages in `Docs/pages`, published to https://docs.testably.org/aweXpect by the
  `Testably/Testably.Site` repository (triggered by `notify-docs-site.yml` after a successful `Build` on `main`).
- `Docs/agents` (this folder) is for contributors and agents and is not published.
- Code blocks are compile-checked by the slow tests of `aweXpect.Docs.Tests`; see [testing](testing.md#documentation-tests).
- The README of the NuGet packages is generated from `README.md` during `Compile`.

## Pipeline targets

`./build.ps1 --help` lists the targets. Running the script without a target runs the defaults (`Pack`, `ApiChecks`,
`Benchmarks`, `CodeAnalysis`), which is rarely what you want locally. The configuration defaults to `Debug` locally
and `Release` on the build server (`--configuration Release`).

| Target | What it does |
| --- | --- |
| `Compile` | Restores, calculates the versions with GitVersion and builds the solution and `aweXpect.Core` |
| `UnitTests` | Runs the unit test projects (`net48` only on Windows), then their slow tests |
| `UnitTestsWithCoverage` | `UnitTests`, plus a `Debug` run against the Core project when `BuildScope` is not `Default` |
| `TestFrameworks` | Runs the projects under `Tests/Frameworks` |
| `CodeCoverage` | `UnitTests` and `TestFrameworks` |
| `ApiChecks` | Runs `aweXpect.Api.Tests` |
| `AotSmokeTests` | Runs `Tests/Aot` normally and published with Native AOT for the current platform |
| `Pack` | Collects the packages into `Artifacts/Packages` |
| `Benchmarks` | Runs the benchmarks and prints the report |
| `CodeAnalysis` | SonarCloud analysis around `CodeCoverage` (needs `SonarToken`) |
| `MutationTestsCore`, `MutationTestsMain` | Stryker.NET mutation tests (currently disabled) |
| `BenchmarkComment`, `MutationTestsComment`, `MutationTestsDashboard` | CI only: PR comments and dashboard upload |

Test results go to `TestResults/`, packages and reports to `Artifacts/`. A project that gets its first slow test must
be added to `ProjectsWithSlowTests` in `Pipeline/Build.UnitTest.cs`; a listed project without slow tests fails.

### Build scope

`BuildScope` in `Pipeline/Build.cs` is a temporary switch for releasing the two packages independently:

- `Default`: build, test and pack both packages.
- `CoreOnly`: for a breaking change in `aweXpect.Core`. Builds and packs only Core, runs `aweXpect.Core.Tests` and
  `aweXpect.Generators.Tests` in `Debug`, and `UnitTestsWithCoverage` runs the remaining projects in `Debug` against
  the Core project.
- `MainOnly`: packs only `aweXpect` and leaves `aweXpect.Core.Tests` out of the unit tests.

Outside `Default`, the framework, AOT, mutation and benchmark targets are skipped.

After the Core release, bump `aweXpect.Core` in `Directory.Packages.props` and reset the scope to `Default`.

## Releases

- A tag `vX.Y.Z` releases `aweXpect`, a tag `core/vX.Y.Z` releases `aweXpect.Core`; both accept a pre-release suffix
  such as `-pre.1`. `Compile` fails on a malformed release tag.
- After a release, a `chore: bump aweXpect.Core to vX.Y.Z` (or `aweXpect`) PR updates `Directory.Packages.props`.

## CI workflows

| Workflow | Trigger | Jobs |
| --- | --- | --- |
| `ci.yml` | Pull requests to `main` | `UnitTestsWithCoverage TestFrameworks` on Ubuntu, Windows and macOS; `ApiChecks`; `AotSmokeTests`; mutation tests; `Benchmarks`; `CodeAnalysis` |
| `ci-analysis.yml` | After `CI` completes | Posts the mutation and benchmark PR comments |
| `build.yml` | Pushes to `main`, release tags | `CodeCoverage` on three platforms, `ApiChecks`, `AotSmokeTests`, `Benchmarks`, `CodeAnalysis`, `Pack`; on a tag, pushes to NuGet and creates the GitHub release |
| `nightly.yml` | Daily, when `main` changed | Sliced mutation tests and the Stryker dashboard |
| `notify-docs-site.yml` | After `Build` on `main` | Triggers the docs site rebuild |

Dependabot opens weekly `chore:` PRs; Mergify squash-merges them once the unit and API tests pass.
