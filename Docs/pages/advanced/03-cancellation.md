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

You can also apply a timeout on individual expectations, using the `WithTimeout(TimeSpan)` method:
```csharp
IAsyncEnumerable<int> myEnumerable = // ...
await Expect.That(myEnumerable).All().AreEqualTo(1)
    .WithTimeout(TimeSpan.FromSeconds(10));
```

*Note: The tighter timeout wins. A local timeout that is longer than the global one, or than the limit of the
expectation itself (e.g. `ExecutesWithin` or `Throws().Within`), does not loosen it, and calling `WithTimeout` more
than once applies the shortest timeout. `Timeout.InfiniteTimeSpan` imposes no limit, and a negative timeout is
rejected when the expectation is built.*


## `CancellationToken`

You can set a global provider for getting a `CancellationToken` that is applied for all expectations:
```csharp
// Uses the CancellationToken from the test context
Customize.aweXpect.Settings().TestCancellation
    .Set(TestCancellation.FromCancellationToken(() => TestContext.Current.CancellationToken));
```

*Note: Like all customization options, the setter returns an `IDisposable` that will remove the cancellation on `Dispose()`.*

You can overwrite or apply the `CancellationToken` also on individual expectations, using the `WithCancellation(CancellationToken)` method:
```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
IAsyncEnumerable<int> myEnumerable = // ...
await Expect.That(myEnumerable).All().AreEqualTo(1)
    .WithCancellation(cts.Token);
```

*Note: A local `CancellationToken` will replace the global one and not be applied additionally.
If necessary, provide a [linked cancellation token](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtokensource.createlinkedtokensource) yourself!*

## Outcome

A timeout and a cancellation are reported the same way by every expectation, whether it awaits a `Task<T>` subject, an
asynchronous `Whose` member or delegate, enumerates an `IAsyncEnumerable<T>`, waits for a `Signaler` or for recorded
events, or retries with `Within(…)` or [`Eventually()`](/docs/expectations/delegates#eventually):

- When a **timeout** elapses (`WithTimeout` or `TestCancellation.FromTimeout`), the expectation fails with "did not
  finish within …" and a `TimeoutException` as inner exception.
- When the **`CancellationToken`** is canceled (`WithCancellation` or `TestCancellation.FromCancellationToken`), the
  expectation is inconclusive: "could not be verified, because it was already canceled". It neither passes nor throws
  the `OperationCanceledException`, so e.g. `DidNotSignal()` does not pass because the cancellation ended the wait.
- An `OperationCanceledException` that a delegate throws for its own reasons, while neither the timeout elapsed nor the
  `CancellationToken` was canceled, is an ordinary exception: e.g. `DoesNotThrow()` fails with "did throw an
  OperationCanceledException".

## Awaited tasks

A timeout or a cancellation also stops waiting for a task that the expectation awaits, such as a `Task<T>` subject or
the task returned by an asynchronous delegate, even if it ignores the `CancellationToken`. A synchronous delegate
cannot be abandoned and runs to completion. The outcome is the same whether the task was abandoned or reacted to the
cancellation itself. With [`Eventually()`](/docs/expectations/delegates#eventually), the timeout bounds each evaluation
in the same way.

## Async enumerables

The `CancellationToken` of the expectation, which includes the timeout, is passed to an `IAsyncEnumerable<T>` subject,
and the expectation stops waiting for the next item once it is canceled, even if the enumerable ignores the token.
After that, the enumerable is not advanced any further.

An expectation that needs an item after the cancellation never reads the cancellation as the end of the enumerable,
regardless of whether it occurs while waiting for an item or between two items. Expectations like `HasCount` or
`IsEmpty` list the items received so far.
