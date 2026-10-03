# Collections

Describes the possible expectations for collections, e.g. arrays, lists, sets or any other `IEnumerable<T>`.

| Page                                         | Expectations                                                                        |
|----------------------------------------------|-------------------------------------------------------------------------------------|
| [Equality and containment](./01-equality.md) | `IsEqualTo`, `Contains`, `IsContainedIn`, `StartsWith`, `EndsWith`                  |
| [Items](./02-items.md)                       | `HasCount`, `IsEmpty`, quantifiers like `All()` or `None()`, `HasSingle`, `HasItem` |
| [Sort order](./03-order.md)                  | `IsInAscendingOrder`, `IsInDescendingOrder`                                         |
| [Dictionaries](./04-dictionaries.md)         | `ContainsKey`, `ContainsValue`, `Keys`, `Values` and dictionary equality            |

:::note[.NET 8 or later: `IAsyncEnumerable<T>`]
On .NET 8 or later, every collection expectation, except the ones for [dictionaries](./04-dictionaries.md), works the
same way for an `IAsyncEnumerable<T>`. The expectation enumerates it asynchronously and passes its
[cancellation token](../03-how-it-works/06-time-and-cancellation.md#async-enumerables).
:::

:::info[C# 13 or later]
Some overloads rely on `[OverloadResolutionPriority]` to bind as described here, e.g. so that a `string` is expected
as a single item and not as a sequence of characters, or that a `params` list or a collection expression `[…]` is the
expected collection. The attribute only takes effect with C# 13 or later, which is the default only for .NET 9 and
later. With an older language version, such a call can bind to a different overload or fail with CS0121, so set
`<LangVersion>` to `13` or `latest` in a project that targets an older framework.
:::

## Spans

:::note[.NET 8 or later]
Spans can only be passed to `Expect.That` on .NET 8 or later. On older targets, verify a property of the span, e.g.
its `Length`, [synchronously](../03-how-it-works/index.md#when-you-cannot-await).
:::

You can pass a `Span<T>` or a `ReadOnlySpan<T>` directly to `Expect.That`. The items are copied into a
`SpanWrapper<T>`, which implements `ICollection<T>`, so all collection expectations are available:

```csharp
await Expect.That("Help!".AsSpan()).HasCount(5);
await Expect.That("Help!".AsSpan()).Contains('!');
```

A span can't be kept across an `await`, so create it in the statement of the expectation.

## Immutable arrays

:::note[.NET 8 or later]
Before .NET 8, only `IsEmpty` and `IsNotEmpty` are available for an `ImmutableArray<T>`. Call `.AsEnumerable()` on the
array to use the other collection expectations.
:::

## Sets

:::warning[A set or a dictionary has no defined order]
`IsEqualTo`, `Contains` with a collection and `IsContainedIn` compare the items in the order in which the collection
enumerates them unless `InAnyOrder()` is used, so for a `HashSet<T>`, or for the entries, keys or values of a
`Dictionary<TKey, TValue>`, the result depends on an implementation detail. `IsEqualTo` on a dictionary itself compares
the entries [by key](./04-dictionaries.md#equality) instead. The analyzer rule
[aweXpect0006](../07-analyzers.md#awexpect0006) warns about it and offers to append `.InAnyOrder()`. It also warns about `StartsWith`, `EndsWith` and `IgnoringInterspersedItems()`, which
have no meaning for such a collection. Sorted sets and dictionaries are not reported.
:::

A set that was created with a custom comparer (a `HashSet<T>`, `SortedSet<T>`, `ImmutableHashSet<T>`,
`ImmutableSortedSet<T>` or `FrozenSet<T>`) compares its items with that comparer. Any other collection, including a set
with the default comparer, is compared with the default equality. The comparer of such a set decides in every
expectation that compares its items: `IsEqualTo`, `Contains` (with an item or a subset), `IsContainedIn`, `HasItem`,
`StartsWith`, `EndsWith` and `All().AreEqualTo`. For `Contains` with an item, the set is asked for the item itself, so
the item is counted at most once.

Only the comparer of the subject is used, not the one of an expected set. A custom comparer, equivalency, a
[tolerance](#tolerance) or a string option such as `IgnoringCase()` takes precedence over the comparer of the set, and
`Using(EqualityComparer<T>.Default)` forces the default equality:

```csharp
HashSet<string> albums = new(StringComparer.OrdinalIgnoreCase) { "Help!", "Revolver" };

await Expect.That(albums).Contains("HELP!");
await Expect.That(albums).IsEqualTo(["REVOLVER", "HELP!"]).InAnyOrder();
await Expect.That(albums).IsContainedIn(["REVOLVER", "ABBEY ROAD", "HELP!"]).InAnyOrder();
await Expect.That(albums).DoesNotContain("HELP!").Using(StringComparer.Ordinal);
await Expect.That(albums).IsNotEqualTo(["REVOLVER", "HELP!"]).InAnyOrder().Using(StringComparer.Ordinal);
```

Whenever the comparer of the set decides, the expectation names it, e.g.
`contains "HELP!" using the subject's StringComparer.OrdinalIgnoreCase at least once`. For an untyped `IEnumerable`,
only a set of the expected item type is recognised, e.g. a `HashSet<string>` for an expected string.

## Tolerance

The expectations that compare items with an expected value accept a tolerance for numbers such as `int`, `long`,
`double` or `decimal`, for `DateTime`, `DateTimeOffset` and `TimeSpan` items, and
[on .NET 8 or later](../02-getting-started.md#target-frameworks) also for `DateOnly`, whose tolerance must be a whole
number of days, and `TimeOnly`, whose items are compared on the clock face, so that `23:59` and `00:01` are two minutes
apart:

```csharp
IEnumerable<double> durations = [1.01, 2.02, 3.04];

await Expect.That(durations).IsEqualTo([3.0, 2.0, 1.0]).Within(0.1).InAnyOrder();
await Expect.That(durations).Contains(2.0).Within(0.1);
await Expect.That(durations).Contains([2.0, 3.0]).Within(0.1);
await Expect.That(durations).IsContainedIn([1.0, 2.0, 3.0, 4.0]).Within(0.1);
await Expect.That(durations).StartsWith(1.0, 2.0).Within(0.1);
await Expect.That(durations).EndsWith(2.0, 3.0).Within(0.1);
await Expect.That(durations).HasItem(2.0).Within(0.1).AtIndex(1);
await Expect.That([2.04, 2.02, 2.01]).All().AreEqualTo(2.0).Within(0.1);
await Expect.That([9, 20, 31]).Contains(10).Within(1);
```

A tolerance takes precedence over the comparer of a set. The values of a [dictionary](./04-dictionaries.md#values)
accept the same tolerance.

Without `Within`, the items of the time types use the
[default tolerance](../04-values/10-datetime-offset.md#default-tolerance), if one is set.
