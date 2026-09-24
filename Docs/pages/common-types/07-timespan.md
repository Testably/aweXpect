# TimeSpan

Describes the possible expectations for `TimeSpan`.

## Equality

You can verify that the `TimeSpan` is equal to another one or not:

```csharp
TimeSpan subject = TimeSpan.FromSeconds(42);

await Expect.That(subject).IsEqualTo(TimeSpan.FromSeconds(42));
await Expect.That(subject).IsNotEqualTo(TimeSpan.FromSeconds(43));
```

You can also specify a tolerance:

```csharp
TimeSpan subject = TimeSpan.FromSeconds(42);

await Expect.That(subject).IsEqualTo(TimeSpan.FromSeconds(43)).Within(TimeSpan.FromSeconds(1))
  .Because("we accept values between 0:41 and 0:43");
```

## One of

You can verify that the `TimeSpan` is one of many alternatives:

```csharp
TimeSpan subject = TimeSpan.FromSeconds(42);

await Expect.That(subject).IsOneOf([TimeSpan.FromSeconds(40), TimeSpan.FromSeconds(42)]);
await Expect.That(subject).IsNotOneOf([TimeSpan.FromSeconds(41), TimeSpan.FromSeconds(43)]);
```

You can also specify a tolerance:

```csharp
TimeSpan subject = TimeSpan.FromSeconds(42);

await Expect.That(subject).IsOneOf([TimeSpan.FromSeconds(43), TimeSpan.FromSeconds(45)]).Within(TimeSpan.FromSeconds(1))
  .Because("we accept values between 0:41 and 0:43 or between 00:44 and 00:46");
```

## Greater than

You can verify that the `TimeSpan` is greater than (or equal to) another number:

```csharp
TimeSpan subject = TimeSpan.FromSeconds(42);

await Expect.That(subject).IsGreaterThan(TimeSpan.FromSeconds(41));
await Expect.That(subject).IsGreaterThanOrEqualTo(TimeSpan.FromSeconds(42));
```

You can also specify a tolerance:

```csharp
TimeSpan subject = TimeSpan.FromSeconds(41);

await Expect.That(subject).IsGreaterThan(TimeSpan.FromSeconds(42)).Within(TimeSpan.FromSeconds(2))
  .Because("we accept values greater than 0:40 (0:42 ± 2s)");
```

## Less than

You can verify that the `TimeSpan` is less than (or equal to) another number:

```csharp
TimeSpan subject = TimeSpan.FromSeconds(42);

await Expect.That(subject).IsLessThan(TimeSpan.FromSeconds(43));
await Expect.That(subject).IsLessThanOrEqualTo(TimeSpan.FromSeconds(42));
```

You can also specify a tolerance:

```csharp
TimeSpan subject = TimeSpan.FromSeconds(43);

await Expect.That(subject).IsLessThan(TimeSpan.FromSeconds(42)).Within(TimeSpan.FromSeconds(2))
  .Because("we accept values less than 0:44 (0:42 ± 2s)");
```

## Between

You can verify that the `TimeSpan` is between two values:

```csharp
TimeSpan subject = TimeSpan.FromSeconds(42);

await Expect.That(subject).IsBetween(TimeSpan.FromSeconds(40)).And(TimeSpan.FromSeconds(50));
```

You can also specify a tolerance:

```csharp
using aweXpect.Chronology; // from the aweXpect.Chronology package

TimeSpan subject = TimeSpan.FromSeconds(42);

await Expect.That(subject)
	.IsBetween(43.Seconds())
    .And(45.Seconds())
	.Within(1.Seconds())
  .Because("it should expand the interval by 1 second");
```

## Positive / negative

You can verify that the `TimeSpan` is positive or negative:

```csharp
await Expect.That(TimeSpan.FromSeconds(42)).IsPositive();
await Expect.That(TimeSpan.FromSeconds(-3)).IsNegative();
```
