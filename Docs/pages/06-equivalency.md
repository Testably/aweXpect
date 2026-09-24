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
using aweXpect.Equivalency; // for the options, e.g. `IgnoringMember`

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
Track expectedTrack = //...
await Expect.That(tracks).Contains(expectedTrack).Equivalent();
await Expect.That(tracks).StartsWith(expectedTrack).Equivalent();
await Expect.That(tracks).All().AreEqualTo(expectedTrack).Equivalent(o => o.IgnoringCollectionOrder());
```

## Default behaviour

By default, equivalency:

- Compares **public fields** and **public properties**.
- Matches a member of the expected object against the member of the same name on the actual object, preferring the
  same kind and falling back to the other one, so a class with public fields can be compared against an anonymous
  object, which can only have properties. The failure names the kind of the *expected* member, and the fallback only
  reaches a kind that is included, so `IncludingFields(IncludeMembers.None)` also stops an expected property from
  matching a field.
- Fails when a member of the expected object does not exist on the actual object, reporting it as missing instead of
  comparing it against `null`.
- Recurses into nested objects.
- Treats primitives, `enum`, `string`, `decimal`, `DateTime`, `DateTimeOffset`, `TimeSpan` and `Guid` as
  *value types* and compares them with `Equals`. The same applies to handles that describe something else instead of
  carrying state of their own: `MemberInfo` (and therefore `Type`), `Assembly`, `Module`, `Delegate`, `Uri` and
  `CultureInfo`, including anything derived from them. Everything else is compared **by members**.
- Compares by value as soon as either side is compared by value, so a string is never equivalent to anything but an
  equal string, however many of its members another object shares, and swapping the subject and the expectation does
  not change the result.
- Ignores a type's own `Equals` while comparing it by members, so two objects are equivalent exactly when their
  members are — an `Equals` that reports everything as equal cannot hide differing members, and one that reports
  nothing as equal cannot reject matching ones. To let `Equals` decide instead, compare the type
  [by value](#comparing-by-value-or-by-members).
- Respects collection **order** when comparing `IEnumerable<T>`, except for a set (`ISet<T>` or `IReadOnlySet<T>`),
  which has none: its elements are matched without an order, exactly as
  [ignoring collection order](#ignoring-collection-order) does. One side being a set is enough, so a `HashSet<T>` can
  be compared against an array.
- Compares a dictionary (`IDictionary`, `IDictionary<TKey, TValue>` or `IReadOnlyDictionary<TKey, TValue>`) **by key**
  instead of by position, and reports a differing, missing or superfluous entry under its key.
- Detects cyclic references so two graphs that reference themselves do not cause infinite recursion. An instance
  that is referenced more than once is still compared against each of its expected counterparts.
- Stops at a recursion depth of 100 nested objects and fails the comparison, instead of overflowing the stack (see
  [Limiting the recursion depth](#limiting-the-recursion-depth)).
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

The name must cover whole segments at the end of the member path, so `"Name"` ignores every member called `Name` at any
depth, while `"ame"` or `"t.Name"` ignore nothing.

### Ignoring members by predicate

There are three overloads of `Ignoring`, depending on which information you need:

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
```

Use `IgnoringFields` or `IgnoringProperties` instead of `Ignoring` to restrict a predicate to one kind of member.
They take the same member path and type, and are never applied to collection elements, which are neither a field nor a
property:

```csharp
await Expect.That(album).IsEquivalentTo(expected, o => o
  .IgnoringProperties((memberPath, _) => memberPath.EndsWith("PlayCount")));
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

The elements do not have to be comparable: each expected element is matched against an element that is equivalent to
it, and every element can be matched only once, so `[1, 1, 2]` is not equivalent to `[1, 2, 2]`. When no such matching
covers both collections, only the elements that were left over are reported, each against the leftover element it
differs from the least.

### Per-type options with `For<T>`

You can apply options to a specific member type only. Type-specific options override the top-level options for members
of that type. They also apply to a member whose runtime type derives from `T`, because an instance of an abstract type
is always an instance of a derived type, and the runtime type of a `Type` member is the internal `RuntimeType` rather
than `Type` itself. When several registrations match, the most derived one wins.

```csharp
await Expect.That(album).IsEquivalentTo(expected, o => o
  .For<Artist>(x => x.IgnoringMember("BornOn"))
  .For<List<Track>>(x => x.IgnoringCollectionOrder()));
```

Unlike the other fluent methods, `For<T>` mutates `CustomOptions` on the options it was called on rather than returning
a copy. That is fine inside a single callback, but means an `EquivalencyOptions` instance you have already configured
with `For<T>` should not be reused across separate assertions. The options of a single expectation start from a copy of
the customized default, so a registration in the callback replaces one for the same type in that default without
changing the default itself.

### Comparing by value or by members

Each type can be compared either by value (`Equals`) or by walking its members. The default is determined by the type
itself (see [Default behaviour](#default-behaviour)). By value, `Equals` decides alone and in both directions; by
members, `Equals` is ignored and only the members count. Comparing by value is therefore how you ask for the equality
a type defines for itself — a value object that compares only its `Id`, for example. One side being compared by value
is enough; when it is only the expectation, the expectation's `Equals` decides, since the subject is compared by
members, which ignores its `Equals`. To override for a specific type:

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

### Limiting the recursion depth

Equivalency walks nested objects recursively, so a graph that is deep enough would overflow the stack and take the
whole test process with it. The comparison therefore stops after 100 nested objects on a single path and reports the
member path at which the limit was hit — shown here with the limit lowered to 3:

```
Expected that subject
is equivalent to Node { … },
but it was not:
  Property Next.Next.Next exceeded the maximum recursion depth of 3

Equivalency options:
 - include public fields and properties
 - limit the recursion depth to 3
```

The depth is counted per path, so two members on the same level are both at the same depth, and members that are
compared [by value](#comparing-by-value-or-by-members) do not add to it. A cyclic reference is caught by the cycle
detection and never reaches the limit.

Use `LimitingRecursionDepth` to raise or lower the limit:

```csharp
await Expect.That(album).IsEquivalentTo(expected, o => o.LimitingRecursionDepth(500));
```

It is equivalent to setting `MaxRecursionDepth` directly. A depth below one, which could not even compare the root,
throws an `ArgumentOutOfRangeException`.

A limit other than the default is listed in the failure message under `Equivalency options:`.

### Customizing the global defaults

You can change the default `EquivalencyOptions` that are used when no callback is provided, via the
[customization API](/docs/expectations/advanced/customization):

```csharp
using aweXpect.Customization;

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
    Artist = Artist {
      Name = "The Beatles"
    },
    Title = "Abbey Road"
  },
but it was not:
  Property Artist.Name differed:
       Found: "Wings"
    Expected: "The Beatles"

Equivalency options:
 - include public fields and properties
```

When the playlist-filter pattern with `It.Is<T>()` fails, the member's expectation is rendered as `Expected`:

```
Expected that midnight
is equivalent to {
    PlayCount = is int that is greater than 2,
    Title = is string that is not empty
  },
but it was not:
  Property PlayCount differed:
       Found: 1
    Expected: is int that is greater than 2

Equivalency options:
 - include public fields and properties
```

## Trimming and Native AOT

Equivalency has to know the members of the compared types. Reflection provides them under the JIT, but publishing
with trimming or Native AOT enabled removes members that are only reached reflectively, so a comparison would
silently verify less than it claims to.

A source generator that ships with the `aweXpect` package closes this gap: for every call site that passes a value to
`IsEquivalentTo`, `IsNotEquivalentTo`, `AreEquivalentTo` or switches to `Equivalent()`, it registers the public
fields and properties of the argument's type, of the subject's type and of every type reachable through their
members. The registration runs when your assembly is loaded and needs no configuration. A type that has a
registration is compared through it, every other type is reflected over under the JIT and fails with an error
naming the fix where reflection is switched off, as described below. A type without any comparable member fails
loudly instead of passing without verifying anything. The registration also feeds the failure message: a registered
object is rendered from its registered members, so the message keeps listing them after trimming, while an
unregistered object is rendered as `{ *unregistered* }` where reflection is switched off. Either way the message lists
public instance members only, without static members or indexers; since a registration holds readable members only,
a registered object also leaves out write-only properties.

Some types cannot be seen by the generator, because it works from the types declared in your source:

- a member declared as `object`, an interface or a base type only reveals the declared type; the instance it holds at
  runtime is compared through reflection,
- a `private`, `protected` or `file`-local type cannot be referenced by generated code and is compared through
  reflection,
- a value that reaches the comparison through your own extension method is only registered if the extension's
  parameter or type parameter carries `[RequiresMemberMetadata]`,
- an anonymous type with a member holding a collection of anonymous types, other than an array, cannot be written
  as an instance and is compared through reflection; the element type itself is registered.

A type the generator merely did not see, such as the runtime type behind an `object` member or a value passed through
an unmarked extension, can be named explicitly to register it anyway:

```csharp
using aweXpect.Core.Metadata;

[assembly: GenerateMetadata(typeof(Track))]
```

A type the generated code cannot reference stays on reflection regardless, and the generator warns with `aweXpect2001`
when a named type yields no registration.

The registration needs `ModuleInitializerAttribute` and C# 9, so nothing is generated for a project that targets
.NET Framework or .NET Standard 2.0 unless it polyfills the attribute. Those targets cannot be trimmed or published
with Native AOT and keep using reflection.

The walk follows every member type the comparison would visit, including framework types. A member of type
`Exception`, for example, registers the types reachable from its properties, because reflection would compare them
too. Members whose getter is marked with `RequiresUnreferencedCode` or `RequiresDynamicCode` cannot be registered, so
their type stays on the reflection path.

Reflection over a type without a registration is switched off when you publish with trimming or Native AOT
enabled, because the trimmer removes members that only reflection reaches, and a comparison would silently verify
less than it claims to. Such a comparison fails with an error that names the type and asks you to register it. The
same applies to a comparison that requests `IncludeMembers.Internal` or `IncludeMembers.Private`, because only public
members are registered. The `aweXpect.ReflectionFallback.IsSupported` runtime switch forces the fallback either way,
and the `AweXpectReflectionFallback` property of your project sets that switch:

```xml
<PropertyGroup>
  <AweXpectReflectionFallback>true</AweXpectReflectionFallback>
</PropertyGroup>
```

With the fallback forced on, a trimmed application reflects over whatever the trimmer left, which is best effort: a
type whose members were all removed still fails with an error that asks you to root it, but a type that lost only
some of them is compared through the rest.
