---
description: The concepts that all expectations share, and how an expectation is built, evaluated and awaited.
---

# Concepts

Describes the concepts that all expectations share: how they are built, evaluated, negated, combined, configured and
limited in time, and how to change their defaults. How aweXpect fits into your test project, e.g. its test frameworks
and target frameworks, is described in [setup and integration](../03-setup/00-index.md).

| Page                                                    | Topics                                                             |
|---------------------------------------------------------|--------------------------------------------------------------------|
| [Anatomy of an expectation](#anatomy-of-an-expectation) | `Expect.That`, failure messages, awaiting, synchronous evaluation  |
| [Negation](./02-negation.md)                            | negated counterparts of expectations                               |
| [Combining expectations](./03-combining.md)             | `And`, `Or`, members, new and different subjects, using the result |
| [`null` subjects](./04-null-subjects.md)                | how expectations and their negations treat a `null` subject        |
| [Options](./05-options.md)                              | `Because`, string options, comparers, equivalency, tolerance       |
| [Time and cancellation](./06-time-and-cancellation.md)  | timeouts, cancellation, waiting, execution time                    |
| [Configuration](./07-configuration.md)                  | customization lifetimes, global defaults, formatting, settings     |

## Anatomy of an expectation

Every expectation starts with `That(subject)`, continues with what you expect and is awaited:

```csharp
string title = "Let It Be";

await That(title).StartsWith("Abbey").Because("the playlist starts with Abbey Road");
```

A failure throws an exception that fails the test, usually the
[assertion exception of your test framework](../03-setup/01-test-frameworks.md#failed-expectations), with a message
that reads like a sentence. The expectation above fails with:

```text title="Failure message"
Expected that title
starts with "Abbey", because the playlist starts with Abbey Road,
but it was "Let It Be", which differs at index 0:
   ↓ (actual)
  "Let It Be"
  "Abbey"
   ↑ (expected prefix)
```

- The first line names the subject with the expression you passed to `Expect.That`.
- The second line states the expectation, followed by the reason from `Because(…)`.
- The line starting with "but" describes what was found instead.

### Evaluated when awaited

Every expectation is lazy: it is only evaluated when it is awaited. An expectation that is not awaited never fails,
so only the second line can fail the test:

```csharp no-compile
That(result).IsTrue();          // never evaluated
await That(result).IsTrue();    // evaluated, fails when result is false
```

The analyzer rule [aweXpect0001](../08-analyzers.md#awexpect0001) reports an expectation that is neither awaited nor
verified, and [aweXpect0005](../08-analyzers.md#awexpect0005) reports an expectation in an `async void` method or
lambda, whose failure would be thrown after the test has completed.

Awaiting an expectation also returns the value it verified, see [combining](./03-combining.md#using-the-result).

### When you cannot await

[`ref struct`](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/ref-struct) values
such as `Span<T>` can't be used in an `async` method. To verify one of their properties in a synchronous test, e.g. on
a target framework that [can't pass spans to `Expect.That`](../05-collections/00-index.md#spans), use:

```csharp
using aweXpect.Synchronous;

ReadOnlySpan<char> title = "Let It Be".AsSpan();

That(title.Length).IsEqualTo(9).VerifySynchronously();
// or alternatively:
Synchronously.Verify(That(title.Length).IsEqualTo(9));
```

The same works for a [combination](./03-combining.md#on-different-subjects) from `Expect.ThatAll` or
`Expect.ThatAny`:

```csharp
using aweXpect.Synchronous;

ReadOnlySpan<char> title = "Let It Be".AsSpan();

Expect.ThatAll(
  That(title.Length).IsEqualTo(9),
  That(title.IsEmpty).IsFalse()).VerifySynchronously();
```

:::warning
It blocks the thread until the expectation is evaluated, so only use it where awaiting is impossible, e.g. for a
`ref struct` such as `Span<T>`, inside a `lock` or in a synchronous callback.
:::
