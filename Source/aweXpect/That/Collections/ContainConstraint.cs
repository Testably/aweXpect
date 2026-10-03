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
	private readonly Action<ResultContextCollector>? _appendOptionsContexts;
	private readonly Func<TItem, ValueTask<bool>>? _asyncPredicate;
	private readonly TItem? _expected;
	private readonly Func<Quantifier, ExpectationGrammars, string> _expectationText;
	private readonly bool _hasExpected;
	private readonly Func<TItem, bool>? _predicate;
	private object? _actual;
	private int _count;
	private TItem? _firstFoundItem;
	private bool _isFinished;
	private bool _isNegated;

	protected ContainConstraintBase(
		string it,
		ExpectationGrammars grammars,
		Func<Quantifier, ExpectationGrammars, string> expectationText,
		Func<TItem, bool> predicate,
		Quantifier quantifier) : base(grammars)
	{
		It = it;
		_expectationText = expectationText;
		_predicate = predicate;
		Quantifier = quantifier;
	}

	protected ContainConstraintBase(
		string it,
		ExpectationGrammars grammars,
		Func<Quantifier, ExpectationGrammars, string> expectationText,
		TItem expected,
		Func<TItem, ValueTask<bool>> predicate,
		Quantifier quantifier,
		Action<ResultContextCollector>? appendOptionsContexts) : base(grammars)
	{
		It = it;
		_expectationText = expectationText;
		_expected = expected;
		_hasExpected = true;
		_asyncPredicate = predicate;
		Quantifier = quantifier;
		_appendOptionsContexts = appendOptionsContexts;
	}

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

	/// <inheritdoc cref="ConstraintResult.Outcome" />
	public override Outcome Outcome
	{
		get => _actual is null ? Outcome.Failure : base.Outcome;
		protected set => base.Outcome = value;
	}

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
		=> _appendOptionsContexts?.Invoke(contexts);

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
			Outcome = Outcome.Failure;
		}
	}

	/// <summary>
	///     Whether the <paramref name="item" /> matches.
	/// </summary>
	protected ValueTask<bool> Matches(TItem item)
		=> _asyncPredicate is null
			? new ValueTask<bool>(UserCode.Invoke(_predicate!, item, "the predicate"))
			: _asyncPredicate(item);

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
		_firstFoundItem = _expected;
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
		=> stringBuilder.Append(_expectationText.Invoke(Quantifier, Grammars));

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
			stringBuilder.Append(It).Append(" contained ");
			if (_hasExpected)
			{
				Formatter.Format(stringBuilder, _count == 1 ? _firstFoundItem : _expected);
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
///     The <c>lookup</c> asks a set subject directly whether it contains the expected item, or returns
///     <see langword="null" /> when the items have to be enumerated instead.
/// </remarks>
internal sealed class ContainConstraint<TEnumerable, TItem>
	: ContainConstraintBase<TItem>,
		IAsyncContextConstraint<TEnumerable>
	where TEnumerable : IEnumerable?
{
	private readonly Func<object, bool?>? _lookup;
	private CollectionContext _collectionContext;

	public ContainConstraint(
		string it,
		ExpectationGrammars grammars,
		Func<Quantifier, ExpectationGrammars, string> expectationText,
		Func<TItem, bool> predicate,
		Quantifier quantifier)
		: base(it, grammars, expectationText, predicate, quantifier)
	{
	}

	public ContainConstraint(
		string it,
		ExpectationGrammars grammars,
		Func<Quantifier, ExpectationGrammars, string> expectationText,
		TItem expected,
		Func<TItem, ValueTask<bool>> predicate,
		Quantifier quantifier,
		Func<object, bool?>? lookup = null,
		Action<ResultContextCollector>? appendOptionsContexts = null)
		: base(it, grammars, expectationText, expected, predicate, quantifier, appendOptionsContexts)
	{
		_lookup = lookup;
	}

	/// <inheritdoc />
	protected override Type CollectionType => typeof(IEnumerable<TItem>);

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
		Start(actual);
		if (actual.IsDefaultImmutableArray())
		{
			return this.AsNullSubject(It);
		}

		if (actual is null)
		{
			return this;
		}

		if (_lookup?.Invoke(actual) is { } isContained)
		{
			CountExpected(isContained);
			CollectionItems<TItem>.Of(actual).SetContext(ref _collectionContext);
			Finish();
			return this;
		}

		CollectionItems<TItem> materialized = CollectionItems<TItem>.Materialize(actual, context);
		foreach (TItem item in materialized.Items)
		{
			if (materialized.IsCanceledBeforeTheEnd(cancellationToken))
			{
				Outcome = Outcome.Undecided;
				materialized.SetContext(ref _collectionContext, true);
				return this;
			}

			if (await Matches(item) && CountMatch(item) is { } outcome)
			{
				Outcome = outcome;
				materialized.SetContext(ref _collectionContext);
				return this;
			}
		}

		materialized.SetContext(ref _collectionContext);
		Finish();
		return this;
	}
}

#if NET8_0_OR_GREATER
internal sealed class AsyncContainConstraint<TItem>
	: ContainConstraintBase<TItem>,
		IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
{
	private CollectionContext _collectionContext;

	public AsyncContainConstraint(
		string it,
		ExpectationGrammars grammars,
		Func<Quantifier, ExpectationGrammars, string> expectationText,
		Func<TItem, bool> predicate,
		Quantifier quantifier)
		: base(it, grammars, expectationText, predicate, quantifier)
	{
	}

	public AsyncContainConstraint(
		string it,
		ExpectationGrammars grammars,
		Func<Quantifier, ExpectationGrammars, string> expectationText,
		TItem expected,
		Func<TItem, ValueTask<bool>> predicate,
		Quantifier quantifier,
		Action<ResultContextCollector>? appendOptionsContexts = null)
		: base(it, grammars, expectationText, expected, predicate, quantifier, appendOptionsContexts)
	{
	}

	/// <inheritdoc />
	protected override Type CollectionType => typeof(IAsyncEnumerable<TItem>);

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
