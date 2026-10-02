#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	public partial class Elements<TItem>
	{
		/// <summary>
		///     …comply with the <paramref name="expectations" />.
		/// </summary>
		public AndOrResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
			ComplyWith(Action<IThatSubject<TItem>> expectations)
		{
			expectations.ThrowIfNull();
			return new(
				_subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
					=> new ComplyWithConstraint<TItem>(expectationBuilder, it, grammars, _quantifier, expectations)),
				_subject);
		}
	}

	public partial class Elements
	{
		/// <summary>
		///     …comply with the <paramref name="expectations" />.
		/// </summary>
		public AndOrResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>
			ComplyWith(Action<IThatSubject<string?>> expectations)
		{
			expectations.ThrowIfNull();
			return new(
				_subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
					=> new ComplyWithConstraint<string?>(expectationBuilder, it, grammars, _quantifier, expectations)),
				_subject);
		}
	}

	private sealed class ComplyWithConstraint<TItem>
		: ConstraintResult.WithNotNullValue<IAsyncEnumerable<TItem>?>,
			IAsyncContextConstraint<IAsyncEnumerable<TItem>?>,
			IExpectationTextConstraint
	{
		private readonly ExpectationBuilder _expectationBuilder;
		private readonly ExpectationGrammars _grammars;
		private readonly ComplyWithItemExpectations<TItem> _itemExpectations;
		private readonly EnumerableQuantifier _quantifier;
		private int _matchingCount;
		private LimitedCollection<TItem>? _matchingItems;
		private int _notMatchingCount;
		private LimitedCollection<TItem>? _notMatchingItems;
		private int? _totalCount;
		private ConstraintResult? _unansweredItem;
		private int _unansweredItemIndex;

		public ComplyWithConstraint(ExpectationBuilder expectationBuilder, string it, ExpectationGrammars grammars,
			EnumerableQuantifier quantifier,
			Action<IThatSubject<TItem>> expectations) : base(it, grammars)
		{
			_expectationBuilder = expectationBuilder;
			_grammars = grammars;
			_quantifier = quantifier;
			_itemExpectations = new ComplyWithItemExpectations<TItem>(quantifier, grammars, expectations);
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

		public async Task<ConstraintResult> IsMetBy(
			IAsyncEnumerable<TItem>? actual,
			IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			_unansweredItem = null;
			await _itemExpectations.PrepareExpectation(context, cancellationToken);
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			IAsyncEnumerable<TItem> materialized =
				context.UseMaterializedAsyncEnumerable<TItem>(actual, cancellationToken);
			_matchingCount = 0;
			_notMatchingCount = 0;
			LimitedCollection<TItem> items = new();
			_matchingItems = new LimitedCollection<TItem>();
			_notMatchingItems = new LimitedCollection<TItem>();

			await foreach (TItem item in materialized.UntilCancelled(cancellationToken))
			{
				ConstraintResult isMatch = await _itemExpectations.Builder.IsMetBy(item, context, cancellationToken);
				items.Add(item);
				// A canceled item expectation decides nothing, so the item must not count as not matching.
				if (isMatch.Outcome == Outcome.Undecided && cancellationToken.IsCancellationRequested)
				{
					Outcome = Outcome.Undecided;
					_expectationBuilder.AddCollectionContext(items, true);
					return this;
				}

				if (isMatch.FailsBothWays())
				{
					_unansweredItem = isMatch;
					_unansweredItemIndex = _matchingCount + _notMatchingCount;
					_expectationBuilder.AddCollectionContext(materialized as IMaterializedAsyncEnumerable<TItem>);
					return this;
				}

				if (isMatch.Outcome == Outcome.Success)
				{
					_matchingCount++;
					_matchingItems.Add(item);
				}
				else
				{
					_notMatchingCount++;
					_notMatchingItems.Add(item);
				}

				if (_quantifier.IsDeterminable(_matchingCount, _notMatchingCount))
				{
					Outcome = _quantifier.GetOutcome(_matchingCount, _notMatchingCount, _totalCount);
					AppendContexts(true);
					_expectationBuilder.AddCollectionContext(items, true);
					return this;
				}
			}

			if (cancellationToken.IsCanceledBeforeTheEndOf(materialized))
			{
				Outcome = Outcome.Undecided;
				_expectationBuilder.AddCollectionContext(items, true);
				return this;
			}

			_totalCount = _matchingCount + _notMatchingCount;
			Outcome = _quantifier.GetOutcome(_matchingCount, _notMatchingCount, _totalCount);
			AppendContexts(false);
			_expectationBuilder.AddCollectionContext(items, totalCount: _totalCount);
			return this;
		}

		public async Task<ConstraintResult> GetExpectationResult(IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			await _itemExpectations.PrepareExpectation(context, cancellationToken);
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> _itemExpectations.AppendExpectation(stringBuilder, false, indentation);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendItemsResult(stringBuilder, indentation, _grammars);

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> _itemExpectations.AppendExpectation(stringBuilder, true, indentation);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendItemsResult(stringBuilder, indentation, Grammars);

		private void AppendItemsResult(StringBuilder stringBuilder, string? indentation, ExpectationGrammars grammars)
		{
			if (_unansweredItem is not null)
			{
				stringBuilder.AppendUnansweredItem(_unansweredItem, _unansweredItemIndex, indentation);
				return;
			}

			_quantifier.AppendResult(stringBuilder, grammars, It, _matchingCount, _notMatchingCount, _totalCount,
				_itemExpectations.Builder.GetResultVerb());
		}

		private void AppendContexts(bool isIncomplete)
		{
			_expectationBuilder.AddQuantifierContexts(this, _quantifier,
				_matchingItems?.Count > 0
					? () => Formatter.Format(_matchingItems, typeof(TItem).GetFormattingOption(_matchingItems?.Count, _matchingCount))
						.AppendIsIncomplete(isIncomplete)
					: null,
				_notMatchingItems?.Count > 0
					? () => Formatter.Format(_notMatchingItems,
							typeof(TItem).GetFormattingOption(_notMatchingItems?.Count, _notMatchingCount))
						.AppendIsIncomplete(isIncomplete)
					: null);
		}
	}
}
#endif
