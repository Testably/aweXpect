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

#pragma warning disable S110 // The depth comes from the public quantified collection constraints and the result values of aweXpect.Core
/// <summary>
///     The comparison of the items of <c>AreUnique</c>, shared by the synchronous and the asynchronous collections.
/// </summary>
internal abstract class AreUniqueConstraintBase<TValue, TItem, TMember>(
	string it,
	ExpectationGrammars grammars,
	EnumerableQuantifier quantifier,
	Func<ExpectationGrammars, string> expectationText,
	Func<TItem, TMember> memberAccessor,
	Func<TMember, TMember, ValueTask<bool>> areConsideredEqual,
	bool expectUnique,
	Action<ResultContextCollector>? appendOptionsContexts,
	Func<Func<TMember, int>?>? createGetHashCode)
	: QuantifiedCollectionConstraint<TValue, TItem>(it, grammars, quantifier, expectationText, "were")
{
	/// <summary>
	///     Whether the items are expected to be unique.
	/// </summary>
	protected bool ExpectUnique => expectUnique;

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
	{
		base.AppendContexts(contexts);
		appendOptionsContexts?.Invoke(contexts);
	}

	/// <summary>
	///     Counts the occurrences of the members of the items.
	/// </summary>
	protected OccurrenceCounter<TMember> CreateCounter()
		=> new(areConsideredEqual, createGetHashCode?.Invoke());

	/// <summary>
	///     Adds the member of the <paramref name="item" /> to the <paramref name="occurrences" />.
	/// </summary>
	protected async Task Add(List<(TItem Item, int MemberIndex)> items, OccurrenceCounter<TMember> occurrences,
		TItem item)
		=> items.Add((item, await occurrences.Add(UserCode.Invoke(memberAccessor, item, "the member selector"))));

	/// <summary>
	///     Records the <paramref name="items" /> by whether their member occurs once.
	/// </summary>
	protected void RecordAll(List<(TItem Item, int MemberIndex)> items, OccurrenceCounter<TMember> occurrences)
	{
		foreach ((TItem item, int memberIndex) in items)
		{
			Record(item, occurrences.IsUnique(memberIndex) == expectUnique);
		}
	}
}

/// <remarks>
///     When <paramref name="isUniqueBySubject" /> holds for the subject, e.g. for a set with a custom comparer
///     whose comparison was not changed, every item is unique without being compared, because a set never holds two
///     items that its comparer considers equal.
///     <para />
///     The items of a non-generic collection are formatted as the type of its first item that is not
///     <see langword="null" />.
/// </remarks>
internal sealed class AreUniqueConstraint<TEnumerable, TItem, TMember>(
	string it,
	ExpectationGrammars grammars,
	EnumerableQuantifier quantifier,
	Func<ExpectationGrammars, string> expectationText,
	Func<TItem, TMember> memberAccessor,
	Func<TMember, TMember, ValueTask<bool>> areConsideredEqual,
	bool expectUnique,
	Func<object?, bool>? isUniqueBySubject = null,
	Action<ResultContextCollector>? appendOptionsContexts = null,
	Func<Func<TMember, int>?>? createGetHashCode = null)
	: AreUniqueConstraintBase<TEnumerable, TItem, TMember>(it, grammars, quantifier, expectationText, memberAccessor,
			areConsideredEqual, expectUnique, appendOptionsContexts, createGetHashCode),
		IAsyncContextConstraint<TEnumerable>
	where TEnumerable : IEnumerable?
{
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

	public async ValueTask<ConstraintResult> IsMetBy(
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
			Outcome = Outcome.Failure;
			return this;
		}

		CollectionItems<TItem> materialized = CollectionItems<TItem>.Materialize(actual, context);
		bool isUntyped = !CollectionItems<TItem>.IsTyped<TEnumerable>();
		if (isUniqueBySubject?.Invoke(actual) == true)
		{
			foreach (TItem item in materialized.Items)
			{
				KeepItemType(item, isUntyped);
				Record(item, ExpectUnique);
			}

			Complete();
			materialized.SetContext(ref _collectionContext);
			return this;
		}

		bool cancelEarly = CollectionItems<TItem>.CountOf(actual) is null;
		OccurrenceCounter<TMember> occurrences = CreateCounter();
		List<(TItem Item, int MemberIndex)> items = [];
		foreach (TItem item in materialized.Items)
		{
			if (materialized.IsCanceledBeforeTheEnd(cancellationToken))
			{
				Outcome = Outcome.Undecided;
				materialized.SetContext(ref _collectionContext, true);
				return this;
			}

			KeepItemType(item, isUntyped);
			await Add(items, occurrences, item);
			if (cancelEarly && occurrences.Determines(Quantifier, ExpectUnique))
			{
				RecordAll(items, occurrences);
				CompleteEarly();
				materialized.SetContext(ref _collectionContext);
				return this;
			}
		}

		RecordAll(items, occurrences);
		Complete();
		materialized.SetContext(ref _collectionContext);
		return this;
	}

	private void KeepItemType(TItem item, bool isUntyped)
	{
		if (isUntyped)
		{
			_itemType ??= item?.GetType();
		}
	}
}

#if NET8_0_OR_GREATER
internal sealed class AsyncAreUniqueConstraint<TItem, TMember>(
	string it,
	ExpectationGrammars grammars,
	EnumerableQuantifier quantifier,
	Func<ExpectationGrammars, string> expectationText,
	Func<TItem, TMember> memberAccessor,
	Func<TMember, TMember, ValueTask<bool>> areConsideredEqual,
	bool expectUnique,
	Action<ResultContextCollector>? appendOptionsContexts = null,
	Func<Func<TMember, int>?>? createGetHashCode = null)
	: AreUniqueConstraintBase<IAsyncEnumerable<TItem>?, TItem, TMember>(it, grammars, quantifier, expectationText,
			memberAccessor, areConsideredEqual, expectUnique, appendOptionsContexts, createGetHashCode),
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
		_collectionContext = default;
		Actual = actual;
		if (actual is null)
		{
			Outcome = Outcome.Failure;
			return this;
		}

		IAsyncEnumerable<TItem> materialized =
			context.UseMaterializedAsyncEnumerable<TItem>(actual, cancellationToken);
		OccurrenceCounter<TMember> occurrences = CreateCounter();
		List<(TItem Item, int MemberIndex)> items = [];
		await foreach (TItem item in materialized.UntilCancelled(cancellationToken))
		{
			await Add(items, occurrences, item);
			if (occurrences.Determines(Quantifier, ExpectUnique))
			{
				RecordAll(items, occurrences);
				CompleteEarly();
				_collectionContext.Set(items.ConvertAll(x => x.Item), true);
				return this;
			}
		}

		List<TItem> collection = items.ConvertAll(x => x.Item);
		if (cancellationToken.IsCanceledBeforeTheEndOf(materialized))
		{
			Outcome = Outcome.Undecided;
			_collectionContext.Set(collection, true);
			return this;
		}

		RecordAll(items, occurrences);
		Complete();
		_collectionContext.Set(collection);
		return this;
	}
}
#endif
