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
[Fact]
public async Task IsInLibrary_WhenAlbumIsMissing_ShouldReturnFalse()
{
  bool result = IsInLibrary("Unknown Album");
  
  await Expect.That(result).IsFalse();
}
```

If it fails, it will throw a framework-specific exception with the following message:

```text title="Failure message"
Expected that result
is False,
but it was True
```

## Add a reason

You can add a reason for all expectations that will be included in the exception message:

```csharp
[Fact]
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

## Target frameworks

aweXpect supports .NET Framework 4.8 and .NET Standard 2.0 as well as .NET 8 and later. The following features are
only available on .NET 8 or later:

- expectations for [`IAsyncEnumerable<T>`](./05-collections/index.md);
- [`DateOnly` and `TimeOnly`](./04-values/11-date-time-only.md);
- [`Span<T>` and `ReadOnlySpan<T>`](./05-collections/index.md#spans) subjects;
- [`IsParsableInto`](./04-values/03-string.md#parsing) for strings;
- [`HasBufferSize`](./04-values/08-stream.md#buffer-size) for buffered streams;
- [number expectations](./04-values/02-number.md) for `INumber<T>` types other than `byte`, `sbyte`, `short`,
  `ushort`, `int`, `uint`, `long`, `ulong`, `float`, `double` and `decimal`, e.g. `nint`, `Half`, `Int128` or
  `BigInteger`;
- [`IsPositive` and `IsNegative`](./04-values/02-number.md#positive--negative) for unsigned numbers;
- a `Half`, `Int128` or `UInt128` typed as `object` being [equal](./04-values/12-object.md#equality) to a number of
  another numeric type with the same value;
- the names `Half.MinValue`, `Half.MaxValue`, `NFloat.MinValue` and `NFloat.MaxValue` in failure messages, which
  otherwise show the number;
- [delegates](./06-behaviour/01-delegates.md) that return a `ValueTask` or `ValueTask<T>`;
- the custom comparer of an `ImmutableHashSet<T>`, `ImmutableSortedSet<T>` or `FrozenSet<T>`
  ([sets](./05-collections/index.md#sets)).

Before .NET 5, e.g. on .NET Framework, the key comparer of a `ConcurrentDictionary<TKey, TValue>` cannot be read. Its
[keys](./05-collections/04-dictionaries.md#keys-and-values) are then compared by their own `Equals`, and so are the
matched keys in [`IsEqualTo`](./05-collections/04-dictionaries.md#equality) and in
[equivalency](./04-values/13-equivalency.md#collections-and-dictionaries), so two expected keys that the comparer
considers the same are only noticed when the entry counts differ. The expected keys are still looked up through the
dictionary.

## Next steps

- [How aweXpect works](./03-how-it-works/index.md) explains how expectations are evaluated, negated and combined,
  how they treat `null`, and the options they share.
- The pages for [values](./04-values/index.md), [collections](./05-collections/index.md),
  [delegates](./06-behaviour/01-delegates.md), [events](./06-behaviour/03-events.md) and
  [equivalency](./04-values/13-equivalency.md) list the available expectations.
