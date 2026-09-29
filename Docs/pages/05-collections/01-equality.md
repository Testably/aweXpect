# Equality and containment

Describes how to compare a collection with another collection, and how to verify that it contains, starts with or ends
with items. The rules for [sets](./index.md#sets) and [tolerances](./index.md#tolerance) apply to all of them.

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
await Expect.That(values).IsNotEqualTo([1, 1, 3, 3, 2, 2]).IgnoringDuplicates();
await Expect.That(values).IsNotEqualTo([3, 3, 2, 2, 1, 1, 4]).InAnyOrder().IgnoringDuplicates();
```

Without `InAnyOrder()` the items are compared in the order in which the collection enumerates them, which is not
defined for a [set](./index.md#sets).

## Contained items

You can verify that the collection contains a specific item or not:

```csharp
IEnumerable<int> values = Enumerable.Range(1, 20);

await Expect.That(values).Contains(13);
await Expect.That(values).DoesNotContain(42);
```

You can also set occurrence constraints on `Contain`:

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

You can also use a [custom comparer](../common-types/06-object.md#custom-comparer) or
configure [equivalency](../06-equivalency.md):

```csharp
IEnumerable<Album> albums = //...
Album expected = //...

await Expect.That(albums).Contains(expected).Equivalent();
await Expect.That(albums).Contains(expected).Using(new AlbumComparer());
```

### Predicate

You can verify that the collection contains an item that satisfies a condition:

```csharp
IEnumerable<int> values = Enumerable.Range(1, 20);

await Expect.That(values).Contains(x => x > 12 && x < 14);
await Expect.That(values).DoesNotContain(x => x >= 42);
```

You can also set occurrence constraints on `Contain`:

```csharp
IEnumerable<int> values = [1, 1, 1, 2];

await Expect.That(values).Contains(x => x == 1).AtLeast(2.Times());
await Expect.That(values).Contains(x => x == 1).Exactly(3.Times());
await Expect.That(values).Contains(x => x == 1).AtMost(4.Times());
await Expect.That(values).Contains(x => x == 1).Between(1).And(5.Times());
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

To check for a proper subset, append `.Properly()` (which would fail for equal collections). The negation is
`DoesNotContain`.

### Superset

You can verify that a collection is contained in another collection (the other collection is a superset):

```csharp
IEnumerable<int> values = Enumerable.Range(1, 3);

await Expect.That(values).IsContainedIn([1, 2, 3, 4]);
await Expect.That(values).IsContainedIn([4, 3, 2, 1]).InAnyOrder();
await Expect.That(values).IsContainedIn([1, 1, 2, 2, 3, 3, 4, 4]).IgnoringDuplicates();
await Expect.That(values).IsContainedIn([4, 4, 3, 3, 2, 2, 1, 1]).InAnyOrder().IgnoringDuplicates();
await Expect.That(values).IsContainedIn([1, 4, 2, 3]).IgnoringInterspersedItems();
```

Without `InAnyOrder` the values must appear in the expected collection in the same order and contiguous, i.e.
without other items in between, so `[1, 3]` is not contained in `[1, 2, 3]` unless `IgnoringInterspersedItems` is used.

To check for a proper superset, append `.Properly()` (which would fail for equal collections). The negation is
`IsNotContainedIn`.

## Collection start

You can verify if a collection starts with another collection or not:

```csharp
IEnumerable<int> values = Enumerable.Range(1, 3);

await Expect.That(values).StartsWith(1, 2);
await Expect.That(values).DoesNotStartWith(2, 3);
```

You can also use a [custom comparer](../common-types/06-object.md#custom-comparer) or
configure [equivalency](../06-equivalency.md):

```csharp
IEnumerable<Album> albums = //...
Album expected = //...

await Expect.That(albums).StartsWith(expected).Equivalent();
await Expect.That(albums).StartsWith(expected).Using(new AlbumComparer());
```

For strings, you can configure this expectation to ignore case, ignore newline style, ignore the indentation, ignoring
leading or trailing whitespace, or use a custom `IEqualityComparer<string>`:

```csharp
await Expect.That(["FOO", "BAR"]).StartsWith(["foo"]).IgnoringCase();
```

## Collection end

You can verify if a collection ends with another collection or not:

```csharp
IEnumerable<int> values = Enumerable.Range(1, 5);

await Expect.That(values).EndsWith(4, 5);
await Expect.That(values).DoesNotEndWith(3, 5);
```

You can also use a [custom comparer](../common-types/06-object.md#custom-comparer) or
configure [equivalency](../06-equivalency.md):

```csharp
IEnumerable<Album> albums = //...
Album expected = //...

await Expect.That(albums).EndsWith(expected).Equivalent();
await Expect.That(albums).EndsWith(expected).Using(new AlbumComparer());
```

For strings, you can configure this expectation to ignore case, ignore newline style, ignore the indentation, ignoring
leading or trailing whitespace, or use a custom `IEqualityComparer<string>`:

```csharp
await Expect.That(["FOO", "BAR"]).EndsWith(["bar"]).IgnoringCase();
```

:::note
`EndsWith` and `DoesNotEndWith` always enumerate the complete collection.
:::
