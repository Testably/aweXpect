---
description: Verify the number of items, how many items meet an expectation, a single item or the item at an index.
---

import PropertyComparisons from '../_property-comparisons.md';

# Items

Describes how to verify the number of items, how many items meet an expectation, a single item or the item at an
index.

| Expectation                         | Negated               | Summary                                                           |
|-------------------------------------|-----------------------|-------------------------------------------------------------------|
| [`HasCount`](#count)                | negated comparison    | has the expected number of items                                  |
| [`IsEmpty`](#count)                 | `IsNotEmpty`          | has no items                                                      |
| [`All()`, `Any()`, …](#quantifiers) | use `None()`          | the selected number of items meet an expectation                  |
| [`HasSingle`](#single-item)         |                       | has exactly one (matching) item                                   |
| [`HasItem`](#item-at-index)         | `DoesNotHaveItem`     | has the expected item at an index or at any index                 |
| [`HasItemThat`](#item-at-index)     | `DoesNotHaveItemThat` | has an item that meets an expectation at an index or at any index |

## Count

You can verify the number of items in a collection:

```csharp
IEnumerable<string> songs = ["Come Together", "Something", "Let It Be"];

await That(songs).HasCount(3);
await That(songs).HasCount().GreaterThan(2);
await That(songs).HasCount().NotBetween(5).And(10);
```

<PropertyComparisons example="HasCount(3)" />

You can also verify that the collection is empty or not:

```csharp
await That(Array.Empty<Track>()).IsEmpty();
await That(new[] { "Let It Be" }).IsNotEmpty();
```

## Quantifiers

You can verify that a certain number of items meet an expectation. A quantifier selects how many:

| Quantifier              | Succeeds when the expectation is met by             |
|-------------------------|-----------------------------------------------------|
| `All()`                 | every item                                          |
| `Any()`                 | at least one item (a shorthand for `AtLeast(1)`)    |
| `AtLeast(minimum)`      | at least `minimum` items                            |
| `AtMost(maximum)`       | at most `maximum` items                             |
| `Between(min).And(max)` | between `min` and `max` items, both bounds included |
| `Exactly(expected)`     | exactly `expected` items                            |
| `LessThan(maximum)`     | fewer than `maximum` items                          |
| `MoreThan(minimum)`     | more than `minimum` items                           |
| `None()`                | no item                                             |

and what the items must meet:

| Expectation                         | Succeeds for an item that                                             |
|-------------------------------------|-----------------------------------------------------------------------|
| [`ComplyWith`](#nested-expectation) | meets the nested expectation                                          |
| [`Satisfy`](#condition)             | satisfies the predicate                                               |
| [`AreEqualTo`](#equality)           | is equal to the expected value                                        |
| [`AreEquivalentTo`](#equality)      | is [equivalent](../04-values/13-equivalency.md) to the expected value |
| [`Are<T>`](#type)                   | is of type `T` or a derived type                                      |
| [`AreExactly<T>`](#type)            | is exactly of type `T`                                                |
| [`AreUnique`](#unique)              | occurs exactly once (`AreNotUnique`: more than once)                  |

An empty collection satisfies `All()`, like it does `Enumerable.All`, so
`That(new int[0]).All().Satisfy(x => false)` succeeds.

### Nested expectation

You can verify that items in a collection comply with an expectation on the individual items:

```csharp
IEnumerable<int> playCounts = [1, 2, 3];

await That(playCounts).All().ComplyWith(item => item.IsLessThan(4));
await That(playCounts).Any().ComplyWith(item => item.IsEqualTo(2));
await That(playCounts).AtLeast(2).ComplyWith(item => item.IsGreaterThanOrEqualTo(2));
await That(playCounts).AtMost(1).ComplyWith(item => item.IsNegative());
await That(playCounts).Between(2).And(3).ComplyWith(item => item.IsPositive());
await That(playCounts).Exactly(1).ComplyWith(item => item.IsEqualTo(2));
await That(playCounts).None().ComplyWith(item => item.IsNegative());
```

<details>
<summary>Items the nested expectation cannot answer</summary>

An item for which the nested expectation fails and its negation fails as well decides the result, because the
expectation could not answer it: the nested expectation threw (the exception becomes the `InnerException`), or the item
or an inspected member is `null`. So `None().ComplyWith(item => item.StartsWith("Let"))` fails for a `null` item, and so
does its negation. The same applies to [`HasItemThat`](#item-at-index) and to collections compared with item
expectations or predicates, where in any order such an item only decides when it matches no other expected item.

</details>

### Condition

You can verify that items in a collection satisfy a condition:

```csharp
Track[] tracks = [new() { Title = "Let It Be", PlayCount = 3 }, new() { Title = "Get Back", PlayCount = 0 }];

await That(tracks).Any().Satisfy(track => track.PlayCount > 0);
await That(tracks).None().Satisfy(track => track.Title == null);
```

[`Contains(predicate)`](./01-equality.md#predicate) verifies the same as `Any().Satisfy(…)`, and
[`HasItem`](#item-at-index) verifies the item at an index, see
[choosing an expectation](./00-index.md#choosing-an-expectation).

### Equality

You can verify that the items in the collection are equal to the `expected` value:

```csharp
IEnumerable<string> playlist = ["Let It Be", "Let It Be", "Let It Be"];

await That(playlist).All().AreEqualTo("Let It Be");
```

The items are [compared](./01-equality.md#comparing-items) like in the other collection expectations, so you can
specify a custom comparer, equivalency, a tolerance or a string option:

```csharp
IEnumerable<Album> albums = //...
Album expected = //...

await That(albums).All().AreEqualTo(expected).Equivalent();
await That(albums).All().AreEqualTo(expected).Using(new AlbumComparer());
await That(albums).AtLeast(2).AreEquivalentTo(expected);
await That(["let it be", "LET IT BE"]).All().AreEqualTo("Let It Be").IgnoringCase();
```

### Type

You can verify that the items are of a given type, or exactly of that type:

```csharp
IEnumerable<INotification> notifications = //...

await That(notifications).All().Are<INotification>();
await That(notifications).AtLeast(1).AreExactly<TrackAddedNotification>();
await That(notifications).None().Are(typeof(TrackRemovedNotification));
```

### Unique

You can verify how many items in a collection occur exactly once:

```csharp
await That(["Help!", "Yesterday", "Let It Be"]).All().AreUnique();
await That(["Help!", "Yesterday", "Let It Be", "Help!"]).AtLeast(1).AreNotUnique();
await That(["Help!", "Yesterday", "Help!", "Yesterday"]).None().AreUnique();
await That(["Help!", "Yesterday", "Let It Be", "Get Back", "Michelle", "Michelle"]).AtLeast(4).AreUnique();
```

For objects, you can also verify the uniqueness of a member, and you can use a
[custom comparer](../04-values/12-object.md#custom-comparer) or ignore the case of strings:

```csharp
Album[] albums = //...

await That(albums).All().AreUnique(x => x.Title);
await That(albums).All().AreUnique().Using(new AlbumComparer());
await That(["Help!", "Revolver"]).All().AreUnique().IgnoringCase();
```

For dictionaries, verify the [values](./04-dictionaries.md#as-collections) instead, as the keys are unique by design.

## Single item

You can verify that the collection contains a single item, and continue with expectations on it with `Which`:

```csharp
IEnumerable<int> playCounts = [42];

await That(playCounts).HasSingle();
await That(playCounts).HasSingle().Which.IsGreaterThan(41);
```

You can also apply filters for the items:

```csharp
IEnumerable<int> playCounts = [1, 2, 3];

await That(playCounts).HasSingle().Matching(it => it > 2);

// Or with generic type:
IEnumerable<Artist> artists = //...

await That(artists).HasSingle().Matching<Band>();
await That(artists).HasSingle().Matching<Band>(band => band.Members.Count == 4);

// Similar to the expectations above, but verify the type exactly:
await That(artists).HasSingle().MatchingExactly<Band>();
await That(artists).HasSingle().MatchingExactly<Band>(band => band.Members.Count == 4);
```

The awaited result is the single item:

```csharp
IEnumerable<string> songs = ["Let It Be"];

string song = await That(songs).HasSingle();
```

## Item at index

You can verify that the collection contains an item that satisfies the expectation on a given index (or any index):

```csharp
IEnumerable<string> songs = ["Two of Us", "Dig a Pony", "Across the Universe", "I Me Mine"];

await That(songs).HasItem("Dig a Pony").AtIndex(1); // at the zero-based index 1
await That(songs).HasItem("Across the Universe").AtIndexFromEnd(1); // zero-based index 1 from the end
await That(songs).HasItem(it => it.StartsWith("I Me")); // at any index
```

You can also check that the item matches a specific type:

```csharp
IEnumerable<INotification> values = //...

// Verify that the item at index 1 is of type `TrackAddedNotification`
await That(values).HasItem().Matching<TrackAddedNotification>().AtIndex(1);
// Verify that an item of type `TrackRemovedNotification` for the track with ID 3 exists
await That(values).HasItem().Matching<TrackRemovedNotification>(x => x.TrackId == 3);

// Similar to the expectations above, but verify the type exactly:
await That(values).HasItem().MatchingExactly<TrackAddedNotification>().AtIndex(1);
await That(values).HasItem().MatchingExactly<TrackRemovedNotification>(x => x.TrackId == 3);
```

You can also use expectations on the individual items:

```csharp
IEnumerable<string> songs = ["Two of Us", "Dig a Pony", "Across the Universe", "I Me Mine"];

await That(songs).HasItemThat(it => it.IsEqualTo("Dig a Pony")).AtIndex(1);
await That(songs).HasItemThat(it => it.StartsWith("Across").And.EndsWith("Universe")); // at any index
```

At any index, `HasItem(predicate)` and `HasItemThat` verify the same as `Any().Satisfy(…)` and
`Any().ComplyWith(…)`, see [choosing an expectation](./00-index.md#choosing-an-expectation).

Each of these expectations has a negated counterpart. It is satisfied when the index holds a different item, and also
when the collection is too short to have an item at that index:

```csharp
IEnumerable<string> songs = ["Two of Us", "Dig a Pony"];

await That(songs).DoesNotHaveItem("I Me Mine").AtIndex(1);
await That(songs).DoesNotHaveItem("Two of Us").AtIndex(4); // no item at index 4
await That(songs).DoesNotHaveItem().Matching(it => it.StartsWith("I Me")).AtIndex(1);
await That(songs).DoesNotHaveItemThat(it => it.StartsWith("I Me")).AtIndex(1);
```
