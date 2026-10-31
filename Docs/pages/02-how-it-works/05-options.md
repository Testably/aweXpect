---
description: The options that many expectations share, such as a reason, string options, comparers, equivalency, tolerances and timeouts.
---

# Options

Many expectations share the same options. An option is appended to the expectation it configures. These rules apply
to all options:

- Options can be chained in any order.
- A repeated option replaces the earlier one, e.g. `IgnoringCase().IgnoringCase(false)` compares case-sensitively.
- An option that switches something on, such as `IgnoringCase()`, takes an optional `bool`; with `false` it behaves as
  if it was not specified.
- Options that set the same thing in different ways, e.g. `AsPrefix().AsSuffix()`, throw.

The reference pages describe the details:

| Option                                                                | Configures                                      | Details                                                             |
|-----------------------------------------------------------------------|-------------------------------------------------|---------------------------------------------------------------------|
| [`Because(reason)`](#because)                                         | the reason in the failure message               | [Anatomy](./00-index.md#anatomy-of-an-expectation)                     |
| [`IgnoringCase()`, `IgnoringLeadingWhiteSpace()`, …](#string-options) | how strings are compared                        | [String](../04-values/03-string.md#string-options)                  |
| [`Using(comparer)`](#comparer)                                        | a custom equality comparer                      | [Object](../04-values/12-object.md#custom-comparer)                 |
| [`Equivalent()`](#equivalency)                                        | a comparison by equivalency instead of equality | [Equivalency](../04-values/13-equivalency.md)                       |
| [`Within(tolerance)`](#tolerance)                                     | a tolerance for numbers and times               | [Number](../04-values/02-number.md#tolerance)                       |
| [`InAnyOrder()`, `IgnoringDuplicates()`, …](#collection-options)      | how collections are compared                    | [Collections](../05-collections/01-equality.md)                     |
| `Once()`, `Never()`, `AtLeast(2)`, `AtLeast().Twice()`, …             | how often an item must occur                    | [Contained items](../05-collections/01-equality.md#contained-items) |
| [`Within(timeout)`, `WithTimeout(…)`, `WithCancellation(…)`](#time)   | how long an expectation may take or wait        | [Time and cancellation](./06-time-and-cancellation.md)              |

## Because

`Because(reason)` adds a reason to the failure message, after the expectation it belongs to:

```csharp
string title = "Let It Be";

await That(title).IsEqualTo("Let It Be").Because("it is the title of the last album");
```

How `Because` applies to `Expect.ThatAll` and `Expect.ThatAny` is described in
[combining expectations](./03-combining.md#on-different-subjects).

## String options

The expectations that compare strings, e.g. `IsEqualTo`, `StartsWith` or `Contains`, but also the items of a
collection of strings or the message of an exception, can ignore the casing, the newline style, the indentation or
leading and trailing whitespace:

```csharp
string title = " Abbey Road ";

await That(title).IsEqualTo("abbey road")
  .IgnoringCase()
  .IgnoringLeadingWhiteSpace()
  .IgnoringTrailingWhiteSpace();
await That(["Let It Be", "Help!"]).Contains("LET IT BE").IgnoringCase();
```

`IsEqualTo` can also match a wildcard or regex pattern with `AsWildcard()` or `AsRegex()`, see
[match types](../04-values/03-string.md#match-types).

## Comparer

`Using(comparer)` replaces the default equality with a custom `IEqualityComparer<T>`:

```csharp
string title = "Abbey Road";

await That(title).IsEqualTo("ABBEY ROAD").Using(StringComparer.OrdinalIgnoreCase);
await That(new Album("Abbey Road")).IsEqualTo(new Album("Abbey Road")).Using(new AlbumComparer());
```

## Equivalency

`Equivalent()` switches an equality expectation to [equivalency](../04-values/13-equivalency.md), which compares the
public members recursively instead of calling `Equals`:

```csharp
Album[] albums = [new("Abbey Road"), new("Let It Be")];

await That(new Album("Abbey Road")).IsEqualTo(new Album("Abbey Road")).Equivalent();
await That(albums).Contains(new Album("Let It Be")).Equivalent();
```

## Tolerance

`Within(tolerance)` accepts a value that differs from the expected one by at most the tolerance. It is available for
numbers, `TimeSpan`, `DateTime`, `DateTimeOffset`, `DateOnly` and `TimeOnly` (the latter two
[on .NET 8 or later](../03-setup/02-target-frameworks.md)), and for the items of a collection of
them:

```csharp
double duration = 4.02;
DateTime releaseDate = new DateTime(1970, 5, 8, 12, 0, 0);

await That(duration).IsEqualTo(4.0).Within(0.05);
await That(releaseDate).IsEqualTo(new DateTime(1970, 5, 8)).Within(TimeSpan.FromDays(1));
await That([3.98, 4.01]).All().AreEqualTo(4.0).Within(0.05);
```

### Default tolerance

On Windows the `DateTime` resolution is [about 10 to 15 milliseconds](https://stackoverflow.com/q/3140826/4003370), so
comparing them as exact values might result in brittle tests. Therefore, it is possible to specify a default tolerance
that is used when a `DateTime`, `DateTimeOffset`, `DateOnly`, `TimeOnly` or `TimeSpan` subject is compared directly
(e.g. with `IsEqualTo`, `IsOneOf`, `IsBefore` or `IsBetween`) and no explicit tolerance is given:

```csharp
using aweXpect.Chronology; // from the aweXpect.Chronology package
using aweXpect.Customization;

using IDisposable lifetime = Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(15.Milliseconds());
```

It also applies to the items of a collection of these types. An explicit `Within` always replaces the default
tolerance. The applied default tolerance is part of the failure message, for example
`is equal to 2024-12-24T13:15:00.0000000 ± 0:00.015`, unless it is zero. For a `DateOnly`, see
[tolerance](../04-values/11-date-time-only.md#tolerance).

<details>
<summary>Where the default tolerance applies</summary>

It applies to the items of a collection of `DateTime`, `DateTimeOffset`, `DateOnly`, `TimeOnly` or `TimeSpan` values
(or their nullable counterparts) in every expectation that compares them with expected items, e.g. `IsEqualTo`,
`Contains`, `IsContainedIn`, `StartsWith`, `EndsWith`, `HasItem` or `All().AreEqualTo`, and in their negations. The same
holds for the values of a dictionary in `Contains(key, value)`, `ContainsValue`, `ContainsValues` and `IsEqualTo`, and in
their negations.

The default tolerance is not used for:
- property verifications like `HasOffset()`
- collection expectations that don't compare items with expected items, like `IsInAscendingOrder` or `AreUnique`
- the keys of a dictionary, which are looked up through its key comparer, e.g. in `ContainsKey`
- values and members compared by `IsEquivalentTo`
- values compared as `object`

</details>

## Collection options

The expectations that compare a collection with another collection take the following options:

| Option                        | Effect                                                                     |
|-------------------------------|----------------------------------------------------------------------------|
| `InAnyOrder()`                | ignores the order of the items                                             |
| `IgnoringDuplicates()`        | ignores repeated items                                                     |
| `IgnoringInterspersedItems()` | allows other items between the expected ones (`Contains`, `IsContainedIn`) |
| `Properly()`                  | requires a proper subset or superset (`Contains`, `IsContainedIn`)         |

```csharp
string[] songs = ["Two of Us", "Dig a Pony", "Let It Be"];

await That(songs).IsEqualTo(["Let It Be", "Two of Us", "Dig a Pony"]).InAnyOrder();
await That(songs).Contains(["Two of Us", "Let It Be"]).IgnoringInterspersedItems();
await That(songs).IsContainedIn(["Two of Us", "Dig a Pony", "Let It Be", "Get Back"]).Properly();
```

## Time

`Within(timeout)` limits how long an expectation waits, e.g. for a condition, an event or a callback, and
`WithTimeout(…)` and `WithCancellation(…)` limit how long any expectation may take:

```csharp
using aweXpect.Chronology; // from the aweXpect.Chronology package

Track track = new();
// Start a background task that plays the track

await That(track).Satisfies(x => x.IsPlayed).Within(2.Seconds())
  .WithTimeout(5.Seconds());
```

See [time and cancellation](./06-time-and-cancellation.md) for the details.
