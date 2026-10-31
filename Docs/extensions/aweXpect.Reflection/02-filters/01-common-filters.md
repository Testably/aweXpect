---
description: The filters and expectations that all types and members share, for access modifiers, attributes, the obsolete marker, names and namespaces.
---

# Common filters

Describes the filters and expectations that all types and members share: access modifiers, attributes, the
obsolete marker, names and namespaces, and the string matching options for names.
[Filters and expectations](./00-index.md#how-to-read-the-tables) explains how to read the tables.

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

await That(method).IsPublic();
await That(methods).ArePublic();
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
    await That(In.AssemblyContaining<PlaylistService>().Types().WhichAre(modifier))
        .AreNotNested();
}

[Test]
[Arguments(typeof(PlaylistService), AccessModifiers.Public)]
[Arguments(typeof(TrackCache), AccessModifiers.Internal)]
public async Task TypeHasTheExpectedAccessModifier(Type type, AccessModifiers modifier)
{
    await That(type).Is(modifier);
}

await That(methods).AreNot(AccessModifiers.Private | AccessModifiers.PrivateProtected);
```

## Attributes

Shared by all types, members, and assemblies.

|                                    | Filter                      | Expect (single)            | Expect (many)               | Negated                                                                   |
|------------------------------------|-----------------------------|----------------------------|-----------------------------|---------------------------------------------------------------------------|
| has attribute                      | `.With<TAttribute>()`       | `.Has<TAttribute>()`       | `.Have<TAttribute>()`       | `Without<TAttribute>`, `DoesNotHave<TAttribute>`, `DoNotHave<TAttribute>` |
| has attribute of a given type      |                             | `.Has(attributeType)`      | `.Have(attributeType)`      | `DoesNotHave(attributeType)`, `DoNotHave(attributeType)`                  |
| has attribute matching a predicate | `.With<TAttribute>(a => …)` | `.Has<TAttribute>(a => …)` | `.Have<TAttribute>(a => …)` |                                                                           |
| any of several attributes          | `.With<T1>().OrWith<T2>()`  | `.Has<T1>().OrHas<T2>()`   | `.Have<T1>().OrHave<T2>()`  |                                                                           |

For types, methods, properties and events, attributes inherited from a base type or an overridden member are
considered as well; append `.Directly()` to only consider the attributes declared on the item itself, e.g.
`.With<TAttribute>().Directly()` or `.Has<TAttribute>().Directly()` (see [Inheritance](./03-types.md#inheritance)). Chain multiple `.Without<TAttribute>()` calls to exclude several attributes (an item
must have none of them).

```csharp
await That(type).Has<ObsoleteAttribute>(a => a.Message == "Use NewClass instead");
await That(methods).Have<FactAttribute>();
```

### Obsolete

Shared by all types and members: a self-documenting shorthand for the `ObsoleteAttribute` (so that
`.WhichAreObsolete()` reads better than `.With<ObsoleteAttribute>()` in architecture rules).

|          | Filter                | Expect (single) | Expect (many)    | Negated                                                  |
|----------|-----------------------|-----------------|------------------|----------------------------------------------------------|
| obsolete | `.WhichAreObsolete()` | `.IsObsolete()` | `.AreObsolete()` | `WhichAreNotObsolete`, `IsNotObsolete`, `AreNotObsolete` |

```csharp
// Verify that nothing public is marked [Obsolete]
await That(In.AssemblyContaining<PlaylistService>().Types().WhichArePublic())
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
await That(In.AssemblyContaining<PlaylistService>().Types())
    .DoNotHaveName("Test").AsSuffix();
```

The name/namespace *equality* filters and expectations (`WithName`, `WithNamespace`, and their
`Has`/`Have` counterparts) accept the
[string matching options](#string-matching-options) (`AsPrefix`, `AsSuffix`, `AsWildcard`,
`AsRegex`, `IgnoringCase`, …). Collection expectations also accept a selector to derive the expected name per
item, also after a quantifier:

```csharp
await That(types).HaveName("Service").AsSuffix();
await That(methods).HaveName(m => "On" + m.GetParameters()[0].ParameterType.Name);
await That(methods).AtLeast(1).HaveName(m => "Get" + m.ReturnType.Name);
```

The `WhichAreWithinNamespace`/`IsWithinNamespace`/`AreWithinNamespace` variants match a namespace and all
its sub-namespaces (so `MusicStore.Data` includes `MusicStore.Data.Sql` but not `MusicStore.DataImport`). They
compare the namespace exactly and case-sensitively. Their negations match types outside the namespace. Unlike `DoesNotHaveNamespace`, which compares the whole namespace as a string with the
string matching options, `IsNotWithinNamespace` also excludes all sub-namespaces.

## String matching options

Every filter and expectation that compares a name, a namespace, a target framework or a dependency, including
the parameter and generic argument names, uses the string matching options of the core aweXpect library (see the
[string options](/aweXpect/values/string#string-options), the
[match types](/aweXpect/values/string#match-types) and the
[general option rules](/aweXpect/how-it-works/options)):

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
await That(types).HaveName("Service").AsSuffix();
await That(types).HaveName("*Test*").AsWildcard();
await That(types).HaveName(@"^Test\w+$").AsRegex();
await That(methods).HaveName("Get*Async").AsWildcard().IgnoringCase();

Filtered.Types services = In.AssemblyContaining<PlaylistService>().Types()
    .WithName("service").IgnoringCase().AsSuffix()
    .WhichAreClasses();
```
