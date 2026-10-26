# Methods, operators and constructors

Describes the filters and expectations for methods, operators and constructors.
[Filters and expectations](./index.md#how-to-read-the-tables) explains how to read the tables.

## Methods

In addition to [access modifiers](./01-common-filters.md#access-modifiers),
[attributes](./01-common-filters.md#attributes) and
[names](./01-common-filters.md#names-and-namespaces):

|                                                                              | Filter                                              | Expect (single)                                                 | Expect (many)                          | Negated                                                                           |
|------------------------------------------------------------------------------|-----------------------------------------------------|-----------------------------------------------------------------|----------------------------------------|-----------------------------------------------------------------------------------|
| static / abstract / sealed / [generic](./03-types.md#generic-type-arguments) | `.WhichAreStatic()` …                               | `.IsStatic()` …                                                 | `.AreStatic()` …                       | `WhichAreNotStatic`, `IsNotStatic`, `AreNotStatic` …                              |
| async                                                                        | `.WhichAreAsync()`                                  | `.IsAsync()`                                                    | `.AreAsync()`                          | `WhichAreNotAsync`, `IsNotAsync`, `AreNotAsync`                                   |
| extension method                                                             | `.WhichAreExtensionMethods()`                       | `.IsAnExtensionMethod()`                                        | `.AreExtensionMethods()`               | `WhichAreNotExtensionMethods`, `IsNotAnExtensionMethod`, `AreNotExtensionMethods` |
| operator                                                                     | `.WhichAreOperators()`                              | `.IsAnOperator()`                                               | `.AreOperators()`                      | `WhichAreNotOperators`, `IsNotAnOperator`, `AreNotOperators`                      |
| virtual                                                                      | `.WhichAreVirtual()`                                | `.IsVirtual()`                                                  | `.AreVirtual()`                        | `WhichAreNotVirtual`, `IsNotVirtual`, `AreNotVirtual`                             |
| overrides a base method                                                      | `.WhichOverride()`                                  | `.Overrides()`                                                  | `.Override()`                          | `WhichDoNotOverride`, `DoesNotOverride`, `DoNotOverride`                          |
| returns type (or a subtype)                                                  | `.WhichReturn<T>()`                                 | `.Returns<T>()`                                                 | `.Return<T>()`                         | `WhichDoNotReturn<T>`, `DoesNotReturn<T>`, `DoNotReturn<T>`                       |
| returns exactly                                                              | `.WhichReturnExactly<T>()`                          | `.ReturnsExactly<T>()`                                          | `.ReturnExactly<T>()`                  | `WhichDoNotReturnExactly<T>`, `DoesNotReturnExactly<T>`, `DoNotReturnExactly<T>`  |
| returns void                                                                 | `.WhichReturnVoid()`                                | `.ReturnsVoid()`                                                | `.ReturnVoid()`                        | `WhichDoNotReturnVoid`, `DoesNotReturnVoid`, `DoNotReturnVoid`                    |
| no parameters                                                                | `.WithoutParameters()`                              | `.HasNoParameters()`                                            | `.HaveNoParameters()`                  |                                                                                   |
| parameter of type (or subtype)                                               | `.WithParameter<T>()` / `.WithParameter<T>("name")` | `.HasParameter<T>()` / `.HasParameter<T>("name")`               | `.HaveParameter<T>()`                  | `WithoutParameter<T>`, `DoesNotHaveParameter<T>`, `DoNotHaveParameter<T>`         |
| parameter of exact type                                                      | `.WithParameterExactly<T>()`                        | `.HasParameterExactly<T>()` / `.HasParameterExactly<T>("name")` | `.HaveParameterExactly<T>()`           |                                                                                   |
| parameter count                                                              | `.WithParameterCount(n)`                            | `.HasParameterCount(n)`                                         | `.HaveParameterCount(n)`               |                                                                                   |
| `ref` parameter                                                              | `.WithRefParameter()`                               | `.HasRefParameter()`                                            | `.HaveRefParameter()`                  |                                                                                   |
| `out` parameter                                                              | `.WithOutParameter()`                               | `.HasOutParameter()`                                            | `.HaveOutParameter()`                  |                                                                                   |
| `in` parameter                                                               | `.WithInParameter()`                                | `.HasInParameter()`                                             | `.HaveInParameter()`                   |                                                                                   |
| `params` parameter                                                           | `.WithParamsParameter()`                            | `.HasParamsParameter()`                                         | `.HaveParamsParameter()`               |                                                                                   |
| optional parameter                                                           | `.WithOptionalParameter()`                          | `.HasOptionalParameter()`                                       | `.HaveOptionalParameter()`             |                                                                                   |
| parameter matching a predicate                                               | `.WithParameter<T>().Matching(p => …)`              | `.HasParameter<T>().Matching(p => …)`                           | `.HaveParameter<T>().Matching(p => …)` | `WithoutParameter<T>().Matching(p => …)`, …                                       |
| custom predicate                                                             | `.Which(m => …)`                                    | `.Satisfies(m => …)`                                            | `.All().Satisfy(m => …)`               | `Except(m => …)`                                                                  |

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

The parameter filters and expectations (`WithParameter`, `HasParameter`, `HaveParameter` and their variants) take these
options to narrow down the parameter; they combine, and all of them must hold for the same parameter:

| Option                                     | Effect                                                                     |
|--------------------------------------------|----------------------------------------------------------------------------|
| `.AtIndex(n)`                              | the parameter at the zero-based index `n`                                  |
| `.AtIndexFromEnd(n)`                       | the parameter at the zero-based index `n`, counted from the last parameter |
| `.WithDefaultValue()`                      | a parameter with a default value                                           |
| `.WithDefaultValue(value)`                 | a parameter with the default value `value`                                 |
| `.WithoutDefaultValue()`                   | a parameter without a default value                                        |
| `.WithRefModifier()` *(filters only)*      | a `ref` parameter                                                          |
| `.WithOutModifier()` *(filters only)*      | an `out` parameter                                                         |
| `.WithInModifier()` *(filters only)*       | an `in` parameter                                                          |
| `.WithOptionalModifier()` *(filters only)* | an optional parameter                                                      |
| `.WithParamsModifier()` *(filters only)*   | a `params` parameter                                                       |
| `.Matching(p => …)`                        | a parameter matching the predicate on its `ParameterInfo`                  |

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

| Scope               | Filter                                                     | Expect (single)                                      | Expect (many)                                         | Negated                                                                                                                                                                          |
|---------------------|------------------------------------------------------------|------------------------------------------------------|-------------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| any operator method | `.WhichAreOperators()`                                     | `.IsAnOperator()`                                    | `.AreOperators()`                                     | `WhichAreNotOperators`, `IsNotAnOperator`, `AreNotOperators`                                                                                                                     |
| specific operator   | `.WhichAreOperators(Operator)`                             | `.IsAnOperator(Operator)`                            | `.AreOperators(Operator)`                             | `WhichAreNotOperators(Operator)`, `IsNotAnOperator(Operator)`, `AreNotOperators(Operator)`                                                                                       |
| type has operator   | `.WhichHaveOperator(Operator)`                             | `.HasOperator(Operator)`                             | `.HaveOperator(Operator)`                             | `WhichDoNotHaveOperator(Operator)`, `DoesNotHaveOperator(Operator)`, `DoNotHaveOperator(Operator)`                                                                               |
| implicit conversion | `.WhichHaveImplicitConversionOperator<TSource, TTarget>()` | `.HasImplicitConversionOperator<TSource, TTarget>()` | `.HaveImplicitConversionOperator<TSource, TTarget>()` | `WhichDoNotHaveImplicitConversionOperator<TSource, TTarget>`, `DoesNotHaveImplicitConversionOperator<TSource, TTarget>`, `DoNotHaveImplicitConversionOperator<TSource, TTarget>` |
| explicit conversion | `.WhichHaveExplicitConversionOperator<TSource, TTarget>()` | `.HasExplicitConversionOperator<TSource, TTarget>()` | `.HaveExplicitConversionOperator<TSource, TTarget>()` | `WhichDoNotHaveExplicitConversionOperator<TSource, TTarget>`, `DoesNotHaveExplicitConversionOperator<TSource, TTarget>`, `DoNotHaveExplicitConversionOperator<TSource, TTarget>` |

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

// Filter for the types that declare an operator themselves
In.AssemblyContaining<TrackDuration>().Types().WhichHaveOperator(Operator.Addition).Directly()
```

`WhichHaveOperator` / `HasOperator` and the conversion operator filters and expectations also match the operators
inherited from base types; append `.Directly()` to only match the operators declared on the type itself. Conversion
source/target types are matched exactly. The filters, the expectations and their negations also come with the operand
overloads, e.g. `WhichHaveOperator<int>(Operator)` or `DoesNotHaveOperator<int>(Operator)`.

:::note[Operators are hidden by default]
Operators are special-name members that are
[hidden by default](../04-customization.md#compiler-generated-members), so the
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

## Constructors

In addition to [access modifiers](./01-common-filters.md#access-modifiers) and
[attributes](./01-common-filters.md#attributes):

|                                | Filter                                              | Expect (single)                                                 | Expect (many)                          | Negated                                                                   |
|--------------------------------|-----------------------------------------------------|-----------------------------------------------------------------|----------------------------------------|---------------------------------------------------------------------------|
| static                         | `.WhichAreStatic()`                                 | `.IsStatic()`                                                   | `.AreStatic()`                         | `WhichAreNotStatic`, `IsNotStatic`, `AreNotStatic`                        |
| no parameters                  | `.WithoutParameters()`                              | `.HasNoParameters()`                                            | `.HaveNoParameters()`                  |                                                                           |
| parameter of type (or subtype) | `.WithParameter<T>()` / `.WithParameter<T>("name")` | `.HasParameter<T>()` / `.HasParameter<T>("name")`               | `.HaveParameter<T>()`                  | `WithoutParameter<T>`, `DoesNotHaveParameter<T>`, `DoNotHaveParameter<T>` |
| parameter of exact type        | `.WithParameterExactly<T>()`                        | `.HasParameterExactly<T>()` / `.HasParameterExactly<T>("name")` | `.HaveParameterExactly<T>()`           |                                                                           |
| parameter count                | `.WithParameterCount(n)`                            | `.HasParameterCount(n)`                                         | `.HaveParameterCount(n)`               |                                                                           |
| `ref` parameter                | `.WithRefParameter()`                               | `.HasRefParameter()`                                            | `.HaveRefParameter()`                  |                                                                           |
| `out` parameter                | `.WithOutParameter()`                               | `.HasOutParameter()`                                            | `.HaveOutParameter()`                  |                                                                           |
| `in` parameter                 | `.WithInParameter()`                                | `.HasInParameter()`                                             | `.HaveInParameter()`                   |                                                                           |
| `params` parameter             | `.WithParamsParameter()`                            | `.HasParamsParameter()`                                         | `.HaveParamsParameter()`               |                                                                           |
| optional parameter             | `.WithOptionalParameter()`                          | `.HasOptionalParameter()`                                       | `.HaveOptionalParameter()`             |                                                                           |
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
