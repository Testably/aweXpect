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

namespace aweXpect;

/// <summary>
///     The verification of the items by a collection matcher and the result texts of <c>IsEqualTo</c>, shared by its
///     variants and by the synchronous and the asynchronous collections.
/// </summary>
internal abstract class CollectionMatchConstraintBase<TValue>(
	string it,
	ExpectationGrammars grammars,
	bool isExpectedNull,
	bool failsForNullSubject,
	CollectionMatchOptions matchOptions)
	: ConstraintResult.WithEqualToValue<TValue>(it, grammars, isExpectedNull)
{
	private const string ExpectedCollectionWasNull = "the expected collection was <null>";

	private CollectionContext _collectionContext;
	private string? _failure;

	/// <summary>
	///     Whether the expected collection is <see langword="null" />.
	/// </summary>
	protected bool IsExpectedNull { get; } = isExpectedNull;

	/// <summary>
	///     The options for the order and the number of the items.
	/// </summary>
	protected CollectionMatchOptions MatchOptions => matchOptions;

	public override Outcome Outcome
	{
		get => failsForNullSubject && Actual is null ? Outcome.Failure : base.Outcome;
		protected set => base.Outcome = value;
	}

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
	{
		_collectionContext.AppendTo(contexts);
		AppendExpectedContexts(contexts);
	}

	/// <summary>
	///     Adds the contexts of the expected items.
	/// </summary>
	protected abstract void AppendExpectedContexts(ResultContextCollector contexts);

	/// <summary>
	///     Whether an item expectation was canceled, so that it did not decide whether an item matches.
	/// </summary>
	protected virtual bool IsAnItemExpectationCanceled(CancellationToken cancellationToken) => false;

	/// <summary>
	///     Starts a new evaluation.
	/// </summary>
	protected virtual void Start()
	{
		_collectionContext = default;
		_failure = null;
	}

	/// <summary>
	///     Fails, because the expected collection is <see langword="null" />, and shows the <paramref name="items" />.
	/// </summary>
	protected ConstraintResult FailForNullExpected<TItem>(CollectionItems<TItem> items)
	{
		Outcome = Outcome.Failure;
		items.SetContext(ref _collectionContext);
		return this;
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Fails, because the expected collection is <see langword="null" />, and shows the <paramref name="items" />.
	/// </summary>
	protected ConstraintResult FailForNullExpected<TItem>(IAsyncEnumerable<TItem> items)
	{
		Outcome = Outcome.Failure;
		_collectionContext.Set(items as IMaterializedAsyncEnumerable<TItem>);
		return this;
	}
#endif

	/// <summary>
	///     Verifies the <paramref name="materialized" /> items by the <paramref name="matcher" />.
	/// </summary>
	protected async Task<ConstraintResult> VerifyItems<TItem, TMatch>(CollectionItems<TItem> materialized,
		ICollectionMatcher<TItem, TMatch> matcher, IOptionsEquality<TMatch> itemOptions,
		CancellationToken cancellationToken)
		where TItem : TMatch
	{
		int maximumNumber = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();
		foreach (TItem item in materialized.Items)
		{
			if (materialized.IsCanceledBeforeTheEnd(cancellationToken))
			{
				Outcome = Outcome.Undecided;
				materialized.SetContext(ref _collectionContext, true);
				return this;
			}

			var (result, failure) = await matcher.Verify(It, item, itemOptions, maximumNumber);
			if (Fails(result, failure, cancellationToken))
			{
				materialized.SetContext(ref _collectionContext);
				return this;
			}

			if (matcher.IsDetermined)
			{
				break;
			}
		}

		await Complete(matcher, itemOptions, maximumNumber, cancellationToken);
		materialized.SetContext(ref _collectionContext);
		return this;
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies the <paramref name="materialized" /> items by the <paramref name="matcher" />.
	/// </summary>
	protected async Task<ConstraintResult> VerifyItems<TItem, TMatch>(IAsyncEnumerable<TItem> materialized,
		ICollectionMatcher<TItem, TMatch> matcher, IOptionsEquality<TMatch> itemOptions,
		CancellationToken cancellationToken)
		where TItem : TMatch
	{
		int maximumNumber = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();
		_collectionContext.Set(materialized as IMaterializedAsyncEnumerable<TItem>);
		bool isDetermined = false;
		await foreach (TItem item in materialized.UntilCancelled(cancellationToken))
		{
			var (result, failure) = await matcher.Verify(It, item, itemOptions, maximumNumber);
			if (Fails(result, failure, cancellationToken))
			{
				return this;
			}

			if (matcher.IsDetermined)
			{
				isDetermined = true;
				break;
			}
		}

		if (!isDetermined && cancellationToken.IsCanceledBeforeTheEndOf(materialized))
		{
			Outcome = Outcome.Undecided;
			return this;
		}

		await Complete(matcher, itemOptions, maximumNumber, cancellationToken);
		return this;
	}
#endif

	private bool Fails(bool result, string? failure, CancellationToken cancellationToken)
	{
		// A canceled item expectation does not match, which must not be reported as a mismatch.
		if (!result || IsAnItemExpectationCanceled(cancellationToken))
		{
			return false;
		}

		Fail(failure);
		return true;
	}

	private async Task Complete<TItem, TMatch>(ICollectionMatcher<TItem, TMatch> matcher,
		IOptionsEquality<TMatch> itemOptions, int maximumNumber, CancellationToken cancellationToken)
		where TItem : TMatch
	{
		var (completedResult, completedFailure) = await matcher.VerifyComplete(It, itemOptions, maximumNumber);
		// The final check can evaluate item expectations as well, e.g. to reassign items in any order.
		if (IsAnItemExpectationCanceled(cancellationToken))
		{
			Outcome = Outcome.Undecided;
		}
		else if (completedResult)
		{
			Fail(completedFailure);
		}
		else
		{
			Outcome = Outcome.Success;
		}
	}

	private void Fail(string? failure)
	{
		_failure = failure ??
		           $"{It} had more than {2L * Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get()} deviations";
		Outcome = Outcome.Failure;
	}

	protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
	{
		if (IsExpectedNull)
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
		if (IsExpectedNull)
		{
			stringBuilder.Append(ExpectedCollectionWasNull);
		}
		else
		{
			stringBuilder.Append(It).Append(matchOptions.GetNegatedResultVerb(It, Grammars));
		}
	}

	/// <summary>
	///     Compares the items by <see cref="object.Equals(object, object)" />, as the expected entries match the items
	///     themselves.
	/// </summary>
	protected sealed class NoOptions<TMatch> : IOptionsEquality<TMatch>
	{
		public static NoOptions<TMatch> Instance { get; } = new();

		public ValueTask<bool> AreConsideredEqual<TExpected>(TMatch actual, TExpected expected)
			=> new(Equals(actual, expected));
	}
}

/// <summary>
///     The expected items and the texts of <c>IsEqualTo</c> with expected items.
/// </summary>
internal abstract class IsEqualToConstraintBase<TValue, TItem, TMatch>(
	string it,
	ExpectationGrammars grammars,
	string? expectedExpression,
	IEnumerable<TItem>? expected,
	IOptionsEquality<TMatch> options,
	CollectionMatchOptions matchOptions,
	bool failsForNullSubject)
	: CollectionMatchConstraintBase<TValue>(it, grammars, expected is null, failsForNullSubject, matchOptions)
	where TItem : TMatch
{
	private ICollection<TItem>? _expectedItems;

	/// <summary>
	///     The comparer of a set subject which compares the items, or <see langword="null" />.
	/// </summary>
	protected SubjectComparer<TItem>? SubjectComparer { get; set; }

	/// <inheritdoc />
	protected override void Start()
	{
		base.Start();
		_expectedItems = null;
		SubjectComparer = null;
	}

	/// <summary>
	///     Materializes the expected items, or returns <see langword="null" /> when the expected collection is
	///     <see langword="null" />.
	/// </summary>
	protected ICollection<TItem>? GetExpectedItems()
	{
		if (expected is null)
		{
			return null;
		}

		_expectedItems = expected as ICollection<TItem> ?? expected.ToArray();
		return _expectedItems;
	}

	/// <summary>
	///     The options to compare the items with.
	/// </summary>
	protected IOptionsEquality<TMatch> GetItemOptions()
		=> options is ObjectEqualityOptions<TMatch> objectOptions ? objectOptions.ForEvaluation() : options;

	/// <inheritdoc />
	protected override void AppendExpectedContexts(ResultContextCollector contexts)
	{
		if (expected is not null && _expectedItems is not null)
		{
			contexts.AddExpectedItemsContext(expected, _expectedItems);
		}

		contexts.AddOptionsContexts(options);
	}

	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		string expectedText = expectedExpression ?? Formatter.Format(expected, FormattingOptions.SingleLine);
		// The options qualify the expected items, not their order.
		stringBuilder.Append(MatchOptions.GetExpectation(expectedText + options + SubjectComparer, Grammars));
	}
}

/// <remarks>
///     When <paramref name="usesDefaultEquality" /> tells that the comparison was not changed, a subject that is a set
///     of <typeparamref name="TItem" /> with a custom comparer compares its items with that comparer, as it does for a
///     single item in <c>Contains</c>, and the expectation names it.
/// </remarks>
internal sealed class IsEqualToConstraint<TEnumerable, TItem, TMatch>(
	string it,
	ExpectationGrammars grammars,
	string? expectedExpression,
	IEnumerable<TItem>? expected,
	IOptionsEquality<TMatch> options,
	CollectionMatchOptions matchOptions,
	bool failsForNullSubject = false,
	Func<bool>? usesDefaultEquality = null)
	: IsEqualToConstraintBase<TEnumerable?, TItem, TMatch>(it, grammars, expectedExpression, expected, options,
			matchOptions, failsForNullSubject),
		IAsyncContextConstraint<TEnumerable?>
	where TEnumerable : IEnumerable?
	where TItem : TMatch
{
	public Task<ConstraintResult> IsMetBy(TEnumerable? actual, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		Start();
		Actual = actual;
		if (actual.IsDefaultImmutableArray())
		{
			return Task.FromResult(this.AsNullSubject(It));
		}

		if (actual is null)
		{
			Outcome = IsExpectedNull ? Outcome.Success : Outcome.Failure;
			return Task.FromResult<ConstraintResult>(this);
		}

		bool isTyped = CollectionItems<TItem>.IsTyped<TEnumerable>();
		if (GetExpectedItems() is not { } expectedItems)
		{
			return Task.FromResult(isTyped
				? FailForNullExpected(CollectionItems<TItem>.Materialize(actual, context))
				: FailForNullExpected(CollectionItems<object?>.Materialize(actual, context)));
		}

		SubjectEqualityOptions<TItem, TMatch> subjectOptions =
			new(GetItemOptions(), usesDefaultEquality ?? (() => false));
		SubjectComparer = subjectOptions.UseComparerOf(actual) ? subjectOptions.Comparer : null;
		return isTyped
			? VerifyItems(CollectionItems<TItem>.Materialize(actual, context),
				MatchOptions.GetCollectionMatcher<TItem, TMatch>(expectedItems), subjectOptions, cancellationToken)
			: VerifyItems(CollectionItems<object?>.Materialize(actual, context),
				MatchOptions.GetCollectionMatcher<object?, object?>(expectedItems.Cast<object?>()),
				new UntypedOptions(subjectOptions), cancellationToken);
	}

	/// <summary>
	///     A non-generic subject can contain items of any type, so an item that is not a
	///     <typeparamref name="TMatch" /> never equals an expected item.
	/// </summary>
	private sealed class UntypedOptions(IOptionsEquality<TMatch> options) : IOptionsEquality<object?>
	{
		public async ValueTask<bool> AreConsideredEqual<TExpected>(object? actual, TExpected expected)
			=> CollectionItems<object?>.TryCast(actual, out TMatch typedActual)
			   && await options.AreConsideredEqual(typedActual, (TItem)(object?)expected!);
	}
}

/// <summary>
///     The expectations on the items and the texts of <c>IsEqualTo</c> with item expectations.
/// </summary>
internal abstract class IsEqualToFromExpectationsConstraintBase<TValue, TItem>(
	string it,
	ExpectationGrammars grammars,
	string? expectedExpression,
	IEnumerable<Action<IThatSubject<TItem?>>>? expected,
	CollectionMatchOptions matchOptions,
	bool failsForNullSubject)
	: CollectionMatchConstraintBase<TValue>(it, grammars, expected is null, failsForNullSubject, matchOptions)
{
	private CollectionMatchOptions.ExpectationItem<TItem>[] _expectations = [];
	private bool _showsExpected;

	/// <inheritdoc />
	protected override void Start()
	{
		base.Start();
		_showsExpected = false;
	}

	/// <summary>
	///     Prepares the expectations on the items, or returns <see langword="null" /> when the expected collection is
	///     <see langword="null" />.
	/// </summary>
	protected async Task<CollectionMatchOptions.ExpectationItem<TItem>[]?> PrepareExpectations(
		IEvaluationContext context, CancellationToken cancellationToken)
	{
		if (expected is null)
		{
			return null;
		}

		_expectations = expected.Select(expectation
				=> new CollectionMatchOptions.ExpectationItem<TItem>(expectation,
					Grammars & ~ExpectationGrammars.Negated,
					context,
					cancellationToken))
			.ToArray();
		foreach (CollectionMatchOptions.ExpectationItem<TItem> expectation in _expectations)
		{
			await expectation.PrepareExpectation();
		}

		_showsExpected = true;
		return _expectations;
	}

	/// <inheritdoc />
	protected override bool IsAnItemExpectationCanceled(CancellationToken cancellationToken)
		=> cancellationToken.IsCancellationRequested && _expectations.Any(expectation => expectation.IsUndecided);

	/// <inheritdoc />
	protected override void AppendExpectedContexts(ResultContextCollector contexts)
	{
		if (_showsExpected)
		{
			CollectionMatchOptions.ExpectationItem<TItem>[] expectations = _expectations;
			contexts.Add(new ResultContext.SyncCallback("Expected",
				() => Formatter.Format(expectations, typeof(TItem).GetFormattingOption(expectations.Length)), -2));
		}
	}

	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append(MatchOptions.GetExpectation(
			expectedExpression ?? Formatter.Format(_expectations, FormattingOptions.SingleLine), Grammars));
}

internal sealed class IsEqualToFromExpectationsConstraint<TEnumerable, TItem, TMatch>(
	string it,
	ExpectationGrammars grammars,
	string? expectedExpression,
	IEnumerable<Action<IThatSubject<TItem?>>>? expected,
	CollectionMatchOptions matchOptions,
	bool failsForNullSubject = false)
	: IsEqualToFromExpectationsConstraintBase<TEnumerable?, TItem>(it, grammars, expectedExpression, expected,
			matchOptions, failsForNullSubject),
		IAsyncContextConstraint<TEnumerable?>
	where TEnumerable : IEnumerable<TItem>?
	where TItem : TMatch
{
	public async Task<ConstraintResult> IsMetBy(TEnumerable? actual, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		Start();
		Actual = actual;
		if (actual.IsDefaultImmutableArray())
		{
			return this.AsNullSubject(It);
		}

		if (actual is null)
		{
			Outcome = IsExpectedNull ? Outcome.Success : Outcome.Failure;
			return this;
		}

		CollectionItems<TItem> materialized = CollectionItems<TItem>.Materialize(actual, context);
		if (await PrepareExpectations(context, cancellationToken) is not { } expectations)
		{
			return FailForNullExpected(materialized);
		}

		return await VerifyItems(materialized, MatchOptions.GetCollectionMatcher<TItem, TMatch>(expectations),
			NoOptions<TMatch>.Instance, cancellationToken);
	}
}

/// <summary>
///     The expected predicates and the texts of <c>IsEqualTo</c> with predicates for the items.
/// </summary>
internal abstract class IsEqualToFromPredicateConstraintBase<TValue, TItem>(
	string it,
	ExpectationGrammars grammars,
	string? expectedExpression,
	IEnumerable<Expression<Func<TItem, bool>>>? expected,
	CollectionMatchOptions matchOptions,
	bool failsForNullSubject)
	: CollectionMatchConstraintBase<TValue>(it, grammars, expected is null, failsForNullSubject, matchOptions)
{
	private ICollection<Expression<Func<TItem, bool>>>? _expectedItems;

	/// <inheritdoc />
	protected override void Start()
	{
		base.Start();
		_expectedItems = null;
	}

	/// <summary>
	///     Materializes the expected predicates, or returns <see langword="null" /> when the expected collection is
	///     <see langword="null" />.
	/// </summary>
	protected ICollection<Expression<Func<TItem, bool>>>? GetExpectedItems()
	{
		if (expected is null)
		{
			return null;
		}

		_expectedItems = expected as ICollection<Expression<Func<TItem, bool>>> ?? expected.ToArray();
		return _expectedItems;
	}

	/// <inheritdoc />
	protected override void AppendExpectedContexts(ResultContextCollector contexts)
	{
		if (_expectedItems is { } expectedItems)
		{
			contexts.Add(new ResultContext.SyncCallback("Expected",
				() => Formatter.Format(expectedItems, FormattingOptions.MultipleLines), -2));
		}
	}

	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append(MatchOptions.GetExpectation(
			expectedExpression ?? Formatter.Format(expected, FormattingOptions.SingleLine), Grammars));
}

internal sealed class IsEqualToFromPredicateConstraint<TEnumerable, TItem, TMatch>(
	string it,
	ExpectationGrammars grammars,
	string? expectedExpression,
	IEnumerable<Expression<Func<TItem, bool>>>? expected,
	CollectionMatchOptions matchOptions,
	bool failsForNullSubject = false)
	: IsEqualToFromPredicateConstraintBase<TEnumerable?, TItem>(it, grammars, expectedExpression, expected,
			matchOptions, failsForNullSubject),
		IAsyncContextConstraint<TEnumerable?>
	where TEnumerable : IEnumerable<TItem>?
	where TItem : TMatch
{
	public Task<ConstraintResult> IsMetBy(TEnumerable? actual, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		Start();
		Actual = actual;
		if (actual.IsDefaultImmutableArray())
		{
			return Task.FromResult(this.AsNullSubject(It));
		}

		if (actual is null)
		{
			Outcome = IsExpectedNull ? Outcome.Success : Outcome.Failure;
			return Task.FromResult<ConstraintResult>(this);
		}

		CollectionItems<TItem> materialized = CollectionItems<TItem>.Materialize(actual, context);
		if (GetExpectedItems() is not { } expectedItems)
		{
			return Task.FromResult(FailForNullExpected(materialized));
		}

		return VerifyItems(materialized, MatchOptions.GetCollectionMatcher<TItem, TMatch>(expectedItems),
			NoOptions<TMatch>.Instance, cancellationToken);
	}
}

#if NET8_0_OR_GREATER
internal sealed class AsyncIsEqualToConstraint<TItem, TMatch>(
	string it,
	ExpectationGrammars grammars,
	string expectedExpression,
	IEnumerable<TItem>? expected,
	IOptionsEquality<TMatch> options,
	CollectionMatchOptions matchOptions,
	bool failsForNullSubject = false)
	: IsEqualToConstraintBase<IAsyncEnumerable<TItem>?, TItem, TMatch>(it, grammars, expectedExpression, expected,
			options, matchOptions, failsForNullSubject),
		IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
	where TItem : TMatch
{
	public Task<ConstraintResult> IsMetBy(IAsyncEnumerable<TItem>? actual, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		Start();
		Actual = actual;
		if (actual is null)
		{
			Outcome = IsExpectedNull ? Outcome.Success : Outcome.Failure;
			return Task.FromResult<ConstraintResult>(this);
		}

		IAsyncEnumerable<TItem> materialized =
			context.UseMaterializedAsyncEnumerable<TItem>(actual, cancellationToken);
		if (GetExpectedItems() is not { } expectedItems)
		{
			return Task.FromResult(FailForNullExpected(materialized));
		}

		return VerifyItems(materialized, MatchOptions.GetCollectionMatcher<TItem, TMatch>(expectedItems),
			GetItemOptions(), cancellationToken);
	}
}

internal sealed class AsyncIsEqualToFromExpectationsConstraint<TItem, TMatch>(
	string it,
	ExpectationGrammars grammars,
	string expectedExpression,
	IEnumerable<Action<IThatSubject<TItem?>>>? expected,
	CollectionMatchOptions matchOptions,
	bool failsForNullSubject = false)
	: IsEqualToFromExpectationsConstraintBase<IAsyncEnumerable<TItem>?, TItem>(it, grammars, expectedExpression,
			expected, matchOptions, failsForNullSubject),
		IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
	where TItem : TMatch
{
	public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<TItem>? actual, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		Start();
		Actual = actual;
		if (actual is null)
		{
			Outcome = IsExpectedNull ? Outcome.Success : Outcome.Failure;
			return this;
		}

		IAsyncEnumerable<TItem> materialized =
			context.UseMaterializedAsyncEnumerable<TItem>(actual, cancellationToken);
		if (await PrepareExpectations(context, cancellationToken) is not { } expectations)
		{
			return FailForNullExpected(materialized);
		}

		return await VerifyItems(materialized, MatchOptions.GetCollectionMatcher<TItem, TMatch>(expectations),
			NoOptions<TMatch>.Instance, cancellationToken);
	}
}

internal sealed class AsyncIsEqualToFromPredicateConstraint<TItem, TMatch>(
	string it,
	ExpectationGrammars grammars,
	string expectedExpression,
	IEnumerable<Expression<Func<TItem, bool>>>? expected,
	CollectionMatchOptions matchOptions,
	bool failsForNullSubject = false)
	: IsEqualToFromPredicateConstraintBase<IAsyncEnumerable<TItem>?, TItem>(it, grammars, expectedExpression,
			expected, matchOptions, failsForNullSubject),
		IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
	where TItem : TMatch
{
	public Task<ConstraintResult> IsMetBy(IAsyncEnumerable<TItem>? actual, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		Start();
		Actual = actual;
		if (actual is null)
		{
			Outcome = IsExpectedNull ? Outcome.Success : Outcome.Failure;
			return Task.FromResult<ConstraintResult>(this);
		}

		IAsyncEnumerable<TItem> materialized =
			context.UseMaterializedAsyncEnumerable<TItem>(actual, cancellationToken);
		if (GetExpectedItems() is not { } expectedItems)
		{
			return Task.FromResult(FailForNullExpected(materialized));
		}

		return VerifyItems(materialized, MatchOptions.GetCollectionMatcher<TItem, TMatch>(expectedItems),
			NoOptions<TMatch>.Instance, cancellationToken);
	}
}
#endif
