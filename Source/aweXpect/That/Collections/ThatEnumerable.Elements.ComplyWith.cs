using System;
using System.Collections;
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

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatEnumerable
{
	public partial class Elements<TItem>
	{
		/// <summary>
		///     …comply with the <paramref name="expectations" />.
		/// </summary>
		public ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
			ComplyWith(Action<IThatSubject<TItem>> expectations)
		{
			ObjectEqualityOptions<TItem> options = new();
			return new ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
				_subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
					=> new ComplyWithConstraint(expectationBuilder, it, grammars, _quantifier, expectations)),
				_subject,
				options);
		}

		private sealed class ComplyWithConstraint
			: ConstraintResult.WithNotNullValue<IEnumerable<TItem>?>,
				IAsyncContextConstraint<IEnumerable<TItem>?>
		{
			private readonly ExpectationBuilder _expectationBuilder;
			private readonly ManualExpectationBuilder<TItem> _itemExpectationBuilder;
			private readonly EnumerableQuantifier _quantifier;
			private int _matchingCount;
			private LimitedCollection<TItem>? _matchingItems;
			private int _notMatchingCount;
			private LimitedCollection<TItem>? _notMatchingItems;
			private int? _totalCount;

			public ComplyWithConstraint(ExpectationBuilder expectationBuilder, string it, ExpectationGrammars grammars,
				EnumerableQuantifier quantifier,
				Action<IThatSubject<TItem>> expectations)
				: base(it, grammars)
			{
				_expectationBuilder = expectationBuilder;
				_quantifier = quantifier;
				_itemExpectationBuilder = new ManualExpectationBuilder<TItem>(null, grammars & ~ExpectationGrammars.Plural);
				expectations.Invoke(new ThatSubject<TItem>(_itemExpectationBuilder));
			}

			public async Task<ConstraintResult> IsMetBy(
				IEnumerable<TItem>? actual,
				IEvaluationContext context,
				CancellationToken cancellationToken)
			{
				Actual = actual;
				if (actual is null)
				{
					Outcome = Outcome.Failure;
					return this;
				}

				IEnumerable<TItem> materialized = context.UseMaterializedEnumerable<TItem, IEnumerable<TItem>>(actual);
				bool cancelEarly = actual is not ICollection<TItem>;
				_matchingCount = 0;
				_notMatchingCount = 0;
				int maxItems = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get() + 1;
				_matchingItems = new LimitedCollection<TItem>(maxItems);
				_notMatchingItems = new LimitedCollection<TItem>(maxItems);

				foreach (TItem item in materialized)
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

					if (cancelEarly && _quantifier.IsDeterminable(_matchingCount, _notMatchingCount))
					{
						Outcome = _quantifier.GetOutcome(_matchingCount, _notMatchingCount, _totalCount);
						AppendContexts(true);
						_expectationBuilder.AddCollectionContext(materialized,
							Outcome == Outcome.Failure && materialized.ExceedsFormatterLimit());
						return this;
					}

					if (cancellationToken.IsCancellationRequested)
					{
						Outcome = Outcome.Undecided;
						_expectationBuilder.AddCollectionContext(materialized, true);
						return this;
					}
				}

				_totalCount = _matchingCount + _notMatchingCount;
				Outcome = _quantifier.GetOutcome(_matchingCount, _notMatchingCount, _totalCount);
				AppendContexts(false);
				_expectationBuilder.AddCollectionContext(materialized);
				return this;
			}

			protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			{
				_itemExpectationBuilder.AppendExpectation(stringBuilder, indentation);
				stringBuilder.Append(For);
				stringBuilder.Append(_quantifier);
				stringBuilder.Append(' ');
				stringBuilder.Append(_quantifier.GetItemString());
			}

			protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
				=> _quantifier.AppendResult(stringBuilder, Grammars, _matchingCount, _notMatchingCount, _totalCount);

			protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			{
				stringBuilder.Append(_quantifier);
				stringBuilder.Append(For);
				_itemExpectationBuilder.AppendExpectation(stringBuilder, indentation);
				stringBuilder.Append(' ');
				stringBuilder.Append(_quantifier.GetItemString());
			}

			protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
				=> _quantifier.AppendResult(stringBuilder, Grammars, _matchingCount, _notMatchingCount,
					_totalCount);

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

	public partial class Elements
	{
		/// <summary>
		///     …comply with the <paramref name="expectations" />.
		/// </summary>
		public ObjectEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>, string?>
			ComplyWith(Action<IThatSubject<string?>> expectations)
		{
			ObjectEqualityOptions<string?> options = new();
			return new ObjectEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>, string?>(
				_subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
					=> new ComplyWithConstraint(expectationBuilder, it, grammars, _quantifier, expectations)),
				_subject,
				options);
		}

		private sealed class ComplyWithConstraint
			: ConstraintResult.WithNotNullValue<IEnumerable<string?>?>,
				IAsyncContextConstraint<IEnumerable<string?>?>
		{
			private readonly ExpectationBuilder _expectationBuilder;
			private readonly ManualExpectationBuilder<string?> _itemExpectationBuilder;
			private readonly EnumerableQuantifier _quantifier;
			private int _matchingCount;
			private LimitedCollection<string?>? _matchingItems;
			private int _notMatchingCount;
			private LimitedCollection<string?>? _notMatchingItems;
			private int? _totalCount;

			public ComplyWithConstraint(ExpectationBuilder expectationBuilder, string it, ExpectationGrammars grammars,
				EnumerableQuantifier quantifier,
				Action<IThatSubject<string?>> expectations)
				: base(it, grammars)
			{
				_expectationBuilder = expectationBuilder;
				_quantifier = quantifier;
				_itemExpectationBuilder = new ManualExpectationBuilder<string?>(null, grammars & ~ExpectationGrammars.Plural);
				expectations.Invoke(new ThatSubject<string?>(_itemExpectationBuilder));
			}

			public async Task<ConstraintResult> IsMetBy(
				IEnumerable<string?>? actual,
				IEvaluationContext context,
				CancellationToken cancellationToken)
			{
				Actual = actual;
				if (actual is null)
				{
					Outcome = Outcome.Failure;
					return this;
				}

				IEnumerable<string?> materialized = context.UseMaterializedEnumerable<string?, IEnumerable<string?>>(actual);
				bool cancelEarly = actual is not ICollection<string?>;
				_matchingCount = 0;
				_notMatchingCount = 0;
				int maxItems = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get() + 1;
				_matchingItems = new LimitedCollection<string?>(maxItems);
				_notMatchingItems = new LimitedCollection<string?>(maxItems);

				foreach (string? item in materialized)
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

					if (cancelEarly && _quantifier.IsDeterminable(_matchingCount, _notMatchingCount))
					{
						Outcome = _quantifier.GetOutcome(_matchingCount, _notMatchingCount, _totalCount);
						AppendContexts(true);
						_expectationBuilder.AddCollectionContext(materialized,
							Outcome == Outcome.Failure && materialized.ExceedsFormatterLimit());
						return this;
					}

					if (cancellationToken.IsCancellationRequested)
					{
						Outcome = Outcome.Undecided;
						_expectationBuilder.AddCollectionContext(materialized, true);
						return this;
					}
				}

				_totalCount = _matchingCount + _notMatchingCount;
				Outcome = _quantifier.GetOutcome(_matchingCount, _notMatchingCount, _totalCount);
				AppendContexts(false);
				_expectationBuilder.AddCollectionContext(materialized);
				return this;
			}

			protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			{
				_itemExpectationBuilder.AppendExpectation(stringBuilder, indentation);
				stringBuilder.Append(For);
				stringBuilder.Append(_quantifier);
				stringBuilder.Append(' ');
				stringBuilder.Append(_quantifier.GetItemString());
			}

			protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
				=> _quantifier.AppendResult(stringBuilder, Grammars, _matchingCount, _notMatchingCount, _totalCount);

			protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			{
				stringBuilder.Append(_quantifier);
				stringBuilder.Append(For);
				_itemExpectationBuilder.AppendExpectation(stringBuilder, indentation);
				stringBuilder.Append(' ');
				stringBuilder.Append(_quantifier.GetItemString());
			}

			protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
				=> _quantifier.AppendResult(stringBuilder, Grammars, _matchingCount, _notMatchingCount,
					_totalCount);

			private void AppendContexts(bool isIncomplete)
			{
				EnumerableQuantifier.QuantifierContexts quantifierContexts = _quantifier.GetQuantifierContext();
				if (quantifierContexts.HasFlag(EnumerableQuantifier.QuantifierContexts.MatchingItems) &&
				    _matchingItems?.Count > 0)
				{
					_expectationBuilder.AddContext(new ResultContext.SyncCallback("Matching items",
							() => Formatter.Format(_matchingItems,
									typeof(string).GetFormattingOption(_matchingItems?.Count))
								.AppendIsIncomplete(isIncomplete),
							int.MaxValue));
				}

				if (quantifierContexts.HasFlag(EnumerableQuantifier.QuantifierContexts.NotMatchingItems) &&
				    _notMatchingItems?.Count > 0)
				{
					_expectationBuilder.AddContext(new ResultContext.SyncCallback("Not matching items",
							() => Formatter.Format(_notMatchingItems,
									typeof(string).GetFormattingOption(_notMatchingItems?.Count))
								.AppendIsIncomplete(isIncomplete),
							int.MaxValue));
				}
			}
		}
	}

	public partial class ElementsForEnumerable<TEnumerable>
	{
		/// <summary>
		///     …comply with the <paramref name="expectations" />.
		/// </summary>
		public ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, object?>
			ComplyWith(Action<IThatSubject<object?>> expectations)
		{
			ObjectEqualityOptions<object?> options = new();
			return new ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, object?>(
				_subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
					=> new ComplyWithConstraint(expectationBuilder, it, grammars, _quantifier, expectations)),
				_subject,
				options);
		}

		private sealed class ComplyWithConstraint
			: ConstraintResult.WithNotNullValue<TEnumerable?>,
				IAsyncContextConstraint<TEnumerable?>
		{
			private readonly ExpectationBuilder _expectationBuilder;
			private readonly ManualExpectationBuilder<object?> _itemExpectationBuilder;
			private readonly EnumerableQuantifier _quantifier;
			private Type? _itemType;
			private int _matchingCount;
			private LimitedCollection<object?>? _matchingItems;
			private int _notMatchingCount;
			private LimitedCollection<object?>? _notMatchingItems;
			private int? _totalCount;

			public ComplyWithConstraint(ExpectationBuilder expectationBuilder, string it, ExpectationGrammars grammars,
				EnumerableQuantifier quantifier,
				Action<IThatSubject<object?>> expectations)
				: base(it, grammars)
			{
				_expectationBuilder = expectationBuilder;
				_quantifier = quantifier;
				_itemExpectationBuilder = new ManualExpectationBuilder<object?>(null, grammars & ~ExpectationGrammars.Plural);
				expectations.Invoke(new ThatSubject<object?>(_itemExpectationBuilder));
			}

			public async Task<ConstraintResult> IsMetBy(
				TEnumerable? actual,
				IEvaluationContext context,
				CancellationToken cancellationToken)
			{
				Actual = actual;
				if (actual is null)
				{
					Outcome = Outcome.Failure;
					return this;
				}

				IEnumerable materialized = context.UseMaterializedEnumerable(actual);
				bool cancelEarly = actual is not ICollection<object?>;
				_matchingCount = 0;
				_notMatchingCount = 0;
				int maxItems = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get() + 1;
				_matchingItems = new LimitedCollection<object?>(maxItems);
				_notMatchingItems = new LimitedCollection<object?>(maxItems);

				foreach (object? item in materialized)
				{
					if (_itemType is null && item is not null)
					{
						_itemType = item.GetType();
					}

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

					if (cancelEarly && _quantifier.IsDeterminable(_matchingCount, _notMatchingCount))
					{
						Outcome = _quantifier.GetOutcome(_matchingCount, _notMatchingCount, _totalCount);
						AppendContexts(true);
						_expectationBuilder.AddCollectionContext(materialized,
							Outcome == Outcome.Failure && materialized.ExceedsFormatterLimit());
						return this;
					}

					if (cancellationToken.IsCancellationRequested)
					{
						Outcome = Outcome.Undecided;
						_expectationBuilder.AddCollectionContext(materialized, true);
						return this;
					}
				}

				_totalCount = _matchingCount + _notMatchingCount;
				Outcome = _quantifier.GetOutcome(_matchingCount, _notMatchingCount, _totalCount);
				AppendContexts(false);
				_expectationBuilder.AddCollectionContext(materialized);
				return this;
			}

			protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			{
				_itemExpectationBuilder.AppendExpectation(stringBuilder, indentation);
				stringBuilder.Append(For);
				stringBuilder.Append(_quantifier);
				stringBuilder.Append(' ');
				stringBuilder.Append(_quantifier.GetItemString());
			}

			protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
				=> _quantifier.AppendResult(stringBuilder, Grammars, _matchingCount, _notMatchingCount, _totalCount);

			protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			{
				stringBuilder.Append(_quantifier);
				stringBuilder.Append(For);
				_itemExpectationBuilder.AppendExpectation(stringBuilder, indentation);
				stringBuilder.Append(' ');
				stringBuilder.Append(_quantifier.GetItemString());
			}

			protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
				=> _quantifier.AppendResult(stringBuilder, Grammars, _matchingCount, _notMatchingCount,
					_totalCount);

			private void AppendContexts(bool isIncomplete)
			{
				EnumerableQuantifier.QuantifierContexts quantifierContexts = _quantifier.GetQuantifierContext();
				if (quantifierContexts.HasFlag(EnumerableQuantifier.QuantifierContexts.MatchingItems) &&
				    _matchingItems?.Count > 0)
				{
					_expectationBuilder.AddContext(new ResultContext.SyncCallback("Matching items",
							() => Formatter.Format(_matchingItems,
									(_itemType ?? typeof(object)).GetFormattingOption(_matchingItems?.Count))
								.AppendIsIncomplete(isIncomplete),
							int.MaxValue));
				}

				if (quantifierContexts.HasFlag(EnumerableQuantifier.QuantifierContexts.NotMatchingItems) &&
				    _notMatchingItems?.Count > 0)
				{
					_expectationBuilder.AddContext(new ResultContext.SyncCallback("Not matching items",
							() => Formatter.Format(_notMatchingItems,
									(_itemType ?? typeof(object)).GetFormattingOption(_notMatchingItems?.Count))
								.AppendIsIncomplete(isIncomplete),
							int.MaxValue));
				}
			}
		}
	}

	public partial class ElementsForStructEnumerable<TEnumerable, TItem>
	{
		/// <summary>
		///     …comply with the <paramref name="expectations" />.
		/// </summary>
		public ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, TItem>
			ComplyWith(Action<IThatSubject<TItem>> expectations)
		{
			ObjectEqualityOptions<TItem> options = new();
			return new ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, TItem>(
				_subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
					=> new ComplyWithConstraint(expectationBuilder, it, grammars, _quantifier, expectations)),
				_subject,
				options);
		}

		private sealed class ComplyWithConstraint
			: ConstraintResult.WithNotNullValue<TEnumerable>,
				IAsyncContextConstraint<TEnumerable>
		{
			private readonly ExpectationBuilder _expectationBuilder;
			private readonly ManualExpectationBuilder<TItem> _itemExpectationBuilder;
			private readonly EnumerableQuantifier _quantifier;
			private int _matchingCount;
			private LimitedCollection<TItem>? _matchingItems;
			private int _notMatchingCount;
			private LimitedCollection<TItem>? _notMatchingItems;
			private int? _totalCount;

			public ComplyWithConstraint(ExpectationBuilder expectationBuilder, string it, ExpectationGrammars grammars,
				EnumerableQuantifier quantifier,
				Action<IThatSubject<TItem>> expectations)
				: base(it, grammars)
			{
				_expectationBuilder = expectationBuilder;
				_quantifier = quantifier;
				_itemExpectationBuilder = new ManualExpectationBuilder<TItem>(null, grammars & ~ExpectationGrammars.Plural);
				expectations.Invoke(new ThatSubject<TItem>(_itemExpectationBuilder));
			}

			public async Task<ConstraintResult> IsMetBy(
				TEnumerable actual,
				IEvaluationContext context,
				CancellationToken cancellationToken)
			{
				Actual = actual;

				IEnumerable<TItem> materialized = context.UseMaterializedEnumerable<TItem, IEnumerable<TItem>>(actual);
				bool cancelEarly = actual is not ICollection<TItem>;
				_matchingCount = 0;
				_notMatchingCount = 0;
				int maxItems = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get() + 1;
				_matchingItems = new LimitedCollection<TItem>(maxItems);
				_notMatchingItems = new LimitedCollection<TItem>(maxItems);

				foreach (TItem item in materialized)
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

					if (cancelEarly && _quantifier.IsDeterminable(_matchingCount, _notMatchingCount))
					{
						Outcome = _quantifier.GetOutcome(_matchingCount, _notMatchingCount, _totalCount);
						AppendContexts(true);
						_expectationBuilder.AddCollectionContext(materialized,
							Outcome == Outcome.Failure && materialized.ExceedsFormatterLimit());
						return this;
					}

					if (cancellationToken.IsCancellationRequested)
					{
						Outcome = Outcome.Undecided;
						_expectationBuilder.AddCollectionContext(materialized, true);
						return this;
					}
				}

				_totalCount = _matchingCount + _notMatchingCount;
				Outcome = _quantifier.GetOutcome(_matchingCount, _notMatchingCount, _totalCount);
				AppendContexts(false);
				_expectationBuilder.AddCollectionContext(materialized);
				return this;
			}

			protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			{
				_itemExpectationBuilder.AppendExpectation(stringBuilder, indentation);
				stringBuilder.Append(For);
				stringBuilder.Append(_quantifier);
				stringBuilder.Append(' ');
				stringBuilder.Append(_quantifier.GetItemString());
			}

			protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
				=> _quantifier.AppendResult(stringBuilder, Grammars, _matchingCount, _notMatchingCount, _totalCount);

			protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			{
				stringBuilder.Append(_quantifier);
				stringBuilder.Append(For);
				_itemExpectationBuilder.AppendExpectation(stringBuilder, indentation);
				stringBuilder.Append(' ');
				stringBuilder.Append(_quantifier.GetItemString());
			}

			protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
				=> _quantifier.AppendResult(stringBuilder, Grammars, _matchingCount, _notMatchingCount,
					_totalCount);

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
}
