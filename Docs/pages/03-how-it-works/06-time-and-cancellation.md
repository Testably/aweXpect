# Time and cancellation

Some expectations take time: they await a task or an asynchronous delegate, enumerate an `IAsyncEnumerable<T>`, wait
for a condition, an event or a callback, or measure how long a delegate runs. A timeout or a `CancellationToken` limits
how long such an expectation may take, and every expectation reports them [the same way](#outcome).

## Timeout

You can set a global timeout that is applied for all expectations, e.g. in an assembly-level setup (see
[global defaults](./07-configuration.md#global-defaults)):

```csharp
using aweXpect.Customization;

// Sets a global timeout of 10 seconds
Customize.aweXpect.Global.Settings().TestCancellation
    .Set(TestCancellation.FromTimeout(TimeSpan.FromSeconds(10)));
```

Like all customization options, the setter returns an `IDisposable` that removes the timeout again on `Dispose()`.
Without `Global`, the timeout only applies to the current async flow, e.g. to a single test.

You can also apply a timeout on individual expectations, using the `WithTimeout(TimeSpan)` method:

```csharp
IAsyncEnumerable<Track> playlist = // ...
await Expect.That(playlist).All().Satisfy(track => track.PlayCount > 0)
    .WithTimeout(TimeSpan.FromSeconds(10));
```

:::note
The tighter timeout wins. A local timeout that is longer than the global one, or than the limit of the expectation
itself (e.g. `ExecutesIn().AtMost(…)` or `Throws().Within(…)`), does not loosen it, and calling `WithTimeout` more
than once applies the shortest timeout. `Timeout.InfiniteTimeSpan` imposes no limit, and a negative timeout is
rejected when the expectation is built.
:::

## `CancellationToken`

You can set a global provider for getting a `CancellationToken` that is applied for all expectations:

```csharp
// Uses the CancellationToken from the test context
Customize.aweXpect.Global.Settings().TestCancellation
    .Set(TestCancellation.FromCancellationToken(() => TestContext.Current.CancellationToken));
```

The setter again returns an `IDisposable` that removes the provider on `Dispose()`.

:::note
The global timeout and the global `CancellationToken` are the same setting, `Settings().TestCancellation`, which holds
either a timeout or a provider: the second `Set` replaces the first. A `WithTimeout(…)` on an individual expectation
still applies together with a global `CancellationToken`.
:::

`TestCancellation` is created with one of:

- `TestCancellation.FromTimeout(TimeSpan timeout)`, which cancels the `CancellationToken` that is used internally and
  forwarded to the [delegates](../06-behaviour/01-delegates.md) after the given timeout.
- `TestCancellation.FromCancellationToken(Func<CancellationToken> cancellationTokenFactory)`, which uses the returned
  `CancellationToken` internally and also forwards it to the [delegates](../06-behaviour/01-delegates.md).
- `TestCancellation.None()`, which applies neither a timeout nor a `CancellationToken`, e.g. to switch off a global
  `TestCancellation` in the current async flow.

You can also apply a `CancellationToken` on individual expectations, using the `WithCancellation(CancellationToken)`
method:

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
IAsyncEnumerable<Track> playlist = // ...
await Expect.That(playlist).All().Satisfy(track => track.PlayCount > 0)
    .WithCancellation(cts.Token);
```

:::note
A local `CancellationToken` replaces the global one instead of being applied additionally. If you need both, pass a
[linked cancellation token](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtokensource.createlinkedtokensource).
:::

The timeout and the `CancellationToken` are also forwarded to a [delegate](../06-behaviour/01-delegates.md) that
accepts a `CancellationToken` parameter.

## Waiting

Some expectations wait for something to happen. `Within(…)` limits how long they wait, and without it they behave as
follows:

| Expectation                                                           | Without `Within(…)`                                                               |
|-----------------------------------------------------------------------|-----------------------------------------------------------------------------------|
| [`Satisfies(…)`, `CompliesWith(…)` and their negations](#a-condition) | does not wait                                                                     |
| [`Eventually()`](#eventually)                                         | retries until the expectations are met, at most `DefaultEventuallyTimeout` (30 s) |
| [`Triggered(…)`, `DidNotTrigger(…)`](#events)                         | does not wait, only the events recorded so far count                              |
| [`Signaled()`](#callbacks)                                            | waits until the callback is signaled, at most `DefaultSignalerTimeout` (30 s)     |
| [`DidNotSignal()`](#callbacks)                                        | waits the full `DefaultSignalerTimeout` (30 s), unless the callback is signaled   |

Both defaults can be changed in the [settings](./07-configuration.md#settings). When a default applied, the failure
message names the wait, e.g. `has never recorded the callback within 0:30` or `eventually is equal to 2 within 0:30`.

### A condition

When an object changes in the background, `Within(…)` lets [`Satisfies`](../04-values/12-object.md#condition) or
[`CompliesWith`](../04-values/12-object.md#nested-expectation) wait until the object meets the condition, and
`DoesNotSatisfy` or `DoesNotComplyWith` until it no longer does:

```csharp
using aweXpect.Chronology; // from the aweXpect.Chronology package

Track track = new() { IsPlayed = false };
List<Track> playlist = new();
// Start a background task that plays the track and fills the playlist

await Expect.That(track).Satisfies(x => x.IsPlayed).Within(2.Seconds());
await Expect.That(playlist).CompliesWith(x => x.HasCount().GreaterThanOrEqualTo(4)).Within(2.Seconds());
```

The condition is checked again every [`DefaultCheckInterval`](./07-configuration.md#settings) (defaults to `100ms`).
Append `CheckEvery(…)` after `Within(…)` to change the interval for a single expectation:

```csharp
using aweXpect.Chronology; // from the aweXpect.Chronology package

Track track = new();

await Expect.That(track).Satisfies(x => x.IsPlayed).Within(2.Seconds()).CheckEvery(50.Milliseconds());
```

The interval must be positive, and `Within` and `CheckEvery` can each only be specified once.

As for [`Eventually()`](#eventually), a shorter `WithTimeout` or global timeout ends the checks with "did not finish
within …", and a cancellation makes the expectation inconclusive.

### Eventually

Some values only become correct after a short delay, e.g. because a background task is still running. Instead of
waiting for a fixed amount of time, `Eventually()` re-evaluates a [delegate](../06-behaviour/01-delegates.md) until the
expectations are met. Because only the subject is re-evaluated, all expectations work as usual, including `And`, `Or`
and `Because`:

```csharp
Track track = new();
// Start a background task that plays the track

await Expect.That(() => track.PlayCount).Eventually().IsGreaterThan(5);
await Expect.That(() => track.Title).Eventually().IsNotNull().And.StartsWith("Let");
```

The delegate is re-evaluated every [`DefaultCheckInterval`](./07-configuration.md#settings) (defaults to `100ms`)
until the timeout configured in [`DefaultEventuallyTimeout`](./07-configuration.md#settings) (defaults to `30s`)
expires. You can overwrite the timeout per expectation with `Within` and the interval with `CheckEvery`, in either
order:

```csharp
using aweXpect.Chronology; // from the aweXpect.Chronology package

Track track = new();

await Expect.That(() => track.PlayCount).Eventually().Within(5.Seconds()).CheckEvery(50.Milliseconds())
  .IsGreaterThan(3)
  .WithTimeout(2.Seconds());
```

When the play count never exceeds 3, the retries end after 2 seconds, because the timeout is shorter than `Within`.

`Within(Timeout.InfiniteTimeSpan)` retries until the expectations are met or the expectation is canceled. A negative
timeout or an interval that is not positive is rejected, and each of them can only be specified once.

An exception thrown by the delegate counts as an unmet expectation and is retried. When the timeout expires while the
delegate is still throwing, the expectation fails and the last exception is reported as the cause of the failure.

<details>
<summary>How timeouts bound the retries</summary>

`WithTimeout` does not change the timeout of the retries, but cancels the evaluation like everywhere else, so the
tighter timeout wins: a `WithTimeout` or a global `TestCancellation.FromTimeout` that is shorter than `Within` ends the
retries and fails the expectation with "did not finish within …", and a longer one does not extend them. A
cancellation before the timeout expires (via `WithCancellation` or `TestCancellation.FromCancellationToken`) makes the
expectation inconclusive instead of failed.

The timeout also bounds each evaluation: an evaluation that is still running when the timeout is used up is abandoned,
even if the delegate ignores its `CancellationToken`, which is canceled at that point, and the expectation fails with
"did not finish within …" and a `TimeoutException` as inner exception. The last evaluation, which is made when the
timeout is used up, still gets one check interval (at most the timeout) to finish. An expectation that waits while it
checks the value, e.g. for the next item of an `IAsyncEnumerable<T>`, is canceled in the same way and fails with the
same message. A synchronous delegate cannot be
interrupted, so an evaluation that returns after that point fails the same way, whatever its result. Only
`Within(TimeSpan.Zero)`, which makes a single evaluation, does not bound it, for a synchronous and an asynchronous
delegate alike; a `WithTimeout` or a global timeout still does.

</details>

In addition to `Func<T>`, the asynchronous variant `Func<Task<T>>` is supported, and
[on .NET 8 or later](../02-getting-started.md#target-frameworks) also `Func<ValueTask<T>>`; each of them also accepts a
`CancellationToken`. Returning the task directly also works with a
language version older than C# 13 (see the [delegates](../06-behaviour/01-delegates.md) page):

```csharp
Func<Task<int>> loadPlayCountAsync = () => Task.FromResult(6);

await Expect.That(() => loadPlayCountAsync()).Eventually().IsGreaterThan(5);
```

### Events

`Within(…)` waits up to the given time for the expected [events](../06-behaviour/03-events.md) and finishes
successfully as soon as they are recorded:

```csharp
using aweXpect.Chronology; // from the aweXpect.Chronology package
using aweXpect.Recording;

class Player
{
  public event EventHandler? TrackStarted;
  public void Play(string title) => TrackStarted?.Invoke(this, EventArgs.Empty);
}
Player player = new();
IEventRecording<Player> recording = player.Record().Events();

_ = Task.Delay(1.Seconds()).ContinueWith(_ => player.Play("Let It Be"));

await Expect.That(recording).Triggered(nameof(Player.TrackStarted)).Within(3.Seconds());
```

Expectations with an upper bound (`DidNotTrigger`, `Never()`, `AtMost(…)`, `Exactly(…)`) wait out the whole timeout
and only return early when one event too many is recorded:

```csharp
IEventRecording<Player> recording = player.Record().Events();

// Waits for 3 seconds and expects that no TrackStarted event is triggered in that time
await Expect.That(recording).DidNotTrigger(nameof(Player.TrackStarted)).Within(3.Seconds());
```

### Callbacks

`Within(…)` limits how long to wait for a [callback](../06-behaviour/04-callbacks.md) to be signaled:

```csharp
using aweXpect.Signaling;

Signaler<string> signaler = new();

await Expect.That(signaler).Signaled().Within(TimeSpan.FromSeconds(5))
  .Because("it should take at most 5 seconds to complete");
```

An expectation without an upper bound (e.g. `AtLeast`) succeeds as soon as enough callbacks were signaled.
Expectations with an upper bound, including `DidNotSignal()`, wait out the whole timeout and only return early when one
signal too many is received. A canceled `CancellationToken` makes the expectation [inconclusive](#outcome), so use
`Within(…)` to limit how long to wait.

## Execution time

[`ExecutesIn()`](../06-behaviour/01-delegates.md#execution-time) applies its upper bound as timeout: the maximum of
`AtMost`, the end of the `Between` range, or the expected time plus the tolerance. A tighter timeout, e.g. from
`WithTimeout(…)`, still applies. A delegate that accepts a `CancellationToken` is canceled once the upper bound elapsed,
and the expectation fails with "did not finish within …" instead of hanging. `AtLeast` has no upper bound and
therefore applies no timeout. The duration of `Throws().Within(…)` is applied as timeout the same way. A
[task](../06-behaviour/02-tasks.md) is already running when the expectation receives it, so only the duration that
remains is measured.

<details>
<summary>Delegates that ignore the timeout</summary>

The task of an asynchronous delegate is abandoned once the timeout elapsed, even if the delegate ignores or does not
accept a `CancellationToken`, and the expectation fails the same way. A synchronous delegate cannot be interrupted and
runs to completion, however long that takes; neither `WithTimeout` nor `WithCancellation` changes that. If it returns
after the timeout elapsed, the expectation fails the same way. `ExecutesIn()` and `Throws().Within(…)` report the
duration it took instead, when the timeout is their own upper bound or the duration also violates their limit.

</details>

## Outcome

A timeout and a cancellation are reported the same way by every expectation, whether it awaits a `Task<T>` subject, an
asynchronous `Whose` member or delegate, enumerates an `IAsyncEnumerable<T>`, waits for a `Signaler` or for recorded
events, or retries with `Within(…)` or [`Eventually()`](#eventually):

- When a **timeout** elapses (`WithTimeout` or `TestCancellation.FromTimeout`), the expectation fails with "did not
  finish within …" and a `TimeoutException` as inner exception.
- When the **`CancellationToken`** is canceled (`WithCancellation` or `TestCancellation.FromCancellationToken`), the
  expectation is inconclusive: "could not be verified, because the evaluation was already canceled". It neither passes
  nor throws the `OperationCanceledException`, so e.g. `DidNotSignal()` does not pass because the cancellation ended the
  wait.
- An `OperationCanceledException` that a delegate throws for its own reasons, while neither the timeout elapsed nor the
  `CancellationToken` was canceled, is an ordinary exception: e.g. `DoesNotThrow()` fails with "did throw an
  OperationCanceledException".

An inconclusive expectation throws the exception that your test framework uses for this outcome, see
[inconclusive](./09-test-frameworks.md#inconclusive).

## Awaited tasks

A timeout or a cancellation also stops waiting for a task that the expectation awaits, such as a `Task<T>` subject or
the task returned by an asynchronous delegate, even if it ignores the `CancellationToken`. A synchronous delegate
cannot be abandoned and runs to completion. The outcome is the same whether the task was abandoned or reacted to the
cancellation itself. With [`Eventually()`](#eventually), the timeout bounds each evaluation in the same way.

## Async enumerables

The `CancellationToken` of the expectation, which includes the timeout, is passed to an `IAsyncEnumerable<T>` subject,
and the expectation stops waiting for the next item once it is canceled, even if the enumerable ignores the token.
After that, the enumerable is not advanced any further.

<details>
<summary>Partial enumerations, disposal and complete collections</summary>

An expectation that needs an item after the cancellation never reads the cancellation as the end of the enumerable,
regardless of whether it occurs while waiting for an item or between two items. Expectations like `HasCount` or
`IsEmpty` list the items received so far.

The enumerator is disposed when the expectation ends. A timeout or a cancellation also stops waiting for a
`DisposeAsync` that does not complete. The outcome is already decided at that point and does not change: a met
expectation stays met, and a failure is reported as it is.

A timeout or a cancellation only stops reading the items that still have to come from a collection, e.g. from a lazily
evaluated `IEnumerable<T>`. A collection that is already complete in memory, such as an array or a `List<T>`, is
evaluated completely like any other value, even when the timeout elapsed while a delegate created it. The same applies
to a collection whose items an earlier expectation on the same subject has already read to the end.

</details>
