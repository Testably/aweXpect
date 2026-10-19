# aweXpect.Reflection

[![Nuget](https://img.shields.io/nuget/v/aweXpect.Reflection)](https://www.nuget.org/packages/aweXpect.Reflection)
[![Build](https://github.com/Testably/aweXpect/actions/workflows/build.yml/badge.svg)](https://github.com/Testably/aweXpect/actions/workflows/build.yml)

Expectations for reflection types for [aweXpect](https://github.com/Testably/aweXpect).

## At a glance

Write architecture and convention tests as plain, readable expectations: **select** the assemblies, types
or members you care about with `In` or `Types`, then **expect** a rule on them with `Expect.That`.

```csharp
// "Every async method must end in 'Async'"
await Expect.That(In.AssemblyContaining<MyClass>()    // ① pick a source
        .Methods()                                    // ② navigate to a member kind
        .WhichAreAsync())                             // ③ filter it down
    .HaveName("Async").AsSuffix();                    // ④ expect
```

Every expectation follows the same four-part shape:

| Step                      | What it does           | Examples                                                                               |
|---------------------------|------------------------|----------------------------------------------------------------------------------------|
| ① **Source**              | choose *where* to look | `In.AllLoadedAssemblies()`, `In.AssemblyContaining<T>()`, `In.Type<T>()`               |
| ② **Navigate**            | move to a member kind  | `.Types()`, `.Methods()`, `.Properties()`, `.Fields()`, `.Events()`, `.Constructors()` |
| ③ **Filter** *(optional)* | narrow the set         | `.WhichArePublic()`, `.With<T>()`, `.WithName(…)`, `.Which(…)`                         |
| ④ **Expect**              | state the rule         | `Expect.That(…).HaveName(…)`, `.AreClasses()`, `.Return<Task>()`                       |

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

## Real-world examples

```csharp
// Verify all test classes (those containing a [Fact] or [Theory] method) follow the naming convention
await Expect.That(In.AllLoadedAssemblies()
        .Public.Classes()
        .WhichContainMethods(m => m.With<FactAttribute>().OrWith<TheoryAttribute>()))
    .HaveName("Tests").AsSuffix();

// Verify all async methods have an "Async" suffix
await Expect.That(In.AssemblyContaining<MyClass>()
        .Methods().WhichAreAsync())
    .HaveName("Async").AsSuffix();

// Verify all methods with an "Async" suffix return Task or ValueTask
await Expect.That(In.AssemblyContaining<MyClass>()
        .Methods().WithName("Async").AsSuffix())
    .Return<Task>().OrReturn<ValueTask>();

// Verify controllers follow the naming convention
await Expect.That(In.AllLoadedAssemblies()
        .Types().WhichInheritFrom<ControllerBase>())
    .HaveName("Controller").AsSuffix();

// Verify each event handler is named after the event it handles (e.g. "OnOrderPlaced")
await Expect.That(In.AssemblyContaining<MyAggregate>()
        .Methods().Which(m => m.GetParameters().Length == 1))
    .HaveName(method => "On" + method.GetParameters()[0].ParameterType.Name);

// Verify each serializable type has exactly one parameterless constructor
await Expect.That(In.AllLoadedAssemblies()
        .Types().With<SerializableAttribute>())
    .ContainConstructors(c => c.WithoutParameters()).Exactly(1);
```

## Overview

Each filter narrows a selection and its expectation counterpart verifies it, e.g. `.WhichArePublic()` and
`.IsPublic()` (`.ArePublic()` for collections):

| Topic                                                                 | Applies to                    | Filters / expectations                                                                                               |
|-----------------------------------------------------------------------|-------------------------------|----------------------------------------------------------------------------------------------------------------------|
| [Access modifiers](../../Docs/extensions/aweXpect.Reflection/02-filters.md#access-modifiers)         | types and members             | `WhichArePublic` / `IsPublic`, `WhichAreInternal` / `IsInternal`, …                                                  |
| [Attributes](../../Docs/extensions/aweXpect.Reflection/02-filters.md#attributes)                     | types, members and assemblies | `With<TAttribute>` / `Has<TAttribute>`, `Without<TAttribute>` / `DoesNotHave<TAttribute>`                            |
| [Obsolete](../../Docs/extensions/aweXpect.Reflection/02-filters.md#obsolete)                         | types and members             | `WhichAreObsolete` / `IsObsolete`, `WhichAreNotObsolete` / `IsNotObsolete`                                           |
| [Names and namespaces](../../Docs/extensions/aweXpect.Reflection/02-filters.md#names-and-namespaces) | types and members             | `WithName` / `HasName`, `WithNamespace` / `HasNamespace`, `WhichAreWithinNamespace` / `IsWithinNamespace`            |
| [Types](../../Docs/extensions/aweXpect.Reflection/02-filters.md#types)                               | types                         | `WhichAreClasses` / `IsAClass`, `WhichInheritFrom<T>` / `InheritsFrom<T>`, `WhichContainMethods` / `ContainsMethods` |
| [Methods](../../Docs/extensions/aweXpect.Reflection/02-filters.md#methods)                           | methods                       | `WhichAreAsync` / `IsAsync`, `WhichReturn<T>` / `Returns<T>`, `WithParameter<T>` / `HasParameter<T>`                 |
| [Operators](../../Docs/extensions/aweXpect.Reflection/02-filters.md#operators)                       | methods and types             | `WhichAreOperators` / `IsAnOperator`, `HasOperator`, `HasImplicitConversionOperator`                                 |
| [Properties & Fields](../../Docs/extensions/aweXpect.Reflection/02-filters.md#properties--fields)    | properties and fields         | `OfType<T>` / `IsOfType<T>`, `WhichAreNullable` / `IsNullable`, `WhichAreRequired` / `IsRequired`                    |
| [Events](../../Docs/extensions/aweXpect.Reflection/02-filters.md#events)                             | events                        | `OfType<T>` / `IsOfType<T>`, `WhichAreStatic` / `IsStatic`, `WhichAreNullable` / `IsNullable`                        |
| [Constructors](../../Docs/extensions/aweXpect.Reflection/02-filters.md#constructors)                 | constructors                  | `WithoutParameters` / `HasNoParameters`, `WithParameter<T>` / `HasParameter<T>`                                      |
| [Assemblies](../../Docs/extensions/aweXpect.Reflection/02-filters.md#assemblies)                     | assemblies                    | `WhichTarget` / `Targets`, `WithVersion` / `HasVersion`, `WhichDependOn` / `DependsOn`                               |

The name filters and expectations accept the
[string matching options](../../Docs/extensions/aweXpect.Reflection/02-filters.md#string-matching-options) like `AsPrefix()`, `AsSuffix()` or
`AsWildcard()`. [Filters and expectations](../../Docs/extensions/aweXpect.Reflection/02-filters.md) documents the complete vocabulary.

## Architecture rules

There is no separate rule engine: a "layer" is just a reusable type selection, and an architecture rule is
just an expectation on it. Combine several rules into a single verification with `Expect.ThatAll`:

```csharp
Filtered.Types domainTypes         = Types.InNamespace("MusicStore.Domain");
Filtered.Types infrastructureTypes = Types.InNamespace("MusicStore.Infrastructure");

await Expect.ThatAll(
    Expect.That(domainTypes).DoNotDependOn(infrastructureTypes),
    Expect.That(domainTypes).AreSealed());
```

A failing rule reports all violations, numbered per expectation:

```text title="Failure message"
Expected all of the following to succeed:
 [01] Expected that types within namespace "MusicStore.Domain" in all loaded assemblies all do not depend on types within namespace "MusicStore.Infrastructure" in all loaded assemblies
 [02] Expected that types within namespace "MusicStore.Domain" in all loaded assemblies are all sealed
but
 [01] it contained types with the dependency
 [02] it contained non-sealed types

[01] Not matching items:
[
  PlaylistService
]

[02] Not matching items:
[
  Album,
  Track
]
```

The dependency rules (`DependOn` / `DependOnlyOn` / `HaveDependenciesOutside`), dependency cycle detection
(`HaveNoDependencyCycles`) and exemptions (`Except`) are documented in
[Architecture rules](../../Docs/extensions/aweXpect.Reflection/03-architecture-rules.md).

## Documentation

The full documentation is available at
[docs.testably.org](https://docs.testably.org/aweXpect/extensions/aweXpect.Reflection/):

- [Selecting types and members](../../Docs/extensions/aweXpect.Reflection/01-sources.md): the `In` and `Types` sources and navigating
  between assemblies, types and members
- [Filters and expectations](../../Docs/extensions/aweXpect.Reflection/02-filters.md): the complete reference for all subject kinds,
  string matching options and quantifiers
- [Architecture rules](../../Docs/extensions/aweXpect.Reflection/03-architecture-rules.md): type dependencies, dependency cycles and
  layers as type selections
- [Configuration](../../Docs/extensions/aweXpect.Reflection/04-configuration.md): assembly exclusions, compiler-generated members and
  the dependency resolver
- Comparisons: feature maps against
  [FluentAssertions](../../Docs/extensions/aweXpect.Reflection/comparison/01-fluentassertions-comparison.mdx) and against
  [NetArchTest and ArchUnitNET](../../Docs/extensions/aweXpect.Reflection/comparison/02-architecture-comparison.mdx)
