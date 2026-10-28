# Types and inheritance

Describes the filters and expectations for types and how inherited members, attributes and relations are
considered. [Filters and expectations](./index.md#how-to-read-the-tables) explains how to read the tables.

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
| only nullable members     | `.WhichOnlyHaveNullableMembers()`               | `.OnlyHasNullableMembers()`    | `.OnlyHaveNullableMembers()`    |                                                                                                       |
| only non-nullable members | `.WhichOnlyHaveNonNullableMembers()`            | `.OnlyHasNonNullableMembers()` | `.OnlyHaveNonNullableMembers()` |                                                                                                       |
| custom predicate          | `.Which(t => …)`                                | `.Satisfies(t => …)`           | `.All().Satisfy(t => …)`        | `Except(t => …)`, `Except<T>()`                                                                       |

`WhichInheritFrom` / `InheritsFrom` consider only the **base-class chain** (not implemented interfaces) and
accept a generic argument or a `Type`. Append `.Directly()` to require the base class to be the *direct* parent.
For interfaces, use `Implements`.

`WhichImplement` / `Implements` consider only implemented **interfaces**. With
`.Directly()`, an interface reached only through a base class or through another implemented interface does not
count as *directly* implemented. The negations accept `.Directly()` as well (e.g.
`WhichDoNotInheritFrom<T>().Directly()`, `DoesNotImplement<T>().Directly()`). [Inheritance](#inheritance) shows
samples.

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
[nullability](./05-properties-fields-and-events.md#nullability) of all fields, properties and events of the type, including the inherited ones unless
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
    .WhichContainConstructors(constructors => constructors.WithNoParameters())
```

By default a type matches when it contains **at least one** matching member. Append a quantifier (the same
quantifiers as in [aweXpect](https://docs.testably.org/aweXpect/collections/items#quantifiers)) to require a
specific count:

```csharp
In.AllLoadedAssemblies().Types()
    .WhichContainConstructors(c => c.WithNoParameters()).Exactly(1)
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
    .ContainsConstructors(constructors => constructors.WithNoParameters()).Exactly(1);

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

The `"name"` overloads accept the [string matching options](./01-common-filters.md#string-matching-options), and
`.AtIndex(n)` or `.AtIndexFromEnd(n)` is chained after a `WithArgument` to pin it to a specific position.

```csharp
// Generic types with a single argument constrained to IEntity
In.AllLoadedAssemblies().Types()
    .WhichAreGeneric().WithArgumentCount(1).WithArgument<IEntity>()

// Generic methods whose first argument is named "TKey"
In.AllLoadedAssemblies().Methods()
    .WhichAreGeneric().WithArgument("TKey").AtIndex(0)
```

## Inheritance

A check on a type considers what the type inherits: inherited attributes, operators, members and relations count by
default, and `.Directly()` restricts the check to what is declared on the item itself, or to a direct relation.
Listing the members of types (`Types().Methods()`, …) only includes the members declared on each type, as an
inherited member would otherwise be listed once for each type that inherits it; `.IncludingInherited()` adds the
inherited members.

The members declared on `System.Object`, `System.ValueType` and `System.Enum` are excluded, as every type would
otherwise have `ToString`, `Equals`, `GetHashCode`, …; `.IncludingObjectMembers()` adds them.

| Area                                                                                          | Default                                                                       | Options                                                                                                         |
|-----------------------------------------------------------------------------------------------|-------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------------------------|
| [Attributes](./01-common-filters.md#attributes) of types, methods, properties and events      | also the attributes inherited from a base type or an overridden member        | `.Directly()`: only the attributes declared on the item                                                         |
| [Operators](./04-methods.md#operators) and conversion operators of types                      | also the operators of base types                                              | `.Directly()`: only the operators declared on the type                                                          |
| Relations (`InheritsFrom<T>()`, `Implements<T>()`, their filters and negations)               | the base class anywhere in the base-class chain, or any implemented interface | `.Directly()`: only the direct base class or a directly implemented interface                                   |
| Member checks (`ContainsMethods(…)`, `WhichContainMethods(…)`, `OnlyHasNullableMembers()`, …) | also the inherited members                                                    | `.Directly()`: only the declared members, or `.IncludingObjectMembers()`: also the members of the three roots   |
| Member listings of types (`Types().Methods()`, `In.Type<T>().Properties()`, …)                | only the declared members                                                     | `.IncludingInherited()`: also the inherited members, `.IncludingObjectMembers()`: also those of the three roots |
| Member listings of assemblies (`In.AssemblyContaining<T>().Methods()`, …)                     | each member once, on the type that declares it                                |                                                                                                                 |
| Constructors, and the attributes of assemblies, constructors and fields                       | no inheritance                                                                |                                                                                                                 |

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
