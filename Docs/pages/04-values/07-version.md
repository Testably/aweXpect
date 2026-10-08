import PropertyComparisons from '../_property-comparisons.md';

# Version

Describes the possible expectations for `Version`.

| Expectation                                          | Negated                     | Summary                                       |
|------------------------------------------------------|-----------------------------|-----------------------------------------------|
| [`IsEqualTo`](#equality)                             | `IsNotEqualTo`              | equal to the expected version                 |
| [`IsOneOf`](#one-of)                                 | `IsNotOneOf`                | equal to one of the expected versions         |
| [`IsGreaterThan`](#greater-than--less-than)          | `IsNotGreaterThan`          | greater than the expected version             |
| [`IsGreaterThanOrEqualTo`](#greater-than--less-than) | `IsNotGreaterThanOrEqualTo` | greater than or equal to the expected version |
| [`IsLessThan`](#greater-than--less-than)             | `IsNotLessThan`             | less than the expected version                |
| [`IsLessThanOrEqualTo`](#greater-than--less-than)    | `IsNotLessThanOrEqualTo`    | less than or equal to the expected version    |
| [`IsBetween`](#between)                              | `IsNotBetween`              | between two versions, both bounds included    |
| [`HasMajor`](#components)                            | negated comparison          | has the expected major component              |
| [`HasMinor`](#components)                            | negated comparison          | has the expected minor component              |
| [`HasBuild`](#components)                            | negated comparison          | has the expected build component              |
| [`HasRevision`](#components)                         | negated comparison          | has the expected revision component           |

A `null` subject fails every expectation on this page except equality and one of, as the
[rule for `null` subjects](../03-how-it-works/04-null-subjects.md) says. Reference equality and `null` checks come
from the [object expectations](./12-object.md).

## Equality

You can verify that the `Version` is equal to another one or not:

```csharp
Version release = new(1, 2);

await Expect.That(release).IsEqualTo(new Version(1, 2));
await Expect.That(release).IsNotEqualTo(new Version(1, 2, 0));
```

An unspecified component is `-1`, not `0`, so `1.2` is not equal to `1.2.0` and, following
[`Version.CompareTo`](https://learn.microsoft.com/en-us/dotnet/api/system.version.compareto), less than it.

## One of

You can verify that the `Version` is one of many alternatives:

```csharp
Version release = new(1, 2);

await Expect.That(release).IsOneOf(new Version(1, 2), new Version(1, 3));
await Expect.That(release).IsNotOneOf(new Version(2, 0), new Version(3, 0));
```

## Greater than / less than

You can verify that the `Version` is greater than or less than (or equal to) another one, or that it is not:

```csharp
Version release = new(1, 5);

await Expect.That(release).IsGreaterThan(new Version(1, 2));
await Expect.That(release).IsGreaterThanOrEqualTo(new Version(1, 5));
await Expect.That(release).IsLessThan(new Version(2, 0));
await Expect.That(release).IsLessThanOrEqualTo(new Version(1, 5));
await Expect.That(release).IsNotGreaterThan(new Version(2, 0));
await Expect.That(release).IsNotLessThan(new Version(1, 2));
```

## Between

You can verify that the `Version` is between two values, with both bounds included:

```csharp
Version release = new(1, 5);

await Expect.That(release).IsBetween(new Version(1, 2)).And(new Version(2, 0));
await Expect.That(release).IsNotBetween(new Version(2, 0)).And(new Version(3, 0));
```

## Components

You can verify the individual components of the `Version`:

```csharp
Version release = new(1, 2, 3, 4);

await Expect.That(release).HasMajor(1);
await Expect.That(release).HasMinor().GreaterThan(1);
await Expect.That(release).HasBuild().LessThanOrEqualTo(3);
await Expect.That(release).HasRevision().NotEqualTo(5);
```

An unspecified build or revision is `-1`, not `0`:

```csharp
Version release = new(1, 2);

await Expect.That(release).HasBuild(-1);
await Expect.That(release).HasRevision(-1);
```

<PropertyComparisons />
