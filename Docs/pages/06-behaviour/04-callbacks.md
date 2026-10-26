# Callbacks

Describes the possible expectations for working with callbacks.

| Expectation                                                          | Negated        | Summary                                            |
|----------------------------------------------------------------------|----------------|----------------------------------------------------|
| [`Signaled`](#signaler)                                              | `DidNotSignal` | the callback was signaled (a number of times)      |
| [`Wait`, `WaitAsync`, `IsSignaled`](#waiting-without-an-expectation) |                | waits for or checks signals without an expectation |

## Signaler

First, you have to start recording callback signals using the `Signaler` class. This class is available in the
`aweXpect.Signaling` namespace:

```csharp
using aweXpect.Signaling;

// ↓ Counts signals from callbacks without parameters
Signaler signaler = new();
Signaler<string> titleSignaler = new();
// ↑ Counts signals from callbacks with a string parameter
```

Then, you can signal the callback on the recording:

```csharp
class Player
{
  public void Play(string title, Action<string> onCompleted)
  {
    // play the track in a background thread and then call the onCompleted callback
  }
}

player.Play("Let It Be", title => titleSignaler.Signal(title));
```

At last, you can wait for the callback to be signaled:

```csharp
await Expect.That(titleSignaler).Signaled();
```

You can also verify that the callback will not be signaled:

```csharp
await Expect.That(titleSignaler).DidNotSignal();
```

:::note
Without `Within(…)`, a signaler expectation waits up to the
[`DefaultSignalerTimeout`](../03-how-it-works/07-configuration.md#settings) (30 s). `DidNotSignal()` always waits the
full timeout to succeed, see [waiting for callbacks](../03-how-it-works/06-time-and-cancellation.md#callbacks).
:::

### Amount

You can specify how often a callback must be signaled:

```csharp
await Expect.That(signaler).Signaled().AtLeast(3);
await Expect.That(signaler).Signaled().Exactly(3);
await Expect.That(signaler).Signaled().AtMost(3);
await Expect.That(signaler).Signaled().MoreThan(2);
await Expect.That(signaler).Signaled().LessThan(3);
await Expect.That(signaler).Signaled().Between(2).And(4);
await Expect.That(signaler).Signaled().Once();
await Expect.That(signaler).Signaled().Twice();
await Expect.That(signaler).Signaled().Never();
```

`Signaled(3)` is the shorthand for `Signaled().AtLeast(3)`, and `DidNotSignal(3)` expects the callback to be
signaled fewer than three times.

### Parameters

You can filter for signals with specific parameters by providing a `predicate`:

```csharp
Signaler<string> signaler = new();

signaler.Signal("Yesterday");
signaler.Signal("Let It Be");
signaler.Signal("Yesterday");

await Expect.That(signaler).Signaled().AtLeast(2).With(p => p == "Yesterday");
```

`WhoseParameters` continues with expectations on the collection of recorded parameters that match the filter from
`With`:

```csharp
Signaler<string> signaler = new();

signaler.Signal("Yesterday");
signaler.Signal("Let It Be");

await Expect.That(signaler).Signaled().AtLeast(2).WhoseParameters.Contains("Let It Be");
```

A failure message lists the recorded parameters.

## Waiting without an expectation

A signaler can also be used directly, e.g. to continue a test only after a callback was signaled. `WaitAsync` waits
without blocking a thread, until the callback was signaled or the timeout expired:

```csharp
Signaler<string> signaler = new();
player.Play("Let It Be", title => signaler.Signal(title));

SignalerResult<string> result = await signaler.WaitAsync(timeout: TimeSpan.FromSeconds(5));

string completed = string.Join(", ", result.Parameters);
Fail.Unless(result.IsSuccess, $"only {result.Count} tracks completed: {completed}");
```

`Wait` takes the same parameters, but blocks the calling thread while it waits, and `IsSignaled` returns immediately:

```csharp
Signaler<string> signaler = new();

bool isSignaled = signaler.IsSignaled(2);
SignalerResult<string> result = signaler.Wait(2, title => title.StartsWith("Let"));
```

All parameters are optional and have the same meaning for `Wait` and `WaitAsync`:

| Parameter           | Without it                                                                                               |
|---------------------|----------------------------------------------------------------------------------------------------------|
| `amount`            | waits for one signal; `IsSignaled` checks for at least one signal                                        |
| `predicate`         | counts every signal; only a `Signaler<T>` has it, to count the signals that match                        |
| `timeout`           | waits for at most the [`DefaultSignalerTimeout`](../03-how-it-works/07-configuration.md#settings) (30 s) |
| `cancellationToken` | only the timeout ends the wait                                                                           |

Signals that were received before the wait count as well, so a callback that was already signaled is not missed.

<details>
<summary>Details of waiting</summary>

- A `timeout` of `TimeSpan.Zero` does not wait at all, and `Timeout.InfiniteTimeSpan` waits without a limit.
- When the timeout expires or the `CancellationToken` is canceled, the wait returns without throwing an exception.
- An exception of the `predicate` ends the wait and is thrown by `Wait` or `WaitAsync`, also when the predicate ran
  on the thread that signaled.

</details>

The `SignalerResult` tells how the wait ended:

| Property     | Value                                                                                                  |
|--------------|--------------------------------------------------------------------------------------------------------|
| `IsSuccess`  | `true` when enough signals were received before the timeout expired or the wait was canceled           |
| `Count`      | the number of signals that were received until the wait ended                                          |
| `Parameters` | the parameters of these signals, also of those that did not match the `predicate` (`Signaler<T>` only) |
