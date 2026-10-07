using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Helpers;

namespace aweXpect;

/// <summary>
///     The comparison of the first items and the texts of <c>StartsWith</c>, shared by the synchronous and the
///     asynchronous collections.
/// </summary>
internal abstract class StartsWithConstraintBase<TValue, TItem, TMatch>(
	string it,
	ExpectationGrammars grammars,
	string? expectedExpression,
	IEnumerable<TMatch> expectedValues,
	TMatch[] expected,
	IOptionsEquality<TMatch> options)
	: ConstraintResult.WithNotNullValue<TValue>(it, grammars)
{
	private IOptionsEquality<TMatch>? _evaluationOptions;
	private string? _expectedText;
	private TItem? _firstMismatchItem;
	private bool _foundMismatch;

	/// <summary>
	///     The expected items in the expectation text, formatted only when the text is written.
	/// </summary>
	private string ExpectedText => _expectedText ??= expectedExpression ?? Formatter.Format(expectedValues);

	/// <summary>
	///     The number of items that were compared.
	/// </summary>
	protected int Count { get; private set; }

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
		=> contexts.AddOptionsContexts(options);

	/// <summary>
	///     Starts a new evaluation in the <paramref name="context" />.
	/// </summary>
	protected void Start(IEvaluationContext context, CancellationToken cancellationToken)
	{
		_evaluationOptions = options.ForEvaluation(context, cancellationToken);
		_firstMismatchItem = default;
		_foundMismatch = false;
		Count = 0;
	}

	/// <summary>
	///     Compares the <paramref name="item" /> with the next expected item.
	/// </summary>
	protected ValueTask<bool> MatchesNext(TItem item)
	{
		TMatch expectedItem = expected[Count++];
		return CollectionItems<TItem>.TryCast(item, out TMatch matchedItem)
			? (_evaluationOptions ?? options).AreConsideredEqual(matchedItem, expectedItem)
			: new ValueTask<bool>(false);
	}

	/// <summary>
	///     Records whether the <paramref name="item" /> matched the expected item and returns the outcome, when it is
	///     decided.
	/// </summary>
	protected Outcome? Decide(TItem item, bool isMatch)
	{
		if (!isMatch)
		{
			_firstMismatchItem = item;
			_foundMismatch = true;
			return Outcome.Failure;
		}

		return expected.Length == Count ? Outcome.Success : null;
	}

	/// <summary>
	///     Appends the first items that matched the expected items.
	/// </summary>
	protected abstract void AppendMatchingItems(StringBuilder stringBuilder);

	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		stringBuilder.Append(Grammars.Verb("starts with ", "start with ")).Append(ExpectedText);
		stringBuilder.Append(options);
	}

	protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
	{
		if (_foundMismatch)
		{
			stringBuilder.Append(It).Append(" contained item ");
			Formatter.Format(stringBuilder, _firstMismatchItem);
			stringBuilder.Append(" at index ").Append(Count - 1).Append(" instead of ");
			stringBuilder.AppendExpectedItem(expected[Count - 1], options);
		}
		else
		{
			stringBuilder.Append(It).Append(" contained only ").AppendItemCount(Count).Append(" and lacked ")
				.AppendItemCount(expected.Length - Count).Append(": ");
			Formatter.Format(stringBuilder, expected.Skip(Count), FormattingOptions.MultipleLines);
		}
	}

	protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		stringBuilder.Append(Grammars.Verb("does not start with ", "do not start with "))
			.Append(ExpectedText);
		stringBuilder.Append(options);
	}

	protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
	{
		stringBuilder.Append(It).Append(" did start with ");
		AppendMatchingItems(stringBuilder);
	}
}

internal sealed class StartsWithConstraint<TEnumerable, TItem, TMatch>(
	string it,
	ExpectationGrammars grammars,
	string? expectedExpression,
	IEnumerable<TMatch> expectedValues,
	TMatch[] expected,
	IOptionsEquality<TMatch> options,
	ISubjectComparing? subjectComparing = null)
	: StartsWithConstraintBase<TEnumerable, TItem, TMatch>(it, grammars, expectedExpression, expectedValues, expected,
			options),
		IAsyncContextConstraint<TEnumerable>
	where TEnumerable : IEnumerable?
{
	private CollectionContext _collectionContext;
	private IEnumerable<TItem>? _items;

	public async ValueTask<ConstraintResult> IsMetBy(TEnumerable actual, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		_collectionContext = default;
		_items = null;
		Start(context, cancellationToken);
		Actual = actual;
		if (actual.IsDefaultImmutableArray())
		{
			return this.AsNullSubject(It);
		}

		if (actual is null)
		{
			Outcome = Outcome.Failure;
			return this;
		}

		subjectComparing?.UseComparerOf(actual);
		CollectionItems<TItem> materialized = CollectionItems<TItem>.Materialize(actual, context);
		_items = materialized.Items;
		foreach (TItem item in materialized)
		{
			if (materialized.IsCanceledBeforeTheEnd(cancellationToken))
			{
				Outcome = Outcome.Undecided;
				materialized.SetContext(ref _collectionContext, true);
				return this;
			}

			if (Decide(item, await MatchesNext(item)) is { } outcome)
			{
				if (outcome == Outcome.Failure)
				{
					materialized.SetContext(ref _collectionContext);
				}

				Outcome = outcome;
				return this;
			}
		}

		materialized.SetContext(ref _collectionContext);
		Outcome = Outcome.Failure;
		return this;
	}

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
	{
		_collectionContext.AppendTo(contexts);
		base.AppendContexts(contexts);
	}

	/// <remarks>
	///     The items of a non-generic collection are laid out by the type of the first one that is not
	///     <see langword="null" />.
	/// </remarks>
	protected override void AppendMatchingItems(StringBuilder stringBuilder)
	{
		IEnumerable<TItem> items = _items?.Take(Count) ?? [];
		Type itemType = CollectionItems<TItem>.IsTyped<TEnumerable>()
			? typeof(TItem)
			: items.Cast<object?>().GetItemType();
		Formatter.Format(stringBuilder, items, itemType.GetFormattingOption(Count));
	}
}

#if NET8_0_OR_GREATER
internal sealed class AsyncStartsWithConstraint<TItem, TMatch>(
	string it,
	ExpectationGrammars grammars,
	string? expectedExpression,
	IEnumerable<TMatch> expectedValues,
	TMatch[] expected,
	IOptionsEquality<TMatch> options)
	: StartsWithConstraintBase<IAsyncEnumerable<TItem>?, TItem, TMatch>(it, grammars, expectedExpression,
			expectedValues, expected, options),
		IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
{
	private readonly List<TItem> _foundValues = [];
	private CollectionContext _collectionContext;

	public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<TItem>? actual, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		_collectionContext = default;
		_foundValues.Clear();
		Start(context, cancellationToken);
		Actual = actual;
		if (actual is null)
		{
			Outcome = Outcome.Failure;
			return this;
		}

		IAsyncEnumerable<TItem> materializedEnumerable =
			context.UseMaterializedAsyncEnumerable<TItem>(actual, cancellationToken);
		await foreach (TItem item in materializedEnumerable.UntilCancelled(cancellationToken))
		{
			Outcome? outcome = Decide(item, await MatchesNext(item));
			if (outcome == Outcome.Failure)
			{
				_collectionContext.Set(materializedEnumerable as IMaterializedAsyncEnumerable<TItem>);
				Outcome = Outcome.Failure;
				return this;
			}

			_foundValues.Add(item);
			if (outcome == Outcome.Success)
			{
				Outcome = Outcome.Success;
				return this;
			}
		}

		if (cancellationToken.IsCanceledBeforeTheEndOf(materializedEnumerable))
		{
			Outcome = Outcome.Undecided;
			_collectionContext.Set(materializedEnumerable as IMaterializedAsyncEnumerable<TItem>, true);
			return this;
		}

		_collectionContext.Set(materializedEnumerable as IMaterializedAsyncEnumerable<TItem>);
		Outcome = Outcome.Failure;
		return this;
	}

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
	{
		_collectionContext.AppendTo(contexts);
		base.AppendContexts(contexts);
	}

	protected override void AppendMatchingItems(StringBuilder stringBuilder)
		=> Formatter.Format(stringBuilder, _foundValues, typeof(TItem).GetFormattingOption(_foundValues.Count));
}
#endif
