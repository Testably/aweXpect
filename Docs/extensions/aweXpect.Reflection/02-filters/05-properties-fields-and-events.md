# Properties, fields and events

Describes the filters and expectations for properties, fields and events.
[Filters and expectations](./index.md#how-to-read-the-tables) explains how to read the tables.

## Properties & Fields

In addition to [access modifiers](./01-common-filters.md#access-modifiers),
[attributes](./01-common-filters.md#attributes) and
[names](./01-common-filters.md#names-and-namespaces):

|                                        | Filter                                      | Expect (single)                 | Expect (many)                     | Negated                                                                                                       |
|----------------------------------------|---------------------------------------------|---------------------------------|-----------------------------------|---------------------------------------------------------------------------------------------------------------|
| of type (or a subtype)                 | `.WhichAreOfType<T>()`                      | `.IsOfType<T>()`                | `.AreOfType<T>()`                 | `WhichAreNotOfType<T>`, `IsNotOfType<T>`, `AreNotOfType<T>`                                                   |
| of exact type                          | `.WhichAreOfExactType<T>()`                 | `.IsOfExactType<T>()`           | `.AreOfExactType<T>()`            | `WhichAreNotOfExactType<T>`, `IsNotOfExactType<T>`, `AreNotOfExactType<T>`                                    |
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
| read-write *(properties only)*         | `.WhichAreReadWrite()`                      | `.IsReadWrite()`                | `.AreReadWrite()`                 |                                                                                                               |
| has getter *(properties only)*         | `.WhichHaveAGetter()`                       | `.HasAGetter()`                 | `.HaveAGetter()`                  |                                                                                                               |
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

`WhichAreExtensionProperties()`, `IsAnExtensionProperty()` and `AreExtensionProperties()` match extension properties
declared with the C# extension block syntax (`extension(...) { … }`), both instance and static.

Use `OrOfType<T>()` / `OrOfExactType<T>()` to allow several types. The negations `WhichAreNotOfType` /
`IsNotOfType` / `AreNotOfType` (and the `…ExactType` forms) have no `Or…` companion: chain
`.WhichAreNotOfType<T1>().WhichAreNotOfType<T2>()` (or combine expectations with `.And`) to exclude several types.
On a collection, `AreNotOfType<T>()` verifies that *none* of the items is of the type.

```csharp
In.AllLoadedAssemblies().Public.Properties()
    .WhichAreOfType<string>()
    .WithName("Id").AsSuffix()
    .With<RequiredAttribute>()

In.AllLoadedAssemblies().Private.Fields()
    .WhichAreOfType<ILogger>()
    .WithName("_").AsPrefix()
```

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

In addition to [access modifiers](./01-common-filters.md#access-modifiers),
[attributes](./01-common-filters.md#attributes) and
[names](./01-common-filters.md#names-and-namespaces):

|                                | Filter                                      | Expect (single)                 | Expect (many)                     | Negated                                                                                                       |
|--------------------------------|---------------------------------------------|---------------------------------|-----------------------------------|---------------------------------------------------------------------------------------------------------------|
| handler of type (or a subtype) | `.WhichAreOfType<T>()`                      | `.IsOfType<T>()`                | `.AreOfType<T>()`                 | `WhichAreNotOfType<T>`, `IsNotOfType<T>`, `AreNotOfType<T>`                                                   |
| handler of exact type          | `.WhichAreOfExactType<T>()`                 | `.IsOfExactType<T>()`           | `.AreOfExactType<T>()`            | `WhichAreNotOfExactType<T>`, `IsNotOfExactType<T>`, `AreNotOfExactType<T>`                                    |
| abstract / sealed              | `.WhichAreAbstract()` / `.WhichAreSealed()` | `.IsAbstract()` / `.IsSealed()` | `.AreAbstract()` / `.AreSealed()` | `WhichAreNotAbstract`, `IsNotAbstract`, `AreNotAbstract` / `WhichAreNotSealed`, `IsNotSealed`, `AreNotSealed` |
| static                         | `.WhichAreStatic()`                         | `.IsStatic()`                   | `.AreStatic()`                    | `WhichAreNotStatic`, `IsNotStatic`, `AreNotStatic`                                                            |
| virtual                        | `.WhichAreVirtual()`                        | `.IsVirtual()`                  | `.AreVirtual()`                   | `WhichAreNotVirtual`, `IsNotVirtual`, `AreNotVirtual`                                                         |
| override                       | `.WhichOverride()`                          | `.Overrides()`                  | `.Override()`                     | `WhichDoNotOverride`, `DoesNotOverride`, `DoNotOverride`                                                      |
| nullable                       | `.WhichAreNullable()`                       | `.IsNullable()`                 | `.AreNullable()`                  | `WhichAreNotNullable`, `IsNotNullable`, `AreNotNullable`                                                      |

The `WhichAreOfType` / `IsOfType` / `AreOfType` filters and expectations match the event's handler type (its
`EventHandlerType`, e.g. `EventHandler<T>`); the `…ExactType` variants match only the exact handler type.
Use `OrOfType<T>()` / `OrOfExactType<T>()` to allow several handler types, and chain
`.WhichAreNotOfType<T1>().WhichAreNotOfType<T2>()` to exclude several.

An event counts as *nullable* when its handler type is annotated as nullable
(e.g. `event EventHandler? Changed;`), following the same [nullability](#nullability) rules as
properties and fields.

```csharp
// Every event must use the generic EventHandler<T> pattern
await Expect.That(In.AllLoadedAssemblies().Public.Events())
    .AreOfType(typeof(EventHandler<>));

In.AllLoadedAssemblies().Public.Events()
    .WhichAreOfType<EventHandler>()
    .WithName("Changed").AsSuffix()
    .With<ObsoleteAttribute>()
```
