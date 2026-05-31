# aweXpect.Reflection

[![Nuget](https://img.shields.io/nuget/v/aweXpect.Reflection)](https://www.nuget.org/packages/aweXpect.Reflection)
[![Build](https://github.com/Testably/aweXpect.Reflection/actions/workflows/build.yml/badge.svg)](https://github.com/Testably/aweXpect.Reflection/actions/workflows/build.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Testably_aweXpect.Reflection&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Testably_aweXpect.Reflection)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Testably_aweXpect.Reflection&metric=coverage)](https://sonarcloud.io/summary/overall?id=Testably_aweXpect.Reflection)
[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2FTestably%2FaweXpect.Reflection%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/Testably/aweXpect.Reflection/main)

Expectations for reflection types for [aweXpect](https://github.com/Testably/aweXpect).

Write architecture and convention tests as plain, readable assertions: **select** the assemblies, types
or members you care about with `In`, then **assert** a rule on them with `Expect.That`.

## At a glance

```csharp
// "Every async method must end in 'Async'"
await Expect.That(In.AssemblyContaining<MyClass>()    // ① pick a source
        .Methods()                                    // ② navigate to a member kind
        .WhichReturn<Task>().OrReturn<ValueTask>())   // ③ filter it down
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
// Verify all test classes follow the naming convention
await Expect.That(In.AllLoadedAssemblies()
        .Public.Classes()
        .WhichContainMethods(m => m.With<FactAttribute>().OrWith<TheoryAttribute>()))
    .HaveName("Tests").AsSuffix();

// Verify all async methods have an "Async" suffix
await Expect.That(In.AssemblyContaining<MyClass>()
        .Methods().WhichReturn<Task>().OrReturn<ValueTask>())
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

// Verify all test classes (those containing a [Fact] or [Theory] method) follow the naming convention
await Expect.That(In.AllLoadedAssemblies()
        .Types().WhichContainMethods(m => m.With<FactAttribute>().OrWith<TheoryAttribute>()))
    .HaveName("Tests").AsSuffix();

// Verify each serializable type has exactly one parameterless constructor
await Expect.That(In.AllLoadedAssemblies()
        .Types().With<SerializableAttribute>()
        .WhichContainConstructors(c => c.WithoutParameters()).Exactly(1))
    .AreClasses();
```

## Sources: the `In` helper

`In` builds the collection of reflection objects you want to reason about. Every source returns a lazily
evaluated collection that you can navigate and filter further.

| Source                                                                  | Returns                                                                              |
|-------------------------------------------------------------------------|--------------------------------------------------------------------------------------|
| `In.AllLoadedAssemblies()`                                              | all currently loaded assemblies (system assemblies [excluded](#assembly-exclusions)) |
| `In.Assemblies(a1, a2, …)` / `In.Assemblies(collection)`                | the given assemblies                                                                 |
| `In.AssemblyContaining<T>()` / `In.AssemblyContaining(typeof(T))`       | the assembly that declares `T`                                                       |
| `In.EntryAssembly()`                                                    | the entry assembly                                                                   |
| `In.ExecutingAssembly()`                                                | the executing assembly                                                               |
| `In.Type<T>()` / `In.Type(typeof(T))`                                   | a single type                                                                        |
| `In.Types<T1, T2>()` / `In.Types<T1, T2, T3>()` / `In.Types(t1, t2, …)` | the given types                                                                      |

## Navigating to members

From a collection of assemblies or types you can navigate to the members they contain, and from members
back to their declaring types.

| From               | Navigate with                                                              | Yields                   |
|--------------------|----------------------------------------------------------------------------|--------------------------|
| assemblies         | `.Types()`                                                                 | the contained types      |
| assemblies / types | `.Methods()`, `.Properties()`, `.Fields()`, `.Events()`, `.Constructors()` | the contained members    |
| members            | `.DeclaringTypes()`                                                        | the declaring types      |
| types              | `.Assemblies()`                                                            | the declaring assemblies |

Assemblies also expose shorthand terminals that select a type *kind* directly:
`.Classes()`, `.Interfaces()`, `.Enums()`, `.Structs()`, `.Records()`, `.RecordStructs()`.

```csharp
// All types in all (non-system) assemblies
In.AllLoadedAssemblies().Types()

// Navigate from members back to their declaring types
In.AllLoadedAssemblies().Methods().DeclaringTypes()

// Shorthand: all public classes
In.AllLoadedAssemblies().Public.Classes()
```

## Filters and the matching assertions

A **filter** (`.WhichAre…` / `.With…`) narrows a collection *before* you assert on it. An **assertion**
(`Expect.That(…).Is… / .Are… / .Has… / .Have…`) states the rule the subject must satisfy. They mirror
each other: a filter has an `Is…`/`Are…` assertion counterpart for the same concept.

The tables below pair them up. The **Filter** column is used inside `In.…`; the **Assert (single)**
column applies to one subject; the **Assert (many)** column applies to a collection.

### Access modifiers

Shared by all types and members: these names are identical for types, methods, properties, fields,
events and constructors.

| Modifier           | Filter                                                 | Assert (single)          | Assert (many)             |
|--------------------|--------------------------------------------------------|--------------------------|---------------------------|
| public             | `.WhichArePublic()` / `.Public`                        | `.IsPublic()`            | `.ArePublic()`            |
| internal           | `.WhichAreInternal()` / `.Internal`                    | `.IsInternal()`          | `.AreInternal()`          |
| private            | `.WhichArePrivate()` / `.Private`                      | `.IsPrivate()`           | `.ArePrivate()`           |
| protected          | `.WhichAreProtected()` / `.Protected`                  | `.IsProtected()`         | `.AreProtected()`         |
| private protected  | `.WhichArePrivateProtected()` / `.Private.Protected`   | `.IsPrivateProtected()`  | `.ArePrivateProtected()`  |
| protected internal | `.WhichAreProtectedInternal()` / `.Protected.Internal` | `.IsProtectedInternal()` | `.AreProtectedInternal()` |

```csharp
// Filter, then assert
In.AllLoadedAssemblies().Public.Methods()        // shorthand modifier
In.AllLoadedAssemblies().Methods().WhichArePublic()

await Expect.That(method).IsPublic();
await Expect.That(methods).ArePublic();
```

### Attributes

Shared by all types, members, and assemblies.

|                                    | Filter                      | Assert (single)            | Assert (many)               |
|------------------------------------|-----------------------------|----------------------------|-----------------------------|
| has attribute                      | `.With<TAttribute>()`       | `.Has<TAttribute>()`       | `.Have<TAttribute>()`       |
| has attribute matching a predicate | `.With<TAttribute>(a => …)` | `.Has<TAttribute>(a => …)` | `.Have<TAttribute>(a => …)` |
| any of several attributes          | `.With<T1>().OrWith<T2>()`  | -                          | -                           |

All attribute filters and assertions (`With`, `OrWith`, `Has`, `Have`, `OrHas`, `OrHave`) take an optional
`inherit` parameter (default `true`) that controls whether attributes inherited from base types are
considered: `.With<TAttribute>(inherit: false)`.

```csharp
await Expect.That(type).Has<ObsoleteAttribute>(a => a.Message == "Use NewClass instead");
await Expect.That(methods).Have<FactAttribute>();
```

### Names and namespaces

Shared by all types and members.

|                                 | Filter                  | Assert (single)           | Assert (many)              |
|---------------------------------|-------------------------|---------------------------|----------------------------|
| by name                         | `.WithName("x")`        | `.HasName("x")`           | `.HaveName("x")`           |
| by namespace *(types only)*     | `.WithNamespace("x")`   | `.HasNamespace("x")`      | `.HaveNamespace("x")`      |
| within namespace *(types only)* | `.WithinNamespace("x")` | `.IsWithinNamespace("x")` | `.AreWithinNamespace("x")` |

The name/namespace *equality* filters and assertions (`WithName`, `WithNamespace`, and their
`Has`/`Have` counterparts) accept the
[string matching options](#string-matching-options) (`AsPrefix`, `AsSuffix`, `AsWildcard`, `AsRegex`,
`IgnoringCase`, …). Collection assertions also accept a selector to derive the expected name per item:

```csharp
await Expect.That(types).HaveName("Service").AsSuffix();
await Expect.That(methods).HaveName(m => "On" + m.GetParameters()[0].ParameterType.Name);
```

The `WithinNamespace`/`IsWithinNamespace`/`AreWithinNamespace` variants match a namespace and all its
sub-namespaces (so `Foo.Bar` includes `Foo.Bar.Baz` but not `Foo.BarBaz`). They compare the namespace
exactly and case-sensitively and do not support any of the string matching options. Each has a negated
form — `NotWithinNamespace`, `IsNotWithinNamespace` and `AreNotWithinNamespace` — that matches types
outside the namespace.

### Types

| Kind             | Filter                                          | Assert (single)      | Assert (many)         |
|------------------|-------------------------------------------------|----------------------|-----------------------|
| class            | `.WhichAreClasses()` / `.Classes()`             | `.IsAClass()`        | `.AreClasses()`       |
| interface        | `.WhichAreInterfaces()` / `.Interfaces()`       | `.IsAnInterface()`   | `.AreInterfaces()`    |
| enum             | `.WhichAreEnums()` / `.Enums()`                 | `.IsAnEnum()`        | `.AreEnums()`         |
| struct           | `.WhichAreStructs()` / `.Structs()`             | `.IsAStruct()`       | `.AreStructs()`       |
| record           | `.WhichAreRecords()` / `.Records()`             | `.IsARecord()`       | `.AreRecords()`       |
| record struct    | `.WhichAreRecordStructs()` / `.RecordStructs()` | `.IsARecordStruct()` | `.AreRecordStructs()` |
| abstract         | `.WhichAreAbstract()` / `.Abstract`             | `.IsAbstract()`      | `.AreAbstract()`      |
| sealed           | `.WhichAreSealed()` / `.Sealed`                 | `.IsSealed()`        | `.AreSealed()`        |
| static           | `.WhichAreStatic()` / `.Static`                 | `.IsStatic()`        | `.AreStatic()`        |
| generic          | `.WhichAreGeneric()` / `.Generic`               | `.IsGeneric()`       | `.AreGeneric()`       |
| nested           | `.WhichAreNested()` / `.Nested`                 | `.IsNested()`        | `.AreNested()`        |
| inherits from    | `.WhichInheritFrom<T>()`                        | `.InheritsFrom<T>()` | `.InheritFrom<T>()`   |
| custom predicate | `.Which(t => …)`                                | -                    | -                     |

`WhichInheritFrom` / `InheritsFrom` accept a generic argument or a `Type`, plus an optional
`forceDirect` flag to require *direct* inheritance.

```csharp
In.AllLoadedAssemblies().Types()
    .WhichAreClasses().WhichArePublic()
    .WithName("Service").AsSuffix()
    .WhichInheritFrom<BaseService>()

// Shorthand for the same access/kind filters
In.AllLoadedAssemblies().Public.Abstract.Classes()
```

> **Negation:** every kind/modifier row above has a negated form: `WhichAreNot…` on filters and
> `IsNot…` / `AreNot…` on assertions (e.g. `WhichAreNotSealed()`, `IsNotAClass()`, `AreNotStatic()`).

#### Types containing specific members

You can select types based on the members they declare. The lambda receives the members declared on each
individual type and may use the full member-filter API:

```csharp
// Types that contain at least one method with [Fact] or [Theory]
In.AllLoadedAssemblies().Types()
    .WhichContainMethods(methods => methods.With<FactAttribute>().OrWith<TheoryAttribute>())

// The same is available for the other member kinds
In.AllLoadedAssemblies().Types()
    .WhichContainProperties(properties => properties.With<RequiredAttribute>())
    .WhichContainFields(fields => fields.WithName("_").AsPrefix())
    .WhichContainEvents(events => events.With<ObsoleteAttribute>())
    .WhichContainConstructors(constructors => constructors.WithoutParameters())
```

By default a type matches when it contains **at least one** matching member. Append a quantifier (the same
quantifiers as in [`aweXpect`](https://docs.testably.org/aweXpect)) to require a specific count:

```csharp
In.AllLoadedAssemblies().Types()
    .WhichContainConstructors(c => c.WithoutParameters()).Exactly(1)
    .WhichContainMethods(m => m.With<ObsoleteAttribute>()).Never()
    .WhichContainProperties(p => p.With<RequiredAttribute>()).AtLeast(2)
    .WhichContainFields(f => f.WhichArePrivate()).Between(1).And(5)
```

Each quantifier applies only to the condition it directly follows; all other conditions implicitly require
the member to occur _at least once_. The available quantifiers are `Exactly`, `AtLeast`, `AtMost`,
`MoreThan`, `LessThan`, `Between(…).And(…)`, `Never`, `Once` and `Twice`.

The same five member kinds are available as **assertions** on a single `Type` (`Contains…`) and on a
collection of types (`Contain…`), using the same member-filter lambdas and the same quantifiers (default:
_at least one_ matching member):

| Member kind  | Filter                         | Assert (single)            | Assert (many)             |
|--------------|--------------------------------|----------------------------|---------------------------|
| methods      | `.WhichContainMethods(…)`      | `.ContainsMethods(…)`      | `.ContainMethods(…)`      |
| properties   | `.WhichContainProperties(…)`   | `.ContainsProperties(…)`   | `.ContainProperties(…)`   |
| fields       | `.WhichContainFields(…)`       | `.ContainsFields(…)`       | `.ContainFields(…)`       |
| events       | `.WhichContainEvents(…)`       | `.ContainsEvents(…)`       | `.ContainEvents(…)`       |
| constructors | `.WhichContainConstructors(…)` | `.ContainsConstructors(…)` | `.ContainConstructors(…)` |

```csharp
// A single type contains at least one [Fact] or [Theory] method
await Expect.That(typeof(MyTests))
    .ContainsMethods(methods => methods.With<FactAttribute>().OrWith<TheoryAttribute>());

// …with exactly two parameterless constructors
await Expect.That(typeof(MyService))
    .ContainsConstructors(constructors => constructors.WithoutParameters()).Exactly(2.Times());

// Every type in a collection contains a matching property
await Expect.That(types)
    .ContainProperties(properties => properties.With<RequiredAttribute>()).AtLeast(1.Times());
```

#### Generic type arguments

After `.WhichAreGeneric()` you can drill into the generic arguments themselves. The same filters are
available on generic **methods** (`.Methods().WhichAreGeneric()`).

| Filter                       | Selects generic types/methods…                                                        |
|------------------------------|---------------------------------------------------------------------------------------|
| `.WithArgumentCount(n)`      | with exactly `n` generic arguments                                                    |
| `.WithArgument<T>()`         | with a generic argument constrained to `T`                                            |
| `.WithArgument<T>("name")`   | with a generic argument constrained to `T` and named `"name"`                         |
| `.WithArgument("name")`      | with a generic argument named `"name"`                                                |
| `.AtIndex(n)` / `.FromEnd()` | restrict the preceding `WithArgument` to a position (optionally counted from the end) |

The `"name"` overloads accept the [string matching options](#string-matching-options), and
`.AtIndex(n)` is chained after a `WithArgument` to pin it to a specific position.

```csharp
// Generic types with a single argument constrained to IEntity
In.AllLoadedAssemblies().Types()
    .WhichAreGeneric().WithArgumentCount(1).WithArgument<IEntity>()

// Generic methods whose first argument is named "TKey"
In.AllLoadedAssemblies().Methods()
    .WhichAreGeneric().WithArgument("TKey").AtIndex(0)
```

### Methods

In addition to [access modifiers](#access-modifiers),
[attributes](#attributes) and
[names](#names-and-namespaces):

|                                      | Filter                                              | Assert (single)                                                 | Assert (many)                |
|--------------------------------------|-----------------------------------------------------|-----------------------------------------------------------------|------------------------------|
| static / abstract / sealed / generic | `.WhichAreStatic()` …                               | `.IsStatic()` …                                                 | `.AreStatic()` …             |
| returns type (or a subtype)          | `.WhichReturn<T>()`                                 | `.Returns<T>()`                                                 | `.Return<T>()`               |
| returns exactly                      | `.WhichReturnExactly<T>()`                          | `.ReturnsExactly<T>()`                                          | `.ReturnExactly<T>()`        |
| no parameters                        | `.WithoutParameters()`                              | `.HasNoParameters()`                                            | `.HaveNoParameters()`        |
| parameter of type (or subtype)       | `.WithParameter<T>()` / `.WithParameter<T>("name")` | `.HasParameter<T>()` / `.HasParameter<T>("name")`               | `.HaveParameter<T>()`        |
| parameter of exact type              | `.WithParameterExactly<T>()`                        | `.HasParameterExactly<T>()` / `.HasParameterExactly<T>("name")` | `.HaveParameterExactly<T>()` |
| parameter count                      | `.WithParameterCount(n)`                            | `.HasParameterCount(n)`                                         | `.HaveParameterCount(n)`     |
| custom predicate                     | `.Which(m => …)`                                    | `.Satisfies(m => …)`                                            | `.All().Satisfy(m => …)`     |

`WhichReturn<Task>()` and `Returns<Task>()` also match `Task<T>`; the `…Exactly` variants match only the
exact type. Use `OrReturn<T>()` / `OrReturnExactly<T>()` to allow several return types.

```csharp
In.AllLoadedAssemblies().Methods()
    .WhichArePublic()
    .WhichReturn<Task>().OrReturn<ValueTask>()
    .WithParameter<CancellationToken>()
    .With<HttpGetAttribute>().OrWith<HttpPostAttribute>()

await Expect.That(method).HasParameter<int>("count");
await Expect.That(methods).Return<Task>().OrReturn<ValueTask>();
```

### Properties & Fields

In addition to [access modifiers](#access-modifiers),
[attributes](#attributes) and
[names](#names-and-namespaces):

|                                       | Filter                                      | Assert (single)                 | Assert (many)                     |
|---------------------------------------|---------------------------------------------|---------------------------------|-----------------------------------|
| of type (or a subtype)                | `.OfType<T>()`                              | `.IsOfType<T>()`                | `.AreOfType<T>()`                 |
| of exact type                         | `.OfExactType<T>()`                         | `.IsOfExactType<T>()`           | `.AreOfExactType<T>()`            |
| static *(properties & fields)*        | `.WhichAreStatic()`                         | `.IsStatic()`                   | `.AreStatic()`                    |
| abstract / sealed *(properties only)* | `.WhichAreAbstract()` / `.WhichAreSealed()` | `.IsAbstract()` / `.IsSealed()` | `.AreAbstract()` / `.AreSealed()` |
| readable *(properties only)*          | `.WhichAreReadable()`                       | `.IsReadable()`                 | `.AreReadable()`                  |
| writable *(properties only)*          | `.WhichAreWritable()`                       | `.IsWritable()`                 | `.AreWritable()`                  |
| read-only *(properties only)*         | `.WhichAreReadOnly()`                       | `.IsReadOnly()`                 | `.AreReadOnly()`                  |
| write-only *(properties only)*        | `.WhichAreWriteOnly()`                      | `.IsWriteOnly()`                | `.AreWriteOnly()`                 |
| read-write *(properties only)*        | `.WhichAreReadWrite()`                      | `.IsReadWrite()`                | `.AreReadWrite()`                 |

Use `OrOfType<T>()` / `OrOfExactType<T>()` to allow several types.

```csharp
In.AllLoadedAssemblies().Public.Properties()
    .OfType<string>()
    .WithName("Id").AsSuffix()
    .With<RequiredAttribute>()

In.AllLoadedAssemblies().Private.Fields()
    .OfType<ILogger>()
    .WithName("_").AsPrefix()
```

### Events

Events support [access modifiers](#access-modifiers),
[attributes](#attributes),
[names](#names-and-namespaces) and the `abstract` / `sealed` modifiers
(`.WhichAreAbstract()` / `.IsAbstract()` / `.AreAbstract()`, likewise for `sealed`).

```csharp
In.AllLoadedAssemblies().Public.Events()
    .WithName("Changed").AsSuffix()
    .With<ObsoleteAttribute>()
```

### Constructors

In addition to [access modifiers](#access-modifiers) and
[attributes](#attributes):

|                                | Filter                                              | Assert (single)                                                 | Assert (many)                |
|--------------------------------|-----------------------------------------------------|-----------------------------------------------------------------|------------------------------|
| static                         | `.WhichAreStatic()`                                 | `.IsStatic()`                                                   | `.AreStatic()`               |
| no parameters                  | `.WithoutParameters()`                              | `.HasNoParameters()`                                            | `.HaveNoParameters()`        |
| parameter of type (or subtype) | `.WithParameter<T>()` / `.WithParameter<T>("name")` | `.HasParameter<T>()` / `.HasParameter<T>("name")`               | `.HaveParameter<T>()`        |
| parameter of exact type        | `.WithParameterExactly<T>()`                        | `.HasParameterExactly<T>()` / `.HasParameterExactly<T>("name")` | `.HaveParameterExactly<T>()` |
| parameter count                | `.WithParameterCount(n)`                            | `.HasParameterCount(n)`                                         | `.HaveParameterCount(n)`     |

```csharp
In.AllLoadedAssemblies().Public.Constructors()
    .WithParameterCount(1)
    .WithParameter<string>()
    .With<JsonConstructorAttribute>()
```

### Assemblies

Assemblies are usually used as a [source](#sources-the-in-helper), but you can also filter and assert
on them directly:

|                             | Filter                                 | Assert (single)                  | Assert (many)                     |
|-----------------------------|----------------------------------------|----------------------------------|-----------------------------------|
| by name                     | `.WithName("x")`                       | `.HasName("x")`                  | `.HaveName("x")`                  |
| by target framework         | `.WhichTarget("net8.0")`               | `.Targets("net8.0")`             | `.Target("net8.0")`               |
| has attribute               | `.With<TAttribute>()`                  | `.Has<TAttribute>()`             | `.Have<TAttribute>()`             |
| depends on assembly         | `.WhichHaveADependencyOn("x")`         | `.HasADependencyOn("x")`         | `.HaveADependencyOn("x")`         |
| does not depend on assembly | `.WhichHaveNoDependencyOn("x")`        | `.HasNoDependencyOn("x")`        | `.HaveNoDependencyOn("x")`        |
| depends only on set         | `.WhichHaveDependenciesOnlyOn("x", …)` | `.HasDependenciesOnlyOn("x", …)` | `.HaveDependenciesOnlyOn("x", …)` |
| custom predicate            | `.Which(a => …)`                       | -                                | -                                 |

```csharp
Assembly subject = Assembly.GetEntryAssembly();
Assembly[] subjects = AppDomain.CurrentDomain.GetAssemblies();

await Expect.That(subject).HasName("aweXpect").AsPrefix();
await Expect.That(subject).HasADependencyOn("System.Core");
await Expect.That(subject).HasNoDependencyOn("UnwantedDependency");
await Expect.That(subject).HasDependenciesOnlyOn("aweXpect.Core", "aweXpect");
await Expect.That(subjects).Have<AssemblyTitleAttribute>();
await Expect.That(subject).Targets("net8.0");
```

The target framework is matched against the short moniker form (e.g. `net8.0`, `netstandard2.0`, `net48`),
derived from the assembly's `[TargetFramework]` attribute. Assemblies without that attribute are treated as
having no target framework and never match.

## Combining filters

Filters chain naturally (each narrows the previous result). Several filters offer an `Or…` companion to
widen a single step:

```csharp
// Any of several attributes
In.AllLoadedAssemblies().Methods()
    .With<FactAttribute>().OrWith<TheoryAttribute>()

// Any of several return types
In.AllLoadedAssemblies().Methods()
    .WhichReturn<Task>().OrReturn<ValueTask>()

// Any of several property/field types
In.AllLoadedAssemblies().Properties()
    .OfType<string>().OrOfType<Guid>()
```

## String matching options

Every name and namespace filter/assertion uses the same string matching options as the core aweXpect
library (see [the docs](https://docs.testably.org/aweXpect/common-types/string#equality)):

| Option                                                           | Effect                                            |
|------------------------------------------------------------------|---------------------------------------------------|
| *(none)*                                                         | exact match (default)                             |
| `.AsPrefix()`                                                    | the value must start with the expected string     |
| `.AsSuffix()`                                                    | the value must end with the expected string       |
| `.AsWildcard()`                                                  | match using `*` and `?` wildcards                 |
| `.AsRegex()`                                                     | match using a regular expression                  |
| `.IgnoringCase()`                                                | case-insensitive comparison                       |
| `.IgnoringLeadingWhiteSpace()` / `.IgnoringTrailingWhiteSpace()` | trim before comparing                             |
| `.Using(comparer)`                                               | compare with a custom `IEqualityComparer<string>` |

```csharp
await Expect.That(types).HaveName("Service").AsSuffix();
await Expect.That(types).HaveName("*Test*").AsWildcard();
await Expect.That(types).HaveName(@"^Test\w+$").AsRegex();
await Expect.That(methods).HaveName("Get*Async").AsWildcard().IgnoringCase();
```

## Collections and quantifiers

Every expectation works with both a single item and a collection. A collection can be an array,
any `IEnumerable<T?>` or — on .NET 8 and later — an `IAsyncEnumerable<T?>`. The plural assertions already
require **every** item to match; for ad-hoc predicates use aweXpect's `All()` / `Any()` quantifiers with
`Satisfy(…)`, and combine selections with LINQ:

```csharp
// The plural assertion already means "every item":
await Expect.That(types).ArePublic();

// Ad-hoc predicate across the whole collection:
await Expect.That(types).All().Satisfy(type => type.IsSealed);
await Expect.That(types).Any().Satisfy(type => type.IsAbstract);

// Mix with LINQ (assign to IEnumerable<Type?> so Where binds to LINQ):
IEnumerable<Type?> publicClasses = In.AllLoadedAssemblies().Types()
    .WhichAreClasses().WhichArePublic();
var managers = publicClasses.Where(type => type!.GetInterfaces().Length > 2);
await Expect.That(managers).HaveName("Manager").AsSuffix();
```

## Configuration

### Assembly exclusions

By default, assemblies whose name starts with one of the following prefixes are excluded from
`In.AllLoadedAssemblies()`:

`mscorlib`, `System`, `Microsoft`, `JetBrains`, `xunit`, `Castle`, `DynamicProxyGenAssembly2`.

Customize this via `Customize.aweXpect.Reflection().ExcludedAssemblyPrefixes`. `Set(…)` replaces the
list and returns a scope that restores the previous value when disposed:

```csharp
using (Customize.aweXpect.Reflection().ExcludedAssemblyPrefixes
    .Set(new[] { "mscorlib", "System", "Microsoft", "MyCompany.Generated" }))
{
    // In.AllLoadedAssemblies() applies the custom prefixes within this scope
}
```

### Compiler-generated members

By default, compiler-generated types and members are **excluded** from every navigation
(`.Types()`, `.Methods()`, `.Fields()`, …). This hides closures, async/iterator state machines,
anonymous types, local functions, auto-property backing fields and the generated members of records
(`ToString`, `Equals`, `<Clone>$`, the copy-constructor, …), so convention tests see only the members
you actually wrote.

Opt specific kinds back in with the `[Flags]` enum `CompilerGeneratedMembers`
(`None`, `Types`, `Constructors`, `Methods`, `Properties`, `Fields`, `Events`, `All`):

```csharp
using (Customize.aweXpect.Reflection().IncludedCompilerGeneratedMembers()
    .Set(CompilerGeneratedMembers.Types | CompilerGeneratedMembers.Methods))
{
    // closures, state machines and compiler-generated methods are now visible
}
```

Operators (`op_*`) and property/event accessors (`get_`, `set_`, `add_`, `remove_`) are *user-written*
but likewise excluded by default. Include them via the separate `SpecialNameMembers` enum
(`None`, `Operators`, `Accessors`, `All`), which only affects `.Methods()`:

```csharp
using (Customize.aweXpect.Reflection().IncludedSpecialNameMembers()
    .Set(SpecialNameMembers.Operators))
{
    // operator methods are now visible in .Methods()
}
```
