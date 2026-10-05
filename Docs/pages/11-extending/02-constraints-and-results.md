# Constraints and results

The samples on this page use the following namespaces:

```csharp
using System.Diagnostics.CodeAnalysis;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Formatting;
using aweXpect.Results;
using static aweXpect.Formatting.Format;
```

## Constraints

The basis for expectations are constraints. You can add different constraints to the `ExpectationBuilder` that is
available for the `IThat<T>`. They differ in the input and output parameters for the `IsMetBy` method:

| Constraint                                            | `IsMetBy` receives                         | Use it                                          |
|-------------------------------------------------------|--------------------------------------------|-------------------------------------------------|
| `IValueConstraint<T>`                                 | the actual value                           | for a synchronous check                         |
| `IAsyncConstraint<T>`                                 | the actual value and a `CancellationToken` | for asynchronous work or to observe the timeout |
| `IContextConstraint<T>`, `IAsyncContextConstraint<T>` | additionally an `IEvaluationContext`       | to share data between constraints               |

The `IEvaluationContext` allows storing and receiving data between expectations. This mechanism is used for example to
avoid enumerating an `IEnumerable` multiple times across multiple constraints. A value stored while an item of a
collection or a member after `Whose` or `Which` is evaluated is only received by the expectations on that item or
member, so a value cached for one item is not handed to the next one. Its `Cancellation` describes the
cancellation of the evaluation: the `Token` (the same token that an asynchronous constraint receives), the effective
`Timeout`, and the `Reason` of a cancellation: `None`, the `Timeout`, or the `Caller`.

`IsMetBy` returns a `ConstraintResult`, which decides the outcome and writes the expectation and the result texts of
the failure message:

```csharp
/// <summary>
///     This example does NOT support the negated case!
/// </summary>
private sealed class IsRadioFriendlyConstraint(string it, ExpectationGrammars grammars)
    : ConstraintResult(grammars),
        IValueConstraint<Track?>
{
    private Track? _actual;
    public ConstraintResult IsMetBy(Track? actual)
    {
        _actual = actual;
        Outcome = actual?.Duration <= TimeSpan.FromMinutes(3) ? Outcome.Success : Outcome.Failure;
        return this;
    }

    public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append("is radio friendly");

    public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
    {
        stringBuilder.Append(it).Append(" was ");
        Formatter.Format(stringBuilder, _actual?.Duration);
        stringBuilder.Append(" long");
    }

    public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
    {
        if (_actual is TValue typedValue)
        {
            value = typedValue;
            return true;
        }

        value = default;
        return typeof(TValue).IsAssignableFrom(typeof(Track));
    }

    public override ConstraintResult Negate()
        => throw new NotSupportedException("Negation of IsRadioFriendly is not supported.");
}
```

`Negate()` is called whenever the expectation is negated, e.g. by `DoesNotComplyWith(x => x.IsRadioFriendly())`, and
the caller relies on the returned result being negated. Returning `this` unchanged would silently check the
non-negated expectation instead, so a constraint that cannot be negated throws, like the built-in `ExecutesIn()`.

## Results

All constraints should also provide the expectations and results for the negated case, so that they are compatible
with `DoesNotComplyWith`. In order to streamline common cases, the recommended practice is to use the same class also
for the `ConstraintResult`, in most cases with one of the following helper classes:

| Helper class                           | `null` subject                                   | Use it when the expectation                         |
|----------------------------------------|--------------------------------------------------|-----------------------------------------------------|
| `ConstraintResult.WithNotNullValue<T>` | fails the expectation and its negation           | inspects the subject                                |
| `ConstraintResult.WithEqualToValue<T>` | is an ordinary value, compared with the expected | compares for equality or identity                   |
| `ConstraintResult.WithValue<T>`        | is not handled                                   | handles `null` itself or has a non-nullable subject |

With all three, you set the `Actual` property in the `IsMetBy` method and override `AppendNormalExpectation` and
`AppendNegatedExpectation` as well as `AppendNormalResult` and `AppendNegatedResult`. `WithEqualToValue<T>`
additionally takes a flag indicating if the expected value is `null`.

All three take the name of the subject (`it`) and the `grammars` in their constructor and expose the name as the
inherited `It` property, which the default result texts use.

With these the above example could be written (with support for the negated case):

```csharp
private sealed class IsRadioFriendlyConstraint(string it, ExpectationGrammars grammars)
    : ConstraintResult.WithNotNullValue<Track>(it, grammars),
        IValueConstraint<Track?>
{
    public ConstraintResult IsMetBy(Track? actual)
    {
        Actual = actual;
        Outcome = actual?.Duration <= TimeSpan.FromMinutes(3) ? Outcome.Success : Outcome.Failure;
        return this;
    }

    protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append("is radio friendly");

    protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
    {
        stringBuilder.Append(It).Append(" was ");
        Formatter.Format(stringBuilder, Actual?.Duration);
        stringBuilder.Append(" long");
    }

    protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append("is not radio friendly");

    protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
    {
        stringBuilder.Append(It).Append(" was ");
        Formatter.Format(stringBuilder, Actual?.Duration);
        stringBuilder.Append(" long");
    }
}
```

Note that the `it` parameter is passed to the base class and the inherited `It` property is used in the body: capturing
the parameter *and* passing it to the base stores it twice, which the compiler warns about (CS9107).

The helper classes also let `.And` combine the result texts like the built-in expectations, e.g. "it was 2 and was
not even" instead of "it was 2 and it was not even". A constraint that derives from `ConstraintResult` directly does
the same by overriding `LeadingSubject` and `TrailingSubject` with `GetSubjectOfResult(it)`.

## `null` subjects

Which of the three helper classes to pick is decided by how your expectation treats a `null` subject, and that follows
the rule that all built-in expectations follow (see [`null` subjects](../03-how-it-works/04-null-subjects.md)):

> A `null` subject fails an expectation **and its negation**, unless the expectation is *about* `null`: equality and
> identity comparisons, where `null` is a legitimate value on either side, or an explicit `null` or tri-state check.

- Your expectation **inspects the subject**: its length, its type, its items, whether it is empty. There is nothing to
  inspect when the subject is `null`, so it has to fail, in the negated case as well: `IsNotEmpty()` fails for a `null`
  subject just like `IsEmpty()` does, and so does `DoesNotComplyWith(x => x.IsEmpty())`. Use
  `ConstraintResult.WithNotNullValue<T>`.
- Your expectation **compares the subject for equality or identity** against a value the caller supplied. Then `null`
  is an ordinary value on both sides: `IsEqualTo(null)` succeeds for a `null` subject, `IsNotEqualTo(null)` fails and
  `IsNotEqualTo("foo")` succeeds. Use `ConstraintResult.WithEqualToValue<T>` and pass whether the expected value is
  `null`; that flag is what makes the subject fail on the side where `null` is not a legitimate answer.

Do not read the second case as "any value the caller supplied": `HasValue(2)` takes one and still fails for `null`,
because it inspects the subject rather than comparing it. Only equality and identity give `null` a meaning on both
sides; an ordering or a range does not, which is why `IsGreaterThan` and `IsNotBetween` use
`ConstraintResult.WithNotNullValue<T>`.

Use `ConstraintResult.WithValue<T>` only when the subject cannot be `null` at all (a non-nullable `bool`, `int` or
`DateTime`), when your expectation is one of the `null` checks that a `null` subject is meant to satisfy, such as
`IsNull()`, or when it decides every `null` case itself, such as `IsOneOf(...)`, whose expected values may or may not
include `null`, which the single flag of `WithEqualToValue<T>` cannot express. It applies no `null` policy of its own,
so deciding the outcome with `Actual is null ? Outcome.Failure : ...` inside `IsMetBy` is **not** enough: that failure
is inverted into a success when the expectation is negated. A failure that the negation keeps is a
[failure both ways](#failing-both-ways), which `WithNotNullValue<T>` reports for a `null` subject.

### Nullability warnings

After an expectation that fails for a `null` subject, the subject can't be `null` any more. Mark such an expectation
with the `[GuaranteesNotNull]` attribute from `aweXpect.Core`, so that the
[nullability suppressor](../07-analyzers.md) of the `aweXpect` package suppresses the nullability warnings for the
subject after your expectation, as it does after `IsNotNull()`:

```csharp no-compile
[GuaranteesNotNull]
public static AndOrResult<Track, IThat<Track?>> IsRadioFriendly(this IThat<Track?> subject)
    => // ...
```

```csharp
Track[] tracks = [new("Love Me Do", new TimeSpan(0, 2, 22)), new("Hey Jude", new TimeSpan(0, 7, 11))];
Track? track = tracks.FirstOrDefault(t => t.Title == "Love Me Do");

await Expect.That(track).IsRadioFriendly();
string title = track.Title;   // no CS8602
```

- Only mark an expectation that fails for a `null` subject regardless of its other arguments, e.g. one that uses
  `ConstraintResult.WithNotNullValue<T>`. A wrongly marked expectation hides real nullability warnings.
- The expectation has to be an extension method on `IThat<TSubject>`.
- An expectation of your package before `.And`, as in `Expect.That(track).IsRadioFriendly().And.IsNotNull()`, keeps
  the subject for the suppressor only when it returns an aweXpect result for the same `IThat<TSubject>`, such as
  `AndOrResult<Track, IThat<Track?>>`. An expectation that returns an `IThat<…>` itself could continue with a
  different subject, so the suppressor ignores the expectations after it.

## Failing both ways

Some expectations cannot be answered at all, e.g. because the subject is `null`, the values are not comparable or code
of the caller threw. Negating such an expectation does not make it true, so it fails the expectation and its negation
alike. Set `Outcome.FailureBothWays` in `IsMetBy` to say so:

```csharp no-compile
catch (Exception exception)
{
    _exception = exception;
    Outcome = Outcome.FailureBothWays;
}
```

- `Outcome.FailureBothWays` fails the expectation like `Outcome.Failure`, but a negation keeps it.
  `WithNotNullValue<T>` reports it for a `null` subject.
- A result that derives from `ConstraintResult` directly only swaps `Success` and `Failure` in `Negate()`.
- Code that reads an outcome treats `Outcome.Failure` and `Outcome.FailureBothWays` alike as a failed expectation. An
  expectation that evaluates nested expectations, e.g. on the items of a collection, checks for
  `Outcome.FailureBothWays` to find out whether an item was answered.

## Negated expectations

A constraint that supports the negated case also allows you to write an explicit negated expectation with the
`.Invert()` method:

```csharp
/// <summary>
///     Verifies that the <paramref name="subject"/> is not radio friendly.
/// </summary>
public static AndOrResult<Track, IThat<Track?>> IsNotRadioFriendly(
    this IThat<Track?> subject)
    => new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
            => new IsRadioFriendlyConstraint(it, grammars).Invert()),
        subject);
```

## Code of the caller

When a constraint calls code of the caller while it is evaluated, e.g. a predicate, a member selector or a comparer,
call it through `UserCode.Invoke`. An exception it throws then fails the expectation and its negation alike, with the
exception as inner exception, instead of aborting the evaluation:

```csharp no-compile
Outcome = UserCode.Invoke(predicate, actual, "the predicate") ? Outcome.Success : Outcome.Failure;
```

The optional last argument names the code in the failure message, e.g. "the predicate did throw an
InvalidOperationException"; without it, the subject is named ("it did throw …").

An exception that your constraint throws itself, e.g. to reject an invalid argument, is still thrown as it is.

## Argument validation

Check the arguments in the extension method itself, so that a wrong argument fails right where the caller passes it.
Use the wording of the built-in expectations: a missing argument reads "The 'title' cannot be null.", and any other
invalid argument is described in a complete sentence, as the [message conventions](./03-message-conventions.md#exceptions)
explain. Throw the exception via `Tracing.WriteException`, which also hands it to the trace writer that users enable
with `Customize.aweXpect.EnableTracing(…)`. Core has no public helper for this, so add a small one to your extension:

```csharp
internal static void ThrowIfNull(object? value, string paramName)
{
    if (value is null)
    {
        throw Tracing.WriteException(new ArgumentNullException(paramName, $"The '{paramName}' cannot be null."));
    }
}
```

```csharp no-compile
public static AndOrResult<Track, IThat<Track?>> HasTitle(this IThat<Track?> subject, string title)
{
    ThrowIfNull(title, nameof(title));
    // ...
}
```

## Collection subjects

A constraint that enumerates a collection subject gets it from the `IEvaluationContext` with
`UseMaterializedEnumerable` instead of enumerating the subject itself. All expectations on the subject, including the
built-in ones, then share one lazily materialized copy, so that a subject which can only be enumerated once, e.g. a
query or an iterator with side effects, is enumerated at most once, also when the expectations are combined with
`.And` or `.Or`. Implement `IContextConstraint<T>` to receive the context:

```csharp
using System.Collections.Generic;
using System.Linq;
using aweXpect.Core.EvaluationContext;

public static AndOrResult<IEnumerable<Track>, IThat<IEnumerable<Track>?>> HasRadioFriendlyTracks(
    this IThat<IEnumerable<Track>?> subject, int expected)
    => new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
            => new HasRadioFriendlyTracksConstraint(it, grammars, expected)),
        subject);

private sealed class HasRadioFriendlyTracksConstraint(string it, ExpectationGrammars grammars, int expected)
    : ConstraintResult.WithNotNullValue<IEnumerable<Track>>(it, grammars),
        IContextConstraint<IEnumerable<Track>?>
{
    private int _count;

    public ConstraintResult IsMetBy(IEnumerable<Track>? actual, IEvaluationContext context)
    {
        Actual = actual;
        if (actual is not null)
        {
            _count = context.UseMaterializedEnumerable(actual)
                .Count(track => track.Duration <= TimeSpan.FromMinutes(3));
            Outcome = _count == expected ? Outcome.Success : Outcome.Failure;
        }

        return this;
    }

    protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append("has ").Append(expected).Append(" radio friendly tracks");

    protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append(It).Append(" had ").Append(_count).Append(" radio friendly tracks");

    protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append("does not have ").Append(expected).Append(" radio friendly tracks");

    protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append(It).Append(" did");
}
```

`await Expect.That(tracks).IsNotEmpty().And.HasRadioFriendlyTracks(3)` then enumerates `tracks` only once.

- Every call for the same subject in the same evaluation returns the same sequence. It reads the subject only as far
  as it is enumerated, and a further enumeration replays the items read so far before it continues the subject.
  Once the expectation and its failure message are complete, the enumerator of the subject is disposed, also when it
  was not read to its end.
- A subject that already is a collection is returned unchanged. For any other subject, an exception while it is
  enumerated fails the expectation like one of the [code of the caller](#code-of-the-caller).
- For an `IAsyncEnumerable<T>`, implement `IAsyncContextConstraint<T>` and call `UseMaterializedAsyncEnumerable` with
  its `CancellationToken`. The subject stays governed by the token of the first call. This method is only available
  on .NET 8 or later.

### Expected collections

A collection that the caller passes as the expected value, e.g. the titles a playlist must contain, can also be a
query that may only be enumerated once. Enumerate it only once per evaluation: copy it at the start of `IsMetBy`
unless it already is a collection, and use only the copy afterwards, also for the failure message:

```csharp no-compile
public ConstraintResult IsMetBy(IEnumerable<Track>? actual, IEvaluationContext context)
{
    Actual = actual;
    ICollection<string> titles = expected as ICollection<string> ?? expected.ToArray();
    if (actual is not null)
    {
        _missingTitles = titles
            .Except(context.UseMaterializedEnumerable(actual).Select(track => track.Title))
            .ToList();
        Outcome = _missingTitles.Count == 0 ? Outcome.Success : Outcome.Failure;
    }

    return this;
}
```

The subject itself still goes through `UseMaterializedEnumerable`, so that it is shared with the other expectations
on it, while the expected collection belongs to your constraint alone.

## Continuing with the value

The first type argument of the result, e.g. `Track` in `AndOrResult<Track, IThat<Track?>>`, is the type of the value
that the expectation passes on. Awaiting the expectation returns this value, and an `AndOrWhoseResult<TType, TThat>`
also continues with `Whose` on a member of it:

```csharp
public static AndOrWhoseResult<Track, IThat<Track?>> IsRadioFriendly(this IThat<Track?> subject)
    => new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
            => new IsRadioFriendlyConstraint(it, grammars)),
        subject);
```

```csharp
Track track = new("Love Me Do", new TimeSpan(0, 2, 22));

Track verifiedTrack = await Expect.That(track).IsRadioFriendly();
await Expect.That(track).IsRadioFriendly().Whose(t => t.Title, title => title.StartsWith("Love"));
```

Both ask the `TryGetStoredValue<TValue>` method of the `ConstraintResult` for the value, so a constraint that narrows
or converts the subject returns the converted value there. `Whose` and `AndWhose` access the member on that value when
the expectation is met; when it is not met, a member of a converted value is not evaluated, only its expectations are
shown. The helper classes return their `Actual` value. A stored
`null` value of a matching type still returns `true`, so that awaiting the expectation returns `null`. When
`TryGetStoredValue` returns `false` for the type, awaiting the successful expectation returns the `default` of the
type, e.g. after an `.Or` whose earlier alternative was met.
`TryGetValue<TValue>` builds on it and only returns `true` for a value that is not `null`.

A result of your own that derives from `AndOrResult<TType, TThat>` reaches the `ExpectationBuilder` through the
protected property of the same name, e.g. to continue with `ExpectationBuilder.And()` or `ExpectationBuilder.Or()`.

## Options

The options, such as `.AtLeast(2)`, `.IgnoringCase()` or `.InAnyOrder()`, are extension methods in the `aweXpect`
namespace. They apply to every result that provides the matching options object, so they can be chained in any order.
A result of your own declares which options it offers by implementing `IOptionsProvider<TOptions>` and passing itself as
`TSelf` to `AndOrResult<TType, TThat, TSelf>`, so that every option returns your result again:

```csharp
using aweXpect.Options;

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
| `AsPrefix`, `AsRegex`, `AsWildcard`, …  | additionally `IStringMatchTypeOptions`                                    |
| `InAnyOrder`, `IgnoringDuplicates`      | `IOptionsProvider<CollectionMatchOptions>`                                |
| `IgnoringInterspersedItems`, `Properly` | additionally `ICollectionContainmentOptions`, `IProperContainmentOptions` |
| `AtIndex`, `AtIndexFromEnd`             | `IOptionsProvider<CollectionIndexOptions>`                                |
| `Using`, `Equivalent`                   | `IObjectEqualityResult<TSelf, TElement>`                                  |
| `Within` on items                       | `IObjectEqualityWithToleranceResult<TSelf, TElement, TTolerance>`         |
| `Within` on numbers                     | `INumberToleranceResult<TSelf, TNumber>`                                  |
| `Within` on times                       | `IOptionsProvider<TimeTolerance>`                                         |

The interfaces with `TSelf` let the compiler infer the element type from your result, so pass your result type as
`TSelf`. Every option can be specified only once, and options that would replace each other, e.g. two match types or
`AtLeast(2).AtMost(5)`, throw an `InvalidOperationException` at the call. An expectation that already sets an option
itself, e.g. `StartsWith` the match type, returns a result that does not offer it again.

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
`DefaultTimeComparisonTolerance` from `Customize.aweXpect.Settings()`, and for a `DayTolerance` only its whole days, like
the built-in expectations do. Append the tolerance to the expectation text like any other option, e.g. with
`stringBuilder.Append(tolerance)`: a `DayTolerance` writes the whole days that apply, e.g. " ± 1 day", and nothing when
there are none.

## Custom match types

`IsEqualTo` compares two values with `Equals`, unless the caller chooses another comparison, e.g. with `.Equivalent()`.
You can offer a comparison of your own: implement `IObjectMatchType` and set it on the options of the result with
`SetMatchType`, passing the name of your option, so that combining it with another comparison throws. For example, two
tracks with the same title can count as equal:

```csharp
using System.Threading.Tasks;
using aweXpect.Options;

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
        => $"{(grammars.IsPlural() ? "are" : "is")} {(grammars.IsNegated() ? "not " : "")}titled like {expected}";

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

        public string GetExtendedFailure(string it, ExpectationGrammars grammars, object? actual, object? expected)
            => $"{it}{(grammars.IsPlural() && it != "it" ? " were" : " was")} titled {Formatter.Format((actual as Track)?.Title)}";
    }
}
```

```csharp
Track track = new("Hey Jude", new TimeSpan(0, 7, 11));

await Expect.That(track).IsEqualTo(new Track("Hey Jude", new TimeSpan(0, 7, 4))).ByTitle();
```

- `AreConsideredEqual` only decides, e.g. for each item of a collection. `AreConsideredEqualWithExplanation` compares
  for a failure message and returns an `IObjectMatchResult`, whose `GetExtendedFailure` writes the result text. A match
  type that keeps the differences of the comparison may return itself as the result, which is then only valid until
  its next comparison.
- `GetExpectation` replaces "is equal to …" in the expectation text.
- `PrependItemAndComparison` describes a single expected item, e.g. in "has item titled like …".
- `AppendContexts` can add [contexts](03-message-conventions.md#contexts) that explain a failure, e.g. the options of
  the comparison.
- For strings, implement `IStringMatchType` instead and set it with `SetMatchType` on the `StringEqualityOptions`.

## Nested expectations

An expectation that lets the caller continue with expectations on a part of the subject, like
`Throws().WithInner<T>(e => …)`, selects the part with `ForMember` and adds the caller's expectations to it:

```csharp
public static AndOrResult<Track, IThat<Track?>> HasTitle(
    this IThat<Track?> subject,
    Action<IThatSubject<string?>> expectations)
    => new(subject.Get().ExpectationBuilder
            .ForMember(
                MemberAccessor<Track?, string?>.FromFunc(track => track?.Title, "title "),
                (member, stringBuilder) => stringBuilder.Append("has ").Append(member).Append("that "))
            .AddExpectations(e => expectations(new ThatSubject<string?>(e)),
                grammars => grammars | ExpectationGrammars.Nested),
        subject);
```

`await Expect.That(track).HasTitle(title => title.StartsWith("Let"))` then reads "has title that starts with …".

- The second argument of `ForMember` writes the text in front of the nested expectations.
- The name of the member replaces `it` in their result texts ("title was …"), unless you pass `replaceIt: false`.
- The function passed to `AddExpectations` sets the grammars of the nested expectations.
- `Validate(…)` before `AddExpectations` adds a constraint on the subject itself, e.g. based on
  `ConstraintResult.WithNotNullValue<T>` to rule out a `null` subject, as `WithInner` does.
- An exception that the member selector throws fails the nested expectations with "… did throw …".
- `ForAsyncMember` does the same for a member that has to be awaited.

## Expectations on collection items

An expectation on the items of a collection, i.e. an extension method on `IEnumerableElements<TItem>`, which
`All()`, `AtLeast(2)` and the other quantifiers return, derives from `QuantifiedCollectionConstraint<TValue, TItem>`.
`IEnumerableElements<TItem>` gives access to the quantifier and the subject:

```csharp
using aweXpect.Core.EvaluationContext;
using aweXpect.Options;

public static AndOrResult<IEnumerable<Track>, IThat<IEnumerable<Track>?>> AreRadioFriendly(
    this IEnumerableElements<Track> elements)
{
    return new(elements.Subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
            => new AreRadioFriendlyConstraint(it, grammars, elements.Quantifier)),
        elements.Subject);
}

private sealed class AreRadioFriendlyConstraint(
    string it,
    ExpectationGrammars grammars,
    EnumerableQuantifier quantifier)
    : QuantifiedCollectionConstraint<IEnumerable<Track>?, Track>(it, grammars, quantifier,
            g => g.IsPlural() ? "are radio friendly" : "is radio friendly", "were"),
        IContextConstraint<IEnumerable<Track>?>
{
    public ConstraintResult IsMetBy(IEnumerable<Track>? actual, IEvaluationContext context)
    {
        StartEvaluation();
        Actual = actual;
        if (actual is not null)
        {
            foreach (Track track in context.UseMaterializedEnumerable(actual))
            {
                Record(track, track.Duration <= TimeSpan.FromMinutes(3));
            }

            Complete();
        }

        return this;
    }
}
```

```csharp
Track[] tracks = [new("Love Me Do", new TimeSpan(0, 2, 22)), new("She Loves You", new TimeSpan(0, 2, 21))];

await Expect.That(tracks).All().AreRadioFriendly();
```

- `StartEvaluation` forgets the items of an earlier evaluation, as the constraint is evaluated again, e.g. by
  `Eventually()` or for each item of an outer collection.
- `Record` classifies an item as matching or not matching, and `Complete` decides the outcome from the quantifier.
- The expectation text is for a single item and is never negated, as the quantifier carries the negation. The
  `Plural` grammar asks for the plural form, e.g. in "whose Tracks are radio friendly for at least 2 items".
- The verb completes the result, e.g. "but only 1 of 3 were".

The base class renders the quantifier like the built-in `Satisfy`, also when negated or nested, and adds the matching
or not matching items as [context](03-message-conventions.md#contexts). When the items are verified by nested
expectations, record the result of each item with `Record(item, itemResult)`, so that the contexts of the item that
explains the failure are shown as well, labelled with its index, e.g. `Actual (item [2]):`. `DoesNotComplyWith(t => t.AtLeast(2).AreRadioFriendly())` then reads "is radio
friendly for fewer than 2 items, but 3 of 3 were", followed by the "Matching items". Unlike the built-in expectations,
it does not add the "Collection" context.

- The other collections have their own interfaces: `IEnumerableStringElements` for strings,
  `INonGenericEnumerableElements<TEnumerable>` for a non-generic `IEnumerable`,
  `IStructEnumerableElements<TEnumerable, TItem>` and `IStructEnumerableStringElements<TEnumerable>` for an
  `ImmutableArray`, and `IAsyncEnumerableElements<TItem>` and `IAsyncEnumerableStringElements` for an
  `IAsyncEnumerable`.
- Call `CompleteEarly()` instead of `Complete()` as soon as `IsDetermined`, to stop reading the items once they
  decide the outcome.
- When the item expectation or the verb are only known while the failure message is created, e.g. because they come
  from nested expectations, derive from `QuantifiedCollectionConstraintBase<TValue, TItem>` and implement
  `AppendItemExpectation` and `Verb` instead.
- An item result with `Outcome.FailureBothWays` is neither matching nor not matching, e.g. because the nested
  expectations threw. Like the built-in `ComplyWith`, `Record(item, itemResult)` stops at such an item and fails with
  its result and the contexts of the item, also when negated. An item result with `Outcome.Undecided`, e.g. after a
  canceled evaluation, leaves the outcome undecided. Neither changes an outcome that the items before already
  determine: `IsDetermined` is then `true`, and the remaining items are not counted.

### Nested expectations on items

When the caller passes expectations for the items, like the built-in `ComplyWith`, evaluate them with a
`ManualExpectationBuilder<TItem>` and record the result of each item:

```csharp
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.EvaluationContext;
using aweXpect.Options;

public static AndOrResult<IEnumerable<Track>, IThat<IEnumerable<Track>?>> AreTracksThat(
    this IEnumerableElements<Track> elements, Action<IThatSubject<Track>> expectations)
    => new(elements.Subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
            => new AreTracksThatConstraint(it, grammars, elements.Quantifier, expectations)),
        elements.Subject);

private sealed class AreTracksThatConstraint
    : QuantifiedCollectionConstraintBase<IEnumerable<Track>?, Track>,
        IAsyncContextConstraint<IEnumerable<Track>?>,
        IExpectationTextConstraint
{
    private readonly ManualExpectationBuilder<Track> _itemExpectations;

    public AreTracksThatConstraint(string it, ExpectationGrammars grammars, EnumerableQuantifier quantifier,
        Action<IThatSubject<Track>> expectations)
        : base(it, grammars, quantifier)
    {
        _itemExpectations = new ManualExpectationBuilder<Track>(grammars);
        expectations(new ThatSubject<Track>(_itemExpectations));
    }

    protected override string Verb => _itemExpectations.GetResultVerb();

    public async ValueTask<ConstraintResult> IsMetBy(IEnumerable<Track>? actual, IEvaluationContext context,
        CancellationToken cancellationToken)
    {
        StartEvaluation();
        Actual = actual;
        await _itemExpectations.PrepareExpectation(context, cancellationToken);
        if (actual is not null)
        {
            foreach (Track track in context.UseMaterializedEnumerable(actual))
            {
                Record(track, await _itemExpectations.IsMetBy(track, context, cancellationToken));
            }

            Complete();
        }

        return this;
    }

    public async ValueTask<ConstraintResult> GetExpectationResult(IEvaluationContext context,
        CancellationToken cancellationToken)
    {
        await _itemExpectations.PrepareExpectation(context, cancellationToken);
        return this;
    }

    protected override void AppendItemExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars,
        string? indentation)
        => _itemExpectations.AppendExpectation(stringBuilder, indentation);

    protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
    {
        base.AppendNormalExpectation(stringBuilder, indentation);
        _itemExpectations.AppendReasons(stringBuilder);
    }

    protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
    {
        base.AppendNegatedExpectation(stringBuilder, indentation);
        _itemExpectations.AppendReasons(stringBuilder);
    }
}
```

```csharp
Track[] playlist = [new("Love Me Do", new TimeSpan(0, 2, 22)), new("She Loves You", new TimeSpan(0, 2, 21))];

await Expect.That(playlist).All().AreTracksThat(track => track.IsRadioFriendly());
```

- `PrepareExpectation` writes the text of the nested expectations without evaluating them, so that it is complete
  also when no item is evaluated, e.g. for an empty or a `null` collection: otherwise a nested `DoesNotComplyWith` is
  shown without its negation, and the reasons that must be awaited are missing. Call it at the start of each
  evaluation and in `GetExpectationResult` of `IExpectationTextConstraint`, which is called when only the expectation
  text is needed.
- `AppendReasons` adds the reasons of the nested expectations after the quantifier, e.g. "for all items, because …".
- Each item is evaluated with its own values in the `IEvaluationContext`, while the materialized collections are
  shared with the whole evaluation.

## Asynchronous constraints

An `IAsyncConstraint<T>` receives the `CancellationToken` of the expectation, which is canceled when the timeout
(`WithTimeout`) elapses or the caller cancels (`WithCancellation`). Pass it on to the asynchronous work:

```csharp no-compile
public async ValueTask<ConstraintResult> IsMetBy(Track? actual, CancellationToken cancellationToken)
{
    Actual = actual;
    if (actual is not null)
    {
        Outcome = await IsInCatalogAsync(actual, cancellationToken) ? Outcome.Success : Outcome.Failure;
    }

    return this;
}
```

- An `OperationCanceledException` from that token needs no handling: a timeout fails the expectation with "did not
  finish within …", and a cancellation by the caller leaves it inconclusive.
- A constraint that stops at the cancellation without throwing leaves its `Outcome` undecided. A timeout then fails the
  expectation in the same way. After a cancellation, the helper classes write the result text with
  `AppendUndecidedResult`, which you can override. By default it writes "it could not be verified, because the
  evaluation was already canceled".
- Leave the `Outcome` undecided only for a cancellation. An expectation whose outcome stays undecided although nothing
  was canceled, e.g. because one branch of `IsMetBy` does not set it or a collection constraint does not call
  `Complete()`, fails with "it could not be verified, because the expectation did not decide its outcome".

### Repeated checks

An expectation that returns a `RepeatedCheckResult<TType, TThat>` lets the caller wait for a condition with
`.Within(timeout)` and `.CheckEvery(interval)`, like `Satisfies` does. Pass the same `RepeatedCheckOptions` to the
result and to the constraint, and make the check with `CheckRepeatedly`, which honours the options. For a player that
starts playing asynchronously:

```csharp
public class Player
{
    public bool IsPlaying { get; private set; }

    public void Play(string title) => IsPlaying = true;
}
```

```csharp
using aweXpect.Core.EvaluationContext;
using aweXpect.Options;

public static RepeatedCheckResult<Player, IThat<Player?>> IsPlaying(this IThat<Player?> subject)
{
    RepeatedCheckOptions options = new();
    return new RepeatedCheckResult<Player, IThat<Player?>>(subject.Get().ExpectationBuilder
            .AddConstraint((it, grammars) => new IsPlayingConstraint(it, grammars, options)),
        subject,
        options);
}

private sealed class IsPlayingConstraint(
    string it,
    ExpectationGrammars grammars,
    RepeatedCheckOptions options)
    : ConstraintResult.WithNotNullValue<Player>(it, grammars),
        IAsyncContextConstraint<Player?>
{
    public async ValueTask<ConstraintResult> IsMetBy(Player? actual, IEvaluationContext context,
        CancellationToken cancellationToken)
    {
        Actual = actual;
        if (actual is not null)
        {
            Outcome outcome = await options.CheckRepeatedly(_ =>
            {
                bool isPlaying = actual.IsPlaying;
                Outcome = isPlaying ? Outcome.Success : Outcome.Failure;
                return new ValueTask<bool>(isPlaying != IsNegated);
            }, context);
            if (outcome == Outcome.Undecided)
            {
                Outcome = Outcome.Undecided;
            }
        }

        return this;
    }

    protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append("is playing").Append(options);

    protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append(It).Append(" was not playing");

    protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append("is not playing").Append(options);

    protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append(It).Append(" was playing");
}
```

```csharp
Player player = new();
_ = Task.Delay(TimeSpan.FromSeconds(1)).ContinueWith(_ => player.Play("Let It Be"));

await Expect.That(player).IsPlaying().Within(TimeSpan.FromSeconds(5)).CheckEvery(TimeSpan.FromMilliseconds(100));
```

- `CheckRepeatedly` makes the first check immediately. When `IsRepeated` is `true`, because `Within` set a positive
  or an infinite timeout, it repeats the check in the interval until it succeeds, and makes the last check at the
  timeout.
- The check returns whether the expectation is met, so for a negated variant created with `.Invert()` it returns
  `true` when the player is *not* playing. The helper class inverts the stored `Outcome` itself.
- The check receives the `IEvaluationContext` to use. The first check gets the context of the evaluation, every
  further check a new one, so that a collection taken with `UseMaterializedEnumerable` is read again instead of
  replaying the items that a previous check read.
- An exception of code of the caller called through `UserCode.Invoke` counts as not met while the check is repeated.
  When the last check still throws, the expectation fails with that exception.
- Appending the options writes " within …" to the expectation text when `Within` was specified.
- `CheckRepeatedly` returns the `Outcome`: `Success` when a check succeeded, `Failure` when the last check failed, and
  `Undecided` when the evaluation was canceled before the timeout. The cancellation is only observed while waiting
  for the next check, so the first check is made even when the evaluation is already canceled.
- A cancellation at the timeout, or by an effective timeout of the evaluation (`WithTimeout` or
  `TestCancellation.FromTimeout`) that is not shorter than `Within`, lets the last check decide. For `Undecided`, the
  constraint sets its `Outcome` to `Undecided`: the helper class then reports that the expectation could not be
  verified, and a shorter effective timeout is reported as "did not finish within …".
