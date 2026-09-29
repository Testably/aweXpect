# Anatomy of an expectation

Every expectation starts with `Expect.That(subject)`, continues with what you expect and is awaited:

```csharp
string title = "Let It Be";

await Expect.That(title).StartsWith("Abbey").Because("it is the album title");
```

A failure throws the exception of your test framework, with a message that reads like a sentence. The expectation
above fails with:

```text title="Failure message"
Expected that title
starts with "Abbey", because it is the album title,
but it was "Let It Be", which differs at index 0:
   ↓ (actual)
  "Let It Be"
  "Abbey"
   ↑ (expected prefix)
```

- The first line names the subject with the expression you passed to `Expect.That`.
- The second line states the expectation, followed by the reason from `Because(…)`.
- The line starting with "but" describes what was found instead.

## Evaluated when awaited

Every expectation is lazy: it is only evaluated when it is awaited. An expectation that is not awaited never fails,
so only the second line can fail the test:

```csharp no-compile
Expect.That(result).IsTrue();          // never evaluated
await Expect.That(result).IsTrue();    // evaluated, fails when result is false
```

The analyzer rule [aweXpect0001](../07-analyzers.md#awexpect0001) reports an expectation that is neither awaited nor
verified, and [aweXpect0005](../07-analyzers.md#awexpect0005) reports an expectation in an `async void` method or
lambda, whose failure would be thrown after the test has completed.

Awaiting an expectation also returns the value it verified, see [combining](./03-combining.md#using-the-result).

## When you cannot await

[`ref struct`](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/ref-struct) values
can't be used in an `async` method. For a property of such a value, or of a span on a target framework that
[can't pass spans to `Expect.That`](../05-collections/index.md#spans), verify the expectation synchronously, so that the
test method itself can remain synchronous:

```csharp
using aweXpect.Synchronous;

ReadOnlySpan<char> title = "Let It Be".AsSpan();

Expect.That(title.Length).IsEqualTo(9).VerifySynchronously();
// or alternatively:
Synchronously.Verify(Expect.That(title.Length).IsEqualTo(9));
```

:::warning
Only use this where awaiting is impossible: it blocks the thread until the expectation is evaluated.
:::
