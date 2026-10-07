#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using aweXpect.Core;
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

	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true,
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
			expectationBuilder.AddConstraint(
				(Options: options, Expected: expected, Quantifier: quantifier, Negated: negated),
				static (state, it, grammars) =>
					new AsyncContainConstraint<TItem>(it, grammars,
						new EqualItem<TItem>(state.Options, state.Expected),
						state.Quantifier).InvertIf(state.Negated)),
			subject,
			quantifier,
			options);
	}

	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true,
		Factory = typeof(ObjectEqualityWithToleranceOptionsFactory),
		Summary = ContainsValue, NegatedSummary = DoesNotContainValue)]
	internal static ObjectCountWithToleranceResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem,
			TTolerance>
		ContainsItemWithToleranceCore<TItem, TTolerance>(
			IThat<IAsyncEnumerable<TItem>?> subject,
			TItem expected, Options.ObjectEqualityWithToleranceOptions<TItem, TTolerance> options,
			bool negated)
	{
		Quantifier quantifier = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCountWithToleranceResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem,
			TTolerance>(
			expectationBuilder.AddConstraint(
				(Options: options, Expected: expected, Quantifier: quantifier, Negated: negated),
				static (state, it, grammars) =>
					new AsyncContainConstraint<TItem>(it, grammars,
						new EqualItem<TItem>(state.Options, state.Expected),
						state.Quantifier).InvertIf(state.Negated)),
			subject,
			quantifier,
			options);
	}

	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true,
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
			expectationBuilder.AddConstraint(
				(Options: options, Expected: expected, Quantifier: quantifier, Negated: negated),
				static (state, it, grammars) =>
					new AsyncContainConstraint<string?>(it, grammars,
						new EqualStringItem(state.Options, state.Expected),
						state.Quantifier).InvertIf(state.Negated)),
			subject,
			quantifier,
			options);
	}

	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true,
		Summary = ContainsMatchingItem, NegatedSummary = DoesNotContainMatchingItem)]
	internal static CountResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		ContainsMatchingItemCore<TItem>(
			IThat<IAsyncEnumerable<TItem>?> subject, System.Func<TItem, bool> predicate,
			string predicateExpression,
			bool negated)
	{
		predicate.ThrowIfNull();
		Quantifier quantifier = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CountResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>(
			expectationBuilder.AddConstraint(
				(PredicateExpression: predicateExpression, Predicate: predicate, Quantifier: quantifier,
					Negated: negated),
				static (state, it, grammars) =>
					new AsyncContainConstraint<TItem>(it, grammars,
						new ItemMatchingPredicate<TItem>(state.Predicate, state.PredicateExpression),
						state.Quantifier).InvertIf(state.Negated)),
			subject,
			quantifier);
	}

	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true,
		Summary = ContainsCollection, NegatedSummary = DoesNotContainCollection,
		Remarks = ContainsRemarks, NegatedRemarks = DoesNotContainRemarks)]
	internal static ObjectProperCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		ContainsCore<TItem>(
			IThat<IAsyncEnumerable<TItem>?> subject,
			IEnumerable<TItem> expected,
			string expectedExpression,
			bool negated)
	{
		IEnumerable<TItem> expectedValues = expected.ToNonEmptyValues(negated);
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectProperCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint(
				(ExpectedExpression: expectedExpression, ExpectedValues: expectedValues, Options: options,
					MatchOptions: matchOptions, Negated: negated),
				static (state, it, grammars) =>
					new AsyncIsEqualToConstraint<TItem, TItem>(it, grammars,
						state.ExpectedExpression.TrimCommonWhiteSpace(), state.ExpectedValues, state.Options,
						state.MatchOptions, true).InvertIf(state.Negated)),
			subject,
			options,
			matchOptions);
	}

	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true,
		Factory = typeof(ObjectEqualityWithToleranceOptionsFactory),
		Summary = ContainsCollection, NegatedSummary = DoesNotContainCollection,
		Remarks = ContainsRemarks, NegatedRemarks = DoesNotContainRemarks)]
	internal static ObjectProperCollectionMatchWithToleranceResult<IAsyncEnumerable<TItem>,
			IThat<IAsyncEnumerable<TItem>?>, TItem, TTolerance>
		ContainsWithToleranceCore<TItem, TTolerance>(
			IThat<IAsyncEnumerable<TItem>?> subject,
			IEnumerable<TItem> expected,
			ObjectEqualityWithToleranceOptions<TItem, TTolerance> options,
			string expectedExpression,
			bool negated)
	{
		IEnumerable<TItem> expectedValues = expected.ToNonEmptyValues(negated);
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectProperCollectionMatchWithToleranceResult<IAsyncEnumerable<TItem>,
			IThat<IAsyncEnumerable<TItem>?>, TItem, TTolerance>(
			expectationBuilder.AddConstraint(
				(ExpectedExpression: expectedExpression, ExpectedValues: expectedValues, Options: options,
					MatchOptions: matchOptions, Negated: negated),
				static (state, it, grammars) =>
					new AsyncIsEqualToConstraint<TItem, TItem>(it, grammars,
						state.ExpectedExpression.TrimCommonWhiteSpace(), state.ExpectedValues, state.Options,
						state.MatchOptions, true).InvertIf(state.Negated)),
			subject,
			options,
			matchOptions);
	}

	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true,
		Summary = ContainsCollection, NegatedSummary = DoesNotContainCollection,
		Remarks = ContainsRemarks, NegatedRemarks = DoesNotContainRemarks)]
	internal static StringProperCollectionMatchResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>
		ContainsForStringsCore(
			IThat<IAsyncEnumerable<string?>?> subject,
			IEnumerable<string?> expected,
			string expectedExpression,
			bool negated)
	{
		IEnumerable<string?> expectedValues = expected.ToNonEmptyValues(negated);
		StringEqualityOptions options = new(negated ? "unexpected" : "expected");
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringProperCollectionMatchResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>(
			expectationBuilder.AddConstraint(
				(ExpectedExpression: expectedExpression, ExpectedValues: expectedValues, Options: options,
					MatchOptions: matchOptions, Negated: negated),
				static (state, it, grammars) =>
					new AsyncIsEqualToConstraint<string?, string?>(it, grammars,
						state.ExpectedExpression.TrimCommonWhiteSpace(), state.ExpectedValues, state.Options,
						state.MatchOptions, true).InvertIf(state.Negated)),
			subject,
			options,
			matchOptions);
	}

	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true,
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
		IEnumerable<Expression<Func<TItem, bool>>> expectedValues =
			expected.ToNonEmptyValues(negated).WithoutNullElements(negated);
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ProperCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint(
				(ExpectedExpression: expectedExpression, ExpectedValues: expectedValues, MatchOptions: matchOptions,
					Negated: negated),
				static (state, it, grammars)
					=> new AsyncIsEqualToFromPredicateConstraint<TItem, TItem>(it, grammars,
						state.ExpectedExpression.TrimCommonWhiteSpace(), state.ExpectedValues, state.MatchOptions,
						true).InvertIf(state.Negated)),
			subject,
			matchOptions);
	}

	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true,
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
		IEnumerable<Action<IThatSubject<TItem?>>> expectedValues =
			expected.ToNonEmptyValues(negated).WithoutNullElements(negated);
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ProperCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint(
				(ExpectedExpression: expectedExpression, ExpectedValues: expectedValues, MatchOptions: matchOptions,
					Negated: negated),
				static (state, it, grammars)
					=> new AsyncIsEqualToFromExpectationsConstraint<TItem, TItem>(it, grammars,
						state.ExpectedExpression.TrimCommonWhiteSpace(), state.ExpectedValues, state.MatchOptions,
						true).InvertIf(state.Negated)),
			subject,
			matchOptions);
	}
}
#endif
