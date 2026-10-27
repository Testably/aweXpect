# Architecture rules

An architecture rule restricts which other types a type may reference: its *dependencies*.
[Type dependencies](#type-dependencies) introduces the dependency filters and expectations (including
[dependency cycles](#dependency-cycles)), and [Layers as type selections](#layers-as-type-selections) shows
how to combine them into a full architecture test suite. The examples use a music store application with the layers
`MusicStore.Api`, `MusicStore.Application`, `MusicStore.Domain`, `MusicStore.Data` and `MusicStore.Infrastructure`.

## Type dependencies

The dependency filters and expectations follow the familiar filter/expect pairing:

|                              | Filter                                  | Expect (single)                   | Expect (many)                      |
|------------------------------|-----------------------------------------|-----------------------------------|------------------------------------|
| depends on namespace         | `.WhichDependOn("x", …)`                | `.DependsOn("x", …)`              | `.DependOn("x", …)`                |
| does not depend on           | `.WhichDoNotDependOn("x", …)`           | `.DoesNotDependOn("x", …)`        | `.DoNotDependOn("x", …)`           |
| depends on type              | `.WhichDependOn<T>()`                   | `.DependsOn<T>()`                 | `.DependOn<T>()`                   |
| does not depend on type      | `.WhichDoNotDependOn<T>()`              | `.DoesNotDependOn<T>()`           | `.DoNotDependOn<T>()`              |
| depends only on set          | `.WhichDependOnlyOn("x", …)`            | `.DependsOnlyOn("x", …)`          | `.DependOnlyOn("x", …)`            |
| has dependencies outside set | `.WhichHaveDependenciesOutside("x", …)` | `.HasDependenciesOutside("x", …)` | `.HaveDependenciesOutside("x", …)` |

```csharp
// The API layer must not reference the data layer
await Expect.That(Types.InNamespace("MusicStore.Api"))
    .DoNotDependOn("MusicStore.Data");

// The API layer may only reference the application and domain layers
await Expect.That(Types.InNamespace("MusicStore.Api"))
    .DependOnlyOn("MusicStore.Application", "MusicStore.Domain");

// Filter for the types that depend on a namespace
In.AllLoadedAssemblies().Types().WhichDependOn("System.Data")
```

A type *depends on* every type referenced in its **declared signature**: the base type and directly
implemented interfaces, generic arguments and parameter constraints, field/property/event types, indexer
parameters, method return/parameter/generic-argument types, constructor parameters and the types of attributes
applied to the type, its members, their parameters and return values (including `typeof(…)` and enum attribute
arguments). Purely synthetic references that you never wrote, like the compiler's own plumbing, are ignored.
Types you write in authored signatures always do count, including primitives and `void` return types (namespace
`System`); in practice, almost every type with members *does* depend on `System`.

:::warning[Signature-level only]
Dependencies are computed from reflection metadata, so body-level references such
as `new Data.AlbumRepository()`, static calls and local variables are **not** detected.
Nested types are separate types with their own dependency surface: verifying `typeof(Outer)` does not
include what `Outer.Inner` references. The collection-based expectations (e.g. over `Types.InNamespace(…)`)
enumerate nested types as their own items and therefore cover them. For IL/body-level accuracy, plug in
your own resolver via `Customize.aweXpect.Reflection().DependencyResolver` (see
[Customization](./04-customization.md#dependency-resolver)).
:::

Namespace matching is ordinal and case-sensitive and, like `WhichAreWithinNamespace`, includes sub-namespaces by
default (so `MusicStore.Data` matches `MusicStore.Data.Sql` but not `MusicStore.DataImport`). A dependency in the
**global namespace** can be targeted or allowed with an empty string (`""`). Each result is chainable:

```csharp
// Widen the set with .OrOn(…)
await Expect.That(Types.InNamespace("MusicStore.Api"))
    .DependOnlyOn("MusicStore.Application").OrOn("MusicStore.Domain");

// Opt out of sub-namespace matching for the whole expression
await Expect.That(types).DoNotDependOn("MusicStore.Data").ExcludingSubNamespaces();
```

For `DependsOnlyOn` a type's own namespace is always allowed, and by default so are its sub-namespaces. Use
`.ExcludingOwnSubNamespaces()` to also forbid
references into a type's own sub-namespaces:

```csharp
await Expect.That(Types.InNamespace("MusicStore.Domain"))
    .DependOnlyOn("MusicStore.Domain").ExcludingSubNamespaces().ExcludingOwnSubNamespaces();
```

`HasDependenciesOutside` is the **positive counterpart** of `DependsOnlyOn` for finding the violators of an
allowed set, without a double-negated "does not depend only on". The allowed set follows the same rules
(sub-namespaces included, the own namespace and framework assemblies never count as outside, the same
chainable refinements):

```csharp
// Select the current violators of an architecture rule (e.g. for a baseline)
In.AllLoadedAssemblies().Types().WhichHaveDependenciesOutside("MusicStore.Application", "MusicStore.Domain")

// Verify that a known legacy type still has its external dependency
await Expect.That(typeof(LegacyImportService))
    .HasDependenciesOutside("MusicStore.Application", "MusicStore.Domain");
```

`DependsOn` and `DoesNotDependOn` also accept a **specific type** via `<T>()` or `(Type)`, with
`.OrOn<T>()` / `.OrOn(Type)` to widen. For collections of types, `DependOn` and `DoNotDependOn` accept the
same forms and verify that every type, or none of them, depends on the specific type. The `WhichDependOn` /
`WhichDoNotDependOn` filters accept the same forms, including `.OrOn<T>()` / `.OrOn(Type)`:

```csharp
await Expect.That(typeof(Album)).DoesNotDependOn<DbContext>().OrOn<SqlConnection>();

// No controller may reference the DbContext directly
await Expect.That(Types.InNamespace("MusicStore.Api.Controllers"))
    .DoNotDependOn<DbContext>();

// The same with a type only known at runtime
Type dbContextType = typeof(DbContext);
await Expect.That(Types.InNamespace("MusicStore.Api.Controllers"))
    .DoNotDependOn(dbContextType);

// Filter for the types that reference the DbContext
In.AllLoadedAssemblies().Types().WhichDependOn(dbContextType)
```

All dependency families additionally accept a reusable `Filtered.Types` selection as target; see
[Layers as type selections](#layers-as-type-selections).

:::warning[Framework dependencies are ignored unless you name one explicitly]
`DependOnlyOn` ignores dependencies whose assembly name matches one of the
[`ExcludedAssemblyPrefixes`](./04-customization.md#assembly-exclusions) (e.g. `System` covers `System` and
`System.Text.Json`), so you never have to whitelist `System.*`, while a type's **own namespace** is always allowed. `DependsOn` / `DoesNotDependOn` / `WhichDependOn` still match a
framework namespace when you name it explicitly (e.g. `DoesNotDependOn("System.Data")`).

The default prefixes include `Microsoft`, so `DependOnlyOn` also ignores dependencies on e.g.
`Microsoft.EntityFrameworkCore`, `Microsoft.AspNetCore` and `Microsoft.Extensions.*`; a domain entity
inheriting `DbContext` does **not** fail `DependOnlyOn("MusicStore.Domain")`. To forbid such dependencies, name
them explicitly (`DoesNotDependOn<DbContext>()` or `DoNotDependOn("Microsoft.EntityFrameworkCore")`) or
customize the [`ExcludedAssemblyPrefixes`](./04-customization.md#assembly-exclusions). Note that the
customization also affects assembly scanning and assembly-level dependency expectations.
:::

<details>
<summary>What counts as a dependency in detail</summary>

Element types of arrays/pointers/by-ref and generic type arguments are unwrapped (`List<Data.Album>` depends on
`List<Data.Album>`, which also matches a `List<>` target, and on `Data.Album`; a closed-generic target like
`List<Data.Track>` only matches that exact construction). Function-pointer signatures (`delegate*<…>`) are not
decomposed; the types inside them are invisible to dependency expectations.

The ignored synthetic references are compiler-generated members, the implicit `object`/`ValueType`/`Enum` base type,
interfaces inherited from the base type, records' synthesized `IEquatable<T>`, delegates' runtime infrastructure
(only the `Invoke` signature counts), enums' underlying-value plumbing and the attributes the compiler emits onto
authored code (nullability metadata, required members, async/iterator state machines, …). Should a future compiler
version emit a marker attribute this library does not know about yet, exclude it yourself via
`Customize.aweXpect.Reflection().ExcludedAttributeTypes` (full attribute type names; extends the built-in set).

</details>

### Dependency cycles

The "slices should be free of cycles" architecture rule: verify that the namespaces of a set of types do not
(transitively) depend on each other.

```csharp
// No dependency cycles among the namespaces under MusicStore
await Expect.That(Types.InNamespace("MusicStore"))
    .HaveNoDependencyCycles();
```

A namespace `A` *depends on* a namespace `B` when some type in `A` references a type in `B` (in its
[signature](#type-dependencies)). Each cycle is reported, e.g.
`MusicStore.Billing -> MusicStore.Catalog -> MusicStore.Billing`.

By default a namespace and its sub-namespaces count as a single namespace, consistent with how the other dependency
expectations treat a type's own sub-namespaces. So a reference between a namespace and its ancestor/descendant (e.g.
`MusicStore.Catalog` ↔ `MusicStore.Catalog.Domain`) cannot by itself form a cycle. Use `ExcludingSubNamespaces()` to
treat every namespace on its own, so that such a parent/child reference can form a cycle:

```csharp
// Treat every namespace as its own node (MusicStore.Catalog ↔ MusicStore.Catalog.Domain can now form a cycle)
await Expect.That(Types.InNamespace("MusicStore"))
    .HaveNoDependencyCycles().ExcludingSubNamespaces();
```

Pass a **slice root** to group all namespaces below it into one slice each (by the namespace segment immediately
following the root), so that, for example, `MusicStore.Catalog`, `MusicStore.Catalog.Domain` and
`MusicStore.Catalog.Api` collapse into the single slice `MusicStore.Catalog`:

```csharp
// Group MusicStore.Catalog.* / MusicStore.Billing.* / … into one slice each before looking for cycles
await Expect.That(Types.InNamespace("MusicStore"))
    .HaveNoDependencyCycles("MusicStore");
```

```text title="Failure message"
Expected that Types.InNamespace("MusicStore")
have no dependency cycles when grouped into slices under "MusicStore",
but it had a dependency cycle

Dependency cycles:
MusicStore.Billing -> MusicStore.Catalog -> MusicStore.Billing
```

<details>
<summary>How cycles are detected</summary>

The dependencies are read through the same resolver as the other dependency expectations. The namespaces of the
analyzed types form the nodes of a directed graph, and each
[strongly-connected component](https://en.wikipedia.org/wiki/Strongly_connected_component) with more than one node is
reported as a cycle. Only namespaces present in the analyzed set form nodes, so dependencies on framework or otherwise
out-of-set namespaces never create an edge, and a namespace referencing itself is not a cycle.

By default a namespace and its sub-namespaces collapse into a single node (a family). Because the family is one node
(not just a suppressed pair of edges), a cycle that leaves the family and returns through a *different* member of it
(e.g. `MusicStore.Catalog -> MusicStore.Billing -> MusicStore.Catalog.Domain`) is still detected.

Configuring a [custom dependency resolver](./04-customization.md#dependency-resolver) (e.g. an IL-level one) also
sharpens cycle detection: body-level references it surfaces can complete a cycle that the signature-level default
cannot see.

</details>

## Layers as type selections

There is no separate rule engine: a "layer" is just a reusable `Filtered.Types` selection (with the full
filter vocabulary at your disposal), and an architecture rule is just an expectation on it.

```csharp
Filtered.Types domainTypes         = Types.InNamespace("MusicStore.Domain");
Filtered.Types infrastructureTypes = Types.InNamespace("MusicStore.Infrastructure");
Filtered.Types repositoryTypes     = Types.InNamespace("MusicStore.Data").WithName("Repository").AsSuffix();
```

The dependency expectations and filters accept such a selection as a **target**, alongside the namespace and
specific-type forms: `DependsOn` / `DoesNotDependOn` / `DependsOnlyOn` / `HasDependenciesOutside` (and the
plural `DependOn` / `DoNotDependOn` / `DependOnlyOn` / `HaveDependenciesOutside` and the `WhichDependOn` /
`WhichDoNotDependOn` / `WhichDependOnlyOn` / `WhichHaveDependenciesOutside`
filters) take one or more `Filtered.Types` arguments. Multiple targets and `.OrOn(…)` mean *any of*; for the
*only-on* and *outside* families the union is the allowed set, while the own-namespace and framework rules apply
unchanged, including the `.ExcludingOwnSubNamespaces()` opt-out.

```csharp
// Outgoing rule with a selection as target:
await Expect.That(domainTypes).DoNotDependOn(infrastructureTypes);

// Incoming rules are written explicitly from the other side:
await Expect.That(infrastructureTypes).DoNotDependOn(domainTypes);

// Allowed set as union of selections (own namespace + framework stay allowed):
await Expect.That(repositoryTypes).DependOnlyOn(domainTypes).OrOn(infrastructureTypes);
```

Combine several rules into a single verification with aweXpect's `Expect.ThatAll(…)` (see
[combining expectations](https://docs.testably.org/aweXpect/how-it-works/combining)): every rule is
evaluated and all failures are reported together. Any expectation works on a selection, not just the
dependency ones, so naming conventions or sealing rules live in the same check:

```csharp
await Expect.ThatAll(
    Expect.That(domainTypes).DoNotDependOn(infrastructureTypes),
    Expect.That(repositoryTypes).DependOnlyOn(domainTypes).OrOn(infrastructureTypes),
    Expect.That(domainTypes).AreSealed());
```

A failing rule reports all violations, numbered per expectation. Here `PlaylistService` references the
infrastructure layer, and `Album` and `Track` are not sealed:

```text title="Failure message"
Expected all of the following to succeed:
 [01] Expected that types within namespace "MusicStore.Domain" in all loaded assemblies all do not depend on types within namespace "MusicStore.Infrastructure" in all loaded assemblies
 [02] Expected that types within namespace "MusicStore.Data" with name ending with "Repository" in all loaded assemblies all depend only on types within namespace "MusicStore.Domain" in all loaded assemblies or types within namespace "MusicStore.Infrastructure" in all loaded assemblies
 [03] Expected that types within namespace "MusicStore.Domain" in all loaded assemblies all are sealed
but
 [01] it contained types with the dependency
 [03] only 1 of 3 were

[01] Not matching items:
[
  PlaylistService
]

[03] Not matching items:
[
  Album,
  Track
]

[03] Collection:
[
  Album,
  PlaylistService,
  Track
]
```

Exemptions to a rule use the [`Except` filter](./02-filters/index.md#custom-predicates) on the subject selection:

```csharp
await Expect.That(domainTypes.Except<LegacyImportService>()).DoNotDependOn(infrastructureTypes);
await Expect.That(domainTypes.Except(type => type.Name.StartsWith("Generated"))).AreSealed();
```

A layer spanning several namespaces is built by widening a dependency *target* with additional selections
(or `.OrOn(…)`); for a *subject* spanning several namespaces, verify each namespace selection as its own
rule inside the same `Expect.ThatAll(…)`.

<details>
<summary>How a selection is matched as a target</summary>

Each target selection is resolved once per expectation; a dependency matches when it is a member of the union of the
resolved selections. Matching is by type identity, where a generic type definition in the selection (e.g. a scanned
`Repository<>`) matches any of its constructions. For the *only-on* and *outside* families, an empty selection allows
only the own namespace and framework dependencies. A selection is an explicit target, so framework types contained in it are matched normally by
`DependsOn` / `DoesNotDependOn`.

</details>
