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
IEnumerable<int> values = Enumerable.Range(1, 3);

await Expect.That(values).IsEqualTo([1, 2, 3]);
await Expect.That(values).IsEqualTo([3, 2, 1]).InAnyOrder();
await Expect.That(values).IsEqualTo([1, 1, 2, 2, 3, 3]).IgnoringDuplicates();
await Expect.That(values).IsEqualTo([3, 3, 2, 2, 1, 1]).InAnyOrder().IgnoringDuplicates();

await Expect.That(values).IsNotEqualTo([2, 3]);
await Expect.That(values).IsNotEqualTo([4, 3, 2, 1]).InAnyOrder();
```

Without `InAnyOrder()` the items are compared in the order in which the collection enumerates them, which is not
defined for a [set](./index.md#sets).

## Contained items

You can verify that the collection contains a specific item or not:

```csharp
IEnumerable<string> songs = ["Come Together", "Something", "Let It Be"];

await Expect.That(songs).Contains("Let It Be");
await Expect.That(songs).DoesNotContain("Yesterday");
```

You can also set occurrence constraints on `Contains`:

```csharp
using aweXpect.Core; // for `Times()`

IEnumerable<int> values = [1, 1, 1, 2];

await Expect.That(values).Contains(1).MoreThan(1.Times());
await Expect.That(values).Contains(1).AtLeast(2.Times());
await Expect.That(values).Contains(1).Exactly(3.Times());
await Expect.That(values).Contains(1).AtMost(4.Times());
await Expect.That(values).Contains(1).LessThan(5.Times());
await Expect.That(values).Contains(1).Between(1).And(5.Times());
```

### Predicate

You can verify that the collection contains an item that satisfies a condition, with the same occurrence constraints:

```csharp
IEnumerable<int> values = [1, 1, 1, 2];

await Expect.That(values).Contains(x => x > 1);
await Expect.That(values).Contains(x => x == 1).Exactly(3.Times());
await Expect.That(values).DoesNotContain(x => x >= 42);
```

### Subset

You can verify that a collection contains another collection as a subset:

```csharp
IEnumerable<int> values = Enumerable.Range(1, 3);

await Expect.That(values).Contains([1, 2]);
await Expect.That(values).Contains([3, 2]).InAnyOrder();
await Expect.That(values).Contains([1, 1, 2, 2]).IgnoringDuplicates();
await Expect.That(values).Contains([3, 3, 1, 1]).InAnyOrder().IgnoringDuplicates();
await Expect.That(values).Contains([1, 3]).IgnoringInterspersedItems();
```

Without `InAnyOrder` the values must appear in the subject in the same order and contiguous, i.e. without other
items in between, so `[1, 3]` is not contained in `[1, 2, 3]` unless `IgnoringInterspersedItems` is used.
`InAnyOrder` and `IgnoringInterspersedItems` exclude each other, here and for `IsContainedIn`: specifying the second one
throws an `InvalidOperationException`.

To check for a proper subset, append `.Properly()` (which would fail for equal collections). The negation is
`DoesNotContain`.

### Superset

You can verify that a collection is contained in another collection (the other collection is a superset):

```csharp
IEnumerable<int> values = Enumerable.Range(1, 3);

await Expect.That(values).IsContainedIn([1, 2, 3, 4]);
await Expect.That(values).IsContainedIn([4, 3, 2, 1]).InAnyOrder();
await Expect.That(values).IsContainedIn([1, 1, 2, 2, 3, 3, 4, 4]).IgnoringDuplicates();
await Expect.That(values).IsContainedIn([1, 4, 2, 3]).IgnoringInterspersedItems();
await Expect.That(values).IsNotContainedIn([1, 2]);
```

Without `InAnyOrder` the values must appear in the expected collection in the same order and contiguous, i.e.
without other items in between, so `[1, 3]` is not contained in `[1, 2, 3]` unless `IgnoringInterspersedItems` is used.

To check for a proper superset, append `.Properly()` (which would fail for equal collections). The negation is
`IsNotContainedIn`.

## Start / end

You can verify if a collection starts or ends with other items or not:

```csharp
IEnumerable<string> songs = ["Come Together", "Something", "Let It Be"];

await Expect.That(songs).StartsWith("Come Together", "Something");
await Expect.That(songs).EndsWith("Let It Be");
await Expect.That(songs).DoesNotStartWith("Let It Be");
await Expect.That(songs).DoesNotEndWith("Something");
```

:::note
`EndsWith` and `DoesNotEndWith` always enumerate the complete collection.
:::

## Predicates and expectations per item

Instead of the expected items, `IsEqualTo`, `Contains` and `IsContainedIn` (and their negations) also accept one
predicate or one expectation per item, with the same options for the order, duplicates and interspersed items:

```csharp
IEnumerable<string> songs = ["Come Together", "Something", "Let It Be"];

await Expect.That(songs).IsEqualTo([x => x.Length > 10, x => x == "Something", x => x.EndsWith("Be")]);
await Expect.That(songs).IsEqualTo([
  x => x.IsEqualTo("Let It Be"),
  x => x.StartsWith("Come"),
  x => x.IsNotEmpty(),
]).InAnyOrder();
await Expect.That(songs).Contains([x => x == "Something", x => x.EndsWith("Be")]);
```

The failure message lists the predicates or expectations as the expected items:

```csharp
using System.Linq.Expressions;

IEnumerable<string> subject = ["a", "b", "c"];
IEnumerable<Expression<Func<string, bool>>> expected = [x => x == "a", x => x == "b", x => x == "c", x => x == "d"];

await Expect.That(subject).IsEqualTo(expected);
```

```text title="Failure message"
Expected that subject
is equal to collection expected in order,
but it lacked 1 of 4 expected items: x => (x == "d")

Collection:
[
  "a",
  "b",
  "c"
]

Expected:
[
  x => (x == "a"),
  x => (x == "b"),
  x => (x == "c"),
  x => (x == "d")
]
```

## Comparing items

The expectations on this page compare the items with the expected items by their default equality, unless you specify a
[custom comparer](../04-values/12-object.md#custom-comparer), [equivalency](../04-values/13-equivalency.md), a
[tolerance](./index.md#tolerance) or, for strings, one of the [string options](../04-values/03-string.md#string-options):

```csharp
IEnumerable<Album> albums = //...
Album expected = //...

await Expect.That(albums).Contains(expected).Equivalent();
await Expect.That(albums).StartsWith(expected).Using(new AlbumComparer());
await Expect.That(["COME TOGETHER", "SOMETHING"]).EndsWith(["something"]).IgnoringCase();
```

The rules for [sets](./index.md#sets) with a custom comparer apply as well.
