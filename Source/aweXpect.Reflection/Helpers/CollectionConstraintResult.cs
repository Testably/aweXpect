using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Reflection.Formatting;

namespace aweXpect.Reflection.Helpers;

/// <summary>
///     A typed <see cref="ConstraintResult" /> which is used to split a collection of <typeparamref name="T" /> in a
///     matching and not matching group.
/// </summary>
/// <remarks>
///     The collection is read through the <see cref="IEvaluationContext" />, so that it is enumerated only once, also
///     when several expectations are combined. A <see langword="null" /> collection fails the expectation and its
///     negation alike, and so does a <see langword="null" /> item, which cannot be inspected, without reaching the
///     predicate.
/// </remarks>
internal abstract class CollectionConstraintResult<T>(string it, ExpectationGrammars grammars)
	: ConstraintResult(grammars)
{
	private IEnumerable<T>? _collectionWithNullItem;
	private object? _items;
	private List<T>? _matching;
	private List<T>? _notMatching;
	private int? _nullItemIndex;
	private Outcome _outcome = Outcome.Undecided;
	private Type? _subjectType;

	/// <summary>
	///     The name of the subject.
	/// </summary>
	protected string It { get; } = it;

	/// <summary>
	///     Flag indicating if the constraint is negated.
	/// </summary>
	protected bool IsNegated { get; private set; }

	/// <inheritdoc />
	public override Outcome Outcome
	{
		get => (_outcome, _isNegated: IsNegated) switch
		{
			(Outcome.Failure, true) => Outcome.Success,
			(Outcome.Success, true) => Outcome.Failure,
			(_, _) => _outcome,
		};
		protected set => _outcome = value;
	}

	/// <inheritdoc />
	public override string? LeadingSubject => GetSubjectOfResult(It);

	/// <inheritdoc />
	public override string? TrailingSubject => GetSubjectOfResult(It);

	/// <summary>
	///     The matching items of the last evaluation.
	/// </summary>
	/// <remarks>
	///     The items are only copied into an array when they are needed for a failure.
	/// </remarks>
	protected T[] Matching => _matching?.ToArray() ?? [];

	/// <summary>
	///     The not matching items of the last evaluation.
	/// </summary>
	protected T[] NotMatching => _notMatching?.ToArray() ?? [];

	/// <summary>
	///     Appends how many items matched in the wording of the generated expectations, e.g. "only 1 of 3 did".
	/// </summary>
	protected void AppendMatchingCount(StringBuilder stringBuilder)
	{
		int matching = _matching?.Count ?? 0;
		int total = matching + (_notMatching?.Count ?? 0);
		if (IsNegated)
		{
			stringBuilder.Append("all ").Append(total).Append(" did");
		}
		else if (matching == 0)
		{
			stringBuilder.Append("none of ").Append(total).Append(" did");
		}
		else
		{
			stringBuilder.Append("only ").Append(matching).Append(" of ").Append(total).Append(" did");
		}
	}

	/// <summary>
	///     Splits the <paramref name="items" /> according to the <paramref name="predicate" /> into
	///     <see cref="Matching" /> and <see cref="NotMatching" />.
	/// </summary>
	protected ConstraintResult SetValue(IEnumerable<T>? items, IEvaluationContext context,
		Func<T, bool> predicate)
	{
		if (!StartEvaluation(items, typeof(IEnumerable<T>)))
		{
			return this;
		}

		List<T> matching = [];
		List<T>? notMatching = null;
		IEnumerable<T> materialized = context.UseMaterializedEnumerable(items!);
		int index = 0;
		foreach (T item in materialized)
		{
			if (item is null)
			{
				return FailForNullItem(index, materialized);
			}

			Add(predicate(item), item, matching, ref notMatching);
			index++;
		}

		return Complete(matching, notMatching);
	}

	/// <summary>
	///     Splits the <paramref name="items" /> according to the <paramref name="predicate" /> into
	///     <see cref="Matching" /> and <see cref="NotMatching" />.
	/// </summary>
	/// <remarks>
	///     Stops at a cancellation of the <paramref name="cancellationToken" /> and leaves the outcome undecided.
	/// </remarks>
	protected async ValueTask<ConstraintResult> SetValue(IEnumerable<T>? items, IEvaluationContext context,
		Func<T, ValueTask<bool>> predicate, CancellationToken cancellationToken)
	{
		if (!StartEvaluation(items, typeof(IEnumerable<T>)))
		{
			return this;
		}

		List<T> matching = [];
		List<T>? notMatching = null;
		IEnumerable<T> materialized = context.UseMaterializedEnumerable(items!);
		int index = 0;
		foreach (T item in materialized)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return this;
			}

			if (item is null)
			{
				return FailForNullItem(index, materialized);
			}

			Add(await predicate(item), item, matching, ref notMatching);
			index++;
		}

		return Complete(matching, notMatching);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Splits the <paramref name="items" /> according to the <paramref name="predicate" /> into
	///     <see cref="Matching" /> and <see cref="NotMatching" />.
	/// </summary>
	/// <remarks>
	///     Stops at a cancellation of the <paramref name="cancellationToken" /> and leaves the outcome undecided.
	/// </remarks>
	protected ValueTask<ConstraintResult> SetAsyncValue(IAsyncEnumerable<T>? items, IEvaluationContext context,
		Func<T, bool> predicate, CancellationToken cancellationToken)
		=> SetAsyncValue(items, context, item => new ValueTask<bool>(predicate(item)), cancellationToken);

	/// <summary>
	///     Splits the <paramref name="items" /> according to the <paramref name="predicate" /> into
	///     <see cref="Matching" /> and <see cref="NotMatching" />.
	/// </summary>
	/// <remarks>
	///     Stops at a cancellation of the <paramref name="cancellationToken" /> and leaves the outcome undecided.
	/// </remarks>
	protected async ValueTask<ConstraintResult> SetAsyncValue(IAsyncEnumerable<T>? items,
		IEvaluationContext context, Func<T, ValueTask<bool>> predicate, CancellationToken cancellationToken)
	{
		if (!StartEvaluation(items, typeof(IAsyncEnumerable<T>)))
		{
			return this;
		}

		List<T> matching = [];
		List<T>? notMatching = null;
		IAsyncEnumerable<T> materialized = context.UseMaterializedAsyncEnumerable(items!, cancellationToken);
		int index = 0;
		int? nullItemIndex = null;
		try
		{
			await foreach (T item in materialized)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return this;
				}

				// After a null item, the remaining items are only read to list the whole collection in the failure.
				if (nullItemIndex is not null)
				{
					continue;
				}

				if (item is null)
				{
					nullItemIndex = index;
					continue;
				}

				Add(await predicate(item), item, matching, ref notMatching);
				index++;
			}
		}
		catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
		{
			return this;
		}

		if (nullItemIndex is not null)
		{
			return FailForNullItem(nullItemIndex.Value,
				(materialized as IMaterializedAsyncEnumerable<T>)?.MaterializedItems);
		}

		return Complete(matching, notMatching);
	}
#endif

	/// <summary>
	///     Appends the expectation to the <paramref name="stringBuilder" /> when the <see cref="ExpectationGrammars" /> are
	///     not negated.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected abstract void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null);

	/// <summary>
	///     Appends the result to the <paramref name="stringBuilder" /> when the <see cref="ExpectationGrammars" /> are not
	///     negated.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected abstract void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null);

	/// <summary>
	///     Appends the expectation to the <paramref name="stringBuilder" /> when the <see cref="ExpectationGrammars" /> are
	///     negated.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected abstract void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null);

	/// <summary>
	///     Appends the result to the <paramref name="stringBuilder" /> when the <see cref="ExpectationGrammars" /> are
	///     negated.
	/// </summary>
	/// <remarks>
	///     By default, the result is described like the normal one, see <see cref="AppendNormalResult" />.
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected virtual void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		=> AppendNormalResult(stringBuilder, indentation);

	/// <inheritdoc cref="ConstraintResult.AppendExpectation(StringBuilder, string?)" />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		if (IsNegated)
		{
			AppendNegatedExpectation(stringBuilder, indentation);
		}
		else
		{
			AppendNormalExpectation(stringBuilder, indentation);
		}
	}

	/// <inheritdoc cref="ConstraintResult.AppendResult(StringBuilder, string?)" />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
	{
		if (_outcome == Outcome.FailureBothWays && _nullItemIndex is not null)
		{
			stringBuilder.Append("for the item at index ").Append(_nullItemIndex.Value).Append(", it was <null>");
		}
		else if (_outcome == Outcome.FailureBothWays)
		{
			stringBuilder.Append(It).Append(Grammars.IsPlural() && It != "it" ? " were <null>" : " was <null>");
		}
		else if (_outcome == Outcome.Undecided)
		{
			AppendCanceledResult(stringBuilder, It);
		}
		else if (IsNegated)
		{
			AppendNegatedResult(stringBuilder, indentation);
		}
		else
		{
			AppendNormalResult(stringBuilder, indentation);
		}
	}

	/// <inheritdoc />
	/// <remarks>
	///     Adds the not matching items, which explain the failure. A negated expectation only fails when all items
	///     match, so it adds the whole collection instead.
	/// </remarks>
	public override void AppendContexts(ResultContextCollector contexts)
	{
		if (_outcome == Outcome.FailureBothWays && _collectionWithNullItem is { } collection)
		{
			contexts.Add(new ResultContext.SyncCallback("Collection", () => FormatCollection(collection), -1));
			return;
		}

		if (Outcome != Outcome.Failure)
		{
			return;
		}

		if (IsNegated)
		{
			contexts.Add(new ResultContext.SyncCallback("Collection", FormatItems(Matching), -1));
		}
		else
		{
			contexts.Add(new ResultContext.SyncCallback("Not matching items", FormatItems(NotMatching),
				int.MaxValue));
		}
	}

	/// <summary>
	///     Returns the callback that formats the <paramref name="items" /> for the context.
	/// </summary>
	/// <remarks>
	///     The constraint can be evaluated again before the callback is invoked, so the callback must only use the
	///     state that is captured when this method is called.
	/// </remarks>
	protected virtual Func<string?> FormatItems(T[] items)
		=> () => FormatCollection(items);

	private static string FormatCollection(IEnumerable<T> items)
		=> Formatter.Format(FormattableMember.FromAll(items), FormattingOptions.MultipleLines);

	/// <inheritdoc cref="ConstraintResult.TryGetStoredValue{TValue}(out TValue)" />
	public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
	{
		if (_items is TValue typedValue)
		{
			value = typedValue;
			return true;
		}

		value = default;
		return _subjectType is not null && typeof(TValue).IsAssignableFrom(_subjectType);
	}

	/// <inheritdoc cref="ConstraintResult.NegateInPlace()" />
	protected override void NegateInPlace()
	{
		IsNegated = !IsNegated;
	}

	private bool StartEvaluation(object? items, Type subjectType)
	{
		_items = items;
		_subjectType = subjectType;
		_matching = null;
		_notMatching = null;
		_nullItemIndex = null;
		_collectionWithNullItem = null;
		_outcome = items is null ? Outcome.FailureBothWays : Outcome.Undecided;
		return items is not null;
	}

	/// <remarks>
	///     The <paramref name="collection" /> is listed in the failure, as the index alone does not show the item.
	/// </remarks>
	private CollectionConstraintResult<T> FailForNullItem(int index, IEnumerable<T>? collection)
	{
		_nullItemIndex = index;
		_collectionWithNullItem = collection;
		_outcome = Outcome.FailureBothWays;
		return this;
	}

	private CollectionConstraintResult<T> Complete(List<T> matching, List<T>? notMatching)
	{
		_matching = matching;
		_notMatching = notMatching;
		_outcome = notMatching is null ? Outcome.Success : Outcome.Failure;
		return this;
	}

	/// <remarks>
	///     The list of the not matching items is only created for the first one, as most evaluations have none.
	/// </remarks>
	private static void Add(bool isMatching, T item, List<T> matching, ref List<T>? notMatching)
	{
		if (isMatching)
		{
			matching.Add(item);
		}
		else
		{
			(notMatching ??= []).Add(item);
		}
	}
}
