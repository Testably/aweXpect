#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Customization;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.SourceGenerators;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	private const string ContainsValue =
		"Verifies that the collection contains the <paramref name=\"expected\" /> value.";

	private const string DoesNotContainValue =
		"Verifies that the collection does not contain the <paramref name=\"unexpected\" /> value.";

	private const string ContainsMatchingItem =
		"Verifies that the collection contains an item that satisfies the <paramref name=\"predicate\" />.";

	private const string DoesNotContainMatchingItem =
		"Verifies that the collection contains no item that satisfies the <paramref name=\"predicate\" />.";

	private const string ContainsCollection =
		"Verifies that the collection contains the provided <paramref name=\"expected\" /> collection.";

	private const string DoesNotContainCollection =
		"Verifies that the collection does not contain the provided <paramref name=\"unexpected\" /> collection.";

	private const string ContainsRemarks =
		"The expected items must appear in the same order and contiguous, i.e. without other items in between. Use\n" +
		"<c>IgnoringInterspersedItems()</c> to allow other items in between or <c>InAnyOrder()</c> to also ignore the\n" +
		"order.";

	private const string DoesNotContainRemarks =
		"The unexpected items are only considered contained when they appear in the same order and contiguous, i.e.\n" +
		"without other items in between. Use <c>IgnoringInterspersedItems()</c> to also consider them contained with\n" +
		"other items in between or <c>InAnyOrder()</c> to also ignore the order.";

	[CreateCollectionExpectation("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true,
		Summary = ContainsValue, NegatedSummary = DoesNotContainValue)]
	internal static ObjectCountResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		ContainsItemCore<TItem>(
			IThat<IAsyncEnumerable<TItem>?> subject,
			TItem expected,
			bool negated)
	{
		Quantifier quantifier = new();
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCountResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new AsyncContainConstraint<TItem>(expectationBuilder, it, grammars,
					(q, g) => q.ToContainsExpectation(g, ContainedItemExpectation(options, expected)),
					expected,
					a => options.AreConsideredEqual(a, expected),
					quantifier).InvertIf(negated)),
			subject,
			quantifier,
			options);
	}

	[CreateCollectionExpectation("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true,
		Summary = ContainsValue, NegatedSummary = DoesNotContainValue)]
	internal static StringEqualityTypeCountResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>
		ContainsItemForStringsCore(
			IThat<IAsyncEnumerable<string?>?> subject,
			string? expected,
			bool negated)
	{
		Quantifier quantifier = new();
		StringEqualityOptions options = new(negated ? "unexpected" : "expected");
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringEqualityTypeCountResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new AsyncContainConstraint<string?>(expectationBuilder, it, grammars,
					(q, g) => q.ToContainsExpectation(g, ContainedStringExpectation(options, expected)),
					expected,
					a => options.AreConsideredEqual(a, expected),
					quantifier).InvertIf(negated)),
			subject,
			quantifier,
			options);
	}

	[CreateCollectionExpectation("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true,
		Summary = ContainsMatchingItem, NegatedSummary = DoesNotContainMatchingItem)]
	internal static CountResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		ContainsMatchingItemCore<TItem>(
			IThat<IAsyncEnumerable<TItem>?> subject,
			Func<TItem, bool> predicate,
			string predicateExpression,
			bool negated)
	{
		predicate.ThrowIfNull();
		Quantifier quantifier = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CountResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainConstraint<TItem>(expectationBuilder, it, grammars,
					(q, g) => q.ToContainsExpectation(g,
						$"an item matching {predicateExpression.TrimCommonWhiteSpace()}"),
					predicate,
					quantifier).InvertIf(negated)),
			subject,
			quantifier);
	}

	[CreateCollectionExpectation("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true,
		Summary = ContainsCollection, NegatedSummary = DoesNotContainCollection,
		Remarks = ContainsRemarks, NegatedRemarks = DoesNotContainRemarks)]
	internal static ObjectProperCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		ContainsCore<TItem>(
			IThat<IAsyncEnumerable<TItem>?> subject,
			IEnumerable<TItem> expected,
			string expectedExpression,
			bool negated)
	{
		expected.ThrowIfNullOrEmpty(negated);
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectProperCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<TItem, TItem>(expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected, options, matchOptions,
					failsForNullSubject: true).InvertIf(negated)),
			subject,
			options,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.ContainsProperly);
	}

	[CreateCollectionExpectation("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true,
		Summary = ContainsCollection, NegatedSummary = DoesNotContainCollection,
		Remarks = ContainsRemarks, NegatedRemarks = DoesNotContainRemarks)]
	internal static StringProperCollectionMatchResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>
		ContainsForStringsCore(
			IThat<IAsyncEnumerable<string?>?> subject,
			IEnumerable<string?> expected,
			string expectedExpression,
			bool negated)
	{
		expected.ThrowIfNullOrEmpty(negated);
		StringEqualityOptions options = new(negated ? "unexpected" : "expected");
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringProperCollectionMatchResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<string?, string?>(expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected, options, matchOptions,
					failsForNullSubject: true).InvertIf(negated)),
			subject,
			options,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.ContainsProperly);
	}

	[CreateCollectionExpectation("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true,
		Summary =
			"Verifies that the collection contains the provided <paramref name=\"expected\" /> collection of predicates.",
		NegatedSummary =
			"Verifies that the collection does not contain the provided <paramref name=\"unexpected\" /> collection of predicates.",
		Remarks =
			"The expected predicates must be satisfied in the same order and contiguous, i.e. without other items in\n" +
			"between. Use <c>IgnoringInterspersedItems()</c> to allow other items in between or <c>InAnyOrder()</c> to also\n" +
			"ignore the order.",
		NegatedRemarks =
			"The unexpected predicates are only considered contained when they are satisfied in the same order and\n" +
			"contiguous, i.e. without other items in between. Use <c>IgnoringInterspersedItems()</c> to also consider them\n" +
			"contained with other items in between or <c>InAnyOrder()</c> to also ignore the order.")]
	internal static ProperCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		ContainsFromPredicatesCore<TItem>(
			IThat<IAsyncEnumerable<TItem>?> subject,
			IEnumerable<Expression<Func<TItem, bool>>> expected,
			string expectedExpression,
			bool negated)
	{
		expected.ThrowIfNullOrEmpty(negated);
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ProperCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToFromPredicateConstraint<TItem, TItem>(expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected, matchOptions,
					failsForNullSubject: true).InvertIf(negated)),
			subject,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.ContainsProperly);
	}

	[CreateCollectionExpectation("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true,
		Summary =
			"Verifies that the collection contains the provided <paramref name=\"expected\" /> collection of expectations.",
		NegatedSummary =
			"Verifies that the collection does not contain the provided <paramref name=\"unexpected\" /> collection of expectations.",
		Remarks =
			"The expectations must be satisfied in the same order and contiguous, i.e. without other items in between. Use\n" +
			"<c>IgnoringInterspersedItems()</c> to allow other items in between or <c>InAnyOrder()</c> to also ignore the\n" +
			"order.",
		NegatedRemarks =
			"The unexpected expectations are only considered contained when they are satisfied in the same order and\n" +
			"contiguous, i.e. without other items in between. Use <c>IgnoringInterspersedItems()</c> to also consider them\n" +
			"contained with other items in between or <c>InAnyOrder()</c> to also ignore the order.")]
	internal static ProperCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		ContainsFromExpectationsCore<TItem>(
			IThat<IAsyncEnumerable<TItem>?> subject,
			IEnumerable<Action<IThatSubject<TItem?>>> expected,
			string expectedExpression,
			bool negated)
	{
		expected.ThrowIfNullOrEmpty(negated);
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ProperCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToFromExpectationsConstraint<TItem, TItem>(expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected, matchOptions,
					failsForNullSubject: true).InvertIf(negated)),
			subject,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.ContainsProperly);
	}


	/// <summary>
	///     The text for the <paramref name="expected" /> item of a <c>contain</c> expectation.
	/// </summary>
	/// <remarks>
	///     The verb keeps a direct object, so a match type that describes the item reads "contains an item
	///     equivalent to …", and one that only formats it names the comparison itself instead of leaving the
	///     reader to guess it from "contains 3".
	/// </remarks>
	private static string ContainedItemExpectation<TItem>(ObjectEqualityOptions<TItem> options, TItem expected)
		=> options.GetItemExpectation(Formatter.Format(expected), "an item", "equal to");

	/// <summary>
	///     The text for the <paramref name="expected" /> string of a <c>contain</c> expectation.
	/// </summary>
	/// <remarks>
	///     A match type other than equality describes the item, so it reads "contains an item matching regex …".
	/// </remarks>
	private static string ContainedStringExpectation(StringEqualityOptions options, string? expected)
		=> options.InspectsSubject
			? "an item " + options.GetExpectation(expected, ExpectationGrammars.None)
			: Formatter.Format(expected) + options;

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
				context.UseMaterializedAsyncEnumerable<TItem, IAsyncEnumerable<TItem>>(actual, cancellationToken);
			int maximumNumberOfCollectionItems =
				Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();
			LimitedCollection<TItem> items = new(maximumNumberOfCollectionItems + 1);
			_count = 0;
			_isFinished = false;
			bool isFailed = false;
			int totalCount = 0;
			await foreach (TItem item in materializedEnumerable.WithCancellation(cancellationToken))
			{
				totalCount++;
				if (items.Count <= maximumNumberOfCollectionItems)
				{
					items.Add(item);
				}

				if (UserCode.Invoke(predicate, item, "the predicate"))
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

			expectationBuilder.AddCollectionContext(items, totalCount: totalCount);
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
		TItem expected,
		Func<TItem, ValueTask<bool>> predicate,
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
				context.UseMaterializedAsyncEnumerable<TItem, IAsyncEnumerable<TItem>>(actual, cancellationToken);
			int maximumNumberOfCollectionItems =
				Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();
			LimitedCollection<TItem> items = new(maximumNumberOfCollectionItems + 1);
			_count = 0;
			_isFinished = false;
			bool isFailed = false;
			int totalCount = 0;
			await foreach (TItem item in materializedEnumerable.WithCancellation(cancellationToken))
			{
				totalCount++;
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

			expectationBuilder.AddCollectionContext(items, totalCount: totalCount);
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
			else if (_isFinished && _count == 0)
			{
				stringBuilder.Append(it).Append(" did not contain it");
			}
			else
			{
				stringBuilder.Append(it).Append(" contained ");
				Formatter.Format(stringBuilder, expected);
				stringBuilder.Append(_isFinished ? " " : " at least ");
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
