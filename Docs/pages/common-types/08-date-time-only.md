# DateOnly / TimeOnly

Describes the possible expectations for `DateOnly` and `TimeOnly`.

:::note[`DateOnly` tolerances are counted in days]
A `DateOnly` has no time of day, so a tolerance must be a whole number of days. Anything else, for example
`Within(TimeSpan.FromHours(23))`, throws an `ArgumentOutOfRangeException` as soon as it is specified instead of silently
rounding down to a tolerance you did not ask for.

The [default tolerance](/docs/expectations/common-types/datetime-offset#default-tolerance) is shared with the other time
types, so it is not rejected: only its whole days apply to a `DateOnly`, and a default below one day has no effect.
:::

:::note[`TimeOnly` is a clock face]
A `TimeOnly` has no date, so midnight is not a boundary for equality and ranges, but it stays one for ordering:

- `IsEqualTo`, `IsNotEqualTo` and `IsOneOf` use the shortest distance around the clock face, so `00:00` and `23:59` are
  one minute apart. That distance never exceeds 12 hours, so a tolerance of 12 hours or more accepts every time.
- `IsBetween` runs clockwise from the minimum to the maximum, so a range from `23:00` to `01:00` contains `00:00`.
- `IsAfter`, `IsOnOrAfter`, `IsBefore` and `IsOnOrBefore` compare the times as they are, so `00:00` is never after
  `23:00`. Their tolerance only ever widens the accepted range and never wraps around midnight.

:::

## Equality

You can verify that the `DateOnly` or `TimeOnly` is equal to another one or not:

```csharp
DateOnly subject = new DateOnly(2024, 12, 24);

await Expect.That(subject).IsEqualTo(new DateOnly(2024, 12, 24));
await Expect.That(subject).IsNotEqualTo(new DateOnly(2024, 12, 23));
```

```csharp
TimeOnly subject = new TimeOnly(14, 15, 16);

await Expect.That(subject).IsEqualTo(new TimeOnly(14, 15, 16));
await Expect.That(subject).IsNotEqualTo(new TimeOnly(13, 15, 16));
```

You can also specify a tolerance:

```csharp
DateOnly subject = new DateOnly(2024, 12, 24);

await Expect.That(subject).IsEqualTo(new DateOnly(2024, 12, 23)).Within(TimeSpan.FromDays(1))
  .Because("we accept values between 2024-12-22 and 2024-12-24");
```

```csharp
TimeOnly subject = new TimeOnly(14, 15, 16);

await Expect.That(subject).IsEqualTo(new TimeOnly(14, 15, 17)).Within(TimeSpan.FromSeconds(1))
  .Because("we accept values between 14:15:16 and 14:15:18");
```

## One of

You can verify that the `DateOnly` or `TimeOnly` is one of many alternatives:

```csharp
DateOnly subject = new DateOnly(2024, 12, 24);

await Expect.That(subject).IsOneOf([new DateOnly(2024, 12, 23), new DateOnly(2024, 12, 24)]);
await Expect.That(subject).IsNotOneOf([new DateOnly(2024, 12, 23), new DateOnly(2024, 12, 25)]);
```

```csharp
TimeOnly subject = new TimeOnly(14, 15, 16);

await Expect.That(subject).IsOneOf([new TimeOnly(14, 15, 15), new TimeOnly(14, 15, 16)]);
await Expect.That(subject).IsNotOneOf([new TimeOnly(13, 15, 16), new TimeOnly(13, 14, 15)]);
```

You can also specify a tolerance:

```csharp
DateOnly subject = new DateOnly(2024, 12, 24);

await Expect.That(subject).IsOneOf([new DateOnly(2024, 12, 23)]).Within(TimeSpan.FromDays(1))
  .Because("we accept values between 2024-12-22 and 2024-12-24");
```

```csharp
TimeOnly subject = new TimeOnly(14, 15, 16);

await Expect.That(subject).IsOneOf([new TimeOnly(14, 15, 17)]).Within(TimeSpan.FromSeconds(1))
  .Because("we accept values between 14:15:16 and 14:15:18");
```

## After

You can verify that the `DateOnly` or `TimeOnly` is (on or) after another value

```csharp
DateOnly subject = DateOnly.FromDateTime(DateTime.Now);

await Expect.That(subject).IsAfter(new DateOnly(2024, 1, 1));
await Expect.That(subject).IsOnOrAfter(new DateOnly(2024, 1, 1));
```

```csharp
TimeOnly subject = TimeOnly.FromDateTime(DateTime.Now);

await Expect.That(subject).IsAfter(new TimeOnly(0, 0, 0));
await Expect.That(subject).IsOnOrAfter(new TimeOnly(0, 0, 0));
```

You can also specify a tolerance:

```csharp
DateOnly subject = DateOnly.FromDateTime(DateTime.Now);

await Expect.That(subject).IsAfter(DateOnly.FromDateTime(DateTime.Now)).Within(TimeSpan.FromDays(1));
```

```csharp
TimeOnly subject = TimeOnly.FromDateTime(DateTime.Now);

await Expect.That(subject).IsAfter(TimeOnly.FromDateTime(DateTime.Now)).Within(TimeSpan.FromSeconds(1));
```

## Before

You can verify that the `DateOnly` or `TimeOnly` is (on or) before another value

```csharp
DateOnly subject = DateOnly.FromDateTime(DateTime.Now);

await Expect.That(subject).IsBefore(new DateOnly(2124, 12, 31));
await Expect.That(subject).IsOnOrBefore(new DateOnly(2124, 12, 31));
```

```csharp
TimeOnly subject = TimeOnly.FromDateTime(DateTime.Now);

await Expect.That(subject).IsBefore(new TimeOnly(23, 59, 59));
await Expect.That(subject).IsOnOrBefore(new TimeOnly(23, 59, 59));
```

You can also specify a tolerance:

```csharp
DateOnly subject = DateOnly.FromDateTime(DateTime.Now);

await Expect.That(subject).IsBefore(DateOnly.FromDateTime(DateTime.Now)).Within(TimeSpan.FromDays(1));
```

```csharp
TimeOnly subject = TimeOnly.FromDateTime(DateTime.Now);

await Expect.That(subject).IsBefore(TimeOnly.FromDateTime(DateTime.Now)).Within(TimeSpan.FromSeconds(1));
```

## Between

You can verify that the `DateOnly` or `TimeOnly` is between two values:

```csharp
DateOnly subject = DateOnly.FromDateTime(DateTime.Now);

await Expect.That(subject).IsBetween(new DateOnly(2024, 1, 1)).And(new DateOnly(2123, 12, 31));
```

```csharp
using aweXpect.Chronology; // from the aweXpect.Chronology package

TimeOnly subject = TimeOnly.FromDateTime(DateTime.Now);

await Expect.That(subject)
    .IsBetween(TimeOnly.FromDateTime(DateTime.Now).Add(-2.Seconds()))
    .And(TimeOnly.FromDateTime(DateTime.Now).Add(2.Seconds()));
```

You can also specify a tolerance:

```csharp
TimeOnly subject = TimeOnly.FromDateTime(DateTime.Now);

await Expect.That(subject)
	.IsBetween(TimeOnly.FromDateTime(DateTime.Now))
    .And(TimeOnly.FromDateTime(DateTime.Now))
	.Within(2.Seconds())
  .Because("it should have taken less than two seconds");
```

## Properties

You can verify the properties of the `DateTime`:

```csharp
DateOnly subject = new DateOnly(2024, 12, 31);

await Expect.That(subject).HasYear(2024);
await Expect.That(subject).HasMonth(12);
await Expect.That(subject).HasDay(31);
// or more explicit
await Expect.That(subject).HasYear().EqualTo(2024);
await Expect.That(subject).HasMonth().EqualTo(12);
await Expect.That(subject).HasDay().EqualTo(31);
```

You can verify the properties of the `TimeOnly`:

```csharp
TimeOnly subject = new TimeOnly(15, 16, 17, 189);

await Expect.That(subject).HasHour(15);
await Expect.That(subject).HasMinute(16);
await Expect.That(subject).HasSecond(17);
await Expect.That(subject).HasMillisecond(189);
// or more explicit
await Expect.That(subject).HasHour().EqualTo(15);
await Expect.That(subject).HasMinute().EqualTo(16);
await Expect.That(subject).HasSecond().EqualTo(17);
await Expect.That(subject).HasMillisecond().EqualTo(189);
```

All property verifications support the following comparisons:

```csharp
DateOnly subject = new DateOnly(2024, 12, 31);

await Expect.That(subject).HasYear().EqualTo(2024);
await Expect.That(subject).HasYear().NotEqualTo(2020);
await Expect.That(subject).HasYear().GreaterThan(2023);
await Expect.That(subject).HasYear().GreaterThanOrEqualTo(2024);
await Expect.That(subject).HasYear().LessThanOrEqualTo(2024);
await Expect.That(subject).HasYear().LessThan(2025);
await Expect.That(subject).HasYear().Between(2000).And(2024);
```
