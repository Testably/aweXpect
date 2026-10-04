using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Helpers;
using aweXpect.Options;
#if NET8_0_OR_GREATER
using System.Collections.Generic;
#endif

namespace aweXpect;

#pragma warning disable S110 // The depth comes from the public quantified collection constraints and the result values of aweXpect.Core
/// <summary>
///     A quantified expectation on the items of a collection that a synchronous or an asynchronous predicate verifies,
///     shared by the synchronous and the asynchronous collections.
/// </summary>
internal abstract class PredicateCollectionConstraint<TValue, TItem>(
	string it,
	ExpectationGrammars grammars,
	EnumerableQuantifier quantifier,
	ElementCondition<TItem> condition,
	string verb)
	: QuantifiedCollectionConstraintBase<TValue, TItem>(it, grammars, quantifier)
{
	private readonly SynchronousElementCondition<TItem>? _synchronousCondition =
		condition as SynchronousElementCondition<TItem>;

	/// <summary>
	///     What every item is verified for.
	/// </summary>
	protected ElementCondition<TItem> Condition => condition;

	/// <inheritdoc />
	protected override string Verb => verb;

	/// <inheritdoc />
	protected override void AppendItemExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars,
		string? indentation)
		=> stringBuilder.Append(condition.GetExpectation(grammars));

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
	{
		base.AppendContexts(contexts);
		condition.AppendContexts(contexts);
	}

	/// <summary>
	///     Whether the condition is synchronous, so that the items can be verified without awaiting it.
	/// </summary>
	protected bool IsSynchronous => _synchronousCondition is not null;

	/// <summary>
	///     Whether the <paramref name="item" /> meets the synchronous condition.
	/// </summary>
	protected bool MatchesSynchronously(TItem item)
		=> _synchronousCondition!.IsMet(item);

	/// <summary>
	///     Whether the <paramref name="item" /> meets the condition.
	/// </summary>
	protected ValueTask<bool> Matches(TItem item)
		=> condition.IsMetBy(item);
}

/// <remarks>
///     The items of a non-generic collection are formatted as the type of its first item that is not
///     <see langword="null" />.
/// </remarks>
internal sealed class CollectionConstraint<TEnumerable, TItem>(
	string it,
	ExpectationGrammars grammars,
	EnumerableQuantifier quantifier,
	ElementCondition<TItem> condition,
	string verb)
	: PredicateCollectionConstraint<TEnumerable, TItem>(it, grammars, quantifier, condition, verb),
		IAsyncContextConstraint<TEnumerable>
	where TEnumerable : IEnumerable?
{
	private readonly bool _isUntyped = !CollectionItems<TItem>.IsTyped<TEnumerable>();
	private CollectionContext _collectionContext;
	private Type? _itemType;

	/// <inheritdoc />
	protected override Type ItemType => _itemType ?? typeof(TItem);

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
	{
		_collectionContext.AppendTo(contexts);
		base.AppendContexts(contexts);
	}

	public ValueTask<ConstraintResult> IsMetBy(
		TEnumerable actual,
		IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		StartEvaluation();
		_collectionContext = default;
		_itemType = null;
		Actual = actual;
		if (actual.IsDefaultImmutableArray())
		{
			return new ValueTask<ConstraintResult>(this.AsNullSubject(It));
		}

		if (actual is null)
		{
			return new ValueTask<ConstraintResult>(this);
		}

		Condition.UseComparerOf(actual);
		CollectionItems<TItem> materialized = CollectionItems<TItem>.Materialize(actual, context);
		bool cancelEarly = CollectionItems<TItem>.CountOf(actual) is null;
		return IsSynchronous
			? new ValueTask<ConstraintResult>(Verify(materialized, cancelEarly, cancellationToken))
			: VerifyAsync(materialized, cancelEarly, cancellationToken);
	}

	private CollectionConstraint<TEnumerable, TItem> Verify(CollectionItems<TItem> materialized, bool cancelEarly,
		CancellationToken cancellationToken)
	{
		foreach (TItem item in materialized)
		{
			if (IsCanceled(materialized, cancellationToken))
			{
				return this;
			}

			if (IsDecidedBy(materialized, item, MatchesSynchronously(item), cancelEarly))
			{
				return this;
			}
		}

		return Finish(materialized);
	}

	private async ValueTask<ConstraintResult> VerifyAsync(CollectionItems<TItem> materialized, bool cancelEarly,
		CancellationToken cancellationToken)
	{
		foreach (TItem item in materialized)
		{
			if (IsCanceled(materialized, cancellationToken))
			{
				return this;
			}

			if (IsDecidedBy(materialized, item, await Matches(item), cancelEarly))
			{
				return this;
			}
		}

		return Finish(materialized);
	}

	private bool IsCanceled(CollectionItems<TItem> materialized, CancellationToken cancellationToken)
	{
		if (!materialized.IsCanceledBeforeTheEnd(cancellationToken))
		{
			return false;
		}

		Outcome = Outcome.Undecided;
		materialized.SetContext(ref _collectionContext, true);
		return true;
	}

	private bool IsDecidedBy(CollectionItems<TItem> materialized, TItem item, bool isMatch, bool cancelEarly)
	{
		if (_isUntyped)
		{
			_itemType ??= item?.GetType();
		}

		Record(item, isMatch);
		if (!cancelEarly || !IsDetermined)
		{
			return false;
		}

		CompleteEarly();
		materialized.SetContext(ref _collectionContext);
		return true;
	}

	private CollectionConstraint<TEnumerable, TItem> Finish(CollectionItems<TItem> materialized)
	{
		Complete();
		materialized.SetContext(ref _collectionContext);
		return this;
	}
}

#if NET8_0_OR_GREATER
internal sealed class AsyncCollectionConstraint<TItem>(
	string it,
	ExpectationGrammars grammars,
	EnumerableQuantifier quantifier,
	ElementCondition<TItem> condition,
	string verb)
	: PredicateCollectionConstraint<IAsyncEnumerable<TItem>?, TItem>(it, grammars, quantifier, condition, verb),
		IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
{
	private CollectionContext _collectionContext;

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
	{
		_collectionContext.AppendTo(contexts);
		base.AppendContexts(contexts);
	}

	public async ValueTask<ConstraintResult> IsMetBy(
		IAsyncEnumerable<TItem>? actual,
		IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		StartEvaluation();
		_collectionContext = default;
		Actual = actual;
		if (actual is null)
		{
			return this;
		}

		IAsyncEnumerable<TItem> materialized =
			context.UseMaterializedAsyncEnumerable<TItem>(actual, cancellationToken);
		LimitedCollection<TItem> items = new();
		int count = 0;
		await foreach (TItem item in materialized.UntilCancelled(cancellationToken))
		{
			Record(item, await Matches(item));
			items.Add(item);
			count++;
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
}
#endif
