---
description: The target frameworks that aweXpect supports and the features that need .NET 8 or later.
---

# Target frameworks

aweXpect supports test projects on .NET Framework 4.8 and on .NET 6 or later. Most expectations work the same on all
of them, but some need .NET 8 or later, because they build on types or language features that only exist there.

| Target framework of the test project | Support                                                                       |
|--------------------------------------|-------------------------------------------------------------------------------|
| .NET 8 or later                      | all expectations                                                              |
| .NET 6 and .NET 7                    | the .NET Standard 2.0 build, without the ones [listed below](#net-8-or-later) |
| .NET Framework 4.8                   | the .NET Standard 2.0 build, without the ones [listed below](#net-8-or-later) |

A test project on .NET 8 or later can also be published with [Native AOT and trimming](./03-native-aot.md).

## .NET 8 or later

These expectations and behaviors are only available on .NET 8 or later:

- expectations for [`IAsyncEnumerable<T>`](../05-collections/00-index.md);
- [`DateOnly` and `TimeOnly`](../04-values/11-date-time-only.md);
- [`Span<T>` and `ReadOnlySpan<T>`](../05-collections/00-index.md#spans) subjects;
- collection expectations for [`ImmutableArray<T>`](../05-collections/00-index.md#immutable-arrays) subjects;
- [`IsParsableInto`](../04-values/03-string.md#parsing) for strings;
- [`HasBufferSize`](../04-values/08-stream.md#buffer-size) for buffered streams;
- [number expectations](../04-values/02-number.md) for `INumber<T>` types other than `byte`, `sbyte`, `short`,
  `ushort`, `int`, `uint`, `long`, `ulong`, `float`, `double` and `decimal`, e.g. `nint`, `Half`, `Int128` or
  `BigInteger`;
- [`IsPositive` and `IsNegative`](../04-values/02-number.md#positive--negative) for unsigned numbers;
- [delegates](../06-behavior/01-delegates.md) that return a `ValueTask` or `ValueTask<T>`;
- a `Half`, `Int128` or `UInt128` typed as `object` is [equal](../04-values/12-object.md#equality) to a number of
  another numeric type with the same value;
- failure messages show the names `Half.MinValue`, `Half.MaxValue`, `NFloat.MinValue` and `NFloat.MaxValue` instead of
  the number.

## .NET Framework

On .NET Framework, the expectations above are not available, and the rest works as on .NET 8.

<details>
<summary>Dictionaries with a custom comparer</summary>

Before .NET 5, e.g. on .NET Framework, aweXpect cannot read the key comparer of a `ConcurrentDictionary<TKey, TValue>`,
so its keys are compared with their own `Equals`, as for a dictionary whose comparer is unknown.

</details>
