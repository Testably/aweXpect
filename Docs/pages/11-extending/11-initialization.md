---
description: Run code before the first expectation, support another test framework, read the running test and register values from the test project.
---

# Test frameworks and initialization

An extension can run code before its first expectation, support a test framework of its own and read the running test.
This page also shows how the test project passes values and code to your extension before the first test.

<details>
<summary>Namespaces used on this page</summary>

```csharp
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using aweXpect.Core.Adapters;
using aweXpect.Core.Extending;
using aweXpect.Formatting;
```

</details>

## Module initializers

An extension often has to run code once before its first expectation is evaluated, e.g. to register a
[value formatter](./04-formatting.md#value-formatters). Use a
[module initializer](https://learn.microsoft.com/dotnet/csharp/language-reference/proposals/csharp-9.0/module-initializers),
which runs before any other code of your assembly, i.e. when a test first uses the extension. In a class library, the
attribute triggers the warning CA2255, so suppress it as shown:

```csharp
namespace MusicStore
{
    internal static class MusicStoreInitializer
    {
#pragma warning disable CA2255 // The initializer of a class library is intended here
        [ModuleInitializer]
        internal static void Initialize()
            => ValueFormatter.Register(new TrackFormatter());
#pragma warning restore CA2255
    }
}
```

`ModuleInitializerAttribute` is missing in `netstandard2.0` and `net48`: declare it in your own package there, see
[polyfills for older targets](./00-index.md#polyfills-for-older-targets).

## Test framework adapter

aweXpect reports a failed, skipped or inconclusive expectation by throwing the exception of the test framework. For
MSTest, NUnit, TUnit and xUnit (v2 and v3), the `aweXpect` package generates an adapter into the test project and
registers it ([test frameworks](../03-setup/01-test-frameworks.md) lists the exceptions it throws). To support
another test framework, implement `ITestFrameworkAdapter` and register it with
`TestFrameworkRegistry.Register` from a module initializer in the test project itself, as the generated adapters are.
A module initializer only runs when its assembly is first used, and the tests never use an assembly that only contains
an adapter, so a registration in a separate package would never run:

```csharp
namespace MyTests
{
    internal sealed class MyTestFrameworkAdapter : ITestFrameworkAdapter
    {
        public bool IsAvailable => true;

        public void Skip(string message) => throw new MyTestSkippedException(message);

        public void Fail(string message) => throw new MyTestFailedException(message);

        public void Fail(string message, Exception innerException)
            => throw new MyTestFailedException(message, innerException);

        public void Inconclusive(string message) => throw new MyTestInconclusiveException(message);

        public bool TryGetCurrentTest([NotNullWhen(true)] out RunningTest? currentTest)
        {
            MyTestContext? context = MyTestContext.Current;
            currentTest = context is null
                ? null
                : new RunningTest(new TestIdentity(context.ClassName, context.ClassArguments, context.MethodName,
                    context.Arguments),
                    context.Id, context.DisplayName, context.RetryCount);
            return currentTest is not null;
        }

        public bool TryGetCancellationToken(out CancellationToken cancellationToken)
        {
            MyTestContext? context = MyTestContext.Current;
            cancellationToken = context?.CancellationToken ?? CancellationToken.None;
            return context is not null;
        }

        // The test framework cannot attach files to the result of a test.
        public bool TryAddAttachment(string filePath, string? description) => false;
    }

    internal static class MyTestFrameworkRegistration
    {
#pragma warning disable CA2255 // A test project is a class library, too
        [ModuleInitializer]
        internal static void Register()
            => TestFrameworkRegistry.Register(new MyTestFrameworkAdapter());
#pragma warning restore CA2255
    }
}
```

- The adapter is resolved once, when the first expectation is created or a value is first formatted with
  `Format.Formatter`, so register it before that.

<details>
<summary>Availability, precedence and fallbacks</summary>

- An adapter whose `IsAvailable` is `false` is ignored, e.g. when the test framework it throws for is not loaded.
- By default, `Register` replaces an adapter that was registered before. With `overwrite: false`, as the generated
  registrations use it, the adapter is only used when no other one was registered, so an explicit registration always
  wins, independent of the order in which the module initializers run.
- Without any registration, aweXpect throws its own `FailException`, `SkipException` or `InconclusiveException`.
  Only below .NET 8, e.g. on .NET Framework, it first scans the loaded assemblies for a type that implements
  `ITestFrameworkAdapter`, see [settings](../02-how-it-works/07-configuration.md#settings).
- The generated registration is a module initializer, which needs C# 9 or later. On .NET 8 or later, a test project
  with an older `<LangVersion>` has to register the generated adapter itself, which the warning
  [aweXpect2002](../08-analyzers.md#test-framework-adapter) points out.

</details>

### The running test

The `TryGetCurrentTest` of an adapter returns the `RunningTest`, or `false` when its test framework does not expose it.
Its `Identity` holds what identifies the test in every run: the full name of the test class and its arguments, the name
of the test method and the arguments of the test case. When the test framework reports them, the `RunningTest` also
holds the `Id` and the `DisplayName` of the test case and its `RetryAttempt`, which counts the runs of the test before,
e.g. by a retry: `0` for the first run. The `Id` is only unique within one test run, so it does not belong in a persisted name.
An extension reads the running test with `TestFrameworkRegistry.TryGetCurrentTest`, e.g. to tell the cases of a
parameterized test apart, as `aweXpect.Snapshots` does with `NamedAfterTestArguments`:

```csharp
internal static class TrackCaseNames
{
    internal static string? GetCaseName()
        => TestFrameworkRegistry.TryGetCurrentTest(out RunningTest? currentTest) &&
           currentTest.Identity.Arguments.Count > 0
            ? string.Join("_", currentTest.Identity.Arguments)
            : null;
}
```

It returns `false` when the test framework does not expose the running test, or when no test is running, e.g. in a
setup of the whole assembly. Handle that case, as not every test framework exposes the running test; the
[test frameworks](../03-setup/01-test-frameworks.md#the-running-test) page lists what each of them supports.

`TestFrameworkRegistry.TryGetCancellationToken` returns the token with which the test framework cancels the running
test. Expectations only observe it when `Settings().TestCancellation` is set to
`TestCancellation.FromCancellationToken(…)` with it, see
[time and cancellation](../02-how-it-works/06-time-and-cancellation.md#cancellationtoken).
`TestFrameworkRegistry.TryAddAttachment(filePath, description)` attaches a file to the result of the running test, as
`aweXpect.Snapshots` does with a recorded mismatch when `AttachPendingFiles` is set:

```csharp
internal static class PlaylistReports
{
    internal static void Attach(string reportFile)
        => _ = TestFrameworkRegistry.TryAddAttachment(reportFile, "The report of the test");
}
```

- Both return `false` when the test framework does not support it, no test is running or the test framework fails,
  e.g. because the file does not exist. Like `TryGetCurrentTest`, they never throw: the registry also returns `false`
  when an adapter throws, but an adapter of your own should still return `false` instead.
- MSTest, NUnit and TUnit only read the file when the test ends, so it must still exist then.

## Registration from the test project

Some extensions need values that the build of the test project records, e.g. its directory, or have to run code
before the first test, e.g. to add a reporter, although no test uses their assembly before. Instead of scanning the
loaded assemblies, let the source generator of `aweXpect.Core` emit a module initializer into the test project. The
build targets of your package (in its `buildTransitive` folder) only pass names to it, in two MSBuild properties:

- `AweXpectRegisteredMetadataKeys`: the keys of the `AssemblyMetadataAttribute`s of the test project whose values
  are registered in `ExtensionRegistry`.
- `AweXpectModuleInitializers`: the static methods without parameters, accessible from the test project, that are
  called afterwards, so that they can already read the registered values.

Both are lists separated by `,` or `;`. Append your names to the value that other packages set, and make the property
visible to the generator:

```xml
<Project>

    <PropertyGroup>
        <AweXpectRegisteredMetadataKeys>$(AweXpectRegisteredMetadataKeys),MusicStore.Catalog</AweXpectRegisteredMetadataKeys>
        <AweXpectModuleInitializers>$(AweXpectModuleInitializers),MusicStore.MusicStoreSetup.Initialize</AweXpectModuleInitializers>
    </PropertyGroup>
    <ItemGroup>
        <CompilerVisibleProperty Remove="AweXpectRegisteredMetadataKeys"/>
        <CompilerVisibleProperty Include="AweXpectRegisteredMetadataKeys"/>
        <CompilerVisibleProperty Remove="AweXpectModuleInitializers"/>
        <CompilerVisibleProperty Include="AweXpectModuleInitializers"/>
        <AssemblyAttribute Include="System.Reflection.AssemblyMetadataAttribute">
            <_Parameter1>MusicStore.Catalog</_Parameter1>
            <_Parameter2>$(MSBuildProjectDirectory)</_Parameter2>
        </AssemblyAttribute>
    </ItemGroup>

</Project>
```

`ExtensionRegistry.GetValues` returns the values that are registered under a key, from every test assembly in the
process, in the order of their registration:

```csharp
namespace MusicStore
{
    public static class MusicStoreSetup
    {
        internal static List<string> Catalogs { get; } = new();

        public static void Initialize()
            => Catalogs.AddRange(ExtensionRegistry.GetValues("MusicStore.Catalog"));
    }
}
```

<details>
<summary>Projects without module initializers</summary>

- The generator runs under the same [conditions](./12-native-aot.md#the-source-generator) as the one for the type
  metadata.
- Otherwise, e.g. on .NET Framework with C# 7.3, or for a test project in F# or Visual Basic, nothing is registered
  and no initializer is called. Tell your users to call the initializer themselves, e.g. in an assembly-level setup of
  the test framework, and read the values from the loaded assemblies as a fallback, but only from those that
  [`AssemblyPrefixes.IsExcluded`](./10-customization-values.md#excluded-assemblies) does not exclude.
- One process can load assemblies of both kinds, e.g. an F# test project that calls a helper library in C#. Decide for
  each assembly whether it registered its values: let your build targets also record the assembly name, e.g. as
  `$(AssemblyName)`, register it with the values, and skip the assemblies with a registered name in the fallback.
- The registered values stay for the lifetime of the process. The list that `GetValues` returns does not change, and
  it is the same instance until another value is registered under the key, so you can keep what you derived from it.

</details>
