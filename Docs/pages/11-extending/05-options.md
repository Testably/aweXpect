# Options

The options, such as `.AtLeast(2)`, `.IgnoringCase()` or `.InAnyOrder()`, are extension methods in the `aweXpect`
namespace. They apply to every result that provides the matching options and follow the
[general rules for options](../03-how-it-works/05-options.md).

<details>
<summary>Namespaces used on this page</summary>

```csharp
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Formatting;
using aweXpect.Options;
using aweXpect.Results;
using static aweXpect.Formatting.Format;
```

</details>

## An expectation with options

An expectation with options creates a holder for them and passes it to its constraint and to its result. The caller
sets the options on the result, and the constraint reads them when it is evaluated. For example, a `HasTitle` that
returns a `StringEqualityResult`, which offers the string options such as `IgnoringCase()`:

```csharp
public static StringEqualityResult<Track, IThat<Track?>> HasTitle(this IThat<Track?> subject, string expected)
{
    OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default);
    return new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
            => new HasTitleConstraint(it, grammars, expected, options)),
        subject,
        options);
}

private sealed class HasTitleConstraint(
    string it,
    ExpectationGrammars grammars,
    string expected,
    IReadOnlyOptionsHolder<StringEqualityOptions> options)
    : ConstraintResult.WithNotNullValue<Track>(it, grammars),
        IAsyncContextConstraint<Track?>
{
    public async ValueTask<ConstraintResult> IsMetBy(Track? actual, IEvaluationContext context,
        CancellationToken cancellationToken)
    {
        Actual = actual;
        if (actual is not null)
        {
            bool isEqual = await options.Value.ForEvaluation(context, cancellationToken)
                .AreConsideredEqual(actual.Title, expected, nameof(expected));
            Outcome = isEqual ? Outcome.Success : Outcome.Failure;
        }

        return this;
    }

    protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append("has title ").Append(options.Value.GetExpectation(expected, ExpectationGrammars.None));

    protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
    {
        stringBuilder.Append(It).Append(" had title ");
        Formatter.Format(stringBuilder, Actual?.Title);
    }

    protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append("does not have title ")
            .Append(options.Value.GetExpectation(expected, ExpectationGrammars.None));
}
```

```csharp
Track track = new("Let It Be", new TimeSpan(0, 4, 3));

await Expect.That(track).HasTitle("let it be").IgnoringCase();
```

- `IgnoringCase()` is called after the constraint was added, so the constraint reads `options.Value` only in
  `IsMetBy` and in its texts.
- The string options write the expectation text with the options, e.g. "has title equal to "let it be" ignoring
  case".
- The constraint compares with the options of its evaluation, see
  [comparing with the options](./06-comparisons.md#comparing-with-the-options).

## One holder per expectation

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

<details>
<summary>Creating the holder only when an option is set</summary>

Most callers set no option, so the holder is often created for nothing. When only your constraint and your result
share the holder, the constraint can create it on demand: it implements `IOptionsHolderSource<TOptions>`, compares with
the default options until the result asks for its `Holder`, and you pass the constraint to the result instead of a
holder. `AddConstraint(state, constraintBuilder, out constraint)` returns the added constraint for this. The results
take an `IOptionsHolderSource<TOptions>`, which a holder implements as well, so both ways work with the same results:

```csharp
public static StringEqualityResult<Track, IThat<Track?>> IsTitled(this IThat<Track?> subject, string expected)
    => new(subject.Get().ExpectationBuilder.AddConstraint(expected,
            static (expected, it, grammars) => new IsTitledConstraint(it, grammars, expected),
            out IsTitledConstraint constraint),
        subject,
        constraint);

private sealed class IsTitledConstraint(string it, ExpectationGrammars grammars, string expected)
    : ConstraintResult.WithNotNullValue<Track>(it, grammars),
        IAsyncContextConstraint<Track?>,
        IOptionsHolderSource<StringEqualityOptions>
{
    private OptionsHolder<StringEqualityOptions>? _holder;

    OptionsHolder<StringEqualityOptions> IOptionsHolderSource<StringEqualityOptions>.Holder
        => _holder ??= new OptionsHolder<StringEqualityOptions>(StringEqualityOptions.Default);

    private StringEqualityOptions Options => _holder?.Value ?? StringEqualityOptions.Default;

    public async ValueTask<ConstraintResult> IsMetBy(Track? actual, IEvaluationContext context,
        CancellationToken cancellationToken)
    {
        Actual = actual;
        if (actual is not null)
        {
            bool isEqual = await Options.ForEvaluation(context, cancellationToken)
                .AreConsideredEqual(actual.Title, expected, nameof(expected));
            Outcome = isEqual ? Outcome.Success : Outcome.Failure;
        }

        return this;
    }

    protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append("is titled ").Append(Options.GetExpectation(expected, ExpectationGrammars.None));

    protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
    {
        stringBuilder.Append(It).Append(" had title ");
        Formatter.Format(stringBuilder, Actual?.Title);
    }

    protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append("is not titled ").Append(Options.GetExpectation(expected, ExpectationGrammars.None));
}
```

Keep creating the holder in the expectation when something else that is built with the expectation reads it as well,
e.g. the item matcher of a collection expectation: it would not see a holder that the constraint creates later.

</details>

## Setting options in your own code

You can also set the options of `aweXpect.Core` in your own code, e.g. in an option of your own. Each method of an
options value that sets an option returns a new value with the option set, e.g. `Quantifier.Default.AtLeast(2)`, and
the same methods are extension methods on the holder in the `aweXpect.Options` namespace, which replace the value in
the holder and return the holder, so that calls chain, e.g. `result.Options.IgnoringCase().AsPrefix()`. The members that only
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

The [general rules for options](../03-how-it-works/05-options.md) apply to the options of `aweXpect.Core` as well as to
those of your own. For an extension, these details matter as well:

- Options that set the same thing in different ways throw an `InvalidOperationException` at the call, with a message
  like `AtMost cannot be combined with AtLeast.` or `AsSuffix cannot be combined with AsPrefix.`
- The hooks for a strategy of your own are named `Set…` and take the name of your option, e.g.
  `SetMatchType(matchType, nameof(AsTitle))` on the `StringEqualityOptions` or `SetMatch(match, nameof(AtTrackNumber))`
  on the `CollectionIndexOptions`, so that a competing option throws with that name, while the same name again
  replaces the earlier strategy. An [object match type](./06-comparisons.md#custom-match-types) carries its name itself, and
  `SetComparer` on the `CollectionOrderOptions` takes no name, as no other option sets the order.
- A hook that combines with the earlier calls instead of replacing them, like the predicates of a signal or event
  filter, is named `Add…`, e.g. `AddPredicate`.

## Options of your own

Follow the same rules for an options type of your own: an immutable value with a `Default`, whose option methods
return a new value, and an extension method on its holder per option. For example, a playlist expectation can ignore
the bonus tracks and compare in album order or shuffled, but not both:

```csharp
[ImmutableOptions]
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
public static TSelf IgnoringBonusTracks<TSelf>(this TSelf result, bool ignoreBonusTracks = true)
    where TSelf : IOptionsProvider<PlaylistOptions>
{
    result.Options.IgnoringBonusTracks(ignoreBonusTracks);
    return result;
}
```

Mark the options type with `[ImmutableOptions]`, so that the analyzer rule
[aweXpect0009](../08-analyzers.md#awexpect0009) reports a call whose returned options are discarded, also for an
option method that is an extension method on the type. `Default` is the value of `default(PlaylistOptions)`, so that
an options field that was never set is valid. Throw
with `Tracing.WriteException`, so that it is written to the trace writer of
`Customize.aweXpect.EnableTracing(…)` like the built-in exceptions, and word the message like the built-in ones. When the expectation text names the options, write that text in `ToString()`: an
`OptionsHolder<TOptions>` returns the same text, so that the expectation can append the holder itself.

## Results that are stored

A result that can be stored and continued more than once, e.g. a filter of `aweXpect.Reflection`, must not change when
a continuation sets an option. Such a result implements `ICopyOnOption<StringEqualityOptions, TSelf>` in addition to
`IOptionsProvider<StringEqualityOptions>`: the string options then pass its options with the new option to
`WithOptions(options)`, which returns a new result with them in a new holder, while the result they were called on stays
unchanged. The quantifiers, e.g. `Once()`, and the index options `AtIndex(…)` and
`AtIndexFromEnd(…)` do the same for a result that implements `ICopyOnOption<Quantifier, TSelf>` or
`ICopyOnOption<CollectionIndexOptions, TSelf>`. A string option of your own, e.g. a preset of several options, passes
the new options to `OptionsProvider.WithOptions(result, options)`, so that it keeps a stored result unchanged as well:
it returns the copy of a result that implements `ICopyOnOption<…>` and sets the options in the holder of any other
result.

```csharp
public static TSelf IgnoringCaseAndIndentation<TSelf>(this TSelf result)
    where TSelf : IOptionsProvider<StringEqualityOptions>
    => OptionsProvider.WithOptions(result, result.Options.Value.IgnoringCase().IgnoringIndentation());
```

<details>
<summary>Options on a stored result</summary>

The new options carry every option set so far, so a repeated option replaces the earlier one and a competing option
still throws. Like every constraint, such a result reads its holder when it evaluates: an option that sets the holder
directly, without `WithOptions`, still applies, but it changes the result it was called on.

</details>

## Time tolerances

A `TimeToleranceResult<TType, TThat>` adds `.Within(…)` and stores the tolerance in the holder of the `TimeTolerance` options
you pass to it and to your constraint; start the holder with `TimeTolerance.Default`. For a date without a time of day,
such as the release date of an album as a `DateOnly`, start it with `TimeTolerance.Days` instead, as the built-in
expectations do.

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
