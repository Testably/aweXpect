# Collections

Describes the possible expectations for collections:

- [Equality and containment](./01-equality.md): compare a collection with another one, or verify that it contains, starts
  or ends with items.
- [Items](./02-items.md): verify the number of items, how many items meet an expectation, a single item or the item at
  an index.
- [Sort order](./03-order.md): verify that the items are sorted.
- [Dictionaries](./04-dictionaries.md): verify the entries, keys and values of a dictionary.

Every expectation has a negated counterpart (`IsNot…`/`DoesNot…`), except `HasCount`, `HasSingle` and the
[quantifiers](./02-items.md#elements) like `All()`, which take a negated comparison or `None()` instead.

:::tip[`IAsyncEnumerable<T>`]
On .NET 8 or later, every collection expectation, except the ones for [dictionaries](./04-dictionaries.md), works the
same way for an `IAsyncEnumerable<T>`.
:::

:::info[C# 13 or later]
Some overloads rely on `[OverloadResolutionPriority]` to bind as described here, e.g. so that a `string` is expected
as a single item and not as a sequence of characters, or that a `params` list or a collection expression `[…]` is the
expected collection. The attribute only takes effect with C# 13 or later, which is the default only for .NET 9 and
later. With an older language version, such a call can bind to a different overload or fail with CS0121, so set
`<LangVersion>` to `13` or `latest` in a project that targets an older framework.
:::

## Spans

On .NET 8 or later, you can pass a `Span<T>` or a `ReadOnlySpan<T>` directly to `Expect.That`. The items are copied
into a `SpanWrapper<T>`, which implements `ICollection<T>`, so all [collection expectations](./index.md)
are available:

```csharp
await Expect.That("foo".AsSpan()).HasCount(3);
await Expect.That("foo".AsSpan()).IsEqualTo(['f', 'o', 'o']);
```

A span can't be kept across an `await`, so create it in the statement of the expectation.

## Sets

:::warning[A set or a dictionary has no defined order]
`IsEqualTo`, `Contains` with a collection and `IsContainedIn` compare the items in the order in which the collection
enumerates them unless `InAnyOrder()` is used, so for a `HashSet<T>` or a `Dictionary<TKey, TValue>` the result depends
on an implementation detail. The analyzer rule `aweXpect0006` warns about it and offers to append `.InAnyOrder()`. It
also warns about `StartsWith`, `EndsWith` and `IgnoringInterspersedItems()`, which have no meaning for such a
collection. Sorted sets and dictionaries are not reported.
:::

A set that was created with a custom comparer (a `HashSet<T>` or `SortedSet<T>`, and on .NET 8 or later also an
`ImmutableHashSet<T>`, `ImmutableSortedSet<T>` or `FrozenSet<T>`) compares its items with that comparer. Any other
collection, including a set with the default comparer, is compared with the default equality. The comparer of such a
set decides in every expectation that compares its items: `IsEqualTo`, `Contains` (with an item or a subset),
`IsContainedIn`, `HasItem`, `StartsWith`, `EndsWith` and `All().AreEqualTo`. For `Contains` with an item, the set is
asked for the item itself, so the item is counted at most once.

Only the comparer of the subject is used, not the one of an expected set. A custom comparer, equivalency, a
[tolerance](#tolerance) or a string option such as `IgnoringCase()` takes precedence over the comparer of the set, and
`Using(EqualityComparer<T>.Default)` forces the default equality:

```csharp
HashSet<string> values = new(StringComparer.OrdinalIgnoreCase) { "foo", "bar" };

await Expect.That(values).Contains("FOO");
await Expect.That(values).IsEqualTo(["BAR", "FOO"]).InAnyOrder();
await Expect.That(values).IsContainedIn(["BAR", "BAZ", "FOO"]).InAnyOrder();
await Expect.That(values).DoesNotContain("FOO").Using(StringComparer.Ordinal);
await Expect.That(values).IsNotEqualTo(["BAR", "FOO"]).InAnyOrder().Using(StringComparer.Ordinal);
```

Whenever the comparer of the set decides, the expectation names it, e.g.
`contains "BAR" using the subject's StringComparer.OrdinalIgnoreCase at least once`. For an untyped `IEnumerable`, only a
set of the expected item type is recognised, e.g. a `HashSet<string>` for an expected string.

## Tolerance

The expectations that compare items with an expected value accept a tolerance for `double`, `float`, `decimal`,
`DateTime`, `DateTimeOffset` and `TimeSpan` items, and on .NET 8 or later also for `DateOnly`, whose tolerance must be
a whole number of days, and `TimeOnly`, whose items are compared on the clock face, so that `23:59` and `00:01` are two
minutes apart:

```csharp
IEnumerable<double> values = [1.01, 2.02, 3.04];

await Expect.That(values).IsEqualTo([3.0, 2.0, 1.0]).Within(0.1).InAnyOrder();
await Expect.That(values).Contains(2.0).Within(0.1);
await Expect.That(values).Contains([2.0, 3.0]).Within(0.1);
await Expect.That(values).IsContainedIn([1.0, 2.0, 3.0, 4.0]).Within(0.1);
await Expect.That(values).StartsWith(1.0, 2.0).Within(0.1);
await Expect.That(values).EndsWith(2.0, 3.0).Within(0.1);
await Expect.That(values).HasItem(2.0).Within(0.1).AtIndex(1);
await Expect.That([2.04, 2.02, 2.01]).All().AreEqualTo(2.0).Within(0.1);
```

A tolerance takes precedence over the comparer of a set.
