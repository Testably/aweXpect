# Architecture rules

An architecture rule restricts which other types a type may reference: its *dependencies*.
[Type dependencies](#type-dependencies) introduces the dependency filters and assertions (including
[dependency cycles](#dependency-cycles)), and [Layers as type selections](#layers-as-type-selections) shows
how to combine them into a full architecture test suite.

## Type dependencies

The dependency filters and assertions follow the familiar filter/assert pairing:

|                              | Filter                                  | Assert (single)                   | Assert (many)                      |
|------------------------------|-----------------------------------------|-----------------------------------|------------------------------------|
| depends on namespace         | `.WhichDependOn("x", …)`                | `.DependsOn("x", …)`              | `.DependOn("x", …)`                |
| does not depend on           | `.WhichDoNotDependOn("x", …)`           | `.DoesNotDependOn("x", …)`        | `.DoNotDependOn("x", …)`           |
| depends only on set          | `.WhichDependOnlyOn("x", …)`            | `.DependsOnlyOn("x", …)`          | `.DependOnlyOn("x", …)`            |
| has dependencies outside set | `.WhichHaveDependenciesOutside("x", …)` | `.HasDependenciesOutside("x", …)` | `.HaveDependenciesOutside("x", …)` |

```csharp
// Presentation must not reference the data layer
await Expect.That(Types.InNamespace("MyApp.Presentation"))
    .DoNotDependOn("MyApp.Data");

// The API layer may only reference the application and domain layers
await Expect.That(Types.InNamespace("MyApp.Api"))
    .DependOnlyOn("MyApp.Application", "MyApp.Domain");

// Filter for the types that depend on a namespace
In.AllLoadedAssemblies().Types().WhichDependOn("System.Data")
```

A type *depends on* every type referenced in its **declared signature**: the base type and directly
implemented interfaces, generic arguments and parameter constraints, field/property/event types, indexer
parameters, method return/parameter/generic-argument types, constructor parameters and the types of attributes
applied to the type, its members, their parameters and return values (including `typeof(…)` and enum attribute
arguments). Element types of
arrays/pointers/by-ref and generic type arguments are unwrapped (`List<Infra.Foo>` depends on `List<Infra.Foo>`,
which also matches a `List<>` target, and on `Infra.Foo`; a closed-generic target like `List<Infra.Bar>`
only matches that exact construction). Purely synthetic references that you never wrote are ignored:
compiler-generated members, the implicit `object`/`ValueType`/`Enum` base type, interfaces inherited from the
base type, records' synthesized `IEquatable<T>`, delegates' runtime infrastructure (only the `Invoke`
signature counts), enums' underlying-value plumbing and the attributes the compiler emits onto authored code
(nullability metadata, required members, async/iterator state machines, …), so the compiler's own plumbing
never counts. Should a future compiler version emit a marker attribute this library does not know about yet,
exclude it yourself via `Customize.aweXpect.Reflection().ExcludedAttributeTypes()` (full attribute type names;
extends the built-in set). Types you write in authored signatures always do count, including primitives and
`void` return types (namespace `System`); in practice, almost every type with members *does* depend on
`System`.

:::warning[Signature-level only]
Dependencies are computed from reflection metadata, so body-level references such
as `new Infra.Foo()`, static calls and local variables are **not** detected. Function-pointer signatures
(`delegate*<…>`) are not decomposed either; the types inside them are invisible to dependency assertions.
Nested types are separate types with their own dependency surface: asserting on `typeof(Outer)` does not
include what `Outer.Inner` references. The collection-based assertions (e.g. over `Types.InNamespace(…)`)
enumerate nested types as their own items and therefore cover them. For IL/body-level accuracy, plug in
your own resolver via `Customize.aweXpect.Reflection().DependencyResolver()` (see
[Configuration](./04-configuration.md#dependency-resolver)).
:::

Namespace matching is ordinal and case-sensitive and, like `WithinNamespace`, includes sub-namespaces by
default (so `Foo.Bar` matches `Foo.Bar.Baz` but not `Foo.BarBaz`). A dependency in the **global namespace**
can be targeted or allowed with an empty string (`""`). Each result is chainable:

```csharp
// Widen the set with .OrOn(…)
await Expect.That(Types.InNamespace("MyApp.Api"))
    .DependOnlyOn("MyApp.Application").OrOn("MyApp.Domain");

// Opt out of sub-namespace matching for the whole expression
await Expect.That(types).DoNotDependOn("MyApp.Data").ExcludingSubNamespaces();
```

For `DependsOnlyOn` a type's own namespace is always allowed, and by default so are its sub-namespaces. Use
`.ExcludingOwnSubNamespaces()` (only available on the *only-on* and *outside* families) to also forbid
references into a type's own sub-namespaces:

```csharp
await Expect.That(Types.InNamespace("MyApp.Domain"))
    .DependOnlyOn("MyApp.Domain").ExcludingSubNamespaces().ExcludingOwnSubNamespaces();
```

`HasDependenciesOutside` is the **positive counterpart** of `DependsOnlyOn` for finding the violators of an
allowed set, without a double-negated "does not depend only on". The allowed set follows the same rules
(sub-namespaces included, the own namespace and framework assemblies never count as outside, the same
chainable refinements):

```csharp
// Select the current violators of an architecture rule (e.g. for a baseline)
In.AllLoadedAssemblies().Types().WhichHaveDependenciesOutside("MyApp.Application", "MyApp.Domain")

// Assert that a known legacy type still has its external dependency
await Expect.That(typeof(LegacyImportService))
    .HasDependenciesOutside("MyApp.Application", "MyApp.Domain");
```

`DependsOn` and `DoesNotDependOn` (single types only) also accept a **specific type** via `<T>()` or
`(Type)`, with `.OrOn<T>()` / `.OrOn(Type)` to widen:

```csharp
await Expect.That(typeof(MyDomainType)).DoesNotDependOn<DbContext>().OrOn<SqlConnection>();
```

All dependency families additionally accept a reusable `Filtered.Types` selection as target; see
[Layers as type selections](#layers-as-type-selections).

:::warning[Framework dependencies are ignored unless you name one explicitly]
`DependOnlyOn` ignores dependencies whose assembly name matches one of the
[`ExcludedAssemblyPrefixes`](./04-configuration.md#assembly-exclusions) at a name-segment boundary: `System`
covers `System` and `System.Text.Json`, but not an assembly named `SystemsBiology` (so you never have to
whitelist `System.*` and unrelated assemblies are never swallowed by a prefix), while a
type's **own namespace** is always allowed. `DependsOn` / `DoesNotDependOn` / `WhichDependOn` still match a
framework namespace when you name it explicitly (e.g. `DoesNotDependOn("System.Data")`).

The default prefixes include `Microsoft`, so `DependOnlyOn` also ignores dependencies on e.g.
`Microsoft.EntityFrameworkCore`, `Microsoft.AspNetCore` and `Microsoft.Extensions.*`; a domain entity
inheriting `DbContext` does **not** fail `DependOnlyOn("MyApp.Domain")`. To forbid such dependencies, name
them explicitly (`DoesNotDependOn<DbContext>()` or `DoNotDependOn("Microsoft.EntityFrameworkCore")`) or
customize the [`ExcludedAssemblyPrefixes`](./04-configuration.md#assembly-exclusions). Note that the
customization also affects assembly scanning and assembly-level dependency assertions.
:::

### Dependency cycles

The "slices should be free of cycles" architecture rule: assert that the namespaces of a set of types do not
(transitively) depend on each other.

```csharp
// No dependency cycles among the namespaces under MyApp
await Expect.That(Types.InNamespace("MyApp"))
    .HaveNoDependencyCycles();
```

A namespace `A` *depends on* a namespace `B` when some type in `A` references a type in `B` (in its
[signature](#type-dependencies), read through the same resolver as the other dependency assertions). The
namespaces of the analyzed types form the nodes of a directed graph, and each
[strongly-connected component](https://en.wikipedia.org/wiki/Strongly_connected_component) with more than one
node is reported as a cycle, e.g. `MyApp.Orders -> MyApp.Billing -> MyApp.Orders`. Only namespaces present in
the analyzed set form nodes, so dependencies on framework or otherwise out-of-set namespaces never create an
edge, and a namespace referencing itself is not a cycle.

By default a namespace and its sub-namespaces collapse into a single node (a family), consistent with how the
other dependency assertions treat a type's own sub-namespaces. So a reference between a namespace and its
ancestor/descendant (e.g. `MyApp.Orders` ↔ `MyApp.Orders.Domain`) never creates an edge and cannot by itself form
a cycle. But because the family is one node (not just a suppressed pair of edges), a cycle that leaves the family
and returns through a *different* member of it (e.g. `MyApp.Orders -> MyApp.Billing -> MyApp.Orders.Domain`) is
still detected. Use `ExcludingSubNamespaces()` to treat every namespace as its own node, so that such a
parent/child reference becomes an edge (and can form a cycle):

```csharp
// Treat every namespace as its own node (MyApp.Orders ↔ MyApp.Orders.Domain can now form a cycle)
await Expect.That(Types.InNamespace("MyApp"))
    .HaveNoDependencyCycles().ExcludingSubNamespaces();
```

Pass a **slice root** to group all namespaces below it into one slice each (by the namespace segment immediately
following the root), so that, for example, `MyApp.Orders`, `MyApp.Orders.Domain` and `MyApp.Orders.Api` collapse
into the single slice `MyApp.Orders`:

```csharp
// Group MyApp.Orders.* / MyApp.Billing.* / … into one slice each before looking for cycles
await Expect.That(Types.InNamespace("MyApp"))
    .HaveNoDependencyCycles("MyApp");
```

Because the edges come from the same dependency resolution as the other dependency assertions, configuring a
[custom dependency resolver](./04-configuration.md#dependency-resolver) (e.g. an IL-level one) also sharpens
cycle detection: body-level references it surfaces can complete a cycle that the signature-level default
cannot see.

## Layers as type selections

There is no separate rule engine: a "layer" is just a reusable `Filtered.Types` selection (with the full
filter vocabulary at your disposal), and an architecture rule is just an expectation on it.

```csharp
Filtered.Types domain         = Types.InNamespace("MyApp.Domain");
Filtered.Types infrastructure = Types.InNamespace("MyApp.Infrastructure");
Filtered.Types repositories   = Types.InNamespace("MyApp.Data").WithName("Repository").AsSuffix();
```

The dependency assertions and filters accept such a selection as a **target**, alongside the namespace and
specific-type forms: `DependsOn` / `DoesNotDependOn` / `DependsOnlyOn` / `HasDependenciesOutside` (and the
plural `DependOn` / `DoNotDependOn` / `DependOnlyOn` / `HaveDependenciesOutside` and the `WhichDependOn` /
`WhichDoNotDependOn` / `WhichDependOnlyOn` / `WhichHaveDependenciesOutside`
filters) take one or more `Filtered.Types` arguments. Each target selection is resolved once per assertion;
a dependency matches when it is a member of the union of the resolved selections. Matching is by type
identity, where a generic type definition in the selection (e.g. a scanned `Repository<>`) matches any of
its constructions.
Multiple targets and `.OrOn(…)` mean *any of*; for the *only-on* and *outside* families the union is the
allowed set, while the own-namespace and framework rules apply unchanged, including the
`.ExcludingOwnSubNamespaces()` opt-out (an empty selection thus allows only the own namespace
and framework dependencies). A selection is an explicit target, so framework types contained in it are
matched normally by `DependsOn` / `DoesNotDependOn`.

```csharp
// Outgoing rule with a selection as target:
await Expect.That(domain).DoNotDependOn(infrastructure);

// Incoming rules are written explicitly from the other side:
await Expect.That(infrastructure).DoNotDependOn(domain);

// Allowed set as union of selections (own namespace + framework stay allowed):
await Expect.That(domain).DependOnlyOn(repositories).OrOn(infrastructure);
```

Combine several rules into a single verification with aweXpect's `Expect.ThatAll(…)` (see
[multiple expectations](https://docs.testably.org/aweXpect/advanced/multiple-expectations)): every rule is
evaluated and all failures are reported together. Any assertion works on a selection, not just the
dependency ones, so naming conventions or sealing rules live in the same check:

```csharp
await Expect.ThatAll(
    Expect.That(domain).DoNotDependOn(infrastructure),
    Expect.That(domain).DependOnlyOn(repositories).OrOn(infrastructure),
    Expect.That(domain).AreSealed());
```

A failing rule reports all violations, numbered per expectation:

```
Expected all of the following to succeed:
 [01] Expected that domain all do not depend on types within namespace "MyApp.Infrastructure" in all loaded assemblies
 [02] Expected that domain are all sealed
but
 [01] it contained types with the dependency [
  OrderService
]
 [02] it contained non-sealed types [
  Order,
  Invoice
]
```

Exemptions to a rule use the [`Except` filter](./02-filters.md) on the subject selection:

```csharp
await Expect.That(domain.Except<LegacyService>()).DoNotDependOn(infrastructure);
await Expect.That(domain.Except(type => type.Name.StartsWith("Generated"))).AreSealed();
```

A layer spanning several namespaces is built by widening a dependency *target* with additional selections
(or `.OrOn(…)`); for a *subject* spanning several namespaces, assert each namespace selection as its own
rule inside the same `Expect.ThatAll(…)`.
