# Callbacks

Describes the possible expectations for working with callbacks.

| Expectation             | Negated        | Summary                                       |
|-------------------------|----------------|-----------------------------------------------|
| [`Signaled`](#signaler) | `DidNotSignal` | the callback was signaled (a number of times) |

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
Without `Within(…)`, a signaler expectation waits for at most 30 seconds, the
[`DefaultSignalerTimeout`](../03-how-it-works/07-configuration.md#settings), which the failure message shows (e.g.
"within 0:30"). `Signaled()` completes as soon as the callback was signaled, and `DidNotSignal()` fails as soon as it
is signaled, but has to wait for the whole timeout to succeed, because only then is it certain that no signal follows.
Use `Within(…)` to wait for a shorter time, see
[waiting for callbacks](../03-how-it-works/06-time-and-cancellation.md#callbacks).
:::

### Amount

You can specify how often a callback must be signaled:

```csharp
using aweXpect.Core; // for `Times()`

await Expect.That(signaler).Signaled().AtLeast(3.Times());
await Expect.That(signaler).Signaled().Exactly(3.Times());
await Expect.That(signaler).Signaled().AtMost(3.Times());
await Expect.That(signaler).Signaled().LessThan(3.Times());
await Expect.That(signaler).Signaled().Between(2).And(4.Times());
await Expect.That(signaler).Signaled().Once();
await Expect.That(signaler).Signaled().Twice();
await Expect.That(signaler).Signaled().Never();
```

`Signaled(3.Times())` is the shorthand for `Signaled().AtLeast(3.Times())`, and `DidNotSignal(3.Times())` expects the
callback to be signaled fewer than three times. An expectation without an upper bound (e.g. `AtLeast`) succeeds as
soon as enough callbacks were signaled. One with an upper bound fails as soon as one signal too many is received, and
otherwise waits for the whole timeout. `DidNotSignal(times)` requires at least one time, because no callback can be
signaled fewer than zero times.

### Parameters

You can filter for signals with specific parameters by providing a `predicate`:

```csharp
Signaler<string> signaler = new();

signaler.Signal("Yesterday");
signaler.Signal("Let It Be");
signaler.Signal("Yesterday");

await Expect.That(signaler).Signaled().AtLeast(2.Times()).With(p => p == "Yesterday");
```

`WhoseParameters` continues with expectations on the collection of recorded parameters that match the filter from
`With`:

```csharp
Signaler<string> signaler = new();

signaler.Signal("Yesterday");
signaler.Signal("Let It Be");

await Expect.That(signaler).Signaled().AtLeast(2.Times()).WhoseParameters.Contains("Let It Be");
```

A failure message lists the recorded parameters.
