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

In the constraint, the `StringEqualityOptions` also write the texts for every match type: `GetExpectation(expected,
grammars)` the expectation and `GetExtendedFailure(it, grammars, actual, expected)` the result, e.g. "it was "Yesterday",
which differs …". When the string is a member of the subject, `GetExtendedMemberFailure(it, "title", grammars, actual,
expected)` names it like the built-in expectations do, e.g. "it had title "Yesterday", which differs …".

For the index options, call `MatchesIndex(index, count)` on the `CollectionIndexOptions` for each item, with the number
of items or `null` while it is not known yet. Only `true` means that the item is at the expected index: `null` means
that a later item could still be, and `false` that no later item can be, so you can stop there.

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
You can offer a comparison of your own: implement `IObjectMatchType` and set it on the options of the result with
`SetMatchType`, passing the name of your option, so that combining it with another comparison throws. For example, two
tracks with the same title can count as equal:

```csharp
public static TSelf ByTitle<TSelf, TElement>(this IObjectEqualityResult<TSelf, TElement> result)
    where TSelf : IObjectEqualityResult<TSelf, TElement>
{
    result.Options.SetMatchType(new ByTitleMatchType(), nameof(ByTitle));
    return (TSelf)result;
}

private sealed class ByTitleMatchType : IObjectMatchType
{
    public ValueTask<bool> AreConsideredEqual<TActual, TExpected>(TActual actual, TExpected expected)
        => new(HaveSameTitle(actual, expected));

    public ValueTask<IObjectMatchResult> AreConsideredEqualWithExplanation<TActual, TExpected>(
        TActual actual, TExpected expected)
        => new(new ByTitleResult(HaveSameTitle(actual, expected)));

    public string GetExpectation(string expected, ExpectationGrammars grammars)
    {
        string verb = grammars.IsPlural() ? "are" : "is";
        string negation = grammars.IsNegated() ? " not" : "";
        return $"{verb}{negation} titled like {expected}";
    }

    public string PrependItemAndComparison(string expected, string? itemNoun = null, string? comparison = null)
        => $"{(itemNoun is null ? "" : itemNoun + " ")}titled like {expected}";

    public void AppendContexts(ResultContextCollector contexts)
    {
    }

    private static bool HaveSameTitle(object? actual, object? expected)
        => actual is Track a && expected is Track e ? a.Title == e.Title : actual is null && expected is null;

    private sealed class ByTitleResult(bool isMatch) : IObjectMatchResult
    {
        public bool IsMatch => isMatch;

        public string GetExtendedFailure(string it, ExpectationGrammars grammars,
            object? actual, object? expected, string? indentation = null)
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

- `AreConsideredEqual` only decides, e.g. for each item of a collection. `AreConsideredEqualWithExplanation` compares
  for a failure message and returns an `IObjectMatchResult`, whose `GetExtendedFailure` writes the result text.
- `GetExpectation` replaces "is equal to …" in the expectation text.
- `PrependItemAndComparison` describes a single expected item, e.g. in "has item titled like …".
- `AppendContexts` can add [contexts](./06-message-conventions.md#contexts) that explain a failure, e.g. the options of
  the comparison.
- For strings, implement `IStringMatchType` instead and set it with `SetMatchType` on the `StringEqualityOptions`.
  Throw in its `ValidateOptions` for a casing or a comparer that it cannot honour, and in its `ValidateExpected` for an
  expected value that it cannot use: the first throws at the call that specifies the conflict, the second for every
  subject, also when nothing is compared with the expected value, e.g. for an empty collection. Its
  `AreConsideredEqual` returns whether the strings are equal as a `StringMatchResult`, to which a `bool` converts, or
  `StringMatchResult.FailureBothWays(reason)` for a subject that it cannot compare at all, e.g. a string that is no
  valid JSON: the expectation and its negation then both fail, with the reason as the result. Like
  [`PredicateResult.FailureBothWays`](./01-generated-expectations.md#questions-that-cannot-be-answered), the reason
  follows "it", e.g. "could not be parsed as JSON" in "but it could not be parsed as JSON". Inside `Whose` it follows
  the member name instead, and in a collection "an item", so name the value in the reason as well.

A string match type adds its contexts in `AppendContexts` too. They are only added to a failure of the string
comparison, also for an item of a collection or a member inside `Whose`, and not to a failure of another expectation on
the same subject. Leave the method empty when the expectation text already explains the comparison:

```csharp no-compile
public void AppendContexts(ResultContextCollector contexts)
    => contexts.Add(new ResultContext.Fixed("Title options", " - ignore leading articles"));
```

A constraint of your own that compares strings with `StringEqualityOptions` forwards these contexts from its
`AppendContexts` with `options.AppendContexts(contexts)`.

<details>
<summary>Match results, multi-line texts and equivalency</summary>

A match type that keeps the differences of the comparison may return itself as the result of
`AreConsideredEqualWithExplanation`, which is then only valid until its next comparison. When the result text spans
several lines, indent the lines after the first by the `indentation`, e.g. with
`FormattingOptions.Indented(indentation)` for a formatted value, so that it stays aligned inside `Expect.ThatAll`.

An `ObjectEqualityOptions<T>` compares like `IsEquivalentTo` with
`SetMatchType(new EquivalencyMatchType(options), "Equivalent")`. A constraint compares with the options that
`ForEvaluation(context, cancellationToken)` returns for its evaluation, so that the timeout and the cancellation of the
evaluation also end the expectations of an `It.Is…` in the expected object. It also appends its contexts from these
options, as only they know what the comparisons found, e.g. a member to ignore that matched no member.

</details>
