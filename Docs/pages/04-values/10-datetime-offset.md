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
| [`HasKind`](#properties)         | negated comparison | a `DateTime` with the expected `Kind`         |
| [`HasOffset`](#properties)       | negated comparison | a `DateTimeOffset` with the expected `Offset` |

## Equality

You can verify that the `DateTime` or `DateTimeOffset` is equal to another one or not:

<Tabs groupId="datetime-offset">
<TabItem value="DateTime" label="DateTime" default>

```csharp
DateTime releaseDate = new DateTime(1969, 9, 26);

await Expect.That(releaseDate).IsEqualTo(new DateTime(1969, 9, 26));
await Expect.That(releaseDate).IsNotEqualTo(new DateTime(1970, 5, 8));
```

</TabItem>
<TabItem value="DateTimeOffset" label="DateTimeOffset">

```csharp
DateTimeOffset releaseDate = new DateTimeOffset(1969, 9, 26, 10, 0, 0, TimeSpan.FromHours(1));

await Expect.That(releaseDate).IsEqualTo(new DateTimeOffset(1969, 9, 26, 10, 0, 0, TimeSpan.FromHours(1)));
await Expect.That(releaseDate).IsNotEqualTo(new DateTimeOffset(1970, 5, 8, 10, 0, 0, TimeSpan.FromHours(1)));
```

</TabItem>
</Tabs>

## One of

You can verify that the `DateTime` or `DateTimeOffset` is one of many alternatives:

<Tabs groupId="datetime-offset">
<TabItem value="DateTime" label="DateTime" default>

```csharp
DateTime releaseDate = new DateTime(1969, 9, 26);

await Expect.That(releaseDate).IsOneOf([new DateTime(1969, 9, 26), new DateTime(1970, 5, 8)]);
await Expect.That(releaseDate).IsNotOneOf([new DateTime(1965, 8, 6), new DateTime(1966, 8, 5)]);
```

</TabItem>
<TabItem value="DateTimeOffset" label="DateTimeOffset">

```csharp
DateTimeOffset releaseDate = new DateTimeOffset(1969, 9, 26, 10, 0, 0, TimeSpan.FromHours(1));

await Expect.That(releaseDate).IsOneOf([
  new DateTimeOffset(1969, 9, 26, 10, 0, 0, TimeSpan.FromHours(1)),
  new DateTimeOffset(1970, 5, 8, 10, 0, 0, TimeSpan.FromHours(1))]);
await Expect.That(releaseDate).IsNotOneOf([
  new DateTimeOffset(1965, 8, 6, 10, 0, 0, TimeSpan.FromHours(1)),
  new DateTimeOffset(1966, 8, 5, 10, 0, 0, TimeSpan.FromHours(1))]);
```

</TabItem>
</Tabs>

## After / before

You can verify that the `DateTime` or `DateTimeOffset` is (on or) after or before another value, or that it is not:

<Tabs groupId="datetime-offset">
<TabItem value="DateTime" label="DateTime" default>

```csharp
DateTime releaseDate = new DateTime(1969, 9, 26);

await Expect.That(releaseDate).IsAfter(new DateTime(1968, 11, 22));
await Expect.That(releaseDate).IsOnOrAfter(new DateTime(1969, 9, 26));
await Expect.That(releaseDate).IsBefore(new DateTime(1970, 5, 8));
await Expect.That(releaseDate).IsOnOrBefore(new DateTime(1969, 9, 26));
await Expect.That(releaseDate).IsNotAfter(new DateTime(1970, 5, 8));
```

</TabItem>
<TabItem value="DateTimeOffset" label="DateTimeOffset">

```csharp
DateTimeOffset releaseDate = new DateTimeOffset(1969, 9, 26, 10, 0, 0, TimeSpan.FromHours(1));

await Expect.That(releaseDate).IsAfter(new DateTimeOffset(1968, 11, 22, 10, 0, 0, TimeSpan.FromHours(1)));
await Expect.That(releaseDate).IsOnOrAfter(new DateTimeOffset(1969, 9, 26, 10, 0, 0, TimeSpan.FromHours(1)));
await Expect.That(releaseDate).IsBefore(new DateTimeOffset(1970, 5, 8, 10, 0, 0, TimeSpan.FromHours(1)));
await Expect.That(releaseDate).IsOnOrBefore(new DateTimeOffset(1969, 9, 26, 10, 0, 0, TimeSpan.FromHours(1)));
await Expect.That(releaseDate).IsNotAfter(new DateTimeOffset(1970, 5, 8, 10, 0, 0, TimeSpan.FromHours(1)));
```

</TabItem>
</Tabs>

## Between

You can verify that the `DateTime` or `DateTimeOffset` is between two values, or that it is not:

<Tabs groupId="datetime-offset">
<TabItem value="DateTime" label="DateTime" default>

```csharp
DateTime releaseDate = new DateTime(1969, 9, 26);

await Expect.That(releaseDate).IsBetween(new DateTime(1969, 1, 1)).And(new DateTime(1969, 12, 31));
await Expect.That(releaseDate).IsNotBetween(new DateTime(1970, 1, 1)).And(new DateTime(1970, 12, 31));
```

</TabItem>
<TabItem value="DateTimeOffset" label="DateTimeOffset">

```csharp
DateTimeOffset releaseDate = new DateTimeOffset(1969, 9, 26, 10, 0, 0, TimeSpan.FromHours(1));

await Expect.That(releaseDate)
  .IsBetween(new DateTimeOffset(1969, 1, 1, 0, 0, 0, TimeSpan.FromHours(1)))
  .And(new DateTimeOffset(1969, 12, 31, 23, 59, 59, TimeSpan.FromHours(1)));
await Expect.That(releaseDate)
  .IsNotBetween(new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.FromHours(1)))
  .And(new DateTimeOffset(1970, 12, 31, 23, 59, 59, TimeSpan.FromHours(1)));
```

</TabItem>
</Tabs>

Both bounds are included. A maximum below the minimum throws an `ArgumentOutOfRangeException` as soon as it is
specified.

## Tolerance

Every comparison on this page except the `Has…` properties accepts a tolerance with `Within`, which widens the
accepted range by the tolerance, the same way for `DateTime` and `DateTimeOffset`:

```csharp
DateTime importedAt = DateTime.Now;

await Expect.That(importedAt).IsEqualTo(DateTime.Now).Within(TimeSpan.FromSeconds(1))
  .Because("the import should have taken less than one second");
await Expect.That(importedAt).IsOneOf([DateTime.Now]).Within(TimeSpan.FromSeconds(1));
await Expect.That(importedAt).IsAfter(DateTime.Now).Within(TimeSpan.FromSeconds(1));
await Expect.That(importedAt).IsOnOrBefore(DateTime.Now).Within(TimeSpan.FromSeconds(1));
await Expect.That(importedAt).IsBetween(DateTime.Today).And(DateTime.Now).Within(TimeSpan.FromSeconds(1));
```

### Default tolerance

On Windows the `DateTime` resolution is [about 10 to 15 milliseconds](https://stackoverflow.com/q/3140826/4003370), so
comparing them as exact values might result in brittle tests. Therefore, it is possible to specify a default tolerance
that is used when a `DateTime`, `DateTimeOffset`, `DateOnly`, `TimeOnly` or `TimeSpan` subject is compared directly
(e.g. with `IsEqualTo`, `IsOneOf`, `IsBefore` or `IsBetween`) and no explicit tolerance is given:

```csharp
using aweXpect.Chronology; // from the aweXpect.Chronology package
using aweXpect.Customization;

IDisposable lifetime = Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(15.Milliseconds());
```

It also applies to the items of a collection of these types. An explicit `Within` always replaces the default
tolerance. The applied default tolerance is part of the failure message, for example
`is equal to 2024-12-24T13:15:00.0000000 ± 0:00.015`, unless it is zero. For a `DateOnly`, see
[tolerance](./11-date-time-only.md#tolerance).

<details>
<summary>Where the default tolerance applies</summary>

It applies to the items of a collection of `DateTime`, `DateTimeOffset`, `DateOnly`, `TimeOnly` or `TimeSpan` values
(or their nullable counterparts) in every expectation that compares them with expected items, e.g. `IsEqualTo`,
`Contains`, `IsContainedIn`, `StartsWith`, `EndsWith`, `HasItem` or `All().AreEqualTo`, and in their negations. The same
holds for the values of a dictionary in `Contains(key, value)`, `ContainsValue`, `ContainsValues` and `IsEqualTo`, and in
their negations.

The default tolerance is not used for:
- property verifications like `HasOffset()`
- collection expectations that don't compare items with expected items, like `IsInAscendingOrder` or `AreUnique`
- the keys of a dictionary, which are looked up through its key comparer, e.g. in `ContainsKey`
- values and members compared by `IsEquivalentTo`
- values compared as `object`

</details>

## Kind

aweXpect refuses to compare a `DateTime` with `DateTimeKind.Utc` against one with `DateTimeKind.Local`, because they
describe different instants: every comparison fails, except `IsNotEqualTo` and `IsNotOneOf`. A value with
`DateTimeKind.Unspecified` is compatible with both kinds, and a `DateTimeOffset` is always comparable:

```csharp
DateTime releaseDate = new DateTime(1969, 9, 26, 0, 0, 0, DateTimeKind.Utc);

// fails with "but it had kind Utc, which cannot be compared with Local"
await Expect.That(releaseDate).IsBefore(new DateTime(1969, 9, 27, 0, 0, 0, DateTimeKind.Local));

await Expect.That(releaseDate).IsNotEqualTo(new DateTime(1969, 9, 26, 0, 0, 0, DateTimeKind.Local));
await Expect.That(releaseDate).IsEqualTo(new DateTime(1969, 9, 26, 0, 0, 0, DateTimeKind.Unspecified));
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
await Expect.That(releaseDates).Contains(new DateTime(1969, 9, 26, 0, 0, 0, DateTimeKind.Local));
```

</details>

## Properties

You can verify the properties of `DateTime` or `DateTimeOffset`:

```csharp
DateTime importedAt = new DateTime(2024, 12, 31, 15, 16, 17, 189, DateTimeKind.Utc);
// or: DateTimeOffset importedAt = new DateTimeOffset(2024, 12, 31, 15, 16, 17, 189, TimeSpan.FromMinutes(90));

await Expect.That(importedAt).HasYear(2024);
await Expect.That(importedAt).HasMonth(12);
await Expect.That(importedAt).HasDay(31);
await Expect.That(importedAt).HasHour().GreaterThan(12);
await Expect.That(importedAt).HasMinute(16);
await Expect.That(importedAt).HasSecond(17);
await Expect.That(importedAt).HasMillisecond().LessThan(500);
```

For `DateTime` you can also verify the `Kind` property, and for `DateTimeOffset` the `Offset` property:

<Tabs groupId="datetime-offset">
<TabItem value="DateTime" label="DateTime" default>

```csharp
DateTime importedAt = new DateTime(2024, 12, 31, 15, 16, 17, 189, DateTimeKind.Utc);

await Expect.That(importedAt).HasKind(DateTimeKind.Utc);
await Expect.That(importedAt).HasKind().NotEqualTo(DateTimeKind.Local);
```

</TabItem>
<TabItem value="DateTimeOffset" label="DateTimeOffset">

```csharp
DateTimeOffset importedAt = new DateTimeOffset(2024, 12, 31, 15, 16, 17, 189, TimeSpan.FromMinutes(90));

await Expect.That(importedAt).HasOffset(TimeSpan.FromMinutes(90));
await Expect.That(importedAt).HasOffset().GreaterThan(TimeSpan.Zero);
```

</TabItem>
</Tabs>

<PropertyComparisons example="HasYear(2024)" />

`HasKind()` only supports `EqualTo` and `NotEqualTo`.
