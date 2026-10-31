---
description: Verify numbers for equality, ordering, ranges, sign, NaN and infinity, with an optional tolerance.
---

# Number

Describes the possible expectations for numbers.

| Expectation                                          | Negated                     | Summary                                                     |
|------------------------------------------------------|-----------------------------|-------------------------------------------------------------|
| [`IsEqualTo`](#equality)                             | `IsNotEqualTo`              | equal to the expected number                                |
| [`IsOneOf`](#one-of)                                 | `IsNotOneOf`                | equal to one of the expected numbers                        |
| [`IsGreaterThan`](#greater-than--less-than)          | `IsNotGreaterThan`          | greater than the expected number                            |
| [`IsGreaterThanOrEqualTo`](#greater-than--less-than) | `IsNotGreaterThanOrEqualTo` | greater than or equal to the expected number                |
| [`IsLessThan`](#greater-than--less-than)             | `IsNotLessThan`             | less than the expected number                               |
| [`IsLessThanOrEqualTo`](#greater-than--less-than)    | `IsNotLessThanOrEqualTo`    | less than or equal to the expected number                   |
| [`IsBetween`](#between)                              | `IsNotBetween`              | between two numbers, both bounds included                   |
| [`IsPositive`](#positive--negative)                  | `IsNotPositive`             | greater than zero                                           |
| [`IsNegative`](#positive--negative)                  | `IsNotNegative`             | less than zero                                              |
| [`IsNaN`](#nan-and-infinity)                         | `IsNotNaN`                  | `NaN` (floating point numbers only)                         |
| [`IsFinite`](#nan-and-infinity)                      | `IsNotFinite`               | neither infinite nor `NaN` (floating point numbers only)    |
| [`IsInfinite`](#nan-and-infinity)                    | `IsNotInfinite`             | positive or negative infinity (floating point numbers only) |

:::info[.NET 8 or later]
Below .NET 8 these expectations only accept `byte`, `sbyte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`,
`float`, `double` and `decimal`, and [`IsPositive` and `IsNegative`](#positive--negative) only the signed ones of them.
On .NET 8 or later they also accept other `INumber<T>` types, e.g. `char`, `nint`, `Half`, `Int128` or `BigInteger`.
:::

## Equality

You can verify that the number is equal to another one or not:

```csharp
int playCount = 42;

await That(playCount).IsEqualTo(42);
await That(playCount).IsNotEqualTo(41);
```

## One of

You can verify that the number is one of many alternatives:

```csharp
int trackCount = 12;

await That(trackCount).IsOneOf(10, 12, 14);
await That(trackCount).IsNotOneOf(11, 13);
```

## Greater than / less than

You can verify that the number is greater than or less than (or equal to) another number, or that it is not:

```csharp
int playCount = 42;

await That(playCount).IsGreaterThan(41);
await That(playCount).IsGreaterThanOrEqualTo(42);
await That(playCount).IsLessThan(43);
await That(playCount).IsLessThanOrEqualTo(42);
await That(playCount).IsNotGreaterThan(42);
await That(playCount).IsNotLessThan(42);
```

## Between

You can verify that the number is between two numbers:

```csharp
int playCount = 42;

await That(playCount).IsBetween(41).And(43);
await That(playCount).IsNotBetween(43).And(50);
```

Both bounds belong to the range, so `IsNotBetween(42).And(50)` fails for `42`.

## Positive / negative

You can verify that the number is positive or negative, or that it is not:

```csharp
await That(42).IsPositive();
await That(-3).IsNegative();
await That(0).IsNotPositive();
await That(0).IsNotNegative();
```

Zero and `NaN` are neither positive nor negative, so both `IsNotPositive` and `IsNotNegative` succeed for them.

## NaN and infinity

For floating point numbers you can verify that the number is `NaN`, finite or infinite, or that it is not:

```csharp
await That(float.NaN).IsNaN();
await That(42.0).IsNotNaN();
await That(42.0).IsFinite();
await That(float.PositiveInfinity).IsInfinite();
await That(double.NaN).IsNotFinite().And.IsNotInfinite();
```

`NaN` is neither greater nor less than any number, so it satisfies `IsNotGreaterThan(5)` although it fails
`IsLessThanOrEqualTo(5)`.

## Tolerance

The comparisons on this page, except the sign, `NaN` and infinity checks, accept a tolerance with `Within`, which
widens the accepted range by the tolerance:

```csharp
double duration = 42.1;

await That(duration).IsEqualTo(42).Within(0.2)
  .Because("we accept values between 41.8 and 42.2 (42 ± 0.2)");
await That(duration).IsOneOf(40, 42, 44).Within(0.2)
  .Because("we accept values between 39.8 and 40.2 or 41.8 and 42.2 or 43.8 and 44.2");
await That(duration).IsGreaterThan(42.2).Within(0.2)
  .Because("we accept values greater than 42.0 (42.2 - 0.2)");
await That(duration).IsLessThan(42).Within(0.2)
  .Because("we accept values less than 42.2 (42 + 0.2)");
```

For `double`, `float`, `decimal` and `Half`, `Percent()` directly after `Within` interprets the tolerance as a
percentage of the expected value, or of each bound:

```csharp
double durationSeconds = 1040.0;

await That(durationSeconds).IsEqualTo(1000).Within(5).Percent()
  .Because("we accept values between 950 and 1050 (1000 ± 5%)");
await That(durationSeconds).IsBetween(1000).And(2000).Within(5).Percent()
  .Because("we accept values between 950 and 2100");
```

The percentage is taken of the absolute value, so `IsEqualTo(-1000).Within(5).Percent()` accepts values between -1050
and -950. An expected value of zero only accepts zero.
