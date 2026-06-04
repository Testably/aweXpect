# Feature Comparison with FluentAssertions

aweXpect.Reflection and FluentAssertions both assert against reflection types, with different APIs and
feature sets. This page maps what each offers for assemblies, types, and members.

## Conceptual model

FluentAssertions extends each reflection object with a synchronous `subject.Should()` entry point and
reaches collections through selectors such as `assembly.Types()` or `type.Methods()`. aweXpect.Reflection
wraps the same objects in an asynchronous `await Expect.That(subject)` call and builds collections from
`In.*` sources (`In.AllLoadedAssemblies()`, `In.AssemblyContaining<T>()`, …) that are narrowed with
`WhichAre*`/`With*` filter chains. aweXpect.Reflection also offers symmetric singular (`Is*`/`Has*`) and
plural (`Are*`/`Have*`) assertions for every target, while FluentAssertions pairs single-subject assertions
with a smaller set of selector assertions.

## Coverage overview

Legend: ✅ dedicated assertion · ⚠️ only via a general mechanism (selector, string assertion, or LINQ) · ❌ no dedicated assertion.

| Capability                                                                    | FluentAssertions | aweXpect.Reflection |
|-------------------------------------------------------------------------------|:----------------:|:-------------------:|
| **Assembly**: references / dependencies                                       |        ✅         |          ✅          |
| **Assembly**: name                                                            |        ⚠️        |          ✅          |
| **Assembly**: version                                                         |        ❌         |          ✅          |
| **Assembly**: strong name / signing                                           |        ✅         |          ✅          |
| **Assembly**: target framework                                                |        ❌         |          ✅          |
| **Type**: kind (class/interface/enum/struct/record/…)                         |        ⚠️        |          ✅          |
| **Type**: abstract / sealed / static                                          |        ✅         |          ✅          |
| **Type**: access modifier                                                     |        ✅         |          ✅          |
| **Type**: assignable / implement / derive                                     |        ✅         |          ✅          |
| **Type**: namespace                                                           |        ⚠️        |          ✅          |
| **Type**: name                                                                |        ⚠️        |          ✅          |
| **Type**: attributes (incl. predicate)                                        |        ✅         |          ✅          |
| **Type**: member presence by signature                                        |        ✅         |         ⚠️          |
| **Type**: operator presence (by kind)                                         |        ⚠️        |          ✅          |
| **Type**: conversion operators by signature                                   |        ✅         |          ✅          |
| **Method**: virtual / async / return type                                     |        ✅         |          ✅          |
| **Method**: static / abstract / sealed / generic / extension / operator       |        ❌         |          ✅          |
| **Method**: override                                                          |        ❌         |          ✅          |
| **Method**: parameter modifiers / types / counts                              |        ❌         |          ✅          |
| **Method**: access modifier                                                   |        ✅         |          ✅          |
| **Method**: name                                                              |        ⚠️        |          ✅          |
| **Property**: readable / writable                                             |        ✅         |          ✅          |
| **Property**: virtual                                                         |        ✅         |          ✅          |
| **Property**: static / abstract / required / indexer / init-setter / override |        ❌         |          ✅          |
| **Property**: return / declared type                                          |        ✅         |          ✅          |
| **Constructor**: dedicated assertions / filtering                             |        ⚠️        |          ✅          |
| **Field**: assertions                                                         |        ❌         |          ✅          |
| **Event**: assertions                                                         |        ❌         |          ✅          |

The sections below detail each target with side-by-side examples (FluentAssertions first, then aweXpect.Reflection).

## Assembly

- **FluentAssertions** (`AssemblyAssertions`): `Reference`/`NotReference`, `BeSignedWithPublicKey`/`BeUnsigned`, `DefineType`. No assertion for assembly name, version, or target framework; use the string API on `assembly.GetName()`.
- **aweXpect.Reflection**: `HasName`, `HasVersion`, `DependsOn`/`DoesNotDependOn`/`DependsOnlyOn`, `IsStrongNamed`, `Targets`, `Has<TAttribute>`; plural equivalents (`HaveName`, …) over collections.

```csharp
// FluentAssertions
assembly.Should().Reference(typeof(string).Assembly);
assembly.Should().NotReference(unwantedAssembly);
assembly.Should().BeSignedWithPublicKey("0024000004800000...");
assembly.GetName().Name.Should().Be("MyAssembly");          // name via string API
```

```csharp
// aweXpect.Reflection
await Expect.That(assembly).HasName("MyAssembly");
await Expect.That(assembly).DependsOn("System.Core");
await Expect.That(assembly).DoesNotDependOn("UnwantedDependency");
await Expect.That(assembly).IsStrongNamed();

await Expect.That(In.AllLoadedAssemblies()).HaveName("System").AsPrefix();
```

## Type

- **FluentAssertions** (`TypeAssertions`): `Be<T>`/`NotBe<T>`, `BeAssignableTo<T>`, `Implement<T>`/`NotImplement<T>`, `BeDerivedFrom<T>`, `BeAbstract`/`BeSealed`/`BeStatic`, `HaveAccessModifier(CSharpAccessModifier)`, `BeDecoratedWith<T>` (with attribute predicate and `OrInherit` variants), member presence (`HaveProperty`/`HaveMethod`/`HaveConstructor`/`HaveIndexer`/`HaveExplicit*`), and conversion operators. Type *kind* is available only as a selector filter (`ThatAreClasses()`), and namespace
  only on the selector (`BeInNamespace`); a single type's name/namespace use the string API.
- **aweXpect.Reflection**: type-kind assertions (`IsAClass`, `IsAnInterface`, `IsAnEnum`, `IsAStruct`, `IsARecord`, `IsARecordStruct`, `IsARefStruct`, `IsADelegate`, `IsAnAttribute`, `IsAnException`), `IsAbstract`/`IsSealed`/`IsStatic`/`IsReadOnly`/`IsNested`/`IsGeneric`/`IsInstantiable`, access modifiers (`IsPublic`, …), `InheritsFrom<T>().Directly()`, `HasName`, `HasNamespace`/`IsWithinNamespace`, `Has<TAttribute>`, quantified member containment (`ContainsMethods()`, …), operator presence by
  kind (`HasOperator(Operator)`, `HasOperator<TOperand>(Operator)`) and conversion operators by signature (`HasImplicitConversionOperator<TSource, TTarget>`, `HasExplicitConversionOperator<TSource, TTarget>`).

```csharp
// FluentAssertions
typeof(MyClass).Should().BeAssignableTo<IMyInterface>();
typeof(MyClass).Should().BeDerivedFrom<BaseClass>();
typeof(AbstractClass).Should().BeAbstract();
typeof(MyClass).Should().HaveAccessModifier(CSharpAccessModifier.Public);
typeof(MyClass).Should().BeDecoratedWith<MyAttribute>(a => a.Value == 3);
typeof(MyClass).Namespace.Should().Be("MyNamespace");       // namespace via string API

// type kind only as a selector filter:
AllTypes.From(assembly).ThatAreClasses().Should().BeInNamespace("MyNamespace");
```

```csharp
// aweXpect.Reflection
await Expect.That(typeof(MyClass)).IsAClass();
await Expect.That(typeof(IMyInterface)).IsAnInterface();
await Expect.That(typeof(MyEnum)).IsAnEnum();
await Expect.That(typeof(AbstractClass)).IsAbstract();
await Expect.That(typeof(MyClass)).HasNamespace("MyNamespace");
await Expect.That(typeof(MyClass)).Has<MyAttribute>(a => a.Value == 3);
await Expect.That(typeof(Money)).HasOperator(Operator.Addition);
await Expect.That(typeof(Money)).HasImplicitConversionOperator<Money, decimal>();

await Expect.That(In.AllLoadedAssemblies()
        .Types().WhichAreClasses().WhichArePublic())
    .HaveNamespace("MyNamespace").AsPrefix();
```

## Method

- **FluentAssertions** (`MethodInfoAssertions`): `BeVirtual`/`NotBeVirtual`, `BeAsync`/`NotBeAsync`, `ReturnVoid`/`NotReturnVoid`, `Return<T>`/`NotReturn<T>`, `HaveAccessModifier(CSharpAccessModifier)`, `BeDecoratedWith<T>`. No assertions for `static`/`abstract`/`sealed`/`generic`/extension/operator/override, no parameter assertions, and no name assertion (use the string API on `method.Name`).
- **aweXpect.Reflection**: the above plus `IsStatic`/`IsAbstract`/`IsSealed`/`IsGeneric`/`IsAnExtensionMethod`/`IsAnOperator` (incl. a specific `Operator`, e.g. `IsAnOperator(Operator.Equality)`), `Overrides<T>`, `ReturnsExactly<T>`, `HasName`, and a parameter family: `HasParameter<T>`/`HasParameterCount`/`HasInParameter`/`HasOutParameter`/`HasRefParameter`/`HasOptionalParameter`/`HasParamsParameter` (with `AtIndex`/`FromEnd`/default-value refinements).

```csharp
// FluentAssertions
MethodInfo method = typeof(MyClass).GetMethod("MyMethod");
method.Should().BeVirtual();
method.Should().Return<Task>();
method.Should().HaveAccessModifier(CSharpAccessModifier.Public);
method.Should().BeDecoratedWith<ObsoleteAttribute>();
method.Name.Should().StartWith("My");                       // name via string API
```

```csharp
// aweXpect.Reflection
MethodInfo method = typeof(MyClass).GetMethod("MyMethod");
await Expect.That(method).IsPublic();
await Expect.That(method).IsAsync();
await Expect.That(method).Returns<Task>();
await Expect.That(method).HasName("My").AsPrefix();
await Expect.That(method).Has<ObsoleteAttribute>();

await Expect.That(In.AssemblyContaining<MyClass>()
        .Methods().WhichArePublic().With<TestAttribute>())
    .HaveName("Test").AsPrefix();
```

## Property

- **FluentAssertions** (`PropertyInfoAssertions`): `BeVirtual`/`NotBeVirtual`, `BeReadable`/`BeWritable` (each with an optional `CSharpAccessModifier`), `NotBeReadable`/`NotBeWritable`, `Return<T>`/`NotReturn<T>`, `BeDecoratedWith<T>`. No assertions for `static`/`abstract`/`sealed`/`required`/indexer/init-setter/override, and no name assertion.
- **aweXpect.Reflection**: `IsReadable`/`IsWritable`/`IsReadOnly`/`IsWriteOnly`/`IsReadWrite`, `HasAGetter`/`HasASetter`/`HasAnInitSetter`, `IsStatic`/`IsAbstract`/`IsSealed`/`IsVirtual`, `IsRequired`, `IsAnIndexer`, `Overrides<T>`, `IsOfType<T>`/`IsOfExactType<T>`, `HasName`, `Has<TAttribute>`.

```csharp
// FluentAssertions
PropertyInfo property = typeof(MyClass).GetProperty("MyProperty");
property.Should().BeReadable(CSharpAccessModifier.Public);
property.Should().BeWritable();
property.Should().Return<int>();
```

```csharp
// aweXpect.Reflection
PropertyInfo property = typeof(MyClass).GetProperty("MyProperty");
await Expect.That(property).HasAGetter();
await Expect.That(property).IsWritable();
await Expect.That(property).IsOfType<int>();
await Expect.That(property).HasName("MyProperty");

await Expect.That(In.AssemblyContaining<MyClass>()
        .Properties().WhichArePublic())
    .HaveName("Id").AsSuffix();
```

## Constructor

- **FluentAssertions**: no dedicated constructor selector. A constructor is reached through `Type.HaveConstructor(parameterTypes)` / `HaveDefaultConstructor()`, after which `.Which` exposes the `ConstructorInfo` for `HaveAccessModifier` / `BeDecoratedWith`.
- **aweXpect.Reflection**: dedicated singular (`ThatConstructor`) and plural (`ThatConstructors`) assertions and an `In.*…Constructors()` filter: `IsStatic`, the full parameter family (`HasParameter<T>`, `HasParameterCount`, `HasInParameter`/`HasOutParameter`/`HasRefParameter`/`HasOptionalParameter`/`HasParamsParameter`), `Has<TAttribute>`.

```csharp
// FluentAssertions
typeof(MyClass).Should().HaveConstructor(new[] { typeof(int) })
    .Which.Should().HaveAccessModifier(CSharpAccessModifier.Public);
typeof(MyClass).Should().HaveDefaultConstructor();
```

```csharp
// aweXpect.Reflection
await Expect.That(In.Type<MyClass>().Constructors().WhichArePublic())
    .HaveParameterCount(1);
await Expect.That(typeof(MyClass)).HasADefaultConstructor();
```

## Field

- **FluentAssertions**: no `FieldInfo` assertions.
- **aweXpect.Reflection**: singular (`ThatField`) and plural (`ThatFields`) assertions and an `In.*…Fields()` filter: `IsStatic`, `IsReadOnly`, `IsConstant`, `IsOfType<T>`/`IsOfExactType<T>`, access modifiers (`IsPublic`, …), `HasName`, `Has<TAttribute>`.

```csharp
// FluentAssertions
// No dedicated FieldInfo assertions; fall back to raw reflection + LINQ/string assertions:
typeof(MyClass).GetField("_counter", BindingFlags.Instance | BindingFlags.NonPublic)
    .IsInitOnly.Should().BeTrue();
```

```csharp
// aweXpect.Reflection
FieldInfo field = typeof(MyClass).GetField("_counter",
    BindingFlags.Instance | BindingFlags.NonPublic);
await Expect.That(field).IsReadOnly();
await Expect.That(field).IsPrivate();

await Expect.That(In.AssemblyContaining<MyClass>().Fields().WhichAreConstant())
    .ArePublic();
```

## Event

- **FluentAssertions**: no `EventInfo` assertions.
- **aweXpect.Reflection**: singular (`ThatEvent`) and plural (`ThatEvents`) assertions and an `In.*…Events()` filter: `IsStatic`, `IsAbstract`, `IsSealed`, `IsOfType<T>`/`IsOfExactType<T>`, access modifiers, `HasName`, `Has<TAttribute>`.

```csharp
// FluentAssertions
// No dedicated EventInfo assertions; fall back to raw reflection:
typeof(MyClass).GetEvent("Changed").Should().NotBeNull();
```

```csharp
// aweXpect.Reflection
EventInfo @event = typeof(MyClass).GetEvent("Changed");
await Expect.That(@event).IsStatic();
await Expect.That(@event).Has<ObsoleteAttribute>();

await Expect.That(In.AssemblyContaining<MyClass>().Events().WhichArePublic())
    .HaveName("Changed");
```

## Selecting and filtering sets

- **FluentAssertions**: build a set with `assembly.Types()`, `AllTypes.From(assembly)`, `type.Methods()`, `type.Properties()`, then narrow with selector methods (`ThatAreClasses()`, `ThatDeriveFrom<T>()`, `ThatAreDecoratedWith<T>()`, `ThatAreInNamespace()`, `ThatReturn<T>()`, `ThatArePublicOrInternal`, `ThatSatisfy(predicate)`, …) and assert with the selector assertions. There are no `FieldInfo`/`EventInfo` selectors and no `ConstructorInfo` selector.
- **aweXpect.Reflection**: start from an `In.*` source, then chain `WhichAre*`/`With<T>`/`WithName`/`Without*` filters (with `Or*` combinations and quantifiers) across every member kind, and assert with the plural `Are*`/`Have*` methods.

```csharp
// FluentAssertions
AllTypes.From(assembly)
    .ThatAreClasses()
    .ThatAreDecoratedWith<TestFixtureAttribute>()
    .Should().BeInNamespace("MyProject.Tests");
```

```csharp
// aweXpect.Reflection
await Expect.That(In.AllLoadedAssemblies()
        .Methods().With<FactAttribute>().OrWith<TheoryAttribute>()
        .DeclaringTypes())
    .HaveName("Tests").AsSuffix();
```

## Name and string matching

- **FluentAssertions**: reflection assertions have no built-in name matching. Names are compared with the string API (`type.Name.Should().StartWith(…)`) or a LINQ predicate on a selector.
- **aweXpect.Reflection**: every name/namespace/dependency comparison exposes string options: `AsPrefix()`, `AsSuffix()`, `AsRegex()`, `AsWildcard()`, `IgnoringCase()`, `Using(comparer)`.

```csharp
// FluentAssertions
type.Name.Should().EndWith("Tests");

// aweXpect.Reflection
await Expect.That(type).HasName("Tests").AsSuffix();
```
