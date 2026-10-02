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

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     Verifies that the collection has an item that complies with the <paramref name="expectations" />…
	/// </summary>
	[GuaranteesNotNull]
	public static HasItemResult<IAsyncEnumerable<TItem>> HasItemThat<TItem>(
		this IThat<IAsyncEnumerable<TItem>?> subject, Action<IThatSubject<TItem>> expectations)
	{
		expectations.ThrowIfNull();
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new HasItemResult<IAsyncEnumerable<TItem>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasItemThatConstraint<TItem>(expectationBuilder, it, grammars, expectations, indexOptions)),
			subject,
			indexOptions);
	}

	/// <summary>
	///     Verifies that the collection does not have an item that complies with the <paramref name="expectations" />…
	/// </summary>
	[GuaranteesNotNull]
	public static HasItemResult<IAsyncEnumerable<TItem>> DoesNotHaveItemThat<TItem>(
		this IThat<IAsyncEnumerable<TItem>?> subject, Action<IThatSubject<TItem>> expectations)
	{
		expectations.ThrowIfNull();
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new HasItemResult<IAsyncEnumerable<TItem>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasItemThatConstraint<TItem>(expectationBuilder, it, grammars, expectations, indexOptions)
					.Invert()),
			subject,
			indexOptions);
	}

	private sealed class HasItemThatConstraint<TItem> : ConstraintResult.WithNotNullValue<IAsyncEnumerable<TItem>?>,
		IAsyncContextConstraint<IAsyncEnumerable<TItem>?>,
		IExpectationTextConstraint
	{
		private readonly ExpectationBuilder _expectationBuilder;
		private readonly string _it;
		private readonly ManualExpectationBuilder<TItem> _itemExpectationBuilder;
		private readonly CollectionIndexOptions _options;
		private TItem? _actual;
		private bool _hasIndex;
		private ConstraintResult? _unansweredItem;
		private int _unansweredItemIndex;

		public HasItemThatConstraint(ExpectationBuilder expectationBuilder,
			string it,
			ExpectationGrammars grammars,
			Action<IThatSubject<TItem>> expectations,
			CollectionIndexOptions options) : base(it, grammars)
		{
			_expectationBuilder = expectationBuilder;
			_it = it;
			_options = options;

			_itemExpectationBuilder = new ManualExpectationBuilder<TItem>(null,
				(Grammars & ~ExpectationGrammars.Plural) | ExpectationGrammars.Introduced);
			expectations.Invoke(new ThatSubject<TItem>(_itemExpectationBuilder));
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

		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<TItem>? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			_unansweredItem = null;
			await _itemExpectationBuilder.PrepareExpectation(context, cancellationToken);
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			IAsyncEnumerable<TItem> materialized =
				context.UseMaterializedAsyncEnumerable<TItem>(actual, cancellationToken);
			_expectationBuilder.AddCollectionContext(materialized as IMaterializedAsyncEnumerable<TItem>);
			_hasIndex = false;
			Outcome = Outcome.Failure;

			int? count = null;
			if (_options.Match is CollectionIndexOptions.IMatchFromEnd)
			{
				count = (await (materialized as IMaterializedAsyncEnumerable<TItem>)!.MaterializeItems(null)).Count;
			}

			int index = -1;
			await foreach (TItem item in materialized.UntilCancelled(cancellationToken))
			{
				index++;
				bool? isIndexInRange = _options.Match switch
				{
					CollectionIndexOptions.IMatchFromBeginning fromBeginning => fromBeginning.MatchesIndex(index),
					CollectionIndexOptions.IMatchFromEnd fromEnd => fromEnd.MatchesIndex(index, count),
					_ => false,
				};
				if (isIndexInRange != true)
				{
					if (isIndexInRange == false)
					{
						return this;
					}

					continue;
				}

				_hasIndex = true;
				_actual = item;
				ConstraintResult isMatch = await _itemExpectationBuilder.IsMetBy(item, context, cancellationToken);
				if (isMatch.Outcome == Outcome.Undecided && cancellationToken.IsCancellationRequested)
				{
					Outcome = Outcome.Undecided;
					return this;
				}

				if (isMatch.FailsBothWays())
				{
					_unansweredItem = isMatch;
					_unansweredItemIndex = index;
					return this;
				}

				Outcome = isMatch.Outcome;
				if (isMatch.Outcome == Outcome.Success)
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

		public async Task<ConstraintResult> GetExpectationResult(IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			await _itemExpectationBuilder.PrepareExpectation(context, cancellationToken);
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("has an item that ", "have an item that "));
			_itemExpectationBuilder.AppendExpectation(stringBuilder, indentation);
			stringBuilder.Append(_options.Match.GetDescription());
			_itemExpectationBuilder.AppendReasons(stringBuilder);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_unansweredItem is not null)
			{
				stringBuilder.AppendUnansweredItem(_unansweredItem, _unansweredItemIndex, indentation);
				return;
			}

			if (_hasIndex)
			{
				if (_options.Match.OnlySingleIndex())
				{
					stringBuilder.Append(_it).Append(" had item ");
					Formatter.Format(stringBuilder, _actual);
					stringBuilder.Append(_options.Match.GetDescription());
				}
				else
				{
					stringBuilder.Append(_it).Append(" had no matching item").Append(_options.Match.GetDescription());
				}
			}
			else
			{
				stringBuilder.Append(_it).Append(" had no item").Append(_options.Match.GetDescription());
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not have an item that ", "do not have an item that "));
			_itemExpectationBuilder.AppendExpectation(stringBuilder, indentation);
			stringBuilder.Append(_options.Match.GetDescription());
			_itemExpectationBuilder.AppendReasons(stringBuilder);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_unansweredItem is not null)
			{
				stringBuilder.AppendUnansweredItem(_unansweredItem, _unansweredItemIndex, indentation);
				return;
			}

			stringBuilder.Append(_it).Append(" had item ");
			Formatter.Format(stringBuilder, _actual);
			stringBuilder.Append(_options.Match.GetDescription());
		}
	}
}
#endif
