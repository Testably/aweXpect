# Delegates

Describes the possible expectations for delegates and exceptions.

A delegate can be any of the following:

- `Action` or `Action<CancellationToken>`  
  a synchronous method without return value (optionally accepting a `CancellationToken` for timeout)
- `Func<Task>` or `Func<CancellationToken, Task>`  
  an asynchronous method without return value (optionally accepting a `CancellationToken` for timeout)
- `Func<ValueTask>` or `Func<CancellationToken, ValueTask>`  
  an asynchronous method using `ValueTask` without return value (optionally accepting a `CancellationToken` for timeout)
- `Func<T>` or `Func<CancellationToken, T>`  
  a synchronous method with return value `T` (optionally accepting a `CancellationToken` for timeout)
- `Func<Task<T>>` or `Func<CancellationToken, Task<T>>`  
  an asynchronous method with return value `T` (optionally accepting a `CancellationToken` for timeout)
- `Func<ValueTask<T>>` or `Func<CancellationToken, ValueTask<T>>`  
  an asynchronous method using `ValueTask` with return value `T` (optionally accepting a `CancellationToken` for
  timeout)
- `Task` or `ValueTask`  
  an asynchronous operation without return value that is already running

```csharp
await Expect.That(DoAsync()).DoesNotThrow();
await Expect.That(DoAsync()).Throws<InvalidOperationException>();
```

A `Task` or `ValueTask` is already running when the expectation receives it, so an expectation on the execution time
only measures the duration that remains, and a timeout cannot stop it: the expectation only stops waiting for it
and fails with "did not finish within …". Pass the method itself (`Expect.That(DoAsync)`) to measure the whole
execution. A `ValueTask` is consumed by `Expect.That`, so it must not be awaited anywhere else.

To make an expectation about the task object rather than about what it does, state the type explicitly:

```csharp
await Expect.That<Task>(task).IsNotNull();
```

Note that `Task<T>` and `ValueTask<T>` behave differently: they are awaited and their **result** becomes the subject.

:::info[C# 13 or later]
An `async` lambda and a lambda that only throws, as in `Expect.That(async () => await x.RunAsync())` or
`Expect.That(() => throw new X())`, bind to the `Task` overload through `[OverloadResolutionPriority]`. The attribute
only takes effect with C# 13 or later, which is the default only for .NET 9 and later. With an older language version
on .NET 8, such a lambda fits both the `Task` and the `ValueTask` overload and fails with CS0121. Set `<LangVersion>`
to `13` or `latest`, return the task directly (`Expect.That(() => x.RunAsync())`), or declare a local function
(`void Act() => throw new X();`) and pass it.
:::

## No exception

You can verify that the delegate does not throw any exception:

```csharp
void Act() { }

await Expect.That(Act).DoesNotThrow();
```

`DoesNotThrow<TException>()` only fails when the delegate throws a `TException` or a derived type, and
`DoesNotThrowExactly<TException>()` only when it throws exactly a `TException`. Any other exception passes both:

```csharp
void Act() => throw new ArgumentNullException("value");

await Expect.That(Act).DoesNotThrow<InvalidOperationException>();
await Expect.That(Act).DoesNotThrowExactly<ArgumentException>();
```

For a delegate with a return value, `WhoseResult` continues with the returned value as subject, and awaiting the
expectation returns it:

```csharp
int Act() => 3;

await Expect.That(Act).DoesNotThrow().WhoseResult.IsEqualTo(3);
int result = await Expect.That(Act).DoesNotThrow();
```

:::warning[An expectation must not be applied to the delegate itself]
`Expect.That(Act).IsEqualTo(3)` compiles, but checks the delegate instead of what it returns, so it silently passes
or fails with a message about the delegate. The analyzer rule `aweXpect0004` reports it and offers to insert
`.DoesNotThrow().WhoseResult`. A delegate without a return value has nothing to compare at all.
:::

## Thrown exception

You can verify that the delegate throws an exception:

```csharp
void Act() => throw new CustomException("my exception");

await Expect.That(Act).Throws();
```

### Specific exception

You can verify that the delegate throws a specific exception:

```csharp
void Act() => throw new CustomException("my exception");

await Expect.That(Act).Throws<CustomException>();
await Expect.That(Act).Throws(typeof(CustomException));
```

This will verify that the thrown exception is of type `CustomException` or any derived type.

### Exact exception

You can verify that the delegate throws exactly a specific exception:

```csharp
void Act() => throw new CustomException("my exception");

await Expect.That(Act).ThrowsExactly<CustomException>();
await Expect.That(Act).ThrowsExactly(typeof(CustomException));
```

This will verify that the thrown exception is of type `CustomException` and not any derived type.

### Conditional throw

You can verify that the delegate throws an exception only if a predicate is satisfied (otherwise it verifies that no
exception is thrown):

```csharp
void Act() => throw new CustomException("my exception");
bool expectThrownException = true;

await Expect.That(Act).Throws<CustomException>().OnlyIf(expectThrownException);
```

This is especially useful with parametrized tests where it depends on a parameter if an exception is thrown or not.

## Exception message

You can verify the message of the thrown exception:

```csharp
void Act() => throw new CustomException("This is my exception text");

await Expect.That(Act).Throws().WithMessage("This is my exception text");
await Expect.That(Act).Throws().WithMessage().NotEqualTo("some other text");
await Expect.That(Act).Throws().WithMessage().Containing("my exception");
await Expect.That(Act).Throws().WithMessage().NotContaining("something else");
await Expect.That(Act).Throws().WithMessage().StartingWith("This is");
await Expect.That(Act).Throws().WithMessage().NotStartingWith("That was");
await Expect.That(Act).Throws().WithMessage().EndingWith("exception text");
await Expect.That(Act).Throws().WithMessage().NotEndingWith("something else");
```

`WithMessage(expected)` is the shorthand for `WithMessage().EqualTo(expected)`.

Only `EqualTo` and `NotEqualTo` accept `null`; the other comparisons reject `null` and the empty string, because
neither is a substring anything could meaningfully be checked against.

You can use the same configuration options as when [comparing strings](/docs/expectations/common-types/string#equality).

## Inner exceptions

You can verify the inner exception of the thrown exception:

```csharp
void Act() => throw new CustomException("outer", new CustomException("inner"));

await Expect.That(Act).Throws().WithInner();
await Expect.That(Act).Throws().WithInner<CustomException>();
```

### Recursive inner exceptions

You can recursively verify the collection of inner exceptions of the thrown exception:

```csharp
void Act() => throw new AggregateException("outer", new CustomException("inner"));

await Expect.That(Act).Throws().WithRecursiveInnerExceptions(innerExceptions => innerExceptions.AtLeast(1).Are<CustomException>());
```

The exception must have at least one inner exception.

### Other members

You can verify additional members of the exception:

```csharp
var exception = new CustomException("outer", hResult: 12345);
void Act() => throw exception;

await Expect.That(Act).Throws().WithHResult(12345)
  .Because("you can verify the `HResult`");
await Expect.That(Act).Throws().WithHResult().GreaterThan(12340)
  .Because("the `HResult` continues with the numeric comparison vocabulary");
await Expect.That(Act).Throws()
  .Whose(e => e.HResult, h => h.IsGreaterThan(12340))
  .Because("you can verify arbitrary additional members");
await Expect.That(Act).Throws()
  .Which.IsSameAs(exception)
  .Because("you can access the thrown exception");

```

The `ParamName` of an `ArgumentException` continues like the message:

```csharp
void Act() => throw new ArgumentNullException("myParameter");

await Expect.That(Act).Throws<ArgumentNullException>().WithParamName("myParameter");
await Expect.That(Act).Throws<ArgumentNullException>().WithParamName().NotEqualTo("otherParameter");
await Expect.That(Act).Throws<ArgumentNullException>().WithParamName().Containing("Parameter");
await Expect.That(Act).Throws<ArgumentNullException>().WithParamName().StartingWith("my");
await Expect.That(Act).Throws<ArgumentNullException>().WithParamName().EndingWith("Parameter");
```

`WithParamName(expected)` is the shorthand for `WithParamName().EqualTo(expected)`, so a `null` argument requires the `ParamName` to be `null` as well.

### `With…` after `Throws`, `Has…` on the exception

Directly after `Throws`, an expectation continues the sentence "throws a `CustomException`", so it uses the
`With…` vocabulary (`WithMessage`, `WithInner`, `WithHResult`, …). The same checks exist as `Has…`
(`HasMessage`, `HasInner`, `HasHResult`, …) for an exception that is the subject itself, because there
they start a new sentence. After `.Which` the thrown exception becomes the subject, so `Has…` applies again:

```csharp
void Act() => throw new CustomException("my exception");
Exception exception = new CustomException("my exception");

await Expect.That(Act).Throws<CustomException>().WithMessage("my exception");
await Expect.That(Act).Throws<CustomException>().Which.HasMessage("my exception");
await Expect.That(exception).HasMessage("my exception");
```

All three verify the same thing, but only the vocabulary that matches its position produces a readable failure
message: `Has…` directly after `Throws` compiles, but reads "throws a CustomException has message …". The analyzer
rule `aweXpect0003` flags it and offers to switch to the `With…` twin or to insert `.Which`.

## Execution time

You can verify the execution time of a delegate:

```csharp
using aweXpect.Chronology; // from the aweXpect.Chronology package

await Expect.That(Task.Delay(200)).ExecutesIn().AtMost(300.Milliseconds())
  .Because("the delegate should execute faster than 300ms");
await Expect.That(Task.Delay(200)).ExecutesIn().AtLeast(100.Milliseconds())
  .Because("the delegate should execute slower than 100ms");
await Expect.That(Task.Delay(200)).ExecutesIn(200.Milliseconds()).Within(50.Milliseconds())
  .Because("the delegate should execute within 200ms ± 50ms");
await Expect.That(Task.Delay(200)).ExecutesIn().Between(100.Milliseconds()).And(300.Milliseconds())
  .Because("the delegate should execute slower than 100ms and faster than 300ms");
```

A delegate that throws an exception fails these expectations, however fast it did so: a crash is not a measurement
of execution time.

The upper bound is the maximum of `AtMost`, the end of the `Between` range, or the expected time plus the tolerance. It
is applied as timeout (a tighter timeout, e.g. from `WithTimeout(…)`, still applies), so that a delegate accepting a
`CancellationToken` is canceled once it elapsed and the expectation fails with "did not finish within …" instead of
hanging. `AtLeast` has no upper bound and therefore applies no timeout. The task of an asynchronous delegate is
abandoned once the timeout elapsed, even if the delegate ignores or does not accept a `CancellationToken`, and the
expectation fails the same way. A synchronous delegate cannot be interrupted and runs to completion, however long
that takes; neither `WithTimeout` nor `WithCancellation` changes that. The duration of `Throws().Within(…)` is applied
as timeout the same way.

### Allowing exceptions

When you want to measure how long a delegate ran *before* it failed, for example a retry that exhausts its attempts,
a circuit breaker that trips, or an operation that runs into its own timeout, `AllowingExceptions()` lets the
measured duration decide alone:

```csharp
await Expect.That(() => retryPolicy.Execute(alwaysFailing)).ExecutesIn().AllowingExceptions()
  .AtLeast(300.Milliseconds())
  .Because("three attempts with a 100ms backoff must have been made before giving up");
```

The duration is measured up to the throw, and the exception is still shown in the failure message when the delegate
misses the expected time. A timeout from `WithTimeout` or from the upper bound still fails the expectation with "did
not finish within …", and a canceled `WithCancellation` token leaves it inconclusive. An `OperationCanceledException`
that the delegate throws for its own reasons is allowed like any other exception.

:::warning[`AllowingExceptions()` with `AtMost(…)` accepts an immediate crash]
A delegate that throws in microseconds satisfies an upper bound, which is the point of the option, but it means the
expectation no longer says anything about the delegate completing. Consider whether you also want an expectation on
what was thrown: for an upper bound on a delegate that is *expected* to throw, `Throws<TException>().Within(…)`
states both at once.
:::

## Eventually

Some values only become correct after a short delay, e.g. because a background task is still running.
Instead of waiting for a fixed amount of time, you can use `Eventually()` to re-evaluate the delegate
until the expectations are met:

```csharp
await Expect.That(() => sut.MyProp).Eventually().IsGreaterThan(5);
```

Because only the subject is re-evaluated, all expectations work as usual, including `And`, `Or` and `Because`:

```csharp
await Expect.That(() => sut.Name).Eventually().IsNotNull().And.StartsWith("foo");
```

The delegate is re-evaluated every
[`DefaultCheckInterval`](/docs/expectations/advanced/customization) (defaults to `100ms`) until the timeout
configured in [`DefaultEventuallyTimeout`](/docs/expectations/advanced/customization) (defaults to `30s`)
expires. The last wait is shortened so that it never exceeds the timeout, which means that an interval
that is longer than the timeout results in exactly two evaluations. You can overwrite the timeout per
expectation with `Within` and the interval with `CheckEvery`, in either order:

```csharp
await Expect.That(() => sut.MyProp).Eventually().Within(5.Seconds()).CheckEvery(50.Milliseconds())
  .IsGreaterThan(5);
// using aweXpect.Chronology
```

`Within(Timeout.InfiniteTimeSpan)` retries until the expectations are met or the expectation is
canceled. A negative timeout or an interval that is not positive is rejected, and each of them can only be
specified once.

`WithTimeout` does not change the timeout of the retries, but cancels the evaluation like everywhere else.

An exception thrown by the delegate counts as an unmet expectation and is retried. When the timeout expires
while the delegate is still throwing, the expectation fails and the last exception is reported as the cause
of the failure. If the expectation is canceled before the timeout expires (via `WithCancellation` or via
`TestCancellation.FromCancellationToken`), it is reported as inconclusive instead of failed, while a `WithTimeout`
or a global `TestCancellation.FromTimeout` that elapses first fails it with "did not finish within …". As everywhere
else, the tighter timeout wins: such a timeout that is shorter than `Within` ends the retries, and a longer one does
not extend them.

The timeout also bounds each evaluation: an evaluation that is still running when the timeout is used up is abandoned,
even if the delegate ignores its `CancellationToken`, which is canceled at that point, and the expectation fails with
"did not finish within …" and a `TimeoutException` as inner exception. The last evaluation, which is made when the
timeout is used up, still gets one check interval (at most the timeout) to finish. A synchronous delegate cannot be
interrupted, so for it the timeout is only checked between evaluations.

In addition to `Func<T>`, the asynchronous variant `Func<Task<T>>` is supported, and on .NET 8 or later also
`Func<ValueTask<T>>`; each of them also accepts a `CancellationToken`. Returning the task directly also works with a
language version [older than C# 13](#delegates):

```csharp
await Expect.That(() => sut.GetCountAsync()).Eventually().IsGreaterThan(5);
```
