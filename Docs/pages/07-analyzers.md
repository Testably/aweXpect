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
`async` (such as one converted to an `Action`), in a constructor, a property, an iterator, a `lock` statement or unsafe
code, in a function with `ref` locals or `ref struct` locals, in a method with `ref`, `out`, `in` or pointer
parameters, in a `[Conditional]` method, or in a method or local function whose return type would have to change
while other code depends on it: an override, a virtual method, an interface implementation (also one that only a
derived class declares), or one that is called or used as a method group elsewhere.

For a `ref struct` that cannot be used in an `async` method, verify the expectation synchronously instead, see
[When you cannot await](./03-how-it-works/index.md#when-you-cannot-await).

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
[Delegates](./06-behaviour/01-delegates.md#with-after-throws-has-on-the-exception).

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

See [Delegates](./06-behaviour/01-delegates.md#no-exception).

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
[Collections](./05-collections/index.md#sets).

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

These older targets are affected because they use the .NET Standard 2.0 build of aweXpect. On any target, the rule
also reports a delegate whose `ValueTask` is given explicitly as the type argument, as in
`Expect.That<ValueTask>(() => Act())`. See [Delegates](./06-behaviour/01-delegates.md).

## Nullability suppressor

After an expectation that a `null` subject can never satisfy, such as `IsNotNull()`, the
`aweXpect` package suppresses the nullability warnings CS8600, CS8602, CS8604 and CS8629 for that subject, with the
suppression IDs `aweXpect1001` to `aweXpect1004`. The subject has to be a local variable or a parameter that is
neither a `ref` nor a parameter of a primary constructor, which any member can write to, and the expectation has to be
awaited or verified in a preceding statement in the same method, local function or lambda, without a branch, a label
or a write to the subject in between. A `ref` alias of the subject prevents the suppression, as it can change the
subject unnoticed. Only the warnings are suppressed; the null state of the compiler is unchanged.
Expectations of extension packages take part when they are marked with `[GuaranteesNotNull]`, see
[nullability warnings](./11-extending/02-constraints-and-results.md#nullability-warnings).

```csharp
string? title = new Track().Title;

await Expect.That(title).IsNotNull();
int length = title.Length;   // no CS8602
```

## Metadata generator

The source generator warns with `aweXpect2001` when a type named in
`[assembly: GenerateMetadata(typeof(…))]` yields no registration and stays on the reflection path. See
[Native AOT and trimming](./03-how-it-works/08-native-aot.md#equivalency).

## Test framework adapter

The source generator warns with `aweXpect2002` when a test project on .NET 8 or later sets `<LangVersion>` below 9.
The generated test framework adapter registers itself with a module initializer, which needs C# 9, and on .NET 8 or
later the loaded assemblies are not scanned for it. Without the adapter, aweXpect throws its own exceptions, so a
skipped or inconclusive test is reported as failed. Set `<LangVersion>` to 9 or later, or register the adapter before
the first expectation, see [initialization](./11-extending/05-initialization.md).

```csharp no-compile
TestFrameworkRegistry.Register(new aweXpect.Frameworks.NunitAdapter());
```
