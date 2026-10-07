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
/// <remarks>
///     The members are compared by the <paramref name="options" />, which only add their contexts when they are
///     <see cref="ObjectEqualityOptions{TSubject}" />. The expectation names the <paramref name="displayedOptions" />,
///     e.g. to name the comparer of a set subject, and the <paramref name="memberAccessorExpression" />, unless the items
///     themselves are compared.
/// </remarks>
internal abstract class AreUniqueConstraintBase<TValue, TItem, TMember>(
	string it,
	ExpectationGrammars grammars,
	EnumerableQuantifier quantifier,
	Func<TItem, TMember> memberAccessor,
	string? memberAccessorExpression,
	IOptionsEquality<TMember> options,
	object displayedOptions,
	bool expectUnique)
	: QuantifiedCollectionConstraintBase<TValue, TItem>(it, grammars, quantifier)
{
	/// <summary>
	///     Whether the items are expected to be unique.
	/// </summary>
	protected bool ExpectUnique => expectUnique;

	/// <inheritdoc />
	protected override string Verb => "were";

	/// <inheritdoc />
	protected override void AppendItemExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars,
		string? indentation)
	{
		ExpectationGrammars itemGrammars = expectUnique ? grammars : grammars.Negate();
		stringBuilder.Append(memberAccessorExpression is null
			? ElementExpectations.IsUnique(itemGrammars, displayedOptions)
			: ElementExpectations.IsUniqueFor(itemGrammars, memberAccessorExpression.TrimCommonWhiteSpace(),
				displayedOptions));
	}

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
	{
		base.AppendContexts(contexts);
		(options as ObjectEqualityOptions<TMember>)?.AppendContexts(contexts);
	}

	/// <summary>
	///     Counts the occurrences of the members of the items.
	/// </summary>
	protected OccurrenceCounter<TMember> CreateCounter()
		=> new(options, MemberHashing.For(options));

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
///     When the <paramref name="subjectComparing" /> lets the subject decide, e.g. for a set with a custom comparer
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
	Func<TItem, TMember> memberAccessor,
	string? memberAccessorExpression,
	IOptionsEquality<TMember> options,
	bool expectUnique,
	ISubjectComparing? subjectComparing = null)
	: AreUniqueConstraintBase<TEnumerable, TItem, TMember>(it, grammars, quantifier, memberAccessor,
			memberAccessorExpression, options, (object?)subjectComparing ?? options, expectUnique),
		IAsyncContextConstraint<TEnumerable>
	where TEnumerable : IEnumerable?
{
	private CollectionContext _collectionContext;
	private Type? _itemType;

	/// <inheritdoc />
	protected override Type ItemType => _itemType ?? typeof(TItem);

	public async ValueTask<ConstraintResult> IsMetBy(
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
			return this.AsNullSubject(It);
		}

		if (actual is null)
		{
			Outcome = Outcome.Failure;
			return this;
		}

		CollectionItems<TItem> materialized = CollectionItems<TItem>.Materialize(actual, context);
		bool isUntyped = !CollectionItems<TItem>.IsTyped<TEnumerable>();
		if (subjectComparing?.UseComparerOf(actual) == true)
		{
			foreach (TItem item in materialized)
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
		foreach (TItem item in materialized)
		{
			if (materialized.IsCanceledBeforeTheEnd(cancellationToken))
			{
				Outcome = Outcome.Undecided;
				materialized.SetContext(ref _collectionContext, true);
				return this;
			}

			KeepItemType(item, isUntyped);
			try
			{
				await Add(items, occurrences, item);
			}
			catch (Exception) when (items.Count > 0 && occurrences.Determines(Quantifier, ExpectUnique))
			{
				// A collection whose number of items is known is counted to its end. When the member selector or the
				// comparison throws for an item after the ones that determine the outcome, the counting ends there,
				// so that the outcome is the same as for a collection that is read only until it is determined.
				return FinishEarly(materialized, items, occurrences);
			}

			if (cancelEarly && occurrences.Determines(Quantifier, ExpectUnique))
			{
				return FinishEarly(materialized, items, occurrences);
			}
		}

		RecordAll(items, occurrences);
		Complete();
		materialized.SetContext(ref _collectionContext);
		return this;
	}

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
	{
		_collectionContext.AppendTo(contexts);
		base.AppendContexts(contexts);
	}

	private AreUniqueConstraint<TEnumerable, TItem, TMember> FinishEarly(CollectionItems<TItem> materialized,
		List<(TItem Item, int MemberIndex)> items, OccurrenceCounter<TMember> occurrences)
	{
		RecordAll(items, occurrences);
		CompleteEarly();
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
	Func<TItem, TMember> memberAccessor,
	string? memberAccessorExpression,
	IOptionsEquality<TMember> options,
	bool expectUnique)
	: AreUniqueConstraintBase<IAsyncEnumerable<TItem>?, TItem, TMember>(it, grammars, quantifier, memberAccessor,
			memberAccessorExpression, options, options, expectUnique),
		IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
{
	private CollectionContext _collectionContext;

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

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
	{
		_collectionContext.AppendTo(contexts);
		base.AppendContexts(contexts);
	}
}
#endif
