# Analyzers

The `aweXpect` package includes analyzers that report common mistakes while you write the test. Errors stop the
build, warnings point at code that compiles but probably does not do what you meant. Most rules come with a code fix.

The analyzers and the source generators need Roslyn 4.8 or later, which ships with the .NET 8 SDK and Visual Studio
2022 17.8. An older compiler skips them with warning `CS8032`.

| Rule                                                        | Severity    | Reported when                                                     |
|-------------------------------------------------------------|-------------|-------------------------------------------------------------------|
| [`aweXpect0001`](#awexpect0001)                             | Error       | an expectation is neither awaited nor verified                    |
| [`aweXpect0002`](#awexpect0002)                             | Error       | `Equals` is called on an expectation                              |
| [`aweXpect0003`](#awexpect0003)                             | Warning     | a `Has…` expectation directly follows `Throws`                    |
| [`aweXpect0004`](#awexpect0004)                             | Error       | an expectation for an object is applied to a delegate             |
| [`aweXpect0005`](#awexpect0005)                             | Warning     | an expectation is awaited in an `async void` method or lambda     |
| [`aweXpect0006`](#awexpect0006)                             | Warning     | a collection without a defined order is compared by position      |
| [`aweXpect0007`](#awexpect0007)                             | Error       | a delegate returns a `ValueTask` on a target without its overload |
| [`aweXpect0008`](#awexpect0008)                             | Warning     | the value of an `.Or` combination is used                         |
| [`aweXpect0009`](#awexpect0009)                             | Warning     | the result of an option method is discarded                       |
| [`aweXpect1001` to `aweXpect1004`](#nullability-suppressor) | Suppression | nullability warnings after e.g. `IsNotNull()`                     |
| [`aweXpect2001`](#metadata-generator)                       | Warning     | a type in `[assembly: GenerateMetadata(…)]` gets no registration  |
| [`aweXpect2002`](#test-framework-adapter)                   | Warning     | a test project on .NET 8 or later sets `<LangVersion>` below 9    |
| [`aweXpect4001` to `aweXpect4004`](#expectation-generator)  | Error       | a method cannot be turned into a generated expectation            |
| [`aweXpect4005`](#registration-generator)                   | Error       | a module initializer of an extension cannot be called             |
| [`aweXpect5001`](#experimental-apis)                        | Error       | the experimental `WithTimeSystem(…)` or `ITimeSystem` is used     |

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
void Act() => throw new InvalidOperationException("Yesterday");

await Expect.That(Act).Throws<InvalidOperationException>().HasMessage("Yesterday");    // reported
await Expect.That(Act).Throws<InvalidOperationException>().WithMessage("Yesterday");   // fixed
```

The `Has…` vocabulary belongs after `.Which`. See
[Delegates](./06-behaviour/01-delegates.md#exception-details).

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
[Collections](./05-collections/01-equality.md#sets).

## aweXpect0007

:::danger[Error]
A delegate subject returns a `ValueTask` on a target without `ValueTask` delegate overloads, so the `ValueTask` is
never awaited. A code fix is available.
:::

```csharp
async ValueTask Act() => await Task.Yield();

await Expect.That(() => Act()).DoesNotThrow();            // reported on targets before .NET 8
await Expect.That(() => Act().AsTask()).DoesNotThrow();   // fixed
```

On any target, the rule also reports a delegate whose `ValueTask` is given explicitly as the type argument, as in
`Expect.That<ValueTask>(() => Act())`. An explicit `Task` type argument has the same effect and is reported as well,
as in `Expect.That<Task>(() => ActAsync())`: remove the type argument, so that the task is awaited. See
[Delegates](./06-behaviour/01-delegates.md).

<details>
<summary>Why targets before .NET 8 are affected</summary>

These older targets use the .NET Standard 2.0 build of aweXpect, which has no `ValueTask` delegate overloads.

</details>

## aweXpect0008

:::warning[Warning]
The value of an expectation is used although an `.Or` combines it with an alternative that has no value of this type,
so the value can be the `default`.
:::

```csharp
object subject = "Yesterday";

int count = await Expect.That(subject).Is<string>().Or.Is<int>();   // reported: 0 for a string subject
await Expect.That(subject).Is<string>().Or.Is<int>();               // fixed: the value is not used
```

The value is `default` (also `null` for a reference type that is not annotated as nullable) when another alternative
was met. Do not use it, or check it first and suppress the warning there. The rule is not reported when the value has
the type of the subject, or a base type or an interface of it, because then every alternative returns the subject. See
[Combining expectations](./03-how-it-works/03-combining.md#using-the-result).

## aweXpect0009

:::warning[Warning]
The value returned by an option method of an immutable option, such as a `Quantifier` or the `StringEqualityOptions`,
is discarded, so the call has no effect.
:::

```csharp
using aweXpect.Core;
using aweXpect.Options;

OptionsHolder<Quantifier> quantifier = new(Quantifier.Default);

quantifier.Value.AtLeast(2);                       // reported: the quantifier stays unchanged
quantifier.Value = quantifier.Value.AtLeast(2);    // fixed: the holder gets the new quantifier
quantifier.AtLeast(2);                             // fixed: the option method of the holder replaces its value
```

The options are immutable values, so an option method returns new options and leaves the ones it is called on
unchanged. The rule covers every options type marked with `[ImmutableOptions]`: those of aweXpect, those of the
extension packages, e.g. the `JsonOptions`, also in a lambda with a block body like
`o => { o.IgnoringMember("id"); return o; }`, and those of other extensions that mark their own. A call that is the
whole body of a lambda or local function written with `=>` is not reported, as it is usually there to check that the
option throws. Authors of extensions find more in [Options](./11-extending/05-options.md#options-of-your-own).

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
[Native AOT and trimming](./08-setup/03-native-aot.md#equivalency).

## Test framework adapter

The source generator warns with `aweXpect2002` when a test project on .NET 8 or later sets `<LangVersion>` below 9.
The generated test framework adapter registers itself with a module initializer, which needs C# 9, and on .NET 8 or
later the loaded assemblies are not scanned for it. Without the adapter, aweXpect throws its own exceptions, so a
skipped or inconclusive test is reported as failed. Set `<LangVersion>` to 9 or later, or register the adapter before
the first expectation, see [test framework adapter](./11-extending/11-initialization.md#test-framework-adapter).

```csharp no-compile
TestFrameworkRegistry.Register(new aweXpect.Frameworks.NunitAdapter());
```

## Experimental APIs

`WithTimeSystem(…)` and the `ITimeSystem` it takes, both in `aweXpect.Core.Internal`, let an expectation wait and
measure on a virtual clock instead of in real time, see
[on a virtual clock](./03-how-it-works/06-time-and-cancellation.md#on-a-virtual-clock). They are marked as
experimental with the diagnostic `aweXpect5001`, as they may change in any version. Suppress it in a test project
that uses a virtual time system:

```xml
<PropertyGroup>
  <NoWarn>$(NoWarn);aweXpect5001</NoWarn>
</PropertyGroup>
```

## For extension authors

These rules are reported while you build an extension package on `aweXpect.Core`.

### Expectation generator

The source generator in `aweXpect.Core` reports an error for a method marked with `[GenerateExpectation]` or
`[GenerateCollectionExpectation]` that it cannot turn into an expectation, see [generated expectations](./11-extending/01-generated-expectations.md):

| Rule           | Reported when                                                                                      |
|----------------|----------------------------------------------------------------------------------------------------|
| `aweXpect4001` | the class of the method is not a top-level, non-generic, `static partial` class or is `file`-local |
| `aweXpect4002` | the method, its parameters, the name or the expectation text are not supported, with the reason    |
| `aweXpect4003` | the `Result` names no `static` method that returns a `string` and takes the subject                |
| `aweXpect4004` | the `FormattedItem` names no `static` method that returns a value and takes the item               |

### Registration generator

The source generator in `aweXpect.Core` reports the error `aweXpect4005` when a name in the MSBuild property
`AweXpectModuleInitializers` names no accessible `static` method without parameters, and leaves the call out of the
module initializer. The build targets of an extension set this property, see
[registration from the test project](./11-extending/11-initialization.md#registration-from-the-test-project).
