# Anatomy of an expectation

Every expectation starts with `Expect.That(subject)`, continues with what you expect and is awaited:

```csharp
string subject = "Let It Be";

await Expect.That(subject).StartsWith("Abbey").Because("it is the album title");
```

The expectation is only evaluated when it is awaited. A failure throws the exception of your test framework, with a
message that reads like a sentence. The expectation above fails with:

```text title="Failure message"
Expected that subject
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
so the following test always passes:

```csharp no-compile
Expect.That(result).IsTrue();          // never evaluated
await Expect.That(result).IsTrue();    // evaluated, fails when result is false
```

The analyzer rule [aweXpect0001](../07-analyzers.md#awexpect0001) reports an expectation that is neither awaited nor
verified, and [aweXpect0005](../07-analyzers.md#awexpect0005) reports an expectation in an `async void` method or
lambda, whose failure would be thrown after the test has completed.

## When you cannot await

`ref struct` values can't be used in an `async` method. For such a subject, or for spans on older target
frameworks, verify the expectation synchronously:

```csharp
using aweXpect.Synchronous;

ReadOnlySpan<char> subject = "foo".AsSpan();

Expect.That(subject.Length).IsEqualTo(3).VerifySynchronously();
```

:::warning
Only use this where awaiting is impossible: it blocks the thread until the expectation is evaluated.
:::
