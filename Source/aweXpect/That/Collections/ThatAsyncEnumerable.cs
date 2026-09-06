#if NET8_0_OR_GREATER
using System;
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
///     Expectations on <see cref="IAsyncEnumerable{TItem}" />.
/// </summary>
public static partial class ThatAsyncEnumerable
{
	private sealed class CollectionConstraint<TItem>
		: ConstraintResult.WithNotNullValue<IAsyncEnumerable<TItem>?>,
			IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
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
			if (Grammars.HasFlag(ExpectationGrammars.Nested))
			{
				stringBuilder.Append(_quantifier);
				stringBuilder.Append(' ');
				stringBuilder.Append(_expectationText(Grammars));
			}
			else
			{
				stringBuilder.Append(_expectationText(Grammars));
				stringBuilder.Append(" for ");
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
				stringBuilder.Append(" for ");
				stringBuilder.Append(_quantifier);
				stringBuilder.Append(' ');
				stringBuilder.Append(_quantifier.GetItemString());
			}
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> _quantifier.AppendResult(stringBuilder, Grammars, _matchingCount, _notMatchingCount, _totalCount, _verb);

		private void AppendContexts(bool isIncomplete)
		{
			EnumerableQuantifier.QuantifierContexts quantifierContexts = _quantifier.GetQuantifierContext();
			if (quantifierContexts.HasFlag(EnumerableQuantifier.QuantifierContexts.MatchingItems) &&
			    _matchingItems?.Count > 0)
			{
				_expectationBuilder.AddContext(new ResultContext.SyncCallback("Matching items",
						() => Formatter.Format(_matchingItems,
								typeof(TItem).GetFormattingOption(_matchingItems?.Count))
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

	private sealed class AsyncCollectionConstraint<TItem>
		: ConstraintResult.WithValue<IAsyncEnumerable<TItem>?>,
			IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
	{
		private readonly ExpectationBuilder _expectationBuilder;
		private readonly Func<ExpectationGrammars, string> _expectationText;
		private readonly ExpectationGrammars _grammars;
		private readonly string _it;
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
			string verb) : base(grammars)
		{
			_expectationBuilder = expectationBuilder;
			_it = it;
			_grammars = grammars;
			_quantifier = quantifier;
			_expectationText = expectationText;
			_predicate = predicate;
			_verb = verb;
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
			if (_grammars.HasFlag(ExpectationGrammars.Nested))
			{
				stringBuilder.Append(_quantifier);
				stringBuilder.Append(' ');
				stringBuilder.Append(_expectationText(_grammars));
			}
			else
			{
				stringBuilder.Append(_expectationText(_grammars));
				stringBuilder.Append(" for ");
				stringBuilder.Append(_quantifier);
				stringBuilder.Append(' ');
				stringBuilder.Append(_quantifier.GetItemString());
			}
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.ItWasNull(_it, _grammars);
			}
			else
			{
				_quantifier.AppendResult(stringBuilder, _grammars, _matchingCount, _notMatchingCount, _totalCount,
					_verb);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_grammars.HasFlag(ExpectationGrammars.Nested))
			{
				stringBuilder.Append("not ");
				stringBuilder.Append(_quantifier);
				stringBuilder.Append(' ');
				stringBuilder.Append(_expectationText(_grammars));
			}
			else
			{
				stringBuilder.Append(_expectationText(_grammars.Negate()));
				stringBuilder.Append(" for ");
				stringBuilder.Append(_quantifier);
				stringBuilder.Append(' ');
				stringBuilder.Append(_quantifier.GetItemString());
			}
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.ItWasNull(_it, _grammars);
			}
			else
			{
				_quantifier.AppendResult(stringBuilder, _grammars.Negate(), _matchingCount, _notMatchingCount,
					_totalCount, _verb);
			}
		}

		private void AppendContexts(bool isIncomplete)
		{
			EnumerableQuantifier.QuantifierContexts quantifierContexts = _quantifier.GetQuantifierContext();
			if (quantifierContexts.HasFlag(EnumerableQuantifier.QuantifierContexts.MatchingItems) &&
			    _matchingItems?.Count > 0)
			{
				_expectationBuilder.AddContext(new ResultContext.SyncCallback("Matching items",
						() => Formatter.Format(_matchingItems,
								typeof(TItem).GetFormattingOption(_matchingItems?.Count))
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

	private sealed class AsyncCollectionCountConstraint<TItem>
		: ConstraintResult.WithNotNullValue<IAsyncEnumerable<TItem>?>,
			IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
	{
		private readonly ExpectationBuilder _expectationBuilder;
		private readonly ExpectationGrammars _grammars;
		private readonly EnumerableQuantifier _quantifier;
		private int _matchingCount;
		private int _notMatchingCount;
		private int? _totalCount;

		public AsyncCollectionCountConstraint(
			ExpectationBuilder expectationBuilder,
			string it,
			ExpectationGrammars grammars,
			EnumerableQuantifier quantifier)
			: base(it, grammars)
		{
			_expectationBuilder = expectationBuilder;
			_grammars = grammars;
			_quantifier = quantifier;
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

			await foreach (TItem _ in materialized.WithCancellation(cancellationToken))
			{
				_matchingCount++;

				if (_quantifier.IsDeterminable(_matchingCount, _notMatchingCount))
				{
					Outcome = _quantifier.GetOutcome(_matchingCount, _notMatchingCount, _totalCount);
					await _expectationBuilder.AddCollectionContext(materialized as IMaterializedEnumerable<TItem>);
					return this;
				}
			}

			if (cancellationToken.IsCancellationRequested)
			{
				Outcome = Outcome.Undecided;
				await _expectationBuilder.AddCollectionContext(materialized as IMaterializedEnumerable<TItem>, true);
				return this;
			}

			_totalCount = _matchingCount + _notMatchingCount;
			Outcome = _quantifier.GetOutcome(_matchingCount, _notMatchingCount, _totalCount);
			await _expectationBuilder.AddCollectionContext(materialized as IMaterializedEnumerable<TItem>);
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("has ", "have "));
			stringBuilder.Append(_quantifier);
			stringBuilder.Append(' ');
			stringBuilder.Append(_quantifier.GetItemString());
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.ItWasNull(It, Grammars);
			}
			else
			{
				_quantifier.AppendResult(stringBuilder, _grammars, _matchingCount, _notMatchingCount, _totalCount);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not have ", "do not have "));
			stringBuilder.Append(_quantifier);
			stringBuilder.Append(' ');
			stringBuilder.Append(_quantifier.GetItemString());
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.ItWasNull(It, Grammars);
			}
			else
			{
				_quantifier.AppendResult(stringBuilder, _grammars.Negate(), _matchingCount, _notMatchingCount,
					_totalCount);
			}
		}
	}

	private sealed class IsEqualToConstraint<TItem, TMatch>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		string expectedExpression,
		IEnumerable<TItem>? expected,
		IOptionsEquality<TMatch> options,
		CollectionMatchOptions matchOptions)
		: ConstraintResult.WithEqualToValue<IAsyncEnumerable<TItem>?>(it, grammars, expected is null),
			IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
		where TItem : TMatch
	{
		private string? _failure;
		private List<TItem>? _items = [];

		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<TItem>? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null || expected is null)
			{
				Outcome = actual is null && expected is null ? Outcome.Success : Outcome.Failure;
				return this;
			}

			IAsyncEnumerable<TItem> materializedEnumerable =
				context.UseMaterializedAsyncEnumerable<TItem, IAsyncEnumerable<TItem>>(actual);
			ICollectionMatcher<TItem, TMatch> matcher = matchOptions.GetCollectionMatcher<TItem, TMatch>(expected);
			int maximumNumber = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();
			if (IsNegated)
			{
				_items = [];
			}

			expectationBuilder.AddContext(new ResultContext.SyncCallback("Expected",
					() => Formatter.Format(expected, typeof(TItem).GetFormattingOption(expected switch
					{
						ICollection<TItem> coll => coll.Count,
						ICountable countable => countable.Count,
						_ => null,
					})),
					-2));
			await foreach (TItem item in materializedEnumerable.WithCancellation(cancellationToken))
			{
				if (_items?.Count < maximumNumber + 1)
				{
					_items.Add(item);
				}

				var (result, failure) = await matcher.Verify(It, item, options, maximumNumber);
				if (result)
				{
					_failure = failure ?? TooManyDeviationsError();
					Outcome = Outcome.Failure;
					await expectationBuilder.AddCollectionContext(
						materializedEnumerable as IMaterializedEnumerable<TItem>);
					return this;
				}
			}

			await expectationBuilder.AddCollectionContext(materializedEnumerable as IMaterializedEnumerable<TItem>);
			var (completedResult, completedFailure) = await matcher.VerifyComplete(It, options, maximumNumber);
			if (completedResult)
			{
				_failure = completedFailure ?? TooManyDeviationsError();
				Outcome = Outcome.Failure;
				return this;
			}

			Outcome = Outcome.Success;
			return this;
		}

		private string TooManyDeviationsError()
		{
			int maximumNumberOfCollectionItems =
				Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();
			StringBuilder sb = new();
			sb.Append(It);
			sb.Append(" had more than ");
			sb.Append(2 * maximumNumberOfCollectionItems);
			sb.Append(" deviations");
			return sb.ToString();
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(matchOptions.GetExpectation(expectedExpression, Grammars));
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (expected is null)
			{
				stringBuilder.Append(It).Append(" cannot compare to <null>");
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
				stringBuilder.Append(It).Append(" cannot compare to <null>");
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
		string expectedExpression,
		IEnumerable<Action<IThatSubject<TItem?>>>? expected,
		CollectionMatchOptions matchOptions)
		: ConstraintResult.WithEqualToValue<IAsyncEnumerable<TItem>?>(it, grammars, expected is null),
			IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
		where TItem : TMatch
	{
		private CollectionMatchOptions.ExpectationItem<TItem>[] _expectations = [];

		private string? _failure;
		private List<TItem>? _items = [];

		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<TItem>? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null || expected is null)
			{
				Outcome = actual is null && expected is null ? Outcome.Success : Outcome.Failure;
				return this;
			}

			IAsyncEnumerable<TItem> materializedEnumerable =
				context.UseMaterializedAsyncEnumerable<TItem, IAsyncEnumerable<TItem>>(actual);
			_expectations = expected.Select(expectation
					=> new CollectionMatchOptions.ExpectationItem<TItem>(expectation,
						Grammars & ~ExpectationGrammars.Negated,
						context,
						CancellationToken.None))
				.ToArray();
			ICollectionMatcher<TItem, TMatch> matcher = matchOptions.GetCollectionMatcher<TItem, TMatch>(_expectations);
			int maximumNumber = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();
			if (IsNegated)
			{
				_items = [];
			}

			expectationBuilder.AddContext(new ResultContext.SyncCallback("Expected",
					() => Formatter.Format(_expectations, typeof(TItem).GetFormattingOption(_expectations.Length)),
					-2));
			NoOptions noOptions = new();
			await foreach (TItem item in materializedEnumerable.WithCancellation(cancellationToken))
			{
				if (_items?.Count < maximumNumber + 1)
				{
					_items.Add(item);
				}

				var (result, failure) = await matcher.Verify(It, item, noOptions, maximumNumber);
				if (result)
				{
					_failure = failure ?? TooManyDeviationsError();
					Outcome = Outcome.Failure;
					await expectationBuilder.AddCollectionContext(
						materializedEnumerable as IMaterializedEnumerable<TItem>);
					return this;
				}
			}

			await expectationBuilder.AddCollectionContext(materializedEnumerable as IMaterializedEnumerable<TItem>);
			var (completedResult, completedFailure) = await matcher.VerifyComplete(It, noOptions, maximumNumber);
			if (completedResult)
			{
				_failure = completedFailure ?? TooManyDeviationsError();
				Outcome = Outcome.Failure;
				return this;
			}

			Outcome = Outcome.Success;
			return this;
		}

		private string TooManyDeviationsError()
		{
			int maximumNumberOfCollectionItems =
				Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();
			StringBuilder sb = new();
			sb.Append(It);
			sb.Append(" had more than ");
			sb.Append(2 * maximumNumberOfCollectionItems);
			sb.Append(" deviations");
			return sb.ToString();
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(matchOptions.GetExpectation(expectedExpression, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (expected is null)
			{
				stringBuilder.Append(It).Append(" cannot compare to <null>");
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
				stringBuilder.Append(It).Append(" cannot compare to <null>");
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
		string expectedExpression,
		IEnumerable<Expression<Func<TItem, bool>>>? expected,
		CollectionMatchOptions matchOptions)
		: ConstraintResult.WithEqualToValue<IAsyncEnumerable<TItem>?>(it, grammars, expected is null),
			IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
		where TItem : TMatch
	{
		private string? _failure;
		private List<TItem>? _items = [];

		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<TItem>? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null || expected is null)
			{
				Outcome = actual is null && expected is null ? Outcome.Success : Outcome.Failure;
				return this;
			}

			IAsyncEnumerable<TItem> materializedEnumerable =
				context.UseMaterializedAsyncEnumerable<TItem, IAsyncEnumerable<TItem>>(actual);
			ICollectionMatcher<TItem, TMatch> matcher = matchOptions.GetCollectionMatcher<TItem, TMatch>(expected);
			int maximumNumber = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();
			if (IsNegated)
			{
				_items = [];
			}

			expectationBuilder.AddContext(new ResultContext.SyncCallback("Expected",
					() => Formatter.Format(expected, typeof(TItem).GetFormattingOption(expected switch
					{
						ICollection<TItem> coll => coll.Count,
						ICountable countable => countable.Count,
						_ => null,
					})),
					-2));
			NoOptions noOptions = new();
			await foreach (TItem item in materializedEnumerable.WithCancellation(cancellationToken))
			{
				if (_items?.Count < maximumNumber + 1)
				{
					_items.Add(item);
				}

				var (result, failure) = await matcher.Verify(It, item, noOptions, maximumNumber);
				if (result)
				{
					_failure = failure ?? TooManyDeviationsError();
					Outcome = Outcome.Failure;
					await expectationBuilder.AddCollectionContext(
						materializedEnumerable as IMaterializedEnumerable<TItem>);
					return this;
				}
			}

			await expectationBuilder.AddCollectionContext(materializedEnumerable as IMaterializedEnumerable<TItem>);
			var (completedResult, completedFailure) = await matcher.VerifyComplete(It, noOptions, maximumNumber);
			if (completedResult)
			{
				_failure = completedFailure ?? TooManyDeviationsError();
				Outcome = Outcome.Failure;
				return this;
			}

			Outcome = Outcome.Success;
			return this;
		}

		private string TooManyDeviationsError()
		{
			int maximumNumberOfCollectionItems =
				Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();
			StringBuilder sb = new();
			sb.Append(It);
			sb.Append(" had more than ");
			sb.Append(2 * maximumNumberOfCollectionItems);
			sb.Append(" deviations");
			return sb.ToString();
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(matchOptions.GetExpectation(expectedExpression, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (expected is null)
			{
				stringBuilder.Append(It).Append(" cannot compare to <null>");
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
				stringBuilder.Append(It).Append(" cannot compare to <null>");
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

	private sealed class IsInOrderConstraint<TItem, TMember>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		Func<TItem, TMember> memberAccessor,
		SortOrder sortOrder,
		CollectionOrderOptions<TMember> options,
		string memberExpression)
		: ConstraintResult.WithNotNullValue<IAsyncEnumerable<TItem>?>(it, grammars),
			IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
	{
		private string? _failureText;

		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<TItem>? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			IAsyncEnumerable<TItem> materialized = context
				.UseMaterializedAsyncEnumerable<TItem, IAsyncEnumerable<TItem>>(actual);
			await expectationBuilder.AddCollectionContext(materialized as IMaterializedEnumerable<TItem>);

			TMember previous = default!;
			int index = 0;
			int maximumNumberOfCollectionItems =
				Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();
			IComparer<TMember> comparer = options.GetComparer();
			await foreach (TItem item in materialized.WithCancellation(cancellationToken))
			{
				TMember current = memberAccessor(item);
				if (index++ == 0)
				{
					previous = current;
					continue;
				}

				int comparisonResult = comparer.Compare(previous, current);
				if ((comparisonResult > 0 && sortOrder == SortOrder.Ascending) ||
				    (comparisonResult < 0 && sortOrder == SortOrder.Descending))
				{
					_failureText ??=
						$"{It} had {Formatter.Format(previous)} before {Formatter.Format(current)} which is not in {sortOrder.ToString().ToLower()} order";
				}

				if (_failureText != null && index > maximumNumberOfCollectionItems)
				{
					break;
				}

				previous = current;
			}

			Outcome = _failureText != null ? Outcome.Failure : Outcome.Success;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is in ", "are in ")).Append(sortOrder.ToString().ToLower())
				.Append(" order");
			stringBuilder.Append(options).Append(memberExpression);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(_failureText);

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not in ", "are not in ")).Append(sortOrder.ToString().ToLower())
				.Append(" order");
			stringBuilder.Append(options).Append(memberExpression);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was", " were"));
	}
}
#endif
