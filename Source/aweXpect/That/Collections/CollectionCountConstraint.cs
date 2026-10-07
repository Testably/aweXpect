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
///     The outcome and the texts of <c>HasCount</c>, shared by the synchronous and the asynchronous collections.
/// </summary>
internal abstract class CollectionCountConstraintBase<TValue>(
	string it,
	ExpectationGrammars grammars,
	EnumerableQuantifier quantifier)
	: ConstraintResult.WithNotNullValue<TValue>(it, grammars)
{
	private int _count;
	private int? _totalCount;

	/// <summary>
	///     The quantifier for the number of items.
	/// </summary>
	protected EnumerableQuantifier Quantifier => quantifier;

	/// <summary>
	///     Determines the outcome from the <paramref name="count" /> of items that were read, which is the
	///     <paramref name="totalCount" />, unless the reading stopped early.
	/// </summary>
	protected void Complete(int count, int? totalCount, bool isCanceled = false)
	{
		_count = count;
		_totalCount = totalCount;
		Outcome = isCanceled ? Outcome.Undecided : quantifier.GetOutcome(count, 0, totalCount);
	}

	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		stringBuilder.Append(Grammars.Verb("has ", "have "));
		stringBuilder.Append(quantifier);
		stringBuilder.Append(' ');
		stringBuilder.Append(quantifier.GetItemString());
	}

	protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		=> quantifier.AppendResult(stringBuilder, Grammars, It, _count, 0, _totalCount);

	protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		stringBuilder.Append(Grammars.Verb("does not have ", "do not have "));
		stringBuilder.Append(quantifier);
		stringBuilder.Append(' ');
		stringBuilder.Append(quantifier.GetItemString());
	}

	protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		=> quantifier.AppendResult(stringBuilder, Grammars, It, _count, 0, _totalCount);
}

internal sealed class CollectionCountConstraint<TEnumerable, TItem>(
	string it,
	ExpectationGrammars grammars,
	EnumerableQuantifier quantifier)
	: CollectionCountConstraintBase<TEnumerable>(it, grammars, quantifier),
		IAsyncContextConstraint<TEnumerable>
	where TEnumerable : IEnumerable?
{
	private CollectionContext _collectionContext;

	public ValueTask<ConstraintResult> IsMetBy(
		TEnumerable actual,
		IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		_collectionContext = default;
		Actual = actual;
		if (actual.IsDefaultImmutableArray())
		{
			return new ValueTask<ConstraintResult>(this.AsNullSubject(It));
		}

		if (actual is null)
		{
			Outcome = Outcome.FailureBothWays;
			return new ValueTask<ConstraintResult>(this);
		}

		if (CollectionItems<TItem>.CountOf(actual) is { } totalCount)
		{
			CollectionItems<TItem>.Of(actual).SetContext(ref _collectionContext);
			Complete(totalCount, totalCount);
			return new ValueTask<ConstraintResult>(this);
		}

		CollectionItems<TItem> materialized = CollectionItems<TItem>.Materialize(actual, context);
		int count = 0;
		foreach (TItem _ in materialized)
		{
			if (materialized.IsCanceledBeforeTheEnd(cancellationToken))
			{
				materialized.SetContext(ref _collectionContext, true);
				Complete(count, null, true);
				return new ValueTask<ConstraintResult>(this);
			}

			count++;
			if (Quantifier.IsDeterminable(count, 0))
			{
				materialized.SetContext(ref _collectionContext);
				Complete(count, null);
				return new ValueTask<ConstraintResult>(this);
			}
		}

		materialized.SetContext(ref _collectionContext);
		Complete(count, count);
		return new ValueTask<ConstraintResult>(this);
	}

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
		=> _collectionContext.AppendTo(contexts);
}

#if NET8_0_OR_GREATER
internal sealed class AsyncCollectionCountConstraint<TItem>(
	string it,
	ExpectationGrammars grammars,
	EnumerableQuantifier quantifier)
	: CollectionCountConstraintBase<IAsyncEnumerable<TItem>?>(it, grammars, quantifier),
		IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
{
	private CollectionContext _collectionContext;

	public async ValueTask<ConstraintResult> IsMetBy(
		IAsyncEnumerable<TItem>? actual,
		IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		_collectionContext = default;
		Actual = actual;
		if (actual is null)
		{
			Outcome = Outcome.FailureBothWays;
			return this;
		}

		IAsyncEnumerable<TItem> materialized =
			context.UseMaterializedAsyncEnumerable<TItem>(actual, cancellationToken);
		int count = 0;
		await foreach (TItem _ in materialized.UntilCancelled(cancellationToken))
		{
			count++;
			if (Quantifier.IsDeterminable(count, 0))
			{
				_collectionContext.Set(materialized as IMaterializedAsyncEnumerable<TItem>);
				Complete(count, null);
				return this;
			}
		}

		if (cancellationToken.IsCanceledBeforeTheEndOf(materialized))
		{
			_collectionContext.Set(materialized as IMaterializedAsyncEnumerable<TItem>, true);
			Complete(count, null, true);
			return this;
		}

		_collectionContext.Set(materialized as IMaterializedAsyncEnumerable<TItem>);
		Complete(count, count);
		return this;
	}

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
		=> _collectionContext.AppendTo(contexts);
}
#endif
