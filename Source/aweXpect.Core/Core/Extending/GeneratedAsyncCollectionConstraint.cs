#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Options;

namespace aweXpect.Core.Extending;

/// <summary>
///     The base class of the constraints that <see cref="GenerateCollectionExpectationAttribute" /> generates for an
///     <see cref="IAsyncEnumerable{T}" />, which classify every item with <see cref="IsMatch" />.
/// </summary>
/// <remarks>
///     An item that the predicate cannot answer stops the evaluation, and the expectation and its negation fail with
///     the reason, unless the items before already determine the outcome.<br />
///     The items are only read until the outcome is determined, unless the expectation is directly on the collection.
/// </remarks>
/// <typeparam name="TItem">The type of the items in the collection.</typeparam>
/// <param name="it">The name of the subject.</param>
/// <param name="grammars">The grammars of the expectation.</param>
/// <param name="quantifier">The quantifier for the items.</param>
public abstract class GeneratedAsyncCollectionConstraint<TItem>(
	string it,
	ExpectationGrammars grammars,
	EnumerableQuantifier quantifier)
	: QuantifiedCollectionConstraintBase<IAsyncEnumerable<TItem>?, TItem>(it, grammars, quantifier),
		IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
{
	private IMaterializedAsyncEnumerable<TItem>? _collection;

	/// <summary>
	///     Initializes the constraint of an expectation directly on the collection, e.g.
	///     <c>That(tracks).AreRadioFriendly()</c>, when <paramref name="isOnCollection" /> is <see langword="true" />.
	/// </summary>
	/// <param name="it">The name of the subject.</param>
	/// <param name="grammars">The grammars of the expectation.</param>
	/// <param name="quantifier">The quantifier for the items, e.g. all items.</param>
	/// <param name="isOnCollection">Whether the expectation is directly on the collection.</param>
	protected GeneratedAsyncCollectionConstraint(string it, ExpectationGrammars grammars,
		EnumerableQuantifier quantifier, bool isOnCollection)
		: this(it, grammars, quantifier)
	{
		IsOnCollection = isOnCollection;
	}

	/// <summary>
	///     Whether the expectation is directly on the collection, e.g. <c>That(tracks).AreRadioFriendly()</c>, instead
	///     of on the items that a quantifier selected.
	/// </summary>
	/// <remarks>
	///     Such an expectation reads the collection to its end, so that the failure message lists all items of a kind,
	///     and its text names the quantifier first, e.g. "all are radio friendly".
	/// </remarks>
	protected bool IsOnCollection { get; }

	/// <inheritdoc cref="IAsyncContextConstraint{TValue}.IsMetBy(TValue, IEvaluationContext, CancellationToken)" />
	public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<TItem>? actual, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		OnEvaluating(context, cancellationToken);
		StartEvaluation();
		Actual = actual;
		_collection = null;
		if (actual is null)
		{
			return this;
		}

		IAsyncEnumerable<TItem> items = context.UseMaterializedAsyncEnumerable(actual, cancellationToken);
		_collection = items as IMaterializedAsyncEnumerable<TItem>;
		try
		{
			await foreach (TItem item in items)
			{
				PredicateResult result = await IsMatch(item, cancellationToken);
				if (RecordAndTryCompleteEarly(item, result, IsOnCollection))
				{
					return this;
				}
			}
		}
		catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
		{
			Outcome = Outcome.Undecided;
			return this;
		}

		Complete();
		return this;
	}

	/// <summary>
	///     Records the <paramref name="result" /> for the <paramref name="item" /> and completes the evaluation early,
	///     when the item is unanswered or, unless <paramref name="readsAllItems" />, the outcome is determined.
	/// </summary>
	/// <returns><see langword="true" />, when the evaluation is complete.</returns>
	private bool RecordAndTryCompleteEarly(TItem item, PredicateResult result, bool readsAllItems)
	{
		if (result.Reason is { } reason)
		{
			Record(item, new PredicateResult.UnansweredItem(item, reason));
			CompleteEarly();
			return true;
		}

		Record(item, result.Outcome == Outcome.Success);
		if (!readsAllItems && IsDetermined)
		{
			CompleteEarly();
			return true;
		}

		return false;
	}

	/// <summary>
	///     Called when an evaluation starts, before the collection is read, e.g. to get the options of the
	///     evaluation from the <paramref name="context" /> or to reject an unusable expected value.
	/// </summary>
	protected virtual void OnEvaluating(IEvaluationContext context, CancellationToken cancellationToken)
	{
	}

	/// <summary>
	///     Whether the <paramref name="item" /> matches the expectation, or why that cannot be answered.
	/// </summary>
	protected abstract ValueTask<PredicateResult> IsMatch(TItem item, CancellationToken cancellationToken);

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
	{
		contexts.AddCollectionContext(_collection, FormatItems);
		base.AppendContexts(contexts);
	}

	/// <inheritdoc />
	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> AppendQuantifiedExpectation(stringBuilder, indentation);

	/// <inheritdoc />
	protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> AppendQuantifiedExpectation(stringBuilder, indentation);

	/// <remarks>
	///     On the collection, the quantifier is the subject of the item expectation, as in a nested expectation, e.g.
	///     "all are radio friendly".
	/// </remarks>
	private void AppendQuantifiedExpectation(StringBuilder stringBuilder, string? indentation)
		=> Quantifier.AppendExpectation(stringBuilder,
			IsOnCollection ? Grammars | ExpectationGrammars.Nested : Grammars,
			(itemExpectation, grammars) => AppendItemExpectation(itemExpectation, grammars, indentation));
}
#endif
