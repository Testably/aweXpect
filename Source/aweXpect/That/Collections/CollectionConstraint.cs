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

/// <summary>
///     A quantified expectation on the items of a collection that a synchronous or an asynchronous predicate verifies,
///     shared by the synchronous and the asynchronous collections.
/// </summary>
internal abstract class PredicateCollectionConstraint<TValue, TItem>
	: QuantifiedCollectionConstraint<TValue, TItem>
{
	private readonly Action<ResultContextCollector>? _appendOptionsContexts;
	private readonly Func<TItem, ValueTask<bool>>? _asyncPredicate;
	private readonly Func<TItem, bool>? _predicate;

	protected PredicateCollectionConstraint(
		string it,
		ExpectationGrammars grammars,
		EnumerableQuantifier quantifier,
		Func<ExpectationGrammars, string> expectationText,
		Func<TItem, bool> predicate,
		string verb)
		: base(it, grammars, quantifier, expectationText, verb)
	{
		_predicate = predicate;
	}

	protected PredicateCollectionConstraint(
		string it,
		ExpectationGrammars grammars,
		EnumerableQuantifier quantifier,
		Func<ExpectationGrammars, string> expectationText,
		Func<TItem, ValueTask<bool>> predicate,
		string verb,
		Action<ResultContextCollector>? appendOptionsContexts)
		: base(it, grammars, quantifier, expectationText, verb)
	{
		_asyncPredicate = predicate;
		_appendOptionsContexts = appendOptionsContexts;
	}

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
	{
		base.AppendContexts(contexts);
		_appendOptionsContexts?.Invoke(contexts);
	}

	/// <summary>
	///     Whether the <paramref name="item" /> satisfies the predicate.
	/// </summary>
	protected ValueTask<bool> Matches(TItem item)
		=> _asyncPredicate is null
			? new ValueTask<bool>(UserCode.Invoke(_predicate!, item, "the predicate"))
			: _asyncPredicate(item);
}

/// <remarks>
///     The items of a non-generic collection are formatted as the type of its first item that is not
///     <see langword="null" />.
/// </remarks>
internal sealed class CollectionConstraint<TEnumerable, TItem>
	: PredicateCollectionConstraint<TEnumerable, TItem>,
		IAsyncContextConstraint<TEnumerable>
	where TEnumerable : IEnumerable?
{
	private readonly Func<object?, bool>? _useComparerOf;
	private CollectionContext _collectionContext;
	private Type? _itemType;

	public CollectionConstraint(
		string it,
		ExpectationGrammars grammars,
		EnumerableQuantifier quantifier,
		Func<ExpectationGrammars, string> expectationText,
		Func<TItem, bool> predicate,
		string verb)
		: base(it, grammars, quantifier, expectationText, predicate, verb)
	{
	}

	public CollectionConstraint(
		string it,
		ExpectationGrammars grammars,
		EnumerableQuantifier quantifier,
		Func<ExpectationGrammars, string> expectationText,
		Func<TItem, ValueTask<bool>> predicate,
		string verb,
		Func<object?, bool>? useComparerOf = null,
		Action<ResultContextCollector>? appendOptionsContexts = null)
		: base(it, grammars, quantifier, expectationText, predicate, verb, appendOptionsContexts)
	{
		_useComparerOf = useComparerOf;
	}

	/// <inheritdoc />
	protected override Type ItemType => _itemType ?? typeof(TItem);

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
	{
		_collectionContext.AppendTo(contexts);
		base.AppendContexts(contexts);
	}

	public async Task<ConstraintResult> IsMetBy(
		TEnumerable actual,
		IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		_collectionContext = default;
		_itemType = null;
		Actual = actual;
		if (actual.IsDefaultImmutableArray())
		{
			return this.AsNullSubject(It);
		}

		if (actual is null)
		{
			return this;
		}

		_useComparerOf?.Invoke(actual);
		CollectionItems<TItem> materialized = CollectionItems<TItem>.Materialize(actual, context);
		bool cancelEarly = CollectionItems<TItem>.CountOf(actual) is null;
		bool isUntyped = !CollectionItems<TItem>.IsTyped<TEnumerable>();
		foreach (TItem item in materialized.Items)
		{
			if (materialized.IsCanceledBeforeTheEnd(cancellationToken))
			{
				Outcome = Outcome.Undecided;
				materialized.SetContext(ref _collectionContext, true);
				return this;
			}

			if (isUntyped)
			{
				_itemType ??= item?.GetType();
			}

			Record(item, await Matches(item));
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
}

#if NET8_0_OR_GREATER
internal sealed class AsyncCollectionConstraint<TItem>
	: PredicateCollectionConstraint<IAsyncEnumerable<TItem>?, TItem>,
		IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
{
	private CollectionContext _collectionContext;

	public AsyncCollectionConstraint(
		string it,
		ExpectationGrammars grammars,
		EnumerableQuantifier quantifier,
		Func<ExpectationGrammars, string> expectationText,
		Func<TItem, bool> predicate,
		string verb)
		: base(it, grammars, quantifier, expectationText, predicate, verb)
	{
	}

	public AsyncCollectionConstraint(
		string it,
		ExpectationGrammars grammars,
		EnumerableQuantifier quantifier,
		Func<ExpectationGrammars, string> expectationText,
		Func<TItem, ValueTask<bool>> predicate,
		string verb,
		Action<ResultContextCollector>? appendOptionsContexts = null)
		: base(it, grammars, quantifier, expectationText, predicate, verb, appendOptionsContexts)
	{
	}

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
	{
		_collectionContext.AppendTo(contexts);
		base.AppendContexts(contexts);
	}

	public async Task<ConstraintResult> IsMetBy(
		IAsyncEnumerable<TItem>? actual,
		IEvaluationContext context,
		CancellationToken cancellationToken)
	{
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
