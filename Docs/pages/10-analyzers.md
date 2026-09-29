# Analyzers

The `aweXpect` package includes analyzers that report common mistakes while you write the test. Most of them come
with a code fix.

## aweXpect0001

**Expectations must be awaited or verified** (Error, code fix available)

An expectation is only evaluated when it is awaited or verified, so `Expect.That(value).IsTrue();` on its own never
fails. Await the expectation (`await Expect.That(value).IsTrue();`) or call `.Verify()` on it in synchronous code.

## aweXpect0002

**Replace "Equals" with "IsEqualTo"** (Error)

`object.Equals` on an expectation compares the expectation object itself and does not verify anything. Use
`IsEqualTo` instead, as in `await Expect.That(subject).IsEqualTo(expected);`.

## aweXpect0003

**Use "With…" instead of "Has…" directly after "Throws"** (Warning, code fix available)

Directly after `Throws` the expectation continues the sentence "throws a …", so it must use the `With…` vocabulary,
e.g. `Throws<MyException>().WithMessage("foo")`. The `Has…` vocabulary belongs after `.Which`. See
[Delegates](./04-delegates.md#with-after-throws-has-on-the-exception).

## aweXpect0004

**Expectations for an object must not be applied to a delegate subject** (Error, code fix available)

An expectation for an object, such as `IsEqualTo`, that is applied to a delegate checks the delegate instead of what
it does. Insert `.DoesNotThrow().WhoseResult` to make the expectation about the returned value, as in
`Expect.That(() => sut.Count()).DoesNotThrow().WhoseResult.IsEqualTo(1)`. See
[Delegates](./04-delegates.md#no-exception).

## aweXpect0005

**Expectations in an async void method or lambda are not observed by the test** (Warning)

An `async void` method, or an `async` lambda that is converted to a void-returning delegate (e.g. in
`list.ForEach(async x => await Expect.That(x).IsTrue())`), returns before the expectation is evaluated, so its
failure is thrown after the test has completed. Return a `Task` instead, or await the expectation in the test method
itself.

## aweXpect0006

**Collections without a defined order must not be compared by position** (Warning, code fix available)

A set or a dictionary enumerates its items in an order that is an implementation detail. `IsEqualTo`, `Contains` and
`IsContainedIn` compare by position unless `.InAnyOrder()` is appended, which the code fix does. `StartsWith`,
`EndsWith` and `IgnoringInterspersedItems()` are about the order itself and have no meaning for such a collection.
See [Collections](./03-collections/index.md).

## aweXpect0007

**A "ValueTask" returned by a delegate subject is never awaited** (Error, code fix available)

On .NET Framework, .NET Standard 2.0, .NET 6 and .NET 7 there are no overloads for delegates that return a
`ValueTask`, so the returned `ValueTask` is never awaited. Return a `Task` instead, as in `() => Act().AsTask()`,
which the code fix does. See [Delegates](./04-delegates.md).
