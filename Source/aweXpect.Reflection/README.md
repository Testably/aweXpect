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

## Advanced Filtering with `In`

The `In` helper provides powerful filtering capabilities to construct collections of reflection types that match specific criteria. This allows for complex queries across assemblies, types, and their members.

### Assembly Selection

You can select assemblies in various ways:

```csharp
// All currently loaded assemblies (excluding system assemblies)
In.AllLoadedAssemblies()

// Specific assemblies
In.Assemblies(assembly1, assembly2)
In.Assemblies(assemblyCollection)

// Assembly containing a specific type
In.AssemblyContaining<MyClass>()
In.AssemblyContaining(typeof(MyClass))

// Special assemblies
In.EntryAssembly()
In.ExecutingAssembly()
```

### Type Selection

From assemblies, you can navigate to types:

```csharp
// All types in assemblies
In.AllLoadedAssemblies().Types()

// Specific types
In.Type<MyClass>()
In.Type(typeof(MyClass))
In.Types<Class1, Class2>()
In.Types<Class1, Class2, Class3>()
In.Types(type1, type2, type3)
```

### Member Navigation

From types, you can navigate to their members:

```csharp
Type myType = typeof(MyClass);

// Get all members
In.Type(myType).Methods()
In.Type(myType).Properties()
In.Type(myType).Fields()
In.Type(myType).Events()
In.Type(myType).Constructors()

// Navigate back to declaring types from members
In.AllLoadedAssemblies().Methods().DeclaringTypes()
```

### Advanced Filtering

You can apply complex filters to narrow down your selections:

#### Type Filters

```csharp
// Filter by type characteristics
In.AllLoadedAssemblies().Types()
    .WhichAreClasses()
    .WhichArePublic()
    .WhichAreAbstract()
    .WhichAreSealed()
    .WhichAreStatic()
    .WhichAreGeneric()
    .WhichAreNested()
	
// Alternatively
In.AllLoadedAssemblies().Public.Abstract.Classes()
In.AllLoadedAssemblies().Internal.Generic.Interfaces()

// Filter by name or namespace
In.AllLoadedAssemblies().Types()
    .WithName("Service").AsSuffix()
    .WithNamespace("MyApp.Services")

// Filter by inheritance
In.AllLoadedAssemblies().Types()
    .WhichInheritFrom<BaseClass>()
    .WhichInheritFrom(typeof(IInterface))

// Filter by attributes
In.AllLoadedAssemblies().Types()
    .With<ObsoleteAttribute>()
    .With<DescriptionAttribute>(a => a.Description.Contains("important"))

// Filter by custom predicates
In.AllLoadedAssemblies().Types()
    .WhichSatisfy(t => t.Name.StartsWith("Test"))
```

#### Method Filters

```csharp
// Filter by method characteristics
In.AllLoadedAssemblies().Methods()
    .WhichArePublic()
    .WhichArePrivate()
    .WhichAreProtected()
    .WhichAreInternal()

// Filter by return types
In.AllLoadedAssemblies().Methods()
    .WhichReturn<Task>()           // Methods returning Task or Task<T>
    .WhichReturnExactly<Task>()    // Methods returning exactly Task
    .WhichReturn<string>()

// Filter by parameters
In.AllLoadedAssemblies().Methods()
    .WithoutParameters()
    .WithParameter<string>()
    .WithParameter<int>("count")
    .WithParameterCount(2)

// Filter by attributes
In.AllLoadedAssemblies().Methods()
    .With<TestAttribute>()
    .With<ObsoleteAttribute>(a => a.Message != null)

// Filter by name
In.AllLoadedAssemblies().Methods()
    .WithName("Get").AsPrefix()
    .WithName("Async").AsSuffix()
```

#### Property, Field, Event, and Constructor Filters

```csharp
// Properties
In.AllLoadedAssemblies().Properties()
    .WhichArePublic()
    .OfType<string>()
    .OfExactType<List<int>>()
    .WithName("Id").AsSuffix()
    .With<RequiredAttribute>()

// Fields
In.AllLoadedAssemblies().Fields()
    .WhichArePrivate()
    .OfType<ILogger>()
    .WithName("_").AsPrefix()
    .With<NonSerializedAttribute>()

// Events
In.AllLoadedAssemblies().Events()
    .WhichArePublic()
    .WithName("Changed").AsSuffix()
    .With<ObsoleteAttribute>()

// Constructors
In.AllLoadedAssemblies().Constructors()
    .WhichArePublic()
    .WithoutParameters()
    .WithParameter<string>()
    .WithParameterCount(1)
    .With<JsonConstructorAttribute>()
```

### Combining Filters

Filters can be chained and combined using `Or` methods:

```csharp
// Multiple attribute options
In.AllLoadedAssemblies().Methods()
    .With<FactAttribute>().OrWith<TheoryAttribute>()

// Multiple return type options
In.AllLoadedAssemblies().Methods()
    .WhichReturn<Task>().OrReturn<ValueTask>()

// Complex combinations
In.AllLoadedAssemblies().Types()
    .WhichAreClasses()
    .WhichArePublic()
    .WithName("Service").AsSuffix()
    .Methods()
    .WhichArePublic()
    .With<HttpGetAttribute>().OrWith<HttpPostAttribute>()
```

### Real-World Examples

Here are some practical examples of using the `In` helper:

```csharp
// Verify all test classes follow naming convention
await Expect.That(In.AllLoadedAssemblies()
        .Methods().With<FactAttribute>().OrWith<TheoryAttribute>()
        .DeclaringTypes())
    .HaveName("Tests").AsSuffix();

// Verify all async methods have "Async" suffix
await Expect.That(In.AssemblyContaining<MyClass>()
        .Methods().WhichReturn<Task>().OrReturn<ValueTask>())
    .HaveName("Async").AsSuffix();

// Verify all methods with "Async" suffix return Task or ValueTask
await Expect.That(In.AssemblyContaining<MyClass>()
        .Methods().WithName("Async").AsSuffix())
    .Return<Task>().OrReturn<ValueTask>();

// Verify all public classes have XML documentation
await Expect.That(In.AllLoadedAssemblies()
        .Types().WhichAreClasses().WhichArePublic())
    .Have<DocumentationAttribute>();

// Verify controllers follow naming convention
await Expect.That(In.AllLoadedAssemblies()
        .Types().WhichInheritFrom<ControllerBase>())
    .HaveName("Controller").AsSuffix();

// Verify all properties with setters are not init-only
await Expect.That(In.AllLoadedAssemblies()
        .Properties().WhichSatisfy(p => p.SetMethod != null))
    .Satisfy(p => !p.SetMethod.ReturnParameter.GetRequiredCustomModifiers()
        .Contains(typeof(System.Runtime.CompilerServices.IsExternalInit)));
```

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

### Dependencies

You can verify whether assemblies have specific dependencies:

```csharp
Assembly subject = Assembly.GetEntryAssembly();
Assembly[] subjects = AppDomain.CurrentDomain.GetAssemblies();

// Single assembly
await Expect.That(subject).HasADependencyOn("System.Core");
await Expect.That(subject).HasNoDependencyOn("UnwantedDependency");

// Multiple assemblies
await Expect.That(subjects).HaveADependencyOn("System.Core");
await Expect.That(subjects).HaveNoDependencyOn("UnwantedDependency");
```

### Attributes

You can verify whether assemblies have specific attributes:

```csharp
Assembly subject = Assembly.GetEntryAssembly();
Assembly[] subjects = AppDomain.CurrentDomain.GetAssemblies();

// Single assembly
await Expect.That(subject).Has<AssemblyTitleAttribute>();
await Expect.That(subject).Has<AssemblyVersionAttribute>(a => a.Version == "1.0.0");

// Multiple assemblies
await Expect.That(subjects).Have<AssemblyTitleAttribute>();
```

## Types

### Name / Namespace

You can verify the name or namespace of a type or a collection of types:

```csharp
Type subject = typeof(MyClass);
IEnumerable<Type> subjects = In.EntryAssembly().Types();

await Expect.That(subject).HasNamespace("aweXpect").AsPrefix();
await Expect.That(subject).HasName("MyClass");

await Expect.That(subjects).HaveNamespace("aweXpect").AsPrefix();
await Expect.That(subjects).HaveName("Tests").AsSuffix();
```

You can use the same configuration options as
when [comparing strings](https://awexpect.com/docs/expectations/string#equality).

### Type Kinds

You can verify what kind of type you're dealing with:

```csharp
Type subject = typeof(MyClass);
IEnumerable<Type> subjects = In.EntryAssembly().Types();

// Single type
await Expect.That(subject).IsAClass();
await Expect.That(subject).IsAnInterface();
await Expect.That(subject).IsAnEnum();
await Expect.That(subject).IsAbstract();
await Expect.That(subject).IsSealed();
await Expect.That(subject).IsStatic();
await Expect.That(subject).IsGeneric();
await Expect.That(subject).IsNested();

// Multiple types
await Expect.That(subjects).AreClasses();
await Expect.That(subjects).AreInterfaces();
await Expect.That(subjects).AreEnums();
await Expect.That(subjects).AreAbstract();
await Expect.That(subjects).AreSealed();
await Expect.That(subjects).AreStatic();
await Expect.That(subjects).AreGeneric();
await Expect.That(subjects).AreNested();

// Negative assertions
await Expect.That(subject).IsNotAClass();
await Expect.That(subject).IsNotAnInterface();
await Expect.That(subject).IsNotAnEnum();
await Expect.That(subject).IsNotAbstract();
await Expect.That(subject).IsNotSealed();
await Expect.That(subject).IsNotStatic();
await Expect.That(subject).IsNotGeneric();
await Expect.That(subject).IsNotNested();

// Multiple types negative assertions
await Expect.That(subjects).AreNotClasses();
await Expect.That(subjects).AreNotInterfaces();
await Expect.That(subjects).AreNotEnums();
await Expect.That(subjects).AreNotAbstract();
await Expect.That(subjects).AreNotSealed();
await Expect.That(subjects).AreNotStatic();
await Expect.That(subjects).AreNotGeneric();
await Expect.That(subjects).AreNotNested();
```

### Access Modifiers

You can verify the access modifiers of types:

```csharp
Type subject = typeof(MyClass);
IEnumerable<Type> subjects = In.EntryAssembly().Types();

// Single type
await Expect.That(subject).IsPublic();
await Expect.That(subject).IsInternal();
await Expect.That(subject).IsPrivate();
await Expect.That(subject).IsProtected();

// Multiple types
await Expect.That(subjects).ArePublic();
await Expect.That(subjects).AreInternal();
await Expect.That(subjects).ArePrivate();
await Expect.That(subjects).AreProtected();

// Negative assertions
await Expect.That(subject).IsNotPublic();
await Expect.That(subject).IsNotInternal();
await Expect.That(subject).IsNotPrivate();
await Expect.That(subject).IsNotProtected();
```

### Attributes

You can verify whether types have specific attributes:

```csharp
Type subject = typeof(MyClass);
IEnumerable<Type> subjects = In.EntryAssembly().Types();

// Single type
await Expect.That(subject).Has<ObsoleteAttribute>();
await Expect.That(subject).Has<ObsoleteAttribute>(a => a.Message == "Use NewClass instead");

// Multiple types
await Expect.That(subjects).Have<SerializableAttribute>();
```

## Methods

### Name

You can verify the names of methods:

```csharp
MethodInfo subject = typeof(MyClass).GetMethod("MyMethod");
IEnumerable<MethodInfo> subjects = typeof(MyClass).GetMethods();

// Single method
await Expect.That(subject).HasName("MyMethod");

// Multiple methods
await Expect.That(subjects).HaveName("Get").AsPrefix();
```

### Parameters

You can verify method parameters:

```csharp
MethodInfo subject = typeof(MyClass).GetMethod("MyMethod");
IEnumerable<MethodInfo> subjects = typeof(MyClass).GetMethods();

// Single method
await Expect.That(subject).HasParameter<string>();
await Expect.That(subject).HasParameter<string>("parameterName");
await Expect.That(subject).HasParameter("parameterName").OfType<int>();

// Multiple methods
await Expect.That(subjects).HaveParameter<string>();
await Expect.That(subjects).HaveParameter<DateTime>("timestamp");
```

### Return Types

You can verify what methods return:

```csharp
MethodInfo subject = typeof(MyClass).GetMethod("MyMethod");
IEnumerable<MethodInfo> subjects = typeof(MyClass).GetMethods();

// Single method
await Expect.That(subject).Returns<string>();
await Expect.That(subject).ReturnsExactly<string>(); // Exact type match
await Expect.That(subject).Returns<Task>(); // Also matches Task<T>
await Expect.That(subject).ReturnsExactly<Task>(); // Only matches Task, not Task<T>

// Multiple methods
await Expect.That(subjects).Return<Task>();
await Expect.That(subjects).ReturnExactly<void>();
```

### Access Modifiers

You can verify the access modifiers of methods:

```csharp
MethodInfo subject = typeof(MyClass).GetMethod("MyMethod");
IEnumerable<MethodInfo> subjects = typeof(MyClass).GetMethods();

// Single method
await Expect.That(subject).IsPublic();
await Expect.That(subject).IsPrivate();
await Expect.That(subject).IsProtected();
await Expect.That(subject).IsInternal();

// Multiple methods
await Expect.That(subjects).ArePublic();
await Expect.That(subjects).ArePrivate();
await Expect.That(subjects).AreProtected();
await Expect.That(subjects).AreInternal();

// Negative assertions
await Expect.That(subject).IsNotPublic();
await Expect.That(subjects).AreNotPrivate();
```

### Attributes

You can verify whether methods have specific attributes:

```csharp
MethodInfo subject = typeof(MyClass).GetMethod("MyMethod");
IEnumerable<MethodInfo> subjects = typeof(MyClass).GetMethods();

// Single method
await Expect.That(subject).Has<ObsoleteAttribute>();
await Expect.That(subject).Has<DescriptionAttribute>(a => a.Description == "My method");

// Multiple methods
await Expect.That(subjects).Have<AsyncStateMachineAttribute>();
```

## Properties

### Name and Type

You can verify properties by name:

```csharp
PropertyInfo subject = typeof(MyClass).GetProperty("MyProperty");
IEnumerable<PropertyInfo> subjects = typeof(MyClass).GetProperties();

// Single property
await Expect.That(subject).HasName("MyProperty");

// Multiple properties
await Expect.That(subjects).HaveName("Id").AsSuffix();
```

### Access Modifiers

You can verify the access modifiers of properties:

```csharp
PropertyInfo subject = typeof(MyClass).GetProperty("MyProperty");
IEnumerable<PropertyInfo> subjects = typeof(MyClass).GetProperties();

// Single property
await Expect.That(subject).IsPublic();
await Expect.That(subject).IsPrivate();
await Expect.That(subject).IsProtected();
await Expect.That(subject).IsInternal();

// Multiple properties
await Expect.That(subjects).ArePublic();
await Expect.That(subjects).AreInternal();

// Negative assertions
await Expect.That(subject).IsNotPrivate();
await Expect.That(subjects).AreNotProtected();
```

### Attributes

You can verify whether properties have specific attributes:

```csharp
PropertyInfo subject = typeof(MyClass).GetProperty("MyProperty");
IEnumerable<PropertyInfo> subjects = typeof(MyClass).GetProperties();

// Single property
await Expect.That(subject).Has<RequiredAttribute>();
await Expect.That(subject).Has<JsonPropertyNameAttribute>(a => a.Name == "my_property");

// Multiple properties
await Expect.That(subjects).Have<JsonIgnoreAttribute>();
```

## Fields

### Name

You can verify fields by name:

```csharp
FieldInfo subject = typeof(MyClass).GetField("MyField");
IEnumerable<FieldInfo> subjects = typeof(MyClass).GetFields();

// Single field
await Expect.That(subject).HasName("MyField");

// Multiple fields
await Expect.That(subjects).HaveName("_").AsPrefix();
```

### Access Modifiers

You can verify the access modifiers of fields:

```csharp
FieldInfo subject = typeof(MyClass).GetField("MyField");
IEnumerable<FieldInfo> subjects = typeof(MyClass).GetFields();

// Single field
await Expect.That(subject).IsPublic();
await Expect.That(subject).IsPrivate();
await Expect.That(subject).IsProtected();
await Expect.That(subject).IsInternal();

// Multiple fields
await Expect.That(subjects).ArePrivate();

// Negative assertions
await Expect.That(subject).IsNotPublic();
await Expect.That(subjects).AreNotPublic();
```

### Attributes

You can verify whether fields have specific attributes:

```csharp
FieldInfo subject = typeof(MyClass).GetField("MyField");
IEnumerable<FieldInfo> subjects = typeof(MyClass).GetFields();

// Single field
await Expect.That(subject).Has<NonSerializedAttribute>();

// Multiple fields
await Expect.That(subjects).Have<CompilerGeneratedAttribute>();
```

## Events

### Name

You can verify event names:

```csharp
EventInfo subject = typeof(MyClass).GetEvent("MyEvent");
IEnumerable<EventInfo> subjects = typeof(MyClass).GetEvents();

// Single event
await Expect.That(subject).HasName("MyEvent");

// Multiple events
await Expect.That(subjects).HaveName("Changed").AsSuffix();
```

### Access Modifiers

You can verify the access modifiers of events:

```csharp
EventInfo subject = typeof(MyClass).GetEvent("MyEvent");
IEnumerable<EventInfo> subjects = typeof(MyClass).GetEvents();

// Single event
await Expect.That(subject).IsPublic();
await Expect.That(subject).IsPrivate();
await Expect.That(subject).IsProtected();
await Expect.That(subject).IsInternal();

// Multiple events
await Expect.That(subjects).ArePublic();
await Expect.That(subjects).AreInternal();

// Negative assertions
await Expect.That(subject).IsNotPrivate();
await Expect.That(subjects).AreNotProtected();
```

### Attributes

You can verify whether events have specific attributes:

```csharp
EventInfo subject = typeof(MyClass).GetEvent("MyEvent");
IEnumerable<EventInfo> subjects = typeof(MyClass).GetEvents();

// Single event
await Expect.That(subject).Has<ObsoleteAttribute>();

// Multiple events
await Expect.That(subjects).Have<EditorBrowsableAttribute>();
```

## Constructors

### Parameters

You can verify constructor parameters:

```csharp
ConstructorInfo subject = typeof(MyClass).GetConstructor(Type.EmptyTypes);
IEnumerable<ConstructorInfo> subjects = typeof(MyClass).GetConstructors();

// Single constructor
await Expect.That(subject).HasParameter<string>();
await Expect.That(subject).HasParameter<string>("name");

// Multiple constructors
await Expect.That(subjects).HaveParameter<ILogger>();
```

### Attributes

You can verify whether constructors have specific attributes:

```csharp
ConstructorInfo subject = typeof(MyClass).GetConstructor(Type.EmptyTypes);
IEnumerable<ConstructorInfo> subjects = typeof(MyClass).GetConstructors();

// Single constructor
await Expect.That(subject).Has<JsonConstructorAttribute>();

// Multiple constructors
await Expect.That(subjects).Have<ObsoleteAttribute>();
```

## String Matching Options

When verifying names and other string properties, you have access to the same powerful string matching options as the core aweXpect library:

### Exact Matching
```csharp
await Expect.That(type).HasName("MyClass"); // Exact match
await Expect.That(assembly).HasName("MyAssembly"); // Exact match
```

### Prefix/Suffix Matching
```csharp
await Expect.That(types).HaveName("Test").AsPrefix();
await Expect.That(types).HaveName("Service").AsSuffix();
await Expect.That(types).HaveNamespace("MyApp").AsPrefix();
```

### Case Sensitivity
```csharp
await Expect.That(type).HasName("myclass").IgnoringCase();
await Expect.That(types).HaveName("SERVICE").AsSuffix().IgnoringCase();
```

### Wildcards and Patterns
```csharp
await Expect.That(types).HaveName("*Test*").AsWildcard();
await Expect.That(methods).HaveName("Get*Async").AsWildcard();
```

### Regular Expressions
```csharp
await Expect.That(types).HaveName(@"^Test\w+$").AsRegex();
await Expect.That(methods).HaveName(@"^(Get|Set)\w+").AsRegex();
```

## Collection Operations

All expectations work seamlessly with both single items and collections. When working with collections, you can:

### Apply expectations to all items
```csharp
// All types must be classes
await Expect.That(types).AreClasses();

// All methods must be public
await Expect.That(methods).ArePublic();

// All assemblies must have a specific dependency
await Expect.That(assemblies).HaveADependencyOn("System.Core");
```

### Use quantifiers
```csharp
// At least one type should be abstract
await Expect.That(types).Any().IsAbstract();

// All types should be public
await Expect.That(types).All().ArePublic();

// Exactly 3 methods should have parameters
await Expect.That(methods).Count().Exactly(3).HaveParameter<string>();
```

### Combine with LINQ
```csharp
// Work with filtered collections
var publicMethods = typeof(MyClass).GetMethods().Where(m => m.IsPublic);
await Expect.That(publicMethods).HaveName("Get").AsPrefix();

// Use complex filtering
var complexTypes = In.AllLoadedAssemblies()
    .Types()
    .WhichAreClasses()
    .WhichArePublic()
    .Where(t => t.GetInterfaces().Length > 2);
await Expect.That(complexTypes).HaveName("Manager").AsSuffix();
```

## Configuration and Customization

### Assembly Exclusions

By default, system assemblies that start with the following prefixes are excluded from `In.AllLoadedAssemblies()`:
- "mscorlib"
- "System"
- "Microsoft"
- "JetBrains"
- "xunit"
- "Castle"
- "DynamicProxyGenAssembly2"

You can customize this behavior through aweXpect's customization system via `Customize.aweXpect.Reflection().ExcludedAssemblyPrefixes`.

### Thread Safety

All expectations are thread-safe and can be used in parallel tests without issues.

### Performance Considerations

- The `In` helper uses lazy evaluation where possible
- Filtering operations are optimized for common scenarios
- Consider caching reflection results if you're performing the same queries repeatedly

