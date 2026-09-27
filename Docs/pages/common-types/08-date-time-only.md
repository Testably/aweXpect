import Tabs from '@theme/Tabs';
import TabItem from '@theme/TabItem';
import PropertyComparisons from '../_property-comparisons.md';

# DateOnly / TimeOnly

Describes the possible expectations for `DateOnly` and `TimeOnly`.

Every expectation has a negated counterpart (`IsNot…`/`DoesNot…`), except the `Has…` properties, which take a negated
comparison instead (e.g. `HasYear().NotEqualTo(2020)`).

:::note[`DateOnly` tolerances are counted in days]
A `DateOnly` has no time of day, so a tolerance must be a whole number of days. Anything else, for example
`Within(TimeSpan.FromHours(23))`, throws an `ArgumentOutOfRangeException` as soon as it is specified instead of silently
rounding down to a tolerance you did not ask for.

The [default tolerance](./09-datetime-offset.md#default-tolerance) is shared with the other time
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

<Tabs groupId="date-time-only">
<TabItem value="DateOnly" label="DateOnly" default>

```csharp
DateOnly subject = new DateOnly(2024, 12, 24);

await Expect.That(subject).IsEqualTo(new DateOnly(2024, 12, 24));
await Expect.That(subject).IsNotEqualTo(new DateOnly(2024, 12, 23));
```

</TabItem>
<TabItem value="TimeOnly" label="TimeOnly">

```csharp
TimeOnly subject = new TimeOnly(14, 15, 16);

await Expect.That(subject).IsEqualTo(new TimeOnly(14, 15, 16));
await Expect.That(subject).IsNotEqualTo(new TimeOnly(13, 15, 16));
```

</TabItem>
</Tabs>

You can also specify a tolerance:

<Tabs groupId="date-time-only">
<TabItem value="DateOnly" label="DateOnly" default>

```csharp
DateOnly subject = new DateOnly(2024, 12, 24);

await Expect.That(subject).IsEqualTo(new DateOnly(2024, 12, 23)).Within(TimeSpan.FromDays(1))
  .Because("we accept values between 2024-12-22 and 2024-12-24");
```

</TabItem>
<TabItem value="TimeOnly" label="TimeOnly">

```csharp
TimeOnly subject = new TimeOnly(14, 15, 16);

await Expect.That(subject).IsEqualTo(new TimeOnly(14, 15, 17)).Within(TimeSpan.FromSeconds(1))
  .Because("we accept values between 14:15:16 and 14:15:18");
```

</TabItem>
</Tabs>

## One of

You can verify that the `DateOnly` or `TimeOnly` is one of many alternatives:

<Tabs groupId="date-time-only">
<TabItem value="DateOnly" label="DateOnly" default>

```csharp
DateOnly subject = new DateOnly(2024, 12, 24);

await Expect.That(subject).IsOneOf([new DateOnly(2024, 12, 23), new DateOnly(2024, 12, 24)]);
await Expect.That(subject).IsNotOneOf([new DateOnly(2024, 12, 23), new DateOnly(2024, 12, 25)]);
```

</TabItem>
<TabItem value="TimeOnly" label="TimeOnly">

```csharp
TimeOnly subject = new TimeOnly(14, 15, 16);

await Expect.That(subject).IsOneOf([new TimeOnly(14, 15, 15), new TimeOnly(14, 15, 16)]);
await Expect.That(subject).IsNotOneOf([new TimeOnly(13, 15, 16), new TimeOnly(13, 14, 15)]);
```

</TabItem>
</Tabs>

You can also specify a tolerance:

<Tabs groupId="date-time-only">
<TabItem value="DateOnly" label="DateOnly" default>

```csharp
DateOnly subject = new DateOnly(2024, 12, 24);

await Expect.That(subject).IsOneOf([new DateOnly(2024, 12, 23)]).Within(TimeSpan.FromDays(1))
  .Because("we accept values between 2024-12-22 and 2024-12-24");
```

</TabItem>
<TabItem value="TimeOnly" label="TimeOnly">

```csharp
TimeOnly subject = new TimeOnly(14, 15, 16);

await Expect.That(subject).IsOneOf([new TimeOnly(14, 15, 17)]).Within(TimeSpan.FromSeconds(1))
  .Because("we accept values between 14:15:16 and 14:15:18");
```

</TabItem>
</Tabs>

## After

You can verify that the `DateOnly` or `TimeOnly` is (on or) after another value:

<Tabs groupId="date-time-only">
<TabItem value="DateOnly" label="DateOnly" default>

```csharp
DateOnly subject = DateOnly.FromDateTime(DateTime.Now);

await Expect.That(subject).IsAfter(new DateOnly(2024, 1, 1));
await Expect.That(subject).IsOnOrAfter(new DateOnly(2024, 1, 1));
```

</TabItem>
<TabItem value="TimeOnly" label="TimeOnly">

```csharp
TimeOnly subject = TimeOnly.FromDateTime(DateTime.Now);

await Expect.That(subject).IsAfter(new TimeOnly(0, 0, 0));
await Expect.That(subject).IsOnOrAfter(new TimeOnly(0, 0, 0));
```

</TabItem>
</Tabs>

You can also specify a tolerance:

<Tabs groupId="date-time-only">
<TabItem value="DateOnly" label="DateOnly" default>

```csharp
DateOnly subject = DateOnly.FromDateTime(DateTime.Now);

await Expect.That(subject).IsAfter(DateOnly.FromDateTime(DateTime.Now)).Within(TimeSpan.FromDays(1));
```

</TabItem>
<TabItem value="TimeOnly" label="TimeOnly">

```csharp
TimeOnly subject = TimeOnly.FromDateTime(DateTime.Now);

await Expect.That(subject).IsAfter(TimeOnly.FromDateTime(DateTime.Now)).Within(TimeSpan.FromSeconds(1));
```

</TabItem>
</Tabs>

## Before

You can verify that the `DateOnly` or `TimeOnly` is (on or) before another value:

<Tabs groupId="date-time-only">
<TabItem value="DateOnly" label="DateOnly" default>

```csharp
DateOnly subject = DateOnly.FromDateTime(DateTime.Now);

await Expect.That(subject).IsBefore(new DateOnly(2124, 12, 31));
await Expect.That(subject).IsOnOrBefore(new DateOnly(2124, 12, 31));
```

</TabItem>
<TabItem value="TimeOnly" label="TimeOnly">

```csharp
TimeOnly subject = TimeOnly.FromDateTime(DateTime.Now);

await Expect.That(subject).IsBefore(new TimeOnly(23, 59, 59));
await Expect.That(subject).IsOnOrBefore(new TimeOnly(23, 59, 59));
```

</TabItem>
</Tabs>

You can also specify a tolerance:

<Tabs groupId="date-time-only">
<TabItem value="DateOnly" label="DateOnly" default>

```csharp
DateOnly subject = DateOnly.FromDateTime(DateTime.Now);

await Expect.That(subject).IsBefore(DateOnly.FromDateTime(DateTime.Now)).Within(TimeSpan.FromDays(1));
```

</TabItem>
<TabItem value="TimeOnly" label="TimeOnly">

```csharp
TimeOnly subject = TimeOnly.FromDateTime(DateTime.Now);

await Expect.That(subject).IsBefore(TimeOnly.FromDateTime(DateTime.Now)).Within(TimeSpan.FromSeconds(1));
```

</TabItem>
</Tabs>

## Between

You can verify that the `DateOnly` or `TimeOnly` is between two values:

<Tabs groupId="date-time-only">
<TabItem value="DateOnly" label="DateOnly" default>

```csharp
DateOnly subject = DateOnly.FromDateTime(DateTime.Now);

await Expect.That(subject).IsBetween(new DateOnly(2024, 1, 1)).And(new DateOnly(2123, 12, 31));
```

</TabItem>
<TabItem value="TimeOnly" label="TimeOnly">

```csharp
using aweXpect.Chronology; // from the aweXpect.Chronology package

TimeOnly subject = TimeOnly.FromDateTime(DateTime.Now);

await Expect.That(subject)
    .IsBetween(TimeOnly.FromDateTime(DateTime.Now).Add(-2.Seconds()))
    .And(TimeOnly.FromDateTime(DateTime.Now).Add(2.Seconds()));
```

</TabItem>
</Tabs>

You can also specify a tolerance:

```csharp
using aweXpect.Chronology; // from the aweXpect.Chronology package

TimeOnly subject = TimeOnly.FromDateTime(DateTime.Now);

await Expect.That(subject)
  .IsBetween(TimeOnly.FromDateTime(DateTime.Now)).And(TimeOnly.FromDateTime(DateTime.Now)).Within(2.Seconds())
  .Because("it should have taken less than two seconds");
```

## Properties

You can verify the properties of the `DateOnly` or `TimeOnly`:

<Tabs groupId="date-time-only">
<TabItem value="DateOnly" label="DateOnly" default>

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

</TabItem>
<TabItem value="TimeOnly" label="TimeOnly">

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

</TabItem>
</Tabs>

<PropertyComparisons />
