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
	private const string SortOrder = " order";
	private const string ExpectedCollectionWasNull = "the expected collection was <null>";

	/// <remarks>
	///     When <paramref name="usesDefaultEquality" /> tells that the comparison was not changed, a set subject with a
	///     custom comparer compares its items with that comparer, as it does for a single item in <c>Contains</c>, and the
	///     expectation names it.
	/// </remarks>
	private sealed class IsEqualToConstraint<TItem, TMatch>(
		string it,
		ExpectationGrammars grammars,
		string? expectedExpression,
		IEnumerable<TItem>? expected,
		IOptionsEquality<TMatch> options,
		CollectionMatchOptions matchOptions,
		bool failsForNullSubject = false,
		Func<bool>? usesDefaultEquality = null)
		: ConstraintResult.WithEqualToValue<IEnumerable<TItem>?>(it, grammars, expected is null),
			IAsyncContextConstraint<IEnumerable<TItem>?>
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
		}

		public async Task<ConstraintResult> IsMetBy(IEnumerable<TItem>? actual, IEvaluationContext context,
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
					context.UseMaterializedEnumerable<TItem>(actual));
				return this;
			}

			ICollection<TItem> expectedItems = expected as ICollection<TItem> ?? expected.ToArray();
			_expectedItems = expectedItems;
			IEnumerable<TItem> materializedEnumerable =
				context.UseMaterializedEnumerable<TItem>(actual);
			ICollectionMatcher<TItem, TMatch> matcher = matchOptions.GetCollectionMatcher<TItem, TMatch>(expectedItems);
			int maximumNumber = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();
			IOptionsEquality<TMatch> itemOptions = options is ObjectEqualityOptions<TMatch> objectOptions
				? objectOptions.ForEvaluation()
				: options;
			SubjectEqualityOptions<TItem, TMatch> subjectOptions = new(itemOptions, usesDefaultEquality ?? (() => false));
			_subjectComparer = subjectOptions.UseComparerOf(actual) ? subjectOptions.Comparer : null;
			itemOptions = subjectOptions;

			foreach (TItem item in materializedEnumerable)
			{
				if (cancellationToken.IsCanceledBeforeTheEndOf(materializedEnumerable))
				{
					Outcome = Outcome.Undecided;
					_collectionContext.Set(materializedEnumerable, true);
					return this;
				}

				var (result, failure) = await matcher.Verify(It, item, itemOptions, maximumNumber);
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

			var (completedResult, completedFailure) = await matcher.VerifyComplete(It, itemOptions, maximumNumber);
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

	/// <remarks>
	///     When <paramref name="usesDefaultEquality" /> tells that the comparison was not changed, a subject that is a set
	///     of <typeparamref name="TItem" /> with a custom comparer compares its items with that comparer, and the
	///     expectation names it.
	/// </remarks>
	private sealed class IsEqualToForEnumerableConstraint<TEnumerable, TItem, TMatch>(
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
				_collectionContext.Set(context.UseMaterializedEnumerable(actual));
				return this;
			}

			ICollection<TItem> expectedItems = expected as ICollection<TItem> ?? expected.ToArray();
			_expectedItems = expectedItems;
			IEnumerable materializedEnumerable = context.UseMaterializedEnumerable(actual);
			ICollectionMatcher<object?, object?> matcher =
				matchOptions.GetCollectionMatcher<object?, object?>(expectedItems.Cast<object?>());
			SubjectEqualityOptions<TItem, TMatch> subjectOptions = new(
				options is ObjectEqualityOptions<TMatch> objectOptions ? objectOptions.ForEvaluation() : options,
				usesDefaultEquality ?? (() => false));
			_subjectComparer = subjectOptions.UseComparerOf(actual) ? subjectOptions.Comparer : null;
			UntypedOptions untypedOptions = new(subjectOptions);
			int maximumNumber = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();

			foreach (object? item in materializedEnumerable)
			{
				if (cancellationToken.IsCanceledBeforeTheEndOf(materializedEnumerable))
				{
					Outcome = Outcome.Undecided;
					_collectionContext.Set(materializedEnumerable, true);
					return this;
				}

				var (result, failure) = await matcher.Verify(It, item, untypedOptions, maximumNumber);
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

			var (completedResult, completedFailure) = await matcher.VerifyComplete(It, untypedOptions, maximumNumber);
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

		/// <summary>
		///     The subject can contain items of any type, so an item that is not a <typeparamref name="TMatch" /> never
		///     equals an expected item.
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

	private sealed class CollectionConstraint<TItem>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		EnumerableQuantifier quantifier,
		Func<ExpectationGrammars, string> expectationText,
		Func<TItem, bool> predicate,
		string verb)
		: QuantifiedCollectionConstraint<IEnumerable<TItem>?, TItem>(expectationBuilder, it, grammars, quantifier,
				expectationText, verb),
			IAsyncContextConstraint<IEnumerable<TItem>?>
	{
		private CollectionContext _collectionContext;

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
		{
			_collectionContext.AppendTo(contexts);
			base.AppendContexts(contexts);
		}

		public Task<ConstraintResult> IsMetBy(
			IEnumerable<TItem>? actual,
			IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_collectionContext = default;
			Actual = actual;
			if (actual is null)
			{
				return Task.FromResult<ConstraintResult>(this);
			}

			IEnumerable<TItem> materialized = context.UseMaterializedEnumerable<TItem>(actual);
			bool cancelEarly = actual is not ICollection<TItem>;
			foreach (TItem item in materialized)
			{
				Record(item, UserCode.Invoke(predicate, item, "the predicate"));
				if (cancelEarly && IsDetermined)
				{
					CompleteEarly();
					_collectionContext.Set(materialized);
					return Task.FromResult<ConstraintResult>(this);
				}

				if (cancellationToken.IsCanceledBeforeTheEndOf(materialized))
				{
					Outcome = Outcome.Undecided;
					_collectionContext.Set(materialized, true);
					return Task.FromResult<ConstraintResult>(this);
				}
			}

			Complete();
			_collectionContext.Set(materialized);
			return Task.FromResult<ConstraintResult>(this);
		}
	}

	private sealed class AsyncCollectionConstraint<TItem>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		EnumerableQuantifier quantifier,
		Func<ExpectationGrammars, string> expectationText,
		Func<TItem, ValueTask<bool>> predicate,
		string verb,
		Func<object?, bool>? useComparerOf = null)
		: QuantifiedCollectionConstraint<IEnumerable<TItem>?, TItem>(expectationBuilder, it, grammars, quantifier,
				expectationText, verb),
			IAsyncContextConstraint<IEnumerable<TItem>?>
	{
		private CollectionContext _collectionContext;

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
		{
			_collectionContext.AppendTo(contexts);
			base.AppendContexts(contexts);
		}

		public async Task<ConstraintResult> IsMetBy(
			IEnumerable<TItem>? actual,
			IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_collectionContext = default;
			Actual = actual;
			if (actual is null)
			{
				return this;
			}

			useComparerOf?.Invoke(actual);
			IEnumerable<TItem> materialized = context.UseMaterializedEnumerable<TItem>(actual);
			bool cancelEarly = actual is not ICollection<TItem>;
			foreach (TItem item in materialized)
			{
				Record(item, await predicate(item));
				if (cancelEarly && IsDetermined)
				{
					CompleteEarly();
					_collectionContext.Set(materialized);
					return this;
				}

				if (cancellationToken.IsCanceledBeforeTheEndOf(materialized))
				{
					Outcome = Outcome.Undecided;
					_collectionContext.Set(materialized, true);
					return this;
				}
			}

			Complete();
			_collectionContext.Set(materialized);
			return this;
		}
	}

	/// <remarks>
	///     The items of a non-generic collection are formatted as the type of its first item that is not
	///     <see langword="null" />.
	/// </remarks>
	private sealed class CollectionForEnumerableConstraint<TEnumerable>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		EnumerableQuantifier quantifier,
		Func<ExpectationGrammars, string> expectationText,
		Func<object?, bool> predicate,
		string verb)
		: QuantifiedCollectionConstraint<TEnumerable, object?>(expectationBuilder, it, grammars, quantifier,
				expectationText, verb),
			IAsyncContextConstraint<TEnumerable>
		where TEnumerable : IEnumerable?
	{
		private CollectionContext _collectionContext;
		private Type? _itemType;

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
		{
			_collectionContext.AppendTo(contexts);
			base.AppendContexts(contexts);
		}

		protected override Type ItemType => _itemType ?? typeof(object);

		public Task<ConstraintResult> IsMetBy(
			TEnumerable actual,
			IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_collectionContext = default;
			Actual = actual;
			if (actual.IsDefaultImmutableArray())
			{
				return Task.FromResult(this.AsNullSubject(It));
			}

			if (actual is null)
			{
				return Task.FromResult<ConstraintResult>(this);
			}

			IEnumerable materialized = context.UseMaterializedEnumerable(actual);
			bool cancelEarly = actual is not ICollection;
			foreach (object? item in materialized)
			{
				_itemType ??= item?.GetType();
				Record(item, UserCode.Invoke(predicate, item, "the predicate"));
				if (cancelEarly && IsDetermined)
				{
					CompleteEarly();
					_collectionContext.Set(materialized);
					return Task.FromResult<ConstraintResult>(this);
				}

				if (cancellationToken.IsCanceledBeforeTheEndOf(materialized))
				{
					Outcome = Outcome.Undecided;
					_collectionContext.Set(materialized, true);
					return Task.FromResult<ConstraintResult>(this);
				}
			}

			Complete();
			_collectionContext.Set(materialized);
			return Task.FromResult<ConstraintResult>(this);
		}
	}

	/// <remarks>
	///     The items are formatted as in <see cref="CollectionForEnumerableConstraint{TEnumerable}" />.
	/// </remarks>
	private sealed class AsyncCollectionForEnumerableConstraint<TEnumerable>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		EnumerableQuantifier quantifier,
		Func<ExpectationGrammars, string> expectationText,
		Func<object?, ValueTask<bool>> predicate,
		string verb,
		Func<object?, bool>? useComparerOf = null)
		: QuantifiedCollectionConstraint<TEnumerable, object?>(expectationBuilder, it, grammars, quantifier,
				expectationText, verb),
			IAsyncContextConstraint<TEnumerable>
		where TEnumerable : IEnumerable?
	{
		private CollectionContext _collectionContext;
		private Type? _itemType;

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
		{
			_collectionContext.AppendTo(contexts);
			base.AppendContexts(contexts);
		}

		protected override Type ItemType => _itemType ?? typeof(object);

		public async Task<ConstraintResult> IsMetBy(
			TEnumerable actual,
			IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_collectionContext = default;
			Actual = actual;
			if (actual.IsDefaultImmutableArray())
			{
				return this.AsNullSubject(It);
			}

			if (actual is null)
			{
				return this;
			}

			useComparerOf?.Invoke(actual);
			IEnumerable materialized = context.UseMaterializedEnumerable(actual);
			bool cancelEarly = actual is not ICollection;
			foreach (object? item in materialized)
			{
				_itemType ??= item?.GetType();
				Record(item, await predicate(item));
				if (cancelEarly && IsDetermined)
				{
					CompleteEarly();
					_collectionContext.Set(materialized);
					return this;
				}

				if (cancellationToken.IsCanceledBeforeTheEndOf(materialized))
				{
					Outcome = Outcome.Undecided;
					_collectionContext.Set(materialized, true);
					return this;
				}
			}

			Complete();
			_collectionContext.Set(materialized);
			return this;
		}
	}

	private sealed class SyncCollectionCountConstraint<TItem>
		: ConstraintResult.WithNotNullValue<IEnumerable<TItem>?>,
			IAsyncContextConstraint<IEnumerable<TItem>?>
	{
		private CollectionContext _collectionContext;
		private readonly EnumerableQuantifier _quantifier;
		private int _matchingCount;
		private int _notMatchingCount;
		private int? _totalCount;

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> _collectionContext.AppendTo(contexts);

		public SyncCollectionCountConstraint(
			string it,
			ExpectationGrammars grammars,
			EnumerableQuantifier quantifier)
			: base(it, grammars)
		{
			_quantifier = quantifier;
		}

		/// <inheritdoc />
		public override Outcome Outcome
		{
			get => _quantifier.FailsBothWays
				? Outcome.Failure
				: base.Outcome;
			protected set => base.Outcome = value;
		}

		public Task<ConstraintResult> IsMetBy(
			IEnumerable<TItem>? actual,
			IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_collectionContext = default;
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
				_collectionContext.Set(actual);
				return Task.FromResult<ConstraintResult>(this);
			}

			IEnumerable<TItem> materialized =
				context.UseMaterializedEnumerable<TItem>(actual);

			foreach (TItem _ in materialized)
			{
				_matchingCount++;

				if (_quantifier.IsDeterminable(_matchingCount, _notMatchingCount))
				{
					Outcome = _quantifier.GetOutcome(_matchingCount, _notMatchingCount, _totalCount);
					_collectionContext.Set(materialized);
					return Task.FromResult<ConstraintResult>(this);
				}

				if (cancellationToken.IsCanceledBeforeTheEndOf(materialized))
				{
					Outcome = Outcome.Undecided;
					_collectionContext.Set(materialized, true);
					return Task.FromResult<ConstraintResult>(this);
				}
			}

			_totalCount = _matchingCount + _notMatchingCount;
			_collectionContext.Set(materialized);
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
			=> _quantifier.AppendResult(stringBuilder, Grammars, It, _matchingCount, _notMatchingCount, _totalCount);

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not have ", "do not have "));
			stringBuilder.Append(_quantifier);
			stringBuilder.Append(' ');
			stringBuilder.Append(_quantifier.GetItemString());
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> _quantifier.AppendResult(stringBuilder, Grammars, It, _matchingCount, _notMatchingCount,
				_totalCount);
	}

	private sealed class SyncCollectionCountForEnumerableConstraint<TEnumerable>
		: ConstraintResult.WithNotNullValue<TEnumerable>,
			IAsyncContextConstraint<TEnumerable>
		where TEnumerable : IEnumerable?
	{
		private CollectionContext _collectionContext;
		private readonly EnumerableQuantifier _quantifier;
		private int _matchingCount;
		private int _notMatchingCount;
		private int? _totalCount;

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> _collectionContext.AppendTo(contexts);

		public SyncCollectionCountForEnumerableConstraint(
			string it,
			ExpectationGrammars grammars,
			EnumerableQuantifier quantifier)
			: base(it, grammars)
		{
			_quantifier = quantifier;
		}

		/// <inheritdoc />
		public override Outcome Outcome
		{
			get => _quantifier.FailsBothWays
				? Outcome.Failure
				: base.Outcome;
			protected set => base.Outcome = value;
		}

		public Task<ConstraintResult> IsMetBy(
			TEnumerable? actual,
			IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_collectionContext = default;
			Actual = actual;
			if (actual.IsDefaultImmutableArray())
			{
				return Task.FromResult(this.AsNullSubject(It));
			}

			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return Task.FromResult<ConstraintResult>(this);
			}

			_matchingCount = 0;
			_notMatchingCount = 0;

			if (actual is ICollection collection)
			{
				_matchingCount = collection.Count;
				_totalCount = _matchingCount;
				Outcome = _quantifier.GetOutcome(_matchingCount, _notMatchingCount, _totalCount);
				_collectionContext.Set(actual);
				return Task.FromResult<ConstraintResult>(this);
			}

			IEnumerable materialized = context.UseMaterializedEnumerable(actual);

			foreach (object? _ in materialized)
			{
				_matchingCount++;

				if (_quantifier.IsDeterminable(_matchingCount, _notMatchingCount))
				{
					Outcome = _quantifier.GetOutcome(_matchingCount, _notMatchingCount, _totalCount);
					_collectionContext.Set(materialized);
					return Task.FromResult<ConstraintResult>(this);
				}

				if (cancellationToken.IsCanceledBeforeTheEndOf(materialized))
				{
					Outcome = Outcome.Undecided;
					_collectionContext.Set(materialized, true);
					return Task.FromResult<ConstraintResult>(this);
				}
			}

			_totalCount = _matchingCount + _notMatchingCount;
			_collectionContext.Set(materialized);
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
			=> _quantifier.AppendResult(stringBuilder, Grammars, It, _matchingCount, _notMatchingCount, _totalCount);

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not have ", "do not have "));
			stringBuilder.Append(_quantifier);
			stringBuilder.Append(' ');
			stringBuilder.Append(_quantifier.GetItemString());
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> _quantifier.AppendResult(stringBuilder, Grammars, It, _matchingCount, _notMatchingCount,
				_totalCount);
	}

	/// <summary>
	///     Returns the comparer of a sorted set <paramref name="subject" /> that orders its items, when neither a member
	///     nor a comparer in the <paramref name="options" /> is specified, or <see langword="null" /> otherwise.
	/// </summary>
	private static IComparer<TMember>? GetSubjectOrder<TMember>(object subject, string memberExpression,
		CollectionOrderOptions<TMember> options)
		=> memberExpression.Length == 0 && !options.HasComparer
			? CollectionComparerHelpers.GetSubjectOrder(subject, options.GetComparer())
			: null;

	private static bool IsOutOfOrder(aweXpect.SortOrder sortOrder, int comparisonResult)
		=> (comparisonResult > 0 && sortOrder == aweXpect.SortOrder.Ascending) ||
		   (comparisonResult < 0 && sortOrder == aweXpect.SortOrder.Descending);

	/// <remarks>
	///     Without a member, i.e. with an empty <paramref name="memberExpression" />, a subject that is a sorted set with a
	///     comparer other than the default order is ordered by that comparer, unless a comparer is specified in the
	///     <paramref name="options" />, and the expectation names it.
	/// </remarks>
	private sealed class IsInOrderConstraint<TItem, TMember>(
		string it,
		ExpectationGrammars grammars,
		Func<TItem, TMember> memberAccessor,
		SortOrder sortOrder,
		CollectionOrderOptions<TMember> options,
		string memberExpression,
		Func<Func<TMember, string?>?>? createIncompatibilityCheck = null)
		: OrderingConstraint<IEnumerable<TItem>?>(it, grammars, false),
			IAsyncContextConstraint<IEnumerable<TItem>?>
	{
		private CollectionContext _collectionContext;
		private string? _failureText;
		private IComparer<TMember>? _subjectOrder;

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> _collectionContext.AppendTo(contexts);

		public Task<ConstraintResult> IsMetBy(IEnumerable<TItem>? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_collectionContext = default;
			Actual = actual;
			_failureText = null;
			IsIncomparable = false;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return Task.FromResult<ConstraintResult>(this);
			}

			IEnumerable<TItem> materialized = context
				.UseMaterializedEnumerable<TItem>(actual);
			_collectionContext.Set(materialized);

			TMember previous = default!;
			int index = 0;
			_subjectOrder = GetSubjectOrder(actual, memberExpression, options);
			IComparer<TMember> comparer = _subjectOrder ?? options.GetComparer();
			Func<TMember, string?>? incompatibilityCheck =
				_subjectOrder is null ? createIncompatibilityCheck?.Invoke() : null;
			foreach (TItem item in materialized)
			{
				if (cancellationToken.IsCanceledBeforeTheEndOf(materialized))
				{
					Outcome = Outcome.Undecided;
					return Task.FromResult<ConstraintResult>(this);
				}

				TMember current = UserCode.Invoke(memberAccessor, item, "the member selector");
				if (IsIncompatible(incompatibilityCheck, current))
				{
					return Task.FromResult<ConstraintResult>(this);
				}

				if (index++ == 0)
				{
					previous = current;
					continue;
				}

				if (IsOutOfOrder(sortOrder, UserCode.Invoke(
					    static values => values.Comparer.Compare(values.Previous, values.Current),
					    (Comparer: comparer, Previous: previous, Current: current), "the comparer")))
				{
					_failureText =
						$"{It} had {Formatter.Format(previous)} before {Formatter.Format(current)}, which is not in {sortOrder.ToString().ToLower()} order";
					Outcome = Outcome.Failure;
					return Task.FromResult<ConstraintResult>(this);
				}

				previous = current;
			}

			Outcome = Outcome.Success;
			return Task.FromResult<ConstraintResult>(this);
		}

		private bool IsIncompatible(Func<TMember, string?>? incompatibilityCheck, TMember current)
		{
			if (incompatibilityCheck?.Invoke(current) is not { } incompatibility)
			{
				return false;
			}

			// The order of incompatible items cannot be verified, so the negated check fails as well.
			_failureText = $"{It} {incompatibility}";
			IsIncomparable = true;
			return true;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is in ", "are in ")).Append(sortOrder.ToString().ToLower())
				.Append(SortOrder);
			stringBuilder.Append(memberExpression).Append(options);
			AppendSubjectOrder(stringBuilder);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(_failureText);

		private void AppendSubjectOrder(StringBuilder stringBuilder)
		{
			if (_subjectOrder is not null)
			{
				stringBuilder.Append(CollectionComparerHelpers.DescribeSubjectComparer(_subjectOrder));
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not in ", "are not in ")).Append(sortOrder.ToString().ToLower())
				.Append(SortOrder);
			stringBuilder.Append(memberExpression).Append(options);
			AppendSubjectOrder(stringBuilder);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (IsIncomparable)
			{
				stringBuilder.Append(_failureText);
			}
			else
			{
				stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was", " were"));
			}
		}
	}

	/// <remarks>
	///     Without a member, i.e. with an empty <paramref name="memberExpression" />, a subject that is a sorted set of
	///     <typeparamref name="TMember" /> with a comparer other than the default order is ordered by that comparer, unless
	///     a comparer is specified in the <paramref name="options" />, and the expectation names it.
	/// </remarks>
	private sealed class IsInOrderForEnumerableConstraint<TEnumerable, TItem, TMember>(
		string it,
		ExpectationGrammars grammars,
		Func<TItem, TMember> memberAccessor,
		SortOrder sortOrder,
		CollectionOrderOptions<TMember> options,
		string memberExpression,
		Func<Func<TMember, string?>?>? createIncompatibilityCheck = null)
		: OrderingConstraint<TEnumerable>(it, grammars, false),
			IAsyncContextConstraint<TEnumerable>
		where TEnumerable : IEnumerable?
	{
		private CollectionContext _collectionContext;
		private string? _failureText;
		private IComparer<TMember>? _subjectOrder;

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> _collectionContext.AppendTo(contexts);

		public Task<ConstraintResult> IsMetBy(TEnumerable actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_collectionContext = default;
			Actual = actual;
			_failureText = null;
			IsIncomparable = false;
			if (actual.IsDefaultImmutableArray())
			{
				return Task.FromResult(this.AsNullSubject(It));
			}

			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return Task.FromResult<ConstraintResult>(this);
			}

			IEnumerable materialized = context.UseMaterializedEnumerable(actual);
			_collectionContext.Set(materialized);

			TMember previous = default!;
			int index = 0;
			_subjectOrder = GetSubjectOrder(actual, memberExpression, options);
			IComparer<TMember> comparer = _subjectOrder ?? options.GetComparer();
			Func<TMember, string?>? incompatibilityCheck =
				_subjectOrder is null ? createIncompatibilityCheck?.Invoke() : null;
			foreach (object? item in materialized)
			{
				if (cancellationToken.IsCanceledBeforeTheEndOf(materialized))
				{
					Outcome = Outcome.Undecided;
					return Task.FromResult<ConstraintResult>(this);
				}

				if (!TryCastItem(item, out TItem typedItem))
				{
					continue;
				}

				TMember current = UserCode.Invoke(memberAccessor, typedItem, "the member selector");
				if (IsIncompatible(incompatibilityCheck, current))
				{
					return Task.FromResult<ConstraintResult>(this);
				}

				if (index++ == 0)
				{
					previous = current;
					continue;
				}

				if (IsOutOfOrder(sortOrder, UserCode.Invoke(
					    static values => values.Comparer.Compare(values.Previous, values.Current),
					    (Comparer: comparer, Previous: previous, Current: current), "the comparer")))
				{
					_failureText =
						$"{It} had {Formatter.Format(previous)} before {Formatter.Format(current)}, which is not in {sortOrder.ToString().ToLower()} order";
					Outcome = Outcome.Failure;
					return Task.FromResult<ConstraintResult>(this);
				}

				previous = current;
			}

			Outcome = Outcome.Success;
			return Task.FromResult<ConstraintResult>(this);
		}

		private bool IsIncompatible(Func<TMember, string?>? incompatibilityCheck, TMember current)
		{
			if (incompatibilityCheck?.Invoke(current) is not { } incompatibility)
			{
				return false;
			}

			// The order of incompatible items cannot be verified, so the negated check fails as well.
			_failureText = $"{It} {incompatibility}";
			IsIncomparable = true;
			return true;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is in ", "are in ")).Append(sortOrder.ToString().ToLower())
				.Append(SortOrder);
			stringBuilder.Append(memberExpression).Append(options);
			AppendSubjectOrder(stringBuilder);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(_failureText);

		private void AppendSubjectOrder(StringBuilder stringBuilder)
		{
			if (_subjectOrder is not null)
			{
				stringBuilder.Append(CollectionComparerHelpers.DescribeSubjectComparer(_subjectOrder));
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not in ", "are not in ")).Append(sortOrder.ToString().ToLower())
				.Append(SortOrder);
			stringBuilder.Append(memberExpression).Append(options);
			AppendSubjectOrder(stringBuilder);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (IsIncomparable)
			{
				stringBuilder.Append(_failureText);
			}
			else
			{
				stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was", " were"));
			}
		}
	}
}
