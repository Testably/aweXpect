# Options and match types

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

## Options

The options, such as `.AtLeast(2)`, `.IgnoringCase()` or `.InAnyOrder()`, are extension methods in the `aweXpect`
namespace. They apply to every result that provides the matching options object, so they can be chained in any order.
A result of your own declares which options it offers by implementing `IOptionsProvider<TOptions>`. Pass it as `TSelf`
to `AndOrResult<TType, TThat, TSelf>`, so that `Because(…)`, `WithTimeout(…)` and `WithCancellation(…)` also return
your result and the options can still follow them:

```csharp
public class TrackCountResult<TThat>(
    ExpectationBuilder expectationBuilder,
    TThat returnValue,
    Quantifier quantifier,
    StringEqualityOptions options)
    : AndOrResult<IEnumerable<Track>, TThat, TrackCountResult<TThat>>(expectationBuilder, returnValue),
        IOptionsProvider<Quantifier>,
        IOptionsProvider<StringEqualityOptions>
{
    Quantifier IOptionsProvider<Quantifier>.Options => quantifier;

    StringEqualityOptions IOptionsProvider<StringEqualityOptions>.Options => options;
}
```

An expectation that returns this result then offers the count and the string options, e.g.
`.ContainsTitle("let it be").IgnoringCase().AtLeast(2)`.

| Options                                 | Offered by implementing                                                   |
|-----------------------------------------|---------------------------------------------------------------------------|
| `AtLeast`, `Between`, `Once`, …         | `IOptionsProvider<Quantifier>`                                            |
| `IgnoringCase`, `Using`, …              | `IOptionsProvider<StringEqualityOptions>`                                 |
| `AsRegex`, `AsWildcard`                 | additionally `IStringPatternMatchTypeOptions`                             |
| `AsPrefix`, `AsSuffix`                  | additionally `IStringMatchTypeOptions`, which includes the pattern marker |
| `InAnyOrder`, `IgnoringDuplicates`      | `IOptionsProvider<CollectionMatchOptions>`                                |
| `IgnoringInterspersedItems`, `Properly` | additionally `ICollectionContainmentOptions`, `IProperContainmentOptions` |
| `AtIndex`, `AtIndexFromEnd`             | `IOptionsProvider<CollectionIndexOptions>`                                |
| `Using`, `Equivalent`                   | `IObjectEqualityResult<TSelf, TElement>`                                  |
| `Within` on items                       | `IObjectEqualityWithToleranceResult<TSelf, TElement, TTolerance>`         |
| `Within` on numbers                     | `INumberToleranceResult<TSelf, TNumber>`                                  |
| `Within` on times                       | `IOptionsProvider<TimeTolerance>`                                         |

The interfaces with `TSelf` let the compiler infer the element type from your result, so pass your result type as
`TSelf`. Every option can be specified only once, and options that would replace each other, e.g. two match types or
`AtLeast(2).AtMost(5)`, throw an `InvalidOperationException` at the call. Follow the same rule for the options of your
own results: throw it with `Tracing.WriteException` at the second call, worded like the built-in ones, e.g.
`WithTitle cannot be specified more than once.` or `AtMost cannot be combined with AtLeast.`. An expectation that
already sets an option itself, e.g. `StartsWith` the match type, returns a result that does not offer it again.

The options objects of `aweXpect.Core` belong to a single expectation and are changed in place. You can also call
their methods directly, e.g. in an option of your own: each method that sets an option returns the options object, so
that calls chain, e.g. `quantifier.AtLeast(2)`, and has the name of the option that the caller writes, e.g. `Within` on
a `TimeTolerance`. The hooks for a strategy of your own are named `Set…` and take the name of your option, e.g.
`SetMatchType(matchType, nameof(ByTitle))`, or `SetMatch(match, nameof(AtTrackNumber))` on the
`CollectionIndexOptions`, so that a second call or a conflicting option throws with that name.

A result that can be stored and continued more than once, e.g. a filter of `aweXpect.Reflection`, must not change when
a continuation sets an option. Such a result implements `ICopyOnOption<StringEqualityOptions, TSelf>` in addition to
`IOptionsProvider<StringEqualityOptions>`: the string options then set the option on a copy of its options and pass it
to `WithOptions(options)`, which returns a new result with them, while the result they were called on stays unchanged.
The copy carries every option set so far, so a repeated or conflicting option still throws.

In the constraint, compare with the options that `ForEvaluation(context, cancellationToken)` returns for its evaluation,
so that the match type can use the evaluation and keep what its comparisons found. `MatchWithExplanation(actual,
expected)` returns a `MatchResult`, which the constraint keeps for its failure message, as the options keep nothing of a
comparison. The `StringEqualityOptions` also write the texts for every match type: `GetExpectation(expected, grammars)`
the expectation and `GetExtendedFailure(it, grammars, actual, expected, result)` the result, e.g. "it was "Yesterday",
which differs …". When the string is a member of the subject, `GetExtendedMemberFailure(it, "title", grammars, actual,
expected, result)` names it like the built-in expectations do, e.g. "it had title "Yesterday", which differs …".

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
| an enum       | `IEnumPropertyResult<TEnum, TResult>`                | `EqualTo`, `NotEqualTo`                           |
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
`Expect.That(track).HasDuration().Between(TimeSpan.FromMinutes(2)).And(TimeSpan.FromMinutes(4))`.

In the constraint, `comparison.Evaluate(actual)` returns the `Outcome`, with `Outcome.FailureBothWays` for an ordering
against `null`, and `comparison.AppendExpectation(stringBuilder)` writes the comparison, e.g. "between" and both values.
`IsNegated` tells that the operator is a negated one, e.g. `NotGreaterThan`: the outcome already includes the negation,
but the text does not, so that you can place the "not" where your sentence needs it, e.g. "has duration not greater
than …" or "does not have duration greater than …".

The equality operators of a `double` or `decimal` property, `EqualTo` and `NotEqualTo`, pass a
`PropertyEqualityComparison<T>` to `CompareEquality` instead, which returns `TToleranceResult`. The tolerance is set
after the operator returned, e.g. `.EqualTo(4.5).Within(0.1)` or `.Within(1).Percent()`, so pass
`comparison.Options` to the result you return, e.g. a `NumberToleranceResult<TType, TThat, TNumber>`, which offers
`Within`. The comparison compares like `IsEqualTo(…).Within(…)`, e.g. `NaN` is equal to `NaN`, and its text
includes the tolerance, e.g. "equal to 4.5 ± 0.1". The ordering operators reject a `NaN` bound.

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
`Expect.That(track).HasTitle().StartingWith("let").IgnoringCase()`.

In the constraint, `await comparison.Evaluate(actual, context, cancellationToken)` compares the value with the options
of the evaluation and returns a `StringPropertyOutcome`, whose `Outcome` already includes the negation of an operator
like `NotStartingWith`. A `null` value fails a comparison that looks into the value, e.g. a prefix or an occurrence,
both ways. `comparison.AppendExpectation(stringBuilder, grammars)` writes the comparison with its options, e.g.
"starting with "let" ignoring case", or "starts with …" with `ExpectationGrammars.Active`; unlike for the other
property types, the text includes the negation of the operator, e.g. "not starting with …", which a negated grammar
toggles. `outcome.AppendResult(stringBuilder, "title", Grammars, indentation)` writes the result, e.g. "title was
"Yesterday", which differs at index 0: …", and `outcome.AppendContexts(contexts)` adds the contexts of the options.

## Time tolerances

A `TimeToleranceResult<TType, TThat>` adds `.Within(…)` and stores the tolerance in the `TimeTolerance` options you
pass to it and to your constraint. For a date without a time of day, such as the release date of an album as a
`DateOnly`, pass a `DayTolerance` instead, as the built-in expectations do: it rejects a tolerance that is not a whole
number of days with an `ArgumentOutOfRangeException`, instead of silently dropping the part below one day.

```csharp no-compile
public static TimeToleranceResult<DateOnly, IThat<DateOnly>> IsOnSameDayAs(
    this IThat<DateOnly> subject, DateOnly expected)
{
    TimeTolerance tolerance = new DayTolerance();
    return new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
            => new IsOnSameDayAsConstraint(it, grammars, expected, tolerance)),
        subject,
        tolerance);
}
```

The caller then allows a difference of whole days, e.g. for the release date of Abbey Road:

```csharp no-compile
DateOnly releaseDate = new(1969, 9, 26);

await Expect.That(releaseDate).IsOnSameDayAs(new DateOnly(1969, 9, 27)).Within(TimeSpan.FromDays(1));
```

In the constraint, compare with `tolerance.GetToleranceOrDefault()`. Without `.Within(…)`, it returns the
`DefaultTimeComparisonTolerance` from `Customize.aweXpect.Settings()`, and for a `DayTolerance` only its whole days,
like the built-in expectations do. Append the tolerance to the expectation text like any other option, e.g. with
`stringBuilder.Append(tolerance)`: a `DayTolerance` writes the whole days that apply, e.g. " ± 1 day", and nothing when
there are none.

## Custom match types

`IsEqualTo` compares two values with `Equals`, unless the caller chooses another comparison, e.g. with `.Equivalent()`.
You can offer a comparison of your own: derive from `ObjectMatchType<TSubject>` and set it on the options of the result
with `SetMatchType`, passing the name of your option, so that combining it with another comparison throws. For example,
two tracks with the same title can count as equal:

```csharp
public static TSelf ByTitle<TSelf, TElement>(this IObjectEqualityResult<TSelf, TElement> result)
    where TSelf : IObjectEqualityResult<TSelf, TElement>
{
    result.Options.SetMatchType(new ByTitleMatchType<TElement>(), nameof(ByTitle));
    return (TSelf)result;
}

private sealed class ByTitleMatchType<TSubject> : ObjectMatchType<TSubject>
{
    private static readonly ByTitleMatcher Matcher = new();

    public override MatchPhrase Phrase { get; } = MatchPhrase.Being("titled like").WithName(" by title");

    public override ObjectMatcher<TSubject> ForEvaluation(MatchEnvironment environment) => Matcher;

    private sealed class ByTitleMatcher : ObjectMatcher<TSubject, Track>
    {
        public override ValueTask<MatchResult> Match(TSubject actual, Track expected)
            => new(MatchResult.From(actual is Track track && track.Title == expected.Title));

        public override string GetFailure(string it, ExpectationGrammars grammars, TSubject? actual,
            Track expected, MatchResult result, string? indentation)
        {
            string verb = grammars.IsPlural() && it != "it" ? " were" : " was";
            return $"{it}{verb} titled {Formatter.Format((actual as Track)?.Title)}";
        }
    }
}
```

```csharp
Track track = new("Hey Jude", new TimeSpan(0, 7, 11));

await Expect.That(track).IsEqualTo(new Track("Hey Jude", new TimeSpan(0, 7, 4))).ByTitle();
```

- The match type is configured once and serves every evaluation of the expectation. `ForEvaluation` creates the
  `ObjectMatcher<TSubject>` for one evaluation, with the context and the cancellation of the evaluation in the
  `MatchEnvironment`. A matcher without state is shared, as above, so that an evaluation allocates nothing.
- `ObjectMatcher<TSubject, TExpected>` compares with expected values of a known type, e.g. a `Track`, and an expected
  value of another type never matches. Derive from `ObjectMatcher<TSubject>` instead when the type of the expected
  value varies, e.g. an anonymous object for an equivalency, and implement the generic `Match<TExpected>`.
- `Match` compares two values and returns a `MatchResult`: `MatchResult.Match`, `MatchResult.NoMatch`, or
  `MatchResult.NotComparable(reason)` for a subject that it cannot compare at all. The expectation and its negation then
  both fail, with the reason as the result. Like
  [`PredicateResult.FailureBothWays`](./01-generated-expectations.md#questions-that-cannot-be-answered), the reason
  follows "it". Inside `Whose` it follows the member name instead, and in a collection "an item", so name the value in
  the reason as well.
- `GetFailure` writes the result text of a mismatch. A comparison that finds more than the values show, e.g. the
  differences of two documents, returns `MatchResult.NoMatchBecause(explanation)` with a `MatchExplanation` of its own,
  which the default `GetFailure` writes. `MatchWithExplanation` compares for a failure message, while `Match` only
  decides, e.g. for each item of a collection; override it when explaining costs more than deciding.
- `Phrase` holds the words of the comparison, from which Core writes the expectation text for every grammar, e.g. "is
  titled like …", "are not titled like …" or "has item titled like …". `MatchPhrase.Being(comparison)` reads "is …",
  `MatchPhrase.Verb(singular, plural, participle)` has a verb of its own, e.g. "starts with", and `MatchPhrase.EqualTo`
  is the equality, which the text of a single item may leave out, e.g. "contains 3". `WithName` names the match type in a
  short description of the options, and `WithSuffix` follows the expected value, e.g. " using TitleComparer". For a
  string match type, `WithPattern(kind, noun)` marks the expected value as a pattern, e.g. `wildcard "Let*"` for an
  expected item, and `WithUnformattedExpected()` writes it as it is instead of quoting it, e.g. a JSON document.
- The matcher can add [contexts](./06-message-conventions.md#contexts) that explain a failure in `AppendContexts`, e.g.
  the options of the comparison.
- For strings, derive from `StringMatchType` instead and set it with `SetMatchType` on the `StringEqualityOptions`.
  Its `ForEvaluation` also receives the `StringMatchSettings`, i.e. whether the casing is ignored and the comparer of the
  caller, and its `StringMatcher` compares the values after the whitespace options are applied. Core appends these
  options to the expectation text, e.g. "starts with "Let it" ignoring case". Declare the settings that the match type
  honours in `SupportedSettings`: Core rejects every other one with the same message, e.g. "IgnoringCase cannot be
  combined with AsTitle.", at the call that specifies the conflict, in either order. The whitespace options are
  supported unless you leave them out. Throw in its `ValidateExpected` for an expected value that it cannot use: it is
  called for every subject, also when nothing is compared with the expected value, e.g. for an empty collection.
- A string match type can also tell Core where its expected value lines up with the subject (`Anchors`, e.g. a prefix
  only at the start), count the occurrences of the expected value for a quantified `Contains` (`CountOccurrences` of
  the matcher), and whether a failure lists the differing lines or hints at a different newline style.

A string matcher adds its contexts in `AppendContexts` too. They are only added to a failure of the string comparison,
also for an item of a collection or a member inside `Whose`, and not to a failure of another expectation on the same
subject. Leave the method empty when the expectation text already explains the comparison:

```csharp no-compile
public override void AppendContexts(ResultContextCollector contexts)
    => contexts.Add(new ResultContext.Fixed("Title options", " - ignore leading articles"));
```

A constraint of your own that compares strings with `StringEqualityOptions` forwards these contexts from its
`AppendContexts` with `options.AppendContexts(contexts)`, on the options of its evaluation.

<details>
<summary>State per evaluation, multi-line texts and equivalency</summary>

The items of a collection are compared one after the other with the matcher of the evaluation, so a matcher may keep
state across them, e.g. which options matched nothing in any comparison, and report it in `AppendContexts`. Keep what a
single comparison found in its `MatchExplanation` instead, as a collection compares many items before it explains one.
When the result text spans several lines, indent the lines after the first by the `indentation`, e.g. with
`FormattingOptions.Indented(indentation)` for a formatted value, so that it stays aligned inside `Expect.ThatAll`.

An `ObjectEqualityOptions<T>` compares like `IsEquivalentTo` with
`SetMatchType(new EquivalencyMatchType<T>(options), "Equivalent")`. A constraint compares with the options that
`ForEvaluation(context, cancellationToken)` returns for its evaluation, so that the timeout and the cancellation of the
evaluation also end the expectations of an `It.Is…` in the expected object. It also appends its contexts from these
options, as only they know what the comparisons found, e.g. a member to ignore that matched no member.

</details>
