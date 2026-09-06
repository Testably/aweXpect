#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Customization;
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
		public ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
			ComplyWith(Action<IThatSubject<TItem>> expectations)
		{
			ObjectEqualityOptions<TItem> options = new();
			return new ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
				_subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
					=> new ComplyWithConstraint<TItem>(expectationBuilder, it, grammars, _quantifier, expectations)),
				_subject,
				options);
		}
	}

	public partial class Elements
	{
		/// <summary>
		///     …comply with the <paramref name="expectations" />.
		/// </summary>
		public ObjectEqualityResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>, string?>
			ComplyWith(Action<IThatSubject<string?>> expectations)
		{
			ObjectEqualityOptions<string?> options = new();
			return new ObjectEqualityResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>, string?>(
				_subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
					=> new ComplyWithConstraint<string?>(expectationBuilder, it, grammars, _quantifier, expectations)),
				_subject,
				options);
		}
	}

	private sealed class ComplyWithConstraint<TItem>
		: ConstraintResult.WithValue<IAsyncEnumerable<TItem>?>,
			IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
	{
		private readonly ExpectationBuilder _expectationBuilder;
		private readonly ExpectationGrammars _grammars;
		private readonly string _it;
		private readonly ManualExpectationBuilder<TItem> _itemExpectationBuilder;
		private readonly EnumerableQuantifier _quantifier;
		private int _matchingCount;
		private LimitedCollection<TItem>? _matchingItems;
		private int _notMatchingCount;
		private LimitedCollection<TItem>? _notMatchingItems;
		private int? _totalCount;

		public ComplyWithConstraint(ExpectationBuilder expectationBuilder, string it, ExpectationGrammars grammars,
			EnumerableQuantifier quantifier,
			Action<IThatSubject<TItem>> expectations) : base(grammars)
		{
			_expectationBuilder = expectationBuilder;
			_it = it;
			_grammars = grammars;
			_quantifier = quantifier;
			_itemExpectationBuilder = new ManualExpectationBuilder<TItem>(null, grammars);
			expectations.Invoke(new ThatSubject<TItem>(_itemExpectationBuilder));
		}

		public async Task<ConstraintResult> IsMetBy(
			IAsyncEnumerable<TItem>? actual,
			IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			IAsyncEnumerable<TItem> materialized =
				context.UseMaterializedAsyncEnumerable<TItem, IAsyncEnumerable<TItem>>(actual);
			_matchingCount = 0;
			_notMatchingCount = 0;
			int maxItems = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get() + 1;
			LimitedCollection<TItem> items = new(maxItems);
			_matchingItems = new LimitedCollection<TItem>(maxItems);
			_notMatchingItems = new LimitedCollection<TItem>(maxItems);

			await foreach (TItem item in materialized.WithCancellation(cancellationToken))
			{
				ConstraintResult isMatch = await _itemExpectationBuilder.IsMetBy(item, context, cancellationToken);
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

				items.Add(item);

				// items.IsReadOnly is set to true, once the limit is reached.
				if (_quantifier.IsDeterminable(_matchingCount, _notMatchingCount) && items.IsReadOnly)
				{
					Outcome = _quantifier.GetOutcome(_matchingCount, _notMatchingCount, _totalCount);
					AppendContexts(true);
					_expectationBuilder.AddCollectionContext(items, true);
					return this;
				}
			}

			if (cancellationToken.IsCancellationRequested)
			{
				Outcome = Outcome.Undecided;
				_expectationBuilder.AddCollectionContext(items, true);
				return this;
			}

			_totalCount = _matchingCount + _notMatchingCount;
			Outcome = _quantifier.GetOutcome(_matchingCount, _notMatchingCount, _totalCount);
			AppendContexts(false);
			_expectationBuilder.AddCollectionContext(items);
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			_itemExpectationBuilder.AppendExpectation(stringBuilder, indentation);
			stringBuilder.Append(" for ");
			stringBuilder.Append(_quantifier);
			stringBuilder.Append(' ');
			stringBuilder.Append(_quantifier.GetItemString());
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.ItWasNull(_it);
			}
			else
			{
				_quantifier.AppendResult(stringBuilder, _grammars, _matchingCount, _notMatchingCount, _totalCount);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(_quantifier);
			stringBuilder.Append(" for ");
			_itemExpectationBuilder.AppendExpectation(stringBuilder, indentation);
			stringBuilder.Append(' ');
			stringBuilder.Append(_quantifier.GetItemString());
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.ItWasNull(_it);
			}
			else
			{
				_quantifier.AppendResult(stringBuilder, _grammars, _matchingCount, _notMatchingCount, _totalCount);
			}
		}

		private void AppendContexts(bool isIncomplete)
		{
			EnumerableQuantifier.QuantifierContexts quantifierContexts = _quantifier.GetQuantifierContext();
			if (quantifierContexts.HasFlag(EnumerableQuantifier.QuantifierContexts.MatchingItems) &&
			    _matchingItems?.Count > 0)
			{
				_expectationBuilder.AddContext(new ResultContext.SyncCallback("Matching items",
						() => Formatter.Format(_matchingItems, typeof(TItem).GetFormattingOption(_matchingItems?.Count))
							.AppendIsIncomplete(isIncomplete),
						int.MaxValue));
			}

			if (quantifierContexts.HasFlag(EnumerableQuantifier.QuantifierContexts.NotMatchingItems) &&
			    _notMatchingItems?.Count > 0)
			{
				_expectationBuilder.AddContext(new ResultContext.SyncCallback("Not matching items",
						() => Formatter.Format(_notMatchingItems,
								typeof(TItem).GetFormattingOption(_notMatchingItems?.Count))
							.AppendIsIncomplete(isIncomplete),
						int.MaxValue));
			}
		}
	}
}
#endif
