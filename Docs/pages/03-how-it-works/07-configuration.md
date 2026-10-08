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
stopped enumerating early. For a multi-dimensional array the maximum applies to the items of all dimensions together.

The maximum number of collection items must be positive, and the other two values must not be negative.

## Reflection

<details>
<summary>Assemblies scanned for a test framework adapter below .NET 8</summary>

Under `Customize.aweXpect.Reflection()`:

| Option                     | Type       | Default                                              | Description                                                                               |
|----------------------------|------------|------------------------------------------------------|-------------------------------------------------------------------------------------------|
| `ExcludedAssemblyPrefixes` | `string[]` | `mscorlib`, `System`, `Microsoft`, `netstandard`, `WindowsBase`, `JetBrains`, `xunit`, `Castle`, `DynamicProxyGenAssembly2` | The assemblies that are not scanned for a test framework adapter, matched by name prefix. |

A prefix matches at a segment boundary of the assembly name, so `System` excludes `System.Net.Http`, but not an
assembly named `Systemics`.

:::note
The loaded assemblies are only scanned for a test framework adapter below .NET 8, e.g. on .NET Framework. On .NET 8 or
later the generated adapter registers itself when the test assembly is loaded, so `ExcludedAssemblyPrefixes` has no
effect there. This registration needs C# 9 or later, see
[aweXpect2002](../07-analyzers.md#test-framework-adapter).
:::

</details>

## Settings

Under `Customize.aweXpect.Settings()`:

| Option                           | Type               | Default | Description                                                                                                         |
|----------------------------------|--------------------|---------|---------------------------------------------------------------------------------------------------------------------|
| `TestCancellation`               | `TestCancellation` | none    | A [timeout or `CancellationToken`](./06-time-and-cancellation.md) that is applied to all expectations.              |
| `DefaultCheckInterval`           | `TimeSpan`         | 100 ms  | The interval for re-checking a condition, e.g. for [`Eventually()`](./06-time-and-cancellation.md#eventually).      |
| `DefaultEventuallyTimeout`       | `TimeSpan`         | 30 s    | How long [`Eventually()`](./06-time-and-cancellation.md#eventually) retries until the expectations must be met.     |
| `DefaultSignalerTimeout`         | `TimeSpan`         | 30 s    | How long a [`Signaler`](../06-behaviour/04-callbacks.md) expectation waits without `Within(…)`.                     |
| `DefaultTimeComparisonTolerance` | `TimeSpan`         | 0       | The [tolerance](../04-values/10-datetime-offset.md#default-tolerance) for date and time values without `Within(…)`. |

See [time and cancellation](./06-time-and-cancellation.md#cancellationtoken) for how to create a `TestCancellation`.

The interval must be positive, and the timeouts must not be negative. `Timeout.InfiniteTimeSpan` retries or waits
without a limit. See [waiting](./06-time-and-cancellation.md#waiting) for which expectations use the timeouts.

:::tip
On Windows the `DateTime` resolution is [about 10 to 15 milliseconds](https://stackoverflow.com/q/3140826/4003370), so
a `DefaultTimeComparisonTolerance` of about 15 ms avoids brittle comparisons of timestamps.
:::

## Extensions

Extensions can add their own groups. For example, the
[`aweXpect.Json`](https://github.com/aweXpect/aweXpect.Json) package adds `Customize.aweXpect.Json()` with the default
[`JsonDocumentOptions`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsondocumentoptions) and
[`JsonSerializerOptions`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions). See
[customization values](../11-extending/07-customization-values.md) to add a group yourself.
