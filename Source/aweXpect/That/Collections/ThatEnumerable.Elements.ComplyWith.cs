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
		public AndOrResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
			ComplyWith(Action<IThatSubject<TItem>> expectations)
		{
			expectations.ThrowIfNull();
			return new(
				_subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
					=> new ComplyWithConstraint(expectationBuilder, it, grammars, _quantifier, expectations)),
				_subject);
		}

		private sealed class ComplyWithConstraint
			: ConstraintResult.WithNotNullValue<IEnumerable<TItem>?>,
				IAsyncContextConstraint<IEnumerable<TItem>?>,
				IExpectationTextConstraint
		{
			private readonly ExpectationBuilder _expectationBuilder;
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
				Action<IThatSubject<TItem>> expectations)
				: base(it, grammars)
			{
				_expectationBuilder = expectationBuilder;
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
				IEnumerable<TItem>? actual,
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

				IEnumerable<TItem> materialized = context.UseMaterializedEnumerable<TItem>(actual);
				bool cancelEarly = actual is not ICollection<TItem>;
				_matchingCount = 0;
				_notMatchingCount = 0;
				_matchingItems = new LimitedCollection<TItem>();
				_notMatchingItems = new LimitedCollection<TItem>();

				foreach (TItem item in materialized)
				{
					ConstraintResult isMatch = await _itemExpectations.Builder.IsMetBy(item, context, cancellationToken);
					// A canceled item expectation decides nothing, so the item must not count as not matching.
					if (isMatch.Outcome == Outcome.Undecided && cancellationToken.IsCancellationRequested)
					{
						Outcome = Outcome.Undecided;
						_expectationBuilder.AddCollectionContext(materialized, true);
						return this;
					}

					if (isMatch.FailsBothWays())
					{
						_unansweredItem = isMatch;
						_unansweredItemIndex = _matchingCount + _notMatchingCount;
						_expectationBuilder.AddCollectionContext(materialized);
						return this;
					}

					if (isMatch.Outcome == Outcome.Success)
					{
						_matchingItems.Add(item, _matchingCount + _notMatchingCount);
						_matchingCount++;
					}
					else
					{
						_notMatchingItems.Add(item, _matchingCount + _notMatchingCount);
						_notMatchingCount++;
					}

					if (cancelEarly && _quantifier.IsDeterminable(_matchingCount, _notMatchingCount))
					{
						Outcome = _quantifier.GetOutcome(_matchingCount, _notMatchingCount, _totalCount);
						AppendContexts(true);
						_expectationBuilder.AddCollectionContext(materialized);
						return this;
					}

					if (cancellationToken.IsCanceledBeforeTheEndOf(materialized))
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

			public async Task<ConstraintResult> GetExpectationResult(IEvaluationContext context,
				CancellationToken cancellationToken)
			{
				await _itemExpectations.PrepareExpectation(context, cancellationToken);
				return this;
			}

			protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
				=> _itemExpectations.AppendExpectation(stringBuilder, false, indentation);

			protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
				=> AppendItemsResult(stringBuilder, indentation);

			protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
				=> _itemExpectations.AppendExpectation(stringBuilder, true, indentation);

			protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
				=> AppendItemsResult(stringBuilder, indentation);

			private void AppendItemsResult(StringBuilder stringBuilder, string? indentation)
			{
				if (_unansweredItem is not null)
				{
					stringBuilder.AppendUnansweredItem(_unansweredItem, _unansweredItemIndex, indentation);
					return;
				}

				_quantifier.AppendResult(stringBuilder, Grammars, It, _matchingCount, _notMatchingCount, _totalCount,
					_itemExpectations.Builder.GetResultVerb());
			}

			private void AppendContexts(bool isIncomplete)
			{
				_expectationBuilder.AddQuantifierContexts(this, _quantifier,
					_matchingItems is { Count: > 0 } matchingItems
						? () => matchingItems.Format(Actual, typeof(TItem), _matchingCount)
							.AppendIsIncomplete(isIncomplete)
						: null,
					_notMatchingItems is { Count: > 0 } notMatchingItems
						? () => notMatchingItems.Format(Actual, typeof(TItem), _notMatchingCount)
							.AppendIsIncomplete(isIncomplete)
						: null);
			}
		}
	}

	public partial class Elements
	{
		/// <summary>
		///     …comply with the <paramref name="expectations" />.
		/// </summary>
		public AndOrResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>
			ComplyWith(Action<IThatSubject<string?>> expectations)
		{
			expectations.ThrowIfNull();
			return new(
				_subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
					=> new ComplyWithConstraint(expectationBuilder, it, grammars, _quantifier, expectations)),
				_subject);
		}

		private sealed class ComplyWithConstraint
			: ConstraintResult.WithNotNullValue<IEnumerable<string?>?>,
				IAsyncContextConstraint<IEnumerable<string?>?>,
				IExpectationTextConstraint
		{
			private readonly ExpectationBuilder _expectationBuilder;
			private readonly ComplyWithItemExpectations<string?> _itemExpectations;
			private readonly EnumerableQuantifier _quantifier;
			private int _matchingCount;
			private LimitedCollection<string?>? _matchingItems;
			private int _notMatchingCount;
			private LimitedCollection<string?>? _notMatchingItems;
			private int? _totalCount;
			private ConstraintResult? _unansweredItem;
			private int _unansweredItemIndex;

			public ComplyWithConstraint(ExpectationBuilder expectationBuilder, string it, ExpectationGrammars grammars,
				EnumerableQuantifier quantifier,
				Action<IThatSubject<string?>> expectations)
				: base(it, grammars)
			{
				_expectationBuilder = expectationBuilder;
				_quantifier = quantifier;
				_itemExpectations = new ComplyWithItemExpectations<string?>(quantifier, grammars, expectations);
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
				IEnumerable<string?>? actual,
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

				IEnumerable<string?> materialized = context.UseMaterializedEnumerable<string?>(actual);
				bool cancelEarly = actual is not ICollection<string?>;
				_matchingCount = 0;
				_notMatchingCount = 0;
				_matchingItems = new LimitedCollection<string?>();
				_notMatchingItems = new LimitedCollection<string?>();

				foreach (string? item in materialized)
				{
					ConstraintResult isMatch = await _itemExpectations.Builder.IsMetBy(item, context, cancellationToken);
					// A canceled item expectation decides nothing, so the item must not count as not matching.
					if (isMatch.Outcome == Outcome.Undecided && cancellationToken.IsCancellationRequested)
					{
						Outcome = Outcome.Undecided;
						_expectationBuilder.AddCollectionContext(materialized, true);
						return this;
					}

					if (isMatch.FailsBothWays())
					{
						_unansweredItem = isMatch;
						_unansweredItemIndex = _matchingCount + _notMatchingCount;
						_expectationBuilder.AddCollectionContext(materialized);
						return this;
					}

					if (isMatch.Outcome == Outcome.Success)
					{
						_matchingItems.Add(item, _matchingCount + _notMatchingCount);
						_matchingCount++;
					}
					else
					{
						_notMatchingItems.Add(item, _matchingCount + _notMatchingCount);
						_notMatchingCount++;
					}

					if (cancelEarly && _quantifier.IsDeterminable(_matchingCount, _notMatchingCount))
					{
						Outcome = _quantifier.GetOutcome(_matchingCount, _notMatchingCount, _totalCount);
						AppendContexts(true);
						_expectationBuilder.AddCollectionContext(materialized);
						return this;
					}

					if (cancellationToken.IsCanceledBeforeTheEndOf(materialized))
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

			public async Task<ConstraintResult> GetExpectationResult(IEvaluationContext context,
				CancellationToken cancellationToken)
			{
				await _itemExpectations.PrepareExpectation(context, cancellationToken);
				return this;
			}

			protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
				=> _itemExpectations.AppendExpectation(stringBuilder, false, indentation);

			protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
				=> AppendItemsResult(stringBuilder, indentation);

			protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
				=> _itemExpectations.AppendExpectation(stringBuilder, true, indentation);

			protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
				=> AppendItemsResult(stringBuilder, indentation);

			private void AppendItemsResult(StringBuilder stringBuilder, string? indentation)
			{
				if (_unansweredItem is not null)
				{
					stringBuilder.AppendUnansweredItem(_unansweredItem, _unansweredItemIndex, indentation);
					return;
				}

				_quantifier.AppendResult(stringBuilder, Grammars, It, _matchingCount, _notMatchingCount, _totalCount,
					_itemExpectations.Builder.GetResultVerb());
			}

			private void AppendContexts(bool isIncomplete)
			{
				_expectationBuilder.AddQuantifierContexts(this, _quantifier,
					_matchingItems is { Count: > 0 } matchingItems
						? () => matchingItems.Format(Actual, typeof(string), _matchingCount)
							.AppendIsIncomplete(isIncomplete)
						: null,
					_notMatchingItems is { Count: > 0 } notMatchingItems
						? () => notMatchingItems.Format(Actual, typeof(string), _notMatchingCount)
							.AppendIsIncomplete(isIncomplete)
						: null);
			}
		}
	}

	public partial class ElementsForEnumerable<TEnumerable>
	{
		/// <summary>
		///     …comply with the <paramref name="expectations" />.
		/// </summary>
		public AndOrResult<TEnumerable, IThat<TEnumerable?>>
			ComplyWith(Action<IThatSubject<object?>> expectations)
		{
			expectations.ThrowIfNull();
			return new(
				_subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
					=> new ComplyWithConstraint(expectationBuilder, it, grammars, _quantifier, expectations)),
				_subject);
		}

		private sealed class ComplyWithConstraint
			: ConstraintResult.WithNotNullValue<TEnumerable?>,
				IAsyncContextConstraint<TEnumerable?>,
				IExpectationTextConstraint
		{
			private readonly ExpectationBuilder _expectationBuilder;
			private readonly ComplyWithItemExpectations<object?> _itemExpectations;
			private readonly EnumerableQuantifier _quantifier;
			private Type? _itemType;
			private int _matchingCount;
			private LimitedCollection<object?>? _matchingItems;
			private int _notMatchingCount;
			private LimitedCollection<object?>? _notMatchingItems;
			private int? _totalCount;
			private ConstraintResult? _unansweredItem;
			private int _unansweredItemIndex;

			public ComplyWithConstraint(ExpectationBuilder expectationBuilder, string it, ExpectationGrammars grammars,
				EnumerableQuantifier quantifier,
				Action<IThatSubject<object?>> expectations)
				: base(it, grammars)
			{
				_expectationBuilder = expectationBuilder;
				_quantifier = quantifier;
				_itemExpectations = new ComplyWithItemExpectations<object?>(quantifier, grammars, expectations);
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
				TEnumerable? actual,
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

				IEnumerable materialized = context.UseMaterializedEnumerable(actual);
				bool cancelEarly = actual is not ICollection;
				_matchingCount = 0;
				_notMatchingCount = 0;
				_matchingItems = new LimitedCollection<object?>();
				_notMatchingItems = new LimitedCollection<object?>();

				foreach (object? item in materialized)
				{
					_itemType ??= item?.GetType();
					ConstraintResult isMatch = await _itemExpectations.Builder.IsMetBy(item, context, cancellationToken);
					// A canceled item expectation decides nothing, so the item must not count as not matching.
					if (isMatch.Outcome == Outcome.Undecided && cancellationToken.IsCancellationRequested)
					{
						Outcome = Outcome.Undecided;
						_expectationBuilder.AddCollectionContext(materialized, true);
						return this;
					}

					if (isMatch.FailsBothWays())
					{
						_unansweredItem = isMatch;
						_unansweredItemIndex = _matchingCount + _notMatchingCount;
						_expectationBuilder.AddCollectionContext(materialized);
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

					if (cancelEarly && _quantifier.IsDeterminable(_matchingCount, _notMatchingCount))
					{
						Outcome = _quantifier.GetOutcome(_matchingCount, _notMatchingCount, _totalCount);
						AppendContexts(true);
						_expectationBuilder.AddCollectionContext(materialized);
						return this;
					}

					if (cancellationToken.IsCanceledBeforeTheEndOf(materialized))
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

			public async Task<ConstraintResult> GetExpectationResult(IEvaluationContext context,
				CancellationToken cancellationToken)
			{
				await _itemExpectations.PrepareExpectation(context, cancellationToken);
				return this;
			}

			protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
				=> _itemExpectations.AppendExpectation(stringBuilder, false, indentation);

			protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
				=> AppendItemsResult(stringBuilder, indentation);

			protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
				=> _itemExpectations.AppendExpectation(stringBuilder, true, indentation);

			protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
				=> AppendItemsResult(stringBuilder, indentation);

			private void AppendItemsResult(StringBuilder stringBuilder, string? indentation)
			{
				if (_unansweredItem is not null)
				{
					stringBuilder.AppendUnansweredItem(_unansweredItem, _unansweredItemIndex, indentation);
					return;
				}

				_quantifier.AppendResult(stringBuilder, Grammars, It, _matchingCount, _notMatchingCount, _totalCount,
					_itemExpectations.Builder.GetResultVerb());
			}

			private void AppendContexts(bool isIncomplete)
			{
				_expectationBuilder.AddQuantifierContexts(this, _quantifier,
					_matchingItems?.Count > 0
						? () => Formatter.Format(_matchingItems,
								(_itemType ?? typeof(object)).GetFormattingOption(_matchingItems?.Count, _matchingCount))
							.AppendIsIncomplete(isIncomplete)
						: null,
					_notMatchingItems?.Count > 0
						? () => Formatter.Format(_notMatchingItems,
								(_itemType ?? typeof(object)).GetFormattingOption(_notMatchingItems?.Count, _notMatchingCount))
							.AppendIsIncomplete(isIncomplete)
						: null);
			}
		}
	}

	public partial class ElementsForStructEnumerable<TEnumerable, TItem>
	{
		/// <summary>
		///     …comply with the <paramref name="expectations" />.
		/// </summary>
		public AndOrResult<TEnumerable, IThat<TEnumerable>>
			ComplyWith(Action<IThatSubject<TItem>> expectations)
		{
			expectations.ThrowIfNull();
			return new(
				_subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
					=> new ComplyWithConstraint(expectationBuilder, it, grammars, _quantifier, expectations)),
				_subject);
		}

		private sealed class ComplyWithConstraint
			: ConstraintResult.WithNotNullValue<TEnumerable>,
				IAsyncContextConstraint<TEnumerable>,
				IExpectationTextConstraint
		{
			private readonly ExpectationBuilder _expectationBuilder;
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
				Action<IThatSubject<TItem>> expectations)
				: base(it, grammars)
			{
				_expectationBuilder = expectationBuilder;
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
				TEnumerable actual,
				IEvaluationContext context,
				CancellationToken cancellationToken)
			{
				Actual = actual;
				_unansweredItem = null;
				await _itemExpectations.PrepareExpectation(context, cancellationToken);
				if (actual.IsDefaultImmutableArray())
				{
					return this.AsNullSubject(It);
				}

				IEnumerable<TItem> materialized = context.UseMaterializedEnumerable<TItem>(actual);
				bool cancelEarly = actual is not ICollection<TItem>;
				_matchingCount = 0;
				_notMatchingCount = 0;
				_matchingItems = new LimitedCollection<TItem>();
				_notMatchingItems = new LimitedCollection<TItem>();

				foreach (TItem item in materialized)
				{
					ConstraintResult isMatch = await _itemExpectations.Builder.IsMetBy(item, context, cancellationToken);
					// A canceled item expectation decides nothing, so the item must not count as not matching.
					if (isMatch.Outcome == Outcome.Undecided && cancellationToken.IsCancellationRequested)
					{
						Outcome = Outcome.Undecided;
						_expectationBuilder.AddCollectionContext(materialized, true);
						return this;
					}

					if (isMatch.FailsBothWays())
					{
						_unansweredItem = isMatch;
						_unansweredItemIndex = _matchingCount + _notMatchingCount;
						_expectationBuilder.AddCollectionContext(materialized);
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

					if (cancelEarly && _quantifier.IsDeterminable(_matchingCount, _notMatchingCount))
					{
						Outcome = _quantifier.GetOutcome(_matchingCount, _notMatchingCount, _totalCount);
						AppendContexts(true);
						_expectationBuilder.AddCollectionContext(materialized);
						return this;
					}

					if (cancellationToken.IsCanceledBeforeTheEndOf(materialized))
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

			public async Task<ConstraintResult> GetExpectationResult(IEvaluationContext context,
				CancellationToken cancellationToken)
			{
				await _itemExpectations.PrepareExpectation(context, cancellationToken);
				return this;
			}

			protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
				=> _itemExpectations.AppendExpectation(stringBuilder, false, indentation);

			protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
				=> AppendItemsResult(stringBuilder, indentation);

			protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
				=> _itemExpectations.AppendExpectation(stringBuilder, true, indentation);

			protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
				=> AppendItemsResult(stringBuilder, indentation);

			private void AppendItemsResult(StringBuilder stringBuilder, string? indentation)
			{
				if (_unansweredItem is not null)
				{
					stringBuilder.AppendUnansweredItem(_unansweredItem, _unansweredItemIndex, indentation);
					return;
				}

				_quantifier.AppendResult(stringBuilder, Grammars, It, _matchingCount, _notMatchingCount, _totalCount,
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

	public partial class ElementsForStructEnumerable<TEnumerable>
	{
		/// <summary>
		///     …comply with the <paramref name="expectations" />.
		/// </summary>
		public AndOrResult<TEnumerable, IThat<TEnumerable>>
			ComplyWith(Action<IThatSubject<string?>> expectations)
			=> new ElementsForStructEnumerable<TEnumerable, string?>(_subject, _quantifier).ComplyWith(expectations);
	}
}
