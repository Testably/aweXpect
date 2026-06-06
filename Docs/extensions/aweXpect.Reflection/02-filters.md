# Filters and assertions

A **filter** (`.WhichAre…` / `.With…`) narrows a collection *before* you assert on it. An **assertion**
(`Expect.That(…).Is… / .Are… / .Has… / .Have…`) states the rule the subject must satisfy. They mirror
each other: a filter has an `Is…`/`Are…` assertion counterpart for the same concept.

The tables below pair them up. The **Filter** column is used inside `In.…`; the **Assert (single)**
column applies to one subject; the **Assert (many)** column applies to a collection.

The custom `.Which(…)` filter has a universal assertion counterpart that works for **every** subject kind
(types, members and assemblies): use aweXpect's `.Satisfies(…)` on a single subject and `.All().Satisfy(…)`
(or `.Any().Satisfy(…)`) on a collection. See
[Collections and quantifiers](#collections-and-quantifiers).

The custom `.Except(…)` filter is the inverse of `.Which(…)`: it **removes** the items that match the
predicate. This is handy for defining exemptions to a rule (e.g. *all async methods except this one*). Like
`.Which(…)` it is available on **every** collection (assemblies, types and members). For types there is also a
typed `.Except<T>()` overload that excludes exactly the type `T`.

## Access modifiers

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

## Attributes

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

## Obsolete

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

## Names and namespaces

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
They accept the same [string matching options](#string-matching-options) as their positive
counterparts:

```csharp
// Verify that no type in the production assembly is named with a "Test" suffix
await Expect.That(In.AssemblyContaining<MyClass>().Types())
    .DoNotHaveName("Test").AsSuffix();
```

The name/namespace *equality* filters and assertions (`WithName`, `WithNamespace`, and their
`Has`/`Have` counterparts) accept the
[string matching options](#string-matching-options) (`AsPrefix`, `AsSuffix`, `AsWildcard`,
`AsRegex`, `IgnoringCase`, …). Collection assertions also accept a selector to derive the expected name per
item:

```csharp
await Expect.That(types).HaveName("Service").AsSuffix();
await Expect.That(methods).HaveName(m => "On" + m.GetParameters()[0].ParameterType.Name);
```

The `WithinNamespace`/`IsWithinNamespace`/`AreWithinNamespace` variants match a namespace and all its
sub-namespaces (so `Foo.Bar` includes `Foo.Bar.Baz` but not `Foo.BarBaz`). They compare the namespace
exactly and case-sensitively and do not support any of the string matching options. Each has a negated
form (`NotWithinNamespace`, `IsNotWithinNamespace` and `AreNotWithinNamespace`) that matches types
outside the namespace.

## Types

| Kind                      | Filter                                          | Assert (single)                | Assert (many)                   |
|---------------------------|-------------------------------------------------|--------------------------------|---------------------------------|
| class                     | `.WhichAreClasses()` / `.Classes()`             | `.IsAClass()`                  | `.AreClasses()`                 |
| interface                 | `.WhichAreInterfaces()` / `.Interfaces()`       | `.IsAnInterface()`             | `.AreInterfaces()`              |
| enum                      | `.WhichAreEnums()` / `.Enums()`                 | `.IsAnEnum()`                  | `.AreEnums()`                   |
| struct                    | `.WhichAreStructs()` / `.Structs()`             | `.IsAStruct()`                 | `.AreStructs()`                 |
| record                    | `.WhichAreRecords()` / `.Records()`             | `.IsARecord()`                 | `.AreRecords()`                 |
| record struct             | `.WhichAreRecordStructs()` / `.RecordStructs()` | `.IsARecordStruct()`           | `.AreRecordStructs()`           |
| readonly struct           | `.WhichAreReadOnly()`                           | `.IsReadOnly()`                | `.AreReadOnly()`                |
| ref struct                | `.WhichAreRefStructs()`                         | `.IsARefStruct()`              | `.AreRefStructs()`              |
| delegate                  | `.WhichAreDelegates()`                          | `.IsADelegate()`               | `.AreDelegates()`               |
| exception                 | `.WhichAreExceptions()`                         | `.IsAnException()`             | `.AreExceptions()`              |
| attribute                 | `.WhichAreAttributes()`                         | `.IsAnAttribute()`             | `.AreAttributes()`              |
| abstract                  | `.WhichAreAbstract()` / `.Abstract`             | `.IsAbstract()`                | `.AreAbstract()`                |
| sealed                    | `.WhichAreSealed()` / `.Sealed`                 | `.IsSealed()`                  | `.AreSealed()`                  |
| static                    | `.WhichAreStatic()` / `.Static`                 | `.IsStatic()`                  | `.AreStatic()`                  |
| generic                   | `.WhichAreGeneric()` / `.Generic`               | `.IsGeneric()`                 | `.AreGeneric()`                 |
| nested                    | `.WhichAreNested()` / `.Nested`                 | `.IsNested()`                  | `.AreNested()`                  |
| inherits from             | `.WhichInheritFrom<T>()`                        | `.InheritsFrom<T>()`           | `.InheritFrom<T>()`             |
| implements                | `.WhichImplement<T>()`                          | `.Implements<T>()`             | `.Implement<T>()`               |
| assignable to             | `.WhichAreAssignableTo<T>()`                    | `.IsAssignableTo<T>()`         | `.AreAssignableTo<T>()`         |
| assignable from           | `.WhichAreAssignableFrom<T>()`                  | `.IsAssignableFrom<T>()`       | `.AreAssignableFrom<T>()`       |
| instantiable              | `.WhichAreInstantiable()`                       | `.IsInstantiable()`            | `.AreInstantiable()`            |
| immutable                 | `.WhichAreImmutable()`                          | `.IsImmutable()`               | `.AreImmutable()`               |
| default constructor       | `.WhichHaveADefaultConstructor()`               | `.HasADefaultConstructor()`    | `.HaveADefaultConstructor()`    |
| only nullable members     | `.WhichOnlyHaveNullableMembers()`               | `.OnlyHasNullableMembers()`    | `.OnlyHaveNullableMembers()`    |
| only non-nullable members | `.WhichOnlyHaveNonNullableMembers()`            | `.OnlyHasNonNullableMembers()` | `.OnlyHaveNonNullableMembers()` |
| custom predicate          | `.Which(t => …)`                                | `.Satisfies(t => …)`           | `.All().Satisfy(t => …)`        |

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

A type is *immutable* when all instance fields (including inherited ones) are `readonly` and all instance
properties (including inherited ones) have no setter or an `init`-only setter. Static members do not affect
immutability. Failure messages list the offending mutable members for actionable feedback.

`OnlyHasNullableMembers` / `OnlyHaveNullableMembers` (and the non-nullable counterparts) verify the
[nullability](#nullability) of all declared fields and properties of the type; the failure message lists the
non-compliant members per type:

```csharp
await Expect.That(In.AssemblyContaining<MyRequest>()
        .Types().WithName("Request").AsSuffix())
    .OnlyHaveNullableMembers();
```

:::note[Negation]
Every kind/modifier row above has a negated form. Most use `WhichAreNot…` on filters and
`IsNot…` / `AreNot…` on assertions (e.g. `WhichAreNotSealed()`, `IsNotAClass()`, `AreNotStatic()`,
`IsNotInstantiable()`). The *default constructor* row uses `WhichDoNotHaveADefaultConstructor()`,
`DoesNotHaveADefaultConstructor()` and `DoNotHaveADefaultConstructor()`.
:::

### Types containing specific members

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

### Generic type arguments

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

## Methods

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

## Operators

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

Operators are special-name members that are
[hidden by default](./04-configuration.md#compiler-generated-members), so the
plain `.Methods()` collection excludes them unless opted in via `IncludedSpecialNameMembers`. The
`.WhichAreOperators(Operator)` filter implicitly re-includes operators for its query, so it works without
that configuration. The negative `.WhichAreNotOperators(Operator)` filter deliberately does **not** re-include
operators: a "not this operator" filter over `.Methods()` is meant to narrow regular methods, and force-including
every *other* operator would surprise more than help. If you want the other operators in that result, opt in via
`IncludedSpecialNameMembers`.

:::note[Negation]
`IsNotAnOperator(Operator)`, `DoesNotHaveOperator(Operator)` / `DoNotHaveOperator(Operator)`
(including the operand overloads, e.g. `DoesNotHaveOperator<int>(Operator)`),
`DoesNotHave…ConversionOperator…` / `DoNotHave…ConversionOperator…` and `WhichAreNotOperators(Operator)`.
:::

## Properties & Fields

In addition to [access modifiers](#access-modifiers),
[attributes](#attributes) and
[names](#names-and-namespaces):

|                                        | Filter                                      | Assert (single)                 | Assert (many)                     |
|----------------------------------------|---------------------------------------------|---------------------------------|-----------------------------------|
| of type (or a subtype)                 | `.OfType<T>()`                              | `.IsOfType<T>()`                | `.AreOfType<T>()`                 |
| of exact type                          | `.OfExactType<T>()`                         | `.IsOfExactType<T>()`           | `.AreOfExactType<T>()`            |
| static *(properties & fields)*         | `.WhichAreStatic()`                         | `.IsStatic()`                   | `.AreStatic()`                    |
| nullable *(properties & fields)*       | `.WhichAreNullable()`                       | `.IsNullable()`                 | `.AreNullable()`                  |
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

:::note[Negation]
The `static`, `nullable`, `abstract`, `sealed`, `virtual`, `required`, `indexer`,
`extension property`, `read-only` *(fields)* and `constant` rows have a negated form: `WhichAreNot…` on filters
and `IsNot…` / `AreNot…` on assertions (e.g. `WhichAreNotConstant()`, `IsNotConstant()`, `AreNotConstant()`);
`override` uses `WhichDoNotOverride()` / `DoesNotOverride()` / `DoNotOverride()`.
:::

### Nullability

A property or field counts as *nullable* when its type is a `Nullable<T>` value type (e.g. `int?`) or a
reference type annotated as nullable (e.g. `string?`, based on the nullable reference type metadata emitted
by the compiler). The check follows the declared annotation on every target framework: reference types
without nullability annotations (oblivious code compiled without `<Nullable>enable</Nullable>`) and
unconstrained generic type parameters (`T`, as opposed to `T?`) count as non-nullable, and post-condition
attributes like `[AllowNull]` or `[MaybeNull]` are ignored.

```csharp
// All properties and fields of the request types must be nullable
await Expect.That(In.AssemblyContaining<MyRequest>()
        .Types().WithName("Request").AsSuffix()
        .Properties())
    .AreNullable();
```

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

## Events

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

:::note[Negation]
The `abstract`, `sealed` and `static` rows have a negated form: `WhichAreNot…` on filters and
`IsNot…` / `AreNot…` on assertions (e.g. `WhichAreNotSealed()`, `IsNotSealed()`, `AreNotSealed()`).
:::

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

## Assemblies

Assemblies are usually used as a [source](./01-sources.md#sources-the-in-helper), but you can also filter
and assert on them directly:

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
