---
description: Customize the behavior and the default values of aweXpect for a single test or globally.
---

# Configuration

You can customize certain behavior or specify default values to use globally.

All customizations are located in the static `Customize.aweXpect` class. Groups such as `Formatting()` bundle related
customization values, and each value is stored on its own with a dedicated `Get` and `Set` method. The `Set` method
always returns a lifetime scope, an `IDisposable` that removes the value it set upon disposal.

The customization options are applied in an
[async context](https://learn.microsoft.com/en-us/dotnet/api/system.threading.asynclocal-1), so they don't directly
influence other parallel tests:

```csharp
using aweXpect.Customization;

using (Customize.aweXpect.Formatting().MaximumStringLength.Set(500))
{
    // strings of up to 500 characters are shown in full here
}
```

## Lifetimes and async flows

A value you set is visible in the current async flow and in every flow that starts from it afterwards, such as a
`Task.Run`. It never reaches a parallel flow or the flow that started the current one.

<details>
<summary>Lifetimes in async methods and out-of-order disposal</summary>

- A value set in a synchronous method is visible to its caller. A value set in an awaited `async` method is not: once
  that method returns, the caller continues with its own values. Set the value in the calling method, or return the
  lifetime from a synchronous helper method.
- Disposing a lifetime removes only the value it set, also when the lifetimes are disposed in a different order than
  they were created: another value, or a later value of the same setting, stays in effect until its own lifetime is
  disposed. Disposing a lifetime a second time has no effect.
- Dispose a lifetime in the flow that created it: disposing a lifetime restores the value in the flow that disposes it.

</details>

## Global defaults

Most test frameworks don't run the tests in the async context of an assembly-level setup, so a value set there with
`Set` usually does not reach the tests. Set such defaults on `Customize.aweXpect.Global` instead, which applies them to
all async flows, in any assembly-level setup or in a module initializer:

```csharp
using System.Runtime.CompilerServices;
using aweXpect.Customization;

internal static class AwexpectDefaults
{
    [ModuleInitializer]
    internal static void Initialize()
        => Customize.aweXpect.Global.Formatting().MaximumStringLength.Set(500);
}
```

- A value set in the current async flow takes precedence over its global value, so a test can still use
  `using (Customize.aweXpect.Formatting().MaximumStringLength.Set(20))` without influencing tests that run in parallel.
  All other values keep following the global values, and once the lifetimes of the value in the current flow are
  disposed, it follows the global value again.
- Set global values once, before the tests run. They can be changed at any time and the change is visible to all
  running tests immediately.
- `Customize.aweXpect.Global.EnableTracing(traceWriter)` enables a trace writer for all async flows. A trace writer
  enabled in the current flow takes precedence.
- Groups of extension packages, such as `Customize.aweXpect.Global.Json()`, work the same way.

## Equivalency

Under `Customize.aweXpect.Equivalency()`:

| Option                      | Type                 | Default                    | Description                                                                                                                   |
|-----------------------------|----------------------|----------------------------|-------------------------------------------------------------------------------------------------------------------------------|
| `DefaultEquivalencyOptions` | `EquivalencyOptions` | `new EquivalencyOptions()` | The [equivalency options](../04-values/13-equivalency.md#customizing-the-global-defaults) that every expectation starts from. |

## Formatting

Under `Customize.aweXpect.Formatting()`:

| Option                                           | Type  | Default | Description                                                                  |
|--------------------------------------------------|-------|---------|------------------------------------------------------------------------------|
| `MaximumNumberOfCollectionItems`                 | `int` | `10`    | The maximum number of items shown for a collection.                          |
| `MaximumStringLength`                            | `int` | `100`   | The maximum length of a shown `string` before it is truncated.               |
| `MinimumNumberOfCharactersAfterStringDifference` | `int` | `45`    | The minimum number of characters shown after the first mismatch of a string. |

`MaximumStringLength` applies to formatted values in general, e.g. a `string` item of a collection or the message of
an exception. String expectations always shorten the expected and actual values in the first lines of their failure
message to 30 characters, so that these lines stay readable. The difference below them shows the details, e.g. at
least `MinimumNumberOfCharactersAfterStringDifference` characters after the first mismatch.

The items of a collection beyond the maximum are summarized at the end of the list: `(… and 7 more)` when the total
number of items is known, and `(… and maybe more)` when it is not, e.g. for a lazy sequence or when the expectation
stopped enumerating early.

To format your own types, register an `IValueFormatter` with `ValueFormatter.Register(…)`, which returns an
`IDisposable` that removes it again, see [value formatters](../11-extending/04-formatting.md#value-formatters).

<details>
<summary>Multi-dimensional arrays</summary>

For a multi-dimensional array the maximum number of collection items applies to the items of all dimensions together.

</details>

## Settings

Under `Customize.aweXpect.Settings()`:

| Option                           | Type                    | Default   | Description                                                                                                                      |
|----------------------------------|-------------------------|-----------|----------------------------------------------------------------------------------------------------------------------------------|
| `TestCancellation`               | `TestCancellation`      | none      | A [timeout or `CancellationToken`](./06-time-and-cancellation.md) that is applied to all expectations.          |
| `DefaultCheckInterval`           | `TimeSpan`              | 100 ms    | The interval for re-checking a condition, e.g. for [`Eventually()`](./06-time-and-cancellation.md#eventually).  |
| `DefaultEventuallyTimeout`       | `TimeSpan`              | 30 s      | How long [`Eventually()`](./06-time-and-cancellation.md#eventually) retries until the expectations must be met. |
| `DefaultRegexTimeout`            | `TimeSpan`              | 1 s       | How long a [wildcard or regex pattern](../04-values/03-string.md#match-types) may take to match.                                 |
| `DefaultSignalerTimeout`         | `TimeSpan`              | 30 s      | How long a [`Signaler`](../06-behavior/04-callbacks.md) expectation waits without `Within(…)`.                                  |
| `DefaultTimeComparisonTolerance` | `TimeSpan`              | 0         | The [tolerance](./05-options.md#default-tolerance) for date and time values without `Within(…)`.                                 |
| `ExcludedAssemblyPrefixes`       | `IReadOnlyList<string>` | see below | The assemblies that are not scanned for a test framework adapter, matched by name prefix.                                        |

See [time and cancellation](./06-time-and-cancellation.md#cancellationtoken) for how to create a
`TestCancellation`.

`Timeout.InfiniteTimeSpan` retries or waits without a limit. See
[waiting](./06-time-and-cancellation.md#waiting) for which expectations use the timeouts.

The `DefaultRegexTimeout` protects against catastrophic backtracking. `Regex.InfiniteMatchTimeout` switches it off. A
scope can allow more time for a large subject on a slow build server:

```csharp
using aweXpect.Customization;

using (Customize.aweXpect.Settings().DefaultRegexTimeout.Set(TimeSpan.FromSeconds(5)))
{
    // a wildcard or regex pattern may take up to five seconds to match here
}
```

:::tip
On Windows the `DateTime` resolution is [about 10 to 15 milliseconds](https://stackoverflow.com/q/3140826/4003370), so
a `DefaultTimeComparisonTolerance` of about 15 ms avoids brittle comparisons of timestamps.
:::

`ExcludedAssemblyPrefixes` defaults to `mscorlib`, `System`, `Microsoft`, `netstandard`, `WindowsBase`, `JetBrains`,
`xunit`, `TUnit`, `Castle` and `DynamicProxyGenAssembly2`. Extensions such as aweXpect.Reflection use the same
prefixes.

<details>
<summary>How the prefixes are matched and when they are used</summary>

A prefix matches at a segment boundary of the assembly name, so `System` excludes `System.Net.Http`, but not an
assembly named `Systemics`. A prefix that ends with a `.`, e.g. `MyCompany.`, excludes all assemblies whose name starts
with it, and an empty prefix excludes nothing. Extensions use the same matching, which
[`AssemblyPrefixes.IsExcluded`](../11-extending/10-customization-values.md#excluded-assemblies) offers to
your own extension.

The test framework detection only uses `ExcludedAssemblyPrefixes` below .NET 8, see
[test frameworks](../03-setup/01-test-frameworks.md#failed-expectations).

</details>

## Tracing

`Customize.aweXpect.EnableTracing(traceWriter)` writes which subjects are checked, which expectations were met and
which exceptions were thrown to an `ITraceWriter`. Like any other value, it returns a lifetime that disables the trace
writer again, and on `Customize.aweXpect.Global` it applies to all async flows:

```csharp
using aweXpect.Customization;

using (Customize.aweXpect.EnableTracing(new ConsoleTraceWriter()))
{
    await That(42).IsGreaterThan(40);
}

Customize.aweXpect.Global.EnableTracing(new ConsoleTraceWriter());

class ConsoleTraceWriter : ITraceWriter
{
    public void WriteMessage(string message) => Console.WriteLine(message);
    public void WriteException(Exception exception) => Console.WriteLine(exception);
}
```

## Extensions

Extensions can add their own groups. For example, the
[`aweXpect.Json`](/aweXpect/extensions/aweXpect.Json) package adds `Customize.aweXpect.Json()` with the default
[`JsonDocumentOptions`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsondocumentoptions) and
[`JsonSerializerOptions`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions). See
[customization values](../11-extending/10-customization-values.md) to add a group yourself.
