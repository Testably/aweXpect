# Equivalency

Describes how to verify that two objects are *equivalent* — that is, structurally equal — rather than referentially or
strictly equal. Equivalency walks both objects recursively and compares them member by member.

## Overview

Equality (`IsEqualTo`) delegates to `object.Equals`, which for most reference types means *reference* equality.
Equivalency instead compares the public state of two objects field by field and property by property, recursing into
nested objects and collections. Two objects are equivalent when every included member compares as equivalent.

```csharp
class Album(string title)
{
  public string Title { get; } = title;
}
Album subject = new("Abbey Road");

await Expect.That(subject).IsEquivalentTo(new Album("Abbey Road"));
await Expect.That(subject).IsNotEquivalentTo(new Album("Revolver"));
```

## Where equivalency is available

Equivalency is exposed on three different surfaces.

### Direct on objects

`IsEquivalentTo` and `IsNotEquivalentTo` are extension methods on any object. They accept an optional callback to
configure the comparison via [`EquivalencyOptions<TExpected>`](#configuration).

```csharp
await Expect.That(album).IsEquivalentTo(expected);
await Expect.That(album).IsEquivalentTo(expected, o => o.IgnoringMember("PlayCount"));
await Expect.That(album).IsNotEquivalentTo(unexpected);
```

### On collection elements

`AreEquivalentTo` checks every selected element of an `IEnumerable<T>` (or `IAsyncEnumerable<T>`) against a single
expected value, using the same equivalency comparison.

```csharp
IEnumerable<Track> tracks = //...
Track expected = //...

await Expect.That(tracks).All().AreEquivalentTo(expected);
await Expect.That(tracks).AtLeast(2).AreEquivalentTo(expected, o => o.IgnoringMember("Title"));
```

### As a modifier on equality assertions

For expectations that accept a custom equality comparer (`IsEqualTo`, `Contains`, `StartsWith`, `EndsWith`, `HasItem`,
`All().AreEqualTo(...)`, …), append `.Equivalent()` to switch the comparison from `Equals` to structural equivalency.

```csharp
await Expect.That(album).IsEqualTo(expected).Equivalent();

IEnumerable<Track> tracks = //...
await Expect.That(tracks).Contains(expected).Equivalent();
await Expect.That(tracks).StartsWith(expected).Equivalent();
await Expect.That(tracks).All().AreEqualTo(expected).Equivalent(o => o.IgnoringCollectionOrder());
```

## Default behaviour

By default, equivalency:

- Compares **public fields** and **public properties**.
- Recurses into nested objects.
- Treats primitives, `enum`, `string`, `decimal`, `DateTime`, `DateTimeOffset`, `TimeSpan` and `Guid` as
  *value types* and compares them with `Equals`. Everything else is compared **by members**.
- Respects collection **order** when comparing `IEnumerable<T>`.
- Detects cyclic references so two graphs that reference themselves do not cause infinite recursion.
- Honours `IEqualityComparer` if either side implements it — that comparer wins over the structural walk.
- Throws an `InvalidOperationException` when a type has no members to compare, instead of succeeding without
  verifying anything. Either include the relevant members, compare the type
  [by value](#comparing-by-value-or-by-members), or exclude all members explicitly with `IncludeMembers.None`.

## Configuration

All equivalency overloads accept an `options` callback that receives an `EquivalencyOptions` (or
`EquivalencyOptions<TExpected>`) record. The fluent methods are chainable.

```csharp
await Expect.That(album).IsEquivalentTo(expected, o => o
  .IncludingFields(IncludeMembers.Public | IncludeMembers.Internal)
  .IgnoringMember("PlayCount")
  .IgnoringCollectionOrder());
```

### Ignoring members by name

```csharp
await Expect.That(album).IsEquivalentTo(expected, o => o.IgnoringMember("PlayCount"));
```

The match is case-insensitive. For nested members, the path is dot-separated (e.g. `"Artist.Name"`); for collection
elements, the index is bracketed (e.g. `"Tracks[3]"`).

### Ignoring members by predicate

There are four overloads of `Ignoring`, depending on which information you need:

```csharp
// by member path and type
await Expect.That(album).IsEquivalentTo(expected, o => o
  .Ignoring((memberPath, memberType)
    => memberPath.EndsWith("PlayCount") && memberType == typeof(int)));

// by member path only
await Expect.That(album).IsEquivalentTo(expected, o => o
  .Ignoring(memberPath => memberPath == "Artist.Name"));

// by type only
await Expect.That(album).IsEquivalentTo(expected, o => o
  .Ignoring(memberType => memberType == typeof(DateTime)));

// by member path, type and reflected MemberInfo
await Expect.That(album).IsEquivalentTo(expected, o => o
  .Ignoring((memberPath, _, memberInfo)
    => memberPath.EndsWith("PlayCount") && memberInfo is PropertyInfo));
```

### Including fields and properties

You can change which fields and properties participate in the comparison. Both methods accept an `IncludeMembers` flags
enum with the values `None`, `Public`, `Internal` and `Private`.

```csharp
await Expect.That(album).IsEquivalentTo(expected, o => o
  .IncludingFields(IncludeMembers.None)                         // exclude all fields
  .IncludingProperties(IncludeMembers.Public | IncludeMembers.Private));
```

Default for both is `IncludeMembers.Public`.

### Ignoring collection order

When comparing collections, order matters by default. To disable that:

```csharp
int[] subject  = [1, 2, 3];
int[] expected = [3, 2, 1];

await Expect.That(subject).IsEquivalentTo(expected, o => o.IgnoringCollectionOrder());
```

Pass `false` to re-enable ordered comparison if it was disabled globally.

### Per-type options with `For<T>`

You can apply options to a specific member type only. Type-specific options override the top-level options for members
of that type.

```csharp
await Expect.That(album).IsEquivalentTo(expected, o => o
  .For<Artist>(x => x.IgnoringMember("BornOn"))
  .For<List<Track>>(x => x.IgnoringCollectionOrder()));
```

Unlike the other fluent methods, `For<T>` mutates `CustomOptions` on the options it was called on rather than returning
a copy. That is fine inside a single callback, but means an `EquivalencyOptions` instance you have already configured
with `For<T>` should not be reused across separate assertions.

### Comparing by value or by members

Each type can be compared either by value (`Equals`) or by walking its members. The default is determined by the type
itself (see [Default behaviour](#default-behaviour)). To override for a specific type:

```csharp
await Expect.That(album).IsEquivalentTo(expected, o => o
  .For<TrackId>(x => x with { ComparisonType = EquivalencyComparisonType.ByValue }));
```

To change the global rule, replace the `DefaultComparisonTypeSelector`:

```csharp
await Expect.That(album).IsEquivalentTo(expected, o => o with
{
  DefaultComparisonTypeSelector = type => type == typeof(TrackId)
    ? EquivalencyComparisonType.ByValue
    : EquivalencyDefaults.DefaultComparisonType(type),
});
```

### Customizing the global defaults

You can change the default `EquivalencyOptions` that are used when no callback is provided, via the
[customization API](/docs/expectations/advanced/customization):

```csharp
using IDisposable scope = Customize.aweXpect.Equivalency().DefaultEquivalencyOptions
  .Set(new EquivalencyOptions().IgnoringCollectionOrder());

// All equivalency checks within this scope ignore collection order by default.
```

## Per-property expectations with `It.Is<T>()`

Equivalency lets you compare against an *anonymous expectation object* in which individual members assert their own
expectations via `It.Is<T>()`. Think of it as a playlist filter: each property carries its own criterion rather than a
concrete value.

```csharp
class Track
{
  public string? Title { get; set; }
  public int PlayCount { get; set; }
}

Track midnight = new()
{
  Title = "Midnight Echo",
  PlayCount = 42,
};

await Expect.That(midnight).IsEquivalentTo(new
{
  Title = It.Is<string>().That.IsNotEmpty(),
  PlayCount = It.Is<int>().That.IsGreaterThan(2),
});
```

`It.Is<T>()` (without `.That`) only asserts that the property has the given type.

*Note: because the type cannot be inferred from `null`, an `It.Is<T>().That.IsNull()` check still works, but
`It.Is<T>().That.IsNotNull()` requires the property to be non-null.*

## Failure messages

Failure messages list each differing member with its full path and the configured options used for the comparison.

For a structural mismatch:

```
Expected that album
is equivalent to Album {
    Title = "Abbey Road",
    Artist = Artist { Name = "The Beatles" }
  },
but it was not:
  Property Artist.Name differed:
       Found: "Wings"
    Expected: "The Beatles"

Equivalency options:
 - include public fields and properties
```

When the playlist-filter pattern with `It.Is<T>()` fails, the failure renders each member's expectation inline:

```
Expected that midnight
is equivalent to { Title = is string that is not empty, PlayCount = is int that is greater than 2 },
but it was not:
  Property PlayCount was 1

Equivalency options:
 - include public fields and properties
```
