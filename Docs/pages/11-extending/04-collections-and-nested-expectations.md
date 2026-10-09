# Collections and nested expectations

<details>
<summary>Namespaces used on this page</summary>

```csharp
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Equivalency;
using aweXpect.Options;
using aweXpect.Results;
```

</details>

## Collection subjects

A constraint that enumerates a collection subject gets it from the `IEvaluationContext` with
`UseMaterializedEnumerable` instead of enumerating the subject itself. All expectations on the subject, including the
built-in ones, then share one lazily materialized copy, so that a subject which can only be enumerated once, e.g. a
query or an iterator with side effects, is enumerated at most once, also when the expectations are combined with
`.And` or `.Or`. Implement `IContextConstraint<T>` to receive the context:

```csharp
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

For an `IAsyncEnumerable<T>`, implement `IAsyncContextConstraint<T>` and call `UseMaterializedAsyncEnumerable` with its
`CancellationToken`. This method is only available on .NET 8 or later.

<details>
<summary>How the materialized collection behaves</summary>

- Every call for the same subject in the same evaluation returns the same sequence. It reads the subject only as far
  as it is enumerated, and a further enumeration replays the items read so far before it continues the subject.
  Once the expectation and its failure message are complete, the enumerator of the subject is disposed, also when it
  was not read to its end.
- A subject that already is a collection is returned unchanged. For any other subject, an exception while it is
  enumerated fails the expectation like one of the
  [code of the caller](./02-constraints-and-results.md#code-of-the-caller).
- An `IAsyncEnumerable<T>` stays governed by the token of the first call to `UseMaterializedAsyncEnumerable`.

</details>

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

A result that continues with a new subject after the expectation, like `HasSingle().Which`, uses `ForWhich` instead.
It selects the new subject from the value of the expectation, and the expectations that the caller adds to the
returned `ThatSubject<T>` apply to it:

```csharp
public class LongestTrackResult(ExpectationBuilder expectationBuilder, IThat<Track[]?> returnValue)
    : AndOrResult<Track[], IThat<Track[]?>>(expectationBuilder, returnValue)
{
    public IThat<Track?> Which
        => new ThatSubject<Track?>(ExpectationBuilder.ForWhich<Track[], Track?>(
            tracks => tracks.OrderByDescending(track => track.Duration).FirstOrDefault(),
            " whose longest track ", "longest track"));
}
```

The second argument of `ForWhich` writes the text in front of the expectations on the new subject, and the third one
replaces `it` in their result texts. Unlike `ForMember`, the expectations are added later by the caller, after `Which`.

### Expectations in expected values

An expected value can contain expectations, like `new { Title = It.Is<string>().That.StartsWith("Let") }`. Pass the
`IEvaluationContext` and the `CancellationToken` of your constraint to `EquivalencyComparison.Compare`, or to
`EquivalencyExpectationBuilder.IsMetBy` when you evaluate such an expectation yourself, so that the timeout and the
cancellation of the evaluation also end these expectations.

<details>
<summary>Example and details</summary>

```csharp
public static AndOrResult<Track, IThat<Track?>> MatchesTrack(this IThat<Track?> subject, object expected)
    => new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
            => new MatchesTrackConstraint(it, grammars, expected)),
        subject);

private sealed class MatchesTrackConstraint(string it, ExpectationGrammars grammars, object expected)
    : ConstraintResult.WithNotNullValue<Track>(it, grammars),
        IAsyncContextConstraint<Track?>
{
    private readonly StringBuilder _differences = new();

    public async ValueTask<ConstraintResult> IsMetBy(Track? actual, IEvaluationContext context,
        CancellationToken cancellationToken)
    {
        Actual = actual;
        if (actual is not null)
        {
            _differences.Clear();
            bool isEquivalent = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(),
                _differences, context, cancellationToken);
            Outcome = isEquivalent ? Outcome.Success : Outcome.Failure;
        }

        return this;
    }

    protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append("matches the expected track");

    protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append(It).Append(" did not:").Append(_differences);

    protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append("does not match the expected track");

    protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append(It).Append(" did");
}
```

```csharp
Track track = new("Let It Be", new TimeSpan(0, 4, 3));

await Expect.That(track).MatchesTrack(new { Title = It.Is<string>().That.StartsWith("Let") });
```

When your constraint evaluates such an expectation itself, e.g. because it compares a format of its own, take the
`EquivalencyExpectationBuilder` from the expectation and evaluate it with the context and the token of your constraint:

```csharp no-compile
if (expectedValue is IOptionsProvider<ExpectationBuilder> { Options: EquivalencyExpectationBuilder expectation, })
{
    ConstraintResult result = await expectation.IsMetBy(actualValue, context, cancellationToken);
    if (result.Outcome != Outcome.Success)
    {
        result.AppendResult(_differences);
    }
}
```

- `IsMetBy` evaluates the expectation in a context of its own, which the timeout and the cancellation of your
  evaluation end. The collections it materializes, the reasons it leaves pending and the values it stores do not reach
  your context, so the expectation can be evaluated again for every value it is compared with.
- The collections are released before `IsMetBy` returns, so describe the failure from the result right away: it only
  shows the items that the expectation read.
- An `Outcome.Undecided` result means that the evaluation was canceled. Leave the outcome of your constraint
  undecided as well, see [stopping without an exception](./05-asynchronous-expectations.md#asynchronous-constraints).
- After a cancellation, `EquivalencyComparison.Compare` throws an `OperationCanceledException` instead, which needs
  no handling.

</details>

## Expectations on collection items

An expectation on the items of a collection, i.e. an extension method on `IEnumerableElements<TItem>`, which
`All()`, `AtLeast(2)` and the other quantifiers return, derives from `QuantifiedCollectionConstraint<TValue, TItem>`.
`IEnumerableElements<TItem>` gives access to the quantifier and the subject:

```csharp
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
- The other collections have their own interfaces: `IEnumerableStringElements` for strings,
  `INonGenericEnumerableElements<TEnumerable>` for a non-generic `IEnumerable`,
  `IStructEnumerableElements<TEnumerable, TItem>` and `IStructEnumerableStringElements<TEnumerable>` for an
  `ImmutableArray`, and `IAsyncEnumerableElements<TItem>` and `IAsyncEnumerableStringElements` for an
  `IAsyncEnumerable`.

The base class renders the quantifier like the built-in `Satisfy`, also when negated or nested, and adds the matching
or not matching items as [context](./06-message-conventions.md#contexts).
`DoesNotComplyWith(t => t.AtLeast(2).AreRadioFriendly())` then reads "is radio friendly for fewer than 2 items, but 3
of 3 were", followed by the "Matching items". Unlike the built-in expectations, it does not add the "Collection"
context.

<details>
<summary>Early completion, nested item results and the base class</summary>

- Call `CompleteEarly()` instead of `Complete()` as soon as `IsDetermined`, to stop reading the items once they
  decide the outcome.
- When the items are verified by nested expectations, record the result of each item with `Record(item, itemResult)`,
  so that the contexts of the item that explains the failure are shown as well, labelled with its index, e.g.
  `Actual (item [2]):`.
- An item result with `Outcome.FailureBothWays` is neither matching nor not matching, e.g. because the nested
  expectations threw. Like the built-in `ComplyWith`, `Record(item, itemResult)` stops at such an item and fails with
  its result and the contexts of the item, also when negated. An item result with `Outcome.Undecided`, e.g. after a
  canceled evaluation, leaves the outcome undecided. Neither changes an outcome that the items before already
  determine: `IsDetermined` is then `true`, and the remaining items are not counted.
- When the item expectation or the verb are only known while the failure message is created, e.g. because they come
  from nested expectations, derive from `QuantifiedCollectionConstraintBase<TValue, TItem>` and implement
  `AppendItemExpectation` and `Verb` instead.

</details>

### Nested expectations on items

When the caller passes expectations for the items, like the built-in `ComplyWith`, evaluate them with a
`ManualExpectationBuilder<TItem>` and record the result of each item.

<details>
<summary>Sample with nested expectations on the items</summary>

```csharp
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

</details>
