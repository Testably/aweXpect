using System;
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
///     The constraint of a quantified <c>ComplyWith</c>, which classifies each item by the item expectations.
/// </summary>
/// <remarks>
///     In a nested expectation, the quantifier is the subject of the item expectations, so their verb agrees with its
///     number (<c>none start with …</c>, <c>at least one starts with …</c>). The negation can change this number, so
///     the negated text then comes from a second set of item expectations, which is only used for the text.
/// </remarks>
internal abstract class ComplyWithConstraint<TValue, TItem>
	: QuantifiedCollectionConstraintBase<TValue, TItem>,
		IExpectationTextConstraint
{
	private readonly ManualExpectationBuilder<TItem> _builder;
#if NET8_0_OR_GREATER
	private CollectionContext _asyncCollectionContext;
#endif
	private readonly ManualExpectationBuilder<TItem> _negatedBuilder;
	private ConstraintResult? _unansweredItem;
	private int _unansweredItemIndex;

	protected ComplyWithConstraint(string it, ExpectationGrammars grammars, EnumerableQuantifier quantifier,
		Action<IThatSubject<TItem>> expectations)
		: base(it, grammars, quantifier)
	{
		// Without a nested quantifier, the item expectations keep the number of the subject that a connector such as
		// "whose values" introduced.
		if (!grammars.IsNested())
		{
			_builder = Create(grammars, expectations);
			_negatedBuilder = _builder;
			return;
		}

		ExpectationGrammars itemGrammars = quantifier.GetItemGrammars(grammars & ~ExpectationGrammars.Negated);
		ExpectationGrammars negatedItemGrammars = quantifier.GetItemGrammars(grammars | ExpectationGrammars.Negated);
		_builder = Create(itemGrammars, expectations);
		_negatedBuilder = negatedItemGrammars == itemGrammars ? _builder : Create(negatedItemGrammars, expectations);
	}

	/// <inheritdoc cref="ConstraintResult.Outcome" />
	/// <remarks>
	///     An item that the expectations did not answer fails the expectation and its negation alike.
	/// </remarks>
	public override Outcome Outcome
	{
		get => _unansweredItem is null ? base.Outcome : Outcome.Failure;
		protected set => base.Outcome = value;
	}

	/// <inheritdoc cref="ConstraintResult.FailureCause" />
	public override Exception? FailureCause => _unansweredItem?.FailureCause;

	/// <inheritdoc />
	protected override string Verb => _builder.GetResultVerb();

	/// <inheritdoc cref="IExpectationTextConstraint.GetExpectationResult(IEvaluationContext, CancellationToken)" />
	public async Task<ConstraintResult> GetExpectationResult(IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		await PrepareExpectation(context, cancellationToken);
		return this;
	}

	/// <summary>
	///     Prepares the item expectations for a new evaluation.
	/// </summary>
	private protected async Task PrepareExpectation(IEvaluationContext context, CancellationToken cancellationToken)
	{
		_unansweredItem = null;
		await _builder.PrepareExpectation(context, cancellationToken);
		if (!ReferenceEquals(_negatedBuilder, _builder))
		{
			await _negatedBuilder.PrepareExpectation(context, cancellationToken);
		}
	}

	/// <summary>
	///     Classifies the <paramref name="items" /> by the item expectations.
	/// </summary>
	/// <param name="items">The items of the collection.</param>
	/// <param name="cancelEarly">Stops reading the items as soon as they determine the outcome.</param>
	/// <param name="isCanceledBeforeTheEnd">Tells if the evaluation was canceled before the last item.</param>
	/// <param name="addCollectionContext">
	///     Adds the "Collection" context, which is incomplete when it receives <see langword="true" />.
	/// </param>
	/// <param name="context">The evaluation context.</param>
	/// <param name="cancellationToken">The cancellation token of the evaluation.</param>
	private protected async Task<ConstraintResult> IsMetByItems(IEnumerable<TItem> items, bool cancelEarly,
		Func<bool> isCanceledBeforeTheEnd, Action<bool> addCollectionContext, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		int index = 0;
		foreach (TItem item in items)
		{
			ConstraintResult isMatch = await _builder.IsMetBy(item, context, cancellationToken);
			if (StopsAt(isMatch, index, cancellationToken))
			{
				addCollectionContext(_unansweredItem is null);
				return this;
			}

			index++;

			Record(item, isMatch);
			if (cancelEarly && IsDetermined)
			{
				CompleteEarly();
				addCollectionContext(false);
				return this;
			}

			if (isCanceledBeforeTheEnd())
			{
				Outcome = Outcome.Undecided;
				addCollectionContext(true);
				return this;
			}
		}

		Complete();
		addCollectionContext(false);
		return this;
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Classifies the items of the <paramref name="materialized" /> asynchronous enumerable by the item expectations.
	/// </summary>
	private protected async Task<ConstraintResult> IsMetByItems(IAsyncEnumerable<TItem> materialized,
		IEvaluationContext context, CancellationToken cancellationToken)
	{
		_asyncCollectionContext = default;
		LimitedCollection<TItem> items = new();
		int count = 0;
		await foreach (TItem item in materialized.UntilCancelled(cancellationToken))
		{
			ConstraintResult isMatch = await _builder.IsMetBy(item, context, cancellationToken);
			items.Add(item);
			if (StopsAt(isMatch, count, cancellationToken))
			{
				if (_unansweredItem is null)
				{
					_asyncCollectionContext.Set(items, true);
				}
				else
				{
					_asyncCollectionContext.Set(materialized as IMaterializedAsyncEnumerable<TItem>);
				}

				return this;
			}

			count++;

			Record(item, isMatch);
			if (IsDetermined)
			{
				CompleteEarly();
				_asyncCollectionContext.Set(items, true);
				return this;
			}
		}

		if (cancellationToken.IsCanceledBeforeTheEndOf(materialized))
		{
			Outcome = Outcome.Undecided;
			_asyncCollectionContext.Set(items, true);
			return this;
		}

		Complete();
		_asyncCollectionContext.Set(items, totalCount: count);
		return this;
	}
#endif

#if NET8_0_OR_GREATER
	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
	{
		_asyncCollectionContext.AppendTo(contexts);
		base.AppendContexts(contexts);
	}

#endif
	/// <inheritdoc />
	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		base.AppendNormalExpectation(stringBuilder, indentation);
		_builder.AppendReasons(stringBuilder);
	}

	/// <inheritdoc />
	protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
	{
		if (_unansweredItem is null)
		{
			base.AppendNormalResult(stringBuilder, indentation);
		}
		else
		{
			stringBuilder.AppendUnansweredItem(_unansweredItem, _unansweredItemIndex, indentation);
		}
	}

	/// <inheritdoc />
	protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		base.AppendNegatedExpectation(stringBuilder, indentation);
		_negatedBuilder.AppendReasons(stringBuilder);
	}

	/// <inheritdoc />
	protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
	{
		if (_unansweredItem is null)
		{
			base.AppendNegatedResult(stringBuilder, indentation);
		}
		else
		{
			stringBuilder.AppendUnansweredItem(_unansweredItem, _unansweredItemIndex, indentation);
		}
	}

	/// <inheritdoc />
	protected override void AppendItemExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars,
		string? indentation)
		=> (Grammars.IsNegated() ? _negatedBuilder : _builder).AppendExpectation(stringBuilder, indentation);

	/// <summary>
	///     Stops the evaluation at an item that a cancellation left undecided, as it must not count as not matching, or
	///     that the item expectations did not answer.
	/// </summary>
	private bool StopsAt(ConstraintResult isMatch, int index, CancellationToken cancellationToken)
	{
		if (isMatch.Outcome == Outcome.Undecided && cancellationToken.IsCancellationRequested)
		{
			Outcome = Outcome.Undecided;
			return true;
		}

		if (isMatch.FailsBothWays())
		{
			_unansweredItem = isMatch;
			_unansweredItemIndex = index;
			return true;
		}

		return false;
	}

	private static ManualExpectationBuilder<TItem> Create(ExpectationGrammars itemGrammars,
		Action<IThatSubject<TItem>> expectations)
	{
		ManualExpectationBuilder<TItem> builder = new(itemGrammars);
		expectations.Invoke(new ThatSubject<TItem>(builder));
		return builder;
	}
}
