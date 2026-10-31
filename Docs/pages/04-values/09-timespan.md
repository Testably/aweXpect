---
description: Verify a `TimeSpan` for equality, ordering, ranges and sign, with an optional tolerance.
---

# TimeSpan

Describes the possible expectations for `TimeSpan`.

| Expectation                                          | Negated                     | Summary                                      |
|------------------------------------------------------|-----------------------------|----------------------------------------------|
| [`IsEqualTo`](#equality)                             | `IsNotEqualTo`              | equal to the expected value                  |
| [`IsOneOf`](#one-of)                                 | `IsNotOneOf`                | equal to one of the expected values          |
| [`IsGreaterThan`](#greater-than--less-than)          | `IsNotGreaterThan`          | greater than the expected value              |
| [`IsGreaterThanOrEqualTo`](#greater-than--less-than) | `IsNotGreaterThanOrEqualTo` | greater than or equal to the expected value  |
| [`IsLessThan`](#greater-than--less-than)             | `IsNotLessThan`             | less than the expected value                 |
| [`IsLessThanOrEqualTo`](#greater-than--less-than)    | `IsNotLessThanOrEqualTo`    | less than or equal to the expected value     |
| [`IsBetween`](#between)                              | `IsNotBetween`              | between two values, both bounds included     |
| [`IsPositive`](#positive--negative)                  | `IsNotPositive`             | greater than zero                            |
| [`IsNegative`](#positive--negative)                  | `IsNotNegative`             | less than zero                               |

## Equality

You can verify that the `TimeSpan` is equal to another one or not:

```csharp
TimeSpan duration = TimeSpan.FromSeconds(42);

await That(duration).IsEqualTo(TimeSpan.FromSeconds(42));
await That(duration).IsNotEqualTo(TimeSpan.FromSeconds(43));
```

## One of

You can verify that the `TimeSpan` is one of many alternatives:

```csharp
TimeSpan duration = TimeSpan.FromSeconds(42);

await That(duration).IsOneOf([TimeSpan.FromSeconds(40), TimeSpan.FromSeconds(42)]);
await That(duration).IsNotOneOf([TimeSpan.FromSeconds(41), TimeSpan.FromSeconds(43)]);
```

## Greater than / less than

You can verify that the `TimeSpan` is greater than or less than (or equal to) another value, or that it is not:

```csharp
TimeSpan duration = TimeSpan.FromSeconds(42);

await That(duration).IsGreaterThan(TimeSpan.FromSeconds(41));
await That(duration).IsGreaterThanOrEqualTo(TimeSpan.FromSeconds(42));
await That(duration).IsLessThan(TimeSpan.FromSeconds(43));
await That(duration).IsLessThanOrEqualTo(TimeSpan.FromSeconds(42));
await That(duration).IsNotGreaterThan(TimeSpan.FromSeconds(42));
```

The comparisons use the signed value and not the length, so `TimeSpan.FromSeconds(-1)` is greater than
`TimeSpan.FromSeconds(-5)`.

## Between

You can verify that the `TimeSpan` is between two values:

```csharp
TimeSpan duration = TimeSpan.FromSeconds(42);

await That(duration).IsBetween(TimeSpan.FromSeconds(40)).And(TimeSpan.FromSeconds(50));
await That(duration).IsNotBetween(TimeSpan.FromSeconds(43)).And(TimeSpan.FromSeconds(50));
```

## Positive / negative

You can verify that the `TimeSpan` is positive or negative, or that it is not:

```csharp
await That(TimeSpan.FromSeconds(42)).IsPositive();
await That(TimeSpan.FromSeconds(-3)).IsNegative();
await That(TimeSpan.Zero).IsNotPositive().And.IsNotNegative();
```

## Tolerance

The comparisons on this page, except the sign checks, accept a tolerance with `Within`, which widens the accepted range
by the tolerance:

```csharp
using aweXpect.Chronology; // from the aweXpect.Chronology package

TimeSpan duration = 42.Seconds();

await That(duration).IsEqualTo(43.Seconds()).Within(1.Seconds())
  .Because("we accept values between 0:42 and 0:44");
await That(duration).IsOneOf([43.Seconds(), 45.Seconds()]).Within(1.Seconds())
  .Because("we accept values between 0:42 and 0:44 or between 0:44 and 0:46");
await That(duration).IsGreaterThan(43.Seconds()).Within(2.Seconds())
  .Because("we accept values greater than 0:41 (0:43 - 2s)");
await That(duration).IsLessThan(41.Seconds()).Within(2.Seconds())
  .Because("we accept values less than 0:43 (0:41 + 2s)");
await That(duration).IsBetween(43.Seconds()).And(45.Seconds()).Within(1.Seconds())
  .Because("it expands the interval by 1 second");
```

Without `Within`, the [default tolerance](../02-how-it-works/05-options.md#default-tolerance) applies, if one is
configured.
