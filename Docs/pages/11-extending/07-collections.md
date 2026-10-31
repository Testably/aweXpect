---
description: Write expectations on collections and their items that read the subject only once and use the quantifiers of aweXpect.
---

# Collections

An expectation on a collection reads the subject only once, also together with other expectations, and can verify its
items with the quantifiers of aweXpect, such as `All()` or `AtLeast(2)`.

<details>
<summary>Namespaces used on this page</summary>

```csharp
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Formatting;
using aweXpect.Options;
using aweXpect.Results;
using static aweXpect.Formatting.Format;
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

`await That(tracks).IsNotEmpty().And.HasRadioFriendlyTracks(3)` then enumerates `tracks` only once.

For an `IAsyncEnumerable<T>`, implement `IAsyncContextConstraint<T>` and call `UseMaterializedAsyncEnumerable` with its
`CancellationToken`.

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

## Expectations on collection items

An expectation on the items of a collection, i.e. an extension method on `IEnumerableItems<TItem>`, the items that
`All()`, `AtLeast(2)` and the other quantifiers select, derives from `QuantifiedCollectionConstraint<TCollection, TItem>`.
`IEnumerableItems<TItem>` gives access to the quantifier and the subject. The built-in expectations on the items, like
`Satisfy` or `AreEqualTo`, are such extension methods as well. For a condition on a single item,
[`[GenerateCollectionExpectation]`](./01-generated-expectations.md#collections) generates such an expectation.

```csharp
public static AndOrResult<IEnumerable<Track>, IThat<IEnumerable<Track>?>> AreRadioFriendly(
    this IEnumerableItems<Track> items)
{
    return new(items.Subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
            => new AreRadioFriendlyConstraint(it, grammars, items.Quantifier)),
        items.Subject);
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

await That(tracks).All().AreRadioFriendly();
```

- `StartEvaluation` forgets the items of an earlier evaluation, as the constraint is evaluated again, e.g. by
  `Eventually()` or for each item of an outer collection.
- `Record` classifies an item as matching or not matching, and `Complete` decides the outcome from the quantifier.
- The expectation text is for a single item and is never negated, as the quantifier carries the negation. The
  `Plural` grammar asks for the plural form, e.g. in "whose Tracks are radio friendly for at least 2 items".
- The verb completes the result, e.g. "but only 1 of 3 were".
- The other collections have their own interfaces: `INonGenericEnumerableItems<TCollection>` for a non-generic
  `IEnumerable`, `IStructEnumerableItems<TCollection, TItem>` for an `ImmutableArray` and
  `IAsyncEnumerableItems<TItem>` for an `IAsyncEnumerable`. Strings have no interface of their own: an expectation
  only for strings extends `IEnumerableItems<string?>`.
- The interfaces are covariant in `TItem`, so an expectation on the items of `Track?` also applies to the items of a
  `List<Track>` or of a collection of a type derived from `Track`. Extend `IEnumerableItems<Track?>` to cover both
  nullabilities.

The base class renders the quantifier like the built-in `Satisfy`, also when negated or nested, and adds the matching
or not matching items as [context](./03-message-conventions.md#contexts).
`DoesNotComplyWith(t => t.AtLeast(2).AreRadioFriendly())` then reads "is radio friendly for fewer than 2 items, but 3
of 3 were", followed by the "Matching items". Unlike the built-in expectations, it does not add the "Collection"
context.

<details>
<summary>Early completion, nested item results, the base class and overloads</summary>

- The quantifiers return the derived `IQuantifiedEnumerableItems<TItem>`, which only adds the type checks
  `Are<TType>()` and `AreExactly<TType>()`, whose type argument cannot be inferred.
- Overload resolution prefers an expectation on `IEnumerableItems<string?>` over one on `IEnumerableItems<TItem>`.
- Call `CompleteEarly()` instead of `Complete()` as soon as `IsDetermined`, to stop reading the items once they
  decide the outcome.
- When the items are verified by nested expectations, record the result of each item with `Record(item, itemResult)`, so that the contexts of the item that explains the failure are shown as well, labeled with its index, e.g. `Actual (item [2]):`.
- An item result with `Outcome.FailureBothWays` is neither matching nor not matching, e.g. because the nested
  expectations threw. Like the built-in `ComplyWith`, `Record(item, itemResult)` stops at such an item and fails with
  its result and the contexts of the item, also when negated. An item result with `Outcome.Undecided`, e.g. after a
  canceled evaluation, leaves the outcome undecided. Neither changes an outcome that the items before already
  determine: `IsDetermined` is then `true`, and the remaining items are not counted.
- When the item expectation or the verb are only known while the failure message is created, e.g. because they come
  from nested expectations, derive from `QuantifiedCollectionConstraintBase<TCollection, TItem>` and implement
  `AppendItemExpectation` and `Verb` instead.

</details>

### Nested expectations on items

When the caller passes expectations for the items, like the built-in `ComplyWith`, evaluate them with a
`ManualExpectationBuilder<TItem>` and record the result of each item:

```csharp
public static AndOrResult<IEnumerable<Track>, IThat<IEnumerable<Track>?>> AreTracksThat(
    this IEnumerableItems<Track> items, Action<IThatSubject<Track>> expectations)
    => new(items.Subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
            => new AreTracksThatConstraint(it, grammars, items.Quantifier, expectations)),
        items.Subject);

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

await That(playlist).All().AreTracksThat(track => track.IsRadioFriendly());
```

- `PrepareExpectation` writes the text of the nested expectations without evaluating them, so that it is complete
  also when no item is evaluated, e.g. for an empty or a `null` collection: otherwise a nested `DoesNotComplyWith` is
  shown without its negation, and the reasons that must be awaited are missing. Call it at the start of each
  evaluation and in `GetExpectationResult` of `IExpectationTextConstraint`, which is called when only the expectation
  text is needed.
- `AppendReasons` adds the reasons of the nested expectations after the quantifier, e.g. "for all items, because …".

<details>
<summary>The evaluation context of each item</summary>

Each item is evaluated with its own values in the `IEvaluationContext`, while the materialized collections are shared
with the whole evaluation.

</details>

## Expected collections

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

### Matching collections

To compare items with an expected collection like the collection `IsEqualTo` does, get a matcher from
`CollectionMatchOptions` and let `VerifyItems` run it over the items. It returns the outcome together with the failure
text of `IsEqualTo`, e.g. `it lacked 1 of 2 expected items: "Outro"`, so the caller's choice of `InAnyOrder()`,
`IgnoringDuplicates()` or `IgnoringCase()` applies to your expectation as well:

```csharp
public static StringCollectionMatchResult<IEnumerable<Track>, IThat<IEnumerable<Track>?>> HasTitles(
    this IThat<IEnumerable<Track>?> subject, params string[] expected)
{
    OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default);
    OptionsHolder<CollectionMatchOptions> matchOptions = new(CollectionMatchOptions.Default);
    return new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
            => new HasTitlesConstraint(it, grammars, expected, options, matchOptions)),
        subject, options, matchOptions);
}

private sealed class HasTitlesConstraint(
    string it,
    ExpectationGrammars grammars,
    string[] expected,
    IReadOnlyOptionsHolder<StringEqualityOptions> options,
    IReadOnlyOptionsHolder<CollectionMatchOptions> matchOptions)
    : ConstraintResult.WithNotNullValue<IEnumerable<Track>>(it, grammars),
        IAsyncContextConstraint<IEnumerable<Track>?>
{
    private string? _failure;

    public async ValueTask<ConstraintResult> IsMetBy(IEnumerable<Track>? actual, IEvaluationContext context,
        CancellationToken cancellationToken)
    {
        Actual = actual;
        _failure = null;
        foreach (string title in expected)
        {
            options.Value.ValidateExpected(title, nameof(expected));
        }

        if (actual is null)
        {
            Outcome = Outcome.Failure;
            return this;
        }

        IEnumerable<string?> titles = context.UseMaterializedEnumerable(actual).Select(track => track.Title);
        (Outcome, _failure) = await matchOptions.Value.GetCollectionMatcher<string?, string?>(expected)
            .VerifyItems(It, titles, options.Value.ForEvaluation(context, cancellationToken), cancellationToken);
        return this;
    }

    protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append("has titles that ").Append(matchOptions.Value.GetExpectation(
            Formatter.Format(expected, FormattingOptions.SingleLine) + options.Value, ExpectationGrammars.Plural));

    protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append(_failure);

    protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append("has titles that ").Append(matchOptions.Value.GetExpectation(
            Formatter.Format(expected, FormattingOptions.SingleLine) + options.Value,
            ExpectationGrammars.Plural | ExpectationGrammars.Negated));

    protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append(It).Append(" did");
}
```

`await That(tracks).HasTitles("Intro", "Outro").InAnyOrder()` then fails with e.g.
`but it contained item "Bonus" at index 1 that was not expected`.

Validate every expected item with `ValidateExpected(item, nameof(expected))` of the options before you look at the
subject, as above, so that an item the options cannot use, e.g. an invalid regex pattern after `AsRegex()`, throws
whichever items the subject has. A [custom match type](./06-comparisons.md#custom-match-types) decides which items it
rejects.

<details>
<summary>How the items are verified</summary>

- The items are verified one by one until the matcher fails or is determined, e.g. once all expected items of a
  containment are found, and then completed. While the matcher and the comparison answer synchronously, the result
  completes synchronously, too.
- An exception while the items are enumerated fails the expectation like one of the
  [code of the caller](./02-constraints-and-results.md#code-of-the-caller).
- The outcome stays `Undecided` when the `CancellationToken` is canceled before all items are read from their source.
  A collection that is complete in memory, e.g. an array, is verified completely.
- `VerifyItems` takes an `IEnumerable<T>`, so read the items of an `IAsyncEnumerable<T>` into a list first.

</details>
