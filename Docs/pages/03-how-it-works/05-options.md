# Options

Many expectations share the same options. An option is appended to the expectation it configures, and the reference
pages describe the details:

| Option                                                                | Configures                                      | Details                                                |
|-----------------------------------------------------------------------|-------------------------------------------------|--------------------------------------------------------|
| [`Because(reason)`](#because)                                         | the reason in the failure message               | [Anatomy](./index.md)                                  |
| [`IgnoringCase()`, `IgnoringLeadingWhiteSpace()`, …](#string-options) | how strings are compared                        | [String](../04-values/03-string.md#string-options)     |
| [`Using(comparer)`](#comparer)                                        | a custom equality comparer                      | [Object](../04-values/12-object.md#custom-comparer)    |
| [`Equivalent()`](#equivalency)                                        | a comparison by equivalency instead of equality | [Equivalency](../04-values/13-equivalency.md)          |
| [`Within(tolerance)`](#tolerance)                                     | a tolerance for numbers and times               | [Number](../04-values/02-number.md#tolerance)          |
| [`InAnyOrder()`, `IgnoringDuplicates()`, …](#collection-options)      | how collections are compared                    | [Collections](../05-collections/01-equality.md)        |
| [`Within(timeout)`, `WithTimeout(…)`, `WithCancellation(…)`](#time)   | how long an expectation may take or wait        | [Time and cancellation](./06-time-and-cancellation.md) |

## Because

`Because(reason)` adds a reason to the failure message, after the expectation it belongs to:

```csharp
string title = "Let It Be";

await Expect.That(title).IsEqualTo("Let It Be").Because("it is the title of the last album");
```

## String options

The expectations that compare strings, e.g. `IsEqualTo`, `StartsWith` or `Contains`, but also the items of a
collection of strings or the message of an exception, can ignore the casing, the newline style, the indentation or
leading and trailing whitespace:

```csharp
string title = " Abbey Road ";

await Expect.That(title).IsEqualTo("abbey road").IgnoringCase().IgnoringLeadingWhiteSpace().IgnoringTrailingWhiteSpace();
await Expect.That(["Let It Be", "Help!"]).Contains("LET IT BE").IgnoringCase();
```

`IsEqualTo` can also match a wildcard or regex pattern with `AsWildcard()` or `AsRegex()`, see
[match types](../04-values/03-string.md#match-types).

## Comparer

`Using(comparer)` replaces the default equality with a custom `IEqualityComparer<T>`:

```csharp
string title = "Abbey Road";

await Expect.That(title).IsEqualTo("ABBEY ROAD").Using(StringComparer.OrdinalIgnoreCase);
await Expect.That(new Album("Abbey Road")).IsEqualTo(new Album("Abbey Road")).Using(new AlbumComparer());
```

## Equivalency

`Equivalent()` switches an equality expectation to [equivalency](../04-values/13-equivalency.md), which compares the
public members recursively instead of calling `Equals`:

```csharp
Album[] albums = [new("Abbey Road"), new("Let It Be")];

await Expect.That(new Album("Abbey Road")).IsEqualTo(new Album("Abbey Road")).Equivalent();
await Expect.That(albums).Contains(new Album("Let It Be")).Equivalent();
```

## Tolerance

`Within(tolerance)` accepts a value that differs from the expected one by at most the tolerance. It is available for
numbers, `TimeSpan`, `DateTime`, `DateTimeOffset`, `DateOnly` and `TimeOnly` (the latter two
[on .NET 8 or later](../02-getting-started.md#target-frameworks)), and for the items of a collection of
them:

```csharp
double duration = 4.02;
DateTime releaseDate = new DateTime(1970, 5, 8, 12, 0, 0);

await Expect.That(duration).IsEqualTo(4.0).Within(0.05);
await Expect.That(releaseDate).IsEqualTo(new DateTime(1970, 5, 8)).Within(TimeSpan.FromDays(1));
await Expect.That([3.98, 4.01]).All().AreEqualTo(4.0).Within(0.05);
```

For the time types, a [default tolerance](../04-values/10-datetime-offset.md#default-tolerance) can be configured.

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

await Expect.That(songs).IsEqualTo(["Let It Be", "Two of Us", "Dig a Pony"]).InAnyOrder();
await Expect.That(songs).Contains(["Two of Us", "Let It Be"]).IgnoringInterspersedItems();
await Expect.That(songs).IsContainedIn(["Two of Us", "Dig a Pony", "Let It Be", "Get Back"]).Properly();
```

## Time

`Within(timeout)` limits how long an expectation waits, e.g. for a condition, an event or a callback, and
`WithTimeout(…)` and `WithCancellation(…)` limit how long any expectation may take:

```csharp
using aweXpect.Chronology; // from the aweXpect.Chronology package

Track track = new();
// Start a background task that plays the track

await Expect.That(track).Satisfies(x => x.IsPlayed).Within(2.Seconds())
  .WithTimeout(5.Seconds());
```

See [time and cancellation](./06-time-and-cancellation.md) for the details.
