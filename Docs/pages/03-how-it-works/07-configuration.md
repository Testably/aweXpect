# Configuration

You can customize certain behavior or specify default values to use globally.

All customizations are located in the static `Customize.aweXpect` class. Customization values are grouped and have a dedicated `Get` and `Set` method.
The `Set` method always returns a lifetime scope which is an `IDisposable` object that will revert the customization value to its previous value upon disposal.

The customization options are applied in an [async context](https://learn.microsoft.com/en-us/dotnet/api/system.threading.asynclocal-1) which means that they don't directly influence other parallel tests:

```csharp
using aweXpect.Customization;

using (Customize.aweXpect.Formatting().MaximumStringLength.Set(500))
{
    // strings of up to 500 characters are shown in full here
}
```

## Lifetimes and async flows

A value you set is visible in the current async flow and in every flow that starts from it afterwards, such as a `Task.Run`. It never reaches a parallel flow or the flow that started the current one:

- A value set in a synchronous method is visible to its caller. A value set in an awaited `async` method is not: once that method returns, the caller continues with its own values. Set the value in the calling method, or return the lifetime from a synchronous helper method.
- Disposing a lifetime restores only the value it set, so another value of the same group that was changed in the meantime is kept. Disposing it a second time has no effect.
- `Update(…)` replaces the whole group, so disposing its lifetime restores the whole group as it was before the update. The function you pass can run again later, e.g. when a lifetime of the same group that was created before is disposed first, so it must compute the new value only from its argument and must not have side effects.
- Dispose lifetimes in the reverse order in which you created them, as nested `using` statements do, and in the same flow: disposing a lifetime restores the value in the flow that disposes it.

## Global defaults

A value set with `Set` in an assembly-level setup only reaches the tests if the test framework runs them in the async context of that setup, and most frameworks don't: in our measurements the value reached the tests for a synchronous `[AssemblyInitialize]` in MSTest, but not for an asynchronous one, in TUnit only after `context.AddAsyncLocalValues()`, in NUnit only partly, and in xUnit v2 not at all. Set such defaults on `Customize.aweXpect.Global` instead, which applies them to all async flows, in any assembly-level setup or in a module initializer:

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

- A value set in the current async flow takes precedence over the global value, so a test can still use `using (Customize.aweXpect.Formatting().MaximumStringLength.Set(20))` without influencing tests that run in parallel.
- While such a value is set in the current flow, its group is kept in this flow as a whole, with the other values taken from the global values at the time of the `Set`: a global change to another value of the same group can be hidden in this flow until all lifetimes of that group in this flow are disposed.
- Set global values once, before the tests run. They can be changed at any time and the change is visible to all running tests immediately.
- `Customize.aweXpect.Global.EnableTracing(traceWriter)` enables a trace writer for all async flows. A trace writer enabled in the current flow takes precedence.
- Groups of extension packages, such as `Customize.aweXpect.Global.Json()`, work the same way.

## Equivalency

Under `Customize.aweXpect.Equivalency()`:

| Option                      | Type                 | Default                     | Description                                                                                                                 |
|-----------------------------|----------------------|-----------------------------|-----------------------------------------------------------------------------------------------------------------------------|
| `DefaultEquivalencyOptions` | `EquivalencyOptions` | `new EquivalencyOptions()`  | The [equivalency options](../06-equivalency.md#customizing-the-global-defaults) that every expectation starts from.         |

## Formatting

Under `Customize.aweXpect.Formatting()`:

| Option                                           | Type  | Default | Description                                                                   |
|--------------------------------------------------|-------|---------|-------------------------------------------------------------------------------|
| `MaximumNumberOfCollectionItems`                 | `int` | `10`    | The maximum number of items shown for a collection.                           |
| `MaximumStringLength`                            | `int` | `100`   | The maximum length of a shown `string` before it is truncated.                |
| `MinimumNumberOfCharactersAfterStringDifference` | `int` | `45`    | The minimum number of characters shown after the first mismatch of a string. |

The items of a collection beyond the maximum are summarized at the end of the list: `(… and 7 more)` when the total
number of items is known, and `(… and maybe more)` when it is not, e.g. for a lazy sequence or when the expectation
stopped enumerating early.

## Reflection

Under `Customize.aweXpect.Reflection()`:

| Option                     | Type       | Default                                               | Description                                                                                  |
|----------------------------|------------|-------------------------------------------------------|----------------------------------------------------------------------------------------------|
| `ExcludedAssemblyPrefixes` | `string[]` | `System`, `Microsoft`, `xunit` and other known names | The assemblies that are not scanned for a test framework adapter, matched by name prefix. |

A prefix matches at a segment boundary of the assembly name, so `System` excludes `System.Net.Http`, but not an
assembly named `Systemics`.

:::note
The loaded assemblies are only scanned for a test framework adapter below .NET 8, e.g. on .NET Framework. On .NET 8 or
later the generated adapter registers itself when the test assembly is loaded, so `ExcludedAssemblyPrefixes` has no
effect there.
:::

## Settings

Under `Customize.aweXpect.Settings()`:

| Option                           | Type               | Default | Description                                                                                                   |
|----------------------------------|--------------------|---------|---------------------------------------------------------------------------------------------------------------|
| `TestCancellation`               | `TestCancellation` | none    | A [timeout or `CancellationToken`](./03-cancellation.md) that is applied to all expectations.                 |
| `DefaultCheckInterval`           | `TimeSpan`         | 100 ms  | The interval for re-checking a condition, e.g. for [`Eventually()`](../04-delegates.md#eventually).           |
| `DefaultEventuallyTimeout`       | `TimeSpan`         | 30 s    | How long [`Eventually()`](../04-delegates.md#eventually) retries until the expectations must be met.          |
| `DefaultSignalerTimeout`         | `TimeSpan`         | 30 s    | How long a [`Signaler`](./04-callbacks.md) expectation waits without `Within(…)`.                             |
| `DefaultTimeComparisonTolerance` | `TimeSpan`         | 0       | The [tolerance](../common-types/08-datetime-offset.md#default-tolerance) for date and time values without `Within(…)`. |

`TestCancellation` is created with one of:

- `TestCancellation.FromTimeout(TimeSpan timeout)`, which cancels the `CancellationToken` that is used internally and
  forwarded to the [delegates](../04-delegates.md) after the given timeout.
- `TestCancellation.FromCancellationToken(Func<CancellationToken> cancellationTokenFactory)`, which uses the returned
  `CancellationToken` internally and also forwards it to the [delegates](../04-delegates.md).

The interval must be positive, and the timeouts must not be negative. `Timeout.InfiniteTimeSpan` retries or waits
without a limit. See [default waits](./03-cancellation.md#default-waits) for which expectations use the timeouts.

:::tip
On Windows the `DateTime` resolution is [about 10 to 15 milliseconds](https://stackoverflow.com/q/3140826/4003370), so
a `DefaultTimeComparisonTolerance` of about 15 ms avoids brittle comparisons of timestamps.
:::

## Extensions

Extensions can add their own groups. For example, the
[`aweXpect.Json`](https://github.com/aweXpect/aweXpect.Json) package adds `Customize.aweXpect.Json()` with the default
[`JsonDocumentOptions`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsondocumentoptions) and
[`JsonSerializerOptions`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions). See
[write your own extension](../08-write-extension.md#customization) to add a group yourself.
