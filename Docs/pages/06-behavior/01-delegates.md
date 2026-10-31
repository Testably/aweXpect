---
description: Verify that a delegate throws or does not throw, the details of the thrown exception, its execution time, its allocations and memory leaks.
---

# Delegates

Describes the possible expectations for delegates and the exceptions they throw.

| Expectation                            | Negated                  | Summary                                                    |
|----------------------------------------|--------------------------|------------------------------------------------------------|
| [`Throws`](#thrown-exception)          | `DoesNotThrow`           | throws an exception                                        |
| [`Throws<T>`](#specific-exception)     | `DoesNotThrow<T>`        | throws a `T` or a derived exception                        |
| [`ThrowsExactly<T>`](#exact-exception) | `DoesNotThrowExactly<T>` | throws exactly a `T`                                       |
| [`Eventually`](#eventually)            |                          | re-evaluates the delegate until the expectations are met   |
| [`ExecutesIn`](#execution-time)        |                          | completes within the expected time                         |
| [`Allocates`](#allocations)            |                          | allocates at most or at least the given number of bytes    |
| [`IsCollectedAfter`](#memory-leaks)    |                          | the returned object is garbage collected after the cleanup |

A delegate can be any of the following. Each of them can also take a `CancellationToken` parameter (e.g.
`Func<CancellationToken, Task>`), which is canceled when the expectation times out or is canceled:

|              | Without return value            | With return value `T`                 |
|--------------|---------------------------------|---------------------------------------|
| Synchronous  | `Action`                        | `Func<T>`                             |
| Asynchronous | `Func<Task>`, `Func<ValueTask>` | `Func<Task<T>>`, `Func<ValueTask<T>>` |

An asynchronous delegate is awaited, like a [task](./02-tasks.md) that is passed directly.

:::warning[.NET 8 or later: `ValueTask` delegates]
On .NET Framework, .NET Standard 2.0, .NET 6 and .NET 7, a delegate that returns a `ValueTask` is never awaited. Return
`.AsTask()` instead, as reported by the analyzer rule [aweXpect0007](../08-analyzers.md#awexpect0007).
:::

<details>
<summary>CS0121 with C# 12 or older on .NET 8</summary>

An `async` lambda and a lambda that only throws, as in `That(async () => await x.RunAsync())` or
`That(() => throw new X())`, bind to the `Task` overload through `[OverloadResolutionPriority]`. The attribute
only takes effect with C# 13 or later, which is the default only for .NET 9 and later. With an older language version
on .NET 8, such a lambda fits both the `Task` and the `ValueTask` overload and fails with CS0121. Set `<LangVersion>`
to `13` or `latest`, return the task directly (`That(() => x.RunAsync())`), or declare a local function
(`void Act() => throw new X();`) and pass it.

</details>

## No exception

You can verify that the delegate does not throw any exception:

```csharp
void Act() { }

await That(Act).DoesNotThrow();
```

`DoesNotThrow<TException>()` only fails when the delegate throws a `TException` or a derived type, and
`DoesNotThrowExactly<TException>()` only when it throws exactly a `TException`. Any other exception passes both:

```csharp
void Act() => throw new ArgumentNullException("value");

await That(Act).DoesNotThrow<InvalidOperationException>();
await That(Act).DoesNotThrowExactly<ArgumentException>();
```

For a delegate with a return value, `WhoseResult` continues with the returned value as subject, and awaiting
`DoesNotThrow()` returns it:

```csharp
int Act() => 3;

await That(Act).DoesNotThrow().WhoseResult.IsEqualTo(3);
int result = await That(Act).DoesNotThrow();
```

Awaiting `DoesNotThrow<TException>()`, `DoesNotThrowExactly<TException>()` or their `Type` overloads returns no value.

:::warning[An expectation must not be applied to the delegate itself]
`That(Act).IsEqualTo(3)` compiles, but checks the delegate instead of what it returns, so it silently passes
or fails with a message about the delegate. The analyzer rule [aweXpect0004](../08-analyzers.md#awexpect0004) reports
it and offers to insert `.DoesNotThrow().WhoseResult`. A delegate without a return value has nothing to compare at all.
:::

<details>
<summary>Why `DoesNotThrow<TException>()` returns no value</summary>

The delegate may have thrown an exception of another type instead, so there may be no value to return. The
`WhoseResult` of `DoesNotThrow<TException>()` and `DoesNotThrowExactly<TException>()` requires that no exception was
thrown at all.

</details>

## Thrown exception

You can verify that the delegate throws an exception:

```csharp
void Act() => throw new CustomException("Yesterday");

await That(Act).Throws();
```

### Specific exception

You can verify that the delegate throws a specific exception, of the given type or any derived type:

```csharp
void Act() => throw new CustomException("Yesterday");

await That(Act).Throws<CustomException>();
await That(Act).Throws(typeof(CustomException));
```

### Exact exception

You can verify that the delegate throws exactly a specific exception, and not any derived type:

```csharp
void Act() => throw new CustomException("Yesterday");

await That(Act).ThrowsExactly<CustomException>();
await That(Act).ThrowsExactly(typeof(CustomException));
```

<details>
<summary>Open generic exception types</summary>

A `Type` argument must be an exception type. Like `Is(typeof(List<>))` for objects, an open generic type such as
`typeof(CustomException<>)` matches every exception whose type is constructed from it or derives from such a type, while
`ThrowsExactly` only matches the constructed types themselves.

</details>

### Conditional throw

You can verify that the delegate throws an exception only if a predicate is satisfied (otherwise it verifies that no
exception is thrown):

```csharp
void Act() => throw new CustomException("Yesterday");
bool expectThrownException = true;

await That(Act).Throws<CustomException>().OnlyIf(expectThrownException);
```

This is especially useful with parameterized tests where it depends on a parameter if an exception is thrown or not.

### Time limit

You can verify that the exception is thrown within a given time:

```csharp
using aweXpect.Chronology; // from the aweXpect.Chronology package

async Task Act()
{
  await Task.Delay(100);
  throw new CustomException("Yesterday");
}

await That(Act).Throws<CustomException>().Within(1.Seconds());
```

The duration is applied as timeout, see
[execution time](../02-how-it-works/06-time-and-cancellation.md#execution-time).

## Exception details

After `Throws`, the following expectations verify the thrown exception, and their `Has…` twins verify an
[exception that is the subject itself](#on-an-exception-subject):

| After `Throws`                                                | On an exception subject        | Verifies                                  |
|---------------------------------------------------------------|--------------------------------|-------------------------------------------|
| [`WithMessage`](#exception-message)                           | `HasMessage`                   | the `Message`                             |
| [`WithParamName`](#other-members)                             | `HasParamName`                 | the `ParamName` of an `ArgumentException` |
| [`WithHResult`](#other-members)                               | `HasHResult`                   | the `HResult`                             |
| [`WithInner`, `WithoutInner`](#inner-exceptions)              | `HasInner`, `DoesNotHaveInner` | the `InnerException`                      |
| [`WithRecursiveInnerExceptions`](#recursive-inner-exceptions) | `HasRecursiveInnerExceptions`  | all inner exceptions, recursively         |
| [`Whose`](#other-members)                                     | `Whose`                        | any other member                          |
| [`Which`](#other-members)                                     |                                | continues with the exception as subject   |

Directly after `Throws`, an expectation continues the sentence "throws a `CustomException`", so it uses the `With…`
vocabulary. For an exception that is the subject itself, the `Has…` twins start a new sentence. After `.Which` the
thrown exception becomes the subject, so `Has…` applies again:

```csharp
void Act() => throw new CustomException("Yesterday");
Exception exception = new CustomException("Yesterday");

await That(Act).Throws<CustomException>().WithMessage("Yesterday");
await That(Act).Throws<CustomException>().Which.HasMessage("Yesterday");
await That(exception).HasMessage("Yesterday");
```

All three verify the same thing, but only the vocabulary that matches its position produces a readable failure
message: `Has…` directly after `Throws` compiles, but reads "throws a CustomException has message …". The analyzer
rule [aweXpect0003](../08-analyzers.md#awexpect0003) flags it and offers to switch to the `With…` twin or to insert
`.Which`.

### Exception message

You can verify the message of the thrown exception:

```csharp
void Act() => throw new CustomException("Yesterday, all my troubles seemed so far away");

await That(Act).Throws().WithMessage("Yesterday, all my troubles seemed so far away");
await That(Act).Throws().WithMessage().NotEqualTo("Let It Be");
await That(Act).Throws().WithMessage().Containing("troubles");
await That(Act).Throws().WithMessage().NotContaining("Help!");
await That(Act).Throws().WithMessage().StartingWith("Yesterday");
await That(Act).Throws().WithMessage().NotStartingWith("Tomorrow");
await That(Act).Throws().WithMessage().EndingWith("far away");
await That(Act).Throws().WithMessage().NotEndingWith("here to stay");
```

`WithMessage(expected)` is the shorthand for `WithMessage().EqualTo(expected)`. The same applies to
`WithParamName(expected)`.

You can use the same [string options](../04-values/03-string.md#string-options) and
[match types](../04-values/03-string.md#match-types) as when comparing strings:

```csharp
void Act() => throw new CustomException("Yesterday, all my troubles seemed so far away");

await That(Act).Throws().WithMessage("yesterday*").AsWildcard().IgnoringCase();
```

`Containing` counts the occurrences like [`Contains`](../04-values/03-string.md#contains) on a string: without a
quantifier the value has to occur at least once, and you can specify how often it has to occur and how it is matched:

```csharp
void Act() => throw new CustomException("Help, I need somebody, help, not just anybody");

await That(Act).Throws().WithMessage().Containing("help").IgnoringCase().Twice();
await That(Act).Throws().WithMessage().Containing("?body").AsWildcard().AtLeast(2);
await That(Act).Throws().WithMessage().Containing("yesterday").Never();
```

<details>
<summary>Expecting a `null` message</summary>

A `null` argument to `WithMessage(expected)` or `WithParamName(expected)` requires the message or parameter name to be
`null` as well.

</details>

### Inner exceptions

You can verify the inner exception of the thrown exception:

```csharp
void Act() => throw new CustomException("outer", new CustomException("inner"));

await That(Act).Throws().WithInner();
await That(Act).Throws().WithInner<CustomException>();
```

Pass expectations to continue with the inner exception as subject. They are only checked when the inner exception
exists and is of the given type:

```csharp
void Act() => throw new CustomException("outer", new CustomException("inner"));

await That(Act).Throws().WithInner(inner => inner.HasMessage("inner"));
await That(Act).Throws().WithInner<CustomException>(inner => inner.HasMessage("inner"));
```

You can also verify that the thrown exception has no inner exception, or none of a given type:

```csharp
void Act() => throw new CustomException("outer");

await That(Act).Throws().WithoutInner();
await That(Act).Throws().WithoutInner<ArgumentException>();
```

`WithInner`, `WithInner(…, expectations)` and `WithoutInner` also take a `Type` instead of a type argument, e.g.
`WithInner(typeof(CustomException))`.

### Recursive inner exceptions

You can recursively verify the collection of inner exceptions of the thrown exception with the
[quantifiers](../05-collections/02-items.md#quantifiers) of a collection:

```csharp
void Act() => throw new AggregateException("outer", new CustomException("inner"));

await That(Act).Throws()
  .WithRecursiveInnerExceptions(innerExceptions => innerExceptions.AtLeast(1).Are<CustomException>());
```

The exception must have at least one inner exception.

### Other members

You can verify additional members of the exception:

```csharp
CustomException exception = new("outer", hResult: 12345);
void Act() => throw exception;

await That(Act).Throws().WithHResult(12345);
// the `HResult` continues with the numeric comparison vocabulary
await That(Act).Throws().WithHResult().GreaterThan(12340);
// verify arbitrary additional members
await That(Act).Throws()
  .Whose(e => e.HResult, h => h.IsGreaterThan(12340));
// access the thrown exception
await That(Act).Throws()
  .Which.IsSameAs(exception);
```

The `ParamName` of an `ArgumentException` continues like the message:

```csharp
void Act() => throw new ArgumentNullException("album");

await That(Act).Throws<ArgumentNullException>().WithParamName("album");
await That(Act).Throws<ArgumentNullException>().WithParamName().NotEqualTo("track");
await That(Act).Throws<ArgumentNullException>().WithParamName().StartingWith("al");
```

### On an exception subject

You can verify an exception that is the subject itself, e.g. when a method returns it or a callback receives it, with
the `Has…` twins:

```csharp
Exception exception = new CustomException("Yesterday", new CustomException("Let It Be"), hResult: 12345);
ArgumentException argumentException = new ArgumentNullException("album");

await That(exception).HasMessage("Yesterday");
await That(exception).HasHResult().GreaterThan(12340);
await That(exception).HasInner<CustomException>(inner => inner.HasMessage("Let It Be"));
await That(exception).HasRecursiveInnerExceptions(innerExceptions => innerExceptions.All().Are<CustomException>());
await That(argumentException).HasParamName("album");
```

## Eventually

You can verify that the value a delegate returns eventually meets the expectations: `Eventually()` re-evaluates a
delegate with a return value until the expectations on the returned value are met, e.g. while a background task is
still running:

```csharp
Track track = new();
// Start a background task that plays the track

await That(() => track.PlayCount).Eventually().IsGreaterThan(5);
```

See [Eventually](../02-how-it-works/06-time-and-cancellation.md#eventually) for how long and how often the delegate is
re-evaluated, and how exceptions, timeouts and cancellation are handled.

## Execution time

You can verify the execution time of a delegate:

```csharp
using aweXpect.Chronology; // from the aweXpect.Chronology package

async Task PlayIntro() => await Task.Delay(200);

await That(PlayIntro).ExecutesIn().AtMost(300.Milliseconds());
await That(PlayIntro).ExecutesIn().AtLeast(100.Milliseconds());
// within 200ms ± 50ms
await That(PlayIntro).ExecutesIn(200.Milliseconds()).Within(50.Milliseconds());
await That(PlayIntro).ExecutesIn().Between(100.Milliseconds()).And(300.Milliseconds());
```

A delegate that throws an exception fails these expectations, however fast it did so: a crash is not a measurement
of execution time.

The upper bound is applied as timeout, see
[execution time](../02-how-it-works/06-time-and-cancellation.md#execution-time) for how a delegate that runs too long
is canceled or abandoned.

### Allowing exceptions

When you want to measure how long a delegate ran *before* it failed, for example a retry that exhausts its attempts,
a circuit breaker that trips, or an operation that runs into its own timeout, `AllowingExceptions()` lets the
measured duration decide alone:

```csharp
await That(() => retryPolicy.Execute(alwaysFailing)).ExecutesIn().AllowingExceptions()
  .AtLeast(300.Milliseconds())
  .Because("three attempts with a 100ms backoff must have been made before giving up");
```

The duration is measured up to the throw, and the exception is still shown in the failure message when the delegate
misses the expected time.

<details>
<summary>Timeouts and cancellation with `AllowingExceptions()`</summary>

A timeout from `WithTimeout` or from the upper bound still fails the expectation with "did not finish within …", and a
canceled `WithCancellation` token leaves it inconclusive. An `OperationCanceledException` that the delegate throws for
its own reasons is allowed like any other exception.

</details>

:::warning[`AllowingExceptions()` with `AtMost(…)` accepts an immediate crash]
With `AtMost(…)`, a delegate that throws immediately also passes. If the delegate is expected to throw, use
`Throws<TException>().Within(…)` instead.
:::

## Allocations

:::info[.NET 8 or later]
`Allocates()` is only available on .NET 8 or later.
:::

You can verify how many bytes a synchronous delegate allocates:

```csharp
string[] playlist = ["Yesterday", "Let It Be", "Help!"];

int CountLongTitles()
{
  int count = 0;
  foreach (string title in playlist)
  {
    if (title.Length > 5)
    {
      count++;
    }
  }

  return count;
}

string JoinTitles() => string.Join(", ", playlist);

await That(CountLongTitles).Allocates().Nothing();
await That(JoinTitles).Allocates().AtMost(1024)
  .Because("joining the titles should only allocate the joined string");
await That(JoinTitles).Allocates().AtLeast(16);
await That(JoinTitles).Allocates().Between(16).And(1024);
```

`Nothing()` is the shorthand for `AtMost(0)`. There is no expectation on an exact number of bytes, because the size of
an object changes between runtime versions and platforms, so prefer bounds with some margin.

Only allocations on the current thread are counted; allocations on other threads, e.g. of a started task, are not
included.

:::warning[Synchronous delegates only]
`Allocates()` supports `Action`, `Func<T>` and their variants with a `CancellationToken`. An asynchronous delegate
can continue on another thread after an `await`, whose allocations would be missed, so `Allocates()` throws a `NotSupportedException` for an asynchronous delegate or a task.
:::

Like for the [execution time](#execution-time), a delegate that throws an exception fails the expectation, unless
`AllowingExceptions()` is specified. The measured bytes then include the exception and the stack trace the runtime
captured for it:

```csharp
void Reject() => throw new ArgumentException("The track is unknown.");

await That(Reject).Allocates().AllowingExceptions().AtMost(10_000)
  .Because("rejecting a track should not allocate more than the exception");
```

The delegate is invoked once for all expectations on it, so it can be combined with other delegate expectations, e.g.
`That(JoinTitles).Allocates().AtMost(1024).And.ExecutesIn().AtMost(…)`.

<details>
<summary>How the allocations are measured</summary>

The bytes are read with `GC.GetAllocatedBytesForCurrentThread()` directly before and after a single invocation of the
delegate. `Allocates()` can only be specified once per expectation.

</details>

### Warm-up

The first invocation of a delegate can include one-time allocations, e.g. of static constructors, lazy caches or
pools. `AfterWarmUp()` invokes the delegate once before the measured invocation, so that they are not counted:

```csharp
Dictionary<int, string> titles = new();

string GetTitle(int track)
{
  if (!titles.TryGetValue(track, out string? title))
  {
    title = $"Track {track}";
    titles[track] = title;
  }

  return title;
}

await That(() => GetTitle(7)).Allocates().AfterWarmUp().Nothing()
  .Because("the title is cached after the first call");
```

The delegate then runs twice, and the other expectations on it, e.g. on the execution time, see the measured second
invocation. Without `AfterWarmUp()`, the delegate runs exactly once.

<details>
<summary>Exceptions during the warm-up</summary>

An exception in the warm-up fails the expectation like one in the measured invocation, which is then skipped. With
`AllowingExceptions()`, it is ignored and the second invocation is measured.

</details>

## Memory leaks

You can verify that the object a delegate creates is garbage collected after a cleanup with `IsCollectedAfter`, e.g.
that a view model no longer listens to a long-lived message bus once it is disposed:

```csharp
MessageBus bus = new();

await That(() => new ViewModel(bus)).IsCollectedAfter(vm => vm.Dispose())
  .Because("the view model must unsubscribe from the bus when it is disposed");
```

The expectation fails with "it was still alive" when anything else still references the object, e.g. an event handler
or a static collection that the cleanup did not remove. When no cleanup is needed, use `IsCollectedAfter(_ => { })`.

:::warning[Create the object inside the delegate]
The delegate must create the object: when it returns an instance that the test also holds, e.g. in a local variable,
this reference keeps the object alive. To check an object you only have a `WeakReference` to, use
[`IsCollected`](../04-values/12-object.md#garbage-collection).
:::

<details>
<summary>How the object is collected</summary>

aweXpect calls the delegate and the cleanup in a separate method that keeps only a weak reference to the object, and
then forces full blocking garbage collections. An asynchronous delegate is awaited before the cleanup runs. An
exception thrown by the delegate or by the cleanup fails the expectation, as does a delegate that returns `null`.

</details>
