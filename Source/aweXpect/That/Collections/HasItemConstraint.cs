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
///     The verification of the items at the index and the texts of <c>HasItem</c>, shared by the synchronous and the
///     asynchronous collections.
/// </summary>
internal abstract class HasItemConstraintBase<TValue, TItem>
	: ConstraintResult.WithNotNullValue<TValue>
{
	private readonly Action<ResultContextCollector>? _appendOptionsContexts;
	private readonly Func<TItem, ValueTask<bool>>? _asyncPredicate;
	private readonly Func<TItem, bool>? _predicate;
	private readonly Func<string> _predicateDescription;
	private TItem? _actual;
	private bool _hasIndex;

	protected HasItemConstraintBase(
		string it,
		ExpectationGrammars grammars,
		Func<TItem, bool> predicate,
		Func<string> predicateDescription,
		CollectionIndexOptions options)
		: base(it, grammars)
	{
		_predicate = predicate;
		_predicateDescription = predicateDescription;
		Options = options;
	}

	protected HasItemConstraintBase(
		string it,
		ExpectationGrammars grammars,
		Func<TItem, ValueTask<bool>> predicate,
		Func<string> predicateDescription,
		CollectionIndexOptions options,
		Action<ResultContextCollector>? appendOptionsContexts)
		: base(it, grammars)
	{
		_asyncPredicate = predicate;
		_predicateDescription = predicateDescription;
		Options = options;
		_appendOptionsContexts = appendOptionsContexts;
	}

	/// <summary>
	///     The options for the index of the item.
	/// </summary>
	protected CollectionIndexOptions Options { get; }

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
		=> _appendOptionsContexts?.Invoke(contexts);

	/// <summary>
	///     Starts a new evaluation, which fails unless an item at the index matches.
	/// </summary>
	protected void Start()
	{
		_actual = default;
		_hasIndex = false;
		Outcome = Outcome.Failure;
	}

	/// <summary>
	///     Verifies the <paramref name="item" /> at the <paramref name="index" /> of a collection with
	///     <paramref name="count" /> items and returns <see langword="true" />, when the remaining items cannot change the
	///     outcome.
	/// </summary>
	protected async ValueTask<bool> IsDecidedBy(TItem item, int index, int? count)
	{
		bool? isIndexInRange = ThatEnumerable.IsIndexInRange(Options, index, count);
		if (isIndexInRange is null)
		{
			return false;
		}

		if (isIndexInRange == false)
		{
			return true;
		}

		_hasIndex = true;
		_actual = item;
		bool isMatch = _asyncPredicate is null
			? UserCode.Invoke(_predicate!, item, "the predicate")
			: await _asyncPredicate(item);
		Outcome = isMatch ? Outcome.Success : Outcome.Failure;
		return isMatch;
	}

	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append(Grammars.Verb("has an item ", "have an item ")).Append(_predicateDescription())
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
			.Append(_predicateDescription())
			.Append(Options.Match.GetDescription());

	protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
	{
		stringBuilder.Append(It).Append(" had item ");
		Formatter.Format(stringBuilder, _actual);
		stringBuilder.Append(Options.Match.GetDescription());
	}
}

internal sealed class HasItemConstraint<TEnumerable, TItem>
	: HasItemConstraintBase<TEnumerable, TItem>,
		IAsyncContextConstraint<TEnumerable>
	where TEnumerable : IEnumerable?
{
	private readonly Func<object?, bool>? _useComparerOf;
	private CollectionContext _collectionContext;

	public HasItemConstraint(
		string it,
		ExpectationGrammars grammars,
		Func<TItem, bool> predicate,
		Func<string> predicateDescription,
		CollectionIndexOptions options)
		: base(it, grammars, predicate, predicateDescription, options)
	{
	}

	public HasItemConstraint(
		string it,
		ExpectationGrammars grammars,
		Func<TItem, ValueTask<bool>> predicate,
		Func<string> predicateDescription,
		CollectionIndexOptions options,
		Func<object?, bool>? useComparerOf = null,
		Action<ResultContextCollector>? appendOptionsContexts = null)
		: base(it, grammars, predicate, predicateDescription, options, appendOptionsContexts)
	{
		_useComparerOf = useComparerOf;
	}

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
	{
		_collectionContext.AppendTo(contexts);
		base.AppendContexts(contexts);
	}

	public async Task<ConstraintResult> IsMetBy(TEnumerable actual, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		_collectionContext = default;
		Actual = actual;
		Start();
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
		materialized.SetContext(ref _collectionContext);

		IEnumerable<TItem> items = materialized.Items;
		if (!ThatEnumerable.TryCountForIndex(Options, actual, items, cancellationToken, out int? count))
		{
			Outcome = Outcome.Undecided;
			return this;
		}

		int index = 0;
		foreach (TItem item in items)
		{
			if (materialized.IsCanceledBeforeTheEnd(cancellationToken))
			{
				Outcome = Outcome.Undecided;
				return this;
			}

			if (await IsDecidedBy(item, index++, count))
			{
				break;
			}
		}

		return this;
	}
}

#if NET8_0_OR_GREATER
internal sealed class AsyncHasItemConstraint<TItem>
	: HasItemConstraintBase<IAsyncEnumerable<TItem>?, TItem>,
		IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
{
	private CollectionContext _collectionContext;

	public AsyncHasItemConstraint(
		string it,
		ExpectationGrammars grammars,
		Func<TItem, bool> predicate,
		Func<string> predicateDescription,
		CollectionIndexOptions options)
		: base(it, grammars, predicate, predicateDescription, options)
	{
	}

	public AsyncHasItemConstraint(
		string it,
		ExpectationGrammars grammars,
		Func<TItem, ValueTask<bool>> predicate,
		Func<string> predicateDescription,
		CollectionIndexOptions options,
		Action<ResultContextCollector>? appendOptionsContexts = null)
		: base(it, grammars, predicate, predicateDescription, options, appendOptionsContexts)
	{
	}

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
	{
		_collectionContext.AppendTo(contexts);
		base.AppendContexts(contexts);
	}

	public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<TItem>? actual, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		_collectionContext = default;
		Actual = actual;
		Start();
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
			if (await IsDecidedBy(item, index++, count))
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
}
#endif
