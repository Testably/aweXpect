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
#if NET8_0_OR_GREATER
using aweXpect.Customization;
#endif

namespace aweXpect;

/// <summary>
///     The counting of the matching items and the texts of <c>Contains</c> with a single item or a predicate, shared by
///     the synchronous and the asynchronous collections.
/// </summary>
internal abstract class ContainConstraintBase<TItem> : ConstraintResult
{
	private readonly ExpectedItem<TItem>? _expected;
	private readonly SynchronouslyMatchedItem<TItem>? _predicate;
	private object? _actual;
	private int _count;
	private TItem? _firstFoundItem;
	private bool _isFinished;
	private bool _isNegated;

	protected ContainConstraintBase(
		string it,
		ExpectationGrammars grammars,
		ContainedItem<TItem> item,
		Quantifier quantifier) : base(grammars)
	{
		It = it;
		Item = item;
		_expected = item as ExpectedItem<TItem>;
		_predicate = item as SynchronouslyMatchedItem<TItem>;
		Quantifier = quantifier;
	}

	/// <summary>
	///     What the collection is searched for.
	/// </summary>
	protected ContainedItem<TItem> Item { get; }

	/// <summary>
	///     The name of the subject.
	/// </summary>
	protected string It { get; }

	/// <summary>
	///     The quantifier for the number of matching items.
	/// </summary>
	protected Quantifier Quantifier { get; }

	/// <summary>
	///     The type of the collection that is served as the value of the result.
	/// </summary>
	protected abstract Type CollectionType { get; }

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
		=> Item.AppendContexts(contexts);

	/// <summary>
	///     Starts a new evaluation of the <paramref name="actual" /> subject.
	/// </summary>
	protected void Start(object? actual)
	{
		_actual = actual;
		_count = 0;
		_firstFoundItem = default;
		_isFinished = false;
		if (actual is null)
		{
			Outcome = Outcome.FailureBothWays;
		}
	}

	/// <summary>
	///     Whether the predicate is synchronous, so that the items can be counted without awaiting it.
	/// </summary>
	protected bool IsSynchronous => _predicate is not null;

	/// <summary>
	///     Whether the <paramref name="item" /> matches the synchronous predicate.
	/// </summary>
	protected bool MatchesSynchronously(TItem item)
		=> _predicate!.IsMatch(item);

	/// <summary>
	///     Whether the <paramref name="item" /> matches.
	/// </summary>
	protected ValueTask<bool> Matches(TItem item)
		=> Item.Matches(item);

	/// <summary>
	///     Counts the matching <paramref name="item" /> and returns the outcome, when the remaining items cannot change
	///     it.
	/// </summary>
	protected Outcome? CountMatch(TItem item)
	{
		if (++_count == 1)
		{
			_firstFoundItem = item;
		}

		return Quantifier.Check(_count, false) switch
		{
			true => Outcome.Success,
			false => Outcome.Failure,
			_ => null,
		};
	}

	/// <summary>
	///     Counts the expected item as contained or not, when a set was asked for it directly.
	/// </summary>
	protected void CountExpected(bool isContained)
	{
		_count = isContained ? 1 : 0;
		_firstFoundItem = _expected is null ? default : _expected.Expected;
	}

	/// <summary>
	///     Determines the outcome after all items were counted.
	/// </summary>
	protected void Finish()
	{
		_isFinished = true;
		Outcome = Quantifier.Check(_count, true) ?? _isNegated ? Outcome.Success : Outcome.Failure;
	}

	public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append(Item.GetExpectation(Quantifier, Grammars));

	public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
	{
		if (_actual == null)
		{
			stringBuilder.ItWasNull(It, Grammars);
		}
		else if (Outcome == Outcome.Undecided)
		{
			AppendCanceledResult(stringBuilder, It);
		}
		else if (_isFinished && _count == 0)
		{
			stringBuilder.Append(It).Append(" did not contain it");
		}
		else
		{
			AppendContainedResult(stringBuilder);
		}
	}

	private void AppendContainedResult(StringBuilder stringBuilder)
	{
		stringBuilder.Append(It).Append(" contained ");
		if (_expected is not null)
		{
			Formatter.Format(stringBuilder, _count == 1 ? _firstFoundItem : _expected.Expected);
		}
		else
		{
			stringBuilder.Append("it");
		}

		stringBuilder.Append(_isFinished ? " " : " at least ");
		if (_count == 1)
		{
			stringBuilder.Append("once");
		}
		else if (_count == 2)
		{
			stringBuilder.Append("twice");
		}
		else
		{
			stringBuilder.Append(_count).Append(" times");
		}
	}

	/// <inheritdoc cref="ConstraintResult.TryGetStoredValue{TValue}(out TValue)" />
	public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
	{
		if (_actual is TValue typedValue)
		{
			value = typedValue;
			return true;
		}

		value = default;
		return typeof(TValue).IsAssignableFrom(CollectionType);
	}

	public override ConstraintResult Negate()
	{
		_isNegated = !_isNegated;
		Quantifier.Negate();
		Outcome = Outcome switch
		{
			Outcome.Failure => Outcome.Success,
			Outcome.Success => Outcome.Failure,
			_ => Outcome,
		};
		return this;
	}
}

/// <remarks>
///     A set subject is asked directly whether it contains the item, when <see cref="ContainedItem{TItem}.LookUpIn" />
///     answers.
/// </remarks>
internal sealed class ContainConstraint<TEnumerable, TItem>(
	string it,
	ExpectationGrammars grammars,
	ContainedItem<TItem> item,
	Quantifier quantifier)
	: ContainConstraintBase<TItem>(it, grammars, item, quantifier),
		IAsyncContextConstraint<TEnumerable>
	where TEnumerable : IEnumerable?
{
	private CollectionContext _collectionContext;

	/// <inheritdoc />
	protected override Type CollectionType => typeof(IEnumerable<TItem>);

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
	{
		_collectionContext.AppendTo(contexts);
		base.AppendContexts(contexts);
	}

	public ValueTask<ConstraintResult> IsMetBy(TEnumerable actual, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		_collectionContext = default;
		Start(actual);
		if (actual.IsDefaultImmutableArray())
		{
			return new ValueTask<ConstraintResult>(this.AsNullSubject(It));
		}

		if (actual is null)
		{
			return new ValueTask<ConstraintResult>(this);
		}

		if (Item.LookUpIn(actual) is { } isContained)
		{
			CountExpected(isContained);
			return new ValueTask<ConstraintResult>(Finish(CollectionItems<TItem>.Of(actual)));
		}

		CollectionItems<TItem> materialized = CollectionItems<TItem>.Materialize(actual, context);
		return IsSynchronous
			? new ValueTask<ConstraintResult>(Count(materialized, cancellationToken))
			: CountAsync(materialized, cancellationToken);
	}

	private ContainConstraint<TEnumerable, TItem> Count(CollectionItems<TItem> materialized, CancellationToken cancellationToken)
	{
		foreach (TItem item in materialized)
		{
			if (IsCanceled(materialized, cancellationToken) ||
			    (MatchesSynchronously(item) && IsDecidedBy(materialized, item)))
			{
				return this;
			}
		}

		return Finish(materialized);
	}

	private async ValueTask<ConstraintResult> CountAsync(CollectionItems<TItem> materialized,
		CancellationToken cancellationToken)
	{
		foreach (TItem item in materialized)
		{
			if (IsCanceled(materialized, cancellationToken) ||
			    (await Matches(item) && IsDecidedBy(materialized, item)))
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

	private bool IsDecidedBy(CollectionItems<TItem> materialized, TItem item)
	{
		if (CountMatch(item) is not { } outcome)
		{
			return false;
		}

		Outcome = outcome;
		materialized.SetContext(ref _collectionContext);
		return true;
	}

	private ContainConstraint<TEnumerable, TItem> Finish(CollectionItems<TItem> items)
	{
		items.SetContext(ref _collectionContext);
		Finish();
		return this;
	}
}

#if NET8_0_OR_GREATER
internal sealed class AsyncContainConstraint<TItem>(
	string it,
	ExpectationGrammars grammars,
	ContainedItem<TItem> item,
	Quantifier quantifier)
	: ContainConstraintBase<TItem>(it, grammars, item, quantifier),
		IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
{
	private CollectionContext _collectionContext;

	/// <inheritdoc />
	protected override Type CollectionType => typeof(IAsyncEnumerable<TItem>);

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
	{
		_collectionContext.AppendTo(contexts);
		base.AppendContexts(contexts);
	}

	public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<TItem>? actual, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		_collectionContext = default;
		Start(actual);
		if (actual is null)
		{
			return this;
		}

		IAsyncEnumerable<TItem> materializedEnumerable =
			context.UseMaterializedAsyncEnumerable<TItem>(actual, cancellationToken);
		int maximumNumberOfCollectionItems =
			Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();
		LimitedCollection<TItem> items = new();
		int totalCount = 0;
		await foreach (TItem item in materializedEnumerable.UntilCancelled(cancellationToken))
		{
			totalCount++;
			if (items.Count <= maximumNumberOfCollectionItems)
			{
				items.Add(item);
			}

			if (await Matches(item) && CountMatch(item) is { } outcome)
			{
				// The verdict is final, so no further items are received only for the context.
				Outcome = outcome;
				_collectionContext.Set(items, true);
				return this;
			}
		}

		if (cancellationToken.IsCanceledBeforeTheEndOf(materializedEnumerable))
		{
			Outcome = Outcome.Undecided;
			_collectionContext.Set(items, true);
			return this;
		}

		_collectionContext.Set(items, totalCount: totalCount);
		Finish();
		return this;
	}
}
#endif
