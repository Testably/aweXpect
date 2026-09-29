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
disposed. When several registered formatters can format a value, the most recently registered one is used.

:::note[Older target frameworks]
`ModuleInitializerAttribute` is a compiler feature requiring C# 9, not a specific target framework. Where it is missing
(e.g. `netstandard2.0` or `net48`), declare it as an `internal` type in your own package.
:::
