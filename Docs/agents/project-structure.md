# Project Structure

## Repository layout

| Path | Contents |
| --- | --- |
| `Source/` | The shipped projects |
| `Tests/` | Test projects (see [testing](testing.md)), shared helpers in `Tests/Shared` |
| `Benchmarks/aweXpect.Benchmarks` | BenchmarkDotNet comparison with FluentAssertions and TUnit.Assertions |
| `Benchmarks/aweXpect.<Extension>.Benchmarks` | BenchmarkDotNet comparison of an extension package with FluentAssertions (none for `aweXpect.Snapshots`) |
| `Pipeline/` | The Fallout build (`Build.*.cs`, one partial file per area), started by `build.ps1`/`build.sh`/`build.cmd` |
| `Docs/pages` | User documentation, published to the docs site |
| `Docs/extensions/<Package>` | User documentation of an extension package |
| `Docs/agents` | Reference for contributors and agents, not published |
| `.github/workflows` | CI (see [workflows](workflows.md#ci-workflows)) |
| `aweXpect.slnx` | The solution |

## Shipped projects

Two NuGet packages are built from `Source/`; the analyzer and generator projects are packed into them under
`analyzers/dotnet/cs`.

| Project | Package | Role |
| --- | --- | --- |
| `aweXpect.Core` | `aweXpect.Core` | Engine and extension API: `Expect.That`, `IThat<T>`, `ExpectationBuilder`, constraints, results, formatting, options, customization, equivalency, recording and signaling primitives |
| `aweXpect.Core.Generators` | in `aweXpect.Core` | Generates expectations from methods marked with `[GenerateExpectation]` (`ExpectationGenerator`); runs in the consumer for the type metadata registrations for trimming and AOT (`TypeMetadataGenerator`), unless an older `aweXpect` without the `aweXpect.TypeMetadataGenerator` assembly metadata ships its own copy, and for the registration hook of the extensions (`ExtensionRegistrationGenerator`) |
| `aweXpect` | `aweXpect` | The expectations for users, as extension methods on `IThat<T>` |
| `aweXpect.Generators` | in `aweXpect` | Runs in the consumer: test framework adapters (`FrameworkGenerator`) |
| `aweXpect.SourceGenerators` | none | Runs only while building `aweXpect`: generates the overloads of collection expectations (`CollectionExpectationGenerator`) |
| `aweXpect.Analyzers` | in `aweXpect` | Analyzers (e.g. missing `await`, unordered collections) and `IsNotNullSuppressor`, which reads `[GuaranteesNotNull]` |
| `aweXpect.Analyzers.CodeFixers` | in `aweXpect` | Code fixes for the analyzers |

`aweXpect` depends on `aweXpect.Core`. Extension packages reference only `aweXpect.Core`, so Core aims to stay more
stable than `aweXpect`; the namespace `aweXpect.Core.Internal` may change in any version, so `WithTimeSystem` and
`ITimeSystem` are `[Experimental("aweXpect5001")]` and the compiler reports an error when another assembly uses them.
Core itself uses `ITimeSystem` throughout, so `aweXpect5001` is in the `NoWarn` of `aweXpect.Core.csproj` only.

The extension packages are built from `Source/` as well and reference `aweXpect.Core` as a project. Each sets its own
description, project URL and README (`PackageReadmeSourceFile`, the index page of its docs) in its project file; the
logo is the one of aweXpect.

Like `aweXpect`, every extension package references `aweXpect.Core` (or `aweXpect.Snapshots`) with
`PrivateAssets="contentfiles;build"`, so that the analyzers of `aweXpect.Core`, and with them the type metadata
generator, reach a consumer that references only an extension. NuGet restores `aweXpect.Core` once, so the generator
runs once however many of the packages a project references. A project reference does not pass analyzers on, so
projects in this repository that need the generator reference `aweXpect.Core.Generators` as an analyzer themselves.

| Project | Package | Role |
| --- | --- | --- |
| `aweXpect.Json` | `aweXpect.Json` | Expectations for JSON strings and `System.Text.Json` |
| `aweXpect.Web` | `aweXpect.Web` | Expectations for `HttpRequestMessage`, `HttpResponseMessage`, `Uri` and `IPAddress` |
| `aweXpect.Xml` | `aweXpect.Xml` (beta) | Expectations for XML strings and `System.Xml.Linq` |
| `aweXpect.Reflection` | `aweXpect.Reflection` | Expectations for reflection types and architecture rules |
| `aweXpect.Snapshots` | `aweXpect.Snapshots` (beta) | Snapshot testing |
| `aweXpect.Snapshots.DiffEngine` | `aweXpect.Snapshots.DiffEngine` (beta) | Opens a diff tool with DiffEngine for snapshots that are missing or different |
| `aweXpect.Snapshots.PublicApi` | `aweXpect.Snapshots.PublicApi` (beta) | Public API snapshots, also used by `aweXpect.Api.Tests` |

## Source layout

- `Source/aweXpect/That/<Area>/That<Subject>.<Expectation>.cs`: one partial static class per subject
  (`ThatString`, `ThatEnumerable`, ...) with one file per expectation. The areas are value kinds such as `Strings`,
  `Numbers`, `Collections`, `DateTimes`, `Delegates` or `Exceptions`. The tests mirror this in
  `Tests/aweXpect.Tests/<Area>/That<Subject>.<Expectation>[.<Variant>]Tests.cs`.
- `Source/aweXpect/{Equivalency,Helpers,Options,Results}`: shared implementation of the expectations.
- `Source/aweXpect.Core/Core`: the evaluation engine. `ExpectationBuilder` collects constraints into a tree of
  `Nodes`; `Constraints` holds `ConstraintResult`; `Sources` wraps values and delegates; `Adapters` maps failures to
  the test framework; `Extending` holds the attributes for generated expectations and `ReflectionFallback.cs`, which
  guards member reflection; `Metadata` holds the registry and attributes for generated type metadata; `Internal` holds unstable
  API such as `WithTimeSystem`; `Polyfills` holds polyfills for the older targets (some are linked into `aweXpect`).
- `Source/aweXpect.Core/{Formatting,Options,Results,Customization,Equivalency,Recording,Signaling,Delegates,Collections}`:
  value formatting, comparison options, result types, the customization API and the corresponding primitives.

## Extension packages

All extension packages put their entry points into the namespace `aweXpect`, so that they show up next to the
built-in ones without an extra `using`: the `That<Subject>` classes with the expectations and entry extensions such as
`JsonExtensions.AsJson()` and `XmlExtensions.AsXml()`. Every other public type lives in `aweXpect.<Package>` or below,
so that it cannot clash with Core or another package: results in `aweXpect.<Package>.Results`, the customization group
and its types, options and formatting in `aweXpect.<Package>` (or a sub-namespace such as
`aweXpect.Web.ContentProcessors`). The layout is the same as in `aweXpect`: one partial class `That<Subject>` per
subject, one file `That<Subject>.<Expectation>.cs` per expectation that also holds its negation and its constraint
classes. `Helpers/ThrowHelper` validates arguments with `Tracing.WriteException` from
`aweXpect.Core`; `Helpers/Polyfills` (or `Polyfills/`) holds what `netstandard2.0` lacks. A new expectation touches the
source file, its tests, an AOT check, the API snapshots, and in `Docs/extensions/<Package>` its page and the overview
table of `00-index.md`.

### aweXpect.Json

- `ThatJsonString.*`: `IsValidJson`, `IsNotValidJson`, `IsValidJsonMatching`, `IsValidJsonMatchingExactly` on `string`;
  `JsonExtensions.AsJson()` plugs `JsonMatchType` into `IsEqualTo(...)`.
- `ThatJsonElement.*`, `ThatNullableJsonElement.*`, `ThatJsonNode.*`: the same expectations for `JsonElement`,
  `JsonElement?` and `JsonNode` (`Matches`, `MatchesExactly`, `IsObject`, `IsArray`, ...); the file without a suffix
  holds the shared constraints. `ThatJsonObject.*`: `IsJsonSerializable`, `IsJsonSerializedAs` on any object.
- `Options/` (namespace `aweXpect.Json.Options`): `JsonOptions`; a new option must be copied in the internal copy
  constructor and listed in the "JSON options" context of a failure. `JsonLookupOptions` holds the options of
  `HasProperty` and `HasValueAt`.
- `Json/` (namespace `aweXpect.Json`): `JsonOptionsExtensions` and `JsonLookupOptionsHolderExtensions` with the
  option methods; `JsonElementValidator`
  reports each difference by its JSON path; `ExpectationJsonConverter` serializes
  expected values and the `It.Is...` expectations inside them, anything but JSON values by reflection behind
  `ReflectionFallback.IsSupported`; `Customize.aweXpect.Json()` with the default document and serializer options.
- On `netstandard2.0` the `System.Text.Json` package is used, on `net8.0`/`net10.0` the framework, so behaviour can
  differ between target frameworks.

### aweXpect.Web

- `ThatHttpRequestMessage`, `ThatHttpResponseMessage`, `ThatUri`, `ThatIPAddress`; the base file of each holds shared
  helpers.
- `Web/Results`: fluent results such as `HasHeaderValueResult` (`.WithValue(...)`, `.WhoseValue(...)`) and
  `ProblemDetailsResult`. `HasStatusCode()` returns the Core enum property result, and the status classes (`.Success()`)
  are extension methods on it in `HttpStatusCodePropertyResultExtensions`.
- `Web/ContentProcessors`: `IContentProcessor` and the JSON, string and binary processors that format `HttpContent`;
  `Customize.aweXpect.Web().ContentProcessors` configures them.
- `Helpers/HttpFormatter` writes the `HTTP-Request:`/`HTTP-Response:` sections appended to failures on requests and
  responses.
- A relative `Uri` fails component expectations both ways ("but it was relative"): the generated expectations of
  `ThatUri` return `PredicateResult.FailureBothWays`, the other constraints derive from `AbsoluteUriConstraint`,
  which also stops further processing, while the property results and `HasQueryParameter`, whose `WhoseValue` maps
  to a member, put `FailIfRelative()` in front.
- `Tests/aweXpect.Web.Samples` is a minimal API application used by `aweXpect.Web.Samples.Tests` and the example
  on the index page of the docs.

### aweXpect.Xml

- `ThatXmlString.*` (`IsValidXml`, optionally with expectations on the root, `IsNotValidXml`, `IsValidXmlMatching*`),
  `ThatXElement.*` and `ThatXDocument.*` (`Matches`, `MatchesExactly`, `HasAttribute`, `HasElement` and their
  negations; a document delegates to its root element). `XmlExtensions.AsXml()` sets a `StringMatchType` for `IsEqualTo(...)`.
- `Xml/XmlComparer` compares by content: names by namespace and local name, attributes in any order, comments and
  processing instructions ignored, text and CDATA as one text. Differences are reported by path, e.g.
  `/order/item[2]/@price`. `MatchesXmlConstraint<TNode>` parses the expected XML once when the expectation is written;
  `HasNodeConstraint<TSubject, TNode>` stores the found node for nested expectations.
- `Options/XmlOptions` (namespace `aweXpect.Xml.Options`) is immutable, its option methods are in
  `Xml/XmlOptionsExtensions` (namespace `aweXpect.Xml`): a new option must be copied in the internal copy constructor
  and listed in the "XML options" failure context. `MembersToIgnoreTracker` names ignored members that matched nothing.
- `Helpers/SecureXml` is the only place that parses XML: DTDs are prohibited and no resolver is set. Never parse with
  `XDocument.Parse` or default `XmlReaderSettings`.

### aweXpect.Reflection

User code picks a source, navigates to a member kind, filters and expects:
`Expect.That(In.AssemblyContaining<T>().Methods().WhichAreAsync()).HaveName("Async").AsSuffix()`.

- Sources: `In.cs` selects concrete objects (`In.AllLoadedAssemblies()`, `In.Type<T>()`, ...); `Types.cs` selects
  types by criteria. Its `In*` methods mirror the assembly sources of `In` and are mirrored again on
  `Filtered.Types.InNamespaceResult`, so a new assembly source goes into all three.
- `Collections/Filtered.*`: immutable filtered collections on top of `Filtered<T, TFiltered>`, `IAsyncEnumerable<T>`
  from `net8.0` on and `IEnumerable<T>` on `netstandard2.0`; `Filter.cs` builds a filter with the description that
  becomes part of the failure message.
- `Filters/<Kind>Filters.<Name>.cs`: filters such as `TypeFilters.WhichAreSealed`, usually with the negated twin in
  the same file.
- `That<Kind>.<Name>.cs` for a single subject (`ThatType`, ...) and `That<Kinds>.<Name>.cs` for collections
  (`ThatTypes`, ...), in the namespace `aweXpect` unlike the rest of the package; `ThatMember`/`ThatMembers` hold what
  all member kinds share.
- `Customization/` (namespace `aweXpect.Reflection`): `Customize.aweXpect.Reflection()` (`DependencyResolver`,
  `ExcludedAttributeTypes`, ...); the `ExcludedAssemblyPrefixes` come from `Customize.aweXpect.Settings()` in
  `aweXpect.Core` and are matched with the public `AssemblyPrefixes.IsExcluded` (`aweXpect.Core.Extending`), the same
  rule that Core uses.
- `Formatting/`: members are formatted through the wrapper `FormattableMember`, registered by a module initializer,
  so that the registration survives trimming.
- Every method that reflects over the members, interfaces, types or assembly references of a subject checks
  `ReflectionFallback.IsSupported` itself, because the trim analyzer does not follow the check into a called method or
  a lambda, and throws `ReflectionFallbackHelpers.NotSupported(...)` when it is off.

### aweXpect.Snapshots

- `ThatSnapshot.MatchesSnapshot*.cs`: `MatchesSnapshot()` for strings, objects, binary data and files; `Results/`
  holds the options of an expectation (`.Named(...)`, `.UniqueFor`, `.For`, `.Accepting(...)`, ...).
- `Snapshots/` (namespace `aweXpect.Snapshots`): identity and storage (`SnapshotId`, `SnapshotStore`,
  `DiskSnapshotStore` next to the test file); `SourceRoots` reads the source root, project directory and target
  framework that `buildTransitive/aweXpect.Snapshots.targets` records in the test assembly, so that snapshots are found
  in deterministic builds (from `ExtensionRegistry`, and by scanning the loaded assemblies that registered nothing, as
  a project cannot always compile a module initializer); the rules (`SnapshotMembers*`, `SnapshotLines*`, `SnapshotText`, ...), whose surface
  `SnapshotRule` repeats for all snapshots (`For_ShouldOfferTheSameRulesAsForAllSnapshots` keeps both equal) and
  whose filters and actions describe each rule for the "Snapshot options" context of a failure; the
  extension points `SnapshotComparer` and `SnapshotReporter`, abstract classes so that members can get a default;
  `SnapshotAcceptance` and `AcceptEnvironment` (`AWEXPECT_ACCEPT_SNAPSHOTS`, build server detection); and
  `ClaimedSnapshots`/`OrphanedSnapshots` for orphan detection.
- Evaluation: resolve the test file through `SourceRoots`, turn the subject into content with the rules, compare it
  with the stored snapshot, then write the pending file or accept it (the expectation still fails) and inform the
  reporters (never on a build server).
- `aweXpect.Snapshots.PublicApi`: `PublicApiDeclarations` reads an assembly with Roslyn from its metadata and writes
  its public API as C# declarations; missing dependencies are resolved from the `deps.json` and the NuGet packages
  folder. Not trimmable. `Options/` (namespace `aweXpect.Snapshots.PublicApi.Options`) holds the immutable
  `PublicApiOptions` and its selectors `Ignoring`, `Within` and `Formatting`, whose methods each return new options;
  a new option must be copied in the internal copy constructor and listed in the "Public API options" context.
- `aweXpect.Snapshots.DiffEngine`: `DiffEngineReporter` opens a diff tool. Its `buildTransitive` targets let the
  generated module initializer call `DiffEngineReporter.Register()` in consuming projects, which
  `<AweXpectSnapshotsDiffEngine>false</AweXpectSnapshotsDiffEngine>` turns off.
- Both use the registration hook of Core: `ExtensionRegistrationGenerator` in `aweXpect.Core.Generators` emits a
  module initializer into the test project (C# 9 and `ModuleInitializerAttribute`, the rule of the type metadata
  generator) that registers the `AssemblyMetadataAttribute` values named in `AweXpectRegisteredMetadataKeys` in
  `ExtensionRegistry` and calls the methods named in `AweXpectModuleInitializers`.
- The `buildTransitive` targets ship in the packages and affect every consumer build. A project reference does not
  import them, so the test projects import them explicitly.

## How an expectation runs

1. `Expect.That(subject)` returns an `IThat<T>` backed by an `ExpectationBuilder` for the subject.
2. Each expectation method adds a constraint to the builder and returns a result type (`AndOrResult`, ...) that offers
   options and `.And`/`.Or` chaining.
3. Awaiting the result evaluates the node tree, formats a failure message with the value formatters and reports it
   through the detected test framework adapter.

The user docs describe the public side: `Docs/pages/03-how-it-works` for users, `Docs/pages/11-extending` for
extension authors, including generated expectations and Native AOT.

## Build settings worth knowing

- Assemblies are strong-name signed with `strongname.snk` (root `Directory.Build.props`), test projects included,
  except `aweXpect.Reflection.Tests`, whose strong-name tests need an unsigned assembly.
- `Source/Directory.Build.targets` adds the `beta` label to the packages with `IsBetaPackage` (see
  [workflows](workflows.md#releases)).
- `Source/Directory.Build.props`: `TreatWarningsAsErrors`, XML documentation, `Nullable` (annotations only on
  `netstandard2.0`), trimming and AOT analyzers for `net8.0+`, package metadata. Local builds get version
  `255.255.255.255`; `Compile` sets the real version from GitVersion.
- `Tests/Directory.Build.props`: test projects are executables with TUnit's reflection discovery
  (`EnableTUnitSourceGeneration=false`) and link `Tests/Shared/*.cs`.
