# aweXpect.Reflection

[![Nuget](https://img.shields.io/nuget/v/aweXpect.Reflection)](https://www.nuget.org/packages/aweXpect.Reflection)
[![Build](https://github.com/Testably/aweXpect.Reflection/actions/workflows/build.yml/badge.svg)](https://github.com/Testably/aweXpect.Reflection/actions/workflows/build.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Testably_aweXpect.Reflection&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Testably_aweXpect.Reflection)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Testably_aweXpect.Reflection&metric=coverage)](https://sonarcloud.io/summary/overall?id=Testably_aweXpect.Reflection)
[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2FTestably%2FaweXpect.Reflection%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/Testably/aweXpect.Reflection/main)

Expectations for reflection types for [aweXpect](https://github.com/Testably/aweXpect).

Write architecture and convention tests as plain, readable assertions: **select** the assemblies, types
or members you care about with `In` or `Types`, then **assert** a rule on them with `Expect.That`.

## At a glance

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
// Verify all test classes follow the naming convention
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

| Source                                                                                        | Returns                                                                              |
|-----------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------------|
| `In.AllLoadedAssemblies()`                                                                    | all currently loaded assemblies (system assemblies [excluded](#assembly-exclusions)) |
| `In.Assemblies(a1, a2, …)` / `In.Assemblies(collection)`                                      | the given assemblies                                                                 |
| `In.AssemblyContaining<T>()` / `In.AssemblyContaining(typeof(T))`                             | the assembly that declares `T`                                                       |
| `In.Type<T>()` / `In.Type(typeof(T))`                                                         | a single type                                                                        |
| `In.Types<T1, T2>()` / `In.Types<T1, T2, T3>()` / `In.Types(t1, t2, …)`                       | the given types                                                                      |
| `In.Constructors(…)` / `In.Events(…)` / `In.Fields(…)` / `In.Methods(…)` / `In.Properties(…)` | the given members                                                                    |

## Sources: the `Types` helper

While `In` starts from concrete reflection objects, `Types` selects types *by criteria* — the natural entry
point for architecture rules:

| Source                                                          | Returns                                                                         |
|-----------------------------------------------------------------|---------------------------------------------------------------------------------|
| `Types.InNamespace("ns")`                                       | all types within a namespace and its sub-namespaces (across loaded assemblies) |
| `Types.InAllLoadedAssemblies()`                                 | all types in all currently loaded assemblies                                   |
| `Types.InAssemblies(a1, a2, …)`                                 | all types in the given assemblies                                              |
| `Types.InAssemblyContaining<T>()` / `…(typeof(T))`              | all types in the assembly that declares `T`                                    |

`Types.InNamespace(…)` searches all loaded assemblies by default; chain one of the same `In*` methods directly
after it to clarify the assembly source (it can only be specified once, before any further filters):

```csharp
// Defaults to all loaded assemblies
await Expect.That(Types.InNamespace("MyApp.Domain")).AreSealed();

// Clarify the assembly source
await Expect.That(Types.InNamespace("MyApp.Domain").InAssemblyContaining<MyApp.Startup>())
    .AreSealed();
```

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

The custom `.Which(…)` filter has a universal assertion counterpart that works for **every** subject kind
(types, members and assemblies): use aweXpect's `.Satisfies(…)` on a single subject and `.All().Satisfy(…)`
(or `.Any().Satisfy(…)`) on a collection. See [Collections and quantifiers](#collections-and-quantifiers).

The custom `.Except(…)` filter is the inverse of `.Which(…)`: it **removes** the items that match the
predicate. This is handy for defining exemptions to a rule (e.g. *all async methods except this one*). Like
`.Which(…)` it is available on **every** collection (assemblies, types and members). For types there is also a
typed `.Except<T>()` overload that excludes exactly the type `T`.

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

|                                    | Filter                      | Assert (single)              | Assert (many)               |
|------------------------------------|-----------------------------|------------------------------|-----------------------------|
| has attribute                      | `.With<TAttribute>()`       | `.Has<TAttribute>()`         | `.Have<TAttribute>()`       |
| has attribute matching a predicate | `.With<TAttribute>(a => …)` | `.Has<TAttribute>(a => …)`   | `.Have<TAttribute>(a => …)` |
| any of several attributes          | `.With<T1>().OrWith<T2>()`  | -                            | -                           |
| does not have attribute            | `.Without<TAttribute>()`    | `.DoesNotHave<TAttribute>()` | `.DoNotHave<TAttribute>()`  |

Most attribute filters and assertions (`With`, `OrWith`, `Without`, `Has`, `Have`, `DoesNotHave`, `DoNotHave`,
`OrHas`, `OrHave`) take an optional `inherit` parameter (default `true`) that controls whether attributes
inherited from base types are considered: `.With<TAttribute>(inherit: false)`. Fields and constructors cannot
inherit attributes, so their attribute filters and assertions omit the parameter. Chain multiple
`.Without<TAttribute>()` calls to exclude several attributes (an item must have none of them).

```csharp
await Expect.That(type).Has<ObsoleteAttribute>(a => a.Message == "Use NewClass instead");
await Expect.That(methods).Have<FactAttribute>();
```

### Obsolete

Shared by all types and members: a self-documenting shorthand for the `ObsoleteAttribute` (so that
`.WhichAreObsolete()` reads better than `.With<ObsoleteAttribute>()` in architecture rules).

|              | Filter                   | Assert (single)    | Assert (many)       |
|--------------|--------------------------|--------------------|---------------------|
| obsolete     | `.WhichAreObsolete()`    | `.IsObsolete()`    | `.AreObsolete()`    |
| not obsolete | `.WhichAreNotObsolete()` | `.IsNotObsolete()` | `.AreNotObsolete()` |

```csharp
// Verify that nothing public is marked [Obsolete]
await Expect.That(In.AssemblyContaining<MyClass>().Types().WhichArePublic())
    .AreNotObsolete();
```

### Names and namespaces

Shared by all types and members.

|                                 | Filter                  | Assert (single)           | Assert (many)              |
|---------------------------------|-------------------------|---------------------------|----------------------------|
| by name                         | `.WithName("x")`        | `.HasName("x")`           | `.HaveName("x")`           |
| not by name                     | `.WithoutName("x")`     | `.DoesNotHaveName("x")`   | `.DoNotHaveName("x")`      |
| by namespace *(types only)*     | `.WithNamespace("x")`   | `.HasNamespace("x")`      | `.HaveNamespace("x")`      |
| within namespace *(types only)* | `.WithinNamespace("x")` | `.IsWithinNamespace("x")` | `.AreWithinNamespace("x")` |

`WithoutName`/`DoesNotHaveName`/`DoNotHaveName` are the negations of the name filter and assertions:
`WithoutName` keeps the items whose name does *not* match, `DoesNotHaveName` verifies a single item is
*not* named the given value, and `DoNotHaveName` verifies that *none* of the items in a collection are.
They accept the same [string matching options](#string-matching-options) as their positive counterparts:

```csharp
// Verify that no type in the production assembly is named with a "Test" suffix
await Expect.That(In.AssemblyContaining<MyClass>().Types())
    .DoNotHaveName("Test").AsSuffix();
```

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
form (`NotWithinNamespace`, `IsNotWithinNamespace` and `AreNotWithinNamespace`) that matches types
outside the namespace.

### Types

| Kind                | Filter                                          | Assert (single)             | Assert (many)                |
|---------------------|-------------------------------------------------|-----------------------------|------------------------------|
| class               | `.WhichAreClasses()` / `.Classes()`             | `.IsAClass()`               | `.AreClasses()`              |
| interface           | `.WhichAreInterfaces()` / `.Interfaces()`       | `.IsAnInterface()`          | `.AreInterfaces()`           |
| enum                | `.WhichAreEnums()` / `.Enums()`                 | `.IsAnEnum()`               | `.AreEnums()`                |
| struct              | `.WhichAreStructs()` / `.Structs()`             | `.IsAStruct()`              | `.AreStructs()`              |
| record              | `.WhichAreRecords()` / `.Records()`             | `.IsARecord()`              | `.AreRecords()`              |
| record struct       | `.WhichAreRecordStructs()` / `.RecordStructs()` | `.IsARecordStruct()`        | `.AreRecordStructs()`        |
| readonly struct     | `.WhichAreReadOnly()`                           | `.IsReadOnly()`             | `.AreReadOnly()`             |
| ref struct          | `.WhichAreRefStructs()`                         | `.IsARefStruct()`           | `.AreRefStructs()`           |
| delegate            | `.WhichAreDelegates()`                          | `.IsADelegate()`            | `.AreDelegates()`            |
| exception           | `.WhichAreExceptions()`                         | `.IsAnException()`          | `.AreExceptions()`           |
| attribute           | `.WhichAreAttributes()`                         | `.IsAnAttribute()`          | `.AreAttributes()`           |
| abstract            | `.WhichAreAbstract()` / `.Abstract`             | `.IsAbstract()`             | `.AreAbstract()`             |
| sealed              | `.WhichAreSealed()` / `.Sealed`                 | `.IsSealed()`               | `.AreSealed()`               |
| static              | `.WhichAreStatic()` / `.Static`                 | `.IsStatic()`               | `.AreStatic()`               |
| generic             | `.WhichAreGeneric()` / `.Generic`               | `.IsGeneric()`              | `.AreGeneric()`              |
| nested              | `.WhichAreNested()` / `.Nested`                 | `.IsNested()`               | `.AreNested()`               |
| inherits from       | `.WhichInheritFrom<T>()`                        | `.InheritsFrom<T>()`        | `.InheritFrom<T>()`          |
| implements          | `.WhichImplement<T>()`                          | `.Implements<T>()`          | `.Implement<T>()`            |
| assignable to       | `.WhichAreAssignableTo<T>()`                    | `.IsAssignableTo<T>()`      | `.AreAssignableTo<T>()`      |
| assignable from     | `.WhichAreAssignableFrom<T>()`                  | `.IsAssignableFrom<T>()`    | `.AreAssignableFrom<T>()`    |
| instantiable        | `.WhichAreInstantiable()`                       | `.IsInstantiable()`         | `.AreInstantiable()`         |
| default constructor | `.WhichHaveADefaultConstructor()`               | `.HasADefaultConstructor()` | `.HaveADefaultConstructor()` |
| custom predicate    | `.Which(t => …)`                                | `.Satisfies(t => …)`        | `.All().Satisfy(t => …)`     |

`WhichInheritFrom` / `InheritsFrom` consider only the **base-class chain** (not implemented interfaces) and
accept a generic argument or a `Type`, plus an optional `forceDirect` flag to require *direct* inheritance.
Passing an interface throws; use `Implements` for that.

`WhichImplement` / `Implements` consider only implemented **interfaces** (also with an optional `forceDirect`
flag); passing a non-interface throws. With `forceDirect`, an interface reached only through a base class or
through another implemented interface does not count as *directly* implemented.

`IsAssignableTo` / `IsAssignableFrom` (and their `WhichAre…` / `Are…` forms) use runtime assignability, which
covers base classes *and* interfaces in one step, treats a type as assignable to itself, and honors closed
generic variance. Open generic type definitions (e.g. `typeof(IFoo<>)`) are not supported and throw.

```csharp
In.AllLoadedAssemblies().Types()
    .WhichAreClasses().WhichArePublic()
    .WithName("Service").AsSuffix()
    .WhichInheritFrom<BaseService>()

// Shorthand for the same access/kind filters
In.AllLoadedAssemblies().Public.Abstract.Classes()
```

A type is *instantiable* when it is a concrete type that is neither abstract, static nor an interface, and not
an open generic type definition. *Default constructor* checks for an accessible parameterless constructor
(value types always have one); this is independent of instantiability (e.g. a type with only a parameterized
constructor is instantiable but has no default constructor).

> **Negation:** every kind/modifier row above has a negated form. Most use `WhichAreNot…` on filters and
> `IsNot…` / `AreNot…` on assertions (e.g. `WhichAreNotSealed()`, `IsNotAClass()`, `AreNotStatic()`,
> `IsNotInstantiable()`). The *default constructor* row uses `WhichDoNotHaveADefaultConstructor()`,
> `DoesNotHaveADefaultConstructor()` and `DoNotHaveADefaultConstructor()`.

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
| async                                | `.WhichAreAsync()`                                  | `.IsAsync()`                                                    | `.AreAsync()`                |
| extension method                     | `.WhichAreExtensionMethods()`                       | `.IsAnExtensionMethod()`                                        | `.AreExtensionMethods()`     |
| operator                             | `.WhichAreOperators()`                              | `.IsAnOperator()`                                               | `.AreOperators()`            |
| virtual                              | `.WhichAreVirtual()`                                | `.IsVirtual()`                                                  | `.AreVirtual()`              |
| overrides a base method              | `.WhichOverride()`                                  | `.Overrides()`                                                  | `.Override()`                |
| returns type (or a subtype)          | `.WhichReturn<T>()`                                 | `.Returns<T>()`                                                 | `.Return<T>()`               |
| returns exactly                      | `.WhichReturnExactly<T>()`                          | `.ReturnsExactly<T>()`                                          | `.ReturnExactly<T>()`        |
| returns void                         | `.WhichReturnVoid()`                                | `.ReturnsVoid()`                                                | `.ReturnVoid()`              |
| no parameters                        | `.WithoutParameters()`                              | `.HasNoParameters()`                                            | `.HaveNoParameters()`        |
| parameter of type (or subtype)       | `.WithParameter<T>()` / `.WithParameter<T>("name")` | `.HasParameter<T>()` / `.HasParameter<T>("name")`               | `.HaveParameter<T>()`        |
| parameter of exact type              | `.WithParameterExactly<T>()`                        | `.HasParameterExactly<T>()` / `.HasParameterExactly<T>("name")` | `.HaveParameterExactly<T>()` |
| parameter count                      | `.WithParameterCount(n)`                            | `.HasParameterCount(n)`                                         | `.HaveParameterCount(n)`     |
| `ref` parameter                      | `.WithRefParameter()`                               | `.HasRefParameter()`                                            | `.HaveRefParameter()`        |
| `out` parameter                      | `.WithOutParameter()`                               | `.HasOutParameter()`                                            | `.HaveOutParameter()`        |
| `in` parameter                       | `.WithInParameter()`                                | `.HasInParameter()`                                             | `.HaveInParameter()`         |
| `params` parameter                   | `.WithParamsParameter()`                            | `.HasParamsParameter()`                                         | `.HaveParamsParameter()`     |
| optional parameter                   | `.WithOptionalParameter()`                          | `.HasOptionalParameter()`                                       | `.HaveOptionalParameter()`   |
| custom predicate                     | `.Which(m => …)`                                    | `.Satisfies(m => …)`                                            | `.All().Satisfy(m => …)`     |

`WhichReturn<Task>()` and `Returns<Task>()` also match `Task<T>`; the `…Exactly` variants match only the
exact type. Use `OrReturn(s)<T>()` / `OrReturn(s)Exactly<T>()` to allow several return types (the single-subject
assertion is `OrReturns…`, the filter and collection assertion are `OrReturn…`). Since `void` cannot be
used as a generic type argument, use `WhichReturnVoid()` / `ReturnsVoid()` / `ReturnVoid()` to match
void-returning methods.

The `ref` / `out` / `in` / `params` / optional parameter filters and assertions mirror `WithParameter`:
each also has `<T>()`, `(Type)`, `<T>("name")` and `…Exactly<T>()` overloads to constrain the parameter's type
and name (e.g. `.WithRefParameter<int>("count")`, `.HasOutParameterExactly<string>()`).

`WhichAreExtensionMethods()`, `IsAnExtensionMethod()` and `AreExtensionMethods()` match both classic `this`-parameter
extension methods and extension methods declared with the C# extension block syntax (`extension(...) { … }`), including
static extension methods. The compiler-generated grouping types backing the extension block syntax are excluded from the
reflected members.

```csharp
In.AllLoadedAssemblies().Methods()
    .WhichArePublic()
    .WhichReturn<Task>().OrReturn<ValueTask>()
    .WithParameter<CancellationToken>()
    .With<HttpGetAttribute>().OrWith<HttpPostAttribute>()

await Expect.That(method).HasParameter<int>("count");
await Expect.That(methods).Return<Task>().OrReturn<ValueTask>();
```

### Operators

The `Operator` enum maps each C# operator to its compiler-emitted `op_*` metadata name (e.g.
`Operator.Addition` ↔ `op_Addition`), so operator assertions are type-safe and discoverable instead of relying
on magic strings. It covers unary, binary, comparison and conversion operators, including the C# 11 `checked`
variants and `>>>` (`UnsignedRightShift`).

| Scope               | Filter                         | Assert (single)                                      | Assert (many)                                         |
|---------------------|--------------------------------|------------------------------------------------------|-------------------------------------------------------|
| any operator method | `.WhichAreOperators()`         | `.IsAnOperator()`                                    | `.AreOperators()`                                     |
| specific operator   | `.WhichAreOperators(Operator)` | `.IsAnOperator(Operator)`                            | n/a                                                   |
| type has operator   | n/a                            | `.HasOperator(Operator)`                             | `.HaveOperator(Operator)`                             |
| implicit conversion | n/a                            | `.HasImplicitConversionOperator<TSource, TTarget>()` | `.HaveImplicitConversionOperator<TSource, TTarget>()` |
| explicit conversion | n/a                            | `.HasExplicitConversionOperator<TSource, TTarget>()` | `.HaveExplicitConversionOperator<TSource, TTarget>()` |

```csharp
// Method-level: narrow an operator method to a specific operator
await Expect.That(methodInfo).IsAnOperator(Operator.Equality);

// Type-level presence (op_* lookup); inherit: true also considers base-type operators
await Expect.That(typeof(MyMoney)).HasOperator(Operator.Addition);
await Expect.That(typeof(MyMoney)).DoesNotHaveOperator(Operator.Modulus);

// Disambiguate overloads by operand type
await Expect.That(typeof(MyMoney)).HasOperator<int>(Operator.Addition);

// Conversion operators are keyed by their source → target signature
await Expect.That(typeof(MyMoney)).HasImplicitConversionOperator<MyMoney, decimal>();
await Expect.That(typeof(MyMoney)).HasExplicitConversionOperator(typeof(MyMoney), typeof(int));
```

`HasOperator` / conversion operators match operators declared on the type itself; pass `inherit: true` to also
consider operators inherited from base types. Conversion source/target types are matched exactly.

Operators are special-name members that are [hidden by default](#compiler-generated-members), so the
plain `.Methods()` collection excludes them unless opted in via `IncludedSpecialNameMembers`. The
`.WhichAreOperators(Operator)` filter implicitly re-includes operators for its query, so it works without
that configuration. The negative `.WhichAreNotOperators(Operator)` filter deliberately does **not** re-include
operators: a "not this operator" filter over `.Methods()` is meant to narrow regular methods, and force-including
every *other* operator would surprise more than help. If you want the other operators in that result, opt in via
`IncludedSpecialNameMembers`.

> **Negation:** `IsNotAnOperator(Operator)`, `DoesNotHaveOperator(Operator)` / `DoNotHaveOperator(Operator)`
> (including the operand overloads, e.g. `DoesNotHaveOperator<int>(Operator)`),
> `DoesNotHave…ConversionOperator…` / `DoNotHave…ConversionOperator…` and `WhichAreNotOperators(Operator)`.

### Properties & Fields

In addition to [access modifiers](#access-modifiers),
[attributes](#attributes) and
[names](#names-and-namespaces):

|                                        | Filter                                      | Assert (single)                 | Assert (many)                     |
|----------------------------------------|---------------------------------------------|---------------------------------|-----------------------------------|
| of type (or a subtype)                 | `.OfType<T>()`                              | `.IsOfType<T>()`                | `.AreOfType<T>()`                 |
| of exact type                          | `.OfExactType<T>()`                         | `.IsOfExactType<T>()`           | `.AreOfExactType<T>()`            |
| static *(properties & fields)*         | `.WhichAreStatic()`                         | `.IsStatic()`                   | `.AreStatic()`                    |
| abstract / sealed *(properties only)*  | `.WhichAreAbstract()` / `.WhichAreSealed()` | `.IsAbstract()` / `.IsSealed()` | `.AreAbstract()` / `.AreSealed()` |
| virtual *(properties only)*            | `.WhichAreVirtual()`                        | `.IsVirtual()`                  | `.AreVirtual()`                   |
| override *(properties only)*           | `.WhichOverride()`                          | `.Overrides()`                  | `.Override()`                     |
| required *(properties only)*           | `.WhichAreRequired()`                       | `.IsRequired()`                 | `.AreRequired()`                  |
| readable *(properties only)*           | `.WhichAreReadable()`                       | `.IsReadable()`                 | `.AreReadable()`                  |
| writable *(properties only)*           | `.WhichAreWritable()`                       | `.IsWritable()`                 | `.AreWritable()`                  |
| read-only *(properties only)*          | `.WhichAreReadOnly()`                       | `.IsReadOnly()`                 | `.AreReadOnly()`                  |
| write-only *(properties only)*         | `.WhichAreWriteOnly()`                      | `.IsWriteOnly()`                | `.AreWriteOnly()`                 |
| read-write *(properties only)*         | `.WhichAreReadWrite()`                      | `.IsReadWrite()`                | `.AreReadWrite()`                 |
| has getter *(properties only)*         | `.WhichHaveAGetter()`                       | `.HasAGetter()`                 | `.HaveAGetter()`                  |
| has setter *(properties only)*         | `.WhichHaveASetter()`                       | `.HasASetter()`                 | `.HaveASetter()`                  |
| has init setter *(properties only)*    | `.WhichHaveAnInitSetter()`                  | `.HasAnInitSetter()`            | `.HaveAnInitSetter()`             |
| indexer *(properties only)*            | `.WhichAreIndexers()`                       | `.IsAnIndexer()`                | `.AreIndexers()`                  |
| extension property *(properties only)* | `.WhichAreExtensionProperties()`            | `.IsAnExtensionProperty()`      | `.AreExtensionProperties()`       |
| read-only *(fields only)*              | `.WhichAreReadOnly()`                       | `.IsReadOnly()`                 | `.AreReadOnly()`                  |
| constant *(fields only)*               | `.WhichAreConstant()`                       | `.IsConstant()`                 | `.AreConstant()`                  |

> **Negation:** the `static`, `abstract`, `sealed`, `virtual`, `required`, `indexer`, `extension property`,
> `read-only` *(fields)* and `constant` rows have a negated form: `WhichAreNot…` on filters and `IsNot…` /
> `AreNot…` on assertions (e.g. `WhichAreNotConstant()`, `IsNotConstant()`, `AreNotConstant()`); `override` uses
> `WhichDoNotOverride()` / `DoesNotOverride()` / `DoNotOverride()`.

`WhichAreExtensionProperties()`, `IsAnExtensionProperty()` and `AreExtensionProperties()` match extension properties
declared with the C# extension block syntax (`extension(...) { … }`), both instance and static. The real properties
live on the compiler-generated grouping types backing the extension block, so they are surfaced from there, while the
public accessor methods they emit are excluded from the reflected methods.

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

In addition to [access modifiers](#access-modifiers),
[attributes](#attributes) and
[names](#names-and-namespaces):

|                                | Filter                                      | Assert (single)                 | Assert (many)                     |
|--------------------------------|---------------------------------------------|---------------------------------|-----------------------------------|
| handler of type (or a subtype) | `.OfType<T>()`                              | `.IsOfType<T>()`                | `.AreOfType<T>()`                 |
| handler of exact type          | `.OfExactType<T>()`                         | `.IsOfExactType<T>()`           | `.AreOfExactType<T>()`            |
| abstract / sealed              | `.WhichAreAbstract()` / `.WhichAreSealed()` | `.IsAbstract()` / `.IsSealed()` | `.AreAbstract()` / `.AreSealed()` |
| static                         | `.WhichAreStatic()`                         | `.IsStatic()`                   | `.AreStatic()`                    |

The `OfType` / `IsOfType` / `AreOfType` filters and assertions match the event's handler type (its
`EventHandlerType`, e.g. `EventHandler<T>`); the `…ExactType` variants match only the exact handler type.
Use `OrOfType<T>()` / `OrOfExactType<T>()` to allow several handler types.

> **Negation:** the `abstract`, `sealed` and `static` rows have a negated form: `WhichAreNot…` on filters and
> `IsNot…` / `AreNot…` on assertions (e.g. `WhichAreNotSealed()`, `IsNotSealed()`, `AreNotSealed()`).

```csharp
// Every event must use the generic EventHandler<T> pattern
await Expect.That(In.AllLoadedAssemblies().Public.Events())
    .AreOfType(typeof(EventHandler<>));

In.AllLoadedAssemblies().Public.Events()
    .OfType<EventHandler>()
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
| `ref` parameter                | `.WithRefParameter()`                               | `.HasRefParameter()`                                            | `.HaveRefParameter()`        |
| `out` parameter                | `.WithOutParameter()`                               | `.HasOutParameter()`                                            | `.HaveOutParameter()`        |
| `in` parameter                 | `.WithInParameter()`                                | `.HasInParameter()`                                             | `.HaveInParameter()`         |
| `params` parameter             | `.WithParamsParameter()`                            | `.HasParamsParameter()`                                         | `.HaveParamsParameter()`     |
| optional parameter             | `.WithOptionalParameter()`                          | `.HasOptionalParameter()`                                       | `.HaveOptionalParameter()`   |

The `ref` / `out` / `in` / `params` / optional parameter filters and assertions accept the same
`<T>()`, `(Type)`, `<T>("name")` and `…Exactly<T>()` overloads as `WithParameter` (see [Methods](#methods)).

```csharp
In.AllLoadedAssemblies().Public.Constructors()
    .WithParameterCount(1)
    .WithParameter<string>()
    .With<JsonConstructorAttribute>()
```

### Assemblies

Assemblies are usually used as a [source](#sources-the-in-helper), but you can also filter and assert
on them directly:

|                             | Filter                       | Assert (single)              | Assert (many)              |
|-----------------------------|------------------------------|------------------------------|----------------------------|
| by name                     | `.WithName("x")`             | `.HasName("x")`              | `.HaveName("x")`           |
| not by name                 | `.WithoutName("x")`          | `.DoesNotHaveName("x")`      | `.DoNotHaveName("x")`      |
| by target framework         | `.WhichTarget("net8.0")`     | `.Targets("net8.0")`         | `.Target("net8.0")`        |
| by version                  | `.WithVersion(…)`            | `.HasVersion(…)`             | `.HaveVersion(…)`          |
| strong named                | `.WhichAreStrongNamed()`     | `.IsStrongNamed()`           | `.AreStrongNamed()`        |
| not strong named            | `.WhichAreNotStrongNamed()`  | `.IsNotStrongNamed()`        | `.AreNotStrongNamed()`     |
| has attribute               | `.With<TAttribute>()`        | `.Has<TAttribute>()`         | `.Have<TAttribute>()`      |
| does not have attribute     | `.Without<TAttribute>()`     | `.DoesNotHave<TAttribute>()` | `.DoNotHave<TAttribute>()` |
| depends on assembly         | `.WhichDependOn("x")`        | `.DependsOn("x")`            | `.DependOn("x")`           |
| does not depend on assembly | `.WhichDoNotDependOn("x")`   | `.DoesNotDependOn("x")`      | `.DoNotDependOn("x")`      |
| depends only on set         | `.WhichDependOnlyOn("x", …)` | `.DependsOnlyOn("x", …)`     | `.DependOnlyOn("x", …)`    |
| custom predicate            | `.Which(a => …)`             | `.Satisfies(a => …)`         | `.All().Satisfy(a => …)`   |

```csharp
Assembly subject = Assembly.GetEntryAssembly();
Assembly[] subjects = AppDomain.CurrentDomain.GetAssemblies();

await Expect.That(subject).HasName("aweXpect").AsPrefix();
await Expect.That(subject).DependsOn("System.Core");
await Expect.That(subject).DoesNotDependOn("UnwantedDependency");
await Expect.That(subject).DependsOnlyOn("aweXpect.Core", "aweXpect");
await Expect.That(subjects).Have<AssemblyTitleAttribute>();
await Expect.That(subject).Targets("net8.0");
```

The target framework is matched against the short moniker form (e.g. `net8.0`, `netstandard2.0`, `net48`),
derived from the assembly's `[TargetFramework]` attribute. Assemblies without that attribute are treated as
having no target framework and never match.

An assembly is considered strong named when its name carries a non-empty public key token.

The version filter and assertions come in two forms. Pass a `Func<Version, bool>` predicate to match the whole
version, or omit it to compare individual components (`WithMajor`, `WithMinor`, `WithBuild`, `WithRevision`)
with `GreaterThan`, `GreaterThanOrEqualTo`, `LessThan`, `LessThanOrEqualTo`, `EqualTo` and `NotEqualTo`.
Component comparisons chain (all must hold), and an assembly without a version never matches. The `Build` and
`Revision` components are `-1` when absent from the version.

```csharp
// Predicate form
In.AllLoadedAssemblies().WithVersion(version => version.Major >= 2)
await Expect.That(subject).HasVersion(version => version.Major >= 2);

// Component form
In.AllLoadedAssemblies().WithVersion().WithMajor.GreaterThanOrEqualTo(2).WithMinor.EqualTo(0)
await Expect.That(subject).HasVersion().WithMajor.GreaterThanOrEqualTo(2);
await Expect.That(subjects).HaveVersion().WithMajor.EqualTo(1);
```

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
any `IEnumerable<T?>` or, on .NET 8 and later, an `IAsyncEnumerable<T?>`. The plural assertions already
require **every** item to match; for ad-hoc predicates use aweXpect's `Satisfies(…)` (single subject) and
`All()` / `Any()` quantifiers with `Satisfy(…)` (collections), and combine selections with LINQ:

```csharp
// The plural assertion already means "every item":
await Expect.That(types).ArePublic();

// Ad-hoc predicate on a single subject:
await Expect.That(type).Satisfies(type => type.IsSealed);

// Ad-hoc predicate across the whole collection:
await Expect.That(types).All().Satisfy(type => type.IsSealed);
await Expect.That(types).Any().Satisfy(type => type.IsAbstract);

// Mix with LINQ (assign to IEnumerable<Type?> so Where binds to LINQ):
IEnumerable<Type?> publicClasses = In.AllLoadedAssemblies().Types()
    .WhichAreClasses().WhichArePublic();
var managers = publicClasses.Where(type => type!.GetInterfaces().Length > 2);
await Expect.That(managers).HaveName("Manager").AsSuffix();
```

## Architecture rules

Layering and architecture rules are expressed over the types a type references **in its signature**:
[Type dependencies](#type-dependencies) covers the dependency filters and assertions (including
[dependency cycles](#dependency-cycles)), and [Layers as type selections](#layers-as-type-selections) shows
how to combine them with reusable type selections into a full architecture test suite.

### Type dependencies

The dependency filters and assertions follow the familiar filter/assert pairing:

|                          | Filter                       | Assert (single)         | Assert (many)          |
|--------------------------|------------------------------|-------------------------|------------------------|
| depends on namespace     | `.WhichDependOn("x", …)`     | `.DependsOn("x", …)`    | `.DependOn("x", …)`    |
| does not depend on       | `.WhichDoNotDependOn("x", …)`| `.DoesNotDependOn("x", …)` | `.DoNotDependOn("x", …)` |
| depends only on set      | `.WhichDependOnlyOn("x", …)` | `.DependsOnlyOn("x", …)`| `.DependOnlyOn("x", …)`|

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

> **Signature-level only:** dependencies are computed from reflection metadata, so body-level references such
> as `new Infra.Foo()`, static calls and local variables are **not** detected. Function-pointer signatures
> (`delegate*<…>`) are not decomposed either; the types inside them are invisible to dependency assertions.
> Nested types are separate types with their own dependency surface: asserting on `typeof(Outer)` does not
> include what `Outer.Inner` references. The collection-based assertions (e.g. over `Types.InNamespace(…)`)
> enumerate nested types as their own items and therefore cover them. For IL/body-level accuracy, plug in
> your own resolver via `Customize.aweXpect.Reflection().DependencyResolver()` (see
> [Configuration](#dependency-resolver)).

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
`.ExcludingOwnSubNamespaces()` (only available on the *only-on* family) to also forbid references into a
type's own sub-namespaces:

```csharp
await Expect.That(Types.InNamespace("MyApp.Domain"))
    .DependOnlyOn("MyApp.Domain").ExcludingSubNamespaces().ExcludingOwnSubNamespaces();
```

`DependsOn` and `DoesNotDependOn` (single types only) also accept a **specific type** via `<T>()` or
`(Type)`, with `.OrOn<T>()` / `.OrOn(Type)` to widen:

```csharp
await Expect.That(typeof(MyDomainType)).DoesNotDependOn<DbContext>().OrOn<SqlConnection>();
```

All three dependency families additionally accept a reusable `Filtered.Types` selection as target; see
[Layers as type selections](#layers-as-type-selections).

> **Framework dependencies are ignored unless you name one explicitly.** `DependOnlyOn` ignores dependencies
> whose assembly name matches one of the
> [`ExcludedAssemblyPrefixes`](#assembly-exclusions) at a name-segment boundary: `System` covers `System`
> and `System.Text.Json`, but not an assembly named `SystemsBiology` (so you never have to whitelist
> `System.*` and unrelated assemblies are never swallowed by a prefix), while a
> type's **own namespace** is always allowed. `DependsOn` / `DoesNotDependOn` / `WhichDependOn` still match a
> framework namespace when you name it explicitly (e.g. `DoesNotDependOn("System.Data")`).
>
> ⚠️ The default prefixes include `Microsoft`, so `DependOnlyOn` also ignores dependencies on e.g.
> `Microsoft.EntityFrameworkCore`, `Microsoft.AspNetCore` and `Microsoft.Extensions.*`; a domain entity
> inheriting `DbContext` does **not** fail `DependOnlyOn("MyApp.Domain")`. To forbid such dependencies, name
> them explicitly (`DoesNotDependOn<DbContext>()` or `DoNotDependOn("Microsoft.EntityFrameworkCore")`) or
> customize the [`ExcludedAssemblyPrefixes`](#assembly-exclusions). Note that the customization also affects
> assembly scanning and assembly-level dependency assertions.

#### Dependency cycles

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
[custom dependency resolver](#dependency-resolver) (e.g. an IL-level one) also sharpens cycle
detection: body-level references it surfaces can complete a cycle that the signature-level default cannot see.

### Layers as type selections

There is no separate rule engine: a "layer" is just a reusable `Filtered.Types` selection (with the full
filter vocabulary at your disposal), and an architecture rule is just an expectation on it.

```csharp
Filtered.Types domain         = Types.InNamespace("MyApp.Domain");
Filtered.Types infrastructure = Types.InNamespace("MyApp.Infrastructure");
Filtered.Types repositories   = Types.InNamespace("MyApp.Data").WithName("Repository").AsSuffix();
```

The dependency assertions and filters accept such a selection as a **target**, alongside the namespace and
specific-type forms: `DependsOn` / `DoesNotDependOn` / `DependsOnlyOn` (and the plural `DependOn` /
`DoNotDependOn` / `DependOnlyOn` and the `WhichDependOn` / `WhichDoNotDependOn` / `WhichDependOnlyOn`
filters) take one or more `Filtered.Types` arguments. Each target selection is resolved once per assertion;
a dependency matches when it is a member of the union of the resolved selections. Matching is by type
identity, where a generic type definition in the selection (e.g. a scanned `Repository<>`) matches any of
its constructions.
Multiple targets and `.OrOn(…)` mean *any of*; for the *only-on* family the union is the allowed set, while
the own-namespace and framework rules apply unchanged, including the `.ExcludingOwnSubNamespaces()` opt-out
(an empty selection thus allows only the own namespace
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

Exemptions to a rule use the [`Except` filter](#filters-and-the-matching-assertions) on the subject
selection:

```csharp
await Expect.That(domain.Except<LegacyService>()).DoNotDependOn(infrastructure);
await Expect.That(domain.Except(type => type.Name.StartsWith("Generated"))).AreSealed();
```

A layer spanning several namespaces is built by widening a dependency *target* with additional selections
(or `.OrOn(…)`); for a *subject* spanning several namespaces, assert each namespace selection as its own
rule inside the same `Expect.ThatAll(…)`.

## Configuration

### Assembly exclusions

By default, assemblies whose name matches one of the following prefixes are excluded from
`In.AllLoadedAssemblies()`:

`mscorlib`, `System`, `Microsoft`, `netstandard`, `WindowsBase`, `JetBrains`, `xunit`, `Castle`,
`DynamicProxyGenAssembly2`.

Both the assembly scanning and the dependency assertions (`DependsOnlyOn` / `DependOnlyOn` /
`WhichDependOnlyOn`, on both assemblies and types) use the same prefixes with the same matching: a prefix
matches at a name-segment boundary, so `System` covers `System` and `System.Text.Json`, but not an assembly
named `SystemsBiology`. A prefix written with a trailing dot (e.g. `MyCompany.`) is boundary-safe by
construction and covers everything starting with it. Empty prefixes are ignored.

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

### Dependency resolver

The type-level dependency assertions compute a type's dependencies with a built-in signature-level
resolver (base type, interfaces, field/property/event types, method/constructor signatures, generic
arguments and applied attributes). Method-body references are not detected by the default; supply a
custom resolver, e.g. backed by [Mono.Cecil](https://github.com/jbevain/cecil) (this library takes no
dependency on it; reference the package yourself), for IL/body-level accuracy:

```csharp
// Replace the resolver within a scope
using (Customize.aweXpect.Reflection().DependencyResolver()
    .Set(type => MyCecilResolver.GetUsedTypes(type)))
{
    // body-level references now count as dependencies
}

// Or augment instead of replace: compose on the current default
var resolver = Customize.aweXpect.Reflection().DependencyResolver();
var builtin = resolver.Get()!;
using (resolver.Set(type => builtin(type).Concat(MyCecilResolver.GetBodyTypes(type))))
{
    // built-in signature dependencies plus the body-level extras
}

// Setting null reverts to the built-in default, e.g. to opt a single test out
// of a globally configured resolver
using (Customize.aweXpect.Reflection().DependencyResolver().Set(null))
{
    // the signature-level default applies within this scope
}
```

`Get()` always returns the resolver currently in effect (the built-in default when none is configured),
so composing on it works regardless of what an outer scope has set up.

A Mono.Cecil-backed resolver boils down to reading the type's assembly and mapping the IL references
back to runtime types:

```csharp
public static IEnumerable<Type> GetUsedTypes(Type type)
{
    using var assembly = Mono.Cecil.AssemblyDefinition.ReadAssembly(type.Assembly.Location);
    var definition = assembly.MainModule.GetType(type.FullName!.Replace('+', '/'));
    foreach (var instruction in definition.Methods
                 .Where(method => method.HasBody)
                 .SelectMany(method => method.Body.Instructions))
    {
        // map the method/field/type references in instruction.Operand back to System.Type
        // (and also walk the signature surface: base type, interfaces, fields, …)
    }
}
```

Every resolver's output is normalized like the built-in's (array/by-ref/pointer element types and
generic arguments are unwrapped, the result is de-duplicated) and cached per type for the lifetime of
the resolver delegate, so a custom resolver needs no caching of its own. It must, however, be **pure**:
deterministic for a given `Type` within its scope; that is what makes the caching safe.
