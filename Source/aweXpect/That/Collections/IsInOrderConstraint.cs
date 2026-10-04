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
///     The comparison of consecutive items and the texts of <c>IsInAscendingOrder</c> and
///     <c>IsInDescendingOrder</c>, shared by the synchronous and the asynchronous collections.
/// </summary>
internal abstract class IsInOrderConstraintBase<TValue, TItem, TMember>(
	string it,
	ExpectationGrammars grammars,
	Func<TItem, TMember> memberAccessor,
	SortOrder sortOrder,
	CollectionOrderOptions<TMember> options,
	string memberExpression,
	Func<Func<TMember, string?>?>? createIncompatibilityCheck)
	: OrderingConstraint<TValue>(it, grammars, false)
{
	private IComparer<TMember> _comparer = null!;
	private string? _failureText;
	private Func<TMember, string?>? _incompatibilityCheck;
	private int _index;
	private TMember _previous = default!;
	private IComparer<TMember>? _subjectOrder;

	/// <summary>
	///     Starts a new evaluation, which orders the items by the <paramref name="subjectOrder" />, when it is set.
	/// </summary>
	protected void Start(IComparer<TMember>? subjectOrder)
	{
		_failureText = null;
		_subjectOrder = subjectOrder;
		_comparer = subjectOrder ?? options.GetComparer();
		_incompatibilityCheck = subjectOrder is null ? createIncompatibilityCheck?.Invoke() : null;
		_index = 0;
		_previous = default!;
		IsIncomparable = false;
	}

	/// <summary>
	///     Compares the <paramref name="item" /> with the previous one and returns <see langword="true" />, when the
	///     expectation fails.
	/// </summary>
	protected bool IsOutOfOrder(TItem item)
	{
		TMember current = UserCode.Invoke(memberAccessor, item, "the member selector");
		if (_incompatibilityCheck?.Invoke(current) is { } incompatibility)
		{
			// The order of incompatible items cannot be verified, so the negated check fails as well.
			_failureText = $"{It} {incompatibility}";
			IsIncomparable = true;
			Outcome = Outcome.FailureBothWays;
			return true;
		}

		TMember previous = _previous;
		_previous = current;
		if (_index++ == 0)
		{
			return false;
		}

		int comparisonResult = UserCode.Invoke(
			static values => values.Comparer.Compare(values.Previous, values.Current),
			(Comparer: _comparer, Previous: previous, Current: current), "the comparer");
		if ((comparisonResult > 0 && sortOrder == SortOrder.Ascending) ||
		    (comparisonResult < 0 && sortOrder == SortOrder.Descending))
		{
			_failureText =
				$"{It} had {Formatter.Format(previous)} before {Formatter.Format(current)}, which is not in {sortOrder.ToString().ToLower()} order";
			Outcome = Outcome.Failure;
			return true;
		}

		return false;
	}

	/// <summary>
	///     Returns the comparer of a sorted set <paramref name="subject" /> that orders its items, when neither a member
	///     nor a comparer is specified, or <see langword="null" /> otherwise.
	/// </summary>
	protected IComparer<TMember>? GetSubjectOrder(object subject)
		=> memberExpression.Length == 0 && !options.HasComparer
			? CollectionComparerHelpers.GetSubjectOrder(subject, options.GetComparer())
			: null;

	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		stringBuilder.Append(Grammars.Verb("is in ", "are in ")).Append(sortOrder.ToString().ToLower())
			.Append(" order");
		stringBuilder.Append(memberExpression).Append(options);
		AppendSubjectOrder(stringBuilder);
	}

	protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append(_failureText);

	protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		stringBuilder.Append(Grammars.Verb("is not in ", "are not in ")).Append(sortOrder.ToString().ToLower())
			.Append(" order");
		stringBuilder.Append(memberExpression).Append(options);
		AppendSubjectOrder(stringBuilder);
	}

	protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
	{
		if (IsIncomparable)
		{
			stringBuilder.Append(_failureText);
		}
		else
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was", " were"));
		}
	}

	private void AppendSubjectOrder(StringBuilder stringBuilder)
	{
		if (_subjectOrder is not null)
		{
			stringBuilder.Append(CollectionComparerHelpers.DescribeSubjectComparer(_subjectOrder));
		}
	}
}

/// <remarks>
///     Without a member, i.e. with an empty <paramref name="memberExpression" />, a subject that is a sorted set of
///     <typeparamref name="TMember" /> with a comparer other than the default order is ordered by that comparer, unless
///     a comparer is specified in the <paramref name="options" />, and the expectation names it.
/// </remarks>
internal sealed class IsInOrderConstraint<TEnumerable, TItem, TMember>(
	string it,
	ExpectationGrammars grammars,
	Func<TItem, TMember> memberAccessor,
	SortOrder sortOrder,
	CollectionOrderOptions<TMember> options,
	string memberExpression,
	Func<Func<TMember, string?>?>? createIncompatibilityCheck = null)
	: IsInOrderConstraintBase<TEnumerable, TItem, TMember>(it, grammars, memberAccessor, sortOrder, options,
			memberExpression, createIncompatibilityCheck),
		IAsyncContextConstraint<TEnumerable>
	where TEnumerable : IEnumerable?
{
	private CollectionContext _collectionContext;

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
		=> _collectionContext.AppendTo(contexts);

	public ValueTask<ConstraintResult> IsMetBy(TEnumerable actual, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		_collectionContext = default;
		Actual = actual;
		Start(null);
		if (actual.IsDefaultImmutableArray())
		{
			return new ValueTask<ConstraintResult>(this.AsNullSubject(It));
		}

		if (actual is null)
		{
			Outcome = Outcome.FailureBothWays;
			return new ValueTask<ConstraintResult>(this);
		}

		CollectionItems<TItem> materialized = CollectionItems<TItem>.Materialize(actual, context);
		materialized.SetContext(ref _collectionContext);
		Start(GetSubjectOrder(actual));
		foreach (TItem item in materialized)
		{
			if (materialized.IsCanceledBeforeTheEnd(cancellationToken))
			{
				Outcome = Outcome.Undecided;
				return new ValueTask<ConstraintResult>(this);
			}

			if (IsOutOfOrder(item))
			{
				return new ValueTask<ConstraintResult>(this);
			}
		}

		Outcome = Outcome.Success;
		return new ValueTask<ConstraintResult>(this);
	}
}

#if NET8_0_OR_GREATER
internal sealed class AsyncIsInOrderConstraint<TItem, TMember>(
	string it,
	ExpectationGrammars grammars,
	Func<TItem, TMember> memberAccessor,
	SortOrder sortOrder,
	CollectionOrderOptions<TMember> options,
	string memberExpression,
	Func<Func<TMember, string?>?>? createIncompatibilityCheck = null)
	: IsInOrderConstraintBase<IAsyncEnumerable<TItem>?, TItem, TMember>(it, grammars, memberAccessor, sortOrder,
			options, memberExpression, createIncompatibilityCheck),
		IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
{
	private CollectionContext _collectionContext;

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
		=> _collectionContext.AppendTo(contexts);

	public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<TItem>? actual, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		_collectionContext = default;
		Actual = actual;
		Start(null);
		if (actual is null)
		{
			Outcome = Outcome.FailureBothWays;
			return this;
		}

		IAsyncEnumerable<TItem> materialized = context
			.UseMaterializedAsyncEnumerable<TItem>(actual, cancellationToken);
		_collectionContext.Set(materialized as IMaterializedAsyncEnumerable<TItem>);
		await using IAsyncEnumerator<TItem> items =
			materialized.UntilCancelled(cancellationToken).GetAsyncEnumerator(CancellationToken.None);
		while (await items.MoveNextAsync())
		{
			if (IsOutOfOrder(items.Current))
			{
				return this;
			}
		}

		Outcome = cancellationToken.IsCanceledBeforeTheEndOf(materialized) ? Outcome.Undecided : Outcome.Success;
		return this;
	}
}
#endif
