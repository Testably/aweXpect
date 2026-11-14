---
description: How the filters and expectations of aweXpect.Reflection pair up, how to combine them and how to use custom predicates, custom filters and quantifiers.
---

# Filters and expectations

A **filter** (`.WhichAre…` / `.With…`) narrows a collection *before* you verify it. An **expectation**
(`That(…).Is… / .Are… / .Has… / .Have…`) states the rule the subject must satisfy. They mirror
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
| [Custom filters](#custom-filters)                                              | types, members and assemblies | `Which(IFilter<T>)`                                                                                                  |
| [Collections and quantifiers](#collections-and-quantifiers)                    | all subjects                  | `Satisfies`, `All().Satisfy`, `Any().Satisfy`                                                                        |

## Combining filters

Filters chain naturally (each narrows the previous result). Several filters offer an `Or…` companion to
widen a single step. The attribute expectations have one as well: `OrHas<TAttribute>()` on a single subject and
`OrHave<TAttribute>()` on a collection, both also with a predicate on the attribute:

```csharp
// Any of several attributes
In.AllLoadedAssemblies().Methods()
    .With<FactAttribute>().OrWith<TheoryAttribute>()
await That(method).Has<FactAttribute>().OrHas<TheoryAttribute>();
await That(methods).Have<FactAttribute>().OrHave<TheoryAttribute>(a => a.Skip == null);

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

## Custom filters

A filter you need in many places, or one for an extension package, can implement `IFilter<T>` from the namespace
`aweXpect.Reflection.Collections` and be passed to `.Which(IFilter<T>)` on any selection. Wrap it in an extension
method so that it reads like the built-in filters:

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Reflection.Collections;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionFilters
{
    public static Filtered.Types WhichAreRegisteredIn(this Filtered.Types types, IServiceCollection services)
        => types.Which(new RegisteredInFilter(services));

    private sealed class RegisteredInFilter(IServiceCollection services) : IFilter<Type>
    {
        private readonly HashSet<Type> _serviceTypes = new(services.Select(s => s.ServiceType));

        public ValueTask<bool> Applies(Type value, CancellationToken cancellationToken)
            => new(_serviceTypes.Contains(value));

        public string Describes(string text)
            => text + "which are registered in the service collection ";
    }
}

// All registered services of the album assembly are interfaces
await That(In.AssemblyContaining<Album>().Types().WhichAreRegisteredIn(services))
    .AreInterfaces();
```

`Describes` receives the description of the selection so far, e.g. `types `, and returns it with the filter added. The
failure message of the example then reads `types which are registered in the service collection in assembly …`. The
built-in filters follow one of two forms, and both end with a space that separates them from the next part:

- A **suffix** filter appends a clause: `text + "which are registered in the service collection "`, like
  `WhichAreClasses()` (`types which are classes `).
- A **prefix** filter puts an adjective in front: `"obsolete " + text`, like `WhichAreObsolete()`
  (`obsolete methods `).

`Applies` receives the cancellation token of the enumeration. A check that awaits something slow, e.g. a file or a
database, should pass the token on so that a cancelled or timed-out expectation stops early.

A filter that reflects over the members of its subject, e.g. with `GetMethods()` or `GetNestedTypes()`, can miss
members when the tests are published with trimming or Native AOT, as the trimmer removes the members nobody references
statically. Guard such a check with `ReflectionFallback.IsSupported` from the namespace `aweXpect.Core.Extending`, and
throw the exception of `ReflectionFallback.NotSupported(…)` otherwise, so that the filter fails with a clear message
instead of silently matching too few items:

```csharp
public ValueTask<bool> Applies(Type value, CancellationToken cancellationToken)
{
    if (!ReflectionFallback.IsSupported)
    {
        throw ReflectionFallback.NotSupported($"The methods of {value.Name}");
    }

    return new(value.GetMethods().Any(method => method.ReturnType == typeof(Task)));
}
```

## Collections and quantifiers

Every expectation works with both a single item and a collection. A collection can be an array,
any `IEnumerable<T?>` or, on .NET 8 and later, an `IAsyncEnumerable<T?>`. The plural expectations already
require **every** item to match; for ad-hoc predicates use aweXpect's `Satisfies(…)` (single subject) and
`All()` / `Any()` quantifiers with `Satisfy(…)` (collections), and combine selections with LINQ:

```csharp
// The plural expectation already means "every item":
await That(types).ArePublic();

// Ad-hoc predicate on a single subject:
await That(type).Satisfies(type => type.IsSealed);

// Ad-hoc predicate across the whole collection:
await That(types).All().Satisfy(type => type.IsSealed);
await That(types).Any().Satisfy(type => type.IsAbstract);

// Mix with LINQ (assign to IEnumerable<Type?> so Where binds to LINQ):
IEnumerable<Type?> publicClasses = In.AllLoadedAssemblies().Types()
    .WhichAreClasses().WhichArePublic();
IEnumerable<Type?> managers = publicClasses.Where(type => type!.GetInterfaces().Length > 2);
await That(managers).HaveName("Manager").AsSuffix();
```

The plural expectations that only check a condition on each item, like `AreSealed()`, `AreNotStatic()`,
`HaveADefaultConstructor()` or `HaveNamespace(…)`, read the whole collection and list every item that violates them.
They also follow aweXpect's quantifiers, which only read the items until the outcome is determined, and keep their
string options:

```csharp
await That(types).AtLeast(2).AreSealed();
await That(methods).AtMost(1).AreAsync();
await That(types).AtLeast(2).HaveNamespace("MusicStore").AsPrefix();
```

The same holds for the relations, operators, generic arguments and members of types: `InheritFrom`, `Implement`,
`AreGeneric`, `HaveOperator`, `HaveImplicitConversionOperator`, `HaveExplicitConversionOperator`,
`OnlyHaveNullableMembers`, `OnlyHaveNonNullableMembers` and the `Contain…` checks. They keep their options, like
`Directly()`, and their negations mean that no item matches (`none inherit from MediaItem`):

```csharp
await That(types).AtLeast(2).InheritFrom<MediaItem>().Directly();
await That(types).None().Implement<IDisposable>();
await That(types).AtMost(1).ContainMethods(methods => methods.With<ObsoleteAttribute>());
```

The parameter and return type expectations of methods and constructors take quantifiers as well: `HaveParameter`,
`HaveParameterExactly` and their `ref` / `out` / `in` / `params` / optional variants, `HaveParameterCount`,
`HaveNoParameters`, `Return`, `ReturnExactly`, `ReturnVoid` and `AreGeneric` on methods. Their options, like
`AtIndex(…)` or `OrReturn<T>()`, apply to each item, and their negations, like `DoNotHaveParameter<bool>()`, read
`none have parameter of type bool`:

```csharp
await That(methods).AtLeast(2).HaveParameter<CancellationToken>().AtIndexFromEnd(0);
await That(methods).None().Return<Task>().OrReturn<ValueTask>();
await That(constructors).AtMost(1).HaveNoParameters();
```

The attribute expectations `Have<TAttribute>()` on types, members and assemblies and the type expectations
`AreOfType<T>()` and `AreOfExactType<T>()` on properties, fields and events take quantifiers as well and keep
`OrHave…`, `OrOfType…` and `Directly()`. Their negations `DoNotHave<TAttribute>()`, `AreNotOfType<T>()` and
`AreNotOfExactType<T>()` mean that no item matches (`none have ObsoleteAttribute`):

```csharp
await That(methods).AtLeast(1).Have<FactAttribute>().OrHave<TheoryAttribute>();
await That(properties).None().AreOfType<DateTime>();
```

On a struct collection, such as an `ImmutableArray<Type>`, pass the type as an argument after a quantifier, e.g.
`AtLeast(2).InheritFrom(typeof(MediaItem))`, as C# cannot infer the collection type next to an explicit type
argument.
