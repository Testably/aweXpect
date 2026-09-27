# Sort order

You can verify if a collection is (or is not) sorted in ascending or descending order:

```csharp
await Expect.That([1, 2, 3]).IsInAscendingOrder();
await Expect.That([1, 3, 2]).IsNotInAscendingOrder();
await Expect.That(["c", "b", "a"]).IsInDescendingOrder();
await Expect.That(["c", "a", "b"]).IsNotInDescendingOrder();
```

You can also specify a custom comparer:

```csharp
await Expect.That(["a", "B", "c"]).IsInAscendingOrder().Using(StringComparer.OrdinalIgnoreCase);
```

For objects, you can also verify the sort order on a member:

```csharp
Album[] albums = //...

await Expect.That(albums).IsInAscendingOrder(x => x.Title);
```

A `SortedSet<T>` (or on .NET 8 or later an `ImmutableSortedSet<T>`) with a custom comparer is ordered by that comparer,
unless a comparer or a member is specified.

A collection of `DateTime` values (or a `DateTime` member) that contains both `DateTimeKind.Utc` and
`DateTimeKind.Local` values fails the check, in its negated form as well, as the order of such values depends on the
time zone. Values with `DateTimeKind.Unspecified` are compatible with both kinds. With a custom comparer, the comparer
decides.
