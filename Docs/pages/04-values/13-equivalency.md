# Equivalency

Describes how to verify that two objects are *equivalent* (that is, structurally equal) rather than referentially or
strictly equal. Equivalency walks both objects recursively and compares them member by member.

| Expectation                                  | Negated                       | Summary                                                 |
|----------------------------------------------|-------------------------------|---------------------------------------------------------|
| [`IsEquivalentTo`](#on-objects)              | `IsNotEquivalentTo`           | structurally equal to the expected object               |
| [`AreEquivalentTo`](#on-collection-items)    | a quantifier such as `None()` | the selected items are equivalent to the expected value |
| [`Equivalent()`](#as-a-modifier-of-equality) |                               | switches an equality expectation to equivalency         |

## Overview

Equality (`IsEqualTo`) delegates to `object.Equals`, which for most reference types means *reference* equality.
Equivalency instead compares the public state of two objects field by field and property by property, recursing into
nested objects and collections. Two objects are equivalent when every included member compares as equivalent.

When you publish with trimming or Native AOT, see
[Native AOT and trimming](../03-how-it-works/08-native-aot.md#equivalency).

### On objects

`IsEquivalentTo` and `IsNotEquivalentTo` are extension methods on any object. They accept an optional callback to
configure the comparison via [`EquivalencyOptions<TExpected>`](#configuration):

```csharp
using aweXpect.Equivalency; // for the options, e.g. `IgnoringMember`

await Expect.That(album).IsEquivalentTo(expected);
await Expect.That(album).IsEquivalentTo(expected, o => o.IgnoringMember("PlayCount"));
await Expect.That(album).IsNotEquivalentTo(unexpected);
```

### On collection items

`AreEquivalentTo` checks every selected item of an `IEnumerable<T>` (or `IAsyncEnumerable<T>`) against a single
expected value, using the same equivalency comparison:

```csharp
IEnumerable<Track> tracks = //...
Track expected = //...

await Expect.That(tracks).All().AreEquivalentTo(expected);
await Expect.That(tracks).AtLeast(2).AreEquivalentTo(expected, o => o.IgnoringMember("Title"));
```

The options are passed to `AreEquivalentTo` itself: appending `.Equivalent(...)` or `.Using(comparer)` throws an
`InvalidOperationException`. To compare the items with a custom comparer, use `AreEqualTo(expected).Using(comparer)`.

### As a modifier of equality

For expectations that accept a custom equality comparer (`IsEqualTo`, `Contains`, `StartsWith`, `EndsWith`, `HasItem`,
`All().AreEqualTo(...)`, …), append `.Equivalent()` to switch the comparison from `Equals` to structural equivalency:

```csharp
await Expect.That(album).IsEqualTo(expected).Equivalent();

IEnumerable<Track> tracks = //...
Track expectedTrack = //...
await Expect.That(tracks).Contains(expectedTrack).Equivalent();
await Expect.That(tracks).StartsWith(expectedTrack).Equivalent();
await Expect.That(tracks).All().AreEqualTo(expectedTrack).Equivalent(o => o.IgnoringCollectionOrder());
```

Like `IsEquivalentTo`, a failure lists the equivalency options that were used.

## Default behaviour

Equivalency takes the **public fields and properties of the expected object** and compares each one with the member
of the same name on the actual object, recursing into nested objects. How a value is compared depends on its type:

| Type                                                                                                                                                                              | Compared                                    |
|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|---------------------------------------------|
| primitives, `enum`, `string`, `decimal`, `DateTime`, `DateTimeOffset`, `DateOnly`, `TimeOnly`, `TimeSpan`, `Guid`, `BigInteger`, `Complex`, `Half`, `NFloat`, `Int128`, `UInt128` | by value, with `Equals`                     |
| `MemberInfo` (and therefore `Type`), `Assembly`, `Module`, `Delegate`, `Uri`, `CultureInfo`, `IPAddress`, `Encoding` and anything derived from them                               | by value, with `Equals`                     |
| `Task`, `Task<T>` and anything derived from them                                                                                                                                  | by reference, without waiting for a result  |
| `ValueTask`, `ValueTask<T>`                                                                                                                                                       | by value, with `Equals`                     |
| `StringBuilder`                                                                                                                                                                   | by its text, so it also matches a `string`  |
| `JsonElement`, `JsonNode`                                                                                                                                                         | by its compact JSON text                    |
| `Regex`                                                                                                                                                                           | by its pattern and options                  |
| collections (`IEnumerable<T>`), `Memory<T>`, `ReadOnlyMemory<T>`                                                                                                                  | item by item, in order                      |
| sets (`ISet<T>`, `IReadOnlySet<T>`)                                                                                                                                               | item by item, without an order              |
| dictionaries (`IDictionary`, `IDictionary<TKey, TValue>`, `IReadOnlyDictionary<TKey, TValue>`)                                                                                    | entry by entry, by key                      |
| everything else                                                                                                                                                                   | by its members, recursively                 |

### Members

- Only the members of the *expected* object are compared; additional members of the actual object are ignored.
- A member that the actual object doesn't have is reported as missing instead of being compared against `null`.
- A field and a property of the same name match each other, so a class with public fields can be compared against an
  anonymous object, which only has properties. The same kind is preferred, and only included kinds are considered:
  with `IncludingFields(IncludeMembers.None)` an expected property no longer matches a field.

<details>
<summary>Explicitly implemented interface properties</summary>

An expected member that the actual object doesn't have is also matched against a property that the actual type
implements explicitly for an interface (`int IHasId.Id => 1;`), by its short name. A public field or property of that
name always takes precedence, and a name that the actual type implements explicitly for more than one interface is
reported as ambiguous. Failures name the kind of the *expected* member.

</details>

### Values and objects

- As soon as either side is compared by value, both are: a string is only equivalent to an equal string, and swapping
  subject and expectation doesn't change the result.
- A type that is compared by members ignores its own `Equals`, also when it implements `IEqualityComparer`, so an
  `Equals` can neither hide differing members nor reject matching ones. To let `Equals` decide, compare the type
  [by value](#comparing-by-value-or-by-members); to check a member against your own criterion, use
  [`It.Is<T>()`](#per-property-expectations-with-itist).
- A `JsonElement` or `JsonNode` is compared by its JSON text without the whitespace between tokens, so the order of
  the properties and the notation of numbers still matter: `{"a":1.0}` differs from `{"a":1}`.

### Collections and dictionaries

- A set on either side is enough to match the items without an order, exactly like
  [ignoring collection order](#ignoring-collection-order), so a `HashSet<T>` can be compared against an array.
- A multi-dimensional array is only equivalent to an array of the same rank with the same length in every dimension,
  as for [`IsEqualTo`](../05-collections/01-equality.md#equality).
- An actual set that was created with a custom comparer, e.g. a `HashSet<T>` or a `SortedSet<T>`, uses that comparer
  to decide which items are the same.
- A dictionary reports a differing, missing or superfluous entry under its key. Each expected key is looked up through
  the actual dictionary, so its key comparer decides which keys are the same, as it does for
  [`IsEqualTo`](../05-collections/04-dictionaries.md#equality).
- A collection or dictionary type that declares members itself is compared by its items or entries and, in addition,
  by these members, so two pages with the same items but a different `TotalCount` are not equivalent. They are
  compared like the members of any other object: they come from the *expected* type, so against an array, which
  declares none, only the items count, and the [options](#configuration) apply to them.

```csharp
// class Playlist : List<Track> { public string Name { get; set; } }
await Expect.That(playlist).IsEquivalentTo(expectedPlaylist);

// compares the tracks only
await Expect.That(playlist).IsEquivalentTo(expectedPlaylist, o => o.IgnoringMember("Name"));
```

<details>
<summary>Which members of a collection type are compared</summary>

The members that describe the collection are left out, because the items already cover them:

- members that a type of the framework declares, which is a type in the `System` or `Microsoft` namespaces, e.g. the
  `Count` and `Capacity` of a `List<T>` or the `Comparer`, `Keys` and `Values` of a `Dictionary<TKey, TValue>`, also
  when your type overrides them,
- properties that implement a property of an interface of the framework, e.g. the `Count` of an
  `IReadOnlyCollection<T>` or the `Keys` of an `IReadOnlyDictionary<TKey, TValue>` that your type implements itself,
- what the compiler generates, e.g. the state of an iterator method.

Every other public field or property is compared, whether your type or one of its base types declares it, including
one that hides a member of the framework with `new`. This also holds for the collection types of other libraries: to
leave a member out, [ignore it](#ignoring-members-by-name), where a [`For<T>`](#per-type-options-with-fort)
registration limits the rule to the collection type, or compare the type
[by value](#comparing-by-value-or-by-members) to let its `Equals` decide.

</details>

<details>
<summary>Sets and dictionaries with a custom comparer</summary>

An expected item that the comparer of the actual set finds in it is matched, whatever its members are, and only the
remaining items are matched by the equivalency comparison. This also holds for an `ImmutableHashSet<T>`, an
`ImmutableSortedSet<T>` and a `FrozenSet<T>` [on .NET 8 or later](../02-getting-started.md#target-frameworks). A
sorted set considers two items the same when its comparer orders neither before the other. The comparer of an expected
set is not used.

The key comparer is read from the same dictionaries as for
[`IsEqualTo`](../05-collections/04-dictionaries.md#equality). Two expected keys that it considers the same can't both
be matched by one entry, so the second one is reported as lacking a distinct key.

In a project that enables trimming or Native AOT, the comparer of a set or a dictionary is only read for the types that
the source generator sees.

</details>

### Safeguards

- Cyclic references are detected, so graphs that reference themselves don't recurse forever. An instance that is
  referenced more than once is still compared against each of its expected counterparts.
- The comparison fails at a recursion depth of 100 nested objects instead of overflowing the stack, see
  [Limiting the recursion depth](#limiting-the-recursion-depth).
- A type without any members to compare throws an `InvalidOperationException` instead of succeeding without verifying
  anything. Include the relevant members, compare the type [by value](#comparing-by-value-or-by-members), or exclude
  all members explicitly with `IncludeMembers.None`.

<details>
<summary>The same instance on both sides</summary>

The same instance on both sides is equivalent to itself, unless its type is compared by value: its members, entries
or items are not read, so a shared singleton without members (e.g. `EventArgs.Empty`) does not throw.

</details>

## Configuration

All equivalency overloads accept an `options` callback that receives an `EquivalencyOptions` (or
`EquivalencyOptions<TExpected>`) record. The fluent methods are chainable:

```csharp
await Expect.That(album).IsEquivalentTo(expected, o => o
  .IncludingFields(IncludeMembers.Public | IncludeMembers.Internal)
  .IgnoringMember("PlayCount")
  .IgnoringCollectionOrder());
```

| Option                                                                               | Effect                                           |
|--------------------------------------------------------------------------------------|--------------------------------------------------|
| [`IgnoringMember`](#ignoring-members-by-name)                                        | ignores members by name                          |
| [`Ignoring`, `IgnoringFields`, `IgnoringProperties`](#ignoring-members-by-predicate) | ignores members by path and type                 |
| [`IncludingFields`, `IncludingProperties`](#including-fields-and-properties)         | chooses which fields and properties are compared |
| [`IgnoringCollectionOrder`](#ignoring-collection-order)                              | matches collection items without an order        |
| [`For<T>`](#per-type-options-with-fort)                                              | applies options to members of type `T` only      |
| [`ComparisonType`](#comparing-by-value-or-by-members)                                | compares a type by value or by its members       |
| [`LimitingRecursionDepth`](#limiting-the-recursion-depth)                            | changes the maximum recursion depth of 100       |

### Ignoring members by name

```csharp
await Expect.That(album).IsEquivalentTo(expected, o => o.IgnoringMember("PlayCount"));
```

The match is case-insensitive. For nested members, the path is dot-separated (e.g. `"Artist.Name"`); for collection
elements, the index is bracketed (e.g. `"Tracks[3]"`), and for dictionary entries the key, formatted with the invariant
culture (e.g. `"Prices[1.5]"`).

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

A field or property is matched by its declared type, with `Nullable<T>` unwrapped, so the last example also ignores a
`DateTime?` member, but not an `object` member that holds a `DateTime`. A collection item or a dictionary value is
matched by the runtime type of its value, or by `object` when it is `null`.

Use `IgnoringFields` or `IgnoringProperties` instead of `Ignoring` to restrict a predicate to one kind of member.
They take the same member path and type, and are never applied to collection elements, which are neither a field nor a
property:

```csharp
await Expect.That(album).IsEquivalentTo(expected, o => o
  .IgnoringProperties((memberPath, _) => memberPath.EndsWith("PlayCount")));
```

### Including fields and properties

You can change which fields and properties participate in the comparison. Both methods accept an `IncludeMembers` flags
enum with the values `None`, `Public` and `Internal`:

```csharp
await Expect.That(album).IsEquivalentTo(expected, o => o
  .IncludingFields(IncludeMembers.None)                         // exclude all fields
  .IncludingProperties(IncludeMembers.Public | IncludeMembers.Internal));
```

Default for both is `IncludeMembers.Public`. `IncludeMembers.Internal` also includes `protected internal` members,
because the whole assembly can access them. Other protected and private members are never compared, because they are
implementation details of a type. To compare such a type, let its `Equals` decide by comparing it
[by value](#comparing-by-value-or-by-members).

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
of that type or of a derived type. When several registrations match, the most derived one wins:

```csharp
await Expect.That(album).IsEquivalentTo(expected, o => o
  .For<Artist>(x => x.IgnoringMember("BornOn"))
  .For<List<Track>>(x => x.IgnoringCollectionOrder()));
```

`For<T>` throws an `ArgumentException` for an interface; register the implementing class or struct instead.

<details>
<summary>How registrations are applied</summary>

Like the other fluent methods, `For<T>` returns a copy and leaves the options it was called on unchanged. The callback
is applied to the final options of the expectation, so every other option applies to `T` as well, no matter whether it
is set before or after `For<T>`. A registration in the callback of a single expectation replaces one for the same type
in the [customized default](#customizing-the-global-defaults).

The options are looked up by the runtime type of a value, which is never an interface. A registration for a nullable
value type `T?` applies to `T`, because a boxed value cannot tell the two apart, so it also applies to members of type
`T`.

When the subject and the expectation have different types and both are registered, the registration for the type of
the expectation wins, because the members that are compared come from the expectation. An extension can read the
options that apply to a type with `GetOptionsFor(type)`.

</details>

### Comparing by value or by members

Each type is compared either by value (`Equals` decides alone) or by its members (`Equals` is ignored). The default
depends on the type (see [Default behaviour](#default-behaviour)). One side compared by value is enough. Comparing by
value is how you ask for the equality a type defines for itself, e.g. a value object that compares only its `Id`. To
override for a specific type:

```csharp
await Expect.That(album).IsEquivalentTo(expected, o => o
  .For<TrackId>(x => x with { ComparisonType = EquivalencyComparisonType.ByValue }));
```

<details>
<summary>Which comparison type a member uses</summary>

Unlike the other type-specific options, the comparison type applies to the type itself and not to its members: a member
without a registration of its own falls back to the comparison type of the top-level options or, if none is set, to the
`DefaultComparisonTypeSelector`, so a string member of a type compared by members is still compared by value. When only
the expectation is compared by value, the expectation's `Equals` decides.

</details>

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
member path at which the limit was hit (shown here with the limit lowered to 3):

```text title="Failure message"
Expected that subject
is equivalent to expected,
but it was not:
  Property Next.Next.Next exceeded the maximum recursion depth of 3

Equivalency options:
 - include public fields and properties
 - limit the recursion depth to 3
```

Use `LimitingRecursionDepth` to raise or lower the limit:

```csharp
await Expect.That(album).IsEquivalentTo(expected, o => o.LimitingRecursionDepth(500));
```

It is equivalent to setting `MaxRecursionDepth` directly.

<details>
<summary>How the depth is counted</summary>

The depth is counted per path, so two members on the same level are both at the same depth, and members that are
compared [by value](#comparing-by-value-or-by-members) do not add to it. A cyclic reference is caught by the cycle
detection and never reaches the limit. A depth below one, which could not even compare the root, throws an
`ArgumentOutOfRangeException`.

</details>

A limit other than the default is listed in the failure message under `Equivalency options:`.

### Customizing the global defaults

You can change the default `EquivalencyOptions` via the [configuration](../03-how-it-works/07-configuration.md).
Every equivalency expectation starts from them: they are used as they are when no callback is provided, and a callback
receives them as its starting point:

```csharp
using aweXpect.Customization;

using IDisposable scope = Customize.aweXpect.Equivalency().DefaultEquivalencyOptions
  .Set(new EquivalencyOptions().IgnoringCollectionOrder());

// All equivalency checks within this scope ignore collection order by default.
```

To change the default for all async flows, e.g. in an assembly-level setup, set it on
[`Customize.aweXpect.Global`](../03-how-it-works/07-configuration.md#global-defaults):

```csharp
using aweXpect.Customization;

Customize.aweXpect.Global.Equivalency().DefaultEquivalencyOptions
  .Set(new EquivalencyOptions().IgnoringCollectionOrder());
```

## Per-property expectations with `It.Is<T>()`

Equivalency lets you compare against an *anonymous expectation object* in which individual members assert their own
expectations via `It.Is<T>()`. Think of it as a playlist filter: each property carries its own criterion rather than a
concrete value:

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

:::note
The type of `null` cannot be determined, so for a `null` property the expectations are verified against `null` when
`T` is a reference type or a nullable value type: `It.Is<string>().That.IsNull()` succeeds, while e.g.
`It.Is<string>().That.IsEmpty()` fails. For a non-nullable value type `T`, such as `int`, a `null` property always
fails.
:::

## Failure messages

Failure messages list each differing member with its full path and the configured options used for the comparison.

For a structural mismatch:

```text title="Failure message"
Expected that album
is equivalent to expected,
but it was not:
  Property Artist.Name differed:
      Actual: "Wings"
    Expected: "The Beatles"

Equivalency options:
 - include public fields and properties
```

When the playlist-filter pattern with `It.Is<T>()` fails, e.g. for a track with `PlayCount = 1`, the member's
expectation is rendered as `Expected`:

```text title="Failure message"
Expected that midnight
is equivalent to new
{
  Title = It.Is<string>().That.IsNotEmpty(),
  PlayCount = It.Is<int>().That.IsGreaterThan(2),
},
but it was not:
  Property PlayCount differed:
      Actual: 1
    Expected: is int that is greater than 2

Equivalency options:
 - include public fields and properties
```
