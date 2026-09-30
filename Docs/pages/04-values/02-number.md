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

A `null` subject, e.g. an `int?`, fails every expectation on this page except equality and one of, as the
[rule for `null` subjects](../03-how-it-works/04-null-subjects.md) says, so even `IsNotPositive()` fails for it.

## Equality

You can verify that the number is equal to another one or not:

```csharp
int playCount = 42;

await Expect.That(playCount).IsEqualTo(42);
await Expect.That(playCount).IsNotEqualTo(41);
```

## One of

You can verify that the number is one of many alternatives:

```csharp
int trackCount = 12;

await Expect.That(trackCount).IsOneOf(10, 12, 14);
await Expect.That(trackCount).IsNotOneOf(11, 13);
```

## Greater than / less than

You can verify that the number is greater than or less than (or equal to) another number, or that it is not:

```csharp
int playCount = 42;

await Expect.That(playCount).IsGreaterThan(41);
await Expect.That(playCount).IsGreaterThanOrEqualTo(42);
await Expect.That(playCount).IsLessThan(43);
await Expect.That(playCount).IsLessThanOrEqualTo(42);
await Expect.That(playCount).IsNotGreaterThan(42);
await Expect.That(playCount).IsNotLessThan(42);
```

`NaN` is neither greater nor less than any number, so it satisfies `IsNotGreaterThan(5)` although it fails
`IsLessThanOrEqualTo(5)`. A `NaN` expected value throws an `ArgumentOutOfRangeException`.

## Between

You can verify that the number is between two numbers:

```csharp
int playCount = 42;

await Expect.That(playCount).IsBetween(41).And(43);
await Expect.That(playCount).IsNotBetween(43).And(50);
```

Both bounds belong to the range, so `IsNotBetween(42).And(50)` fails for `42`. A maximum below the minimum or a `NaN`
bound throws an `ArgumentOutOfRangeException`.

## Positive / negative

You can verify that the number is positive or negative, or that it is not:

```csharp
await Expect.That(42).IsPositive();
await Expect.That(-3).IsNegative();
await Expect.That(0).IsNotPositive();
await Expect.That(0).IsNotNegative();
```

Zero and `NaN` are neither positive nor negative, so both `IsNotPositive` and `IsNotNegative` succeed for them.

:::note[.NET 8 or later]
Below .NET 8 these expectations are only available for signed numbers. On .NET 8 or later they are available for every
`INumber<T>`, including unsigned types.
:::

## NaN and infinity

For floating point numbers you can verify that the number is `NaN`, finite or infinite, or that it is not:

```csharp
await Expect.That(float.NaN).IsNaN();
await Expect.That(42.0).IsNotNaN();
await Expect.That(42.0).IsFinite();
await Expect.That(float.PositiveInfinity).IsInfinite();
await Expect.That(double.NaN).IsNotFinite().And.IsNotInfinite();
```

## Tolerance

The comparisons on this page, except the sign, `NaN` and infinity checks, accept a tolerance with `Within`, which
widens the accepted range by the tolerance:

```csharp
double duration = 42.1;

await Expect.That(duration).IsEqualTo(42).Within(0.2)
  .Because("we accept values between 41.8 and 42.2 (42 ± 0.2)");
await Expect.That(duration).IsOneOf(40, 42, 44).Within(0.2)
  .Because("we accept values between 39.8 and 40.2 or 41.8 and 42.2 or 43.8 and 44.2");
await Expect.That(duration).IsGreaterThan(42.2).Within(0.2)
  .Because("we accept values greater than 42.0 (42.2 - 0.2)");
await Expect.That(duration).IsLessThan(42).Within(0.2)
  .Because("we accept values less than 42.2 (42 + 0.2)");
```
