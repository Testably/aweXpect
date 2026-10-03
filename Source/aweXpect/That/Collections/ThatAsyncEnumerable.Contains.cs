#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
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
						(q, g) => q.ToContainsExpectation(g, ContainedItemExpectation(state.Options, state.Expected)),
						state.Expected,
						a => state.Options.AreConsideredEqual(a, state.Expected),
						state.Quantifier,
						appendOptionsContexts: state.Options.AppendContexts).InvertIf(state.Negated)),
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
			TItem expected,
			ObjectEqualityWithToleranceOptions<TItem, TTolerance> options,
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
						(q, g) => q.ToContainsExpectation(g, ContainedItemExpectation(state.Options, state.Expected)),
						state.Expected,
						a => state.Options.AreConsideredEqual(a, state.Expected),
						state.Quantifier,
						appendOptionsContexts: state.Options.AppendContexts).InvertIf(state.Negated)),
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
						(q, g) => q.ToContainsExpectation(g, ContainedStringExpectation(state.Options, state.Expected)),
						state.Expected,
						a => state.Options.AreConsideredEqual(a, state.Expected),
						state.Quantifier).InvertIf(state.Negated)),
			subject,
			quantifier,
			options);
	}

	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true,
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
			expectationBuilder.AddConstraint(
				(PredicateExpression: predicateExpression, Predicate: predicate, Quantifier: quantifier,
					Negated: negated),
				static (state, it, grammars) =>
					new AsyncContainConstraint<TItem>(it, grammars,
						(q, g) => q.ToContainsExpectation(g,
							$"an item matching {state.PredicateExpression.TrimCommonWhiteSpace()}"),
						state.Predicate,
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
						state.MatchOptions, failsForNullSubject: true).InvertIf(state.Negated)),
			subject,
			options,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.ContainsProperly);
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
						state.MatchOptions, failsForNullSubject: true).InvertIf(state.Negated)),
			subject,
			options,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.ContainsProperly);
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
						state.MatchOptions, failsForNullSubject: true).InvertIf(state.Negated)),
			subject,
			options,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.ContainsProperly);
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
		IEnumerable<Expression<Func<TItem, bool>>> expectedValues = expected.ToNonEmptyValues(negated);
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ProperCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint(
				(ExpectedExpression: expectedExpression, ExpectedValues: expectedValues, MatchOptions: matchOptions,
					Negated: negated),
				static (state, it, grammars)
					=> new AsyncIsEqualToFromPredicateConstraint<TItem, TItem>(it, grammars,
						state.ExpectedExpression.TrimCommonWhiteSpace(), state.ExpectedValues, state.MatchOptions,
						failsForNullSubject: true).InvertIf(state.Negated)),
			subject,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.ContainsProperly);
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
		IEnumerable<Action<IThatSubject<TItem?>>> expectedValues = expected.ToNonEmptyValues(negated);
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ProperCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint(
				(ExpectedExpression: expectedExpression, ExpectedValues: expectedValues, MatchOptions: matchOptions,
					Negated: negated),
				static (state, it, grammars)
					=> new AsyncIsEqualToFromExpectationsConstraint<TItem, TItem>(it, grammars,
						state.ExpectedExpression.TrimCommonWhiteSpace(), state.ExpectedValues, state.MatchOptions,
						failsForNullSubject: true).InvertIf(state.Negated)),
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
}
#endif
