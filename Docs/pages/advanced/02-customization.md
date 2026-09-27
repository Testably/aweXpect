# Customization

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

## Equivalency

Under `Customize.aweXpect.Equivalency()`:

| Option                      | Type                 | Default                     | Description                                                                                                                 |
|-----------------------------|----------------------|-----------------------------|-----------------------------------------------------------------------------------------------------------------------------|
| `DefaultEquivalencyOptions` | `EquivalencyOptions` | `new EquivalencyOptions()`  | The [equivalency options](../06-equivalency.md#customizing-the-global-defaults) used when an expectation configures none. |

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
