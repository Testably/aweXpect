# Workflows

Commands are run from the repository root. `./build.ps1` (Windows) and `./build.sh` (Linux, macOS) run the Fallout
build in `Pipeline/`; plain `dotnet` commands are faster for a local edit-test loop.

## Build

```shell
dotnet build aweXpect.slnx
dotnet build Source/aweXpect.Core
```

- The default configuration is `Debug`. Every configuration compiles `Source/aweXpect` against the `aweXpect.Core`
  project.
- Building `Source/aweXpect` also builds the analyzers, code fixers and generators it packs.
- `GeneratePackageOnBuild` is on, so every library build writes a `.nupkg` into its `bin` folder.

## Test

The unit test projects are TUnit executables on the Microsoft.Testing.Platform, so they start with `dotnet run`.
Arguments after `--` go to the test application.

```shell
dotnet run --project Tests/aweXpect.Tests -f net10.0
dotnet run --project Tests/aweXpect.Tests -f net10.0 -- --treenode-filter "/*/*/ThatString+IsEqualTo+AsPrefixTests/*"
dotnet run --project Tests/aweXpect.Docs.Tests -f net10.0 -- --treenode-filter "/*/*/*/*[Category=Slow]"
dotnet run --project Tests/aweXpect.Api.Tests
```

- The tree-node filter is `/<assembly>/<namespace>/<class>/<test>`; nested classes are joined with `+`.
- `net48` runs only on Windows.
- The VSTest projects under `Tests/Frameworks` (MSTest, NUnit, xUnit v2, Fallback) run with `dotnet test`; the TUnit
  and xUnit v3 projects under `Tests/Frameworks` run with `dotnet run`.
- See [testing](testing.md) for which projects to run and how to accept snapshot changes.

## Benchmarks

`Benchmarks/aweXpect.Benchmarks` compares aweXpect with FluentAssertions and TUnit.Assertions in happy-case scenarios
with BenchmarkDotNet; each extension package compares its main feature areas with FluentAssertions in
`Benchmarks/aweXpect.<Extension>.Benchmarks`. Where FluentAssertions has no expectation for the subject, its benchmark
does what a FluentAssertions user writes instead, e.g. read the content of a response as a string, and the summary of
the benchmark says so. `aweXpect.Snapshots`
has no benchmarks: its time goes into file access, and FluentAssertions has no snapshots to compare with.

```shell
dotnet run -c Release --project Benchmarks/aweXpect.Benchmarks -- --filter "*String*"
./build.ps1 Benchmarks --benchmark-package aweXpect.Json
```

- Compare before and after in ABBA order (after, before, before, after) on an idle machine. Time differences below
  about 20 ns are noise; allocations are stable.
- `BenchmarkProjects` in `Pipeline/Build.Benchmarks.cs` lists the benchmark project of each package and the packages
  it measures. The parameter `BenchmarkPackage` selects one of them; without it, all run.
- CI runs one job per package. In a pull request, a job only runs the benchmarks when the pull request changes the
  source of a measured package, the benchmark project or a shared build input (`SharedBenchmarkInputs`), and
  `ci-analysis.yml` posts one PR comment per package that ran. A method name ending in `_aweXpect` is highlighted in
  the comment.
- On `main`, every package appends its results to its history on the `benchmarks` branch, which the docs site reads:
  `Docs/pages/static/js` for `aweXpect`, `Docs/extensions/<Package>/static/js` for an extension package. The results
  are shown in `Docs/pages/12-benchmarks.mdx` and `Docs/extensions/<Package>/99-benchmarks.mdx`; a new benchmark
  needs a tab there.

## Mutation tests

Stryker.NET mutates `aweXpect` (`MutationTestsMain`, tested by `aweXpect.Tests` and `aweXpect.Internal.Tests`) and
`aweXpect.Core` (`MutationTestsCore`, tested by `aweXpect.Core.Tests`), and each extension package with its own
target (`MutationTestsJson`, `MutationTestsReflection`, `MutationTestsSnapshots`, `MutationTestsWeb`,
`MutationTestsXml`), tested by its test projects. `aweXpect.Snapshots.DiffEngine` and `aweXpect.Snapshots.PublicApi`
have no mutation tests.

- Stryker starts in the folder of the mutated project (project mode), so it runs only the configured `test-projects`;
  started from the solution folder it would ignore them and run every test project. The framework projects run on
  VSTest, which Stryker's runner for the Microsoft.Testing.Platform (`--test-runner mtp`) cannot execute.
- `MutationTestsMain`, `MutationTestsCore` and `MutationTestsReflection` are switched off by
  `DisableMainMutationTests = true`, `DisableCoreMutationTests = true` and `DisableReflectionMutationTests = true` in
  `Pipeline/Build.MutationTests.cs`, because Stryker's runner for the Microsoft.Testing.Platform cannot capture the
  coverage of their tests within the job time limit. Their nightly jobs
  still exist and skip the targets, and the dashboard ignores their artifacts. `DisableExtensionMutationTests = true`
  switches off those of the extension packages. With all flags set the dashboard target is skipped too.
- Pull requests do not run mutation tests, only the nightly workflow does.
- The source is split into disjoint slices (`MainMutationSlices`, `CoreMutationSlices`, `ReflectionMutationSlices`),
  selected with the build parameter `MutationSlice`; without it the whole project is mutated. The nightly workflow
  runs one job per slice, and one job for each of the smaller extension packages, which are not sliced.
- A full run takes hours. Do not start one locally unless asked.

## Documentation

- The user docs are Markdown pages in `Docs/pages`, published to https://docs.testably.org/aweXpect by the
  `Testably/Testably.Site` repository (triggered by `notify-docs-site.yml` after a successful `Build` on `main`).
- The docs of an extension package are in `Docs/extensions/<Package>` and are published to
  `https://docs.testably.org/aweXpect/extensions/<Package>`.
- `Docs/agents` (this folder) is for contributors and agents and is not published.
- Code blocks in `Docs/pages` are compile-checked by the slow tests of `aweXpect.Docs.Tests`; see
  [testing](testing.md#documentation-tests). Those in `Docs/extensions` are not, so check their examples against a
  test.
- The README of the `aweXpect` and `aweXpect.Core` packages is generated from `README.md` during `Compile`. An
  extension package ships `Source/<Package>/README.md`, which repeats the index page of its docs with absolute links to
  the docs pages, as nuget.org cannot follow relative ones and does not render the components of the docs site; keep
  both in sync when adding or renaming an expectation.

## Pipeline targets

`./build.ps1 --help` lists the targets. Running the script without a target runs the defaults (`Pack`, `ApiChecks`,
`DocsTests`, `Benchmarks`, `CodeAnalysis`), which is rarely what you want locally. The configuration defaults to `Debug` locally
and `Release` on the build server (`--configuration Release`).

| Target | What it does |
| --- | --- |
| `Compile` | Restores, calculates the version with GitVersion and builds the solution |
| `UnitTests` | Runs the unit test projects (`net48` only on Windows), then their slow tests |
| `TestFrameworks` | Runs the projects under `Tests/Frameworks` |
| `CodeCoverage` | `UnitTests` and `TestFrameworks` with coverage, merged into `TestResults/reports`; the tests collect coverage only when this target runs |
| `ApiChecks` | Runs `aweXpect.Api.Tests` |
| `DocsTests` | Runs `aweXpect.Docs.Tests` (`net48` only on Windows), then its slow tests |
| `AotSmokeTests` | Runs `Tests/Aot` normally and published with Native AOT for the current platform (needs the native toolchain, on Windows the C++ build tools of Visual Studio) |
| `Pack` | Collects the packages into `Artifacts/Packages` |
| `ReleaseNotes` | Writes the changes of each package since the last stable release into `Artifacts/ReleaseNotes.md` |
| `Benchmarks` | Runs the benchmarks (of `BenchmarkPackage`, or all) and prints the reports; on `main` it appends them to the `benchmarks` branch |
| `CodeAnalysis` | SonarCloud analysis around `CodeCoverage` (needs `SonarToken`) |
| `MutationTestsCore`, `MutationTestsMain`, `MutationTests<Package>` | Stryker.NET mutation tests (`MutationTestsCore`, `MutationTestsMain` and `MutationTestsReflection` currently disabled) |
| `BenchmarkComment`, `MutationTestsDashboard` | CI only: benchmark PR comment and mutation dashboard upload |

Test results go to `TestResults/`, packages and reports to `Artifacts/`. A project that gets its first slow test must
be added to `ProjectsWithSlowTests` in `Pipeline/Build.UnitTest.cs`; a listed project without slow tests fails.

## Releases

- A tag `vX.Y.Z` releases all packages with the same version; it accepts a pre-release suffix such as `-pre.1`.
  `Compile` fails on a malformed release tag.
- A package with `<IsBetaPackage>true</IsBetaPackage>` gets a `beta` label (`Source/Directory.Build.targets`):
  `4.1.0-beta.1` for a release and `4.1.0-beta.0.pre.1` for a pre-release, so that the beta of a release sorts above
  the betas of its pre-releases.
- The GitHub release lists the changes of each package (`ReleaseNotes`) above the generated list of all pull requests.
  A commit counts for a package when it changes one of the package's source directories (`PackageDirectories` in
  `Pipeline/Build.ReleaseNotes.cs`); its conventional-commit type decides the category.
- `build.yml` also uploads the release notes as the artifact `ReleaseNotes` on `main`, as a preview of the next
  release.

## CI workflows

| Workflow | Trigger | Jobs |
| --- | --- | --- |
| `ci.yml` | Pull requests to `main` | `UnitTests TestFrameworks` on Ubuntu, Windows and macOS; `ApiChecks DocsTests`; `AotSmokeTests`; `Benchmarks` per changed package; `CodeAnalysis` |
| `ci-analysis.yml` | After `CI` completes | Posts a benchmark PR comment per package |
| `build.yml` | Pushes to `main`, release tags | `UnitTests TestFrameworks` on three platforms, `ApiChecks DocsTests`, `AotSmokeTests`, `Benchmarks` per package, `CodeAnalysis`, `Pack`, `ReleaseNotes`; on a tag, pushes all packages to NuGet and creates the GitHub release |
| `nightly.yml` | Daily, when `main` changed | Sliced mutation tests and the Stryker dashboard |
| `notify-docs-site.yml` | After `Build` on `main` | Triggers the docs site rebuild |

Dependabot opens weekly `chore:` PRs; Mergify squash-merges them once the unit tests and the API and Docs tests pass.
