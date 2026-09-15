# DateTime / DateTimeOffset

Describes the possible expectations for `DateTime` and `DateTimeOffset`.

## Equality

You can verify that the `DateTime` or `DateTimeOffset` is equal to another one or not:

```csharp
DateTime subject = new DateTime(2024, 12, 24);

await Expect.That(subject).IsEqualTo(new DateTime(2024, 12, 24));
```

```csharp
DateTimeOffset subject = new DateTimeOffset(2024, 12, 24, 13, 15, 0, TimeSpan.FromHours(2));

await Expect.That(subject).IsEqualTo(new DateTimeOffset(2024, 12, 24, 13, 15, 0, TimeSpan.FromHours(2)));
```

You can also specify a tolerance:

```csharp
DateTime subject = new DateTime(2024, 12, 24);

await Expect.That(subject).IsEqualTo(new DateTime(2024, 12, 23)).Within(TimeSpan.FromDays(1))
  .Because("we accept values between 2024-12-23 and 2024-12-25");
```

```csharp
DateTimeOffset subject = new DateTimeOffset(2024, 12, 24, 13, 15, 0, TimeSpan.FromHours(2));

await Expect.That(subject).IsEqualTo(new DateTimeOffset(2024, 12, 24, 13, 5, 0, TimeSpan.FromHours(2))).Within(TimeSpan.FromMinutes(10))
  .Because("we accept values between 2024-12-24T12:55:00+2:00 and 2024-12-24T13:15:00+2:00");
```

## One of

You can verify that the `DateTime` or `DateTimeOffset` is one of many alternatives:

```csharp
DateTime subject = new DateTime(2024, 12, 24);

await Expect.That(subject).IsOneOf([new DateTime(2024, 12, 23), new DateTime(2024, 12, 24)]);
await Expect.That(subject).IsNotOneOf([new DateTime(2022, 12, 24), new DateTime(2023, 12, 24)]);
```

```csharp
DateTimeOffset subject = new DateTimeOffset(2024, 12, 24, 13, 15, 0, TimeSpan.FromHours(2));

await Expect.That(subject).IsOneOf([new DateTimeOffset(2024, 12, 24, 13, 5, 0, TimeSpan.FromHours(2)), new DateTimeOffset(2024, 12, 24, 13, 15, 0, TimeSpan.FromHours(2))]);
await Expect.That(subject).IsNotOneOf([new DateTimeOffset(2024, 12, 24, 13, 5, 0, TimeSpan.FromHours(2)), new DateTimeOffset(2025, 12, 24, 13, 15, 0, TimeSpan.FromHours(3))]);
```

You can also specify a tolerance:

```csharp
DateTime subject = new DateTime(2024, 12, 24);

await Expect.That(subject).IsOneOf([new DateTime(2024, 12, 23)]).Within(TimeSpan.FromDays(1))
  .Because("we accept values between 2024-12-22 and 2024-12-24");
```

```csharp
DateTimeOffset subject = new DateTimeOffset(2024, 12, 24, 13, 15, 0, TimeSpan.FromHours(2));

await Expect.That(subject).IsOneOf([new DateTimeOffset(2024, 12, 24, 13, 5, 0, TimeSpan.FromHours(2))]).Within(TimeSpan.FromMinutes(10))
  .Because("we accept values between 2024-12-24T12:55:00+2:00 and 2024-12-24T13:15:00+2:00");
```

## After

You can verify that the `DateTime` or `DateTimeOffset` is (on or) after another value:

```csharp
DateTime subject = DateTime.Now;

await Expect.That(subject).IsAfter(new DateTime(2024, 1, 1));
await Expect.That(subject).IsOnOrAfter(new DateTime(2024, 1, 1));
```

```csharp
DateTimeOffset subject = DateTimeOffset.Now;

await Expect.That(subject).IsAfter(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.FromHours(2)));
await Expect.That(subject).IsOnOrAfter(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.FromHours(2)));
```

You can also specify a tolerance:

```csharp
DateTime subject = DateTime.Now;

await Expect.That(subject).IsAfter(DateTime.Now).Within(TimeSpan.FromSeconds(1))
  .Because("it should have taken less than one second");
```

## Before

You can verify that the `DateTime` or `DateTimeOffset` is (on or) before another value:

```csharp
DateTime subject = DateTime.Now;

await Expect.That(subject).IsBefore(new DateTime(2124, 12, 31));
await Expect.That(subject).IsOnOrBefore(new DateTime(2124, 12, 31));
```

```csharp
DateTimeOffset subject = DateTimeOffset.Now;

await Expect.That(subject).IsBefore(new DateTimeOffset(2124, 12, 31, 23, 59, 59, TimeSpan.FromHours(2)));
await Expect.That(subject).IsOnOrBefore(new DateTimeOffset(2124, 12, 31, 23, 59, 59, TimeSpan.FromHours(2)));
```

You can also specify a tolerance:

```csharp
DateTime subject = DateTime.Now;

await Expect.That(subject).IsOnOrBefore(DateTime.Now).Within(TimeSpan.FromSeconds(1))
  .Because("it should have taken less than one second");
```

## Between

You can verify that the `DateTime` or `DateTimeOffset` is between two values:

```csharp
DateTime subject = DateTime.Now;

await Expect.That(subject).IsBetween(new DateTime(2024, 1, 1)).And(new DateTime(2123, 12, 31));
```

```csharp
DateTimeOffset subject = DateTimeOffset.Now;

await Expect.That(subject)
    .IsBetween(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.FromHours(2)))
    .And(new DateTimeOffset(2124, 12, 31, 23, 59, 59, TimeSpan.FromHours(2)));
```

You can also specify a tolerance:

```csharp
DateTime subject = DateTime.Now;

await Expect.That(subject).IsBetween(DateTime.Today).And(DateTime.Now).Within(TimeSpan.FromSeconds(1))
  .Because("it should have taken less than one second");
```

## Properties

You can verify, the properties of `DateTime` or `DateTimeOffset`:

```csharp
DateTime subject = new DateTime(2024, 12, 31, 15, 16, 17, 189, DateTimeKind.Utc);
// or
DateTimeOffset subject = new DateTimeOffset(2024, 12, 31, 15, 16, 17, 189, TimeSpan.FromMinutes(90));

await Expect.That(subject).HasYear().EqualTo(2024);
await Expect.That(subject).HasMonth().EqualTo(12);
await Expect.That(subject).HasDay().EqualTo(31);
await Expect.That(subject).HasHour().EqualTo(15);
await Expect.That(subject).HasMinute().EqualTo(16);
await Expect.That(subject).HasSecond().EqualTo(17);
await Expect.That(subject).HasMillisecond().EqualTo(189);
```

For `DateTime` you can also verify the `Kind` property:

```csharp
await Expect.That(subject).HasKind().EqualTo(DateTimeKind.Utc);
```

For `DateTimeOffset` you can also verify the `Offset` property:

```csharp
await Expect.That(subject).HasOffset().EqualTo(TimeSpan.FromMinutes(90));
```

All property verifications support the following comparisons:

```csharp
DateTime subject = new DateTime(2024, 12, 31, 15, 16, 17);

await Expect.That(subject).HasYear().EqualTo(2024);
await Expect.That(subject).HasYear().NotEqualTo(2020);
await Expect.That(subject).HasYear().GreaterThan(2023);
await Expect.That(subject).HasYear().GreaterThanOrEqualTo(2024);
await Expect.That(subject).HasYear().LessThanOrEqualTo(2024);
await Expect.That(subject).HasYear().LessThan(2025);
await Expect.That(subject).HasYear().Between(2000).And(2024);
```

## Default Tolerance

In Windows the `DateTime` resolution is [about 10 to 15 milliseconds](https://stackoverflow.com/q/3140826/4003370), so
comparing them as exact values might result in brittle tests.
Therefore, it is possible to specify a default tolerance that is used for all `DateTime`, `DateTimeOffset`, `DateOnly`,
`TimeOnly` and `TimeSpan` comparisons (unless an explicit tolerance is given):

```csharp
IDisposable lifetime = Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(15.Milliseconds());
```
