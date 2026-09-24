# Number

Describes the possible expectations for numbers.

## Equality

You can verify that the number is equal to another one or not:

```csharp
int subject = 42;

await Expect.That(subject).IsEqualTo(42);
```

You can also specify a tolerance:

```csharp
double subject = 42.1;

await Expect.That(subject).IsEqualTo(42).Within(0.2)
  .Because("we accept values between 41.8 and 42.2 (42 ± 0.2)");
```

## One of

You can verify that the number is one of many alternatives:

```csharp
int subject = 42;

await Expect.That(subject).IsOneOf(40, 42, 44);
```

You can also specify a tolerance:

```csharp
double subject = 42.1;

await Expect.That(subject).IsOneOf(40, 42, 44).Within(0.2)
  .Because("we accept values between 39.8 and 40.2 or 41.8 and 42.2 or 43.8 and 44.2");
```

## Greater than

You can verify that the number is greater than (or equal to) another number, or that it is not:

```csharp
int subject = 42;

await Expect.That(subject).IsGreaterThan(41);
await Expect.That(subject).IsGreaterThanOrEqualTo(42);
await Expect.That(subject).IsNotGreaterThan(42);
await Expect.That(subject).IsNotGreaterThanOrEqualTo(43);
```

`NaN` is neither greater nor less than any number, so it satisfies `IsNotGreaterThan(5)` although it fails
`IsLessThanOrEqualTo(5)`.

You can also specify a tolerance:

```csharp
double subject = 41.9;

await Expect.That(subject).IsGreaterThan(42).Within(0.2)
  .Because("we accept values greater than 41.8 (42 ± 0.2)");
```

## Less than

You can verify that the number is less than (or equal to) another number, or that it is not:

```csharp
int subject = 42;

await Expect.That(subject).IsLessThanOrEqualTo(42);
await Expect.That(subject).IsLessThan(43);
await Expect.That(subject).IsNotLessThan(42);
await Expect.That(subject).IsNotLessThanOrEqualTo(41);
```

You can also specify a tolerance:

```csharp
double subject = 42.1;

await Expect.That(subject).IsLessThan(42).Within(0.2)
  .Because("we accept values less than 42.2 (42 ± 0.2)");
```

## Between

You can verify that the number is between two numbers:

```csharp
int subject = 42;

await Expect.That(subject).IsBetween(41).And(43);
```

## Positive / negative

You can verify that the number is positive or negative, or that it is not:

```csharp
await Expect.That(42).IsPositive();
await Expect.That(-3).IsNegative();
await Expect.That(0).IsNotPositive();
await Expect.That(0).IsNotNegative();
```

Zero and `NaN` are neither positive nor negative, so both `IsNotPositive` and `IsNotNegative` succeed for them.

*Note: below .NET 8 these expectations are only available for signed numbers; on .NET 8 or later they are available for every `INumber<T>`, including unsigned types.*

## NaN

For floating point numbers you can verify that the number is `NaN` or not:

```csharp
await Expect.That(float.NaN).IsNaN();
await Expect.That(42.0).IsNotNaN();
```

## Infinity

For floating point numbers you can verify that the number is finite or infinite:

```csharp
await Expect.That(float.PositiveInfinity).IsInfinite();
await Expect.That(42.0).IsFinite();
```
