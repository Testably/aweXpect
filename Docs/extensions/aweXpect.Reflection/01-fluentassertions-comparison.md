# Feature Comparison with FluentAssertions

This document provides a comprehensive comparison between aweXpect.Reflection and FluentAssertions for reflection-based
testing scenarios. Both libraries offer powerful capabilities for asserting against reflection types, but with different
syntax patterns and feature sets.

## Overview

**aweXpect.Reflection** is designed as an extension to the aweXpect testing framework, providing expectations for
reflection types with async/await support and rich filtering capabilities.

**FluentAssertions** is a popular general-purpose assertion library that includes extensive reflection testing features
alongside other assertion types.

## Feature Comparison Matrix

| Feature Category         | aweXpect.Reflection | FluentAssertions | Notes                                         |
|--------------------------|---------------------|------------------|-----------------------------------------------|
| **Assembly Testing**     | ✅                   | ✅                | Both support assembly-level assertions        |
| **Type Testing**         | ✅                   | ✅                | Both support comprehensive type assertions    |
| **Method Testing**       | ✅                   | ✅                | Both support method-level assertions          |
| **Property Testing**     | ✅                   | ✅                | Both support property assertions              |
| **Field Testing**        | ✅                   | ✅                | Both support field assertions                 |
| **Event Testing**        | ✅                   | ✅                | Both support event assertions                 |
| **Constructor Testing**  | ✅                   | ✅                | Both support constructor assertions           |
| **Attribute Testing**    | ✅                   | ✅                | Both support attribute presence/value testing |
| **Collection Filtering** | ✅                   | ⚠️               | aweXpect has more advanced filtering via `In` |

| **String Matching Options** | ✅ | ✅ | Both support prefix, suffix, regex, wildcards |
| **Dependency Testing** | ✅ | ✅ | Both support assembly dependency checks |
| **Access Modifier Testing** | ✅ | ✅ | Both support public/private/protected/internal |
| **Type Kind Testing** | ✅ | ✅ | Both support class/interface/enum/abstract/etc |

## Detailed Feature Breakdown

### 1. Assembly Assertions

#### aweXpect.Reflection

```csharp
// Single assembly
await Expect.That(assembly).HasName("MyAssembly");
await Expect.That(assembly).HasADependencyOn("System.Core");
await Expect.That(assembly).HasNoDependencyOn("UnwantedDependency");

// Multiple assemblies
Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
await Expect.That(assemblies).HaveName("System").AsPrefix();
```

#### FluentAssertions

```csharp
// Single assembly
assembly.Should().HaveAssemblyName("MyAssembly");
assembly.Should().Reference("System.Core");
assembly.Should().NotReference("UnwantedDependency");

// Multiple assemblies - requires more manual work
Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
assemblies.Should().OnlyContain(a => a.FullName.StartsWith("System"));
```

### 2. Type Assertions

#### aweXpect.Reflection

```csharp
// Single type
await Expect.That(typeof(MyClass)).IsAClass();
await Expect.That(typeof(IMyInterface)).IsAnInterface();
await Expect.That(typeof(MyEnum)).IsAnEnum();
await Expect.That(typeof(AbstractClass)).IsAbstract();
await Expect.That(typeof(MyClass)).HasNamespace("MyNamespace");
await Expect.That(typeof(MyClass)).Has<MyAttribute>();

// Multiple types with advanced filtering
await Expect.That(In.AllLoadedAssemblies()
    .Types().WhichAreClasses().WhichArePublic())
    .HaveNamespace("MyNamespace").AsPrefix();
```

#### FluentAssertions

```csharp
// Single type
typeof(MyClass).Should().BeClass();
typeof(MyInterface).Should().BeInterface();
typeof(MyEnum).Should().BeEnum();
typeof(AbstractClass).Should().BeAbstract();
typeof(MyClass).Should().BeInNamespace("MyNamespace");
typeof(MyClass).Should().BeDecoratedWith<MyAttribute>();

// Multiple types
var types = Assembly.GetExecutingAssembly().GetTypes();
types.Should().OnlyContain(t => t.IsClass && t.IsPublic);
```

### 3. Method Assertions

#### aweXpect.Reflection

```csharp
// Single method
MethodInfo method = typeof(MyClass).GetMethod("MyMethod");
await Expect.That(method).IsPublic();
await Expect.That(method).HasName("MyMethod");
await Expect.That(method).Has<ObsoleteAttribute>();

// Multiple methods with filtering
await Expect.That(In.AssemblyContaining<MyClass>()
    .Methods().WhichArePublic().With<TestAttribute>())
    .HaveName("Test").AsPrefix();
```

#### FluentAssertions

```csharp
// Single method
MethodInfo method = typeof(MyClass).GetMethod("MyMethod");
method.Should().BePublic();
method.Should().HaveName("MyMethod");
method.Should().BeDecoratedWith<ObsoleteAttribute>();

// Multiple methods
var methods = typeof(MyClass).GetMethods();
methods.Should().OnlyContain(m => m.IsPublic);
```

### 4. Property Assertions

#### aweXpect.Reflection

```csharp
// Single property
PropertyInfo property = typeof(MyClass).GetProperty("MyProperty");
await Expect.That(property).IsPublic();
await Expect.That(property).HasName("MyProperty");

// Multiple properties
await Expect.That(In.AssemblyContaining<MyClass>()
    .Properties().WhichArePublic())
    .HaveName("Id").AsSuffix();
```

#### FluentAssertions

```csharp
// Single property
PropertyInfo property = typeof(MyClass).GetProperty("MyProperty");
property.Should().BePublic();
property.Should().HaveName("MyProperty");

// Multiple properties
var properties = typeof(MyClass).GetProperties();
properties.Should().OnlyContain(p => p.GetGetMethod().IsPublic);
```

### 5. Advanced Filtering and Collection Operations

#### aweXpect.Reflection

```csharp
// Complex filtering scenarios
await Expect.That(In.AllLoadedAssemblies()
        .Methods().With<FactAttribute>().OrWith<TheoryAttribute>()
        .DeclaringTypes())
    .HaveName("Tests").AsSuffix();

// Filter by return types
await Expect.That(In.AssemblyContaining(typeof(In))
        .Methods().WhichReturn<Task>().OrReturn<ValueTask>())
    .HaveName("Async").AsSuffix();
```

#### FluentAssertions

```csharp
// Requires more manual LINQ operations
var testClasses = AppDomain.CurrentDomain.GetAssemblies()
    .SelectMany(a => a.GetTypes())
    .Where(t => t.GetMethods()
        .Any(m => m.HasAttribute<FactAttribute>() || m.HasAttribute<TheoryAttribute>()));

testClasses.Should().OnlyContain(t => t.Name.EndsWith("Tests"));

// Return type filtering requires manual work
var methods = AppDomain.CurrentDomain.GetAssemblies()
    .SelectMany(a => a.GetTypes().SelectMany(t => t.GetMethods()))
    .Where(m => m.ReturnType == typeof(Task) || m.ReturnType == typeof(ValueTask));
methods.Should().OnlyContain(m => m.Name.EndsWith("Async"));
```

