---
description: aweXpect.Reflection extends aweXpect with expectations for assemblies, types and members, for architecture and convention tests.
---

import PackageInstall from '@site/src/components/PackageInstall';

# aweXpect.Reflection

[aweXpect.Reflection](https://github.com/Testably/aweXpect/tree/main/Source/aweXpect.Reflection) extends [aweXpect](/aweXpect) with expectations for reflection types, so you can write architecture and convention tests for assemblies, types and members.

## Installation

<PackageInstall package="aweXpect.Reflection" prerelease />

The package brings `Expect.That` with it, but not the expectations for strings, numbers or collections, so reference
[`aweXpect`](https://www.nuget.org/packages/aweXpect) as well. The expectations live in the namespace `aweXpect`; the
sources `In` and `Types`, their filters and the [customization](./04-customization.md) need
`using aweXpect.Reflection;`, and a stored selection like `Filtered.Types` needs
`using aweXpect.Reflection.Collections;`.

The examples write `That(...)` instead of `Expect.That(...)`, which needs `using static aweXpect.Expect;` (see [Getting started](/aweXpect/getting-started#install-the-package)).

## At a glance

Write architecture and convention tests as plain, readable expectations: **select** the assemblies, types
or members you care about with `In` or `Types`, then **expect** a rule on them with `Expect.That`.

```csharp
// "Every async method must end in 'Async'"
await That(In.AssemblyContaining<PlaylistService>()           // ① pick a source
        .Methods()                                     // ② navigate to a member kind
        .WhichAreAsync())                              // ③ filter it down
    .HaveName("Async").AsSuffix();                     // ④ expect
```

Every expectation follows the same four-part shape:

| Step                      | What it does           | Examples                                                                               |
|---------------------------|------------------------|----------------------------------------------------------------------------------------|
| ① **Source**              | choose *where* to look | `In.AllLoadedAssemblies()`, `In.AssemblyContaining<T>()`, `In.Type<T>()`               |
| ② **Navigate**            | move to a member kind  | `.Types()`, `.Methods()`, `.Properties()`, `.Fields()`, `.Events()`, `.Constructors()` |
| ③ **Filter** *(optional)* | narrow the set         | `.WhichArePublic()`, `.With<T>()`, `.WithName(…)`, `.Which(…)`                         |
| ④ **Expect**              | state the rule         | `That(…).HaveName(…)`, `.AreClasses()`, `.Return<Task>()`                       |

Steps ② and ③ are optional. You can verify a single `Type`, `MethodInfo`, … or a whole `Assembly`
directly, and every expectation works the same whether the subject is one item or a collection
(`Assembly[]`, `IEnumerable<Type?>`, …).

The supported reflection subjects are
[`Assembly`](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.assembly),
[`Type`](https://learn.microsoft.com/en-us/dotnet/api/system.type),
[`ConstructorInfo`](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.constructorinfo),
[`EventInfo`](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.eventinfo),
[`FieldInfo`](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.fieldinfo),
[`MethodInfo`](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.methodinfo) and
[`PropertyInfo`](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo).

### Real-world examples

```csharp
// Verify all test classes (those containing a [Fact] or [Theory] method) follow the naming convention
await That(In.AllLoadedAssemblies()
        .Public.Classes()
        .WhichContainMethods(m => m.With<FactAttribute>().OrWith<TheoryAttribute>()))
    .HaveName("Tests").AsSuffix();

// Verify all async methods have an "Async" suffix
await That(In.AssemblyContaining<PlaylistService>()
        .Methods().WhichAreAsync())
    .HaveName("Async").AsSuffix();

// Verify all methods with an "Async" suffix return Task or ValueTask
await That(In.AssemblyContaining<PlaylistService>()
        .Methods().WithName("Async").AsSuffix())
    .Return<Task>().OrReturn<ValueTask>();

// Verify controllers follow the naming convention
await That(In.AllLoadedAssemblies()
        .Types().WhichInheritFrom<ControllerBase>())
    .HaveName("Controller").AsSuffix();

// Verify each event handler is named after the event it handles (e.g. "OnTrackAdded")
await That(In.AssemblyContaining<Playlist>()
        .Methods().Which(m => m.GetParameters().Length == 1))
    .HaveName(method => "On" + method.GetParameters()[0].ParameterType.Name);

// Verify each serializable type has exactly one parameterless constructor
await That(In.AllLoadedAssemblies()
        .Types().With<SerializableAttribute>())
    .ContainConstructors(c => c.WithNoParameters()).Exactly(1);
```

### Architecture rules

There is no separate rule engine: a "layer" is just a reusable type selection, and an architecture rule is
just an expectation on it. Combine several rules into a single verification with `Expect.ThatAll`:

```csharp
Filtered.Types domainTypes         = Types.InNamespace("MusicStore.Domain");
Filtered.Types infrastructureTypes = Types.InNamespace("MusicStore.Infrastructure");

await Expect.ThatAll(
    That(domainTypes).DoNotDependOn(infrastructureTypes),
    That(domainTypes).AreSealed());
```

A failing rule reports all violations, numbered per expectation:

```text title="Failure message"
Expected all of the following to succeed:
 [01] Expected that types within namespace "MusicStore.Domain" in all loaded assemblies all do not depend on types within namespace "MusicStore.Infrastructure" in all loaded assemblies
 [02] Expected that types within namespace "MusicStore.Domain" in all loaded assemblies all are sealed
but
 [01] it contained types with the dependency
 [02] only 1 of 3 were

[01] Not matching items:
[
  PlaylistService
]

[02] Not matching items:
[
  Album,
  Track
]

[02] Collection:
[
  Album,
  PlaylistService,
  Track
]
```

The dependency rules (`DependOn` / `DependOnlyOn` / `HaveDependenciesOutside`), dependency cycle detection
(`HaveNoDependencyCycles`) and exemptions (`Except`) are documented in
[Architecture rules](./03-architecture-rules.md).

## Overview

Each filter narrows a selection and its expectation counterpart verifies it, e.g. `.WhichArePublic()` and
`.IsPublic()` (`.ArePublic()` for collections):

| Page                                                                               | Covers                                                                                       | Examples                                                                          |
|------------------------------------------------------------------------------------|----------------------------------------------------------------------------------------------|-----------------------------------------------------------------------------------|
| [Common filters](./02-filters/01-common-filters.md)                                | access modifiers, attributes, the obsolete marker, names and namespaces of types and members | `WhichArePublic` / `IsPublic`, `With<TAttribute>` / `Has<TAttribute>`             |
| [Assemblies](./02-filters/02-assemblies.md)                                        | name, target framework, version, strong name and dependencies                                | `WhichTarget` / `Targets`, `WhichDependOn` / `DependsOn`                          |
| [Types and inheritance](./02-filters/03-types.md)                                  | kinds of types, base classes, interfaces and contained members, and how inherited members count | `WhichAreClasses` / `IsAClass`, `WhichInheritFrom<T>` / `InheritsFrom<T>`      |
| [Methods, operators and constructors](./02-filters/04-methods.md)                  | return types and parameters                                                                  | `WhichAreAsync` / `IsAsync`, `WhichReturn<T>` / `Returns<T>`, `WithParameter<T>` / `HasParameter<T>` |
| [Properties, fields and events](./02-filters/05-properties-fields-and-events.md)   | member types, nullability and accessors                                                      | `WhichAreOfType<T>` / `IsOfType<T>`, `WhichAreRequired` / `IsRequired`            |
| [Architecture rules](./03-architecture-rules.md)                                   | type dependencies, dependency cycles and layers as type selections                           | `DependOn` / `DependOnlyOn` / `HaveDependenciesOutside`, `HaveNoDependencyCycles` |

The [overview](./02-filters/00-index.md#overview) lists all topics,
including combined filters, custom predicates (`Which` / `Satisfies`, `Except`) and quantifiers.

Each filter and expectation has its negation where it makes sense, e.g. `WithoutName` / `DoesNotHaveName`,
`WithoutNamespace` / `DoesNotHaveNamespace`, `WhichAreNotOfType<T>` / `IsNotOfType<T>`, `WhichDoNotReturn<T>` /
`DoesNotReturn<T>`, `WithoutParameter<T>` / `DoesNotHaveParameter<T>` or `WhichDoNotTarget` / `DoesNotTarget`.
On a collection, a negated expectation (`DoNotReturn<T>`, `AreNotOfType<T>`, …) verifies that *none* of the items
match. The tables in
[Filters and expectations](./02-filters/00-index.md#how-to-read-the-tables) list every negation in a **Negated** column.

The name filters and expectations accept the
[string matching options](./02-filters/01-common-filters.md#string-matching-options) like `AsPrefix()`,
`AsSuffix()` or `AsWildcard()`. [Filters and expectations](./02-filters/00-index.md) documents the complete vocabulary.

## Customization

Set a value under `Customize.aweXpect.Reflection()` for the current
[async flow](/aweXpect/how-it-works/configuration#lifetimes-and-async-flows), or under
`Customize.aweXpect.Global.Reflection()` for
[all tests](/aweXpect/how-it-works/configuration#global-defaults).

| Value                                                                                  | Default                           | Summary                                                                         |
|----------------------------------------------------------------------------------------|-----------------------------------|---------------------------------------------------------------------------------|
| [`Settings().ExcludedAssemblyPrefixes`](./04-customization.md#assembly-exclusions)     | `System`, `Microsoft`, …          | assemblies excluded from `In.AllLoadedAssemblies()` and from `DependOnlyOn`     |
| [`IncludedCompilerGeneratedMembers`](./04-customization.md#compiler-generated-members) | `CompilerGeneratedMembers.None`   | the compiler-generated types and members included in the navigations            |
| [`IncludedSpecialNameMembers`](./04-customization.md#compiler-generated-members)       | `SpecialNameMembers.None`         | the operators and accessors included in `.Methods()`                            |
| [`DependencyResolver`](./04-customization.md#dependency-resolver)                      | built-in signature-level resolver | how the type-level dependency expectations determine the dependencies of a type |
| [`ExcludedAttributeTypes`](./03-architecture-rules.md#type-dependencies)               | none                              | additional compiler-emitted attributes that never count as dependencies         |

## Documentation

- [Selecting types and members](./01-sources.md): the `In` and `Types` sources and navigating
  between assemblies, types and members
- [Filters and expectations](./02-filters/00-index.md): the complete reference for all subject kinds, string matching
  options and quantifiers
- [Architecture rules](./03-architecture-rules.md): type dependencies, dependency cycles and
  layers as type selections
- [Customization](./04-customization.md): assembly exclusions, compiler-generated members and
  the dependency resolver
- Comparisons: feature maps against
  [FluentAssertions](./05-comparison/01-fluentassertions-comparison.mdx) and against
  [NetArchTest and ArchUnitNET](./05-comparison/02-architecture-comparison.mdx)
