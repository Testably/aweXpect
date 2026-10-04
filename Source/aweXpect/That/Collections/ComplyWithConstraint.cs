using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Helpers;
using aweXpect.Options;

namespace aweXpect;

/// <summary>
///     The constraint of a quantified <c>ComplyWith</c>, which classifies each item by the item expectations.
/// </summary>
/// <remarks>
///     In a nested expectation, the quantifier is the subject of the item expectations, so their verb agrees with its
///     number (<c>none start with …</c>, <c>at least one starts with …</c>). The negation can change this number, so
///     the negated text then comes from a second set of item expectations, which is only used for the text.
/// </remarks>
internal abstract class ComplyWithConstraintBase<TValue, TItem>
	: QuantifiedCollectionConstraintBase<TValue, TItem>,
		IExpectationTextConstraint
{
	private readonly ManualExpectationBuilder<TItem> _builder;
	private CollectionContext _collectionContext;
	private readonly ManualExpectationBuilder<TItem> _negatedBuilder;
	private ConstraintResult? _unansweredItem;
	private int _unansweredItemIndex;

	protected ComplyWithConstraintBase(string it, ExpectationGrammars grammars, EnumerableQuantifier quantifier,
		Action<IThatSubject<TItem>> expectations)
		: base(it, grammars, quantifier)
	{
		// Without a nested quantifier, the item expectations keep the number of the subject that a connector such as
		// "whose values" introduced.
		if (!grammars.IsNested())
		{
			_builder = Create(grammars, expectations);
			_negatedBuilder = _builder;
			return;
		}

		ExpectationGrammars itemGrammars = quantifier.GetItemGrammars(grammars & ~ExpectationGrammars.Negated);
		ExpectationGrammars negatedItemGrammars = quantifier.GetItemGrammars(grammars | ExpectationGrammars.Negated);
		_builder = Create(itemGrammars, expectations);
		_negatedBuilder = negatedItemGrammars == itemGrammars ? _builder : Create(negatedItemGrammars, expectations);
	}

	/// <inheritdoc cref="ConstraintResult.FailureCause" />
	public override Exception? FailureCause => _unansweredItem?.FailureCause;

	/// <inheritdoc />
	protected override string Verb => _builder.GetResultVerb();

	/// <inheritdoc cref="IExpectationTextConstraint.GetExpectationResult(IEvaluationContext, CancellationToken)" />
	public async Task<ConstraintResult> GetExpectationResult(IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		await PrepareExpectation(context, cancellationToken);
		return this;
	}

	/// <summary>
	///     Starts a new evaluation and prepares the item expectations for it.
	/// </summary>
	private protected Task Start(IEvaluationContext context, CancellationToken cancellationToken)
	{
		_collectionContext = default;
		return PrepareExpectation(context, cancellationToken);
	}

	/// <summary>
	///     Prepares the item expectations for a new evaluation.
	/// </summary>
	private protected async Task PrepareExpectation(IEvaluationContext context, CancellationToken cancellationToken)
	{
		_unansweredItem = null;
		await _builder.PrepareExpectation(context, cancellationToken);
		if (!ReferenceEquals(_negatedBuilder, _builder))
		{
			await _negatedBuilder.PrepareExpectation(context, cancellationToken);
		}
	}

	/// <summary>
	///     Classifies the <paramref name="items" /> of the <paramref name="materialized" /> collection by the item
	///     expectations.
	/// </summary>
	/// <param name="materialized">The materialized collection.</param>
	/// <param name="items">The items of the collection.</param>
	/// <param name="cancelEarly">Stops reading the items as soon as they determine the outcome.</param>
	/// <param name="context">The evaluation context.</param>
	/// <param name="cancellationToken">The cancellation token of the evaluation.</param>
	private protected async Task<ConstraintResult> IsMetByItems(CollectionItems<TItem> materialized,
		IEnumerable<TItem> items, bool cancelEarly, IEvaluationContext context, CancellationToken cancellationToken)
	{
		int index = 0;
		foreach (TItem item in items)
		{
			if (materialized.IsCanceledBeforeTheEnd(cancellationToken))
			{
				Outcome = Outcome.Undecided;
				materialized.SetContext(ref _collectionContext, true);
				return this;
			}

			ConstraintResult isMatch = await _builder.IsMetBy(item, context, cancellationToken);
			if (StopsAt(isMatch, index, cancellationToken))
			{
				materialized.SetContext(ref _collectionContext, _unansweredItem is null);
				return this;
			}

			index++;

			Record(item, isMatch);
			if (cancelEarly && IsDetermined)
			{
				CompleteEarly();
				materialized.SetContext(ref _collectionContext);
				return this;
			}
		}

		Complete();
		materialized.SetContext(ref _collectionContext);
		return this;
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Classifies the items of the <paramref name="materialized" /> asynchronous enumerable by the item expectations.
	/// </summary>
	private protected async Task<ConstraintResult> IsMetByItems(IAsyncEnumerable<TItem> materialized,
		IEvaluationContext context, CancellationToken cancellationToken)
	{
		LimitedCollection<TItem> items = new();
		int count = 0;
		await foreach (TItem item in materialized.UntilCancelled(cancellationToken))
		{
			ConstraintResult isMatch = await _builder.IsMetBy(item, context, cancellationToken);
			items.Add(item);
			if (StopsAt(isMatch, count, cancellationToken))
			{
				if (_unansweredItem is null)
				{
					_collectionContext.Set(items, true);
				}
				else
				{
					_collectionContext.Set(materialized as IMaterializedAsyncEnumerable<TItem>);
				}

				return this;
			}

			count++;

			Record(item, isMatch);
			if (IsDetermined)
			{
				CompleteEarly();
				_collectionContext.Set(items, true);
				return this;
			}
		}

		if (cancellationToken.IsCanceledBeforeTheEndOf(materialized))
		{
			Outcome = Outcome.Undecided;
			_collectionContext.Set(items, true);
			return this;
		}

		Complete();
		_collectionContext.Set(items, totalCount: count);
		return this;
	}
#endif

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
	{
		_collectionContext.AppendTo(contexts);
		base.AppendContexts(contexts);
	}

	/// <inheritdoc />
	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		base.AppendNormalExpectation(stringBuilder, indentation);
		_builder.AppendReasons(stringBuilder);
	}

	/// <inheritdoc />
	protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
	{
		if (_unansweredItem is null)
		{
			base.AppendNormalResult(stringBuilder, indentation);
		}
		else
		{
			stringBuilder.AppendUnansweredItem(_unansweredItem, _unansweredItemIndex, indentation);
		}
	}

	/// <inheritdoc />
	protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		base.AppendNegatedExpectation(stringBuilder, indentation);
		_negatedBuilder.AppendReasons(stringBuilder);
	}

	/// <inheritdoc />
	protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
	{
		if (_unansweredItem is null)
		{
			base.AppendNegatedResult(stringBuilder, indentation);
		}
		else
		{
			stringBuilder.AppendUnansweredItem(_unansweredItem, _unansweredItemIndex, indentation);
		}
	}

	/// <inheritdoc />
	protected override void AppendItemExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars,
		string? indentation)
		=> (Grammars.IsNegated() ? _negatedBuilder : _builder).AppendExpectation(stringBuilder, indentation);

	/// <summary>
	///     Stops the evaluation at an item that a cancellation left undecided, as it must not count as not matching, or
	///     that the item expectations did not answer.
	/// </summary>
	private bool StopsAt(ConstraintResult isMatch, int index, CancellationToken cancellationToken)
	{
		if (isMatch.Outcome == Outcome.Undecided && cancellationToken.IsCancellationRequested)
		{
			Outcome = Outcome.Undecided;
			return true;
		}

		if (isMatch.Outcome == Outcome.FailureBothWays)
		{
			Outcome = Outcome.FailureBothWays;
			_unansweredItem = isMatch;
			_unansweredItemIndex = index;
			return true;
		}

		return false;
	}

	private static ManualExpectationBuilder<TItem> Create(ExpectationGrammars itemGrammars,
		Action<IThatSubject<TItem>> expectations)
	{
		ManualExpectationBuilder<TItem> builder = new(itemGrammars);
		expectations.Invoke(new ThatSubject<TItem>(builder));
		return builder;
	}
}

/// <remarks>
///     The items of a non-generic collection are formatted as the type of its first item that is not
///     <see langword="null" />.
/// </remarks>
internal sealed class ComplyWithConstraint<TEnumerable, TItem>(
	string it,
	ExpectationGrammars grammars,
	EnumerableQuantifier quantifier,
	Action<IThatSubject<TItem>> expectations)
	: ComplyWithConstraintBase<TEnumerable, TItem>(it, grammars, quantifier, expectations),
		IAsyncContextConstraint<TEnumerable>
	where TEnumerable : IEnumerable?
{
	private Type? _itemType;

	/// <inheritdoc />
	protected override Type ItemType => _itemType ?? typeof(TItem);

	public async ValueTask<ConstraintResult> IsMetBy(
		TEnumerable actual,
		IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		_itemType = null;
		Actual = actual;
		await Start(context, cancellationToken);
		if (actual.IsDefaultImmutableArray())
		{
			return this.AsNullSubject(It);
		}

		if (actual is null)
		{
			return this;
		}

		CollectionItems<TItem> materialized = CollectionItems<TItem>.Materialize(actual, context);
		IEnumerable<TItem> items = CollectionItems<TItem>.IsTyped<TEnumerable>()
			? materialized.Items
			: WithItemType(materialized.Items);
		return await IsMetByItems(materialized, items, CollectionItems<TItem>.CountOf(actual) is null, context,
			cancellationToken);
	}

	private IEnumerable<TItem> WithItemType(IEnumerable<TItem> items)
	{
		foreach (TItem item in items)
		{
			_itemType ??= item?.GetType();
			yield return item;
		}
	}
}

#if NET8_0_OR_GREATER
internal sealed class AsyncComplyWithConstraint<TItem>(
	string it,
	ExpectationGrammars grammars,
	EnumerableQuantifier quantifier,
	Action<IThatSubject<TItem>> expectations)
	: ComplyWithConstraintBase<IAsyncEnumerable<TItem>?, TItem>(it, grammars, quantifier, expectations),
		IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
{
	public async ValueTask<ConstraintResult> IsMetBy(
		IAsyncEnumerable<TItem>? actual,
		IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		Actual = actual;
		await Start(context, cancellationToken);
		if (actual is null)
		{
			return this;
		}

		return await IsMetByItems(context.UseMaterializedAsyncEnumerable<TItem>(actual, cancellationToken),
			context, cancellationToken);
	}
}
#endif
