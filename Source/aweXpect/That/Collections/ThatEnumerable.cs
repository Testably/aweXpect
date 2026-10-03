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
using aweXpect.SourceGenerators;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

/// <summary>
///     Expectations on <see cref="IEnumerable{TItem}" />.
/// </summary>
[CollectionSubjects("System.Collections.Immutable.ImmutableArray<{item}>")]
public static partial class ThatEnumerable
{
	private const string ExpectedCollectionWasNull = "the expected collection was <null>";

	/// <remarks>
	///     When <paramref name="usesDefaultEquality" /> tells that the comparison was not changed, a subject that is a set
	///     of <typeparamref name="TItem" /> with a custom comparer compares its items with that comparer, as it does for a
	///     single item in <c>Contains</c>, and the expectation names it.
	/// </remarks>
	private sealed class IsEqualToConstraint<TEnumerable, TItem, TMatch>(
		string it,
		ExpectationGrammars grammars,
		string? expectedExpression,
		IEnumerable<TItem>? expected,
		IOptionsEquality<TMatch> options,
		CollectionMatchOptions matchOptions,
		bool failsForNullSubject = false,
		Func<bool>? usesDefaultEquality = null)
		: ConstraintResult.WithEqualToValue<TEnumerable?>(it, grammars, expected is null),
			IAsyncContextConstraint<TEnumerable?>
		where TEnumerable : IEnumerable?
		where TItem : TMatch
	{
		private CollectionContext _collectionContext;
		private ICollection<TItem>? _expectedItems;
		private string? _failure;
		private SubjectComparer<TItem>? _subjectComparer;

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

		public async Task<ConstraintResult> IsMetBy(TEnumerable? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_collectionContext = default;
			_expectedItems = null;
			_failure = null;
			_subjectComparer = null;
			Actual = actual;
			if (actual.IsDefaultImmutableArray())
			{
				return this.AsNullSubject(It);
			}

			if (actual is null)
			{
				Outcome = expected is null ? Outcome.Success : Outcome.Failure;
				return this;
			}

			bool isTyped = CollectionItems<TItem>.IsTyped<TEnumerable>();
			if (expected is null)
			{
				Outcome = Outcome.Failure;
				if (isTyped)
				{
					CollectionItems<TItem>.Materialize(actual, context).SetContext(ref _collectionContext);
				}
				else
				{
					CollectionItems<object?>.Materialize(actual, context).SetContext(ref _collectionContext);
				}

				return this;
			}

			ICollection<TItem> expectedItems = expected as ICollection<TItem> ?? expected.ToArray();
			_expectedItems = expectedItems;
			SubjectEqualityOptions<TItem, TMatch> subjectOptions = new(
				options is ObjectEqualityOptions<TMatch> objectOptions ? objectOptions.ForEvaluation() : options,
				usesDefaultEquality ?? (() => false));
			_subjectComparer = subjectOptions.UseComparerOf(actual) ? subjectOptions.Comparer : null;
			int maximumNumber = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();
			return isTyped
				? await Verify(CollectionItems<TItem>.Materialize(actual, context),
					matchOptions.GetCollectionMatcher<TItem, TMatch>(expectedItems), subjectOptions, maximumNumber,
					cancellationToken)
				: await Verify(CollectionItems<object?>.Materialize(actual, context),
					matchOptions.GetCollectionMatcher<object?, object?>(expectedItems.Cast<object?>()),
					new UntypedOptions(subjectOptions), maximumNumber, cancellationToken);
		}

		private async Task<ConstraintResult> Verify<TRead, TReadMatch>(CollectionItems<TRead> materialized,
			ICollectionMatcher<TRead, TReadMatch> matcher, IOptionsEquality<TReadMatch> itemOptions, int maximumNumber,
			CancellationToken cancellationToken)
			where TRead : TReadMatch
		{
			foreach (TRead item in materialized.Items)
			{
				if (materialized.IsCanceledBeforeTheEnd(cancellationToken))
				{
					Outcome = Outcome.Undecided;
					materialized.SetContext(ref _collectionContext, true);
					return this;
				}

				var (result, failure) = await matcher.Verify(It, item, itemOptions, maximumNumber);
				if (result)
				{
					_failure = failure ?? TooManyDeviationsError();
					Outcome = Outcome.Failure;
					materialized.SetContext(ref _collectionContext);
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
				materialized.SetContext(ref _collectionContext);
				return this;
			}

			materialized.SetContext(ref _collectionContext);
			Outcome = Outcome.Success;
			return this;
		}

		private string TooManyDeviationsError()
			=> $"{It} had more than {2L * Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get()} deviations";

		/// <summary>
		///     A non-generic subject can contain items of any type, so an item that is not a
		///     <typeparamref name="TMatch" /> never equals an expected item.
		/// </summary>
		private sealed class UntypedOptions(IOptionsEquality<TMatch> options) : IOptionsEquality<object?>
		{
			public async ValueTask<bool> AreConsideredEqual<TExpected>(object? actual, TExpected expected)
				=> TryCastItem(actual, out TMatch typedActual)
				   && await options.AreConsideredEqual(typedActual, (TItem)(object?)expected!);
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			string expectedText = expectedExpression ?? Formatter.Format(expected, FormattingOptions.SingleLine);
			// The options qualify the expected items, not their order.
			stringBuilder.Append(matchOptions.GetExpectation(expectedText + options + _subjectComparer, Grammars));
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (expected is null)
			{
				stringBuilder.Append(ExpectedCollectionWasNull);
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
				stringBuilder.Append(ExpectedCollectionWasNull);
			}
			else
			{
				stringBuilder.Append(It).Append(matchOptions.GetNegatedResultVerb(It, Grammars));
			}
		}
	}

	private sealed class IsEqualToFromExpectationsConstraint<TEnumerable, TItem, TMatch>(
		string it,
		ExpectationGrammars grammars,
		string? expectedExpression,
		IEnumerable<Action<IThatSubject<TItem?>>>? expected,
		CollectionMatchOptions matchOptions,
		bool failsForNullSubject = false)
		: ConstraintResult.WithEqualToValue<TEnumerable?>(it, grammars, expected is null),
			IAsyncContextConstraint<TEnumerable?>
		where TEnumerable : IEnumerable<TItem>?
		where TItem : TMatch
	{
		private CollectionContext _collectionContext;
		private CollectionMatchOptions.ExpectationItem<TItem>[] _expectations = [];
		private bool _showsExpected;

		private string? _failure;

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

		public async Task<ConstraintResult> IsMetBy(TEnumerable? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_collectionContext = default;
			_showsExpected = false;
			Actual = actual;
			if (actual.IsDefaultImmutableArray())
			{
				return this.AsNullSubject(It);
			}

			if (actual is null)
			{
				Outcome = expected is null ? Outcome.Success : Outcome.Failure;
				return this;
			}

			if (expected is null)
			{
				Outcome = Outcome.Failure;
				_collectionContext.Set(
					context.UseMaterializedEnumerable<TItem>(actual));
				return this;
			}

			IEnumerable<TItem> materializedEnumerable =
				context.UseMaterializedEnumerable<TItem>(actual);
			_expectations = expected.Select(expectation
					=> new CollectionMatchOptions.ExpectationItem<TItem>(expectation,
						Grammars & ~ExpectationGrammars.Negated,
						context,
						cancellationToken))
				.ToArray();
			await PrepareExpectations();
			_showsExpected = true;
			ICollectionMatcher<TItem, TMatch> matcher = matchOptions.GetCollectionMatcher<TItem, TMatch>(_expectations);
			int maximumNumber = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();

			NoOptions noOptions = new();
			foreach (TItem item in materializedEnumerable)
			{
				if (cancellationToken.IsCanceledBeforeTheEndOf(materializedEnumerable))
				{
					Outcome = Outcome.Undecided;
					_collectionContext.Set(materializedEnumerable, true);
					return this;
				}

				var (result, failure) = await matcher.Verify(It, item, noOptions, maximumNumber);
				// A canceled item expectation does not match, which must not be reported as a mismatch.
				if (result && !IsAnItemExpectationCanceled(cancellationToken))
				{
					_failure = failure ?? TooManyDeviationsError();
					Outcome = Outcome.Failure;
					_collectionContext.Set(materializedEnumerable);
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
				_collectionContext.Set(materializedEnumerable);
				return this;
			}

			if (completedResult)
			{
				_failure = completedFailure ?? TooManyDeviationsError();
				Outcome = Outcome.Failure;
				_collectionContext.Set(materializedEnumerable);
				return this;
			}

			_collectionContext.Set(materializedEnumerable);
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
			=> $"{It} had more than {2L * Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get()} deviations";

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(matchOptions.GetExpectation(
				expectedExpression ?? Formatter.Format(_expectations, FormattingOptions.SingleLine), Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (expected is null)
			{
				stringBuilder.Append(ExpectedCollectionWasNull);
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
				stringBuilder.Append(ExpectedCollectionWasNull);
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

	private sealed class IsEqualToFromPredicateConstraint<TEnumerable, TItem, TMatch>(
		string it,
		ExpectationGrammars grammars,
		string? expectedExpression,
		IEnumerable<Expression<Func<TItem, bool>>>? expected,
		CollectionMatchOptions matchOptions,
		bool failsForNullSubject = false)
		: ConstraintResult.WithEqualToValue<TEnumerable?>(it, grammars, expected is null),
			IAsyncContextConstraint<TEnumerable?>
		where TEnumerable : IEnumerable<TItem>?
		where TItem : TMatch
	{
		private CollectionContext _collectionContext;
		private ICollection<Expression<Func<TItem, bool>>>? _expectedItems;
		private string? _failure;

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

		public async Task<ConstraintResult> IsMetBy(TEnumerable? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_collectionContext = default;
			_expectedItems = null;
			Actual = actual;
			if (actual.IsDefaultImmutableArray())
			{
				return this.AsNullSubject(It);
			}

			if (actual is null)
			{
				Outcome = expected is null ? Outcome.Success : Outcome.Failure;
				return this;
			}

			if (expected is null)
			{
				Outcome = Outcome.Failure;
				_collectionContext.Set(
					context.UseMaterializedEnumerable<TItem>(actual));
				return this;
			}

			ICollection<Expression<Func<TItem, bool>>> expectedItems =
				expected as ICollection<Expression<Func<TItem, bool>>> ?? expected.ToArray();
			_expectedItems = expectedItems;
			IEnumerable<TItem> materializedEnumerable =
				context.UseMaterializedEnumerable<TItem>(actual);
			ICollectionMatcher<TItem, TMatch> matcher = matchOptions.GetCollectionMatcher<TItem, TMatch>(expectedItems);
			int maximumNumber = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();

			NoOptions noOptions = new();
			foreach (TItem item in materializedEnumerable)
			{
				if (cancellationToken.IsCanceledBeforeTheEndOf(materializedEnumerable))
				{
					Outcome = Outcome.Undecided;
					_collectionContext.Set(materializedEnumerable, true);
					return this;
				}

				var (result, failure) = await matcher.Verify(It, item, noOptions, maximumNumber);
				if (result)
				{
					_failure = failure ?? TooManyDeviationsError();
					Outcome = Outcome.Failure;
					_collectionContext.Set(materializedEnumerable);
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
				_collectionContext.Set(materializedEnumerable);
				return this;
			}

			_collectionContext.Set(materializedEnumerable);
			Outcome = Outcome.Success;
			return this;
		}

		private string TooManyDeviationsError()
			=> $"{It} had more than {2L * Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get()} deviations";

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(matchOptions.GetExpectation(
				expectedExpression ?? Formatter.Format(expected, FormattingOptions.SingleLine), Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (expected is null)
			{
				stringBuilder.Append(ExpectedCollectionWasNull);
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
				stringBuilder.Append(ExpectedCollectionWasNull);
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
