---
description: Compare a collection with another collection and verify that it contains, starts with or ends with items.
---

# Equality and containment

Describes how to compare a collection with another collection, and how to verify that it contains, starts with or ends
with items.

| Expectation                          | Negated            | Summary                                                |
|--------------------------------------|--------------------|--------------------------------------------------------|
| [`IsEqualTo`](#equality)             | `IsNotEqualTo`     | has the same items as the expected collection          |
| [`Contains(item)`](#contained-items) | `DoesNotContain`   | contains the item, optionally a number of times        |
| [`Contains(predicate)`](#predicate)  | `DoesNotContain`   | contains an item that satisfies the predicate          |
| [`Contains(collection)`](#subset)    | `DoesNotContain`   | contains the expected items (a subset)                 |
| [`IsContainedIn`](#superset)         | `IsNotContainedIn` | all its items are contained in the expected collection |
| [`StartsWith`](#start--end)          | `DoesNotStartWith` | starts with the expected items                         |
| [`EndsWith`](#start--end)            | `DoesNotEndWith`   | ends with the expected items                           |

## Equality

You can verify that a collection is equal to another collection:

```csharp
IEnumerable<int> trackNumbers = Enumerable.Range(1, 3);

await That(trackNumbers).IsEqualTo([1, 2, 3]);
await That(trackNumbers).IsEqualTo([3, 2, 1]).InAnyOrder();
await That(trackNumbers).IsEqualTo([1, 1, 2, 2, 3, 3]).IgnoringDuplicates();
await That(trackNumbers).IsEqualTo([3, 3, 2, 2, 1, 1]).InAnyOrder().IgnoringDuplicates();

await That(trackNumbers).IsNotEqualTo([2, 3]);
await That(trackNumbers).IsNotEqualTo([4, 3, 2, 1]).InAnyOrder();
```

Without `InAnyOrder()` the items are compared in the order in which the collection enumerates them, which is not
defined for a [set](#sets).

With `IgnoringDuplicates()`, only which items occur matters, not how often: every expected item has to be matched by an
item of the collection, and every item of the collection has to match an expected item.

<details>
<summary>Multi-dimensional arrays</summary>

A multi-dimensional array is only equal to an array of the same rank with the same length in every dimension, also with
`InAnyOrder()` or `IgnoringDuplicates()`, while the other expectations on this page only look at its items. A failed
comparison with a collection names an item of such an array by its index in every dimension (e.g. `[1,2]`). The same
holds for [equivalency](../04-values/13-equivalency.md#collections-and-dictionaries), also when the collection order is
ignored.

</details>

## Contained items

You can verify that the collection contains a specific item or not:

```csharp
IEnumerable<string> songs = ["Come Together", "Something", "Let It Be"];

await That(songs).Contains("Let It Be");
await That(songs).DoesNotContain("Yesterday");
```

You can also set occurrence constraints on `Contains`:

```csharp
IEnumerable<string> playlist = ["Let It Be", "Let It Be", "Let It Be", "Yesterday"];

await That(playlist).Contains("Let It Be").MoreThan(1);
await That(playlist).Contains("Let It Be").AtLeast(2);
await That(playlist).Contains("Let It Be").Exactly(3);
await That(playlist).Contains("Let It Be").AtMost(4);
await That(playlist).Contains("Let It Be").LessThan(5);
await That(playlist).Contains("Let It Be").Between(1).And(5);
await That(playlist).Contains("Yesterday").Once();
await That(playlist).Contains("Let It Be").AtLeast().Twice();
await That(playlist).Contains("Help!").Never();
```

`AtLeast(2.Times())` with the `Times()` extension from `aweXpect.Core` reads the same, but the plain number is enough.

### Predicate

You can verify that the collection contains an item that satisfies a condition, with the same occurrence constraints:

```csharp
IEnumerable<int> playCounts = [1, 1, 1, 2];

await That(playCounts).Contains(x => x > 1);
await That(playCounts).Contains(x => x == 1).Exactly(3);
await That(playCounts).DoesNotContain(x => x >= 42);
```

A [quantifier](./02-items.md#quantifiers) such as `Any().Satisfy(…)` verifies the same and also accepts a nested
expectation, and [`HasItem`](./02-items.md#item-at-index) verifies the item at an index, see
[choosing an expectation](./00-index.md#choosing-an-expectation).

### Subset

You can verify that a collection contains another collection as a subset:

```csharp
IEnumerable<int> trackNumbers = Enumerable.Range(1, 3);

await That(trackNumbers).Contains([1, 2]);
await That(trackNumbers).Contains([3, 2]).InAnyOrder();
await That(trackNumbers).Contains([1, 1, 2, 2]).IgnoringDuplicates();
await That(trackNumbers).Contains([3, 3, 1, 1]).InAnyOrder().IgnoringDuplicates();
await That(trackNumbers).Contains([1, 3]).IgnoringInterspersedItems();
```

Without `InAnyOrder` the values must appear in the subject in the same order and contiguous, i.e. without other
items in between, so `[1, 3]` is not contained in `[1, 2, 3]` unless `IgnoringInterspersedItems` is used.

`IgnoringDuplicates()` works as for [equality](#equality), except that only the expected items have to be matched.

To check for a proper subset, append `.Properly()` (which would fail for equal collections).

<details>
<summary>`InAnyOrder` together with `IgnoringInterspersedItems`</summary>

`InAnyOrder` and `IgnoringInterspersedItems` exclude each other, here and for `IsContainedIn`: specifying the second one
throws an `InvalidOperationException`.

</details>

### Superset

You can verify that a collection is contained in another collection (the other collection is a superset):

```csharp
IEnumerable<int> trackNumbers = Enumerable.Range(1, 3);

await That(trackNumbers).IsContainedIn([1, 2, 3, 4]);
await That(trackNumbers).IsContainedIn([4, 3, 2, 1]).InAnyOrder();
await That(trackNumbers).IsContainedIn([1, 1, 2, 2, 3, 3, 4, 4]).IgnoringDuplicates();
await That(trackNumbers).IsContainedIn([1, 4, 2, 3]).IgnoringInterspersedItems();
await That(trackNumbers).IsNotContainedIn([1, 2]);
```

Without `InAnyOrder` the values must appear in the expected collection in the same order and contiguous, i.e.
without other items in between, so `[1, 3]` is not contained in `[1, 2, 3]` unless `IgnoringInterspersedItems` is used.

`IgnoringDuplicates()` works as for [equality](#equality), except that only the items of the collection have to match.

To check for a proper superset, append `.Properly()` (which would fail for equal collections).

## Start / end

You can verify that a collection starts or ends with other items or not:

```csharp
IEnumerable<string> songs = ["Come Together", "Something", "Let It Be"];

await That(songs).StartsWith("Come Together", "Something");
await That(songs).EndsWith("Let It Be");
await That(songs).DoesNotStartWith("Let It Be");
await That(songs).DoesNotEndWith("Something");
```

:::note
`EndsWith` and `DoesNotEndWith` always enumerate the complete collection.
:::

## Predicates and expectations per item

Instead of the expected items, `IsEqualTo`, `Contains` and `IsContainedIn` (and their negations) also accept one
predicate or one expectation per item, with the same options as with expected items:

```csharp
IEnumerable<string> songs = ["Come Together", "Something", "Let It Be"];

await That(songs).IsEqualTo([x => x.Length > 10, x => x == "Something", x => x.EndsWith("Be")]);
await That(songs).IsEqualTo([
  x => x.IsEqualTo("Let It Be"),
  x => x.StartsWith("Come"),
  x => x.IsNotEmpty(),
]).InAnyOrder();
await That(songs).Contains([x => x == "Something", x => x.EndsWith("Be")]);
```

The failure message lists the predicates or expectations as the expected items:

```csharp
using System.Linq.Expressions;

IEnumerable<string> songs = ["Two of Us", "Dig a Pony", "Let It Be"];
Expression<Func<string, bool>>[] expected =
  [x => x == "Two of Us", x => x == "Dig a Pony", x => x == "Let It Be", x => x == "Get Back"];

await That(songs).IsEqualTo(expected);
```

```text title="Failure message"
Expected that songs
is equal to collection expected in order,
but it lacked 1 of 4 expected items: x => (x == "Get Back")

Collection:
[
  "Two of Us",
  "Dig a Pony",
  "Let It Be"
]

Expected:
[
  x => (x == "Two of Us"),
  x => (x == "Dig a Pony"),
  x => (x == "Let It Be"),
  x => (x == "Get Back")
]
```

<details>
<summary>Predicates with `IgnoringDuplicates()`</summary>

Predicates and expectations are never compared with each other. With `IgnoringDuplicates()`, one item may therefore
match several of them, e.g. `[5]` is equal to `[x => x > 0, x => x > 1]` in any order ignoring duplicates.

</details>

## Comparing items

The expectations on this page compare the items with the expected items by their default equality, unless you specify a
[custom comparer](../04-values/12-object.md#custom-comparer), [equivalency](../04-values/13-equivalency.md), a
[tolerance](#tolerance) or, for strings, one of the [string options](../04-values/03-string.md#string-options):

```csharp
IEnumerable<Album> albums = //...
Album expected = //...

await That(albums).Contains(expected).Equivalent();
await That(albums).StartsWith(expected).Using(new AlbumComparer());
await That(["COME TOGETHER", "SOMETHING"]).EndsWith(["something"]).IgnoringCase();
```

The rules for [sets](#sets) with a custom comparer apply as well.

### Sets

:::warning[A set or a dictionary has no defined order]
`IsEqualTo`, `Contains` with a collection and `IsContainedIn` compare the items in the order in which the collection
enumerates them unless `InAnyOrder()` is used, so for a `HashSet<T>`, or for the entries, keys or values of a
`Dictionary<TKey, TValue>`, the result depends on an implementation detail. `IsEqualTo` on a dictionary itself compares
the entries [by key](./04-dictionaries.md#equality) instead. The analyzer rule
[aweXpect0006](../08-analyzers.md#awexpect0006) warns about it and offers to append `.InAnyOrder()`. It also warns
about `StartsWith`, `EndsWith` and `IgnoringInterspersedItems()`, which have no meaning for such a collection. Sorted
sets and dictionaries are not reported.
:::

A set that was created with a custom comparer (a `HashSet<T>`, `SortedSet<T>`, `ImmutableHashSet<T>`,
`ImmutableSortedSet<T>` or `FrozenSet<T>`) compares its items with that comparer. Any other collection, including a set
with the default comparer, is compared with the default equality. The comparer of such a set decides in every
expectation that compares its items: `IsEqualTo`, `Contains` (with an item or a subset), `IsContainedIn`, `HasItem`,
`StartsWith`, `EndsWith` and `All().AreEqualTo`.

Only the comparer of the subject is used, not the one of an expected set. A custom comparer, equivalency, a
[tolerance](#tolerance) or a string option such as `IgnoringCase()` takes precedence over the comparer of the set, and
`Using(EqualityComparer<T>.Default)` forces the default equality:

```csharp
HashSet<string> albums = new(StringComparer.OrdinalIgnoreCase) { "Help!", "Revolver" };

await That(albums).Contains("HELP!");
await That(albums).IsEqualTo(["REVOLVER", "HELP!"]).InAnyOrder();
await That(albums).IsContainedIn(["REVOLVER", "ABBEY ROAD", "HELP!"]).InAnyOrder();
await That(albums).DoesNotContain("HELP!").Using(StringComparer.Ordinal);
await That(albums).IsNotEqualTo(["REVOLVER", "HELP!"]).InAnyOrder().Using(StringComparer.Ordinal);
```

Whenever the comparer of the set decides, the expectation names it, e.g.
`contains "HELP!" using the subject's StringComparer.OrdinalIgnoreCase at least once`.

<details>
<summary>Untyped collections</summary>

For an untyped `IEnumerable`, only a set of the expected item type is recognized, e.g. a `HashSet<string>` for an expected string.

</details>

### Tolerance

The expectations that compare items with an expected value accept a tolerance for numbers such as `int`, `long`,
`double` or `decimal`, for `DateTime`, `DateTimeOffset` and `TimeSpan` items, and
[on .NET 8 or later](../03-setup/02-target-frameworks.md) also for `DateOnly` and `TimeOnly`:

```csharp
IEnumerable<double> durations = [1.01, 2.02, 3.04];
IEnumerable<int> playCounts = [9, 20, 31];

await That(durations).IsEqualTo([3.0, 2.0, 1.0]).Within(0.1).InAnyOrder();
await That(durations).Contains(2.0).Within(0.1);
await That(durations).Contains([2.0, 3.0]).Within(0.1);
await That(durations).IsContainedIn([1.0, 2.0, 3.0, 4.0]).Within(0.1);
await That(durations).StartsWith(1.0, 2.0).Within(0.1);
await That(durations).EndsWith(2.0, 3.0).Within(0.1);
await That(durations).HasItem(2.0).Within(0.1).AtIndex(1);
await That([2.04, 2.02, 2.01]).All().AreEqualTo(2.0).Within(0.1);
await That(playCounts).Contains(10).Within(1);
```

The values of a [dictionary](./04-dictionaries.md#contained) accept the same tolerance.

For `double`, `float` and `decimal` items, `Percent()` directly after `Within` interprets the tolerance as a
[percentage](../04-values/02-number.md#tolerance) of each expected item:

```csharp
await That([104.0, 209.0]).IsEqualTo([100.0, 200.0]).Within(5).Percent();
```

Without `Within`, the items of the time types use the
[default tolerance](../02-how-it-works/05-options.md#default-tolerance), if one is set.

<details>
<summary>`DateOnly` and `TimeOnly` tolerances</summary>

The tolerance for `DateOnly` items must be a whole number of days. `TimeOnly` items are compared on the clock face, so
that `23:59` and `00:01` are two minutes apart.

</details>
