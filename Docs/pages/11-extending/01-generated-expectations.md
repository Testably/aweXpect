---
description: "Generate an expectation, its negation and its collection variants from a static method that checks a condition."
---

# Generated expectations

An expectation that only checks a condition on its subject doesn't need a constraint class of its own: mark a
`static` method that decides the condition with `[GenerateExpectation]` from `aweXpect.Core.Extending`, and the source
generator in the `aweXpect.Core` package writes the extension methods, the constraint and its failure messages. It
runs in every project that references `aweXpect.Core`, directly or through `aweXpect`, so an extension package can use it
as well as a test project.

<details>
<summary>Namespaces used on this page</summary>

```csharp
using System.Diagnostics.CodeAnalysis;
using aweXpect.Core;
using aweXpect.Core.Extending;
```

</details>

```csharp no-compile
public static partial class ThatTrack
{
    [GenerateExpectation("Is{Not}LongerThan", "is {not} longer than {minimum}")]
    private static bool IsLongerThan(Track track, TimeSpan minimum)
        => track.Duration > minimum;
}
```

```csharp no-compile
Track track = new("Love Me Do", new TimeSpan(0, 2, 22));

await That(track).IsLongerThan(TimeSpan.FromMinutes(2));
await That(track).IsNotLongerThan(TimeSpan.FromMinutes(3));
```

## The method

- The method is `static` and returns `bool`, `Task<bool>` or `ValueTask<bool>`, or a `PredicateResult` instead of the
  `bool` when it [cannot always answer](#questions-that-cannot-be-answered). Its first parameter is the subject, the
  other parameters become the parameters of the expectation.
- A `CancellationToken` parameter marked with `[EvaluationCancellationToken]` receives the token of the evaluation
  and is no parameter of the expectation. It can be at any position after the subject, e.g. before a `params`
  parameter. A `CancellationToken` parameter that is not marked is a parameter of the expectation like any other.

  ```csharp no-compile
  [GenerateExpectation("Is{Not}OneOf", "is {not} one of {titles}")]
  private static async Task<bool> IsOneOf(Track track,
      [EvaluationCancellationToken] CancellationToken cancellationToken, params string[] titles)
      => titles.Contains(await LookUpTitle(track, cancellationToken));
  ```
- It is declared in a top-level, non-generic, `static partial` class that is not `file`-local, which receives the
  generated extension methods. The method itself can be `private`.

<details>
<summary>Accessibility of the generated methods</summary>

The generated extension methods are `public`, unless the class, the subject or a parameter type is `internal`: then
they are `internal` as well.

</details>

## Name and texts

The `name` is the name of the expectation. A `{Not}` in it, or a `NegatedName`, also generates the negated
expectation, e.g. `"Is{Not}LongerThan"` generates `IsLongerThan` and `IsNotLongerThan`.

The `expectationText` follows the [message conventions](./03-message-conventions.md). `{not}` is removed from the
expectation and becomes "not" in its negation. Any other placeholder, e.g. `{minimum}`, names a parameter, whose
formatted value replaces it.

When "Not" and "not" in place of `{Not}` and `{not}` don't read well, give the negated name in `NegatedName` and the
negated text in `NegatedExpectationText`, with the same placeholders. Every expectation can be negated with
`DoesNotComplyWith`, even one without a negated name, so the text needs either `{not}` or a `NegatedExpectationText`:

```csharp no-compile
[GenerateExpectation("HasTitle", "has title {title}",
    NegatedName = "DoesNotHaveTitle", NegatedExpectationText = "does not have title {title}")]
private static bool HasTitle(Track track, string title)
    => track.Title == title;
```

The negated expectation names its parameters like the method. To give a parameter another name there, e.g.
`unexpected` instead of `expected`, mark it with `[NegatedParameterName]`. The name applies to the signature, the
argument check and the derived XML documentation of the negated expectation, while the placeholders of both texts keep
the name of the method's parameter:

```csharp no-compile
[GenerateExpectation("Is{Not}Named", "is {not} named {expected}")]
private static bool IsNamed(Track track, [NegatedParameterName("unexpected")] string expected)
    => track.Title == expected;
```

This generates `IsNamed(string expected)` and `IsNotNamed(string unexpected)`.

The result after "but" shows the formatted subject. To describe it differently, name a `static` method that returns the
text after the subject in `Result`. It takes the subject, optionally followed by the other parameters:

```csharp no-compile
[GenerateExpectation("Is{Not}LongerThan", "is {not} longer than {minimum}",
    Result = nameof(DescribeDuration))]
private static bool IsLongerThan(Track track, TimeSpan minimum)
    => track.Duration > minimum;

private static string DescribeDuration(Track track)
    => $"was {track.Duration:m\\:ss} long";
```

```text title="Failure message"
Expected that track
is longer than 3:00,
but it was 2:22 long
```

To describe the subject itself, the method can end with a `string` and an `ExpectationGrammars` parameter. It then
receives how the subject is referred to, e.g. "it", and returns the whole text after "but":

```csharp no-compile
private static string DescribeDuration(Track track, string it, ExpectationGrammars grammars)
    => $"{it}{grammars.SubjectVerb(it, " was ", " were ")}{track.Duration:m\\:ss} long";
```

The XML documentation is derived from the expectation text. `Summary`, `NegatedSummary`, `Remarks` and
`NegatedRemarks` replace or extend it with XML documentation of your own.

## `null` subjects

The generated expectations follow the [rule for `null` subjects](./02-constraints-and-results.md#null-subjects), and
the type of the subject parameter decides how:

| Subject parameter                                    | Generated expectations                                                              |
|------------------------------------------------------|-------------------------------------------------------------------------------------|
| a reference type, e.g. `Track`                       | on `IThat<Track?>`; `null` fails the expectation and its negation                   |
| a value type, e.g. `TimeSpan`                        | on `IThat<TimeSpan>` and on `IThat<TimeSpan?>`, where `null` fails both             |
| a nullable type, e.g. `Track?` or `TimeSpan?`        | on `IThat<Track?>` or `IThat<TimeSpan?>`; the method decides about `null`           |
| a nullable type with `[DisallowNull]`                | on the nullable type; `null` fails the expectation and its negation                 |

Set `NullableOverload = false` to generate no overload on the nullable value type, e.g. when a separate class of yours
already holds the expectations on it.

An expectation that fails for a `null` subject is marked with [`[GuaranteesNotNull]`](./02-constraints-and-results.md#nullability-warnings).
When the method decides about `null`, `[NotNullWhen]` on its subject parameter tells which of the two expectations
guarantees a subject that is not `null`, like on `string.IsNullOrEmpty`:

```csharp no-compile
[GenerateExpectation("Is{Not}Untitled", "is {not} untitled")]
private static bool IsUntitled([NotNullWhen(false)] Track? track)
    => track is null || track.Title.Length == 0;
```

Here `IsNotUntitled()` fails for a `null` subject, so the subject cannot be `null` after it.

## Questions that cannot be answered

When the method cannot decide for some subjects, e.g. a track without a duration, it returns a `PredicateResult`. A
`bool` converts implicitly, so only that case needs `PredicateResult.FailureBothWays` with the reason, which follows
"it" in the failure message:

```csharp no-compile
[GenerateExpectation("Is{Not}LongerThan", "is {not} longer than {minimum}")]
private static PredicateResult IsLongerThan(Track track, TimeSpan minimum)
    => track.Duration <= TimeSpan.Zero
        ? PredicateResult.FailureBothWays("had no duration")
        : track.Duration > minimum;
```

```text title="Failure message"
Expected that track
is not longer than 3:00,
but it had no duration
```

The expectation and its negation fail alike, as negating a question that cannot be answered does not make it true.
The `Result` method still describes a subject that the method decided about.

## Collections

`[GenerateCollectionExpectation]` generates an expectation on the items of a collection from the same kind of method,
e.g. `AreLongerThan` for `That(tracks).All().AreLongerThan(…)`. It takes the name, the expectation text on a single
item and its plural:

```csharp no-compile
[GenerateCollectionExpectation("AreLongerThan", "is longer than {minimum}", "are longer than {minimum}")]
private static bool IsLongerThan(Track track, TimeSpan minimum)
    => track.Duration > minimum;
```

Together with `[GenerateExpectation]`, the text on a single item can be left out, as it is the expectation text of
`[GenerateExpectation]` without `{not}`:

```csharp no-compile
[GenerateExpectation("Is{Not}LongerThan", "is {not} longer than {minimum}")]
[GenerateCollectionExpectation("AreLongerThan", "are longer than {minimum}")]
private static bool IsLongerThan(Track track, TimeSpan minimum)
    => track.Duration > minimum;
```

```csharp no-compile
await That(tracks).All().AreLongerThan(TimeSpan.FromMinutes(2));
await That(tracks).AtLeast(2).AreLongerThan(TimeSpan.FromMinutes(2));
await That(tracks).None().AreLongerThan(TimeSpan.FromMinutes(5));
```

- The method follows the rules above, and its first parameter is the item.
- The expectation text describes a single item, e.g. "is longer than … for all items". The plural text is used
  where the quantifier is the subject, e.g. "has values of which at least 2 are longer than …". The quantifier
  carries the negation.
- The result counts the items, e.g. "but only 1 of 3 were", with "did" instead of "were" when the text does not start
  with a form of "be". The failure message lists the matching or not matching items and the collection.
- A `null` item does not match, unless the item parameter is nullable: then the method decides. `[DisallowNull]` and
  `NullableOverload` work like for a subject.
- `NullItemFailsBothWays = true` makes a `null` item fail the expectation and its negation instead, like a `null`
  subject, e.g. `but for the item at index 1, it was <null>`. Use it when the question cannot be answered for a
  `null` item, instead of a nullable item parameter that returns `PredicateResult.FailureBothWays` for it.
- An item that the method [cannot answer](#questions-that-cannot-be-answered) fails the expectation and its negation,
  e.g. "but for the item at index 1, it had no duration", unless the items before already determine the outcome.
- `Summary` and `Remarks` replace or extend the XML documentation, which is derived from the plural text.

<details>
<summary>Which collections and item types are extended</summary>

- The expectation extends the items of every kind of collection that can hold the item: an `IEnumerable<T>`, an
  `ImmutableArray<T>` and, on .NET 8 or later, an `IAsyncEnumerable<T>`. The items of a reference type are extended
  as nullable, so that the expectation applies to a `List<Track>`, a `List<Track?>` and a collection of a type
  derived from `Track`.
- The type of the item can be the only type parameter of the method, when a constraint restricts it to reference
  types and no other parameter uses it, e.g. `IsLive<TRecording>(TRecording recording) where TRecording : Recording`.
  The expectations are then generic as well and keep the type of the items, e.g. for a collection of albums.

</details>

### On the collection

`CollectionOverload = true` also generates the expectation as an overload directly on the collection, which all items
have to meet, and a `NegatedName` generates an expectation directly on the collection, which no item may meet. The two
are independent: `CollectionOverload` only adds the positive overload, so the negated one on the collection needs a
`NegatedName`, while the quantified `None()` is always available.

```csharp no-compile
[GenerateCollectionExpectation("AreLongerThan", "is longer than {minimum}", "are longer than {minimum}",
    CollectionOverload = true, NegatedName = "AreNotLongerThan")]
private static bool IsLongerThan(Track track, TimeSpan minimum)
    => track.Duration > minimum;
```

```csharp no-compile
await That(tracks).AreLongerThan(TimeSpan.FromMinutes(2));
await That(tracks).AreNotLongerThan(TimeSpan.FromMinutes(5));
```

- The quantifier is the subject of the text, e.g. "all are longer than …" and "none are longer than …", whose
  negations read "not all are longer than …" and "at least one is longer than …". The result counts the items like
  above, e.g. "but only 2 of 3 were".
- The collection is read to its end, so that the failure message lists every item that does not meet the expectation,
  or that meets it for the `NegatedName`. Use a quantifier like `All()` instead on a collection that never ends.

<details>
<summary>Extended collections, XML documentation and parameter names</summary>

- They extend an `IEnumerable<T>` and, on .NET 8 or later, an `IAsyncEnumerable<T>` of the item, and a `null`
  collection fails them and their negations.
- The summary of the XML documentation is derived from the plural text, e.g. "Verifies that all items are longer
  than …" and "Verifies that no items are longer than …"; the `Remarks` apply to them as well.
- A parameter marked with `[NegatedParameterName]` has that name in the `NegatedName` expectation, also in a
  `<paramref>` of the `Remarks`, and keeps its name in all other expectations.

</details>

### String options

A `StringEqualityOptions` parameter marked with `[ExpectationOptions]` gives the expectation the string options, e.g.
`AsPrefix()`, `IgnoringCase()` or `AsRegex()`, which compare with the `string` parameter of the method:

```csharp no-compile
[GenerateCollectionExpectation("HaveTitle", "has title {expected}", "have title {expected}",
    CollectionOverload = true, NegatedName = "DoNotHaveTitle")]
private static ValueTask<bool> HasTitle(Track track, [NegatedParameterName("unexpected")] string expected,
    [ExpectationOptions] StringEqualityOptions options)
    => options.AreConsideredEqual(track.Title, expected, nameof(expected));
```

```csharp no-compile
await That(tracks).HaveTitle("Intro").AsPrefix().IgnoringCase();
await That(tracks).AtLeast(2).HaveTitle("*live*").AsWildcard();
```

The options are not a parameter of the expectation: every generated form returns a `StringEqualityTypeResult` with
the option methods and passes the chosen options to the method. The options also describe the compared value in the
failure message, e.g. "has title starting with "Intro" ignoring case for at least 2 items".

<details>
<summary>When the options are prepared and the expected value is validated</summary>

The generated constraints override `OnEvaluating` of `GeneratedCollectionConstraint<TCollection, TItem>` and
`GeneratedAsyncCollectionConstraint<TItem>`, which is called when an evaluation starts, before the collection is read.
There they get the options of the evaluation and validate the expected value, so that an unusable value, e.g. an
invalid regex pattern, is rejected also for an empty collection and names the parameter of the expectation, e.g.
`unexpected` for the `NegatedName`.

</details>

### Formatting the items

The failure message lists the items with their formatter. To show something else for them, e.g. a wrapper that a
[formatter of your own](./04-formatting.md#value-formatters) formats, name a `static` method in the same class in
`FormattedItem`. It takes the item, which can be `null` for a reference type, and its result stands for the item in
the listed items and in the collection:

```csharp no-compile
[GenerateCollectionExpectation("AreLongerThan", "is longer than {minimum}", "are longer than {minimum}",
    FormattedItem = nameof(ToTitle))]
private static bool IsLongerThan(Track track, TimeSpan minimum)
    => track.Duration > minimum;

private static string? ToTitle(Track? track)
    => track?.Title;
```

## Diagnostics

A method that the generator cannot turn into an expectation is reported as an error, see the
[expectation generator](../08-analyzers.md#expectation-generator).
