# Delegates

Describes the possible expectations for delegates and the exceptions they throw.

| Expectation                            | Negated                  | Summary                                                  |
|----------------------------------------|--------------------------|----------------------------------------------------------|
| [`Throws`](#thrown-exception)          | `DoesNotThrow`           | throws an exception                                      |
| [`Throws<T>`](#specific-exception)     | `DoesNotThrow<T>`        | throws a `T` or a derived exception                      |
| [`ThrowsExactly<T>`](#exact-exception) | `DoesNotThrowExactly<T>` | throws exactly a `T`                                     |
| [`ExecutesIn`](#execution-time)        |                          | completes within the expected time                       |
| [`Eventually`](#eventually)            |                          | re-evaluates the delegate until the expectations are met |

A delegate can be any of the following. Each of them can also take a `CancellationToken` parameter (e.g.
`Func<CancellationToken, Task>`), which is canceled when the expectation times out or is canceled:

|              | Without return value            | With return value `T`                 |
|--------------|---------------------------------|---------------------------------------|
| Synchronous  | `Action`                        | `Func<T>`                             |
| Asynchronous | `Func<Task>`, `Func<ValueTask>` | `Func<Task<T>>`, `Func<ValueTask<T>>` |

An asynchronous delegate is awaited, like a [task](./02-tasks.md) that is passed directly.

:::warning[.NET 8 or later: `ValueTask` delegates]
.NET Framework, .NET Standard 2.0, .NET 6 and .NET 7 use the .NET Standard 2.0 build of aweXpect, which has no
overloads for `Func<ValueTask>` and `Func<ValueTask<T>>`. Such a delegate is treated as a `Func<T>` whose result is the
`ValueTask`, so it is never awaited: an exception thrown asynchronously is not seen. Return a `Task` instead, as in
`() => Act().AsTask()`. The analyzer rule [aweXpect0007](../07-analyzers.md#awexpect0007) reports it as an error and
offers to add `.AsTask()`.
:::

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
or fails with a message about the delegate. The analyzer rule [aweXpect0004](../07-analyzers.md#awexpect0004) reports
it and offers to insert `.DoesNotThrow().WhoseResult`. A delegate without a return value has nothing to compare at all.
:::

## Thrown exception

You can verify that the delegate throws an exception:

```csharp
void Act() => throw new CustomException("Yesterday");

await Expect.That(Act).Throws();
```

### Specific exception

You can verify that the delegate throws a specific exception, of the given type or any derived type:

```csharp
void Act() => throw new CustomException("Yesterday");

await Expect.That(Act).Throws<CustomException>();
await Expect.That(Act).Throws(typeof(CustomException));
```

### Exact exception

You can verify that the delegate throws exactly a specific exception, and not any derived type:

```csharp
void Act() => throw new CustomException("Yesterday");

await Expect.That(Act).ThrowsExactly<CustomException>();
await Expect.That(Act).ThrowsExactly(typeof(CustomException));
```

A `Type` argument must be an exception type. Like `Is(typeof(List<>))` for objects, an open generic type such as
`typeof(CustomException<>)` matches every exception whose type is constructed from it or derives from such a type, while
`ThrowsExactly` only matches the constructed types themselves.

### Conditional throw

You can verify that the delegate throws an exception only if a predicate is satisfied (otherwise it verifies that no
exception is thrown):

```csharp
void Act() => throw new CustomException("Yesterday");
bool expectThrownException = true;

await Expect.That(Act).Throws<CustomException>().OnlyIf(expectThrownException);
```

This is especially useful with parametrized tests where it depends on a parameter if an exception is thrown or not.

### Time limit

You can verify that the exception is thrown within a given time:

```csharp
using aweXpect.Chronology; // from the aweXpect.Chronology package

async Task Act()
{
  await Task.Delay(100);
  throw new CustomException("Yesterday");
}

await Expect.That(Act).Throws<CustomException>().Within(1.Seconds());
```

The duration is applied as timeout, see
[execution time](../03-how-it-works/06-time-and-cancellation.md#execution-time).

## Exception details

After `Throws`, the following expectations verify the thrown exception. The same checks exist with the `Has…`
vocabulary for an exception that is the subject itself:

| After `Throws`                                                | On an exception subject        | Verifies                                  |
|---------------------------------------------------------------|--------------------------------|-------------------------------------------|
| [`WithMessage`](#exception-message)                           | `HasMessage`                   | the `Message`                             |
| [`WithParamName`](#other-members)                             | `HasParamName`                 | the `ParamName` of an `ArgumentException` |
| [`WithHResult`](#other-members)                               | `HasHResult`                   | the `HResult`                             |
| [`WithInner`, `WithoutInner`](#inner-exceptions)              | `HasInner`, `DoesNotHaveInner` | the `InnerException`                      |
| [`WithRecursiveInnerExceptions`](#recursive-inner-exceptions) | `HasRecursiveInnerExceptions`  | all inner exceptions, recursively         |
| [`Whose`](#other-members)                                     | `Whose`                        | any other member                          |
| [`Which`](#other-members)                                     |                                | continues with the exception as subject   |

### Exception message

You can verify the message of the thrown exception:

```csharp
void Act() => throw new CustomException("Yesterday, all my troubles seemed so far away");

await Expect.That(Act).Throws().WithMessage("Yesterday, all my troubles seemed so far away");
await Expect.That(Act).Throws().WithMessage().NotEqualTo("Let It Be");
await Expect.That(Act).Throws().WithMessage().Containing("troubles");
await Expect.That(Act).Throws().WithMessage().NotContaining("Help!");
await Expect.That(Act).Throws().WithMessage().StartingWith("Yesterday");
await Expect.That(Act).Throws().WithMessage().NotStartingWith("Tomorrow");
await Expect.That(Act).Throws().WithMessage().EndingWith("far away");
await Expect.That(Act).Throws().WithMessage().NotEndingWith("here to stay");
```

`WithMessage(expected)` is the shorthand for `WithMessage().EqualTo(expected)`, so a `null` argument requires the
message to be `null` as well.

Only `EqualTo` and `NotEqualTo` accept `null`; the other comparisons reject `null` and the empty string, because
neither is a substring anything could meaningfully be checked against.

You can use the same [string options](../04-values/03-string.md#string-options) and
[match types](../04-values/03-string.md#match-types) as when comparing strings:

```csharp
void Act() => throw new CustomException("Yesterday, all my troubles seemed so far away");

await Expect.That(Act).Throws().WithMessage("yesterday*").AsWildcard().IgnoringCase();
```

### Inner exceptions

You can verify the inner exception of the thrown exception:

```csharp
void Act() => throw new CustomException("outer", new CustomException("inner"));

await Expect.That(Act).Throws().WithInner();
await Expect.That(Act).Throws().WithInner<CustomException>();
```

Pass expectations to continue with the inner exception as subject. They are only checked when the inner exception
exists and is of the given type:

```csharp
void Act() => throw new CustomException("outer", new CustomException("inner"));

await Expect.That(Act).Throws().WithInner(inner => inner.HasMessage("inner"));
await Expect.That(Act).Throws().WithInner<CustomException>(inner => inner.HasMessage("inner"));
```

You can also verify that the thrown exception has no inner exception, or none of a given type:

```csharp
void Act() => throw new CustomException("outer");

await Expect.That(Act).Throws().WithoutInner();
await Expect.That(Act).Throws().WithoutInner<ArgumentException>();
```

### Recursive inner exceptions

You can recursively verify the collection of inner exceptions of the thrown exception with the
[quantifiers](../05-collections/02-items.md#quantifiers) of a collection:

```csharp
void Act() => throw new AggregateException("outer", new CustomException("inner"));

await Expect.That(Act).Throws()
  .WithRecursiveInnerExceptions(innerExceptions => innerExceptions.AtLeast(1).Are<CustomException>());
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
void Act() => throw new ArgumentNullException("album");

await Expect.That(Act).Throws<ArgumentNullException>().WithParamName("album");
await Expect.That(Act).Throws<ArgumentNullException>().WithParamName().NotEqualTo("track");
await Expect.That(Act).Throws<ArgumentNullException>().WithParamName().StartingWith("al");
```

`WithParamName(expected)` is the shorthand for `WithParamName().EqualTo(expected)`, so a `null` argument requires the
`ParamName` to be `null` as well.

### `With…` after `Throws`, `Has…` on the exception

Directly after `Throws`, an expectation continues the sentence "throws a `CustomException`", so it uses the
`With…` vocabulary. For an exception that is the subject itself, the `Has…` twins start a new sentence. After `.Which`
the thrown exception becomes the subject, so `Has…` applies again:

```csharp
void Act() => throw new CustomException("Yesterday");
Exception exception = new CustomException("Yesterday");

await Expect.That(Act).Throws<CustomException>().WithMessage("Yesterday");
await Expect.That(Act).Throws<CustomException>().Which.HasMessage("Yesterday");
await Expect.That(exception).HasMessage("Yesterday");
```

All three verify the same thing, but only the vocabulary that matches its position produces a readable failure
message: `Has…` directly after `Throws` compiles, but reads "throws a CustomException has message …". The analyzer
rule [aweXpect0003](../07-analyzers.md#awexpect0003) flags it and offers to switch to the `With…` twin or to insert
`.Which`.

## Execution time

You can verify the execution time of a delegate:

```csharp
using aweXpect.Chronology; // from the aweXpect.Chronology package

async Task PlayIntro() => await Task.Delay(200);

await Expect.That(PlayIntro).ExecutesIn().AtMost(300.Milliseconds())
  .Because("the intro should play faster than 300ms");
await Expect.That(PlayIntro).ExecutesIn().AtLeast(100.Milliseconds())
  .Because("the intro should play slower than 100ms");
await Expect.That(PlayIntro).ExecutesIn(200.Milliseconds()).Within(50.Milliseconds())
  .Because("the intro should play within 200ms ± 50ms");
await Expect.That(PlayIntro).ExecutesIn().Between(100.Milliseconds()).And(300.Milliseconds())
  .Because("the intro should play slower than 100ms and faster than 300ms");
```

A delegate that throws an exception fails these expectations, however fast it did so: a crash is not a measurement
of execution time.

The upper bound is applied as timeout, see
[execution time](../03-how-it-works/06-time-and-cancellation.md#execution-time) for how a delegate that runs too long
is canceled or abandoned.

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

`Eventually()` re-evaluates a delegate with a return value until the expectations on the returned value are met, e.g.
while a background task is still running:

```csharp
Track track = new();
// Start a background task that plays the track

await Expect.That(() => track.PlayCount).Eventually().IsGreaterThan(5);
```

See [Eventually](../03-how-it-works/06-time-and-cancellation.md#eventually) for how long and how often the delegate is
re-evaluated, and how exceptions, timeouts and cancellation are handled.
