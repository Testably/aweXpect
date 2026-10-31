---
description: Verify that the items of a collection are sorted in ascending or descending order.
---

# Sort order

Describes how to verify that the items of a collection are sorted.

| Expectation                                     | Negated                  | Summary                         |
|-------------------------------------------------|--------------------------|---------------------------------|
| [`IsInAscendingOrder`](#ascending--descending)  | `IsNotInAscendingOrder`  | the items are sorted ascending  |
| [`IsInDescendingOrder`](#ascending--descending) | `IsNotInDescendingOrder` | the items are sorted descending |

## Ascending / descending

You can verify that a collection is (or is not) sorted in ascending or descending order:

```csharp
IEnumerable<int> trackNumbers = [1, 2, 3];
IEnumerable<int> shuffledTrackNumbers = [1, 3, 2];

await That(trackNumbers).IsInAscendingOrder();
await That(shuffledTrackNumbers).IsNotInAscendingOrder();
await That(["Let It Be", "Help!", "Abbey Road"]).IsInDescendingOrder();
await That(["Help!", "Abbey Road", "Let It Be"]).IsNotInDescendingOrder();
```

You can also specify a custom comparer:

```csharp
await That(["abbey road", "Help!", "let it be"]).IsInAscendingOrder()
  .Using(StringComparer.OrdinalIgnoreCase);
```

For objects, you can also verify the sort order on a member:

```csharp
Album[] albums = //...

await That(albums).IsInAscendingOrder(x => x.Title);
```

Strings are compared ordinally by default, also in an untyped collection. A `SortedSet<T>` or `ImmutableSortedSet<T>`,
and the keys of a sorted dictionary, are ordered by their own comparer, unless a comparer or a member is specified.

<details>
<summary>Mixed `DateTimeKind` values</summary>

A `DateTime` sequence that mixes `Utc` and `Local` values fails, also when negated, unless a custom comparer is used.
The order of `Utc` and `Local` values depends on the time zone, so such a sequence fails the check in both forms. This
also applies to a `DateTime` member and to an untyped collection. Values with `DateTimeKind.Unspecified` are
compatible with both kinds.

</details>
