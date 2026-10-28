# Filters and expectations

A **filter** (`.WhichAre…` / `.With…`) narrows a collection *before* you verify it. An **expectation**
(`Expect.That(…).Is… / .Are… / .Has… / .Have…`) states the rule the subject must satisfy. They mirror
each other: a filter has an `Is…`/`Are…` expectation counterpart for the same concept.

## How to read the tables

The tables on the pages of this group pair them up. The **Filter** column is used on a selection (`In.…` or
`Types.…`); the **Expect (single)** column applies to one subject; the **Expect (many)** column applies to a
collection. The **Negated** column lists the negations in the same order (filter, single, many), leaving out a form
that has none. An empty cell means that there is no such form, e.g. no negation.

## Overview

| Section                                                                        | Applies to                    | Filters / expectations                                                                                               |
|--------------------------------------------------------------------------------|-------------------------------|----------------------------------------------------------------------------------------------------------------------|
| [Access modifiers](./01-common-filters.md#access-modifiers)                    | types and members             | `WhichArePublic` / `IsPublic`, `WhichAre(AccessModifiers)` / `Is(AccessModifiers)`, …                                |
| [Attributes](./01-common-filters.md#attributes)                                | types, members and assemblies | `With<TAttribute>` / `Has<TAttribute>`, `Without<TAttribute>` / `DoesNotHave<TAttribute>`                            |
| [Obsolete](./01-common-filters.md#obsolete)                                    | types and members             | `WhichAreObsolete` / `IsObsolete`, `WhichAreNotObsolete` / `IsNotObsolete`                                           |
| [Names and namespaces](./01-common-filters.md#names-and-namespaces)            | types and members             | `WithName` / `HasName`, `WithNamespace` / `HasNamespace`, `WhichAreWithinNamespace` / `IsWithinNamespace`            |
| [String matching options](./01-common-filters.md#string-matching-options)      | names and namespaces          | `AsPrefix`, `AsSuffix`, `AsWildcard`, `AsRegex`, `IgnoringCase`                                                      |
| [Assemblies](./02-assemblies.md)                                               | assemblies                    | `WhichTarget` / `Targets`, `WithVersion` / `HasVersion`, `WhichDependOn` / `DependsOn`                               |
| [Types](./03-types.md#types)                                                   | types                         | `WhichAreClasses` / `IsAClass`, `WhichInheritFrom<T>` / `InheritsFrom<T>`, `WhichContainMethods` / `ContainsMethods` |
| [Inheritance](./03-types.md#inheritance)                                       | types and members             | `Directly`, `IncludingInherited`, `IncludingObjectMembers`                                                           |
| [Methods](./04-methods.md#methods)                                             | methods                       | `WhichAreAsync` / `IsAsync`, `WhichReturn<T>` / `Returns<T>`, `WithParameter<T>` / `HasParameter<T>`                 |
| [Operators](./04-methods.md#operators)                                         | methods and types             | `WhichAreOperators` / `IsAnOperator`, `WhichHaveOperator` / `HasOperator`, `WhichHaveImplicitConversionOperator`     |
| [Constructors](./04-methods.md#constructors)                                   | constructors                  | `WithNoParameters` / `HasNoParameters`, `WithParameter<T>` / `HasParameter<T>`                                       |
| [Properties & Fields](./05-properties-fields-and-events.md#properties--fields) | properties and fields         | `WhichAreOfType<T>` / `IsOfType<T>`, `WhichAreNullable` / `IsNullable`, `WhichAreRequired` / `IsRequired`            |
| [Events](./05-properties-fields-and-events.md#events)                          | events                        | `WhichAreOfType<T>` / `IsOfType<T>`, `WhichAreStatic` / `IsStatic`, `WhichAreNullable` / `IsNullable`                |
| [Combining filters](#combining-filters)                                        | filters and expectations      | `OrWith<TAttribute>` / `OrHas<TAttribute>`, `OrReturn<T>`, `OrOfType<T>`                                             |
| [Custom predicates](#custom-predicates)                                        | types, members and assemblies | `Which` / `Satisfies`, `Except`                                                                                      |
| [Collections and quantifiers](#collections-and-quantifiers)                    | all subjects                  | `Satisfies`, `All().Satisfy`, `Any().Satisfy`                                                                        |

## Combining filters

Filters chain naturally (each narrows the previous result). Several filters offer an `Or…` companion to
widen a single step. The attribute expectations have one as well: `OrHas<TAttribute>()` on a single subject and
`OrHave<TAttribute>()` on a collection, both also with a predicate on the attribute:

```csharp
// Any of several attributes
In.AllLoadedAssemblies().Methods()
    .With<FactAttribute>().OrWith<TheoryAttribute>()
await Expect.That(method).Has<FactAttribute>().OrHas<TheoryAttribute>();
await Expect.That(methods).Have<FactAttribute>().OrHave<TheoryAttribute>(a => a.Skip == null);

// Any of several return types
In.AllLoadedAssemblies().Methods()
    .WhichReturn<Task>().OrReturn<ValueTask>()

// Any of several property/field types
In.AllLoadedAssemblies().Properties()
    .WhichAreOfType<string>().OrOfType<Guid>()
```

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

The plural expectations that only check a condition on each item, like `AreSealed()`, `AreNotStatic()`,
`HaveADefaultConstructor()` or `HaveNamespace(…)`, read the whole collection and list every item that violates them.
They also follow aweXpect's quantifiers, which only read the items until the outcome is determined, and keep their
string options:

```csharp
await Expect.That(types).AtLeast(2).AreSealed();
await Expect.That(methods).AtMost(1).AreAsync();
await Expect.That(types).AtLeast(2).HaveNamespace("MusicStore").AsPrefix();
```

The same holds for the relations, operators, generic arguments and members of types: `InheritFrom`, `Implement`,
`AreGeneric`, `HaveOperator`, `HaveImplicitConversionOperator`, `HaveExplicitConversionOperator`,
`OnlyHaveNullableMembers`, `OnlyHaveNonNullableMembers` and the `Contain…` checks. They keep their options, like
`Directly()`, and their negations mean that no item matches (`none inherit from MediaItem`):

```csharp
await Expect.That(types).AtLeast(2).InheritFrom<MediaItem>().Directly();
await Expect.That(types).None().Implement<IDisposable>();
await Expect.That(types).AtMost(1).ContainMethods(methods => methods.With<ObsoleteAttribute>());
```

On a struct collection, such as an `ImmutableArray<Type>`, pass the type as an argument after a quantifier, e.g.
`AtLeast(2).InheritFrom(typeof(MediaItem))`, as C# cannot infer the collection type next to an explicit type
argument.
