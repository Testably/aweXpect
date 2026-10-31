---
description: Verify `DateTime` and `DateTimeOffset` values for equality, ordering, ranges and their properties, with an optional tolerance.
---

import Tabs from '@theme/Tabs';
import TabItem from '@theme/TabItem';
import PropertyComparisons from '../_property-comparisons.md';

# DateTime / DateTimeOffset

Describes the possible expectations for `DateTime` and `DateTimeOffset`.

| Expectation                      | Negated            | Summary                                       |
|----------------------------------|--------------------|-----------------------------------------------|
| [`IsEqualTo`](#equality)         | `IsNotEqualTo`     | equal to the expected value                   |
| [`IsOneOf`](#one-of)             | `IsNotOneOf`       | equal to one of the expected values           |
| [`IsAfter`](#after--before)      | `IsNotAfter`       | later than the expected value                 |
| [`IsOnOrAfter`](#after--before)  | `IsNotOnOrAfter`   | not earlier than the expected value           |
| [`IsBefore`](#after--before)     | `IsNotBefore`      | earlier than the expected value               |
| [`IsOnOrBefore`](#after--before) | `IsNotOnOrBefore`  | not later than the expected value             |
| [`IsBetween`](#between)          | `IsNotBetween`     | between two values, both bounds included      |
| [`HasYear`](#properties), …      | negated comparison | has the expected year, month, day, hour, …    |
| [`HasDayOfWeek`](#properties)    | negated comparison | falls on the expected day of the week         |
| [`HasKind`](#properties)         | negated comparison | a `DateTime` with the expected `Kind`         |
| [`HasOffset`](#properties)       | negated comparison | a `DateTimeOffset` with the expected `Offset` |

## Equality

You can verify that the `DateTime` or `DateTimeOffset` is equal to another one or not:

<Tabs groupId="datetime-offset">
<TabItem value="DateTime" label="DateTime" default>

```csharp
DateTime releaseDate = new DateTime(1969, 9, 26);

await That(releaseDate).IsEqualTo(new DateTime(1969, 9, 26));
await That(releaseDate).IsNotEqualTo(new DateTime(1970, 5, 8));
```

</TabItem>
<TabItem value="DateTimeOffset" label="DateTimeOffset">

```csharp
using aweXpect.Chronology; // from the aweXpect.Chronology package

DateTimeOffset releaseDate = new DateTimeOffset(1969, 9, 26, 10, 0, 0, 1.Hours());

await That(releaseDate).IsEqualTo(new DateTimeOffset(1969, 9, 26, 10, 0, 0, 1.Hours()));
await That(releaseDate).IsNotEqualTo(new DateTimeOffset(1970, 5, 8, 10, 0, 0, 1.Hours()));
```

</TabItem>
</Tabs>

## One of

You can verify that the `DateTime` or `DateTimeOffset` is one of many alternatives:

<Tabs groupId="datetime-offset">
<TabItem value="DateTime" label="DateTime" default>

```csharp
DateTime releaseDate = new DateTime(1969, 9, 26);

await That(releaseDate).IsOneOf([new DateTime(1969, 9, 26), new DateTime(1970, 5, 8)]);
await That(releaseDate).IsNotOneOf([new DateTime(1965, 8, 6), new DateTime(1966, 8, 5)]);
```

</TabItem>
<TabItem value="DateTimeOffset" label="DateTimeOffset">

```csharp
using aweXpect.Chronology; // from the aweXpect.Chronology package

DateTimeOffset releaseDate = new DateTimeOffset(1969, 9, 26, 10, 0, 0, 1.Hours());

await That(releaseDate).IsOneOf([
  new DateTimeOffset(1969, 9, 26, 10, 0, 0, 1.Hours()),
  new DateTimeOffset(1970, 5, 8, 10, 0, 0, 1.Hours())]);
await That(releaseDate).IsNotOneOf([
  new DateTimeOffset(1965, 8, 6, 10, 0, 0, 1.Hours()),
  new DateTimeOffset(1966, 8, 5, 10, 0, 0, 1.Hours())]);
```

</TabItem>
</Tabs>

## After / before

You can verify that the `DateTime` or `DateTimeOffset` is (on or) after or before another value, or that it is not:

<Tabs groupId="datetime-offset">
<TabItem value="DateTime" label="DateTime" default>

```csharp
DateTime releaseDate = new DateTime(1969, 9, 26);

await That(releaseDate).IsAfter(new DateTime(1968, 11, 22));
await That(releaseDate).IsOnOrAfter(new DateTime(1969, 9, 26));
await That(releaseDate).IsBefore(new DateTime(1970, 5, 8));
await That(releaseDate).IsOnOrBefore(new DateTime(1969, 9, 26));
await That(releaseDate).IsNotAfter(new DateTime(1970, 5, 8));
```

</TabItem>
<TabItem value="DateTimeOffset" label="DateTimeOffset">

```csharp
using aweXpect.Chronology; // from the aweXpect.Chronology package

DateTimeOffset releaseDate = new DateTimeOffset(1969, 9, 26, 10, 0, 0, 1.Hours());

await That(releaseDate).IsAfter(new DateTimeOffset(1968, 11, 22, 10, 0, 0, 1.Hours()));
await That(releaseDate).IsOnOrAfter(new DateTimeOffset(1969, 9, 26, 10, 0, 0, 1.Hours()));
await That(releaseDate).IsBefore(new DateTimeOffset(1970, 5, 8, 10, 0, 0, 1.Hours()));
await That(releaseDate).IsOnOrBefore(new DateTimeOffset(1969, 9, 26, 10, 0, 0, 1.Hours()));
await That(releaseDate).IsNotAfter(new DateTimeOffset(1970, 5, 8, 10, 0, 0, 1.Hours()));
```

</TabItem>
</Tabs>

## Between

You can verify that the `DateTime` or `DateTimeOffset` is between two values, or that it is not:

<Tabs groupId="datetime-offset">
<TabItem value="DateTime" label="DateTime" default>

```csharp
DateTime releaseDate = new DateTime(1969, 9, 26);

await That(releaseDate).IsBetween(new DateTime(1969, 1, 1)).And(new DateTime(1969, 12, 31));
await That(releaseDate).IsNotBetween(new DateTime(1970, 1, 1)).And(new DateTime(1970, 12, 31));
```

</TabItem>
<TabItem value="DateTimeOffset" label="DateTimeOffset">

```csharp
using aweXpect.Chronology; // from the aweXpect.Chronology package

DateTimeOffset releaseDate = new DateTimeOffset(1969, 9, 26, 10, 0, 0, 1.Hours());

await That(releaseDate)
  .IsBetween(new DateTimeOffset(1969, 1, 1, 0, 0, 0, 1.Hours()))
  .And(new DateTimeOffset(1969, 12, 31, 23, 59, 59, 1.Hours()));
await That(releaseDate)
  .IsNotBetween(new DateTimeOffset(1970, 1, 1, 0, 0, 0, 1.Hours()))
  .And(new DateTimeOffset(1970, 12, 31, 23, 59, 59, 1.Hours()));
```

</TabItem>
</Tabs>

Both bounds are included.

## Tolerance

Every comparison on this page except the `Has…` properties accepts a tolerance with `Within`, which widens the
accepted range by the tolerance, the same way for `DateTime` and `DateTimeOffset`:

```csharp
DateTime importedAt = DateTime.Now;

await That(importedAt).IsEqualTo(DateTime.Now).Within(TimeSpan.FromSeconds(1))
  .Because("the import should have taken less than one second");
await That(importedAt).IsOneOf([DateTime.Now]).Within(TimeSpan.FromSeconds(1));
await That(importedAt).IsAfter(DateTime.Now).Within(TimeSpan.FromSeconds(1));
await That(importedAt).IsOnOrBefore(DateTime.Now).Within(TimeSpan.FromSeconds(1));
await That(importedAt).IsBetween(DateTime.Today).And(DateTime.Now).Within(TimeSpan.FromSeconds(1));
```

Without `Within`, the [default tolerance](../02-how-it-works/05-options.md#default-tolerance) applies, if one is
configured.

## Kind

aweXpect refuses to compare a `DateTime` with `DateTimeKind.Utc` against one with `DateTimeKind.Local`, because they
describe different instants: every comparison fails, except `IsNotEqualTo` and `IsNotOneOf`. A value with
`DateTimeKind.Unspecified` is compatible with both kinds, and a `DateTimeOffset` is always comparable:

```csharp
DateTime releaseDate = new DateTime(1969, 9, 26, 0, 0, 0, DateTimeKind.Utc);

// fails with "but it had kind Utc, which cannot be compared with Local"
await That(releaseDate).IsBefore(new DateTime(1969, 9, 27, 0, 0, 0, DateTimeKind.Local));

await That(releaseDate).IsNotEqualTo(new DateTime(1969, 9, 26, 0, 0, 0, DateTimeKind.Local));
await That(releaseDate).IsEqualTo(new DateTime(1969, 9, 26, 0, 0, 0, DateTimeKind.Unspecified));
```

The same rule applies wherever a `DateTime` is compared as a value, e.g. in collection expectations and in
`IsEquivalentTo`.

<details>
<summary>Details of the `Kind` rule</summary>

The negated ordering expectations `IsNotAfter`, `IsNotBefore`, `IsNotOnOrAfter`, `IsNotOnOrBefore` and `IsNotBetween`
fail for an incompatible pair as well. For `IsBetween` and `IsNotBetween` the subject must be comparable to both bounds.
For `IsOneOf` and `IsNotOneOf` an alternative with an incompatible `Kind` can never be the match, but the remaining
alternatives are still considered.

In a collection, two values that differ only in their kind never match:

```csharp
DateTime[] releaseDates = [new DateTime(1969, 9, 26, 0, 0, 0, DateTimeKind.Utc)];

// fails, because the expected value denotes a different instant
await That(releaseDates).Contains(new DateTime(1969, 9, 26, 0, 0, 0, DateTimeKind.Local));
```

</details>

## Properties

You can verify the properties of `DateTime` or `DateTimeOffset`:

```csharp
DateTime importedAt = new DateTime(2024, 12, 31, 15, 16, 17, 189, DateTimeKind.Utc);
// or: DateTimeOffset importedAt = new(2024, 12, 31, 15, 16, 17, 189, TimeSpan.FromMinutes(90));

await That(importedAt).HasYear(2024);
await That(importedAt).HasMonth(12);
await That(importedAt).HasDay(31);
await That(importedAt).HasHour().GreaterThan(12);
await That(importedAt).HasMinute(16);
await That(importedAt).HasSecond(17);
await That(importedAt).HasMillisecond().LessThan(500);
await That(importedAt).HasDayOfWeek(DayOfWeek.Tuesday);
await That(importedAt).HasDayOfWeek().NotEqualTo(DayOfWeek.Sunday);
```

For `DateTime` you can also verify the `Kind` property, and for `DateTimeOffset` the `Offset` property:

<Tabs groupId="datetime-offset">
<TabItem value="DateTime" label="DateTime" default>

```csharp
DateTime importedAt = new DateTime(2024, 12, 31, 15, 16, 17, 189, DateTimeKind.Utc);

await That(importedAt).HasKind(DateTimeKind.Utc);
await That(importedAt).HasKind().NotEqualTo(DateTimeKind.Local);
```

</TabItem>
<TabItem value="DateTimeOffset" label="DateTimeOffset">

```csharp
DateTimeOffset importedAt = new DateTimeOffset(2024, 12, 31, 15, 16, 17, 189, TimeSpan.FromMinutes(90));

await That(importedAt).HasOffset(TimeSpan.FromMinutes(90));
await That(importedAt).HasOffset().GreaterThan(TimeSpan.Zero);
```

</TabItem>
</Tabs>

<PropertyComparisons example="HasYear(2024)" />

`HasDayOfWeek` and `HasKind` only offer `EqualTo` and `NotEqualTo`.
