# Version

Describes the possible expectations for `Version`.

A `null` subject fails every expectation on this page. Equality, reference equality and `null` checks come from
the [object expectations](/docs/expectations/common-types/object).

## Components

You can verify the individual components of the `Version`:

```csharp
Version subject = new(1, 2, 3, 4);

await Expect.That(subject).HasMajor(1);
// or more explicit
await Expect.That(subject).HasMajor().EqualTo(1);

await Expect.That(subject).HasMinor().GreaterThan(1);
await Expect.That(subject).HasBuild().LessThanOrEqualTo(3);
await Expect.That(subject).HasRevision().NotEqualTo(5);
```

Each of `HasMajor()`, `HasMinor()`, `HasBuild()` and `HasRevision()` supports `EqualTo`, `NotEqualTo`,
`GreaterThan`, `GreaterThanOrEqualTo`, `LessThan` and `LessThanOrEqualTo`.

An unspecified build or revision is `-1`, not `0`:

```csharp
Version subject = new(1, 2);

await Expect.That(subject).HasBuild(-1);
await Expect.That(subject).HasRevision(-1);
```

## Greater than

You can verify that the `Version` is greater than (or equal to) another one:

```csharp
Version subject = new(1, 5);

await Expect.That(subject).IsGreaterThan(new Version(1, 2));
await Expect.That(subject).IsGreaterThanOrEqualTo(new Version(1, 5));
await Expect.That(subject).IsNotGreaterThan(new Version(2, 0));
```

## Less than

You can verify that the `Version` is less than (or equal to) another one:

```csharp
Version subject = new(1, 5);

await Expect.That(subject).IsLessThan(new Version(2, 0));
await Expect.That(subject).IsLessThanOrEqualTo(new Version(1, 5));
await Expect.That(subject).IsNotLessThan(new Version(1, 2));
```

## Between

You can verify that the `Version` is between two values, with both bounds included:

```csharp
Version subject = new(1, 5);

await Expect.That(subject).IsBetween(new Version(1, 2)).And(new Version(2, 0));
await Expect.That(subject).IsNotBetween(new Version(2, 0)).And(new Version(3, 0));
```

All comparisons follow
[`Version.CompareTo`](https://learn.microsoft.com/en-us/dotnet/api/system.version.compareto), which treats an
unspecified component as less than an explicit zero, so `1.2` is less than `1.2.0`.
