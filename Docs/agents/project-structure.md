# Project Structure

## Repository layout

| Path | Contents |
| --- | --- |
| `Source/` | The shipped projects |
| `Tests/` | Test projects (see [testing](testing.md)), shared helpers in `Tests/Shared` |
| `Benchmarks/aweXpect.Benchmarks` | BenchmarkDotNet comparison with FluentAssertions and TUnit.Assertions |
| `Pipeline/` | The Fallout build (`Build.*.cs`, one partial file per area), started by `build.ps1`/`build.sh`/`build.cmd` |
| `Docs/pages` | User documentation, published to the docs site |
| `Docs/agents` | Reference for contributors and agents, not published |
| `.github/workflows` | CI (see [workflows](workflows.md#ci-workflows)) |
| `aweXpect.slnx` | The solution |

## Shipped projects

Two NuGet packages are built from `Source/`; the analyzer and generator projects are packed into them under
`analyzers/dotnet/cs`.

| Project | Package | Role |
| --- | --- | --- |
| `aweXpect.Core` | `aweXpect.Core` | Engine and extension API: `Expect.That`, `IThat<T>`, `ExpectationBuilder`, constraints, results, formatting, options, customization, equivalency, recording and signaling primitives |
| `aweXpect.Core.Generators` | in `aweXpect.Core` | Generates expectations from methods marked with `[GenerateExpectation]` (`ExpectationGenerator`) |
| `aweXpect` | `aweXpect` | The expectations for users, as extension methods on `IThat<T>` |
| `aweXpect.Generators` | in `aweXpect` | Runs in the consumer: test framework adapters (`FrameworkGenerator`) and the type metadata registrations for trimming and AOT (`TypeMetadataGenerator`) |
| `aweXpect.SourceGenerators` | none | Runs only while building `aweXpect`: generates the overloads of collection expectations (`CollectionExpectationGenerator`) |
| `aweXpect.Analyzers` | in `aweXpect` | Analyzers (e.g. missing `await`, unordered collections) and `IsNotNullSuppressor`, which reads `[GuaranteesNotNull]` |
| `aweXpect.Analyzers.CodeFixers` | in `aweXpect` | Code fixes for the analyzers |

`aweXpect` depends on `aweXpect.Core`. Extension packages reference only `aweXpect.Core`, so Core aims to stay more
stable than `aweXpect`; the namespace `aweXpect.Core.Internal` may change in any version.

## Source layout

- `Source/aweXpect/That/<Area>/That<Subject>.<Expectation>.cs`: one partial static class per subject
  (`ThatString`, `ThatEnumerable`, ...) with one file per expectation. The areas are value kinds such as `Strings`,
  `Numbers`, `Collections`, `DateTimes`, `Delegates` or `Exceptions`. The tests mirror this in
  `Tests/aweXpect.Tests/<Area>/That<Subject>.<Expectation>[.<Variant>]Tests.cs`.
- `Source/aweXpect/{Equivalency,Helpers,Options,Results}`: shared implementation of the expectations.
- `Source/aweXpect.Core/Core`: the evaluation engine. `ExpectationBuilder` collects constraints into a tree of
  `Nodes`; `Constraints` holds `ConstraintResult`; `Sources` wraps values and delegates; `Adapters` maps failures to
  the test framework; `Extending` holds the attributes for generated expectations; `Metadata` holds the registry and
  attributes for generated type metadata; `ReflectionFallback.cs` guards all reflection; `Internal` holds unstable
  API such as `WithTimeSystem`; `Polyfills` holds polyfills for the older targets (some are linked into `aweXpect`).
- `Source/aweXpect.Core/{Formatting,Options,Results,Customization,Equivalency,Recording,Signaling,Delegates,Collections}`:
  value formatting, comparison options, result types, the customization API and the corresponding primitives.

## How an expectation runs

1. `Expect.That(subject)` returns an `IThat<T>` backed by an `ExpectationBuilder` for the subject.
2. Each expectation method adds a constraint to the builder and returns a result type (`AndOrResult`, ...) that offers
   options and `.And`/`.Or` chaining.
3. Awaiting the result evaluates the node tree, formats a failure message with the value formatters and reports it
   through the detected test framework adapter.

The user docs describe the public side: `Docs/pages/03-how-it-works` for users, `Docs/pages/11-extending` for
extension authors, including generated expectations and Native AOT.

## Build settings worth knowing

- Assemblies are strong-name signed with `strongname.snk` (root `Directory.Build.props`).
- `Source/Directory.Build.props`: `TreatWarningsAsErrors`, XML documentation, `Nullable` (annotations only on
  `netstandard2.0`), trimming and AOT analyzers for `net8.0+`, package metadata. Local builds get version
  `255.255.255.255`; `Compile` sets the real version from GitVersion.
- `Tests/Directory.Build.props`: test projects are executables with TUnit's reflection discovery
  (`EnableTUnitSourceGeneration=false`) and link `Tests/Shared/*.cs`.
