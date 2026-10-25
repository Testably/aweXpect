# Initialization

An extension often has to run code once before its first expectation is evaluated, e.g. to register a custom value
formatter. Use a
[module initializer](https://learn.microsoft.com/dotnet/csharp/language-reference/proposals/csharp-9.0/module-initializers),
which runs before any other code of your assembly, i.e. when a test first uses the extension. In a class library, the
attribute triggers the warning CA2255, so suppress it as shown:

```csharp
using System.Runtime.CompilerServices;
using aweXpect.Formatting;

namespace MyExtension
{
    internal static class MyExtensionInitializer
    {
#pragma warning disable CA2255 // The initializer of a class library is intended here
        [ModuleInitializer]
        internal static void Initialize()
            => ValueFormatter.Register(new MyValueFormatter());
#pragma warning restore CA2255
    }
}
```

## Value formatters

A value formatter returns `false` for every value it does not format, so that the next formatter or the built-in
formatting applies. The `MyValueFormatter` registered above writes a `Track` with its title and duration:

```csharp
using System.Text;

internal sealed class MyValueFormatter : IValueFormatter
{
    public bool TryFormat(StringBuilder stringBuilder, object value, FormattingOptions? options)
    {
        if (value is not Track track)
        {
            return false;
        }

        stringBuilder.Append(track.Title).Append(" (");
        Format.Formatter.Format(stringBuilder, track.Duration);
        stringBuilder.Append(')');
        return true;
    }
}
```

A registered formatter applies process-wide, to all threads and async flows, until the returned `IDisposable` is
disposed. It is asked for every value that is not `null`, including built-in types such as `DateTime` or `int`,
whether the value is formatted on its own, as an item of a collection or as a member of an object. When several
registered formatters can format a value, the most recently registered one is used.

Register a formatter only for the types that your extension declares. The module initializer runs before any of them
is used, so their messages never depend on the order of the tests. A formatter for a type that your extension does not
own, e.g. `System.Version`, would only apply to the tests that run after the first test that used your extension, and
would change how other tests format it. Add such a formatter to your expectations instead.

:::note[Older target frameworks]
`ModuleInitializerAttribute` is a compiler feature requiring C# 9, not a specific target framework. Where it is missing
(e.g. `netstandard2.0` or `net48`), declare it as an `internal` type in your own package.
:::

### Formatters of an expectation

`ExpectationBuilder.AddFormatter` formats the values of one expectation with a formatter, before the registered ones.
It applies while the expectation is evaluated and while its failure message is created, also to the expectations
chained to it, e.g. collection expectations on the items of a member, but never outside of the expectation:

```csharp
using System.Text;
using aweXpect.Core;
using aweXpect.Formatting;

internal static class MyExtensionFormatting
{
    private static readonly IValueFormatter VersionFormatter = new MyVersionFormatter();

    internal static ExpectationBuilder WithVersionFormatter(this ExpectationBuilder expectationBuilder)
        => expectationBuilder.AddFormatter(VersionFormatter);

    private sealed class MyVersionFormatter : IValueFormatter
    {
        public bool TryFormat(StringBuilder stringBuilder, object value, FormattingOptions? options)
        {
            if (value is not Version version)
            {
                return false;
            }

            stringBuilder.Append('v').Append(version.ToString(3));
            return true;
        }
    }
}
```

When the expectation is nested in another one, e.g. with `ComplyWith`, or combined with others in `Expect.ThatAll` or
`Expect.ThatAny`, the whole failure message uses the formatter. Pass a cached instance, as adding the same formatter
again has no effect.

## Test framework adapter

aweXpect reports a failed, skipped or inconclusive expectation by throwing the exception of the test framework. For
MSTest, NUnit, TUnit and xUnit (v2 and v3), the `aweXpect` package generates an adapter into the test project and
registers it ([test frameworks](../03-how-it-works/09-test-frameworks.md) lists the exceptions it throws). To support
another test framework, implement `ITestFrameworkAdapter` and register it with
`TestFrameworkRegistry.Register` from a module initializer in the test project itself, as the generated adapters are.
A module initializer only runs when its assembly is first used, and the tests never use an assembly that only contains
an adapter, so a registration in a separate package would never run:

```csharp
using System.Runtime.CompilerServices;
using aweXpect.Core.Adapters;

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
  `ITestFrameworkAdapter`, see [settings](../03-how-it-works/07-configuration.md#settings).
- The generated registration is a module initializer, which needs C# 9 or later. On .NET 8 or later, a test project
  with an older `<LangVersion>` has to register the generated adapter itself, which the warning
  [aweXpect2002](../08-analyzers.md#test-framework-adapter) points out.

</details>

## Registration from the test project

Some extensions need values that the build of the test project records, e.g. its directory, or have to run code
before the first test, e.g. to add a reporter, although no test uses their assembly before. Instead of scanning the
loaded assemblies, let the source generator of `aweXpect.Core` emit a module initializer into the test project. The
build targets of your package (in its `buildTransitive` folder) only pass names to it, in two MSBuild properties:

- `AweXpectRegisteredMetadataKeys`: the keys of the `AssemblyMetadataAttribute`s of the test project whose values
  are registered in `ExtensionRegistry`.
- `AweXpectModuleInitializers`: the public static methods without parameters that are called afterwards, so that
  they can already read the registered values.

Both are lists separated by `,`, as the compiler receives them in an `.editorconfig`, where a `;` starts a comment.
Append your names to the value that other packages set, and make the property visible to the generator:

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
using aweXpect.Core.Extending;

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

- The generator runs under the same conditions as the one for the [type metadata](./09-native-aot.md): the project
  gets the analyzers of `aweXpect.Core`, is compiled with C# 9 or later and has the `ModuleInitializerAttribute`
  (.NET 5 or later, or an own `internal` declaration).
- Otherwise, e.g. on .NET Framework with C# 7.3, nothing is registered and no initializer is called. Tell your users
  to call the initializer themselves, e.g. in an assembly-level setup of the test framework, and read the values from
  the loaded assemblies as a fallback, but only from those that
  [`AssemblyPrefixes.IsExcluded`](./07-customization-values.md#exclude-the-framework-assemblies) does not exclude.
  A value that your build targets always record tells whether the test project registered its values.
- The registered values stay for the lifetime of the process. The list that `GetValues` returns does not change, and
  it is the same instance until another value is registered under the key, so you can keep what you derived from it.

</details>
