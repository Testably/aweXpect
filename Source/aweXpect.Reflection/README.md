# aweXpect.Reflection

[![Nuget](https://img.shields.io/nuget/v/aweXpect.Reflection)](https://www.nuget.org/packages/aweXpect.Reflection)
[![Build](https://github.com/Testably/aweXpect.Reflection/actions/workflows/build.yml/badge.svg)](https://github.com/Testably/aweXpect.Reflection/actions/workflows/build.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Testably_aweXpect.Reflection&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Testably_aweXpect.Reflection)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Testably_aweXpect.Reflection&metric=coverage)](https://sonarcloud.io/summary/overall?id=Testably_aweXpect.Reflection)
[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2FTestably%2FaweXpect.Reflection%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/Testably/aweXpect.Reflection/main)

Expectations for reflection types for [aweXpect](https://github.com/Testably/aweXpect).

## At a glance

Write architecture and convention tests as plain, readable assertions: **select** the assemblies, types
or members you care about with `In` or `Types`, then **assert** a rule on them with `Expect.That`.

```csharp
// "Every async method must end in 'Async'"
await Expect.That(In.AssemblyContaining<MyClass>()    // ① pick a source
        .Methods()                                    // ② navigate to a member kind
        .WhichAreAsync())                             // ③ filter it down
    .HaveName("Async").AsSuffix();                    // ④ assert
```

Every expectation follows the same four-part shape:

| Step                      | What it does           | Examples                                                                               |
|---------------------------|------------------------|----------------------------------------------------------------------------------------|
| ① **Source**              | choose *where* to look | `In.AllLoadedAssemblies()`, `In.AssemblyContaining<T>()`, `In.Type<T>()`               |
| ② **Navigate**            | move to a member kind  | `.Types()`, `.Methods()`, `.Properties()`, `.Fields()`, `.Events()`, `.Constructors()` |
| ③ **Filter** *(optional)* | narrow the set         | `.WhichArePublic()`, `.With<T>()`, `.WithName(…)`, `.Which(…)`                         |
| ④ **Assert**              | state the rule         | `Expect.That(…).HaveName(…)`, `.AreClasses()`, `.Return<Task>()`                       |

Steps ② and ③ are optional. You can assert directly on a single `Type`, `MethodInfo`, … or on a whole
`Assembly`, and every expectation works the same whether the subject is one item or a collection
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
        .Types().With<SerializableAttribute>()
        .WhichContainConstructors(c => c.WithoutParameters()).Exactly(1))
    .AreClasses();
```

[Filters and assertions](../../Docs/extensions/aweXpect.Reflection/02-filters.md) documents the complete filter and assertion
vocabulary: access modifiers, attributes, names and namespaces, type kinds, methods (return types,
parameters, async, operators, …), properties, fields, events, constructors and assemblies.

## Architecture rules

There is no separate rule engine: a "layer" is just a reusable type selection, and an architecture rule is
just an expectation on it. Combine several rules into a single verification with `Expect.ThatAll`:

```csharp
Filtered.Types domainTypes         = Types.InNamespace("MyApp.Domain");
Filtered.Types infrastructureTypes = Types.InNamespace("MyApp.Infrastructure");

await Expect.ThatAll(
    Expect.That(domainTypes).DoNotDependOn(infrastructureTypes),
    Expect.That(domainTypes).AreSealed());
```

A failing rule reports all violations, numbered per expectation:

```
Expected all of the following to succeed:
 [01] Expected that domainTypes all do not depend on types within namespace "MyApp.Infrastructure" in all loaded assemblies
 [02] Expected that domainTypes are all sealed
but
 [01] it contained types with the dependency
 [02] it contained non-sealed types

[01] Not matching items:
[
  OrderService
]

[02] Not matching items:
[
  Order,
  Invoice
]
```

The dependency rules (`DependOn` / `DependOnlyOn` / `HaveDependenciesOutside`), dependency cycle detection
(`HaveNoDependencyCycles`) and exemptions (`Except`) are documented in
[Architecture rules](../../Docs/extensions/aweXpect.Reflection/03-architecture-rules.md).

## Documentation

The full documentation is available at
[docs.testably.org](https://docs.testably.org/Extensions/aweXpect.Reflection/):

- [Selecting types and members](../../Docs/extensions/aweXpect.Reflection/01-sources.md): the `In` and `Types` sources and navigating
  between assemblies, types and members
- [Filters and assertions](../../Docs/extensions/aweXpect.Reflection/02-filters.md): the complete reference for all subject kinds,
  string matching options and quantifiers
- [Architecture rules](../../Docs/extensions/aweXpect.Reflection/03-architecture-rules.md): type dependencies, dependency cycles and
  layers as type selections
- [Configuration](../../Docs/extensions/aweXpect.Reflection/04-configuration.md): assembly exclusions, compiler-generated members and
  the dependency resolver
- Comparisons: feature maps against
  [FluentAssertions](../../Docs/extensions/aweXpect.Reflection/comparison/01-fluentassertions-comparison.mdx) and against
  [NetArchTest and ArchUnitNET](../../Docs/extensions/aweXpect.Reflection/comparison/02-architecture-comparison.mdx)
