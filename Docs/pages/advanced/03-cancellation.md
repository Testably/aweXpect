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
the task returned by an asynchronous delegate, even if it ignores the `CancellationToken`. The task is abandoned and
the expectation is evaluated as if the task had been canceled. A synchronous delegate cannot be abandoned and runs to
completion.
