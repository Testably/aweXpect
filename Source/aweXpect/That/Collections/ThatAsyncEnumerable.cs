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
	private sealed class IsEqualToConstraint<TItem, TMatch>(
		string it,
		ExpectationGrammars grammars,
		string expectedExpression,
		IEnumerable<TItem>? expected,
		IOptionsEquality<TMatch> options,
		CollectionMatchOptions matchOptions,
		bool failsForNullSubject = false)
		: ConstraintResult.WithEqualToValue<IAsyncEnumerable<TItem>?>(it, grammars, expected is null),
			IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
		where TItem : TMatch
	{
		private CollectionContext _collectionContext;
		private ICollection<TItem>? _expectedItems;
		private string? _failure;
		private List<TItem>? _items = [];

		public override Outcome Outcome
		{
			get => failsForNullSubject && Actual is null ? Outcome.Failure : base.Outcome;
			protected set => base.Outcome = value;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
		{
			_collectionContext.AppendTo(contexts);
			if (expected is not null && _expectedItems is not null)
			{
				contexts.AddExpectedItemsContext(expected, _expectedItems);
			}
			contexts.AddOptionsContexts(options);
		}

		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<TItem>? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_collectionContext = default;
			_expectedItems = null;
			Actual = actual;
			if (actual is null)
			{
				Outcome = expected is null ? Outcome.Success : Outcome.Failure;
				return this;
			}

			if (expected is null)
			{
				Outcome = Outcome.Failure;
				_collectionContext.Set(
					context.UseMaterializedAsyncEnumerable<TItem>(actual, cancellationToken) as IMaterializedAsyncEnumerable<TItem>);
				return this;
			}

			IAsyncEnumerable<TItem> materializedEnumerable =
				context.UseMaterializedAsyncEnumerable<TItem>(actual, cancellationToken);
			ICollection<TItem> expectedItems = expected as ICollection<TItem> ?? expected.ToArray();
			ICollectionMatcher<TItem, TMatch> matcher = matchOptions.GetCollectionMatcher<TItem, TMatch>(expectedItems);
			IOptionsEquality<TMatch> itemOptions = options is ObjectEqualityOptions<TMatch> objectOptions
				? objectOptions.ForEvaluation()
				: options;
			int maximumNumber = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();
			if (IsNegated)
			{
				_items = [];
			}

			_expectedItems = expectedItems;
			_collectionContext.Set(materializedEnumerable as IMaterializedAsyncEnumerable<TItem>);
			await foreach (TItem item in materializedEnumerable.WithCancellation(cancellationToken))
			{
				if (_items?.Count <= maximumNumber)
				{
					_items.Add(item);
				}

				var (result, failure) = await matcher.Verify(It, item, itemOptions, maximumNumber);
				if (result)
				{
					_failure = failure ?? TooManyDeviationsError();
					Outcome = Outcome.Failure;
					return this;
				}

				if (matcher.IsDetermined)
				{
					break;
				}
			}

			var (completedResult, completedFailure) = await matcher.VerifyComplete(It, itemOptions, maximumNumber);
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
			sb.Append(2L * maximumNumberOfCollectionItems);
			sb.Append(" deviations");
			return sb.ToString();
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			// The options qualify the expected items, not their order.
			stringBuilder.Append(matchOptions.GetExpectation(expectedExpression + options, Grammars));
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (expected is null)
			{
				stringBuilder.Append("the expected collection was <null>");
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
				stringBuilder.Append("the expected collection was <null>");
			}
			else
			{
				stringBuilder.Append(It).Append(matchOptions.GetNegatedResultVerb(It, Grammars));
			}
		}
	}

	private sealed class IsEqualToFromExpectationsConstraint<TItem, TMatch>(
		string it,
		ExpectationGrammars grammars,
		string expectedExpression,
		IEnumerable<Action<IThatSubject<TItem?>>>? expected,
		CollectionMatchOptions matchOptions,
		bool failsForNullSubject = false)
		: ConstraintResult.WithEqualToValue<IAsyncEnumerable<TItem>?>(it, grammars, expected is null),
			IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
		where TItem : TMatch
	{
		private CollectionContext _collectionContext;
		private CollectionMatchOptions.ExpectationItem<TItem>[] _expectations = [];
		private bool _showsExpected;

		private string? _failure;
		private List<TItem>? _items = [];

		public override Outcome Outcome
		{
			get => failsForNullSubject && Actual is null ? Outcome.Failure : base.Outcome;
			protected set => base.Outcome = value;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
		{
			_collectionContext.AppendTo(contexts);
			if (_showsExpected)
			{
				CollectionMatchOptions.ExpectationItem<TItem>[] expectations = _expectations;
				contexts.Add(new ResultContext.SyncCallback("Expected",
					() => Formatter.Format(expectations, typeof(TItem).GetFormattingOption(expectations.Length)), -2));
			}
		}

		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<TItem>? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_collectionContext = default;
			_showsExpected = false;
			Actual = actual;
			if (actual is null)
			{
				Outcome = expected is null ? Outcome.Success : Outcome.Failure;
				return this;
			}

			if (expected is null)
			{
				Outcome = Outcome.Failure;
				_collectionContext.Set(
					context.UseMaterializedAsyncEnumerable<TItem>(actual, cancellationToken) as IMaterializedAsyncEnumerable<TItem>);
				return this;
			}

			IAsyncEnumerable<TItem> materializedEnumerable =
				context.UseMaterializedAsyncEnumerable<TItem>(actual, cancellationToken);
			_expectations = expected.Select(expectation
					=> new CollectionMatchOptions.ExpectationItem<TItem>(expectation,
						Grammars & ~ExpectationGrammars.Negated,
						context,
						cancellationToken))
				.ToArray();
			await PrepareExpectations();
			ICollectionMatcher<TItem, TMatch> matcher = matchOptions.GetCollectionMatcher<TItem, TMatch>(_expectations);
			int maximumNumber = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();
			if (IsNegated)
			{
				_items = [];
			}

			_showsExpected = true;
			NoOptions noOptions = new();
			_collectionContext.Set(materializedEnumerable as IMaterializedAsyncEnumerable<TItem>);
			await foreach (TItem item in materializedEnumerable.WithCancellation(cancellationToken))
			{
				if (_items?.Count <= maximumNumber)
				{
					_items.Add(item);
				}

				var (result, failure) = await matcher.Verify(It, item, noOptions, maximumNumber);
				// A canceled item expectation does not match, which must not be reported as a mismatch.
				if (result && !IsAnItemExpectationCanceled(cancellationToken))
				{
					_failure = failure ?? TooManyDeviationsError();
					Outcome = Outcome.Failure;
					return this;
				}

				if (matcher.IsDetermined)
				{
					break;
				}
			}

			var (completedResult, completedFailure) = await matcher.VerifyComplete(It, noOptions, maximumNumber);
			// The final check can evaluate item expectations as well, e.g. to reassign items in any order.
			if (IsAnItemExpectationCanceled(cancellationToken))
			{
				Outcome = Outcome.Undecided;
				return this;
			}

			if (completedResult)
			{
				_failure = completedFailure ?? TooManyDeviationsError();
				Outcome = Outcome.Failure;
				return this;
			}

			Outcome = Outcome.Success;
			return this;
		}

		private async Task PrepareExpectations()
		{
			foreach (CollectionMatchOptions.ExpectationItem<TItem> expectation in _expectations)
			{
				await expectation.PrepareExpectation();
			}
		}

		private bool IsAnItemExpectationCanceled(CancellationToken cancellationToken)
			=> cancellationToken.IsCancellationRequested && _expectations.Any(expectation => expectation.IsUndecided);

		private string TooManyDeviationsError()
		{
			int maximumNumberOfCollectionItems =
				Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();
			StringBuilder sb = new();
			sb.Append(It);
			sb.Append(" had more than ");
			sb.Append(2L * maximumNumberOfCollectionItems);
			sb.Append(" deviations");
			return sb.ToString();
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(matchOptions.GetExpectation(expectedExpression, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (expected is null)
			{
				stringBuilder.Append("the expected collection was <null>");
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
				stringBuilder.Append("the expected collection was <null>");
			}
			else
			{
				stringBuilder.Append(It).Append(matchOptions.GetNegatedResultVerb(It, Grammars));
			}
		}

		private sealed class NoOptions : IOptionsEquality<TMatch>
		{
			public ValueTask<bool> AreConsideredEqual<TExpected>(TMatch actual, TExpected expected)
				=> new ValueTask<bool>(Equals(actual, expected));
		}
	}

	private sealed class IsEqualToFromPredicateConstraint<TItem, TMatch>(
		string it,
		ExpectationGrammars grammars,
		string expectedExpression,
		IEnumerable<Expression<Func<TItem, bool>>>? expected,
		CollectionMatchOptions matchOptions,
		bool failsForNullSubject = false)
		: ConstraintResult.WithEqualToValue<IAsyncEnumerable<TItem>?>(it, grammars, expected is null),
			IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
		where TItem : TMatch
	{
		private CollectionContext _collectionContext;
		private ICollection<Expression<Func<TItem, bool>>>? _expectedItems;
		private string? _failure;
		private List<TItem>? _items = [];

		public override Outcome Outcome
		{
			get => failsForNullSubject && Actual is null ? Outcome.Failure : base.Outcome;
			protected set => base.Outcome = value;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
		{
			_collectionContext.AppendTo(contexts);
			if (_expectedItems is { } expectedItems)
			{
				contexts.Add(new ResultContext.SyncCallback("Expected",
					() => Formatter.Format(expectedItems, FormattingOptions.MultipleLines), -2));
			}
		}

		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<TItem>? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_collectionContext = default;
			_expectedItems = null;
			Actual = actual;
			if (actual is null)
			{
				Outcome = expected is null ? Outcome.Success : Outcome.Failure;
				return this;
			}

			if (expected is null)
			{
				Outcome = Outcome.Failure;
				_collectionContext.Set(
					context.UseMaterializedAsyncEnumerable<TItem>(actual, cancellationToken) as IMaterializedAsyncEnumerable<TItem>);
				return this;
			}

			IAsyncEnumerable<TItem> materializedEnumerable =
				context.UseMaterializedAsyncEnumerable<TItem>(actual, cancellationToken);
			ICollection<Expression<Func<TItem, bool>>> expectedItems =
				expected as ICollection<Expression<Func<TItem, bool>>> ?? expected.ToArray();
			ICollectionMatcher<TItem, TMatch> matcher = matchOptions.GetCollectionMatcher<TItem, TMatch>(expectedItems);
			int maximumNumber = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();
			if (IsNegated)
			{
				_items = [];
			}

			_expectedItems = expectedItems;
			NoOptions noOptions = new();
			_collectionContext.Set(materializedEnumerable as IMaterializedAsyncEnumerable<TItem>);
			await foreach (TItem item in materializedEnumerable.WithCancellation(cancellationToken))
			{
				if (_items?.Count <= maximumNumber)
				{
					_items.Add(item);
				}

				var (result, failure) = await matcher.Verify(It, item, noOptions, maximumNumber);
				if (result)
				{
					_failure = failure ?? TooManyDeviationsError();
					Outcome = Outcome.Failure;
					return this;
				}

				if (matcher.IsDetermined)
				{
					break;
				}
			}

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
			sb.Append(2L * maximumNumberOfCollectionItems);
			sb.Append(" deviations");
			return sb.ToString();
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(matchOptions.GetExpectation(expectedExpression, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (expected is null)
			{
				stringBuilder.Append("the expected collection was <null>");
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
				stringBuilder.Append("the expected collection was <null>");
			}
			else
			{
				stringBuilder.Append(It).Append(matchOptions.GetNegatedResultVerb(It, Grammars));
			}
		}

		private sealed class NoOptions : IOptionsEquality<TMatch>
		{
			public ValueTask<bool> AreConsideredEqual<TExpected>(TMatch actual, TExpected expected)
				=> new ValueTask<bool>(Equals(actual, expected));
		}
	}
}
#endif
