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
///     The verification of the items at the index and the texts of <c>HasItem</c>, shared by the synchronous and the
///     asynchronous collections.
/// </summary>
internal abstract class HasItemConstraintBase<TValue, TItem>
	: ConstraintResult.WithNotNullValue<TValue>
{
	/// <summary>
	///     The grammars when the expectation was added, which the description of the item uses.
	/// </summary>
	private readonly ExpectationGrammars _itemGrammars;

	private readonly SynchronouslyMatchedItem<TItem>? _synchronousItem;
	private TItem? _actual;
	private ContainedItem<TItem> _evaluationItem;
	private bool _hasIndex;

	protected HasItemConstraintBase(
		string it,
		ExpectationGrammars grammars,
		ContainedItem<TItem> item,
		CollectionIndexOptions options)
		: base(it, grammars)
	{
		_itemGrammars = grammars;
		Item = item;
		_evaluationItem = item;
		_synchronousItem = item as SynchronouslyMatchedItem<TItem>;
		Options = options;
	}

	/// <summary>
	///     What the collection is searched for.
	/// </summary>
	protected ContainedItem<TItem> Item { get; }

	/// <summary>
	///     The options for the index of the item.
	/// </summary>
	protected CollectionIndexOptions Options { get; }

	/// <summary>
	///     Whether the predicate is synchronous, so that the items can be verified without awaiting it.
	/// </summary>
	protected bool IsSynchronous => _synchronousItem is not null;

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
		=> Item.AppendContexts(contexts);

	/// <summary>
	///     Starts a new evaluation in the <paramref name="context" />, which fails unless an item at the index matches.
	/// </summary>
	protected void Start(IEvaluationContext context, CancellationToken cancellationToken)
	{
		_evaluationItem = Item.ForEvaluation(context, cancellationToken);
		_actual = default;
		_hasIndex = false;
		Outcome = Outcome.Failure;
	}

	/// <summary>
	///     Whether the <paramref name="item" /> matches the synchronous predicate.
	/// </summary>
	protected bool MatchesSynchronously(TItem item)
		=> _synchronousItem!.IsMatch(item);

	/// <summary>
	///     Whether the <paramref name="item" /> matches.
	/// </summary>
	protected ValueTask<bool> Matches(TItem item)
		=> _evaluationItem.Matches(item);

	/// <summary>
	///     Whether the item at the <paramref name="index" /> of a collection with <paramref name="count" /> items is at
	///     the expected index: <see langword="false" /> when no later item can be, and <see langword="null" /> when a
	///     later item can be.
	/// </summary>
	protected bool? IsAtIndex(int index, int? count)
		=> ThatEnumerable.IsIndexInRange(Options, index, count);

	/// <summary>
	///     Records whether the <paramref name="item" /> at the expected index matches and returns
	///     <see langword="true" />, when it decides the outcome.
	/// </summary>
	protected bool Record(TItem item, bool isMatch)
	{
		_hasIndex = true;
		_actual = item;
		Outcome = isMatch ? Outcome.Success : Outcome.Failure;
		return isMatch || Options.Match.OnlySingleIndex();
	}

	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append(Grammars.Verb("has an item ", "have an item "))
			.Append(Item.GetHasItemExpectation(_itemGrammars))
			.Append(Options.Match.GetDescription());

	protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
	{
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
		=> stringBuilder.Append(Grammars.Verb("does not have an item ", "do not have an item "))
			.Append(Item.GetHasItemExpectation(_itemGrammars))
			.Append(Options.Match.GetDescription());

	protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
	{
		stringBuilder.Append(It).Append(" had item ");
		Formatter.Format(stringBuilder, _actual);
		stringBuilder.Append(Options.Match.GetDescription());
	}
}

internal sealed class HasItemConstraint<TEnumerable, TItem>(
	string it,
	ExpectationGrammars grammars,
	ContainedItem<TItem> containedItem,
	CollectionIndexOptions options)
	: HasItemConstraintBase<TEnumerable, TItem>(it, grammars, containedItem, options),
		IAsyncContextConstraint<TEnumerable>
	where TEnumerable : IEnumerable?
{
	private CollectionContext _collectionContext;

	public ValueTask<ConstraintResult> IsMetBy(TEnumerable actual, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		_collectionContext = default;
		Actual = actual;
		Start(context, cancellationToken);
		if (actual.IsDefaultImmutableArray())
		{
			return new ValueTask<ConstraintResult>(this.AsNullSubject(It));
		}

		if (actual is null)
		{
			return new ValueTask<ConstraintResult>(this);
		}

		Item.UseComparerOf(actual);
		CollectionItems<TItem> materialized = CollectionItems<TItem>.Materialize(actual, context);
		materialized.SetContext(ref _collectionContext);

		if (!ThatEnumerable.TryCountForIndex(Options, actual, materialized.Items, cancellationToken,
			    out int? count))
		{
			Outcome = Outcome.Undecided;
			return new ValueTask<ConstraintResult>(this);
		}

		return IsSynchronous
			? new ValueTask<ConstraintResult>(Verify(materialized, count, cancellationToken))
			: VerifyAsync(materialized, count, cancellationToken);
	}

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
	{
		_collectionContext.AppendTo(contexts);
		base.AppendContexts(contexts);
	}

	private HasItemConstraint<TEnumerable, TItem> Verify(CollectionItems<TItem> materialized, int? count,
		CancellationToken cancellationToken)
	{
		int index = 0;
		foreach (TItem item in materialized)
		{
			if (materialized.IsCanceledBeforeTheEnd(cancellationToken))
			{
				Outcome = Outcome.Undecided;
				return this;
			}

			bool? isAtIndex = IsAtIndex(index++, count);
			if (isAtIndex == false || (isAtIndex == true && Record(item, MatchesSynchronously(item))))
			{
				return this;
			}
		}

		return this;
	}

	private async ValueTask<ConstraintResult> VerifyAsync(CollectionItems<TItem> materialized, int? count,
		CancellationToken cancellationToken)
	{
		int index = 0;
		foreach (TItem item in materialized)
		{
			if (materialized.IsCanceledBeforeTheEnd(cancellationToken))
			{
				Outcome = Outcome.Undecided;
				return this;
			}

			bool? isAtIndex = IsAtIndex(index++, count);
			if (isAtIndex == false || (isAtIndex == true && Record(item, await Matches(item))))
			{
				return this;
			}
		}

		return this;
	}
}

#if NET8_0_OR_GREATER
internal sealed class AsyncHasItemConstraint<TItem>(
	string it,
	ExpectationGrammars grammars,
	ContainedItem<TItem> containedItem,
	CollectionIndexOptions options)
	: HasItemConstraintBase<IAsyncEnumerable<TItem>?, TItem>(it, grammars, containedItem, options),
		IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
{
	private CollectionContext _collectionContext;

	public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<TItem>? actual, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		_collectionContext = default;
		Actual = actual;
		Start(context, cancellationToken);
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
			bool? isAtIndex = IsAtIndex(index++, count);
			if (isAtIndex == false || (isAtIndex == true && Record(item, await Matches(item))))
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

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
	{
		_collectionContext.AppendTo(contexts);
		base.AppendContexts(contexts);
	}
}
#endif
