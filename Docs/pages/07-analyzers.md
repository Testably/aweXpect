# Analyzers

The `aweXpect` package includes analyzers that report common mistakes while you write the test. Errors stop the
build, warnings point at code that compiles but probably does not do what you meant. Most rules come with a code fix.

The analyzers and the source generators need Roslyn 4.8 or later, which ships with the .NET 8 SDK and Visual Studio
2022 17.8. An older compiler skips them with warning `CS8032`.

## aweXpect0001

:::danger[Error]
An expectation is neither awaited nor verified, so it is never evaluated and never fails. A code fix is available.
:::

```csharp
bool value = false;

Expect.That(value).IsTrue();          // reported: never evaluated
await Expect.That(value).IsTrue();    // fixed: awaited, so it fails
```

The code fix awaits the expectation and makes the enclosing method, local function or lambda `async` where needed. It
is not offered where that would not compile or would change a signature that other code depends on.

<details>
<summary>Where the code fix is not offered</summary>

- in a lambda that is not `async`, such as one converted to an `Action`
- in a constructor, a property, an iterator, a `lock` statement or unsafe code
- in a function with `ref` locals or `ref struct` locals
- in a method with `ref`, `out`, `in`, pointer or `ref struct` parameters, or one that returns by reference
- in a `partial` or `[Conditional]` method
- in a method or local function whose return type would have to change while other code depends on it: an override, a
  virtual method, an interface implementation (also one that only a derived class declares), or one that is called or
  used as a method group elsewhere

</details>

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
A `Has…` expectation is used directly after `Throws`, where the sentence "throws a …" needs the `With…` vocabulary. Two
code fixes are available: switch to the `With…` twin, or insert `.Which`.
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
A code fix is available for a delegate that returns a value.
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
`Expect.That<ValueTask>(() => Act())`. An explicit `Task` type argument has the same effect and is reported as well,
as in `Expect.That<Task>(() => ActAsync())`: remove the type argument, so that the task is awaited. See
[Delegates](./06-behaviour/01-delegates.md).

## aweXpect0008

:::warning[Warning]
The value of an expectation is used although an `.Or` combines it with an alternative that has no value of this type,
so the value can be the `default`.
:::

```csharp
object subject = "Yesterday";

int count = await Expect.That(subject).Is<string>().Or.Is<int>();   // reported: 0 when the subject is a string
await Expect.That(subject).Is<string>().Or.Is<int>();               // fixed: the value is not used
```

The value is `default` (also `null` for a reference type that is not annotated as nullable) when another alternative
was met. Do not use it, or check it first and suppress the warning there. The rule is not reported when the value has
the type of the subject, or a base type or an interface of it, because then every alternative returns the subject. See
[Combining expectations](./03-how-it-works/03-combining.md#using-the-result).

## Nullability suppressor

After an expectation that a `null` subject can never satisfy, such as `IsNotNull()`, the
`aweXpect` package suppresses the nullability warnings CS8600, CS8602, CS8604 and CS8629 for that subject, with the
suppression IDs `aweXpect1001` to `aweXpect1004`. Expectations of extension packages take part when they are marked
with `[GuaranteesNotNull]`, see
[nullability warnings](./11-extending/02-constraints-and-results.md#nullability-warnings).

```csharp
string? title = new Track().Title;

await Expect.That(title).IsNotNull();
int length = title.Length;   // no CS8602
```

<details>
<summary>When the suppression applies</summary>

The subject has to be a local variable or a parameter that is neither a `ref` nor a parameter of a primary
constructor, which any member can write to, and the expectation has to be awaited or verified in a preceding statement
in the same method, local function or lambda, without a branch, a label or a write to the subject in that statement or
in between. Assigning the result of an expectation to its own subject, as in
`subject = await Expect.That(subject).IsNotNull();`, does not count as a write, because the result is the subject. A
`ref` alias of the subject prevents the suppression, as it can change the subject unnoticed. Only the warnings are
suppressed; the null state of the compiler is unchanged.

</details>

## Metadata generator

The source generator warns with `aweXpect2001` when a type named in
`[assembly: GenerateMetadata(typeof(…))]` yields no registration and stays on the reflection path. See
[Native AOT and trimming](./03-how-it-works/08-native-aot.md#equivalency).

## Test framework adapter

The source generator warns with `aweXpect2002` when a test project on .NET 8 or later sets `<LangVersion>` below 9.
The generated test framework adapter registers itself with a module initializer, which needs C# 9, and on .NET 8 or
later the loaded assemblies are not scanned for it. Without the adapter, aweXpect throws its own exceptions, so a
skipped or inconclusive test is reported as failed. Set `<LangVersion>` to 9 or later, or register the adapter before
the first expectation, see [initialization](./11-extending/08-initialization.md).

```csharp no-compile
TestFrameworkRegistry.Register(new aweXpect.Frameworks.NunitAdapter());
```
