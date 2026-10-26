# Getting started

## Installation

1. Install the [`aweXpect`](https://www.nuget.org/packages/aweXpect) nuget package
   ```ps
   dotnet add package aweXpect
   ```

2. Add the following `using` statement:
   ```csharp
   using aweXpect;
   ```
   This brings the static `Expect` class and lots of extension methods into scope.

3. Simplify expectations (optional)  
   If you want to simplify the assertions, you can add a `global using static aweXpect.Expect;` statement anywhere in
   your test project.
   This allows writing a more concise syntax:
   ```csharp
   //    ↓ Default behaviour
   await Expect.That(subject).IsTrue();
   await That(subject).IsTrue();
   //    ↑ With global static
   ```

## Write your first expectation

Every expectation starts with `Expect.That(subject)`, continues with what you expect and is awaited:

```csharp
[Test]
public async Task IsInLibrary_WhenAlbumIsMissing_ShouldReturnFalse()
{
  bool result = IsInLibrary("Unknown Album");
  
  await Expect.That(result).IsFalse();
}
```

If it fails, it throws [an exception that fails the test](./03-how-it-works/09-test-frameworks.md#failed-expectations)
with the following message:

```text title="Failure message"
Expected that result
is False,
but it was True
```

## Add a reason

You can add a reason for all expectations that will be included in the exception message:

```csharp
[Test]
public async Task IsInLibrary_WhenAlbumIsMissing_ShouldReturnFalse()
{
  bool result = IsInLibrary("Unknown Album");
  
  await Expect.That(result).IsFalse().Because("the album is not in the library");
}
```

The reason then appears in the failure message:

```text title="Failure message"
Expected that result
is False, because the album is not in the library,
but it was True
```

## Find the expectation you need

The expectations are grouped by the type of the subject:

| What you test                                                                                                                                                                                                                                                                                                                                                                                                                                                                      | Where to look                                  |
|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|------------------------------------------------|
| [`bool`](./04-values/01-boolean.md), [numbers](./04-values/02-number.md), [`string`](./04-values/03-string.md), [`char`](./04-values/04-char.md), [enums](./04-values/05-enum.md), [`Guid`](./04-values/06-guid.md), [`Version`](./04-values/07-version.md), [`Stream`](./04-values/08-stream.md), [`TimeSpan`](./04-values/09-timespan.md), [`DateTime`](./04-values/10-datetime-offset.md), [any object](./04-values/12-object.md), [equivalency](./04-values/13-equivalency.md) | [Values](./04-values/index.md)                 |
| `IEnumerable<T>`, arrays, [dictionaries](./05-collections/04-dictionaries.md)                                                                                                                                                                                                                                                                                                                                                                                                      | [Collections](./05-collections/index.md)       |
| [delegates and exceptions](./06-behaviour/01-delegates.md), [`Task`](./06-behaviour/02-tasks.md), [events](./06-behaviour/03-events.md), [callbacks](./06-behaviour/04-callbacks.md)                                                                                                                                                                                                                                                                                               | [Behaviour](./06-behaviour/index.md)           |
| JSON, XML, HTTP, `Uri`, `IPAddress`, reflection, snapshots                                                                                                                                                                                                                                                                                                                                                                                                                         | [Extension packages](./07-extensions/index.md) |

## Target frameworks

aweXpect supports .NET Framework 4.8, .NET Standard 2.0 and .NET 8 or later; some features need .NET 8 or later.
A test project can also be published with [Native AOT and trimming](./03-how-it-works/08-native-aot.md).

<details>
<summary>Differences between the target frameworks</summary>

Only on .NET 8 or later:

- expectations for [`IAsyncEnumerable<T>`](./05-collections/index.md);
- [`DateOnly` and `TimeOnly`](./04-values/11-date-time-only.md);
- [`Span<T>` and `ReadOnlySpan<T>`](./05-collections/index.md#spans) subjects;
- collection expectations for [`ImmutableArray<T>`](./05-collections/index.md#immutable-arrays) subjects;
- [`IsParsableInto`](./04-values/03-string.md#parsing) for strings;
- [`HasBufferSize`](./04-values/08-stream.md#buffer-size) for buffered streams;
- [number expectations](./04-values/02-number.md) for `INumber<T>` types other than `byte`, `sbyte`, `short`,
  `ushort`, `int`, `uint`, `long`, `ulong`, `float`, `double` and `decimal`, e.g. `nint`, `Half`, `Int128` or
  `BigInteger`;
- [`IsPositive` and `IsNegative`](./04-values/02-number.md#positive--negative) for unsigned numbers;
- [delegates](./06-behaviour/01-delegates.md) that return a `ValueTask` or `ValueTask<T>`;
- a `Half`, `Int128` or `UInt128` typed as `object` is [equal](./04-values/12-object.md#equality) to a number of
  another numeric type with the same value;
- failure messages show the names `Half.MinValue`, `Half.MaxValue`, `NFloat.MinValue` and `NFloat.MaxValue` instead of
  the number.

Before .NET 5, e.g. on .NET Framework, aweXpect cannot read the key comparer of a `ConcurrentDictionary<TKey, TValue>`,
so its keys are compared with their own `Equals`, as for a dictionary whose comparer is unknown.

</details>

## Next steps

- [How aweXpect works](./03-how-it-works/index.md) explains how expectations are evaluated, negated and combined,
  how they treat `null`, and the options they share.
- [Find the expectation you need](#find-the-expectation-you-need) leads you to the page that lists the expectations
  for your subject: values, collections, behaviour or an extension package.
- [Extending aweXpect](./11-extending/index.md) shows how to add expectations for your own types.
