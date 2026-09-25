# Cancellation

You can add cancellation support on the expectations, so that they don't run indefinitely:


## Timeout
You can set a global timeout that is applied for all expectations:
```csharp
using aweXpect.Customization;

// Sets a global timeout of 10 seconds
Customize.aweXpect.Settings().TestCancellation
    .Set(TestCancellation.FromTimeout(TimeSpan.FromSeconds(10)));
```

*Note: Like all customization options, the setter returns an `IDisposable` that will remove the cancellation on `Dispose()`.*

You can overwrite or apply the timeout also on individual expectations, using the `WithTimeout(TimeSpan)` method, e.g.
```csharp
IAsyncEnumerable<int> myEnumerable = // ...
await Expect.That(myEnumerable).All().AreEqualTo(1)
    .WithTimeout(TimeSpan.FromSeconds(10));
```

*Note: A local timeout will replace the global one and not be applied additionally.


## `CancellationToken`

You can set a global provider for getting a `CancellationToken` that is applied for all expectations:
```csharp
// Uses the CancellationToken from the test context
Customize.aweXpect.Settings().TestCancellation
    .Set(TestCancellation.FromCancellationToken(() => TestContext.Current.CancellationToken));
```

*Note: Like all customization options, the setter returns an `IDisposable` that will remove the cancellation on `Dispose()`.*

You can overwrite or apply the `CancellationToken` also on individual expectations, using the `WithCancellation(CancellationToken)` method, e.g. 
```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
IAsyncEnumerable<int> myEnumerable = // ...
await Expect.That(myEnumerable).All().AreEqualTo(1)
    .WithCancellation(cts.Token);
```

*Note: A local `CancellationToken` will replace the global one and not be applied additionally.
If necessary, provide a [linked cancellation token](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtokensource.createlinkedtokensource) yourself!*

## Awaited tasks

A timeout or a cancellation also stops waiting for a task that the expectation awaits, such as a `Task<T>` subject or
the task returned by an asynchronous delegate, even if it ignores the `CancellationToken`. A synchronous delegate
cannot be abandoned and runs to completion.

- When a **timeout** elapses, the expectation fails with "did not finish within …" and a `TimeoutException` as inner
  exception. This is the same whether the task was abandoned or reacted to the cancellation itself. With
  [`Eventually()`](/docs/expectations/delegates#eventually), the timeout bounds each evaluation in the same way.
- When the **`CancellationToken`** is cancelled, no limit is known, so the expectation is evaluated as if the task had
  been canceled: an execution time expectation fails with "was canceled after …", and `DoesNotThrow` reports the
  cancellation exception.

## Async enumerables

The `CancellationToken` of the expectation, which includes the timeout, is passed to an `IAsyncEnumerable<T>` subject,
and the expectation stops waiting for the next item once it is cancelled, even if the enumerable ignores the token.
After that, the enumerable is not advanced any further.

An expectation that needs an item after the cancellation never reads the cancellation as the end of the enumerable,
regardless of whether it occurs while waiting for an item or between two items:

- Expectations that report a cancellation as "could not be verified, because it was already canceled", like the
  quantified expectations (e.g. `All()` or `None()`), `HasCount` or `IsEmpty`, do so as well when the cancellation or
  timeout occurs during the enumeration, and list the items received so far.
- All other expectations fail with "did not finish within …" and a `TimeoutException` as inner exception when a
  **timeout** elapses, and abort with an `InvalidOperationException` whose inner exception is the
  `OperationCanceledException` when the **`CancellationToken`** is cancelled.
