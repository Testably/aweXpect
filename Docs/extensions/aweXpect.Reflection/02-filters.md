# Filters and expectations

A **filter** (`.WhichAre…` / `.With…`) narrows a collection *before* you verify it. An **expectation**
(`Expect.That(…).Is… / .Are… / .Has… / .Have…`) states the rule the subject must satisfy. They mirror
each other: a filter has an `Is…`/`Are…` expectation counterpart for the same concept.

| Section                                                     | Applies to                    | Filters / expectations                                                                                               |
|-------------------------------------------------------------|-------------------------------|----------------------------------------------------------------------------------------------------------------------|
| [Access modifiers](#access-modifiers)                       | types and members             | `WhichArePublic` / `IsPublic`, `WhichAre(AccessModifiers)` / `Is(AccessModifiers)`, …                                |
| [Attributes](#attributes)                                   | types, members and assemblies | `With<TAttribute>` / `Has<TAttribute>`, `Without<TAttribute>` / `DoesNotHave<TAttribute>`                            |
| [Obsolete](#obsolete)                                       | types and members             | `WhichAreObsolete` / `IsObsolete`, `WhichAreNotObsolete` / `IsNotObsolete`                                           |
| [Names and namespaces](#names-and-namespaces)               | types and members             | `WithName` / `HasName`, `WithNamespace` / `HasNamespace`, `WhichAreWithinNamespace` / `IsWithinNamespace`            |
| [Types](#types)                                             | types                         | `WhichAreClasses` / `IsAClass`, `WhichInheritFrom<T>` / `InheritsFrom<T>`, `WhichContainMethods` / `ContainsMethods` |
| [Methods](#methods)                                         | methods                       | `WhichAreAsync` / `IsAsync`, `WhichReturn<T>` / `Returns<T>`, `WithParameter<T>` / `HasParameter<T>`                 |
| [Operators](#operators)                                     | methods and types             | `WhichAreOperators` / `IsAnOperator`, `HasOperator`, `HasImplicitConversionOperator`                                 |
| [Properties & Fields](#properties--fields)                  | properties and fields         | `OfType<T>` / `IsOfType<T>`, `WhichAreNullable` / `IsNullable`, `WhichAreRequired` / `IsRequired`                    |
| [Events](#events)                                           | events                        | `OfType<T>` / `IsOfType<T>`, `WhichAreStatic` / `IsStatic`, `WhichAreNullable` / `IsNullable`                        |
| [Constructors](#constructors)                               | constructors                  | `WithoutParameters` / `HasNoParameters`, `WithParameter<T>` / `HasParameter<T>`                                      |
| [Assemblies](#assemblies)                                   | assemblies                    | `WhichTarget` / `Targets`, `WithVersion` / `HasVersion`, `WhichDependOn` / `DependsOn`                               |
| [Custom predicates](#custom-predicates)                     | types, members and assemblies | `Which` / `Satisfies`, `Except`                                                                                      |
| [Combining filters](#combining-filters)                     | filters                       | `OrWith<TAttribute>`, `OrReturn<T>`, `OrOfType<T>`                                                                   |
| [Inheritance](#inheritance)                                 | types and members             | `Directly`, `IncludingInherited`, `IncludingObjectMembers`                                                           |
| [String matching options](#string-matching-options)         | names and namespaces          | `AsPrefix`, `AsSuffix`, `AsWildcard`, `AsRegex`, `IgnoringCase`                                                      |
| [Collections and quantifiers](#collections-and-quantifiers) | all subjects                  | `Satisfies`, `All().Satisfy`, `Any().Satisfy`                                                                        |

The tables in the sections below pair them up. The **Filter** column is used on a selection (`In.…` or
`Types.…`); the **Expect (single)** column applies to one subject; the **Expect (many)** column applies to a
collection. The **Negated** column lists the negations in the same order (filter, single, many), leaving out a form
that has none; a row without a negation shows `-`.

## Access modifiers

Shared by all types and members: these names are identical for types, methods, properties, fields,
events and constructors.

| Modifier           | Filter                                                 | Expect (single)          | Expect (many)             | Negated                                                                             |
|--------------------|--------------------------------------------------------|--------------------------|---------------------------|-------------------------------------------------------------------------------------|
| public             | `.WhichArePublic()` / `.Public`                        | `.IsPublic()`            | `.ArePublic()`            | `WhichAreNotPublic`, `IsNotPublic`, `AreNotPublic`                                  |
| internal           | `.WhichAreInternal()` / `.Internal`                    | `.IsInternal()`          | `.AreInternal()`          | `WhichAreNotInternal`, `IsNotInternal`, `AreNotInternal`                            |
| private            | `.WhichArePrivate()` / `.Private`                      | `.IsPrivate()`           | `.ArePrivate()`           | `WhichAreNotPrivate`, `IsNotPrivate`, `AreNotPrivate`                               |
| protected          | `.WhichAreProtected()` / `.Protected`                  | `.IsProtected()`         | `.AreProtected()`         | `WhichAreNotProtected`, `IsNotProtected`, `AreNotProtected`                         |
| private protected  | `.WhichArePrivateProtected()` / `.Private.Protected`   | `.IsPrivateProtected()`  | `.ArePrivateProtected()`  | `WhichAreNotPrivateProtected`, `IsNotPrivateProtected`, `AreNotPrivateProtected`    |
| protected internal | `.WhichAreProtectedInternal()` / `.Protected.Internal` | `.IsProtectedInternal()` | `.AreProtectedInternal()` | `WhichAreNotProtectedInternal`, `IsNotProtectedInternal`, `AreNotProtectedInternal` |
| as a value         | `.WhichAre(AccessModifiers)`                           | `.Is(AccessModifiers)`   | `.Are(AccessModifiers)`   | `WhichAreNot(AccessModifiers)`, `IsNot(AccessModifiers)`, `AreNot(AccessModifiers)` |

```csharp
// Filter, then verify
In.AllLoadedAssemblies().Public.Methods()        // shorthand modifier
In.AllLoadedAssemblies().Methods().WhichArePublic()

await Expect.That(method).IsPublic();
await Expect.That(methods).ArePublic();
```

To pass the modifier as a value, e.g. in a parameterized test, use the *as a value* row, on types and on every
member kind. Combined flags are met by any of them, e.g. `AccessModifiers.Public | AccessModifiers.Internal` reads "is public or internal", and
`AccessModifiers.Any` is met by every type and member.

```csharp
[Test]
[Arguments(AccessModifiers.Public)]
[Arguments(AccessModifiers.Public | AccessModifiers.Internal)]
public async Task TypesAreNotNested(AccessModifiers modifier)
{
    await Expect.That(In.AssemblyContaining<PlaylistService>().Types().WhichAre(modifier))
        .AreNotNested();
}

[Test]
[Arguments(typeof(PlaylistService), AccessModifiers.Public)]
[Arguments(typeof(TrackCache), AccessModifiers.Internal)]
public async Task TypeHasTheExpectedAccessModifier(Type type, AccessModifiers modifier)
{
    await Expect.That(type).Is(modifier);
}

await Expect.That(methods).AreNot(AccessModifiers.Private | AccessModifiers.PrivateProtected);
```

## Attributes

Shared by all types, members, and assemblies.

|                                    | Filter                      | Expect (single)            | Expect (many)               | Negated                                                                   |
|------------------------------------|-----------------------------|----------------------------|-----------------------------|---------------------------------------------------------------------------|
| has attribute                      | `.With<TAttribute>()`       | `.Has<TAttribute>()`       | `.Have<TAttribute>()`       | `Without<TAttribute>`, `DoesNotHave<TAttribute>`, `DoNotHave<TAttribute>` |
| has attribute matching a predicate | `.With<TAttribute>(a => …)` | `.Has<TAttribute>(a => …)` | `.Have<TAttribute>(a => …)` | -                                                                         |
| any of several attributes          | `.With<T1>().OrWith<T2>()`  | -                          | -                           | -                                                                         |

For types, methods, properties and events, attributes inherited from a base type or an overridden member are
considered as well; append `.Directly()` to only consider the attributes declared on the item itself, e.g.
`.With<TAttribute>().Directly()` or `.Has<TAttribute>().Directly()` (see [Inheritance](#inheritance)). Chain multiple `.Without<TAttribute>()` calls to exclude several attributes (an item
must have none of them).

```csharp
await Expect.That(type).Has<ObsoleteAttribute>(a => a.Message == "Use NewClass instead");
await Expect.That(methods).Have<FactAttribute>();
```

## Obsolete

Shared by all types and members: a self-documenting shorthand for the `ObsoleteAttribute` (so that
`.WhichAreObsolete()` reads better than `.With<ObsoleteAttribute>()` in architecture rules).

|          | Filter                | Expect (single) | Expect (many)    | Negated                                                  |
|----------|-----------------------|-----------------|------------------|----------------------------------------------------------|
| obsolete | `.WhichAreObsolete()` | `.IsObsolete()` | `.AreObsolete()` | `WhichAreNotObsolete`, `IsNotObsolete`, `AreNotObsolete` |

```csharp
// Verify that nothing public is marked [Obsolete]
await Expect.That(In.AssemblyContaining<PlaylistService>().Types().WhichArePublic())
    .AreNotObsolete();
```

## Names and namespaces

Shared by all types and members.

|                                 | Filter                          | Expect (single)           | Expect (many)              | Negated                                                                       |
|---------------------------------|---------------------------------|---------------------------|----------------------------|-------------------------------------------------------------------------------|
| by name                         | `.WithName("x")`                | `.HasName("x")`           | `.HaveName("x")`           | `WithoutName`, `DoesNotHaveName`, `DoNotHaveName`                             |
| by namespace *(types only)*     | `.WithNamespace("x")`           | `.HasNamespace("x")`      | `.HaveNamespace("x")`      | `WithoutNamespace`, `DoesNotHaveNamespace`, `DoNotHaveNamespace`              |
| within namespace *(types only)* | `.WhichAreWithinNamespace("x")` | `.IsWithinNamespace("x")` | `.AreWithinNamespace("x")` | `WhichAreNotWithinNamespace`, `IsNotWithinNamespace`, `AreNotWithinNamespace` |

The negated filter keeps the items whose name does *not* match, the single expectation verifies an item is *not*
named the given value, and the collection expectation verifies that *none* of the items are; the same holds for the
namespace of types. They accept the same [string matching options](#string-matching-options) as their positive
counterparts:

```csharp
// Verify that no type in the production assembly is named with a "Test" suffix
await Expect.That(In.AssemblyContaining<PlaylistService>().Types())
    .DoNotHaveName("Test").AsSuffix();
```

The name/namespace *equality* filters and expectations (`WithName`, `WithNamespace`, and their
`Has`/`Have` counterparts) accept the
[string matching options](#string-matching-options) (`AsPrefix`, `AsSuffix`, `AsWildcard`,
`AsRegex`, `IgnoringCase`, …). Collection expectations also accept a selector to derive the expected name per
item:

```csharp
await Expect.That(types).HaveName("Service").AsSuffix();
await Expect.That(methods).HaveName(m => "On" + m.GetParameters()[0].ParameterType.Name);
```

The `WhichAreWithinNamespace`/`IsWithinNamespace`/`AreWithinNamespace` variants match a namespace and all
its sub-namespaces (so `MusicStore.Data` includes `MusicStore.Data.Sql` but not `MusicStore.DataImport`). They
compare the namespace exactly and case-sensitively. Their negations match types outside the namespace. Unlike `DoesNotHaveNamespace`, which compares the whole namespace as a string with the
string matching options, `IsNotWithinNamespace` also excludes all sub-namespaces.

## Types

| Kind                      | Filter                                          | Expect (single)                | Expect (many)                   | Negated                                                                                               |
|---------------------------|-------------------------------------------------|--------------------------------|---------------------------------|-------------------------------------------------------------------------------------------------------|
| class                     | `.WhichAreClasses()` / `.Classes()`             | `.IsAClass()`                  | `.AreClasses()`                 | `WhichAreNotClasses`, `IsNotAClass`, `AreNotClasses`                                                  |
| interface                 | `.WhichAreInterfaces()` / `.Interfaces()`       | `.IsAnInterface()`             | `.AreInterfaces()`              | `WhichAreNotInterfaces`, `IsNotAnInterface`, `AreNotInterfaces`                                       |
| enum                      | `.WhichAreEnums()` / `.Enums()`                 | `.IsAnEnum()`                  | `.AreEnums()`                   | `WhichAreNotEnums`, `IsNotAnEnum`, `AreNotEnums`                                                      |
| struct                    | `.WhichAreStructs()` / `.Structs()`             | `.IsAStruct()`                 | `.AreStructs()`                 | `WhichAreNotStructs`, `IsNotAStruct`, `AreNotStructs`                                                 |
| record                    | `.WhichAreRecords()` / `.Records()`             | `.IsARecord()`                 | `.AreRecords()`                 | `WhichAreNotRecords`, `IsNotARecord`, `AreNotRecords`                                                 |
| record struct             | `.WhichAreRecordStructs()` / `.RecordStructs()` | `.IsARecordStruct()`           | `.AreRecordStructs()`           | `WhichAreNotRecordStructs`, `IsNotARecordStruct`, `AreNotRecordStructs`                               |
| readonly struct           | `.WhichAreReadOnly()`                           | `.IsReadOnly()`                | `.AreReadOnly()`                | `WhichAreNotReadOnly`, `IsNotReadOnly`, `AreNotReadOnly`                                              |
| ref struct                | `.WhichAreRefStructs()`                         | `.IsARefStruct()`              | `.AreRefStructs()`              | `WhichAreNotRefStructs`, `IsNotARefStruct`, `AreNotRefStructs`                                        |
| delegate                  | `.WhichAreDelegates()`                          | `.IsADelegate()`               | `.AreDelegates()`               | `WhichAreNotDelegates`, `IsNotADelegate`, `AreNotDelegates`                                           |
| exception                 | `.WhichAreExceptions()`                         | `.IsAnException()`             | `.AreExceptions()`              | `WhichAreNotExceptions`, `IsNotAnException`, `AreNotExceptions`                                       |
| attribute                 | `.WhichAreAttributes()`                         | `.IsAnAttribute()`             | `.AreAttributes()`              | `WhichAreNotAttributes`, `IsNotAnAttribute`, `AreNotAttributes`                                       |
| abstract                  | `.WhichAreAbstract()` / `.Abstract`             | `.IsAbstract()`                | `.AreAbstract()`                | `WhichAreNotAbstract`, `IsNotAbstract`, `AreNotAbstract`                                              |
| sealed                    | `.WhichAreSealed()` / `.Sealed`                 | `.IsSealed()`                  | `.AreSealed()`                  | `WhichAreNotSealed`, `IsNotSealed`, `AreNotSealed`                                                    |
| static                    | `.WhichAreStatic()` / `.Static`                 | `.IsStatic()`                  | `.AreStatic()`                  | `WhichAreNotStatic`, `IsNotStatic`, `AreNotStatic`                                                    |
| generic                   | `.WhichAreGeneric()` / `.Generic`               | `.IsGeneric()`                 | `.AreGeneric()`                 | `WhichAreNotGeneric`, `IsNotGeneric`, `AreNotGeneric`                                                 |
| nested                    | `.WhichAreNested()` / `.Nested`                 | `.IsNested()`                  | `.AreNested()`                  | `WhichAreNotNested`, `IsNotNested`, `AreNotNested`                                                    |
| inherits from             | `.WhichInheritFrom<T>()`                        | `.InheritsFrom<T>()`           | `.InheritFrom<T>()`             | `WhichDoNotInheritFrom<T>`, `DoesNotInheritFrom<T>`, `DoNotInheritFrom<T>`                            |
| implements                | `.WhichImplement<T>()`                          | `.Implements<T>()`             | `.Implement<T>()`               | `WhichDoNotImplement<T>`, `DoesNotImplement<T>`, `DoNotImplement<T>`                                  |
| assignable to             | `.WhichAreAssignableTo<T>()`                    | `.IsAssignableTo<T>()`         | `.AreAssignableTo<T>()`         | `WhichAreNotAssignableTo<T>`, `IsNotAssignableTo<T>`, `AreNotAssignableTo<T>`                         |
| assignable from           | `.WhichAreAssignableFrom<T>()`                  | `.IsAssignableFrom<T>()`       | `.AreAssignableFrom<T>()`       | `WhichAreNotAssignableFrom<T>`, `IsNotAssignableFrom<T>`, `AreNotAssignableFrom<T>`                   |
| instantiable              | `.WhichAreInstantiable()`                       | `.IsInstantiable()`            | `.AreInstantiable()`            | `WhichAreNotInstantiable`, `IsNotInstantiable`, `AreNotInstantiable`                                  |
| immutable                 | `.WhichAreImmutable()`                          | `.IsImmutable()`               | `.AreImmutable()`               | `WhichAreNotImmutable`, `IsNotImmutable`, `AreNotImmutable`                                           |
| default constructor       | `.WhichHaveADefaultConstructor()`               | `.HasADefaultConstructor()`    | `.HaveADefaultConstructor()`    | `WhichDoNotHaveADefaultConstructor`, `DoesNotHaveADefaultConstructor`, `DoNotHaveADefaultConstructor` |
| only nullable members     | `.WhichOnlyHaveNullableMembers()`               | `.OnlyHasNullableMembers()`    | `.OnlyHaveNullableMembers()`    | -                                                                                                     |
| only non-nullable members | `.WhichOnlyHaveNonNullableMembers()`            | `.OnlyHasNonNullableMembers()` | `.OnlyHaveNonNullableMembers()` | -                                                                                                     |
| custom predicate          | `.Which(t => …)`                                | `.Satisfies(t => …)`           | `.All().Satisfy(t => …)`        | `Except(t => …)`, `Except<T>()`                                                                       |

`WhichInheritFrom` / `InheritsFrom` consider only the **base-class chain** (not implemented interfaces) and
accept a generic argument or a `Type`. Append `.Directly()` to require the base class to be the *direct* parent.
For interfaces, use `Implements`.

`WhichImplement` / `Implements` consider only implemented **interfaces**. With
`.Directly()`, an interface reached only through a base class or through another implemented interface does not
count as *directly* implemented. The negations accept `.Directly()` as well (e.g.
`WhichDoNotInheritFrom<T>().Directly()`, `DoesNotImplement<T>().Directly()`).

```csharp
await Expect.That(typeof(AlbumController)).InheritsFrom<ControllerBase>().Directly();
await Expect.That(typeof(AlbumRepository)).Implements<IRepository>().Directly();
```

`IsAssignableTo` / `IsAssignableFrom` (and their `WhichAre…` / `Are…` forms) use runtime assignability, which
covers base classes *and* interfaces in one step, treats a type as assignable to itself, and honors closed
generic variance.

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

A type is *immutable* when all instance fields (including inherited ones) are `readonly` and all instance
properties (including inherited ones) have no setter or an `init`-only setter. Static members do not affect
immutability. Failure messages list the offending mutable members for actionable feedback.

`OnlyHasNullableMembers` / `OnlyHaveNullableMembers` (and the non-nullable counterparts) verify the
[nullability](#nullability) of all fields, properties and events of the type, including the inherited ones unless
`.Directly()` is appended; the failure message lists the non-compliant members per type:

```csharp
await Expect.That(In.AssemblyContaining<AddTrackRequest>()
        .Types().WithName("Request").AsSuffix())
    .OnlyHaveNullableMembers();
```

### Types containing specific members

You can select types based on their members. The lambda receives the members of each individual type, including
the inherited ones unless `.Directly()` is appended (see [Inheritance](#inheritance)), and may use the full
member-filter API:

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
quantifiers as in [aweXpect](https://docs.testably.org/aweXpect/collections/items#quantifiers)) to require a
specific count:

```csharp
In.AllLoadedAssemblies().Types()
    .WhichContainConstructors(c => c.WithoutParameters()).Exactly(1)
    .WhichContainMethods(m => m.With<ObsoleteAttribute>()).Never()
    .WhichContainProperties(p => p.With<RequiredAttribute>()).AtLeast(2)
    .WhichContainFields(f => f.WhichArePrivate()).Between(1).And(5)
```

Each quantifier applies only to the condition it directly follows; all other conditions implicitly require
the member to occur _at least once_. The available quantifiers are `Exactly`, `AtLeast`, `AtMost`,
`MoreThan`, `LessThan`, `Between(…).And(…)`, `Never`, `Once` and `Twice`. The counts are plain `int` values, both on
filters and on expectations.

The same five member kinds are available as **expectations** on a single `Type` (`Contains…`) and on a
collection of types (`Contain…`), using the same member-filter lambdas and the same quantifiers (default:
_at least one_ matching member):

| Member kind  | Filter                         | Expect (single)            | Expect (many)             | Negated        |
|--------------|--------------------------------|----------------------------|---------------------------|----------------|
| methods      | `.WhichContainMethods(…)`      | `.ContainsMethods(…)`      | `.ContainMethods(…)`      | use `.Never()` |
| properties   | `.WhichContainProperties(…)`   | `.ContainsProperties(…)`   | `.ContainProperties(…)`   | use `.Never()` |
| fields       | `.WhichContainFields(…)`       | `.ContainsFields(…)`       | `.ContainFields(…)`       | use `.Never()` |
| events       | `.WhichContainEvents(…)`       | `.ContainsEvents(…)`       | `.ContainEvents(…)`       | use `.Never()` |
| constructors | `.WhichContainConstructors(…)` | `.ContainsConstructors(…)` | `.ContainConstructors(…)` | use `.Never()` |

```csharp
// A single type contains at least one [Fact] or [Theory] method
await Expect.That(typeof(PlaylistServiceTests))
    .ContainsMethods(methods => methods.With<FactAttribute>().OrWith<TheoryAttribute>());

// …with exactly one parameterless constructor
await Expect.That(typeof(PlaylistService))
    .ContainsConstructors(constructors => constructors.WithoutParameters()).Exactly(1);

// Every type in a collection contains a matching property
await Expect.That(types)
    .ContainProperties(properties => properties.With<RequiredAttribute>()).AtLeast(1);
```

### Generic type arguments

After `.WhichAreGeneric()` you can drill into the generic arguments themselves. The same filters are
available on generic **methods** (`.Methods().WhichAreGeneric()`).

| Filter                               | Selects generic types/methods…                                                        |
|--------------------------------------|---------------------------------------------------------------------------------------|
| `.WithArgumentCount(n)`              | with exactly `n` generic arguments                                                    |
| `.WithArgument<T>()`                 | with a generic argument constrained to `T`                                            |
| `.WithArgument<T>("name")`           | with a generic argument constrained to `T` and named `"name"`                         |
| `.WithArgument("name")`              | with a generic argument named `"name"`                                                |
| `.AtIndex(n)` / `.AtIndexFromEnd(n)` | restrict the preceding `WithArgument` to a position (optionally counted from the end) |

The `"name"` overloads accept the [string matching options](#string-matching-options), and
`.AtIndex(n)` or `.AtIndexFromEnd(n)` is chained after a `WithArgument` to pin it to a specific position.

```csharp
// Generic types with a single argument constrained to IEntity
In.AllLoadedAssemblies().Types()
    .WhichAreGeneric().WithArgumentCount(1).WithArgument<IEntity>()

// Generic methods whose first argument is named "TKey"
In.AllLoadedAssemblies().Methods()
    .WhichAreGeneric().WithArgument("TKey").AtIndex(0)
```

## Methods

In addition to [access modifiers](#access-modifiers),
[attributes](#attributes) and
[names](#names-and-namespaces):

|                                      | Filter                                              | Expect (single)                                                 | Expect (many)                          | Negated                                                                           |
|--------------------------------------|-----------------------------------------------------|-----------------------------------------------------------------|----------------------------------------|-----------------------------------------------------------------------------------|
| static / abstract / sealed / generic | `.WhichAreStatic()` …                               | `.IsStatic()` …                                                 | `.AreStatic()` …                       | `WhichAreNotStatic`, `IsNotStatic`, `AreNotStatic` …                              |
| async                                | `.WhichAreAsync()`                                  | `.IsAsync()`                                                    | `.AreAsync()`                          | `WhichAreNotAsync`, `IsNotAsync`, `AreNotAsync`                                   |
| extension method                     | `.WhichAreExtensionMethods()`                       | `.IsAnExtensionMethod()`                                        | `.AreExtensionMethods()`               | `WhichAreNotExtensionMethods`, `IsNotAnExtensionMethod`, `AreNotExtensionMethods` |
| operator                             | `.WhichAreOperators()`                              | `.IsAnOperator()`                                               | `.AreOperators()`                      | `WhichAreNotOperators`, `IsNotAnOperator`, `AreNotOperators`                      |
| virtual                              | `.WhichAreVirtual()`                                | `.IsVirtual()`                                                  | `.AreVirtual()`                        | `WhichAreNotVirtual`, `IsNotVirtual`, `AreNotVirtual`                             |
| overrides a base method              | `.WhichOverride()`                                  | `.Overrides()`                                                  | `.Override()`                          | `WhichDoNotOverride`, `DoesNotOverride`, `DoNotOverride`                          |
| returns type (or a subtype)          | `.WhichReturn<T>()`                                 | `.Returns<T>()`                                                 | `.Return<T>()`                         | `WhichDoNotReturn<T>`, `DoesNotReturn<T>`, `DoNotReturn<T>`                       |
| returns exactly                      | `.WhichReturnExactly<T>()`                          | `.ReturnsExactly<T>()`                                          | `.ReturnExactly<T>()`                  | `WhichDoNotReturnExactly<T>`, `DoesNotReturnExactly<T>`, `DoNotReturnExactly<T>`  |
| returns void                         | `.WhichReturnVoid()`                                | `.ReturnsVoid()`                                                | `.ReturnVoid()`                        | `WhichDoNotReturnVoid`, `DoesNotReturnVoid`, `DoNotReturnVoid`                    |
| no parameters                        | `.WithoutParameters()`                              | `.HasNoParameters()`                                            | `.HaveNoParameters()`                  | -                                                                                 |
| parameter of type (or subtype)       | `.WithParameter<T>()` / `.WithParameter<T>("name")` | `.HasParameter<T>()` / `.HasParameter<T>("name")`               | `.HaveParameter<T>()`                  | `WithoutParameter<T>`, `DoesNotHaveParameter<T>`, `DoNotHaveParameter<T>`         |
| parameter of exact type              | `.WithParameterExactly<T>()`                        | `.HasParameterExactly<T>()` / `.HasParameterExactly<T>("name")` | `.HaveParameterExactly<T>()`           | -                                                                                 |
| parameter count                      | `.WithParameterCount(n)`                            | `.HasParameterCount(n)`                                         | `.HaveParameterCount(n)`               | -                                                                                 |
| `ref` parameter                      | `.WithRefParameter()`                               | `.HasRefParameter()`                                            | `.HaveRefParameter()`                  | -                                                                                 |
| `out` parameter                      | `.WithOutParameter()`                               | `.HasOutParameter()`                                            | `.HaveOutParameter()`                  | -                                                                                 |
| `in` parameter                       | `.WithInParameter()`                                | `.HasInParameter()`                                             | `.HaveInParameter()`                   | -                                                                                 |
| `params` parameter                   | `.WithParamsParameter()`                            | `.HasParamsParameter()`                                         | `.HaveParamsParameter()`               | -                                                                                 |
| optional parameter                   | `.WithOptionalParameter()`                          | `.HasOptionalParameter()`                                       | `.HaveOptionalParameter()`             | -                                                                                 |
| parameter matching a predicate       | `.WithParameter<T>().Matching(p => …)`              | `.HasParameter<T>().Matching(p => …)`                           | `.HaveParameter<T>().Matching(p => …)` | `WithoutParameter<T>().Matching(p => …)`, …                                       |
| custom predicate                     | `.Which(m => …)`                                    | `.Satisfies(m => …)`                                            | `.All().Satisfy(m => …)`               | `Except(m => …)`                                                                  |

`WhichReturn<Task>()` and `Returns<Task>()` also match `Task<T>`; the `…Exactly` variants match only the
exact type. Use `OrReturn(s)<T>()` / `OrReturn(s)Exactly<T>()` to allow several return types (the single-subject
expectation is `OrReturns…`, the filter and collection expectation are `OrReturn…`). Since `void` cannot be
used as a generic type argument, use `WhichReturnVoid()` / `ReturnsVoid()` / `ReturnVoid()` to match
void-returning methods.

The negations `WhichDoNotReturn…` / `DoesNotReturn…` / `DoNotReturn…` match methods that do not return the
type; on a collection, `DoNotReturn<T>()` verifies that *none* of the methods return it. They have no `Or…`
companion: chain `.WhichDoNotReturn<T1>().WhichDoNotReturn<T2>()` (or combine expectations with `.And`) to
exclude several return types.

```csharp
// Async methods never return void
await Expect.That(In.AssemblyContaining<PlaylistService>().Methods().WhichAreAsync())
    .DoNotReturnVoid();
```

The `ref` / `out` / `in` / `params` / optional parameter filters and expectations mirror `WithParameter`:
each also has `<T>()`, `(Type)`, `<T>("name")` and `…Exactly<T>()` overloads to constrain the parameter's type
and name (e.g. `.WithRefParameter<int>("count")`, `.HasOutParameterExactly<string>()`).

`WithoutParameter` / `DoesNotHaveParameter` / `DoNotHaveParameter` are the negations of `WithParameter` /
`HasParameter` / `HaveParameter`, with the `<T>()`, `(Type)`, `<T>("name")`, `(Type, "name")` and `("name")`
overloads and the same options (`AtIndex(…)`, `AtIndexFromEnd(…)`, `WithDefaultValue(…)`, the string matching options for
the name and, on the filter, the modifiers like `WithRefModifier()`). They match when *no* parameter satisfies all
of them, and `DoNotHaveParameter` verifies that for every item of a collection. The `…Exactly` and the `ref` /
`out` / `in` / `params` / optional variants have no negation; on filters use e.g.
`.WithoutParameter<int>().WithRefModifier()`.

```csharp
// Public methods take no bool flag
await Expect.That(In.AssemblyContaining<PlaylistService>().Public.Methods())
    .DoNotHaveParameter<bool>();
```

`Matching(p => …)` narrows down the parameter with a predicate on its `ParameterInfo`, for anything the other
options do not cover (attributes, position, type details, …). It is available on every parameter filter and
expectation, including the negations and the parameterless `ref` / `out` / `in` / `params` / optional variants, and
combines with the other options. `WithParamsParameter()` matches `params` arrays and C# 13 `params` collections alike;
`Matching` tells them apart:

```csharp
// Consumers below C# 13 cannot pass single values to a params collection
await Expect.That(In.AssemblyContaining<PlaylistService>().Public.Methods()
        .WithParamsParameter().Matching(p => !p.ParameterType.IsArray))
    .IsEmpty();
```

`WhichAreExtensionMethods()`, `IsAnExtensionMethod()` and `AreExtensionMethods()` match both classic `this`-parameter
extension methods and extension methods declared with the C# extension block syntax (`extension(...) { … }`), including
static extension methods.

```csharp
In.AllLoadedAssemblies().Methods()
    .WhichArePublic()
    .WhichReturn<Task>().OrReturn<ValueTask>()
    .WithParameter<CancellationToken>()
    .With<HttpGetAttribute>().OrWith<HttpPostAttribute>()

await Expect.That(method).HasParameter<int>("count");
await Expect.That(methods).Return<Task>().OrReturn<ValueTask>();
```

<details>
<summary>Extension block internals</summary>

The compiler-generated grouping types backing the extension block syntax are excluded from the reflected members.

</details>

## Operators

The `Operator` enum maps each C# operator to its compiler-emitted `op_*` metadata name (e.g.
`Operator.Addition` ↔ `op_Addition`), so operator expectations are type-safe and discoverable instead of relying
on magic strings. It covers unary, binary, comparison and conversion operators, including the C# 11 `checked`
variants and `>>>` (`UnsignedRightShift`).

| Scope               | Filter                         | Expect (single)                                      | Expect (many)                                         | Negated                                                                                                            |
|---------------------|--------------------------------|------------------------------------------------------|-------------------------------------------------------|--------------------------------------------------------------------------------------------------------------------|
| any operator method | `.WhichAreOperators()`         | `.IsAnOperator()`                                    | `.AreOperators()`                                     | `WhichAreNotOperators`, `IsNotAnOperator`, `AreNotOperators`                                                       |
| specific operator   | `.WhichAreOperators(Operator)` | `.IsAnOperator(Operator)`                            | `.AreOperators(Operator)`                             | `WhichAreNotOperators(Operator)`, `IsNotAnOperator(Operator)`, `AreNotOperators(Operator)`                         |
| type has operator   | n/a                            | `.HasOperator(Operator)`                             | `.HaveOperator(Operator)`                             | `DoesNotHaveOperator(Operator)`, `DoNotHaveOperator(Operator)`                                                     |
| implicit conversion | n/a                            | `.HasImplicitConversionOperator<TSource, TTarget>()` | `.HaveImplicitConversionOperator<TSource, TTarget>()` | `DoesNotHaveImplicitConversionOperator<TSource, TTarget>`, `DoNotHaveImplicitConversionOperator<TSource, TTarget>` |
| explicit conversion | n/a                            | `.HasExplicitConversionOperator<TSource, TTarget>()` | `.HaveExplicitConversionOperator<TSource, TTarget>()` | `DoesNotHaveExplicitConversionOperator<TSource, TTarget>`, `DoNotHaveExplicitConversionOperator<TSource, TTarget>` |

```csharp
// Method-level: narrow an operator method to a specific operator
await Expect.That(methodInfo).IsAnOperator(Operator.Equality);
await Expect.That(methods).AreOperators(Operator.Equality);

// Type-level presence (op_* lookup), including the operators of base types
await Expect.That(typeof(TrackDuration)).HasOperator(Operator.Addition);
await Expect.That(typeof(TrackDuration)).HasOperator(Operator.Subtraction).Directly();
await Expect.That(typeof(TrackDuration)).DoesNotHaveOperator(Operator.Modulus);

// Disambiguate overloads by operand type
await Expect.That(typeof(TrackDuration)).HasOperator<int>(Operator.Addition);

// Conversion operators are keyed by their source → target signature
await Expect.That(typeof(TrackDuration)).HasImplicitConversionOperator<TrackDuration, TimeSpan>();
await Expect.That(typeof(TrackDuration)).HasExplicitConversionOperator(typeof(TrackDuration), typeof(int));
```

`HasOperator` / conversion operators also match the operators inherited from base types; append `.Directly()` to only
match the operators declared on the type itself. Conversion source/target types are matched exactly. The negations
also come with the operand overloads, e.g. `DoesNotHaveOperator<int>(Operator)`.

:::note[Operators are hidden by default]
Operators are special-name members that are
[hidden by default](./04-configuration.md#compiler-generated-members), so the
plain `.Methods()` collection excludes them unless opted in via `IncludedSpecialNameMembers`. The
`.WhichAreOperators(Operator)` filter implicitly re-includes operators for its query, so it works without
that configuration. The negative `.WhichAreNotOperators(Operator)` filter does **not** re-include operators; if you
want the other operators in that result, opt in via `IncludedSpecialNameMembers`.
:::

<details>
<summary>Why WhichAreNotOperators does not re-include operators</summary>

A "not this operator" filter over `.Methods()` is meant to narrow regular methods, and force-including every
*other* operator would surprise more than help.

</details>

## Properties & Fields

In addition to [access modifiers](#access-modifiers),
[attributes](#attributes) and
[names](#names-and-namespaces):

|                                        | Filter                                      | Expect (single)                 | Expect (many)                     | Negated                                                                                                       |
|----------------------------------------|---------------------------------------------|---------------------------------|-----------------------------------|---------------------------------------------------------------------------------------------------------------|
| of type (or a subtype)                 | `.OfType<T>()`                              | `.IsOfType<T>()`                | `.AreOfType<T>()`                 | `NotOfType<T>`, `IsNotOfType<T>`, `AreNotOfType<T>`                                                           |
| of exact type                          | `.OfExactType<T>()`                         | `.IsOfExactType<T>()`           | `.AreOfExactType<T>()`            | `NotOfExactType<T>`, `IsNotOfExactType<T>`, `AreNotOfExactType<T>`                                            |
| static *(properties & fields)*         | `.WhichAreStatic()`                         | `.IsStatic()`                   | `.AreStatic()`                    | `WhichAreNotStatic`, `IsNotStatic`, `AreNotStatic`                                                            |
| nullable *(properties & fields)*       | `.WhichAreNullable()`                       | `.IsNullable()`                 | `.AreNullable()`                  | `WhichAreNotNullable`, `IsNotNullable`, `AreNotNullable`                                                      |
| abstract / sealed *(properties only)*  | `.WhichAreAbstract()` / `.WhichAreSealed()` | `.IsAbstract()` / `.IsSealed()` | `.AreAbstract()` / `.AreSealed()` | `WhichAreNotAbstract`, `IsNotAbstract`, `AreNotAbstract` / `WhichAreNotSealed`, `IsNotSealed`, `AreNotSealed` |
| virtual *(properties only)*            | `.WhichAreVirtual()`                        | `.IsVirtual()`                  | `.AreVirtual()`                   | `WhichAreNotVirtual`, `IsNotVirtual`, `AreNotVirtual`                                                         |
| override *(properties only)*           | `.WhichOverride()`                          | `.Overrides()`                  | `.Override()`                     | `WhichDoNotOverride`, `DoesNotOverride`, `DoNotOverride`                                                      |
| required *(properties & fields)*       | `.WhichAreRequired()`                       | `.IsRequired()`                 | `.AreRequired()`                  | `WhichAreNotRequired`, `IsNotRequired`, `AreNotRequired`                                                      |
| readable *(properties only)*           | `.WhichAreReadable()`                       | `.IsReadable()`                 | `.AreReadable()`                  | use *write-only*                                                                                              |
| writable *(properties only)*           | `.WhichAreWritable()`                       | `.IsWritable()`                 | `.AreWritable()`                  | use *read-only*                                                                                               |
| read-only *(properties only)*          | `.WhichAreReadOnly()`                       | `.IsReadOnly()`                 | `.AreReadOnly()`                  | use *writable*                                                                                                |
| write-only *(properties only)*         | `.WhichAreWriteOnly()`                      | `.IsWriteOnly()`                | `.AreWriteOnly()`                 | use *readable*                                                                                                |
| read-write *(properties only)*         | `.WhichAreReadWrite()`                      | `.IsReadWrite()`                | `.AreReadWrite()`                 | -                                                                                                             |
| has getter *(properties only)*         | `.WhichHaveAGetter()`                       | `.HasAGetter()`                 | `.HaveAGetter()`                  | -                                                                                                             |
| has setter *(properties only)*         | `.WhichHaveASetter()`                       | `.HasASetter()`                 | `.HaveASetter()`                  | `WhichDoNotHaveASetter`, `DoesNotHaveASetter`, `DoNotHaveASetter`                                             |
| has init setter *(properties only)*    | `.WhichHaveAnInitSetter()`                  | `.HasAnInitSetter()`            | `.HaveAnInitSetter()`             | `WhichDoNotHaveAnInitSetter`, `DoesNotHaveAnInitSetter`, `DoNotHaveAnInitSetter`                              |
| indexer *(properties only)*            | `.WhichAreIndexers()`                       | `.IsAnIndexer()`                | `.AreIndexers()`                  | `WhichAreNotIndexers`, `IsNotAnIndexer`, `AreNotIndexers`                                                     |
| extension property *(properties only)* | `.WhichAreExtensionProperties()`            | `.IsAnExtensionProperty()`      | `.AreExtensionProperties()`       | `WhichAreNotExtensionProperties`, `IsNotAnExtensionProperty`, `AreNotExtensionProperties`                     |
| read-only *(fields only)*              | `.WhichAreReadOnly()`                       | `.IsReadOnly()`                 | `.AreReadOnly()`                  | `WhichAreNotReadOnly`, `IsNotReadOnly`, `AreNotReadOnly`                                                      |
| constant *(fields only)*               | `.WhichAreConstant()`                       | `.IsConstant()`                 | `.AreConstant()`                  | `WhichAreNotConstant`, `IsNotConstant`, `AreNotConstant`                                                      |

An init-only setter is not a (regular) setter: a property with only an init-only setter has no setter.

The `readable`, `writable`, `read-only` and `write-only` rows of properties have no negated form, as each
negation is another row: for *not read-only* use `IsWritable`, for *not writable* use `IsReadOnly`, for
*not readable* use `IsWriteOnly` and for *not write-only* use `IsReadable` (the same holds for the filters and
the plural forms). A property that is *not read-write* is either read-only or write-only.

### Nullability

A property or field counts as *nullable* when its type is a `Nullable<T>` value type (e.g. `int?`) or a
reference type annotated as nullable (e.g. `string?`, based on the nullable reference type metadata emitted
by the compiler). The same applies to [events](#events), whose handler type is always a delegate (reference)
type.

```csharp
// All properties and fields of the request types must be nullable
await Expect.That(In.AssemblyContaining<AddTrackRequest>()
        .Types().WithName("Request").AsSuffix()
        .Properties())
    .AreNullable();
```

`WhichAreExtensionProperties()`, `IsAnExtensionProperty()` and `AreExtensionProperties()` match extension properties
declared with the C# extension block syntax (`extension(...) { … }`), both instance and static.

Use `OrOfType<T>()` / `OrOfExactType<T>()` to allow several types. The negations `NotOfType` / `IsNotOfType` /
`AreNotOfType` (and the `…ExactType` forms) have no `Or…` companion: chain `.NotOfType<T1>().NotOfType<T2>()` (or
combine expectations with `.And`) to exclude several types. On a collection, `AreNotOfType<T>()` verifies that
*none* of the items is of the type.

```csharp
In.AllLoadedAssemblies().Public.Properties()
    .OfType<string>()
    .WithName("Id").AsSuffix()
    .With<RequiredAttribute>()

In.AllLoadedAssemblies().Private.Fields()
    .OfType<ILogger>()
    .WithName("_").AsPrefix()
```

<details>
<summary>Nullability and extension properties in detail</summary>

The nullability check follows the declared annotation on every target framework: reference types without
nullability annotations (oblivious code compiled without `<Nullable>enable</Nullable>`) and unconstrained generic
type parameters (`T`, as opposed to `T?`) count as non-nullable, and post-condition attributes like `[AllowNull]` or
`[MaybeNull]` are ignored.

The real extension properties live on the compiler-generated grouping types backing the extension block, so they are
surfaced from there, while the public accessor methods they emit are excluded from the reflected methods.

</details>

## Events

In addition to [access modifiers](#access-modifiers),
[attributes](#attributes) and
[names](#names-and-namespaces):

|                                | Filter                                      | Expect (single)                 | Expect (many)                     | Negated                                                                                                       |
|--------------------------------|---------------------------------------------|---------------------------------|-----------------------------------|---------------------------------------------------------------------------------------------------------------|
| handler of type (or a subtype) | `.OfType<T>()`                              | `.IsOfType<T>()`                | `.AreOfType<T>()`                 | `NotOfType<T>`, `IsNotOfType<T>`, `AreNotOfType<T>`                                                           |
| handler of exact type          | `.OfExactType<T>()`                         | `.IsOfExactType<T>()`           | `.AreOfExactType<T>()`            | `NotOfExactType<T>`, `IsNotOfExactType<T>`, `AreNotOfExactType<T>`                                            |
| abstract / sealed              | `.WhichAreAbstract()` / `.WhichAreSealed()` | `.IsAbstract()` / `.IsSealed()` | `.AreAbstract()` / `.AreSealed()` | `WhichAreNotAbstract`, `IsNotAbstract`, `AreNotAbstract` / `WhichAreNotSealed`, `IsNotSealed`, `AreNotSealed` |
| static                         | `.WhichAreStatic()`                         | `.IsStatic()`                   | `.AreStatic()`                    | `WhichAreNotStatic`, `IsNotStatic`, `AreNotStatic`                                                            |
| virtual                        | `.WhichAreVirtual()`                        | `.IsVirtual()`                  | `.AreVirtual()`                   | `WhichAreNotVirtual`, `IsNotVirtual`, `AreNotVirtual`                                                         |
| override                       | `.WhichOverride()`                          | `.Overrides()`                  | `.Override()`                     | `WhichDoNotOverride`, `DoesNotOverride`, `DoNotOverride`                                                      |
| nullable                       | `.WhichAreNullable()`                       | `.IsNullable()`                 | `.AreNullable()`                  | `WhichAreNotNullable`, `IsNotNullable`, `AreNotNullable`                                                      |

The `OfType` / `IsOfType` / `AreOfType` filters and expectations match the event's handler type (its
`EventHandlerType`, e.g. `EventHandler<T>`); the `…ExactType` variants match only the exact handler type.
Use `OrOfType<T>()` / `OrOfExactType<T>()` to allow several handler types, and chain
`.NotOfType<T1>().NotOfType<T2>()` to exclude several.

An event counts as *nullable* when its handler type is annotated as nullable
(e.g. `event EventHandler? Changed;`), following the same [nullability](#nullability) rules as
properties and fields.

```csharp
// Every event must use the generic EventHandler<T> pattern
await Expect.That(In.AllLoadedAssemblies().Public.Events())
    .AreOfType(typeof(EventHandler<>));

In.AllLoadedAssemblies().Public.Events()
    .OfType<EventHandler>()
    .WithName("Changed").AsSuffix()
    .With<ObsoleteAttribute>()
```

## Constructors

In addition to [access modifiers](#access-modifiers) and
[attributes](#attributes):

|                                | Filter                                              | Expect (single)                                                 | Expect (many)                          | Negated                                                                   |
|--------------------------------|-----------------------------------------------------|-----------------------------------------------------------------|----------------------------------------|---------------------------------------------------------------------------|
| static                         | `.WhichAreStatic()`                                 | `.IsStatic()`                                                   | `.AreStatic()`                         | `WhichAreNotStatic`, `IsNotStatic`, `AreNotStatic`                        |
| no parameters                  | `.WithoutParameters()`                              | `.HasNoParameters()`                                            | `.HaveNoParameters()`                  | -                                                                         |
| parameter of type (or subtype) | `.WithParameter<T>()` / `.WithParameter<T>("name")` | `.HasParameter<T>()` / `.HasParameter<T>("name")`               | `.HaveParameter<T>()`                  | `WithoutParameter<T>`, `DoesNotHaveParameter<T>`, `DoNotHaveParameter<T>` |
| parameter of exact type        | `.WithParameterExactly<T>()`                        | `.HasParameterExactly<T>()` / `.HasParameterExactly<T>("name")` | `.HaveParameterExactly<T>()`           | -                                                                         |
| parameter count                | `.WithParameterCount(n)`                            | `.HasParameterCount(n)`                                         | `.HaveParameterCount(n)`               | -                                                                         |
| `ref` parameter                | `.WithRefParameter()`                               | `.HasRefParameter()`                                            | `.HaveRefParameter()`                  | -                                                                         |
| `out` parameter                | `.WithOutParameter()`                               | `.HasOutParameter()`                                            | `.HaveOutParameter()`                  | -                                                                         |
| `in` parameter                 | `.WithInParameter()`                                | `.HasInParameter()`                                             | `.HaveInParameter()`                   | -                                                                         |
| `params` parameter             | `.WithParamsParameter()`                            | `.HasParamsParameter()`                                         | `.HaveParamsParameter()`               | -                                                                         |
| optional parameter             | `.WithOptionalParameter()`                          | `.HasOptionalParameter()`                                       | `.HaveOptionalParameter()`             | -                                                                         |
| parameter matching a predicate | `.WithParameter<T>().Matching(p => …)`              | `.HasParameter<T>().Matching(p => …)`                           | `.HaveParameter<T>().Matching(p => …)` | `WithoutParameter<T>().Matching(p => …)`, …                               |
The `ref` / `out` / `in` / `params` / optional parameter filters and expectations accept the same
`<T>()`, `(Type)`, `<T>("name")` and `…Exactly<T>()` overloads as `WithParameter`, and all parameter filters and
expectations accept `Matching(p => …)` (see [Methods](#methods)).

```csharp
In.AllLoadedAssemblies().Public.Constructors()
    .WithParameterCount(1)
    .WithParameter<string>()
    .With<JsonConstructorAttribute>()
```

## Assemblies

Assemblies are usually used as a [source](./01-sources.md#sources-the-in-helper), but you can also filter
and verify them directly:

|                     | Filter                       | Expect (single)          | Expect (many)            | Negated                                                                   |
|---------------------|------------------------------|--------------------------|--------------------------|---------------------------------------------------------------------------|
| by name             | `.WithName("x")`             | `.HasName("x")`          | `.HaveName("x")`         | `WithoutName`, `DoesNotHaveName`, `DoNotHaveName`                         |
| by target framework | `.WhichTarget("net8.0")`     | `.Targets("net8.0")`     | `.Target("net8.0")`      | `WhichDoNotTarget`, `DoesNotTarget`, `DoNotTarget`                        |
| by version          | `.WithVersion(…)`            | `.HasVersion(…)`         | `.HaveVersion(…)`        | -                                                                         |
| strong named        | `.WhichAreStrongNamed()`     | `.IsStrongNamed()`       | `.AreStrongNamed()`      | `WhichAreNotStrongNamed`, `IsNotStrongNamed`, `AreNotStrongNamed`         |
| has attribute       | `.With<TAttribute>()`        | `.Has<TAttribute>()`     | `.Have<TAttribute>()`    | `Without<TAttribute>`, `DoesNotHave<TAttribute>`, `DoNotHave<TAttribute>` |
| depends on assembly | `.WhichDependOn("x")`        | `.DependsOn("x")`        | `.DependOn("x")`         | `WhichDoNotDependOn`, `DoesNotDependOn`, `DoNotDependOn`                  |
| depends only on set | `.WhichDependOnlyOn("x", …)` | `.DependsOnlyOn("x", …)` | `.DependOnlyOn("x", …)`  | -                                                                         |
| custom predicate    | `.Which(a => …)`             | `.Satisfies(a => …)`     | `.All().Satisfy(a => …)` | `Except(a => …)`                                                          |

```csharp
Assembly subject = Assembly.GetEntryAssembly();
Assembly[] subjects = AppDomain.CurrentDomain.GetAssemblies();

await Expect.That(subject).HasName("MusicStore").AsPrefix();
await Expect.That(subject).DependsOn("System.Core");
await Expect.That(subject).DoesNotDependOn("UnwantedDependency");
await Expect.That(subject).DependsOnlyOn("MusicStore.Domain");
await Expect.That(subjects).Have<AssemblyTitleAttribute>();
await Expect.That(subject).Targets("net8.0");
```

The target framework is matched against the short moniker form (e.g. `net8.0`, `netstandard2.0`, `net48`),
derived from the assembly's `[TargetFramework]` attribute. The negations `WhichDoNotTarget` / `DoesNotTarget` / `DoNotTarget`
accept the same [string matching options](#string-matching-options), and `DoNotTarget` verifies that *none* of
the assemblies targets the framework.

An assembly is considered strong named when its name carries a non-empty public key token.

The version filter and expectations come in two forms. Pass a `Func<Version, bool>` predicate to match the whole
version, or omit it to compare individual components (`WithMajor`, `WithMinor`, `WithBuild`, `WithRevision`).
The filter and the expectations compare a component with the same operators as the `int` properties of core aweXpect,
e.g. `HasLength()`: `EqualTo`, `GreaterThan`, `GreaterThanOrEqualTo`, `LessThan`, `LessThanOrEqualTo`,
`Between(…).And(…)` and their negations `NotEqualTo`, `NotGreaterThan`, `NotBetween(…).And(…)`, and so on. Component
comparisons chain (all must hold).

```csharp
// Predicate form
In.AllLoadedAssemblies().WithVersion(version => version.Major >= 2)
await Expect.That(subject).HasVersion(version => version.Major >= 2);

// Component form
In.AllLoadedAssemblies().WithVersion().WithMajor.GreaterThanOrEqualTo(2).WithMinor.EqualTo(0)
In.AllLoadedAssemblies().WithVersion().WithMajor.Between(2).And(4).WithMinor.NotLessThan(1)
await Expect.That(subject).HasVersion().WithMajor.GreaterThanOrEqualTo(2);
await Expect.That(subjects).HaveVersion().WithMajor.EqualTo(1);
await Expect.That(subject).HasVersion().WithMajor.Between(2).And(4).WithMinor.NotEqualTo(0);
```

<details>
<summary>Assemblies without a target framework or version</summary>

Assemblies without a `[TargetFramework]` attribute are treated as having no target framework and never match. An
assembly without a version never matches a version filter or expectation. The `Build` and `Revision` components are
`-1` when absent from the version.

</details>

## Custom predicates

When no filter or expectation covers your rule, pass your own predicate. Like the other filters, they are available
on **every** selection (assemblies, types and members):

|                  | Filter           | Expect (single)      | Expect (many)                                       | Negated                                        |
|------------------|------------------|----------------------|-----------------------------------------------------|------------------------------------------------|
| custom predicate | `.Which(x => …)` | `.Satisfies(x => …)` | `.All().Satisfy(x => …)` / `.Any().Satisfy(x => …)` | `Except(x => …)`, `Except<T>()` *(types only)* |

The custom `.Which(…)` filter has a universal expectation counterpart that works for **every** subject kind
(types, members and assemblies): use aweXpect's `.Satisfies(…)` on a single subject and `.All().Satisfy(…)`
(or `.Any().Satisfy(…)`) on a collection. See
[Collections and quantifiers](#collections-and-quantifiers).

The custom `.Except(…)` filter is the inverse of `.Which(…)`: it **removes** the items that match the
predicate. This is handy for defining exemptions to a rule (e.g. *all async methods except this one*). For types
there is also a typed `.Except<T>()` overload that excludes exactly the type `T`.

```csharp
// Methods with a single parameter, except the event handlers
In.AllLoadedAssemblies().Methods()
    .Which(method => method.GetParameters().Length == 1)
    .Except(method => method.Name.StartsWith("On"))

// All types of the album assembly, except one legacy type
In.AssemblyContaining<Album>().Types().Except<LegacyImportService>()
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

## Inheritance

A check on a type considers what the type inherits: inherited attributes, operators, members and relations count by
default, and `.Directly()` restricts the check to what is declared on the item itself, or to a direct relation.
Listing the members of types (`Types().Methods()`, …) only includes the members declared on each type, as an
inherited member would otherwise be listed once for each type that inherits it; `.IncludingInherited()` adds the
inherited members.

The members declared on `System.Object`, `System.ValueType` and `System.Enum` are excluded, as every type would
otherwise have `ToString`, `Equals`, `GetHashCode`, …; `.IncludingObjectMembers()` adds them.

| Area                                                                                          | Default                                                                         | Options                                                                                                         |
|-----------------------------------------------------------------------------------------------|---------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------------------------|
| [Attributes](#attributes) of types, methods, properties and events                            | also the attributes inherited from a base type or an overridden member          | `.Directly()`: only the attributes declared on the item                                                         |
| [Operators](#operators) and conversion operators of types                                     | also the operators of base types                                                | `.Directly()`: only the operators declared on the type                                                          |
| Relations (`InheritsFrom<T>()`, `Implements<T>()`, their filters and negations)               | the base class anywhere in the base-class chain, or any implemented interface   | `.Directly()`: only the direct base class or a directly implemented interface                                   |
| Member checks (`ContainsMethods(…)`, `WhichContainMethods(…)`, `OnlyHasNullableMembers()`, …) | also the inherited members                                                      | `.Directly()`: only the declared members, or `.IncludingObjectMembers()`: also the members of the three roots   |
| Member listings of types (`Types().Methods()`, `In.Type<T>().Properties()`, …)                | only the declared members                                                       | `.IncludingInherited()`: also the inherited members, `.IncludingObjectMembers()`: also those of the three roots |
| Member listings of assemblies (`In.AssemblyContaining<T>().Methods()`, …)                     | each member once, on the type that declares it                                  | -                                                                                                               |
| Constructors, and the attributes of assemblies, constructors and fields                       | no inheritance                                                                  | -                                                                                                               |

```csharp
await Expect.That(typeof(AlbumController)).InheritsFrom<ControllerBase>().Directly();
await Expect.That(typeof(AlbumRepository)).Implements<IRepository>().Directly();
await Expect.That(typeof(TrackDuration)).HasOperator(Operator.Addition).Directly();
await Expect.That(typeof(Album)).Has<SerializableAttribute>().Directly();
await Expect.That(typeof(Album)).ContainsMethods(methods => methods.WithName("Play")).Directly();

// The declared methods of the album types, also the inherited ones, and also those of System.Object
await Expect.That(In.AssemblyContaining<Album>().Types().Methods()).AreNotObsolete();
await Expect.That(In.AssemblyContaining<Album>().Types().Methods().IncludingInherited()).AreNotObsolete();
await Expect.That(In.AssemblyContaining<Album>().Types().Methods().IncludingObjectMembers()).AreNotObsolete();
```

The options apply to the whole result, e.g. to all attributes of `.With<T1>().OrWith<T2>().Directly()`. On a member
listing, set them directly after selecting the members, e.g. `Types().Methods().IncludingInherited().WhichArePublic()`.

<details>
<summary>Inherited members in detail</summary>

Inherited members never include those a derived type cannot access: the `private` members of base types and the
`internal` members of base types in another assembly (`InternalsVisibleTo` is not considered). Only the
[immutability](#types) check considers all fields and properties of base types, including those a derived type
cannot access, as they hold the state of an instance. An interface inherits the members of its base interfaces.

On a member check, `.Directly()` and `.IncludingObjectMembers()` exclude each other, so only one of them can be set.
On the `WhichContain…` filters, set them before or after the quantifier. Inside a member check,
`.Directly()` on an attribute filter only restricts the attributes: in
`ContainsMethods(m => m.With<FactAttribute>().Directly())` the method may still be inherited.

</details>

## String matching options

Every filter and expectation that compares a name, a namespace, a target framework or a dependency, including
the parameter and generic argument names, uses the string matching options of the core aweXpect library (see the
[string options](https://docs.testably.org/aweXpect/values/string#string-options), the
[match types](https://docs.testably.org/aweXpect/values/string#match-types) and the
[general option rules](https://docs.testably.org/aweXpect/how-it-works/options)):

| Option                                                           | Effect                                            |
|------------------------------------------------------------------|---------------------------------------------------|
| *(none)*                                                         | exact match (default)                             |
| `.AsPrefix()`                                                    | the value must start with the expected string     |
| `.AsSuffix()`                                                    | the value must end with the expected string       |
| `.AsWildcard()`                                                  | match using `*` and `?` wildcards                 |
| `.AsRegex()` / `.AsRegex(regexOptions)`                          | match using a regular expression                  |
| `.IgnoringCase()`                                                | case-insensitive comparison                       |
| `.IgnoringLeadingWhiteSpace()` / `.IgnoringTrailingWhiteSpace()` | trim before comparing                             |
| `.NormalizingWhiteSpace()`                                       | collapse runs of whitespace before comparing      |
| `.Using(comparer)`                                               | compare with a custom `IEqualityComparer<string>` |

```csharp
await Expect.That(types).HaveName("Service").AsSuffix();
await Expect.That(types).HaveName("*Test*").AsWildcard();
await Expect.That(types).HaveName(@"^Test\w+$").AsRegex();
await Expect.That(methods).HaveName("Get*Async").AsWildcard().IgnoringCase();

Filtered.Types services = In.AssemblyContaining<PlaylistService>().Types()
    .WithName("service").IgnoringCase().AsSuffix()
    .WhichAreClasses();
```

## Collections and quantifiers

Every expectation works with both a single item and a collection. A collection can be an array,
any `IEnumerable<T?>` or, on .NET 8 and later, an `IAsyncEnumerable<T?>`. The plural expectations already
require **every** item to match; for ad-hoc predicates use aweXpect's `Satisfies(…)` (single subject) and
`All()` / `Any()` quantifiers with `Satisfy(…)` (collections), and combine selections with LINQ:

```csharp
// The plural expectation already means "every item":
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

The plural expectations that only check a condition on each item, like `AreSealed()`, `AreNotStatic()` or
`HaveADefaultConstructor()`, read the whole collection and list every item that violates them. They also follow
aweXpect's quantifiers, which only read the items until the outcome is determined:

```csharp
await Expect.That(types).AtLeast(2).AreSealed();
await Expect.That(methods).AtMost(1).AreAsync();
```
