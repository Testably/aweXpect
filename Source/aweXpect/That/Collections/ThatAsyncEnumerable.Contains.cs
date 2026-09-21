#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
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

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     Verifies that the collection contains the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static ObjectCountResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		Contains<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject,
			TItem expected)
	{
		Quantifier quantifier = new();
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCountResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new AsyncContainConstraint<TItem>(
					expectationBuilder, it, grammars,
					(q, g) => q.IsNever
						? $"{g.Verb("does not contain", "do not contain")} {ContainedItemExpectation(options, expected)}"
						: $"{g.Verb("contains", "contain")} {ContainedItemExpectation(options, expected)} {q}",
					a => options.AreConsideredEqual(a, expected),
					quantifier)),
			subject,
			quantifier,
			options);
	}

	/// <summary>
	///     Verifies that the collection contains the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeCountResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>
		Contains(
			this IThat<IAsyncEnumerable<string?>?> subject,
			string? expected)
	{
		Quantifier quantifier = new();
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringEqualityTypeCountResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new AsyncContainConstraint<string?>(
					expectationBuilder, it, grammars,
					(q, g) => q.IsNever
						? $"{g.Verb("does not contain", "do not contain")} {Formatter.Format(expected)}{options}"
						: $"{g.Verb("contains", "contain")} {Formatter.Format(expected)}{options} {q}",
					a => options.AreConsideredEqual(a, expected),
					quantifier)),
			subject,
			quantifier,
			options);
	}

	/// <summary>
	///     Verifies that the collection contains an item that satisfies the <paramref name="predicate" />.
	/// </summary>
	[GuaranteesNotNull]
	public static CountResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		Contains<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject,
			Func<TItem, bool> predicate,
			[CallerArgumentExpression("predicate")]
			string doNotPopulateThisValue = "")
	{
		predicate.ThrowIfNull();
		Quantifier quantifier = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CountResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainConstraint<TItem>(
					expectationBuilder, it, grammars,
					(q, g) => q.IsNever
						? $"{g.Verb("does not contain", "do not contain")} item matching {doNotPopulateThisValue.TrimCommonWhiteSpace()}"
						: $"{g.Verb("contains", "contain")} item matching {doNotPopulateThisValue.TrimCommonWhiteSpace()} {q}",
					predicate,
					quantifier)),
			subject,
			quantifier);
	}

	/// <summary>
	///     Verifies that the collection contains the provided <paramref name="expected" /> collection.
	/// </summary>
	/// <remarks>
	///     The expected items must appear in the same order and contiguous, i.e. without other items in between. Use
	///     <c>IgnoringInterspersedItems()</c> to allow other items in between or <c>InAnyOrder()</c> to also ignore the
	///     order.
	/// </remarks>
	[GuaranteesNotNull]
	public static ObjectProperCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		Contains<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject,
			IEnumerable<TItem> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		expected.ThrowIfNullOrEmpty();
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectProperCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<TItem, TItem>(
					expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(), expected,
					options, matchOptions, failsForNullSubject: true)),
			subject,
			options,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.ContainsProperly);
	}

	/// <summary>
	///     Verifies that the collection contains the provided <paramref name="expected" /> collection.
	/// </summary>
	/// <remarks>
	///     The expected items must appear in the same order and contiguous, i.e. without other items in between. Use
	///     <c>IgnoringInterspersedItems()</c> to allow other items in between or <c>InAnyOrder()</c> to also ignore the
	///     order.
	/// </remarks>
	[GuaranteesNotNull]
	public static StringProperCollectionMatchResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>
		Contains(
			this IThat<IAsyncEnumerable<string?>?> subject,
			IEnumerable<string?> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		expected.ThrowIfNullOrEmpty();
		StringEqualityOptions options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringProperCollectionMatchResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<string?, string?>(
					expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expected, options, matchOptions, failsForNullSubject: true)),
			subject,
			options,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.ContainsProperly);
	}

	/// <summary>
	///     Verifies that the collection contains the provided <paramref name="expected" /> collection of predicates.
	/// </summary>
	/// <remarks>
	///     The expected predicates must be satisfied in the same order and contiguous, i.e. without other items in
	///     between. Use <c>IgnoringInterspersedItems()</c> to allow other items in between or <c>InAnyOrder()</c> to also
	///     ignore the order.
	/// </remarks>
	[GuaranteesNotNull]
	public static ProperCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		Contains<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject,
			IEnumerable<Expression<Func<TItem, bool>>> expected,
			[CallerArgumentExpression("expected")]
			string doNotPopulateThisValue = "")
	{
		expected.ThrowIfNullOrEmpty();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ProperCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToFromPredicateConstraint<TItem, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expected,
					matchOptions, failsForNullSubject: true)),
			subject,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.ContainsProperly);
	}

	/// <summary>
	///     Verifies that the collection contains the provided <paramref name="expected" /> collection of expectations.
	/// </summary>
	/// <remarks>
	///     The expectations must be satisfied in the same order and contiguous, i.e. without other items in between. Use
	///     <c>IgnoringInterspersedItems()</c> to allow other items in between or <c>InAnyOrder()</c> to also ignore the
	///     order.
	/// </remarks>
	[GuaranteesNotNull]
	public static ProperCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		Contains<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject,
			IEnumerable<Action<IThatSubject<TItem?>>> expected,
			[CallerArgumentExpression("expected")]
			string doNotPopulateThisValue = "")
	{
		expected.ThrowIfNullOrEmpty();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ProperCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToFromExpectationsConstraint<TItem, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expected,
					matchOptions, failsForNullSubject: true)),
			subject,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.ContainsProperly);
	}

	/// <summary>
	///     Verifies that the collection does not contain the <paramref name="unexpected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static ObjectCountResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		DoesNotContain<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject,
			TItem unexpected)
	{
		Quantifier quantifier = new();
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCountResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new AsyncContainConstraint<TItem>(
					expectationBuilder, it, grammars,
					(q, g) => q.ToDoesNotContainExpectation(g, ContainedItemExpectation(options, unexpected)),
					a => options.AreConsideredEqual(a, unexpected),
					quantifier).Invert()),
			subject,
			quantifier,
			options);
	}

	/// <summary>
	///     Verifies that the collection does not contain the <paramref name="unexpected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeCountResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>
		DoesNotContain(
			this IThat<IAsyncEnumerable<string?>?> subject,
			string? unexpected)
	{
		Quantifier quantifier = new();
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringEqualityTypeCountResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new AsyncContainConstraint<string?>(
					expectationBuilder, it, grammars,
					(q, g) => q.ToDoesNotContainExpectation(g, $"{Formatter.Format(unexpected)}{options}"),
					a => options.AreConsideredEqual(a, unexpected),
					quantifier).Invert()),
			subject,
			quantifier,
			options);
	}

	/// <summary>
	///     Verifies that the collection contains no item that satisfies the <paramref name="predicate" />.
	/// </summary>
	[GuaranteesNotNull]
	public static CountResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		DoesNotContain<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject,
			Func<TItem, bool> predicate,
			[CallerArgumentExpression("predicate")]
			string doNotPopulateThisValue = "")
	{
		predicate.ThrowIfNull();
		Quantifier quantifier = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CountResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainConstraint<TItem>(
					expectationBuilder, it, grammars,
					(q, g) => q.ToDoesNotContainExpectation(g, $"item matching {doNotPopulateThisValue.TrimCommonWhiteSpace()}"),
					predicate,
					quantifier).Invert()),
			subject,
			quantifier);
	}

	/// <summary>
	///     Verifies that the collection does not contain the provided <paramref name="unexpected" /> collection.
	/// </summary>
	/// <remarks>
	///     The unexpected items are only considered contained when they appear in the same order and contiguous, i.e.
	///     without other items in between. Use <c>IgnoringInterspersedItems()</c> to also consider them contained with
	///     other items in between or <c>InAnyOrder()</c> to also ignore the order.
	/// </remarks>
	[GuaranteesNotNull]
	public static ObjectProperCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		DoesNotContain<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject,
			IEnumerable<TItem> unexpected,
			[CallerArgumentExpression("unexpected")] string doNotPopulateThisValue = "")
	{
		unexpected.ThrowIfNullOrEmpty();
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectProperCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<TItem, TItem>(
					expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(), unexpected,
					options, matchOptions, failsForNullSubject: true).Invert()),
			subject,
			options,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.ContainsProperly);
	}

	/// <summary>
	///     Verifies that the collection does not contain the provided <paramref name="unexpected" /> collection.
	/// </summary>
	/// <remarks>
	///     The unexpected items are only considered contained when they appear in the same order and contiguous, i.e.
	///     without other items in between. Use <c>IgnoringInterspersedItems()</c> to also consider them contained with
	///     other items in between or <c>InAnyOrder()</c> to also ignore the order.
	/// </remarks>
	[GuaranteesNotNull]
	public static StringProperCollectionMatchResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>
		DoesNotContain(
			this IThat<IAsyncEnumerable<string?>?> subject,
			IEnumerable<string?> unexpected,
			[CallerArgumentExpression("unexpected")] string doNotPopulateThisValue = "")
	{
		unexpected.ThrowIfNullOrEmpty();
		StringEqualityOptions options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringProperCollectionMatchResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<string?, string?>(
					expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpected, options, matchOptions, failsForNullSubject: true).Invert()),
			subject,
			options,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.ContainsProperly);
	}

	/// <summary>
	///     Verifies that the collection does not contain the provided <paramref name="unexpected" /> collection of predicates.
	/// </summary>
	/// <remarks>
	///     The unexpected predicates are only considered contained when they are satisfied in the same order and
	///     contiguous, i.e. without other items in between. Use <c>IgnoringInterspersedItems()</c> to also consider them
	///     contained with other items in between or <c>InAnyOrder()</c> to also ignore the order.
	/// </remarks>
	[GuaranteesNotNull]
	public static ProperCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		DoesNotContain<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject,
			IEnumerable<Expression<Func<TItem, bool>>> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		unexpected.ThrowIfNullOrEmpty();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ProperCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToFromPredicateConstraint<TItem, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpected,
					matchOptions, failsForNullSubject: true).Invert()),
			subject,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.ContainsProperly);
	}

	/// <summary>
	///     Verifies that the collection does not contain the provided <paramref name="unexpected" /> collection of expectations.
	/// </summary>
	/// <remarks>
	///     The unexpected expectations are only considered contained when they are satisfied in the same order and
	///     contiguous, i.e. without other items in between. Use <c>IgnoringInterspersedItems()</c> to also consider them
	///     contained with other items in between or <c>InAnyOrder()</c> to also ignore the order.
	/// </remarks>
	[GuaranteesNotNull]
	public static ProperCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		DoesNotContain<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject,
			IEnumerable<Action<IThatSubject<TItem?>>> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		unexpected.ThrowIfNullOrEmpty();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ProperCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToFromExpectationsConstraint<TItem, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpected,
					matchOptions, failsForNullSubject: true).Invert()),
			subject,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.ContainsProperly);
	}

	/// <summary>
	///     The text for the <paramref name="expected" /> item of a <c>contain</c> expectation.
	/// </summary>
	/// <remarks>
	///     The verb keeps a direct object, so a match type that describes the item reads "contains an item
	///     equivalent to …" instead of only naming the value.
	/// </remarks>
	private static string ContainedItemExpectation<TItem>(ObjectEqualityOptions<TItem> options, TItem expected)
		=> options.GetItemExpectation(Formatter.Format(expected), "an item");

	private sealed class ContainConstraint<TItem>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		Func<Quantifier, ExpectationGrammars, string> expectationText,
		Func<TItem, bool> predicate,
		Quantifier quantifier)
		: ConstraintResult(grammars),
			IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
	{
		private IAsyncEnumerable<TItem>? _actual;
		private int _count;
		private bool _isFinished;
		private bool _isNegated;

		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<TItem>? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			IAsyncEnumerable<TItem> materializedEnumerable =
				context.UseMaterializedAsyncEnumerable<TItem, IAsyncEnumerable<TItem>>(actual);
			int maximumNumberOfCollectionItems =
				Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();
			LimitedCollection<TItem> items = new(maximumNumberOfCollectionItems + 1);
			_count = 0;
			_isFinished = false;
			bool isFailed = false;
			await foreach (TItem item in materializedEnumerable.WithCancellation(cancellationToken))
			{
				if (items.Count <= maximumNumberOfCollectionItems)
				{
					items.Add(item);
				}

				if (predicate(item))
				{
					_count++;
					bool? check = quantifier.Check(_count, false);
					if (check == false)
					{
						isFailed = true;
					}

					if (check == true)
					{
						Outcome = Outcome.Success;
						return this;
					}
				}

				if (items.Count > maximumNumberOfCollectionItems && isFailed)
				{
					Outcome = Outcome.Failure;
					expectationBuilder.AddCollectionContext(items, true);
					return this;
				}
			}

			expectationBuilder.AddCollectionContext(items);
			_isFinished = true;
			if (quantifier.Check(_count, true) ?? _isNegated)
			{
				Outcome = Outcome.Success;
				return this;
			}

			Outcome = Outcome.Failure;
			return this;
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(expectationText.Invoke(quantifier, Grammars));

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_actual == null)
			{
				stringBuilder.ItWasNull(it, Grammars);
			}
			else if (_isFinished)
			{
				if (_count == 0)
				{
					stringBuilder.Append(it).Append(" did not contain it");
				}
				else if (_count == 1)
				{
					stringBuilder.Append(it).Append(" contained it once");
				}
				else if (_count == 2)
				{
					stringBuilder.Append(it).Append(" contained it twice");
				}
				else
				{
					stringBuilder.Append(it).Append(" contained it ").Append(_count).Append(" times");
				}
			}
			else
			{
				stringBuilder.Append(it).Append(" contained it at least ");
				if (_count == 1)
				{
					stringBuilder.Append("once");
				}
				else if (_count == 2)
				{
					stringBuilder.Append("twice");
				}
				else
				{
					stringBuilder.Append(_count).Append(" times");
				}
			}
		}

		/// <inheritdoc cref="ConstraintResult.TryGetValue{TValue}(out TValue)" />
		public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value) where TValue : default
		{
			if (_actual is TValue typedValue)
			{
				value = typedValue;
				return true;
			}

			value = default;
			return typeof(TValue).IsAssignableFrom(typeof(IAsyncEnumerable<TItem>));
		}

		/// <inheritdoc cref="ConstraintResult.Outcome" />
		public override Outcome Outcome
		{
			get => _actual is null ? Outcome.Failure : base.Outcome;
			protected set => base.Outcome = value;
		}

		public override ConstraintResult Negate()
		{
			_isNegated = !_isNegated;
			quantifier.Negate();
			Outcome = Outcome switch
			{
				Outcome.Failure => Outcome.Success,
				Outcome.Success => Outcome.Failure,
				_ => Outcome,
			};
			return this;
		}
	}

	private sealed class AsyncContainConstraint<TItem>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		Func<Quantifier, ExpectationGrammars, string> expectationText,
#if NET8_0_OR_GREATER
		Func<TItem, ValueTask<bool>> predicate,
#else
		Func<TItem, Task<bool>> predicate,
#endif
		Quantifier quantifier)
		: ConstraintResult(grammars),
			IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
	{
		private IAsyncEnumerable<TItem>? _actual;
		private int _count;
		private bool _isFinished;
		private bool _isNegated;

		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<TItem>? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			IAsyncEnumerable<TItem> materializedEnumerable =
				context.UseMaterializedAsyncEnumerable<TItem, IAsyncEnumerable<TItem>>(actual);
			int maximumNumberOfCollectionItems =
				Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();
			LimitedCollection<TItem> items = new(maximumNumberOfCollectionItems + 1);
			_count = 0;
			_isFinished = false;
			bool isFailed = false;
			await foreach (TItem item in materializedEnumerable.WithCancellation(cancellationToken))
			{
				if (items.Count <= maximumNumberOfCollectionItems)
				{
					items.Add(item);
				}

				if (await predicate(item))
				{
					_count++;
					bool? check = quantifier.Check(_count, false);
					if (check == false)
					{
						isFailed = true;
					}

					if (check == true)
					{
						Outcome = Outcome.Success;
						return this;
					}
				}

				if (items.Count > maximumNumberOfCollectionItems && isFailed)
				{
					Outcome = Outcome.Failure;
					expectationBuilder.AddCollectionContext(items, true);
					return this;
				}
			}

			expectationBuilder.AddCollectionContext(items);
			_isFinished = true;
			if (quantifier.Check(_count, true) ?? _isNegated)
			{
				Outcome = Outcome.Success;
				return this;
			}

			Outcome = Outcome.Failure;
			return this;
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(expectationText.Invoke(quantifier, Grammars));

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_actual == null)
			{
				stringBuilder.ItWasNull(it, Grammars);
			}
			else if (_isFinished)
			{
				if (_count == 0)
				{
					stringBuilder.Append(it).Append(" did not contain it");
				}
				else if (_count == 1)
				{
					stringBuilder.Append(it).Append(" contained it once");
				}
				else if (_count == 2)
				{
					stringBuilder.Append(it).Append(" contained it twice");
				}
				else
				{
					stringBuilder.Append(it).Append(" contained it ").Append(_count).Append(" times");
				}
			}
			else
			{
				stringBuilder.Append(it).Append(" contained it at least ");
				if (_count == 1)
				{
					stringBuilder.Append("once");
				}
				else if (_count == 2)
				{
					stringBuilder.Append("twice");
				}
				else
				{
					stringBuilder.Append(_count).Append(" times");
				}
			}
		}

		/// <inheritdoc cref="ConstraintResult.TryGetValue{TValue}(out TValue)" />
		public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value) where TValue : default
		{
			if (_actual is TValue typedValue)
			{
				value = typedValue;
				return true;
			}

			value = default;
			return typeof(TValue).IsAssignableFrom(typeof(IAsyncEnumerable<TItem>));
		}

		/// <inheritdoc cref="ConstraintResult.Outcome" />
		public override Outcome Outcome
		{
			get => _actual is null ? Outcome.Failure : base.Outcome;
			protected set => base.Outcome = value;
		}

		public override ConstraintResult Negate()
		{
			_isNegated = !_isNegated;
			quantifier.Negate();
			Outcome = Outcome switch
			{
				Outcome.Failure => Outcome.Success,
				Outcome.Success => Outcome.Failure,
				_ => Outcome,
			};
			return this;
		}
	}
}
#endif
