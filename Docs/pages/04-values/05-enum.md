import PropertyComparisons from '../_property-comparisons.md';

# Enum

Describes the possible expectations for `enum` values.

| Expectation              | Negated            | Summary                                   |
|--------------------------|--------------------|-------------------------------------------|
| [`IsEqualTo`](#equality) | `IsNotEqualTo`     | equal to the expected value               |
| [`IsOneOf`](#one-of)     | `IsNotOneOf`       | equal to one of the expected values       |
| [`HasValue`](#value)     | negated comparison | has the expected underlying numeric value |
| [`IsDefined`](#defined)  | `IsNotDefined`     | a named member of the `enum`              |
| [`HasFlag`](#flags)      | `DoesNotHaveFlag`  | has the expected flag set                 |

The samples on this page use the following `enum`:

```csharp
enum Genre { Rock = 1, Pop = 2, Jazz = 3, Blues = 4 }
```

## Equality

You can verify that the `enum` is equal to another one or not:

```csharp
await Expect.That(Genre.Rock).IsEqualTo(Genre.Rock);
await Expect.That(Genre.Rock).IsNotEqualTo(Genre.Jazz);
```

## One of

You can verify that the `enum` is one of many alternatives:

```csharp
await Expect.That(Genre.Rock).IsOneOf(Genre.Rock, Genre.Pop, Genre.Jazz);
await Expect.That(Genre.Blues).IsNotOneOf(Genre.Rock, Genre.Pop, Genre.Jazz);
```

## Value

You can verify that the `enum` has a given underlying numeric value or not:

```csharp
await Expect.That(Genre.Rock).HasValue(1);
await Expect.That(Genre.Rock).HasValue().NotEqualTo(2);
await Expect.That(Genre.Jazz).HasValue().GreaterThan(2);
```

<PropertyComparisons />

Every backing type from `sbyte` to `ulong` is covered. Each comparison takes a `long` or a `ulong`, so a member of
a `ulong`-backed `enum` above `long.MaxValue` can be named as well:

```csharp
enum Big : ulong { Max = ulong.MaxValue }

await Expect.That(Big.Max).HasValue(ulong.MaxValue);
await Expect.That(Big.Max).HasValue().GreaterThan(0);
```

## Defined

You can verify that the `enum` has a defined value or not:

```csharp
await Expect.That((Genre)3).IsDefined()
  .Because("3 corresponds to 'Jazz'");
await Expect.That((Genre)5).IsNotDefined()
  .Because("5 is no valid genre");
```

## Flags

You can verify that the `enum` has a specific flag or not:

```csharp
using System.Text.RegularExpressions;

RegexOptions options = RegexOptions.Multiline | RegexOptions.IgnoreCase;

await Expect.That(options).HasFlag(RegexOptions.IgnoreCase);
await Expect.That(options).DoesNotHaveFlag(RegexOptions.ExplicitCapture);
```

Unlike the property-style `Has…` expectations such as `HasValue`, `HasFlag` has no continuation: it asks whether a bit
is set, not how two ordered values compare, so `GreaterThan`, `Between` and the rest would have no meaning for it.
