using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Customization;
using aweXpect.Helpers;
using aweXpect.Options;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

/// <summary>
///     Expectations on <see cref="IEnumerable{TItem}" />.
/// </summary>
public static partial class ThatEnumerable
{
	private const string For = " for ";
	private const string SortOrder = " order";
	private const string CannotCompareToNull = " cannot compare to <null>";

	private sealed class IsEqualToConstraint<TItem, TMatch>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		string? expectedExpression,
		IEnumerable<TItem>? expected,
		IOptionsEquality<TMatch> options,
		CollectionMatchOptions matchOptions)
		: ConstraintResult.WithEqualToValue<IEnumerable<TItem>?>(it, grammars, expected is null),
			IAsyncContextConstraint<IEnumerable<TItem>?>
		where TItem : TMatch
	{
		private string? _failure;

		public async Task<ConstraintResult> IsMetBy(IEnumerable<TItem>? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null || expected is null)
			{
				Outcome = actual is null && expected is null ? Outcome.Success : Outcome.Failure;
				return this;
			}

			expectationBuilder.AddContext(new ResultContext.SyncCallback("Expected",
					() => Formatter.Format(expected, typeof(TItem).GetFormattingOption(expected switch
					{
						ICollection<TItem> coll => coll.Count,
						ICountable countable => countable.Count,
						_ => null,
					})),
					-2));
			IEnumerable<TItem> materializedEnumerable =
				context.UseMaterializedEnumerable<TItem, IEnumerable<TItem>>(actual);
			ICollectionMatcher<TItem, TMatch> matcher = matchOptions.GetCollectionMatcher<TItem, TMatch>(expected);
			int maximumNumber = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();

			foreach (TItem item in materializedEnumerable)
			{
				var (result, failure) = await matcher.Verify(It, item, options, maximumNumber);
				if (result)
				{
					_failure = failure ?? TooManyDeviationsError();
					Outcome = Outcome.Failure;
					expectationBuilder.AddCollectionContext(materializedEnumerable,
						materializedEnumerable.ExceedsFormatterLimit());
					return this;
				}
			}

			var (completedResult, completedFailure) = await matcher.VerifyComplete(It, options, maximumNumber);
			if (completedResult)
			{
				_failure = completedFailure ?? TooManyDeviationsError();
				Outcome = Outcome.Failure;
				expectationBuilder.AddCollectionContext(materializedEnumerable);
				return this;
			}

			expectationBuilder.AddCollectionContext(materializedEnumerable);
			Outcome = Outcome.Success;
			return this;
		}

		private string TooManyDeviationsError()
			=> $"{It} had more than {2 * Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get()} deviations";

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(matchOptions.GetExpectation(
				expectedExpression ?? Formatter.Format(expected, FormattingOptions.SingleLine), Grammars));
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (expected is null)
			{
				stringBuilder.Append(It).Append(CannotCompareToNull);
			}
			else if (_failure is not null)
			{
				stringBuilder.Append(_failure);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalExpectation(stringBuilder, indentation);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (expected is null)
			{
				stringBuilder.Append(It).Append(CannotCompareToNull);
			}
			else
			{
				stringBuilder.Append(It).Append(" did");
			}
		}
	}

	private sealed class IsEqualToFromExpectationsConstraint<TItem, TMatch>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		string? expectedExpression,
		IEnumerable<Action<IThatSubject<TItem?>>>? expected,
		CollectionMatchOptions matchOptions)
		: ConstraintResult.WithEqualToValue<IEnumerable<TItem>?>(it, grammars, expected is null),
			IAsyncContextConstraint<IEnumerable<TItem>?>
		where TItem : TMatch
	{
		private CollectionMatchOptions.ExpectationItem<TItem>[] _expectations = [];

		private string? _failure;

		public async Task<ConstraintResult> IsMetBy(IEnumerable<TItem>? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null || expected is null)
			{
				Outcome = actual is null && expected is null ? Outcome.Success : Outcome.Failure;
				return this;
			}

			IEnumerable<TItem> materializedEnumerable =
				context.UseMaterializedEnumerable<TItem, IEnumerable<TItem>>(actual);
			_expectations = expected.Select(expectation
					=> new CollectionMatchOptions.ExpectationItem<TItem>(expectation,
						Grammars & ~ExpectationGrammars.Negated,
						context,
						CancellationToken.None))
				.ToArray();
			expectationBuilder.AddContext(new ResultContext.SyncCallback("Expected",
					() => Formatter.Format(_expectations, typeof(TItem).GetFormattingOption(_expectations.Length)),
					-2));
			ICollectionMatcher<TItem, TMatch> matcher = matchOptions.GetCollectionMatcher<TItem, TMatch>(_expectations);
			int maximumNumber = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();

			NoOptions noOptions = new();
			foreach (TItem item in materializedEnumerable)
			{
				var (result, failure) = await matcher.Verify(It, item, noOptions, maximumNumber);
				if (result)
				{
					_failure = failure ?? TooManyDeviationsError();
					Outcome = Outcome.Failure;
					expectationBuilder.AddCollectionContext(materializedEnumerable,
						materializedEnumerable.ExceedsFormatterLimit());
					return this;
				}
			}

			var (completedResult, completedFailure) = await matcher.VerifyComplete(It, noOptions, maximumNumber);
			if (completedResult)
			{
				_failure = completedFailure ?? TooManyDeviationsError();
				Outcome = Outcome.Failure;
				expectationBuilder.AddCollectionContext(materializedEnumerable);
				return this;
			}

			expectationBuilder.AddCollectionContext(materializedEnumerable);
			Outcome = Outcome.Success;
			return this;
		}

		private string TooManyDeviationsError()
			=> $"{It} had more than {2 * Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get()} deviations";

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(matchOptions.GetExpectation(
				expectedExpression ?? Formatter.Format(_expectations, FormattingOptions.SingleLine), Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (expected is null)
			{
				stringBuilder.Append(It).Append(CannotCompareToNull);
			}
			else if (_failure is not null)
			{
				stringBuilder.Append(_failure);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalExpectation(stringBuilder, indentation);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (expected is null)
			{
				stringBuilder.Append(It).Append(CannotCompareToNull);
			}
			else
			{
				stringBuilder.Append(It).Append(" did");
			}
		}

		private sealed class NoOptions : IOptionsEquality<TMatch>
		{
#if NET8_0_OR_GREATER
			public ValueTask<bool> AreConsideredEqual<TExpected>(TMatch actual, TExpected expected)
				=> ValueTask.FromResult(Equals(actual, expected));
#else
			public Task<bool> AreConsideredEqual<TExpected>(TMatch actual, TExpected expected)
				=> Task.FromResult(Equals(actual, expected));
#endif
		}
	}

	private sealed class IsEqualToFromPredicateConstraint<TItem, TMatch>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		string? expectedExpression,
		IEnumerable<Expression<Func<TItem, bool>>>? expected,
		CollectionMatchOptions matchOptions)
		: ConstraintResult.WithEqualToValue<IEnumerable<TItem>?>(it, grammars, expected is null),
			IAsyncContextConstraint<IEnumerable<TItem>?>
		where TItem : TMatch
	{
		private string? _failure;

		public async Task<ConstraintResult> IsMetBy(IEnumerable<TItem>? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null || expected is null)
			{
				Outcome = actual is null && expected is null ? Outcome.Success : Outcome.Failure;
				return this;
			}

			expectationBuilder.AddContext(new ResultContext.SyncCallback("Expected",
					() => Formatter.Format(expected, typeof(TItem).GetFormattingOption(expected switch
					{
						ICollection<TItem> coll => coll.Count,
						ICountable countable => countable.Count,
						_ => null,
					})),
					-2));
			IEnumerable<TItem> materializedEnumerable =
				context.UseMaterializedEnumerable<TItem, IEnumerable<TItem>>(actual);
			ICollectionMatcher<TItem, TMatch> matcher = matchOptions.GetCollectionMatcher<TItem, TMatch>(expected);
			int maximumNumber = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();

			NoOptions noOptions = new();
			foreach (TItem item in materializedEnumerable)
			{
				var (result, failure) = await matcher.Verify(It, item, noOptions, maximumNumber);
				if (result)
				{
					_failure = failure ?? TooManyDeviationsError();
					Outcome = Outcome.Failure;
					expectationBuilder.AddCollectionContext(materializedEnumerable,
						materializedEnumerable.ExceedsFormatterLimit());
					return this;
				}
			}

			var (completedResult, completedFailure) = await matcher.VerifyComplete(It, noOptions, maximumNumber);
			if (completedResult)
			{
				_failure = completedFailure ?? TooManyDeviationsError();
				Outcome = Outcome.Failure;
				expectationBuilder.AddCollectionContext(materializedEnumerable);
				return this;
			}

			expectationBuilder.AddCollectionContext(materializedEnumerable);
			Outcome = Outcome.Success;
			return this;
		}

		private string TooManyDeviationsError()
			=> $"{It} had more than {2 * Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get()} deviations";

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(matchOptions.GetExpectation(
				expectedExpression ?? Formatter.Format(expected, FormattingOptions.SingleLine), Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (expected is null)
			{
				stringBuilder.Append(It).Append(CannotCompareToNull);
			}
			else if (_failure is not null)
			{
				stringBuilder.Append(_failure);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalExpectation(stringBuilder, indentation);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (expected is null)
			{
				stringBuilder.Append(It).Append(CannotCompareToNull);
			}
			else
			{
				stringBuilder.Append(It).Append(" did");
			}
		}

		private sealed class NoOptions : IOptionsEquality<TMatch>
		{
#if NET8_0_OR_GREATER
			public ValueTask<bool> AreConsideredEqual<TExpected>(TMatch actual, TExpected expected)
				=> ValueTask.FromResult(Equals(actual, expected));
#else
			public Task<bool> AreConsideredEqual<TExpected>(TMatch actual, TExpected expected)
				=> Task.FromResult(Equals(actual, expected));
#endif
		}
	}

	private sealed class IsEqualToForEnumerableConstraint<TEnumerable, TItem, TMatch>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		string? expectedExpression,
		IEnumerable<TItem>? expected,
		IOptionsEquality<TMatch> options,
		CollectionMatchOptions matchOptions)
		: ConstraintResult.WithEqualToValue<TEnumerable?>(it, grammars, expected is null),
			IAsyncContextConstraint<TEnumerable?>
		where TEnumerable : IEnumerable?
		where TItem : TMatch
	{
		private string? _failure;

		public async Task<ConstraintResult> IsMetBy(TEnumerable? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null || expected is null)
			{
				Outcome = actual is null && expected is null ? Outcome.Success : Outcome.Failure;
				return this;
			}

			expectationBuilder.AddContext(new ResultContext.SyncCallback("Expected",
					() => Formatter.Format(expected, typeof(TItem).GetFormattingOption(expected switch
					{
						ICollection<TItem> coll => coll.Count,
						ICountable countable => countable.Count,
						_ => null,
					})),
					-2));
			IEnumerable materializedEnumerable = context.UseMaterializedEnumerable(actual);
			ICollectionMatcher<TItem, TMatch> matcher = matchOptions.GetCollectionMatcher<TItem, TMatch>(expected);
			int maximumNumber = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();

			foreach (TItem item in materializedEnumerable)
			{
				var (result, failure) = await matcher.Verify(It, item, options, maximumNumber);
				if (result)
				{
					_failure = failure ?? TooManyDeviationsError();
					Outcome = Outcome.Failure;
					expectationBuilder.AddCollectionContext(materializedEnumerable,
						materializedEnumerable.ExceedsFormatterLimit());
					return this;
				}
			}

			var (completedResult, completedFailure) = await matcher.VerifyComplete(It, options, maximumNumber);
			if (completedResult)
			{
				_failure = completedFailure ?? TooManyDeviationsError();
				Outcome = Outcome.Failure;
				expectationBuilder.AddCollectionContext(materializedEnumerable);
				return this;
			}

			expectationBuilder.AddCollectionContext(materializedEnumerable);
			Outcome = Outcome.Success;
			return this;
		}

		private string TooManyDeviationsError()
			=> $"{It} had more than {2 * Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get()} deviations";

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(matchOptions.GetExpectation(
				expectedExpression ?? Formatter.Format(expected, FormattingOptions.SingleLine), Grammars));
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (expected is null)
			{
				stringBuilder.Append(It).Append(CannotCompareToNull);
			}
			else if (_failure is not null)
			{
				stringBuilder.Append(_failure);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalExpectation(stringBuilder, indentation);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (expected is null)
			{
				stringBuilder.Append(It).Append(CannotCompareToNull);
			}
			else
			{
				stringBuilder.Append(It).Append(" did");
			}
		}
	}

	private sealed class CollectionConstraint<TItem>
		: ConstraintResult.WithNotNullValue<IEnumerable<TItem>?>,
			IAsyncContextConstraint<IEnumerable<TItem>?>
	{
		private readonly ExpectationBuilder _expectationBuilder;
		private readonly Func<ExpectationGrammars, string> _expectationText;
		private readonly Func<TItem, bool> _predicate;
		private readonly EnumerableQuantifier _quantifier;
		private readonly string _verb;
		private int _matchingCount;
		private LimitedCollection<TItem>? _matchingItems;
		private int _notMatchingCount;
		private LimitedCollection<TItem>? _notMatchingItems;
		private int? _totalCount;

		public CollectionConstraint(
			ExpectationBuilder expectationBuilder,
			string it,
			ExpectationGrammars grammars,
			EnumerableQuantifier quantifier,
			Func<ExpectationGrammars, string> expectationText,
			Func<TItem, bool> predicate,
			string verb) : base(it, grammars)
		{
			_expectationBuilder = expectationBuilder;
			_quantifier = quantifier;
			_expectationText = expectationText;
			_predicate = predicate;
			_verb = verb;
		}

		public Task<ConstraintResult> IsMetBy(
			IEnumerable<TItem>? actual,
			IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return Task.FromResult<ConstraintResult>(this);
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
				if (_predicate(item))
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
					return Task.FromResult<ConstraintResult>(this);
				}

				if (cancellationToken.IsCancellationRequested)
				{
					Outcome = Outcome.Undecided;
					_expectationBuilder.AddCollectionContext(materialized, true);
					return Task.FromResult<ConstraintResult>(this);
				}
			}

			_totalCount = _matchingCount + _notMatchingCount;
			Outcome = _quantifier.GetOutcome(_matchingCount, _notMatchingCount, _totalCount);
			AppendContexts(false);
			_expectationBuilder.AddCollectionContext(materialized);
			return Task.FromResult<ConstraintResult>(this);
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Grammars.HasFlag(ExpectationGrammars.Nested))
			{
				stringBuilder.Append(_quantifier);
				stringBuilder.Append(' ');
				stringBuilder.Append(_expectationText(Grammars));
			}
			else
			{
				stringBuilder.Append(_expectationText(Grammars));
				stringBuilder.Append(For);
				stringBuilder.Append(_quantifier);
				stringBuilder.Append(' ');
				stringBuilder.Append(_quantifier.GetItemString());
			}
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> _quantifier.AppendResult(stringBuilder, Grammars, _matchingCount, _notMatchingCount, _totalCount, _verb);

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Grammars.HasFlag(ExpectationGrammars.Nested))
			{
				stringBuilder.Append("not ");
				stringBuilder.Append(_quantifier);
				stringBuilder.Append(' ');
				stringBuilder.Append(_expectationText(Grammars));
			}
			else
			{
				stringBuilder.Append(_expectationText(Grammars));
				stringBuilder.Append(For);
				stringBuilder.Append(_quantifier);
				stringBuilder.Append(' ');
				stringBuilder.Append(_quantifier.GetItemString());
			}
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> _quantifier.AppendResult(stringBuilder, Grammars, _matchingCount, _notMatchingCount,
				_totalCount, _verb);

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
						() => Formatter.Format(_notMatchingItems, typeof(TItem).GetFormattingOption(_notMatchingItems?.Count))
							.AppendIsIncomplete(isIncomplete),
						int.MaxValue));
			}
		}
	}

	private sealed class AsyncCollectionConstraint<TItem>
		: ConstraintResult.WithNotNullValue<IEnumerable<TItem>?>,
			IAsyncContextConstraint<IEnumerable<TItem>?>
	{
		private readonly ExpectationBuilder _expectationBuilder;
		private readonly Func<ExpectationGrammars, string> _expectationText;
#if NET8_0_OR_GREATER
		private readonly Func<TItem, ValueTask<bool>> _predicate;
#else
		private readonly Func<TItem, Task<bool>> _predicate;
#endif
		private readonly EnumerableQuantifier _quantifier;
		private readonly string _verb;
		private int _matchingCount;
		private LimitedCollection<TItem>? _matchingItems;
		private int _notMatchingCount;
		private LimitedCollection<TItem>? _notMatchingItems;
		private int? _totalCount;

		public AsyncCollectionConstraint(
			ExpectationBuilder expectationBuilder,
			string it,
			ExpectationGrammars grammars,
			EnumerableQuantifier quantifier,
			Func<ExpectationGrammars, string> expectationText,
#if NET8_0_OR_GREATER
			Func<TItem, ValueTask<bool>> predicate,
#else
			Func<TItem, Task<bool>> predicate,
#endif
			string verb) : base(it, grammars)
		{
			_expectationBuilder = expectationBuilder;
			_quantifier = quantifier;
			_expectationText = expectationText;
			_predicate = predicate;
			_verb = verb;
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
				if (await _predicate(item))
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
			if (Grammars.HasFlag(ExpectationGrammars.Nested))
			{
				stringBuilder.Append(_quantifier);
				stringBuilder.Append(' ');
				stringBuilder.Append(_expectationText(Grammars));
			}
			else
			{
				stringBuilder.Append(_expectationText(Grammars));
				stringBuilder.Append(For);
				stringBuilder.Append(_quantifier);
				stringBuilder.Append(' ');
				stringBuilder.Append(_quantifier.GetItemString());
			}
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> _quantifier.AppendResult(stringBuilder, Grammars, _matchingCount, _notMatchingCount, _totalCount,
			_verb);

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Grammars.HasFlag(ExpectationGrammars.Nested))
			{
				stringBuilder.Append("not ");
				stringBuilder.Append(_quantifier);
				stringBuilder.Append(' ');
				stringBuilder.Append(_expectationText(Grammars));
			}
			else
			{
				stringBuilder.Append(_expectationText(Grammars));
				stringBuilder.Append(For);
				stringBuilder.Append(_quantifier);
				stringBuilder.Append(' ');
				stringBuilder.Append(_quantifier.GetItemString());
			}
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> _quantifier.AppendResult(stringBuilder, Grammars, _matchingCount, _notMatchingCount,
				_totalCount, _verb);

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
						() => Formatter.Format(_notMatchingItems, typeof(TItem).GetFormattingOption(_notMatchingItems?.Count))
							.AppendIsIncomplete(isIncomplete),
						int.MaxValue));
			}
		}
	}

	private sealed class CollectionForEnumerableConstraint<TEnumerable>
		: ConstraintResult.WithNotNullValue<TEnumerable>,
			IAsyncContextConstraint<TEnumerable>
		where TEnumerable : IEnumerable?
	{
		private readonly ExpectationBuilder _expectationBuilder;
		private readonly Func<ExpectationGrammars, string> _expectationText;
		private readonly Func<object?, bool> _predicate;
		private readonly EnumerableQuantifier _quantifier;
		private readonly string _verb;
		private Type? _itemType;
		private int _matchingCount;
		private LimitedCollection<object?>? _matchingItems;
		private int _notMatchingCount;
		private LimitedCollection<object?>? _notMatchingItems;
		private int? _totalCount;

		public CollectionForEnumerableConstraint(
			ExpectationBuilder expectationBuilder,
			string it,
			ExpectationGrammars grammars,
			EnumerableQuantifier quantifier,
			Func<ExpectationGrammars, string> expectationText,
			Func<object?, bool> predicate,
			string verb) : base(it, grammars)
		{
			_expectationBuilder = expectationBuilder;
			_quantifier = quantifier;
			_expectationText = expectationText;
			_predicate = predicate;
			_verb = verb;
		}

		public Task<ConstraintResult> IsMetBy(
			TEnumerable actual,
			IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return Task.FromResult<ConstraintResult>(this);
			}

			IEnumerable materialized = context.UseMaterializedEnumerable(actual);
			bool cancelEarly = actual is not ICollection;
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

				if (_predicate(item))
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
					return Task.FromResult<ConstraintResult>(this);
				}

				if (cancellationToken.IsCancellationRequested)
				{
					Outcome = Outcome.Undecided;
					_expectationBuilder.AddCollectionContext(materialized, true);
					return Task.FromResult<ConstraintResult>(this);
				}
			}

			_totalCount = _matchingCount + _notMatchingCount;
			Outcome = _quantifier.GetOutcome(_matchingCount, _notMatchingCount, _totalCount);
			AppendContexts(false);
			_expectationBuilder.AddCollectionContext(materialized);
			return Task.FromResult<ConstraintResult>(this);
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Grammars.HasFlag(ExpectationGrammars.Nested))
			{
				stringBuilder.Append(_quantifier);
				stringBuilder.Append(' ');
				stringBuilder.Append(_expectationText(Grammars));
			}
			else
			{
				stringBuilder.Append(_expectationText(Grammars));
				stringBuilder.Append(For);
				stringBuilder.Append(_quantifier);
				stringBuilder.Append(' ');
				stringBuilder.Append(_quantifier.GetItemString());
			}
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> _quantifier.AppendResult(stringBuilder, Grammars, _matchingCount, _notMatchingCount, _totalCount, _verb);

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Grammars.HasFlag(ExpectationGrammars.Nested))
			{
				stringBuilder.Append("not ");
				stringBuilder.Append(_quantifier);
				stringBuilder.Append(' ');
				stringBuilder.Append(_expectationText(Grammars));
			}
			else
			{
				stringBuilder.Append(_expectationText(Grammars.Negate()));
				stringBuilder.Append(For);
				stringBuilder.Append(_quantifier);
				stringBuilder.Append(' ');
				stringBuilder.Append(_quantifier.GetItemString());
			}
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> _quantifier.AppendResult(stringBuilder, Grammars.Negate(), _matchingCount, _notMatchingCount,
				_totalCount, _verb);

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

	private sealed class AsyncCollectionForEnumerableConstraint<TEnumerable>
		: ConstraintResult.WithNotNullValue<TEnumerable>,
			IAsyncContextConstraint<TEnumerable>
		where TEnumerable : IEnumerable?
	{
		private readonly ExpectationBuilder _expectationBuilder;
		private readonly Func<ExpectationGrammars, string> _expectationText;
#if NET8_0_OR_GREATER
		private readonly Func<object?, ValueTask<bool>> _predicate;
#else
		private readonly Func<object?, Task<bool>> _predicate;
#endif
		private readonly EnumerableQuantifier _quantifier;
		private readonly string _verb;
		private Type? _itemType;
		private int _matchingCount;
		private LimitedCollection<object?>? _matchingItems;
		private int _notMatchingCount;
		private LimitedCollection<object?>? _notMatchingItems;
		private int? _totalCount;

		public AsyncCollectionForEnumerableConstraint(
			ExpectationBuilder expectationBuilder,
			string it,
			ExpectationGrammars grammars,
			EnumerableQuantifier quantifier,
			Func<ExpectationGrammars, string> expectationText,
#if NET8_0_OR_GREATER
			Func<object?, ValueTask<bool>> predicate,
#else
			Func<object?, Task<bool>> predicate,
#endif
			string verb) : base(it, grammars)
		{
			_expectationBuilder = expectationBuilder;
			_quantifier = quantifier;
			_expectationText = expectationText;
			_predicate = predicate;
			_verb = verb;
		}

		public async Task<ConstraintResult> IsMetBy(
			TEnumerable actual,
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
			bool cancelEarly = actual is not ICollection;
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

				if (await _predicate(item))
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
			if (Grammars.HasFlag(ExpectationGrammars.Nested))
			{
				stringBuilder.Append(_quantifier);
				stringBuilder.Append(' ');
				stringBuilder.Append(_expectationText(Grammars));
			}
			else
			{
				stringBuilder.Append(_expectationText(Grammars));
				stringBuilder.Append(For);
				stringBuilder.Append(_quantifier);
				stringBuilder.Append(' ');
				stringBuilder.Append(_quantifier.GetItemString());
			}
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> _quantifier.AppendResult(stringBuilder, Grammars, _matchingCount, _notMatchingCount, _totalCount, _verb);

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Grammars.HasFlag(ExpectationGrammars.Nested))
			{
				stringBuilder.Append("not ");
				stringBuilder.Append(_quantifier);
				stringBuilder.Append(' ');
				stringBuilder.Append(_expectationText(Grammars));
			}
			else
			{
				stringBuilder.Append(_expectationText(Grammars.Negate()));
				stringBuilder.Append(For);
				stringBuilder.Append(_quantifier);
				stringBuilder.Append(' ');
				stringBuilder.Append(_quantifier.GetItemString());
			}
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> _quantifier.AppendResult(stringBuilder, Grammars.Negate(), _matchingCount, _notMatchingCount,
				_totalCount, _verb);

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

	private sealed class SyncCollectionCountConstraint<TItem>
		: ConstraintResult.WithNotNullValue<IEnumerable<TItem>?>,
			IAsyncContextConstraint<IEnumerable<TItem>?>
	{
		private readonly ExpectationBuilder _expectationBuilder;
		private readonly EnumerableQuantifier _quantifier;
		private int _matchingCount;
		private int _notMatchingCount;
		private int? _totalCount;

		public SyncCollectionCountConstraint(
			ExpectationBuilder expectationBuilder,
			string it,
			ExpectationGrammars grammars,
			EnumerableQuantifier quantifier)
			: base(it, grammars)
		{
			_expectationBuilder = expectationBuilder;
			_quantifier = quantifier;
		}

		public Task<ConstraintResult> IsMetBy(
			IEnumerable<TItem>? actual,
			IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return Task.FromResult<ConstraintResult>(this);
			}

			_matchingCount = 0;
			_notMatchingCount = 0;

			if (actual is ICollection<TItem> collectionOfT)
			{
				_matchingCount = collectionOfT.Count;
				_totalCount = _matchingCount;
				Outcome = _quantifier.GetOutcome(_matchingCount, _notMatchingCount, _totalCount);
				_expectationBuilder.AddCollectionContext(actual);
				return Task.FromResult<ConstraintResult>(this);
			}

			IEnumerable<TItem> materialized =
				context.UseMaterializedEnumerable<TItem, IEnumerable<TItem>>(actual);

			foreach (TItem _ in materialized)
			{
				_matchingCount++;

				if (_quantifier.IsDeterminable(_matchingCount, _notMatchingCount))
				{
					Outcome = _quantifier.GetOutcome(_matchingCount, _notMatchingCount, _totalCount);
					_expectationBuilder.AddCollectionContext(materialized,
						Outcome == Outcome.Failure && materialized.ExceedsFormatterLimit());
					return Task.FromResult<ConstraintResult>(this);
				}

				if (cancellationToken.IsCancellationRequested)
				{
					Outcome = Outcome.Undecided;
					_expectationBuilder.AddCollectionContext(materialized, true);
					return Task.FromResult<ConstraintResult>(this);
				}
			}

			_totalCount = _matchingCount + _notMatchingCount;
			_expectationBuilder.AddCollectionContext(materialized);
			Outcome = _quantifier.GetOutcome(_matchingCount, _notMatchingCount, _totalCount);
			return Task.FromResult<ConstraintResult>(this);
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("has ", "have "));
			stringBuilder.Append(_quantifier);
			stringBuilder.Append(' ');
			stringBuilder.Append(_quantifier.GetItemString());
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> _quantifier.AppendResult(stringBuilder, Grammars, _matchingCount, _notMatchingCount, _totalCount);

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not have ", "do not have "));
			stringBuilder.Append(_quantifier);
			stringBuilder.Append(' ');
			stringBuilder.Append(_quantifier.GetItemString());
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> _quantifier.AppendResult(stringBuilder, Grammars, _matchingCount, _notMatchingCount,
				_totalCount);
	}

	private sealed class SyncCollectionCountForEnumerableConstraint<TEnumerable>
		: ConstraintResult.WithNotNullValue<TEnumerable>,
			IAsyncContextConstraint<TEnumerable>
		where TEnumerable : IEnumerable?
	{
		private readonly ExpectationBuilder _expectationBuilder;
		private readonly EnumerableQuantifier _quantifier;
		private int _matchingCount;
		private int _notMatchingCount;
		private int? _totalCount;

		public SyncCollectionCountForEnumerableConstraint(
			ExpectationBuilder expectationBuilder,
			string it,
			ExpectationGrammars grammars,
			EnumerableQuantifier quantifier)
			: base(it, grammars)
		{
			_expectationBuilder = expectationBuilder;
			_quantifier = quantifier;
		}

		public Task<ConstraintResult> IsMetBy(
			TEnumerable? actual,
			IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return Task.FromResult<ConstraintResult>(this);
			}

			_matchingCount = 0;
			_notMatchingCount = 0;

			if (actual is ICollection<TEnumerable> collectionOfT)
			{
				_matchingCount = collectionOfT.Count;
				_totalCount = _matchingCount;
				Outcome = _quantifier.GetOutcome(_matchingCount, _notMatchingCount, _totalCount);
				_expectationBuilder.AddCollectionContext(actual);
				return Task.FromResult<ConstraintResult>(this);
			}

			IEnumerable materialized = context.UseMaterializedEnumerable(actual);

			foreach (object? _ in materialized)
			{
				_matchingCount++;

				if (_quantifier.IsDeterminable(_matchingCount, _notMatchingCount))
				{
					Outcome = _quantifier.GetOutcome(_matchingCount, _notMatchingCount, _totalCount);
					_expectationBuilder.AddCollectionContext(materialized,
						Outcome == Outcome.Failure && materialized.ExceedsFormatterLimit());
					return Task.FromResult<ConstraintResult>(this);
				}

				if (cancellationToken.IsCancellationRequested)
				{
					Outcome = Outcome.Undecided;
					_expectationBuilder.AddCollectionContext(materialized, true);
					return Task.FromResult<ConstraintResult>(this);
				}
			}

			_totalCount = _matchingCount + _notMatchingCount;
			_expectationBuilder.AddCollectionContext(materialized);
			Outcome = _quantifier.GetOutcome(_matchingCount, _notMatchingCount, _totalCount);
			return Task.FromResult<ConstraintResult>(this);
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("has ", "have "));
			stringBuilder.Append(_quantifier);
			stringBuilder.Append(' ');
			stringBuilder.Append(_quantifier.GetItemString());
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> _quantifier.AppendResult(stringBuilder, Grammars, _matchingCount, _notMatchingCount, _totalCount);

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not have ", "do not have "));
			stringBuilder.Append(_quantifier);
			stringBuilder.Append(' ');
			stringBuilder.Append(_quantifier.GetItemString());
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> _quantifier.AppendResult(stringBuilder, Grammars, _matchingCount, _notMatchingCount,
				_totalCount);
	}

	private sealed class IsInOrderConstraint<TItem, TMember>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		Func<TItem, TMember> memberAccessor,
		SortOrder sortOrder,
		CollectionOrderOptions<TMember> options,
		string memberExpression)
		: ConstraintResult.WithNotNullValue<IEnumerable<TItem>?>(it, grammars),
			IContextConstraint<IEnumerable<TItem>?>
	{
		private string? _failureText;

		public ConstraintResult IsMetBy(IEnumerable<TItem>? actual, IEvaluationContext context)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			IEnumerable<TItem> materialized = context
				.UseMaterializedEnumerable<TItem, IEnumerable<TItem>>(actual);
			expectationBuilder.AddCollectionContext(materialized);

			TMember previous = default!;
			int index = 0;
			IComparer<TMember> comparer = options.GetComparer();
			foreach (TItem item in materialized)
			{
				TMember current = memberAccessor(item);
				if (index++ == 0)
				{
					previous = current;
					continue;
				}

				int comparisonResult = comparer.Compare(previous, current);
				if ((comparisonResult > 0 && sortOrder == aweXpect.SortOrder.Ascending) ||
				    (comparisonResult < 0 && sortOrder == aweXpect.SortOrder.Descending))
				{
					_failureText =
						$"{It} had {Formatter.Format(previous)} before {Formatter.Format(current)} which is not in {sortOrder.ToString().ToLower()} order";
					Outcome = Outcome.Failure;
					return this;
				}

				previous = current;
			}

			Outcome = Outcome.Success;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is in ", "are in ")).Append(sortOrder.ToString().ToLower())
				.Append(SortOrder);
			stringBuilder.Append(options).Append(memberExpression);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(_failureText);

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not in ", "are not in ")).Append(sortOrder.ToString().ToLower())
				.Append(SortOrder);
			stringBuilder.Append(options).Append(memberExpression);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was", " were"));
	}

	private sealed class IsInOrderForEnumerableConstraint<TEnumerable, TItem, TMember>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		Func<TItem, TMember> memberAccessor,
		SortOrder sortOrder,
		CollectionOrderOptions<TMember> options,
		string memberExpression)
		: ConstraintResult.WithNotNullValue<TEnumerable>(it, grammars),
			IContextConstraint<TEnumerable>
		where TEnumerable : IEnumerable?
	{
		private string? _failureText;

		public ConstraintResult IsMetBy(TEnumerable actual, IEvaluationContext context)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			IEnumerable materialized = context.UseMaterializedEnumerable(actual);
			expectationBuilder.AddCollectionContext(materialized);

			TMember previous = default!;
			int index = 0;
			IComparer<TMember> comparer = options.GetComparer();
			foreach (object? item in materialized)
			{
				if (item is not TItem typedItem)
				{
					continue;
				}

				TMember current = memberAccessor(typedItem);
				if (index++ == 0)
				{
					previous = current;
					continue;
				}

				int comparisonResult = comparer.Compare(previous, current);
				if ((comparisonResult > 0 && sortOrder == aweXpect.SortOrder.Ascending) ||
				    (comparisonResult < 0 && sortOrder == aweXpect.SortOrder.Descending))
				{
					_failureText =
						$"{It} had {Formatter.Format(previous)} before {Formatter.Format(current)} which is not in {sortOrder.ToString().ToLower()} order";
					Outcome = Outcome.Failure;
					return this;
				}

				previous = current;
			}

			Outcome = Outcome.Success;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is in ", "are in ")).Append(sortOrder.ToString().ToLower())
				.Append(SortOrder);
			stringBuilder.Append(options).Append(memberExpression);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(_failureText);

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not in ", "are not in ")).Append(sortOrder.ToString().ToLower())
				.Append(SortOrder);
			stringBuilder.Append(options).Append(memberExpression);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was", " were"));
	}
}
