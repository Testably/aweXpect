# Analyzers

The `aweXpect` package includes analyzers that report common mistakes while you write the test. Errors stop the
build, warnings point at code that compiles but probably does not do what you meant. Most rules come with a code fix.

## aweXpect0001

:::danger[Error]
An expectation is neither awaited nor verified, so it is never evaluated and never fails. A code fix is available.
:::

```csharp
bool value = false;

Expect.That(value).IsTrue();          // reported: never evaluated
await Expect.That(value).IsTrue();    // fixed: awaited, so it fails
```

The code fix awaits the expectation in its innermost enclosing method, local function or lambda. A method or local
function that is not `async` yet is made `async`: a `void` or `T` return type becomes `Task` or `Task<T>`, and the
`return` statements of a method that already returns a task are adjusted where needed. The fix is not offered where
the result would not compile or would change a signature that other code depends on, e.g. in a lambda that is not
`async` (such as one converted to an `Action`), in a constructor, a property, an iterator or a `lock` statement, in a
method with `ref`, `out` or `in` parameters, or in an override, a virtual method or an interface implementation whose
return type would have to change.

For a `ref struct` that cannot be used in an `async` method, verify the expectation synchronously instead, see
[Ref struct](./advanced/05-ref-struct.md).

## aweXpect0002

:::danger[Error]
`Equals` is called on an expectation. It compares the expectation object itself and does not verify anything.
:::

```csharp
int subject = 1;

bool isEqual = Expect.That(subject).Equals(2);   // reported: verifies nothing
await Expect.That(subject).IsEqualTo(2);         // fixed
```

## aweXpect0003

:::warning[Warning]
A `Has…` expectation is used directly after `Throws`, where the sentence "throws a …" needs the `With…` vocabulary. A
code fix is available.
:::

```csharp
void Act() => throw new InvalidOperationException("foo");

await Expect.That(Act).Throws<InvalidOperationException>().HasMessage("foo");    // reported
await Expect.That(Act).Throws<InvalidOperationException>().WithMessage("foo");   // fixed
```

The `Has…` vocabulary belongs after `.Which`. See
[Delegates](./04-delegates.md#with-after-throws-has-on-the-exception).

## aweXpect0004

:::danger[Error]
An expectation for an object is applied to a delegate subject, so it checks the delegate instead of what it returns.
A code fix is available.
:::

```csharp
int Act() => 3;

await Expect.That(Act).IsEqualTo(3);                              // reported: checks the delegate
await Expect.That(Act).DoesNotThrow().WhoseResult.IsEqualTo(3);   // fixed: checks the returned value
```

See [Delegates](./04-delegates.md#no-exception).

## aweXpect0005

:::warning[Warning]
An expectation is awaited in an `async void` method or lambda, so its failure is thrown after the test has completed
and is not observed by the test.
:::

```csharp
List<int> values = [1, 2, 3];

values.ForEach(async x => await Expect.That(x).IsGreaterThan(0));   // reported: async void lambda
foreach (int x in values)
{
    await Expect.That(x).IsGreaterThan(0);                            // fixed: awaited by the test
}
```

Return a `Task` instead, or await the expectation in the test method itself.

## aweXpect0006

:::warning[Warning]
A collection without a defined order, such as a set or a dictionary, is compared by position. A code fix is available.
:::

```csharp
HashSet<int> values = [1, 2, 3];

await Expect.That(values).IsEqualTo([1, 2, 3]);                  // reported: depends on the enumeration order
await Expect.That(values).IsEqualTo([1, 2, 3]).InAnyOrder();     // fixed
```

`IsEqualTo`, `Contains` and `IsContainedIn` compare by position unless `.InAnyOrder()` is appended, which the code fix
does. `StartsWith`, `EndsWith` and `IgnoringInterspersedItems()` are about the order itself and have no meaning for
such a collection, so `.InAnyOrder()` does not help there. Sorted collections are not reported. See
[Collections](./03-collections/index.md).

## aweXpect0007

:::danger[Error]
A delegate subject returns a `ValueTask` on a target without `ValueTask` delegate overloads, so the `ValueTask` is
never awaited. A code fix is available.
:::

```csharp
async ValueTask Act() => await Task.Yield();

await Expect.That(() => Act()).DoesNotThrow();            // reported on .NET Framework, .NET Standard 2.0, .NET 6 and .NET 7
await Expect.That(() => Act().AsTask()).DoesNotThrow();   // fixed
```

Only these older targets are affected, as they use the .NET Standard 2.0 build of aweXpect. See
[Delegates](./04-delegates.md).
