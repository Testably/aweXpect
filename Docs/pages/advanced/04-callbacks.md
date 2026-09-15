# Callbacks

Describes the possible expectations for working with callbacks.

## Signaler

First, you have to start recording callback signals using the `Signaler` class. This class is available in the "
aweXpect.Signaling" namespace.

```csharp
// ↓ Counts signals from callbacks without parameters
Signaler signaler = new();
Signaler<string> signaler = new();
// ↑ Counts signals from callbacks with a string parameter
```

Then, you can signal the callback on the recording.

```csharp
class MyClass
{
  public void Execute(Action<string> onCompleted)
  {
    // do something in a background thread and then call the onCompleted callback
  }
}

sut.Execute(v => signaler.Signal(v));
```

At last, you can wait for the callback to be signaled:

```csharp
await Expect.That(signaler).Signaled();
```

You can also verify that the callback will not be signaled:

```csharp
await Expect.That(signaler).DidNotSignal();
```

*NOTE: The last statement will result never return, unless a timeout or cancellation is specified.
Therefore, when nothing is specified, a default timeout of 30 seconds is applied!*

### Timeout

You can specify a timeout, how long you want to wait for the callback to be signaled:

```csharp
await Expect.That(signaler).Signaled().Within(TimeSpan.FromSeconds(5))
  .Because("it should take at most 5 seconds to complete");
```

Alternatively you can also use a `CancellationToken` for a timeout:

```csharp
CancellationToken cancellationToken = new CancellationTokenSource(5000).Token;
await Expect.That(signaler).Signaled().WithCancellation(cancellationToken)
  .Because("it should be completed, before the cancellationToken is cancelled");
```

### Amount

You can specify how often a callback must be signaled:

```csharp
await Expect.That(signaler).Signaled().AtLeast(3.Times());
await Expect.That(signaler).Signaled().Exactly(3.Times());
await Expect.That(signaler).Signaled().AtMost(3.Times());
await Expect.That(signaler).Signaled().LessThan(3.Times());
await Expect.That(signaler).Signaled().Between(2).And(4.Times());
await Expect.That(signaler).Signaled().Once();
await Expect.That(signaler).Signaled().Twice();
await Expect.That(signaler).Signaled().Never();
```

`Signaled(3.Times())` and `DidNotSignal(3.Times())` are shorthands for the `AtLeast` form.

*NOTE: Only expectations without an upper bound (e.g. `AtLeast`) can complete as soon as enough callbacks were signaled.
All others have to wait for the timeout to expire, because only then is the number of signals final!*

### Parameters

You can also include a parameter during signaling:

```csharp
Signaler<string> signaler = new();

signaler.Signal("Yesterday");
signaler.Signal("Let It Be");

await Expect.That(signaler).Signaled().AtLeast(2.Times());
```

You can filter for signals with specific parameters by providing a `predicate`:

```csharp
Signaler<string> signaler = new();

signaler.Signal("Yesterday");
signaler.Signal("Let It Be");
signaler.Signal("Yesterday");

await Expect.That(signaler).Signaled().AtLeast(2.Times()).With(p => p == "Yesterday");
```

*In case of a failed expectation, the recorded parameters will be displayed in the error message.*
