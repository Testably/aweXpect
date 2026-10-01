# Sort order

Describes how to verify that the items of a collection are sorted.

| Expectation                                     | Negated                  | Summary                         |
|-------------------------------------------------|--------------------------|---------------------------------|
| [`IsInAscendingOrder`](#ascending--descending)  | `IsNotInAscendingOrder`  | the items are sorted ascending  |
| [`IsInDescendingOrder`](#ascending--descending) | `IsNotInDescendingOrder` | the items are sorted descending |

## Ascending / descending

You can verify if a collection is (or is not) sorted in ascending or descending order:

```csharp
await Expect.That([1, 2, 3]).IsInAscendingOrder();
await Expect.That([1, 3, 2]).IsNotInAscendingOrder();
await Expect.That(["Let It Be", "Help!", "Abbey Road"]).IsInDescendingOrder();
await Expect.That(["Help!", "Abbey Road", "Let It Be"]).IsNotInDescendingOrder();
```

You can also specify a custom comparer:

```csharp
await Expect.That(["abbey road", "Help!", "let it be"]).IsInAscendingOrder().Using(StringComparer.OrdinalIgnoreCase);
```

For objects, you can also verify the sort order on a member:

```csharp
Album[] albums = //...

await Expect.That(albums).IsInAscendingOrder(x => x.Title);
```

Strings are compared ordinally by default, also in an untyped collection. A `SortedSet<T>` or `ImmutableSortedSet<T>`,
and the keys of a sorted dictionary, are ordered by their own comparer, unless a comparer or a member is specified.

A collection of `DateTime` values (or a `DateTime` member) that contains both `DateTimeKind.Utc` and
`DateTimeKind.Local` values fails the check, in its negated form as well, as the order of such values depends on the
time zone. Values with `DateTimeKind.Unspecified` are compatible with both kinds. With a custom comparer, the comparer
decides.
