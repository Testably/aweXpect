# TimeSpan

Describes the possible expectations for `TimeSpan`.

| Expectation                                          | Negated                     | Summary                                  |
|------------------------------------------------------|-----------------------------|------------------------------------------|
| [`IsEqualTo`](#equality)                             | `IsNotEqualTo`              | equal to the expected value              |
| [`IsOneOf`](#one-of)                                 | `IsNotOneOf`                | equal to one of the expected values      |
| [`IsGreaterThan`](#greater-than--less-than)          | `IsNotGreaterThan`          | longer than the expected value           |
| [`IsGreaterThanOrEqualTo`](#greater-than--less-than) | `IsNotGreaterThanOrEqualTo` | at least as long as the expected value   |
| [`IsLessThan`](#greater-than--less-than)             | `IsNotLessThan`             | shorter than the expected value          |
| [`IsLessThanOrEqualTo`](#greater-than--less-than)    | `IsNotLessThanOrEqualTo`    | at most as long as the expected value    |
| [`IsBetween`](#between)                              | `IsNotBetween`              | between two values, both bounds included |
| [`IsPositive`](#positive--negative)                  | `IsNotPositive`             | greater than zero                        |
| [`IsNegative`](#positive--negative)                  | `IsNotNegative`             | less than zero                           |

## Equality

You can verify that the `TimeSpan` is equal to another one or not:

```csharp
TimeSpan duration = TimeSpan.FromSeconds(42);

await Expect.That(duration).IsEqualTo(TimeSpan.FromSeconds(42));
await Expect.That(duration).IsNotEqualTo(TimeSpan.FromSeconds(43));
```

## One of

You can verify that the `TimeSpan` is one of many alternatives:

```csharp
TimeSpan duration = TimeSpan.FromSeconds(42);

await Expect.That(duration).IsOneOf([TimeSpan.FromSeconds(40), TimeSpan.FromSeconds(42)]);
await Expect.That(duration).IsNotOneOf([TimeSpan.FromSeconds(41), TimeSpan.FromSeconds(43)]);
```

## Greater than / less than

You can verify that the `TimeSpan` is greater than or less than (or equal to) another value, or that it is not:

```csharp
TimeSpan duration = TimeSpan.FromSeconds(42);

await Expect.That(duration).IsGreaterThan(TimeSpan.FromSeconds(41));
await Expect.That(duration).IsGreaterThanOrEqualTo(TimeSpan.FromSeconds(42));
await Expect.That(duration).IsLessThan(TimeSpan.FromSeconds(43));
await Expect.That(duration).IsLessThanOrEqualTo(TimeSpan.FromSeconds(42));
await Expect.That(duration).IsNotGreaterThan(TimeSpan.FromSeconds(42));
```

## Between

You can verify that the `TimeSpan` is between two values:

```csharp
TimeSpan duration = TimeSpan.FromSeconds(42);

await Expect.That(duration).IsBetween(TimeSpan.FromSeconds(40)).And(TimeSpan.FromSeconds(50));
await Expect.That(duration).IsNotBetween(TimeSpan.FromSeconds(43)).And(TimeSpan.FromSeconds(50));
```

## Positive / negative

You can verify that the `TimeSpan` is positive or negative, or that it is not:

```csharp
await Expect.That(TimeSpan.FromSeconds(42)).IsPositive();
await Expect.That(TimeSpan.FromSeconds(-3)).IsNegative();
await Expect.That(TimeSpan.Zero).IsNotPositive().And.IsNotNegative();
```

## Tolerance

The comparisons on this page, except the sign checks, accept a tolerance with `Within`, which widens the accepted range
by the tolerance:

```csharp
using aweXpect.Chronology; // from the aweXpect.Chronology package

TimeSpan duration = 42.Seconds();

await Expect.That(duration).IsEqualTo(43.Seconds()).Within(1.Seconds())
  .Because("we accept values between 0:42 and 0:44");
await Expect.That(duration).IsOneOf([43.Seconds(), 45.Seconds()]).Within(1.Seconds())
  .Because("we accept values between 0:42 and 0:44 or between 0:44 and 0:46");
await Expect.That(duration).IsGreaterThan(43.Seconds()).Within(2.Seconds())
  .Because("we accept values greater than 0:41 (0:43 - 2s)");
await Expect.That(duration).IsLessThan(41.Seconds()).Within(2.Seconds())
  .Because("we accept values less than 0:43 (0:41 + 2s)");
await Expect.That(duration).IsBetween(43.Seconds()).And(45.Seconds()).Within(1.Seconds())
  .Because("it expands the interval by 1 second");
```

Without `Within`, the [default tolerance](./10-datetime-offset.md#default-tolerance) applies.
