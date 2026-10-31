---
description: Overview of the expectations for collections such as arrays, lists, sets and any other `IEnumerable<T>`.
---

# Collections

Describes the possible expectations for collections, e.g. arrays, lists, sets or any other `IEnumerable<T>`.

| Page                                         | Expectations                                                                        |
|----------------------------------------------|-------------------------------------------------------------------------------------|
| [Equality and containment](./01-equality.md) | `IsEqualTo`, `Contains`, `IsContainedIn`, `StartsWith`, `EndsWith`                  |
| [Items](./02-items.md)                       | `HasCount`, `IsEmpty`, quantifiers like `All()` or `None()`, `HasSingle`, `HasItem` |
| [Sort order](./03-order.md)                  | `IsInAscendingOrder`, `IsInDescendingOrder`                                         |
| [Dictionaries](./04-dictionaries.md)         | `ContainsKey`, `ContainsValue`, `Keys`, `Values` and dictionary equality            |

In the overview tables of these pages, "negated comparison" marks a `Has…` expectation without a negated twin, such as
`HasCount()`: negate the comparison instead, e.g. `HasCount().NotEqualTo(3)`.

:::info[.NET 8 or later: `IAsyncEnumerable<T>`]
On .NET 8 or later, every collection expectation, except the ones for [dictionaries](./04-dictionaries.md), works the
same way for an `IAsyncEnumerable<T>`. The expectation enumerates it asynchronously and passes its
[cancellation token](../02-how-it-works/06-time-and-cancellation.md#outcome).
:::

<details>
<summary>CS0121 or an unexpected overload with C# 12 or older</summary>

Some overloads rely on `[OverloadResolutionPriority]` to bind as described here, e.g. so that a `string` is expected
as a single item and not as a sequence of characters, or that a `params` list or a collection expression `[…]` is the
expected collection. The attribute only takes effect with C# 13 or later, which is the default only for .NET 9 and
later. With an older language version, such a call can bind to a different overload or fail with CS0121, so set
`<LangVersion>` to `13` or `latest` in a project that targets an older framework.

</details>

## Choosing an expectation

Several expectations verify that some item matches. They differ in what you can add to them:

| Expectation                                               | Use it when                                                                                     |
|-----------------------------------------------------------|-------------------------------------------------------------------------------------------------|
| [`Contains(predicate)`](./01-equality.md#predicate)       | a predicate is enough, optionally with how often it matches, e.g. `.AtLeast(2)`                 |
| [`Any().Satisfy(…)`](./02-items.md#condition)             | the same with a quantifier, which also offers `All()` and `None()`                              |
| [`Any().ComplyWith(…)`](./02-items.md#nested-expectation) | the item needs an expectation instead of a predicate, e.g. `IsEquivalentTo` or a string option  |
| [`HasItem`, `HasItemThat`](./02-items.md#item-at-index)   | the position matters (`AtIndex(…)`); a failure then names the item that was found at the index |

A failure message shows the predicate as written, while for a nested expectation it describes the expectation, e.g.
`is greater than 2 for at least one item`.

## Supported collection types

### Spans

:::info[.NET 8 or later]
Spans can only be passed to `Expect.That` on .NET 8 or later. On older targets, verify a property of the span, e.g.
its `Length`, [synchronously](../02-how-it-works/00-index.md#when-you-cannot-await).
:::

You can pass a `Span<T>` or a `ReadOnlySpan<T>` directly to `Expect.That`. The items are copied into a
`SpanWrapper<T>`, which implements `ICollection<T>`, so all collection expectations are available:

```csharp
await That("Help!".AsSpan()).HasCount(5);
await That("Help!".AsSpan()).Contains('!');
```

A span can't be kept across an `await`, so create it in the statement of the expectation.

### Immutable arrays

:::info[.NET 8 or later]
Before .NET 8, only `IsEmpty` and `IsNotEmpty` are available for an `ImmutableArray<T>`. Call `.AsEnumerable()` on the
array to use the other collection expectations.
:::

A default `ImmutableArray<T>` is treated like a `null` collection, as the subject and as the expected collection. To
tell the two apart, verify its `IsDefault` property.
