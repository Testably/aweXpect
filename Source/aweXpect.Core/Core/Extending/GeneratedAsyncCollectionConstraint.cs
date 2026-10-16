#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
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
///     The items are only read until the outcome is determined.
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

	/// <inheritdoc cref="IAsyncContextConstraint{TValue}.IsMetBy(TValue, IEvaluationContext, CancellationToken)" />
	public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<TItem>? actual, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
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
				if (result.Reason is { } reason)
				{
					Record(item, new PredicateResult.UnansweredItem(item, reason));
					CompleteEarly();
					return this;
				}

				Record(item, result.Outcome == Outcome.Success);
				if (IsDetermined)
				{
					CompleteEarly();
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
	///     Whether the <paramref name="item" /> matches the expectation, or why that cannot be answered.
	/// </summary>
	protected abstract ValueTask<PredicateResult> IsMatch(TItem item, CancellationToken cancellationToken);

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
	{
		contexts.AddCollectionContext(_collection);
		base.AppendContexts(contexts);
	}
}
#endif
