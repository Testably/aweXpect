# Initialization

An extension often has to run code once before the first expectation is evaluated, e.g. to register a custom value
formatter. Use a
[module initializer](https://learn.microsoft.com/dotnet/csharp/language-reference/proposals/csharp-9.0/module-initializers),
which runs when your assembly is loaded and before any other code in it:

```csharp
using System.Runtime.CompilerServices;
using aweXpect.Formatting;

namespace MyExtension
{
    internal static class MyExtensionInitializer
    {
        [ModuleInitializer]
        internal static void Initialize()
            => ValueFormatter.Register(new MyValueFormatter());
    }
}
```

A registered formatter applies process-wide, to all threads and async flows, until the returned `IDisposable` is
disposed. It is asked for every value that is not `null`, including built-in types such as `DateTime` or `int`,
whether the value is formatted on its own, as an item of a collection or as a member of an object. When several
registered formatters can format a value, the most recently registered one is used.

:::note[Older target frameworks]
`ModuleInitializerAttribute` is a compiler feature requiring C# 9, not a specific target framework. Where it is missing
(e.g. `netstandard2.0` or `net48`), declare it as an `internal` type in your own package.
:::

## Test framework adapter

aweXpect reports a failed, skipped or inconclusive expectation by throwing the exception of the test framework. For
MSTest, NUnit, TUnit and xUnit (v2 and v3), the `aweXpect` package generates an adapter into the test project and
registers it. To support another test framework, implement `ITestFrameworkAdapter` and register it with
`TestFrameworkRegistry.Register` from a module initializer:

```csharp
using System.Runtime.CompilerServices;
using aweXpect.Core.Adapters;

namespace MyExtension
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
        [ModuleInitializer]
        internal static void Register()
            => TestFrameworkRegistry.Register(new MyTestFrameworkAdapter());
    }
}
```

- The adapter is resolved once, when the first expectation is evaluated, so register it before that.
- An adapter whose `IsAvailable` is `false` is ignored, e.g. when the test framework it throws for is not loaded.
- By default, `Register` replaces an adapter that was registered before. With `overwrite: false`, as the generated
  registrations use it, the adapter is only used when no other one was registered, so an explicit registration always
  wins, independent of the order in which the module initializers run.
- Without any registration, aweXpect scans the loaded assemblies for a type that implements `ITestFrameworkAdapter`,
  which does not work when the tests are published with trimming or Native AOT enabled. If it finds none, it throws its
  own `FailException`, `SkipException` or `InconclusiveException`.
