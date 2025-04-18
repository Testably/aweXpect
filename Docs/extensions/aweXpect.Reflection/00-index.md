# [aweXpect.Reflection](https://github.com/aweXpect/aweXpect.Reflection) [![Nuget](https://img.shields.io/nuget/v/aweXpect.Reflection)](https://www.nuget.org/packages/aweXpect.Reflection)

Expectations for reflection types.

## Overview

This library contains expectations on reflection types:

- [`Assembly`](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.assembly)
- [`Type`](https://learn.microsoft.com/en-us/dotnet/api/system.type)
- [`ConstructorInfo`](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.constructorinfo)
- [`EventInfo`](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.eventinfo)
- [`FieldInfo`](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.fieldinfo)
- [`MethodInfo`](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.methodinfo)
- [`PropertyInfo`](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)

You can apply the expectations either on a single type or a collection of types (e.g. `Assembly[]` or
`IEnumerable<Type?>`).

### In

There is also a helper construct (`In`) to simplify the construction of collections of reflection types that match
specific criteria:

```csharp
// Verifies that all xunit test classes (types with at least one method with a `FactAttribute` or `TheoryAttribute`) are sealed:
var xunitTestClasses = In.AllLoadedAssemblies().Types().Methods().With<FactAttribute>().OrWith<TheoryAttribute>().Types();
await That(xunitTestClasses).AreSealed();
```

This helper consists of the static class `In` for accessing assemblies and allows filtering and navigating between the
reflection types. The helper types are themselves a collection, so the collection expectations can be applied to them.

## Assemblies

### Name

You can verify the name of an assembly or a collection of assemblies:

```csharp
Assembly subject = Assembly.GetEntryAssembly();
Assembly[] subjects = AppDomain.CurrentDomain.GetAssemblies();

await Expect.That(subject).HasName("aweXpect.Reflection");
await Expect.That(subjects).HaveName("aweXpect").AsPrefix();
```

You can use the same configuration options as
when [comparing strings](https://awexpect.com/docs/expectations/string#equality).

## Types

### Name / Namespace

You can verify the name or namespace of a type or a collection of types:

```csharp
Type subject = typeof(MyClass);
IEnumerable<Type> subjects = In.EntryAssembly().Types();

await Expect.That(subject).HasNamespace("aweXpect").AsPrefix();
await Expect.That(subject).HasName("MyClass");

await Expect.That(subject).HaveNamespace("aweXpect").AsPrefix();
await Expect.That(subjects).HaveName("Tests").AsSuffix();
```

You can use the same configuration options as
when [comparing strings](https://awexpect.com/docs/expectations/string#equality).
