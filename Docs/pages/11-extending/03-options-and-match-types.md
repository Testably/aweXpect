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
namespace. They apply to every result that provides the matching options, so they can be chained in any order.

### One holder per expectation

The options of an expectation are an immutable value, e.g. a `Quantifier` or `StringEqualityOptions`, in an
`OptionsHolder<TOptions>`. The holder is a plain reference to the current value: an option replaces the value in the holder,
and the value itself never changes, so a value can be shared and stored. Each options type starts from its `Default`,
e.g. `Quantifier.Default` ("at least once") or `StringEqualityOptions.Default`.

Create the holder in your expectation, e.g. `new OptionsHolder<Quantifier>(Quantifier.Default)`, and pass it to your
constraint and to your result. A result declares which options it offers by implementing `IOptionsProvider<TOptions>`,
which returns this holder. The constraint reads `holder.Value` when it evaluates, never when it is built, as the options
are set on the result after the constraint was added; a constraint that only reads the options can take the holder as a
covariant `IReadOnlyOptionsHolder<TOptions>`. Pass your result as `TSelf` to `AndOrResult<TType, TThat, TSelf>`, so
that `Because(…)`, `WithTimeout(…)` and `WithCancellation(…)` also return your result and the options can still
follow them:

```csharp
public class TrackCountResult<TThat>(
    ExpectationBuilder expectationBuilder,
    TThat returnValue,
    OptionsHolder<Quantifier> quantifier,
    OptionsHolder<StringEqualityOptions> options)
    : AndOrResult<IEnumerable<Track>, TThat, TrackCountResult<TThat>>(expectationBuilder, returnValue),
        IOptionsProvider<Quantifier>,
        IOptionsProvider<StringEqualityOptions>
{
    OptionsHolder<Quantifier> IOptionsProvider<Quantifier>.Options => quantifier;

    OptionsHolder<StringEqualityOptions> IOptionsProvider<StringEqualityOptions>.Options => options;
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
`TSelf`. An expectation that already sets an option itself, e.g. `StartsWith` the match type, returns a result that
does not offer it again.

### Setting options in your own code

You can also set the options of `aweXpect.Core` in your own code, e.g. in an option of your own. Each method of an
options value that sets an option returns a new value with the option set, e.g. `Quantifier.Default.AtLeast(2)`, and
the same methods are extension methods on the holder in the `aweXpect.Options` namespace, which replace the value in
the holder and return the holder, so that calls chain, e.g. `result.Options.AtLeast(2).AtMost(5)`. The members that only
read the options are on `result.Options.Value`.

:::warning
Import `aweXpect.Options` where you call an option on a holder. Without it, the compiler only finds the user-facing
option of the same name in `aweXpect`, which extends the result and not the holder, and reports `CS0311`.
:::

:::warning
Use the value that an option method returns. Most options types are `readonly struct`s, so a call whose result is
discarded, e.g. `options.AtLeast(2);`, compiles and changes nothing. The analyzer rule
[aweXpect0009](../08-analyzers.md#awexpect0009) reports it in a project that references `aweXpect`.
:::

The rules for the options are the same everywhere:

- A repeated option replaces the earlier one, e.g. `AtLeast(2).AtLeast(3)` requires three.
- An option set to `false` is the same as not set, e.g. `IgnoringCase(false)`.
- Options that set the same thing in different ways throw an `InvalidOperationException` at the call, e.g.
  `AtMost cannot be combined with AtLeast.` or `AsSuffix cannot be combined with AsPrefix.`
- The hooks for a strategy of your own are named `Set…` and take the name of your option, e.g.
  `SetMatchType(matchType, nameof(AsTitle))` on the `StringEqualityOptions` or `SetMatch(match, nameof(AtTrackNumber))`
  on the `CollectionIndexOptions`, so that a competing option throws with that name, while the same name again
  replaces the earlier strategy. An [object match type](#custom-match-types) carries its name itself.

### Options of your own

Follow the same rules for an options type of your own: an immutable value with a `Default`, whose option methods
return a new value, and an extension method on its holder per option. For example, a playlist expectation can ignore
the bonus tracks and compare in album order or shuffled, but not both:

```csharp
public readonly struct PlaylistOptions
{
    private readonly string? _orderOption;

    private PlaylistOptions(string? orderOption, bool ignoresBonusTracks)
    {
        _orderOption = orderOption;
        IgnoresBonusTracks = ignoresBonusTracks;
    }

    public static PlaylistOptions Default => default;

    public bool IgnoresBonusTracks { get; }

    public bool IsShuffled => _orderOption == nameof(Shuffled);

    public PlaylistOptions IgnoringBonusTracks(bool ignoreBonusTracks = true)
        => new(_orderOption, ignoreBonusTracks);

    public PlaylistOptions InAlbumOrder() => WithOrder(nameof(InAlbumOrder));

    public PlaylistOptions Shuffled() => WithOrder(nameof(Shuffled));

    private PlaylistOptions WithOrder(string option)
    {
        if (_orderOption is not null && _orderOption != option)
        {
            throw Tracing.WriteException(new InvalidOperationException(
                $"{option} cannot be combined with {_orderOption}."));
        }

        return new PlaylistOptions(option, IgnoresBonusTracks);
    }
}

public static class PlaylistOptionsHolderExtensions
{
    public static OptionsHolder<PlaylistOptions> IgnoringBonusTracks(
        this OptionsHolder<PlaylistOptions> holder, bool ignoreBonusTracks = true)
    {
        holder.Value = holder.Value.IgnoringBonusTracks(ignoreBonusTracks);
        return holder;
    }
}
```

The user-facing option extends every result that provides the options and returns it, so that the caller can go on
with the result, e.g. `.HasTracks(12).IgnoringBonusTracks()`:

```csharp
public static TResult IgnoringBonusTracks<TResult>(this TResult result, bool ignoreBonusTracks = true)
    where TResult : IOptionsProvider<PlaylistOptions>
{
    result.Options.IgnoringBonusTracks(ignoreBonusTracks);
    return result;
}
```

`Default` is the value of `default(PlaylistOptions)`, so that an options field that was never set is valid. Throw
with `Tracing.WriteException`, so that it is written to the trace writer of
`Customize.aweXpect.EnableTracing(…)` like the built-in exceptions, and word the message like the built-in ones. When the expectation text names the options, write that text in `ToString()`: an
`OptionsHolder<TOptions>` returns the same text, so that the expectation can append the holder itself.

### Results that are stored

A result that can be stored and continued more than once, e.g. a filter of `aweXpect.Reflection`, must not change when
a continuation sets an option. Such a result implements `ICopyOnOption<StringEqualityOptions, TSelf>` in addition to
`IOptionsProvider<StringEqualityOptions>`: the string options then pass its options with the new option to
`WithOptions(options)`, which returns a new result with them in a new holder, while the result they were called on stays
unchanged. The new options carry every option set so far, so a repeated option replaces the earlier one and a
competing option still throws. A string option of your own, e.g. a preset of several options, does the same, so
that it keeps a stored result unchanged as well:

```csharp
public static TResult IgnoringCaseAndIndentation<TResult>(this TResult result)
    where TResult : IOptionsProvider<StringEqualityOptions>
{
    if (result is ICopyOnOption<StringEqualityOptions, TResult> copyOnOption)
    {
        return copyOnOption.WithOptions(result.Options.Value.IgnoringCase().IgnoringIndentation());
    }

    result.Options.IgnoringCase().IgnoringIndentation();
    return result;
}
```

Like every constraint, such a result reads its holder when it evaluates: an option that sets the holder directly, without
`WithOptions`, still applies, but it changes the result it was called on.

### Comparing with the options

In the constraint, compare with the options that `ForEvaluation(context, cancellationToken)` returns for its evaluation,
so that the match type can use the evaluation and keep what its comparisons found. `MatchWithExplanation(actual,
expected, nameof(expected))` returns a `MatchResult`, which the constraint keeps for its failure message, as the options
keep nothing of a comparison. The string options carry no parameter name: the members that can reject the expected
value, e.g. an invalid regex pattern, take the name of the parameter that the `ArgumentException` reports, e.g.
`ValidateExpected(expected, nameof(expected))`, `MatchWithExplanation(…)`, `AreConsideredEqual(actual, expected,
nameof(expected))` or `CountOccurrences(…)`. The `StringEqualityOptions` also write the texts for every match type:
`GetExpectation(expected, grammars)` the expectation and `GetExtendedFailure(it, grammars, actual, expected, result)`
the result, e.g. "it was "Yesterday", which differs …". When the string is a member of the subject,
`GetExtendedMemberFailure(it, "title", grammars, actual, expected, result)` names it like the built-in expectations do,
e.g. "it had title "Yesterday", which differs …".

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

In the constraint, call `comparison.ValidateExpected()` before you look at the subject, so that an unusable expected
value, e.g. an invalid regex pattern, throws whichever value the subject has. Then
`await comparison.Evaluate(actual, context, cancellationToken)` compares the value with the options of the evaluation
and returns a `StringPropertyOutcome`, whose `Outcome` already includes the negation of an operator like
`NotStartingWith`. A `null` value fails a comparison that looks into the value, e.g. a prefix or an occurrence, both
ways. `comparison.AppendExpectation(stringBuilder, grammars)` writes the comparison with its options, e.g.
"starting with "let" ignoring case", or "starts with …" with `ExpectationGrammars.Active`, and
`outcome.AppendResult(stringBuilder, "title", grammars, indentation)` writes the result, e.g. "title was "Yesterday",
which differs at index 0: …". Like for the other property types, neither text includes the negation of the operator,
so that you can place it where your sentence needs it: for a comparison that `IsNegated`, add
`ExpectationGrammars.Negated` to the grammars, e.g. `grammars.Negate()`, which writes "not starting with …" or
"does not start with …". `outcome.AppendContexts(contexts)` adds the contexts of the options.

### Custom operators

An operator of your own for any of these property types, except `string`, passes the comparison that
`PropertyComparison<T>.Satisfying(predicate, expectation)` creates to `Compare`, like the built-in operators do. It
works on every result that implements the interface, also on those of aweXpect.Core and of other packages. For
example, the day of the week on which an album was released can be on a weekend:

```csharp
public static TResult OnWeekend<TResult>(this IEnumPropertyResult<DayOfWeek, TResult> result)
    => result.Compare(PropertyComparison<DayOfWeek>.Satisfying(
        static day => day is DayOfWeek.Saturday or DayOfWeek.Sunday, "on a weekend"));

public static TResult NotOnWeekend<TResult>(this IEnumPropertyResult<DayOfWeek, TResult> result)
    => result.Compare(PropertyComparison<DayOfWeek>.Satisfying(
        static day => day is DayOfWeek.Saturday or DayOfWeek.Sunday, "on a weekend", isNegated: true));
```

The predicate receives the value of the property, which is `null` when the property is. The expectation is the text
of the comparison without its negation and follows the name of the property, e.g. "has release day on a weekend" or
"whose release day is on a weekend". For a negated operator pass `isNegated: true` with the same predicate and text:
the comparison is then met when the predicate does not hold, and the result places the "not", e.g. "does not have
release day on a weekend" or "whose release day is not on a weekend". Create a comparison that does not depend on an
argument once and reuse it, as it is immutable.

## Time tolerances

A `TimeToleranceResult<TType, TThat>` adds `.Within(…)` and stores the tolerance in the holder of the `TimeTolerance` options
you pass to it and to your constraint; start the holder with `TimeTolerance.Default`. For a date without a time of day,
such as the release date of an album as a `DateOnly`, start it with `TimeTolerance.Days` instead, as the built-in
expectations do: it rejects a tolerance that is not a whole number of days with an `ArgumentOutOfRangeException`,
instead of silently dropping the part below one day.

```csharp no-compile
public static TimeToleranceResult<DateOnly, IThat<DateOnly>> IsOnSameDayAs(
    this IThat<DateOnly> subject, DateOnly expected)
{
    OptionsHolder<TimeTolerance> tolerance = new(TimeTolerance.Days);
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

In the constraint, compare with `tolerance.Value.GetToleranceOrDefault()`. Without `.Within(…)`, it returns the
`DefaultTimeComparisonTolerance` from `Customize.aweXpect.Settings()`, and for `TimeTolerance.Days` only its whole
days, like the built-in expectations do. Append the tolerance to the expectation text like any other option, e.g. with
`stringBuilder.Append(tolerance)`: `TimeTolerance.Days` writes the whole days that apply, e.g. " ± 1 day", and nothing
when there are none.

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
Its common leading whitespace is removed, and an expected `null` is still shown as `<null>`.

## Custom match types

`IsEqualTo` compares two values with `Equals`, unless the caller chooses another comparison, e.g. with `.Equivalent()`.
You can offer a comparison of your own: derive from `ObjectMatchType<TSubject>`, pass the name of your option to its
base constructor and set it on the options of the result with `SetMatchType(matchType)`, which reads that name: the
same option again replaces the earlier match type, and combining it with another comparison throws, e.g. "ByTitle
cannot be combined with Equivalent.". `SetMatchType` rejects a match type without a name. For example, two tracks with the same title can count as equal:

```csharp
public static TSelf ByTitle<TSelf, TElement>(this IObjectEqualityResult<TSelf, TElement> result)
    where TSelf : IObjectEqualityResult<TSelf, TElement>
{
    result.Options.SetMatchType(new ByTitleMatchType<TElement>());
    return (TSelf)result;
}

private sealed class ByTitleMatchType<TSubject>() : ObjectMatchType<TSubject>("ByTitle")
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
- For strings, derive from `StringMatchType` instead and set it with `SetMatchType(matchType, nameof(AsTitle))` on the
  `StringEqualityOptions`, which takes the name of your option, as a string match type carries none.
  Its `ForEvaluation` also receives the `StringMatchSettings`, i.e. whether the casing is ignored and the comparer of the
  caller, and its `StringMatcher` compares the values after the whitespace options are applied. Core appends these
  options to the expectation text, e.g. "starts with "Let it" ignoring case". Declare the settings that the match type
  honours in `SupportedSettings`: Core rejects every other one with the same message, e.g. "IgnoringCase cannot be
  combined with AsTitle.", at the call that specifies the conflict, in either order. The whitespace options are
  supported unless you leave them out. Throw in its `ValidateExpected(expected, parameterName)` for an expected value
  that it cannot use, with an `ArgumentException` for the `parameterName` it receives, as neither the match type nor the
  options know the parameter of the expectation: it is called for every subject, also when nothing is compared with the
  expected value, e.g. for an empty collection.
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

An `EquivalencyMatchType<T>` compares like `IsEquivalentTo`. A constraint compares with the matcher that
`ForEvaluation(new MatchEnvironment(context, cancellationToken))` returns for its evaluation, e.g. with
`CompareWithExplanation(actual, expected)`, so that the timeout and the cancellation of the evaluation also end the
expectations of an `It.Is…` in the expected object. It also appends its contexts from this matcher, as only it knows
what the comparisons found, e.g. a member to ignore that matched no member.

</details>
