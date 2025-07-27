# aweXpect.Reflection

[![Nuget](https://img.shields.io/nuget/v/aweXpect.Reflection)](https://www.nuget.org/packages/aweXpect.Reflection)
[![Build](https://github.com/aweXpect/aweXpect.Reflection/actions/workflows/build.yml/badge.svg)](https://github.com/aweXpect/aweXpect.Reflection/actions/workflows/build.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=aweXpect_aweXpect.Reflection&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=aweXpect_aweXpect.Reflection)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=aweXpect_aweXpect.Reflection&metric=coverage)](https://sonarcloud.io/summary/overall?id=aweXpect_aweXpect.Reflection)
[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2FaweXpect%2FaweXpect.Reflection%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/aweXpect/aweXpect.Reflection/main)

Expectations for reflection types for [aweXpect](https://github.com/aweXpect/aweXpect).

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
// Verifies that all xunit test classes (types with at least one method with
// a `FactAttribute` or `TheoryAttribute`) have a name that ends with "Tests":
await That(In.AllLoadedAssemblies()
        .Methods().With<FactAttribute>().OrWith<TheoryAttribute>()
        .DeclaringTypes())
    .HaveName("Tests").AsSuffix();
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

