---
description: Use the equality, custom match types and property comparisons of aweXpect, so that callers choose the comparison with the built-in options.
---

# Comparisons

An expectation that compares the subject with an expected value can use the comparisons of aweXpect, so that the
caller chooses the comparison with the same options as for the built-in expectations, e.g. `Using(comparer)`,
`Equivalent()` or `IgnoringCase()`.

<details>
<summary>Namespaces used on this page</summary>

```csharp
using aweXpect.Core;
using aweXpect.Formatting;
using aweXpect.Options;
using aweXpect.Results;
using static aweXpect.Formatting.Format;
```

</details>

## Equality expectations

An expectation that compares the subject for equality, e.g. an `IsEqualTo` for your own type, adds the equality
constraint of `IsEqualTo` with `AddEqualityConstraint` from `aweXpect.Core.Extending`. Return an `ObjectEqualityResult`
that shares the `OptionsHolder<ObjectMatchType<T>>`, so that the caller can choose the comparison, e.g. with `Using(comparer)` or
a [custom match type](#custom-match-types), and the expectation and the failure read like those of `IsEqualTo`:

```csharp
public static ObjectEqualityResult<Track?, IThat<Track?>, Track?> IsEqualTo(
    this IThat<Track?> subject, Track? expected)
{
    OptionsHolder<ObjectMatchType<Track?>> options = new(ObjectMatchType<Track?>.Default);
    return new ObjectEqualityResult<Track?, IThat<Track?>, Track?>(
        subject.Get().ExpectationBuilder.AddEqualityConstraint(expected, options),
        subject,
        options);
}
```

For `IsNotEqualTo`, pass `isNegated: true`. A `null` subject is never equal to an expected value that is not `null`,
whatever the comparison. To show the code of the caller instead of the formatted value, e.g. "is equivalent to
expected" like `IsEquivalentTo`, pass the value of a `[CallerArgumentExpression]` parameter as `expectedExpression`.

<details>
<summary>How the expected expression is shown</summary>

Its common leading whitespace is removed, and an expected `null` is still shown as `<null>`.

</details>

## Custom match types

`IsEqualTo` compares two values with `Equals`, unless the caller chooses another comparison, e.g. with `.Equivalent()`.
You can offer a comparison of your own: derive from `ObjectMatchType<TElement>`, pass the name of your option to its
base constructor and set it on the options of the result with `SetMatchType(matchType)`, which reads that name. For
example, two tracks with the same title can count as equal:

```csharp
public static TSelf ByTitle<TSelf, TElement>(this IObjectEqualityResult<TSelf, TElement> result)
    where TSelf : IObjectEqualityResult<TSelf, TElement>
{
    result.Options.SetMatchType(new ByTitleMatchType<TElement>());
    return (TSelf)result;
}

private sealed class ByTitleMatchType<TElement>() : ObjectMatchType<TElement>("ByTitle")
{
    private static readonly ByTitleMatcher Matcher = new();

    public override MatchPhrase Phrase { get; } = MatchPhrase.Being("titled like").WithName(" by title");

    public override ObjectMatcher<TElement> ForEvaluation(MatchEnvironment environment) => Matcher;

    private sealed class ByTitleMatcher : ObjectMatcher<TElement, Track>
    {
        public override ValueTask<MatchResult> Match(TElement actual, Track expected)
            => new(MatchResult.From(actual is Track track && track.Title == expected.Title));

        public override string GetFailure(string it, ExpectationGrammars grammars, TElement? actual,
            Track expected, MatchResult result, string? indentation)
            => $"{it}{grammars.SubjectVerb(it, " was", " were")} titled {Formatter.Format((actual as Track)?.Title)}";
    }
}
```

```csharp
Track track = new("Hey Jude", new TimeSpan(0, 7, 11));

await That(track).IsEqualTo(new Track("Hey Jude", new TimeSpan(0, 7, 4))).ByTitle();
```

- `ForEvaluation` returns the matcher for one evaluation. A matcher without state can be shared, as above.
- `Match` decides whether two values match, and `GetFailure` writes the result text of a mismatch.
- `Phrase` holds the words of the comparison, from which Core writes the expectation text in every grammar, e.g. "is
  titled like …", "are not titled like …" or "has item titled like …".
- When the result text of `GetFailure` spans several lines, indent the lines after the first by the `indentation`,
  e.g. with `FormattingOptions.Indented(indentation)` for a formatted value, so that it stays aligned inside
  `Expect.ThatAll`.
- Override `ValidateExpected(expected, parameterName)` to reject an expected value that the match type cannot use,
  with an `ArgumentException` for the `parameterName` it receives, as neither the match type nor the options know the
  parameter of the expectation.
- For strings, derive from `StringMatchType` instead and set it with `SetMatchType(matchType, nameof(AsTitle))` on the
  `StringEqualityOptions`, which takes the name of your option, as a string match type carries none.
- Declare the settings that a string match type honors in `SupportedSettings`: Core rejects every other one, e.g. "IgnoringCase cannot be combined with AsTitle.". The whitespace options are supported unless you leave them out.

<details>
<summary>Matchers, results, phrases and string match types in detail</summary>

- `ForEvaluation` receives the context and the cancellation of the evaluation in the `MatchEnvironment`.
- `ObjectMatcher<TElement, TExpected>` compares with expected values of a known type, e.g. a `Track`, and an expected
  value of another type never matches. Derive from `ObjectMatcher<TElement>` instead when the type of the expected
  value varies, e.g. an anonymous object for an equivalency, and implement the generic `Match<TExpected>`.
- `Match` returns `MatchResult.Match`, `MatchResult.NoMatch`, or `MatchResult.NotComparable(reason)` for a subject that
  it cannot compare at all. The expectation and its negation then both fail, with the reason as the result. Like
  [`PredicateResult.FailureBothWays`](./01-generated-expectations.md#questions-that-cannot-be-answered), the reason
  follows "it". Inside `Whose` it follows the member name instead, and in a collection "an item", so name the value in
  the reason as well.
- A comparison that finds more than the values show, e.g. the differences of two documents, returns
  `MatchResult.NoMatchBecause(explanation)` with a `MatchExplanation` of its own, which the default `GetFailure` writes.
  `MatchWithExplanation` compares for a failure message, while `Match` only decides, e.g. for each item of a
  collection; override it when explaining costs more than deciding.
- `MatchPhrase.Being(comparison)` reads "is …", `MatchPhrase.Verb(singular, plural, participle)` has a verb of its own,
  e.g. "starts with", and `MatchPhrase.EqualTo` is the equality, which the text of a single item may leave out, e.g.
  "contains 3". `WithName` names the match type in a short description of the options, and `WithSuffix` follows the
  expected value, e.g. " using TitleComparer". For a string match type, `WithPattern(kind, noun)` marks the expected
  value as a pattern, e.g. `wildcard "Let*"` for an expected item, and `WithUnformattedExpected()` writes it as it is
  instead of quoting it, e.g. a JSON document.
- The matcher can add [contexts](./03-message-conventions.md#contexts) that explain a failure in `AppendContexts`, e.g.
  the options of the comparison. A string matcher only adds them to a failure of the string comparison, also for an
  item of a collection or a member inside `Whose`, and not to a failure of another expectation on the same subject.
  A constraint of your own that compares strings with `StringEqualityOptions` forwards these contexts from its
  `AppendContexts` with `options.AppendContexts(contexts)`, on the options of its evaluation.
- The `ForEvaluation` of a `StringMatchType` also receives the `StringMatchSettings`, i.e. whether the casing is ignored
  and the comparer of the caller, and its `StringMatcher` compares the values after the whitespace options are applied.
  Core appends these options to the expectation text, e.g. "starts with "Let it" ignoring case".
- A string match type can also tell Core where its expected value lines up with the subject (`Anchors`, e.g. a prefix
  only at the start), count the occurrences of the expected value for a quantified `Contains` (`CountOccurrences` of
  the matcher), and whether a failure lists the differing lines or hints at a different newline style.

Through its name, the same option again replaces the earlier match type, and combining it with another comparison
throws, e.g. "ByTitle cannot be combined with Equivalent.". Core rejects an unsupported setting of a string match type
at the call that specifies the conflict, in either order. The `ValidateExpected` of a match type is called for every
subject, also when nothing is compared with the expected value, e.g. for an empty collection.

The items of a collection are compared one after the other with the matcher of the evaluation, so a matcher may keep
state across them, e.g. which options matched nothing in any comparison, and report it in `AppendContexts`. Keep what a
single comparison found in its `MatchExplanation` instead, as a collection compares many items before it explains one.

An `EquivalencyMatchType<T>` compares like `IsEquivalentTo`. A constraint compares with the matcher that
`ForEvaluation(new MatchEnvironment(context, cancellationToken))` returns for its evaluation, e.g. with
`CompareWithExplanation(actual, expected)`, so that the timeout and the cancellation of the evaluation also end the
expectations of an `It.Is…` in the expected object. It also appends its contexts from this matcher, as only it knows
what the comparisons found, e.g. a member to ignore that matched no member.

</details>

## Comparing with the options

In the constraint, compare with the options of the evaluation:

- Compare with the options that `ForEvaluation(context, cancellationToken)` returns for its evaluation, so that the
  match type can use the evaluation and keep what its comparisons found.
- `MatchWithExplanation(actual, expected, nameof(expected))` returns a `MatchResult`, which the constraint keeps for its
  failure message, as the options keep nothing of a comparison.
- The string options carry no parameter name: the members that can reject the expected value, e.g. an invalid regex
  pattern, take the name of the parameter that the `ArgumentException` reports, e.g.
  `ValidateExpected(expected, nameof(expected))`, `MatchWithExplanation(…)`,
  `AreConsideredEqual(actual, expected, nameof(expected))` or `CountOccurrences(…)`.
- The `StringEqualityOptions` also write the texts for every match type: `GetExpectation(expected, grammars)` the
  expectation and `GetExtendedFailure(it, grammars, actual, expected, result)` the result, e.g. "it was "Yesterday",
  which differs …".
- When the string is a member of the subject, `GetExtendedMemberFailure(it, "title", grammars, actual, expected,
  result)` names it like the built-in expectations do, e.g. "it had title "Yesterday", which differs …".

For the index options, call `MatchesIndex(index, count)` on the `CollectionIndexOptions` for each item, with the number
of items or `null` while it is not known yet. Only `true` means that the item is at the expected index: `null` means
that a later item could still be, and `false` that no later item can be, so you can stop there.

## Property comparisons

An expectation on a property of the subject, e.g. the duration of a track, can return a result that offers the same
operators as the built-in property expectations like `HasLength()`: `EqualTo`, `GreaterThan`, `Between(…).And(…)` and
their `Not…` counterparts, with the same argument checks and texts. Implement the interface for the type of the
property:

| Property type | Interface                                            | Operators                                         |
|---------------|------------------------------------------------------|---------------------------------------------------|
| `int`         | `IIntPropertyResult<TResult>`                        | equality, ordering, `Between` and their negations |
| `long`        | `ILongPropertyResult<TResult>`                       | equality, ordering, `Between` and their negations |
| `double`      | `IDoublePropertyResult<TResult, TToleranceResult>`   | equality, ordering, `Between` and their negations |
| `decimal`     | `IDecimalPropertyResult<TResult, TToleranceResult>`  | equality, ordering, `Between` and their negations |
| `TimeSpan`    | `ITimeSpanPropertyResult<TResult>`                   | equality, ordering, `Between` and their negations |
| an enum       | `IEnumPropertyResult<TResult, TEnum>`                | `EqualTo`, `NotEqualTo`                           |
| `string`      | see [String properties](#string-properties)          |                                                   |

The operators are extension methods in the `aweXpect` namespace. They check the arguments with your `Validation`, which
receives the value and the name of its parameter, e.g. `minimum`, and pass a `PropertyComparison<T>` to your `Compare`
method. `Compare` adds the constraint and returns `TResult`, the result the caller continues with. Implement the
interface explicitly, so that its members do not show up next to the operators:

```csharp no-compile
public class DurationResult(IThat<Track?> subject)
    : ITimeSpanPropertyResult<AndOrResult<Track, IThat<Track?>>>
{
    Action<TimeSpan?, string>? ITimeSpanPropertyResult<AndOrResult<Track, IThat<Track?>>>.Validation
        => null;

    AndOrResult<Track, IThat<Track?>> ITimeSpanPropertyResult<AndOrResult<Track, IThat<Track?>>>.Compare(
        PropertyComparison<TimeSpan> comparison)
        => new(subject.Get().ExpectationBuilder.AddConstraint(comparison,
                static (c, it, grammars) => new HasDurationConstraint(it, grammars, c)),
            subject);
}
```

An expectation that returns this result, e.g. `HasDuration()`, then reads
`That(track).HasDuration().Between(TimeSpan.FromMinutes(2)).And(TimeSpan.FromMinutes(4))`.

In the constraint, `comparison.Evaluate(actual)` returns the `Outcome`, and `comparison.AppendExpectation(stringBuilder)` writes the
comparison, e.g. "between" and both values.
`IsNegated` tells that the operator is a negated one, e.g. `NotGreaterThan`: the outcome already includes the negation,
but the text does not, so that you can place the "not" where your sentence needs it, e.g. "has duration not greater
than …" or "does not have duration greater than …".

The equality operators of a `double` or `decimal` property, `EqualTo` and `NotEqualTo`, pass a
`PropertyEqualityComparison<T>` to `CompareEquality` instead, which returns `TToleranceResult`. The tolerance is set
after the operator returned, e.g. `.EqualTo(4.5).Within(0.1)` or `.Within(1).Percent()`, so pass
`comparison.Options` to the result you return, e.g. a `NumberToleranceResult<TType, TThat, TNumber>`, which offers
`Within`. Its text includes the tolerance, e.g. "equal to 4.5 ± 0.1".

<details>
<summary>`null` and `NaN` in property comparisons</summary>

An ordering against `null` returns `Outcome.FailureBothWays`. The equality operators of a `double` or `decimal`
property compare like `IsEqualTo(…).Within(…)`, e.g. `NaN` is equal to `NaN`.

</details>

### String properties

The operators of a `string` property continue with three kinds of results, so they are split over three interfaces,
each with its own `Validation` and `Compare` method. Implement those whose operators your property offers:

| Interface                                  | Operators                                                      | Usual `TResult`                  |
|--------------------------------------------|----------------------------------------------------------------|----------------------------------|
| `IStringEqualityPropertyResult<TResult>`   | `EqualTo`, `NotEqualTo`                                        | `StringEqualityTypeResult<…>`    |
| `IStringAffixPropertyResult<TResult>`      | `StartingWith`, `EndingWith`, their negations, `NotContaining` | `StringEqualityResult<…>`        |
| `IStringOccurrencePropertyResult<TResult>` | `Containing`, with a quantifier such as `AtLeast(2)`           | `StringOccurrenceCountResult<…>` |

`Compare` receives a `StringPropertyComparison`. The string options are set on your result after the operator
returned it, e.g. `.IgnoringCase()`, so pass `comparison.Options`, and for `Containing` also `comparison.Quantifier`,
to the result you return, and evaluate the comparison only in your constraint:

```csharp no-compile
public class TitleResult(IThat<Track?> subject)
    : IStringAffixPropertyResult<StringEqualityResult<Track, IThat<Track?>>>
{
    Action<string?, string>? IStringAffixPropertyResult<StringEqualityResult<Track, IThat<Track?>>>.Validation
        => null;

    StringEqualityResult<Track, IThat<Track?>> IStringAffixPropertyResult<StringEqualityResult<Track, IThat<Track?>>>.Compare(
        StringPropertyComparison comparison)
        => new(subject.Get().ExpectationBuilder.AddConstraint(comparison,
                static (c, it, grammars) => new HasTitleConstraint(it, grammars, c)),
            subject,
            comparison.Options);
}
```

An expectation that returns this result, e.g. `HasTitle()`, then reads
`That(track).HasTitle().StartingWith("let").IgnoringCase()`.

In the constraint, call `comparison.ValidateExpected()` before you look at the subject, so that an unusable expected
value, e.g. an invalid regex pattern, throws whichever value the subject has. Then
`await comparison.Evaluate(actual, context, cancellationToken)` compares the value with the options of the evaluation
and returns a `StringPropertyOutcome`, whose `Outcome` already includes the negation of an operator like
`NotStartingWith`. `comparison.AppendExpectation(stringBuilder, grammars)` writes the comparison with its options, e.g.
"starting with "let" ignoring case", or "starts with …" with `ExpectationGrammars.Active`, and
`outcome.AppendResult(stringBuilder, "title", grammars, indentation)` writes the result, e.g. "title was "Yesterday",
which differs at index 0: …". Like for the other property types, neither text includes the negation of the operator,
so that you can place it where your sentence needs it: for a comparison that `IsNegated`, add
`ExpectationGrammars.Negated` to the grammars, e.g. `grammars.Negate()`, which writes "not starting with …" or
"does not start with …". `outcome.AppendContexts(contexts)` adds the contexts of the options.

<details>
<summary>A `null` string property</summary>

A `null` value fails a comparison that looks into the value, e.g. a prefix or an occurrence, both ways.

</details>

### Custom operators

An operator of your own for any of these property types, except `string`, passes the comparison that
`PropertyComparison<T>.Satisfying(predicate, expectation)` creates to `Compare`, like the built-in operators do. It
works on every result that implements the interface, also on those of aweXpect.Core and of other packages. For
example, the day of the week on which an album was released can be on a weekend:

```csharp
public static TResult OnWeekend<TResult>(this IEnumPropertyResult<TResult, DayOfWeek> result)
    => result.Compare(PropertyComparison<DayOfWeek>.Satisfying(
        static day => day is DayOfWeek.Saturday or DayOfWeek.Sunday, "on a weekend"));

public static TResult NotOnWeekend<TResult>(this IEnumPropertyResult<TResult, DayOfWeek> result)
    => result.Compare(PropertyComparison<DayOfWeek>.Satisfying(
        static day => day is DayOfWeek.Saturday or DayOfWeek.Sunday, "on a weekend", isNegated: true));
```

The predicate receives the value of the property, which is `null` when the property is. The expectation is the text
of the comparison without its negation and follows the name of the property, e.g. "has release day on a weekend" or
"whose release day is on a weekend". For a negated operator pass `isNegated: true` with the same predicate and text:
the comparison is then met when the predicate does not hold, and the result places the "not", e.g. "does not have
release day on a weekend" or "whose release day is not on a weekend". Create a comparison that does not depend on an
argument once and reuse it, as it is immutable.
