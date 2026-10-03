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
///     The verification of the items at the index by the item expectations and the texts of <c>HasItemThat</c>, shared
///     by the synchronous and the asynchronous collections.
/// </summary>
internal abstract class HasItemThatConstraintBase<TValue, TItem> :
	ConstraintResult.WithNotNullValue<TValue>,
	IExpectationTextConstraint
{
	private readonly ManualExpectationBuilder<TItem> _itemExpectationBuilder;
	private TItem? _actual;
	private bool _hasIndex;
	private ConstraintResult? _unansweredItem;
	private int _unansweredItemIndex;

	protected HasItemThatConstraintBase(string it,
		ExpectationGrammars grammars,
		Action<IThatSubject<TItem>> expectations,
		CollectionIndexOptions options) : base(it, grammars)
	{
		Options = options;
		_itemExpectationBuilder =
			new ManualExpectationBuilder<TItem>((Grammars & ~ExpectationGrammars.Plural) | ExpectationGrammars.Introduced);
		expectations.Invoke(new ThatSubject<TItem>(_itemExpectationBuilder));
	}

	/// <summary>
	///     The options for the index of the item.
	/// </summary>
	protected CollectionIndexOptions Options { get; }

	/// <inheritdoc cref="ConstraintResult.FailureCause" />
	public override Exception? FailureCause => _unansweredItem?.FailureCause;

	/// <inheritdoc cref="IExpectationTextConstraint.GetExpectationResult(IEvaluationContext, CancellationToken)" />
	public async Task<ConstraintResult> GetExpectationResult(IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		await _itemExpectationBuilder.PrepareExpectation(context, cancellationToken);
		return this;
	}

	/// <summary>
	///     Starts a new evaluation, which fails unless an item at the index complies with the expectations.
	/// </summary>
	protected async Task Start(IEvaluationContext context, CancellationToken cancellationToken)
	{
		_actual = default;
		_hasIndex = false;
		_unansweredItem = null;
		Outcome = Outcome.Failure;
		await _itemExpectationBuilder.PrepareExpectation(context, cancellationToken);
	}

	/// <summary>
	///     Checks the <paramref name="item" /> at the <paramref name="index" /> of a collection with
	///     <paramref name="count" /> items and returns whether it decides the outcome, so that no further item is checked.
	/// </summary>
	protected async Task<bool> IsDecidedBy(TItem item, int index, int? count, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		bool? isIndexInRange = ThatEnumerable.IsIndexInRange(Options, index, count);
		if (isIndexInRange is null)
		{
			return false;
		}

		if (isIndexInRange == false)
		{
			return true;
		}

		_hasIndex = true;
		_actual = item;
		ConstraintResult isMatch = await _itemExpectationBuilder.IsMetBy(item, context, cancellationToken);
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

		Outcome = isMatch.Outcome;
		return isMatch.Outcome == Outcome.Success;
	}

	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		stringBuilder.Append(Grammars.Verb("has an item that ", "have an item that "));
		_itemExpectationBuilder.AppendExpectation(stringBuilder, indentation);
		stringBuilder.Append(Options.Match.GetDescription());
		_itemExpectationBuilder.AppendReasons(stringBuilder);
	}

	protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
	{
		if (_unansweredItem is not null)
		{
			stringBuilder.AppendUnansweredItem(_unansweredItem, _unansweredItemIndex, indentation);
			return;
		}

		if (_hasIndex)
		{
			if (Options.Match.OnlySingleIndex())
			{
				stringBuilder.Append(It).Append(" had item ");
				Formatter.Format(stringBuilder, _actual);
				stringBuilder.Append(Options.Match.GetDescription());
			}
			else
			{
				stringBuilder.Append(It).Append(" had no matching item").Append(Options.Match.GetDescription());
			}
		}
		else
		{
			stringBuilder.Append(It).Append(" had no item").Append(Options.Match.GetDescription());
		}
	}

	protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		stringBuilder.Append(Grammars.Verb("does not have an item that ", "do not have an item that "));
		_itemExpectationBuilder.AppendExpectation(stringBuilder, indentation);
		stringBuilder.Append(Options.Match.GetDescription());
		_itemExpectationBuilder.AppendReasons(stringBuilder);
	}

	protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
	{
		if (_unansweredItem is not null)
		{
			stringBuilder.AppendUnansweredItem(_unansweredItem, _unansweredItemIndex, indentation);
			return;
		}

		stringBuilder.Append(It).Append(" had item ");
		Formatter.Format(stringBuilder, _actual);
		stringBuilder.Append(Options.Match.GetDescription());
	}
}

internal sealed class HasItemThatConstraint<TEnumerable, TItem>(
	string it,
	ExpectationGrammars grammars,
	Action<IThatSubject<TItem>> expectations,
	CollectionIndexOptions options)
	: HasItemThatConstraintBase<TEnumerable, TItem>(it, grammars, expectations, options),
		IAsyncContextConstraint<TEnumerable>
	where TEnumerable : IEnumerable?
{
	private CollectionContext _collectionContext;

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
		=> _collectionContext.AppendTo(contexts);

	public async Task<ConstraintResult> IsMetBy(TEnumerable actual, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		_collectionContext = default;
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
		materialized.SetContext(ref _collectionContext);

		IEnumerable<TItem> items = materialized.Items;
		if (!ThatEnumerable.TryCountForIndex(Options, actual, items, cancellationToken, out int? count))
		{
			Outcome = Outcome.Undecided;
			return this;
		}

		int index = 0;
		foreach (TItem item in items)
		{
			if (materialized.IsCanceledBeforeTheEnd(cancellationToken))
			{
				Outcome = Outcome.Undecided;
				return this;
			}

			if (await IsDecidedBy(item, index++, count, context, cancellationToken))
			{
				break;
			}
		}

		return this;
	}
}

#if NET8_0_OR_GREATER
internal sealed class AsyncHasItemThatConstraint<TItem>(
	string it,
	ExpectationGrammars grammars,
	Action<IThatSubject<TItem>> expectations,
	CollectionIndexOptions options)
	: HasItemThatConstraintBase<IAsyncEnumerable<TItem>?, TItem>(it, grammars, expectations, options),
		IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
{
	private CollectionContext _collectionContext;

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
		=> _collectionContext.AppendTo(contexts);

	public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<TItem>? actual, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		_collectionContext = default;
		Actual = actual;
		await Start(context, cancellationToken);
		if (actual is null)
		{
			return this;
		}

		IAsyncEnumerable<TItem> materialized =
			context.UseMaterializedAsyncEnumerable<TItem>(actual, cancellationToken);
		_collectionContext.Set(materialized as IMaterializedAsyncEnumerable<TItem>);

		int? count = null;
		if (Options.Match is CollectionIndexOptions.IMatchFromEnd)
		{
			count = (await (materialized as IMaterializedAsyncEnumerable<TItem>)!.MaterializeItems(null)).Count;
			if (count is null)
			{
				Outcome = Outcome.Undecided;
				return this;
			}
		}

		int index = 0;
		await foreach (TItem item in materialized.UntilCancelled(cancellationToken))
		{
			if (await IsDecidedBy(item, index++, count, context, cancellationToken))
			{
				return this;
			}
		}

		if (cancellationToken.IsCanceledBeforeTheEndOf(materialized))
		{
			Outcome = Outcome.Undecided;
		}

		return this;
	}
}
#endif
