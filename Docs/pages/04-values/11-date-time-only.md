import Tabs from '@theme/Tabs';
import TabItem from '@theme/TabItem';
import PropertyComparisons from '../_property-comparisons.md';

# DateOnly / TimeOnly

Describes the possible expectations for `DateOnly` and `TimeOnly`.

| Expectation                      | Negated            | Summary                                    |
|----------------------------------|--------------------|--------------------------------------------|
| [`IsEqualTo`](#equality)         | `IsNotEqualTo`     | equal to the expected value                |
| [`IsOneOf`](#one-of)             | `IsNotOneOf`       | equal to one of the expected values        |
| [`IsAfter`](#after--before)      | `IsNotAfter`       | later than the expected value              |
| [`IsOnOrAfter`](#after--before)  | `IsNotOnOrAfter`   | not earlier than the expected value        |
| [`IsBefore`](#after--before)     | `IsNotBefore`      | earlier than the expected value            |
| [`IsOnOrBefore`](#after--before) | `IsNotOnOrBefore`  | not later than the expected value          |
| [`IsBetween`](#between)          | `IsNotBetween`     | between two values, both bounds included   |
| [`HasYear`](#properties), …      | negated comparison | has the expected year, month, day, hour, … |

:::note[.NET 8 or later]
The `DateOnly` and `TimeOnly` expectations are only available on .NET 8 or later.
:::

## Equality

You can verify that the `DateOnly` or `TimeOnly` is equal to another one or not:

<Tabs groupId="date-time-only">
<TabItem value="DateOnly" label="DateOnly" default>

```csharp
DateOnly releaseDate = new DateOnly(1969, 9, 26);

await Expect.That(releaseDate).IsEqualTo(new DateOnly(1969, 9, 26));
await Expect.That(releaseDate).IsNotEqualTo(new DateOnly(1970, 5, 8));
```

</TabItem>
<TabItem value="TimeOnly" label="TimeOnly">

```csharp
TimeOnly startTime = new TimeOnly(14, 15, 16);

await Expect.That(startTime).IsEqualTo(new TimeOnly(14, 15, 16));
await Expect.That(startTime).IsNotEqualTo(new TimeOnly(13, 15, 16));
```

</TabItem>
</Tabs>

## One of

You can verify that the `DateOnly` or `TimeOnly` is one of many alternatives:

<Tabs groupId="date-time-only">
<TabItem value="DateOnly" label="DateOnly" default>

```csharp
DateOnly releaseDate = new DateOnly(1969, 9, 26);

await Expect.That(releaseDate).IsOneOf([new DateOnly(1969, 9, 26), new DateOnly(1970, 5, 8)]);
await Expect.That(releaseDate).IsNotOneOf([new DateOnly(1965, 8, 6), new DateOnly(1966, 8, 5)]);
```

</TabItem>
<TabItem value="TimeOnly" label="TimeOnly">

```csharp
TimeOnly startTime = new TimeOnly(14, 15, 16);

await Expect.That(startTime).IsOneOf([new TimeOnly(14, 15, 15), new TimeOnly(14, 15, 16)]);
await Expect.That(startTime).IsNotOneOf([new TimeOnly(13, 15, 16), new TimeOnly(13, 14, 15)]);
```

</TabItem>
</Tabs>

## After / before

You can verify that the `DateOnly` or `TimeOnly` is (on or) after or before another value, or that it is not:

<Tabs groupId="date-time-only">
<TabItem value="DateOnly" label="DateOnly" default>

```csharp
DateOnly releaseDate = new DateOnly(1969, 9, 26);

await Expect.That(releaseDate).IsAfter(new DateOnly(1968, 11, 22));
await Expect.That(releaseDate).IsOnOrAfter(new DateOnly(1969, 9, 26));
await Expect.That(releaseDate).IsBefore(new DateOnly(1970, 5, 8));
await Expect.That(releaseDate).IsOnOrBefore(new DateOnly(1969, 9, 26));
await Expect.That(releaseDate).IsNotAfter(new DateOnly(1970, 5, 8));
```

</TabItem>
<TabItem value="TimeOnly" label="TimeOnly">

```csharp
TimeOnly startTime = new TimeOnly(14, 15, 16);

await Expect.That(startTime).IsAfter(new TimeOnly(12, 0));
await Expect.That(startTime).IsOnOrAfter(new TimeOnly(14, 15, 16));
await Expect.That(startTime).IsBefore(new TimeOnly(18, 0));
await Expect.That(startTime).IsOnOrBefore(new TimeOnly(14, 15, 16));
await Expect.That(startTime).IsNotAfter(new TimeOnly(18, 0));
```

</TabItem>
</Tabs>

## Between

You can verify that the `DateOnly` or `TimeOnly` is between two values, or that it is not:

<Tabs groupId="date-time-only">
<TabItem value="DateOnly" label="DateOnly" default>

```csharp
DateOnly releaseDate = new DateOnly(1969, 9, 26);

await Expect.That(releaseDate).IsBetween(new DateOnly(1969, 1, 1)).And(new DateOnly(1969, 12, 31));
await Expect.That(releaseDate).IsNotBetween(new DateOnly(1970, 1, 1)).And(new DateOnly(1970, 12, 31));
```

</TabItem>
<TabItem value="TimeOnly" label="TimeOnly">

```csharp
TimeOnly startTime = new TimeOnly(14, 15, 16);

await Expect.That(startTime).IsBetween(new TimeOnly(14, 0)).And(new TimeOnly(15, 0));
await Expect.That(startTime).IsNotBetween(new TimeOnly(18, 0)).And(new TimeOnly(20, 0));
```

</TabItem>
</Tabs>

Both bounds are included. For a `DateOnly`, a maximum below the minimum throws an `ArgumentOutOfRangeException` as soon
as it is specified. For a `TimeOnly`, such a range runs across midnight instead, see [clock face](#clock-face).

## Tolerance

Every comparison on this page except the `Has…` properties accepts a tolerance with `Within`, which widens the
accepted range by the tolerance:

<Tabs groupId="date-time-only">
<TabItem value="DateOnly" label="DateOnly" default>

```csharp
DateOnly releaseDate = new DateOnly(1969, 9, 26);

await Expect.That(releaseDate).IsEqualTo(new DateOnly(1969, 9, 25)).Within(TimeSpan.FromDays(1))
  .Because("we accept values between 1969-09-24 and 1969-09-26");
await Expect.That(releaseDate).IsBefore(new DateOnly(1969, 9, 26)).Within(TimeSpan.FromDays(1));
```

</TabItem>
<TabItem value="TimeOnly" label="TimeOnly">

```csharp
TimeOnly startTime = new TimeOnly(14, 15, 16);

await Expect.That(startTime).IsEqualTo(new TimeOnly(14, 15, 17)).Within(TimeSpan.FromSeconds(1))
  .Because("we accept values between 14:15:16 and 14:15:18");
await Expect.That(startTime).IsOneOf([new TimeOnly(14, 15, 17)]).Within(TimeSpan.FromSeconds(1));
```

</TabItem>
</Tabs>

A `DateOnly` has no time of day, so its tolerance must be a whole number of days. Anything else, for example
`Within(TimeSpan.FromHours(23))`, throws an `ArgumentOutOfRangeException` as soon as it is specified instead of silently
rounding down to a tolerance you did not ask for.

The [default tolerance](./10-datetime-offset.md#default-tolerance) is shared with the other time types, so it is not
rejected: only its whole days apply to a `DateOnly`, and a default below one day has no effect.

## Clock face

A `TimeOnly` has no date, so midnight is not a boundary for equality and ranges, but it stays one for ordering:

- `IsEqualTo`, `IsNotEqualTo` and `IsOneOf` use the shortest distance around the clock face, so `00:00` and `23:59` are
  one minute apart. That distance never exceeds 12 hours, so a tolerance of 12 hours or more accepts every time.
- `IsBetween` runs clockwise from the minimum to the maximum, so a range from `23:00` to `01:00` contains `00:00`.
- `IsAfter`, `IsOnOrAfter`, `IsBefore` and `IsOnOrBefore` compare the times as they are, so `00:00` is never after
  `23:00`. Their tolerance only ever widens the accepted range and never wraps around midnight.

```csharp
TimeOnly midnight = new TimeOnly(0, 0);

await Expect.That(midnight).IsBetween(new TimeOnly(23, 0)).And(new TimeOnly(1, 0));
await Expect.That(midnight).IsEqualTo(new TimeOnly(23, 59)).Within(TimeSpan.FromMinutes(1));
await Expect.That(midnight).IsNotAfter(new TimeOnly(23, 0));
```

## Properties

You can verify the properties of the `DateOnly` or `TimeOnly`:

<Tabs groupId="date-time-only">
<TabItem value="DateOnly" label="DateOnly" default>

```csharp
DateOnly releaseDate = new DateOnly(1969, 9, 26);

await Expect.That(releaseDate).HasYear(1969);
await Expect.That(releaseDate).HasMonth().GreaterThan(6);
await Expect.That(releaseDate).HasDay(26);
```

</TabItem>
<TabItem value="TimeOnly" label="TimeOnly">

```csharp
TimeOnly startTime = new TimeOnly(15, 16, 17, 189);

await Expect.That(startTime).HasHour(15);
await Expect.That(startTime).HasMinute(16);
await Expect.That(startTime).HasSecond().LessThan(30);
await Expect.That(startTime).HasMillisecond(189);
```

</TabItem>
</Tabs>

<PropertyComparisons />
