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
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.SourceGenerators;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatEnumerable
{
	private const string ContainsValue =
		"Verifies that the collection contains the <paramref name=\"expected\" /> value.";

	private const string DoesNotContainValue =
		"Verifies that the collection does not contain the <paramref name=\"unexpected\" /> value.";

	private const string ContainsMatchingItem =
		"Verifies that the collection contains an item that satisfies the <paramref name=\"predicate\" />.";

	private const string DoesNotContainMatchingItem =
		"Verifies that the collection contains no item that satisfies the <paramref name=\"predicate\" />.";

	private const string NullLiteralRemarks =
		"The priority lets a <see langword=\"null\" /> literal bind to this overload instead of the collection overloads.";

	private const string BelowValuePriorityRemarks =
		"The priority is below the one of the value overload, so that a <see langword=\"null\" /> literal binds to the value\n" +
		"overload instead of to this one.";

	private const string UntypedCollectionItemRemarks =
		"Without this overload a collection argument without an item type would bind to the item overload and be\n" +
		"expected as a single item.";

	private const string SetLookupRemarks =
		"A set with a custom comparer, e.g. a <c>HashSet&lt;T&gt;</c> created with one, is asked for the item itself, so\n" +
		"that its comparer decides and the item is counted at most once. Its items are enumerated instead when the\n" +
		"comparison is changed, e.g. with <c>Using(…)</c>, or when the item is <see langword=\"null\" />.";

	private const string SetItemComparerRemarks =
		"A subject that is a set with a custom comparer, e.g. a <c>HashSet&lt;T&gt;</c> created with one, compares its items\n" +
		"with that comparer, unless the comparison is changed, e.g. with <c>Using(…)</c>.";

	private const string SetComparerRemarks = SetItemComparerRemarks + " The comparer of an expected set is not used.";

	private const string UntypedSetComparerRemarks =
		"A subject that is a set of the expected item type with a custom comparer, e.g. a <c>HashSet&lt;string&gt;</c>\n" +
		"created with one for expected strings, compares its items with that comparer, unless the comparison is changed,\n" +
		"e.g. with <c>Using(…)</c>. The comparer of a set of another item type cannot be read without reflection and is\n" +
		"not used.";

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

	private const string ContainsPredicates =
		"Verifies that the collection contains the provided <paramref name=\"expected\" /> collection of predicates.";

	private const string DoesNotContainPredicates =
		"Verifies that the collection does not contain the provided <paramref name=\"unexpected\" /> collection of predicates.";

	private const string ContainsPredicatesRemarks =
		"The expected predicates must be satisfied in the same order and contiguous, i.e. without other items in\n" +
		"between. Use <c>IgnoringInterspersedItems()</c> to allow other items in between or <c>InAnyOrder()</c> to also\n" +
		"ignore the order.";

	private const string DoesNotContainPredicatesRemarks =
		"The unexpected predicates are only considered contained when they are satisfied in the same order and\n" +
		"contiguous, i.e. without other items in between. Use <c>IgnoringInterspersedItems()</c> to also consider them\n" +
		"contained with other items in between or <c>InAnyOrder()</c> to also ignore the order.";

	private const string ContainsExpectations =
		"Verifies that the collection contains the provided <paramref name=\"expected\" /> collection of expectations.";

	private const string DoesNotContainExpectations =
		"Verifies that the collection does not contain the provided <paramref name=\"unexpected\" /> collection of expectations.";

	private const string ContainsExpectationsRemarks =
		"The expectations must be satisfied in the same order and contiguous, i.e. without other items in between. Use\n" +
		"<c>IgnoringInterspersedItems()</c> to allow other items in between or <c>InAnyOrder()</c> to also ignore the\n" +
		"order.";

	private const string DoesNotContainExpectationsRemarks =
		"The unexpected expectations are only considered contained when they are satisfied in the same order and\n" +
		"contiguous, i.e. without other items in between. Use <c>IgnoringInterspersedItems()</c> to also consider them\n" +
		"contained with other items in between or <c>InAnyOrder()</c> to also ignore the order.";

	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true,
		Summary = ContainsValue, NegatedSummary = DoesNotContainValue, Remarks = SetLookupRemarks)]
	internal static ObjectCountResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		ContainsItemCore<TItem>(
			IThat<IEnumerable<TItem>?> subject,
			TItem expected,
			bool negated)
	{
		Quantifier quantifier = new();
		ItemEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCountResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint(
				(Options: options, Expected: expected, Quantifier: quantifier, Negated: negated),
				static (state, it, grammars) =>
				{
					SubjectEqualityOptions<TItem, TItem> itemOptions =
						new(state.Options, () => state.Expected is not null && state.Options.HasDefaultMatchType);
					return new ContainConstraint<IEnumerable<TItem>?, TItem>(it, grammars,
						(q, g) => q.ToContainsExpectation(g,
							ContainedItemExpectation(state.Options, state.Expected) + itemOptions.Comparer),
						state.Expected,
						a => itemOptions.AreConsideredEqual(a, state.Expected),
						state.Quantifier,
						a => itemOptions.UseComparerOf(a) ? ContainsBySetLookup(a, state.Expected) : null,
						appendOptionsContexts: state.Options.AppendContexts).InvertIf(state.Negated);
				}),
			subject,
			quantifier,
			options);
	}

	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true, Priority = 1,
		Summary = ContainsValue, NegatedSummary = DoesNotContainValue,
		Remarks = NullLiteralRemarks + "\n" + SetLookupRemarks)]
	internal static StringEqualityTypeCountResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>
		ContainsItemForStringsCore(
			IThat<IEnumerable<string?>?> subject,
			string? expected,
			bool negated)
	{
		Quantifier quantifier = new();
		StringEqualityOptions options = new(negated ? "unexpected" : "expected");
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringEqualityTypeCountResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>(
			expectationBuilder.AddConstraint(
				(Options: options, Expected: expected, Quantifier: quantifier, Negated: negated),
				static (state, it, grammars) =>
				{
					SubjectEqualityOptions<string?, string?> itemOptions =
						new(state.Options, () => state.Expected is not null && state.Options.ComparesByOrdinalEquality);
					return new ContainConstraint<IEnumerable<string?>?, string?>(it, grammars,
						(q, g) => q.ToContainsExpectation(g,
							ContainedStringExpectation(state.Options, state.Expected) + itemOptions.Comparer),
						state.Expected,
						a => itemOptions.AreConsideredEqual(a, state.Expected),
						state.Quantifier,
						a => itemOptions.UseComparerOf(a) ? ContainsBySetLookup(a, state.Expected) : null)
						.InvertIf(state.Negated);
				}),
			subject,
			quantifier,
			options);
	}

	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true,
		Factory = typeof(ObjectEqualityWithToleranceOptionsFactory),
		Summary = ContainsValue, NegatedSummary = DoesNotContainValue, Remarks = SetLookupRemarks)]
	internal static ObjectCountWithToleranceResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem, TTolerance>
		ContainsItemWithToleranceCore<TItem, TTolerance>(
			IThat<IEnumerable<TItem>?> subject,
			TItem expected,
			ObjectEqualityWithToleranceOptions<TItem, TTolerance> options,
			bool negated)
	{
		Quantifier quantifier = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCountWithToleranceResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem, TTolerance>(
			expectationBuilder.AddConstraint(
				(Options: options, Expected: expected, Quantifier: quantifier, Negated: negated),
				static (state, it, grammars) =>
				{
					SubjectEqualityOptions<TItem, TItem> itemOptions = new(state.Options,
						() => state.Expected is not null &&
						      ObjectEqualityWithToleranceOptionsFactory.HasDefaultMatchType(state.Options));
					return new ContainConstraint<IEnumerable<TItem>?, TItem>(it, grammars,
						(q, g) => q.ToContainsExpectation(g,
							ContainedItemExpectation(state.Options, state.Expected) + itemOptions.Comparer),
						state.Expected,
						a => itemOptions.AreConsideredEqual(a, state.Expected),
						state.Quantifier,
						a => itemOptions.UseComparerOf(a) ? ContainsBySetLookup(a, state.Expected) : null,
						appendOptionsContexts: state.Options.AppendContexts).InvertIf(state.Negated);
				}),
			subject,
			quantifier,
			options);
	}

	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true,
		Summary = ContainsMatchingItem, NegatedSummary = DoesNotContainMatchingItem)]
	internal static CountResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
		ContainsMatchingItemCore<TItem>(
			IThat<IEnumerable<TItem>?> subject,
			Func<TItem, bool> predicate,
			string predicateExpression,
			bool negated)
	{
		predicate.ThrowIfNull();
		Quantifier quantifier = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CountResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>(
			expectationBuilder.AddConstraint(
				(PredicateExpression: predicateExpression, Predicate: predicate, Quantifier: quantifier,
					Negated: negated),
				static (state, it, grammars) =>
					new ContainConstraint<IEnumerable<TItem>?, TItem>(it, grammars,
						(q, g) => q.ToContainsExpectation(g,
							$"an item matching {state.PredicateExpression.TrimCommonWhiteSpace()}"),
						state.Predicate,
						state.Quantifier).InvertIf(state.Negated)),
			subject,
			quantifier);
	}

	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true, Priority = -1,
		Summary = ContainsValue, NegatedSummary = DoesNotContainValue, Remarks = UntypedSetComparerRemarks)]
	internal static ObjectCountResult<IEnumerable, IThat<IEnumerable?>, object?>
		ContainsItemForEnumerableCore(
			IThat<IEnumerable?> subject,
			object? expected,
			bool negated)
		=> ContainsItemForEnumerable<object?>(subject, expected, negated);

	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true, Priority = -1,
		Summary = ContainsValue, NegatedSummary = DoesNotContainValue,
		Remarks = SingleValueRemarks + "\n" + UntypedSetComparerRemarks)]
	internal static ObjectCountResult<IEnumerable, IThat<IEnumerable?>, object?>
		ContainsSingleStringForEnumerableCore(
			IThat<IEnumerable?> subject,
			string? expected,
			bool negated)
		=> ContainsItemForEnumerable<string?>(subject, expected, negated);

	/// <remarks>
	///     A subject that is a set of <typeparamref name="TSetItem" /> with a custom comparer compares its items with that
	///     comparer, unless the comparison is changed.
	/// </remarks>
	private static ObjectCountResult<IEnumerable, IThat<IEnumerable?>, object?>
		ContainsItemForEnumerable<TSetItem>(
			IThat<IEnumerable?> subject,
			object? expected,
			bool negated)
	{
		Quantifier quantifier = new();
		ItemEqualityOptions<object?> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCountResult<IEnumerable, IThat<IEnumerable?>, object?>(
			expectationBuilder.AddConstraint(
				(Options: options, Expected: expected, Quantifier: quantifier, Negated: negated),
				static (state, it, grammars) =>
				{
					SubjectEqualityOptions<TSetItem, object?> itemOptions =
						new(state.Options, () => state.Expected is not null && state.Options.HasDefaultMatchType);
					return new ContainConstraint<IEnumerable?, object?>(it, grammars,
						(q, g) => q.ToContainsExpectation(g,
							ContainedItemExpectation(state.Options, state.Expected) + itemOptions.Comparer),
						state.Expected,
						a => itemOptions.AreConsideredEqual(a, state.Expected),
						state.Quantifier,
						a => itemOptions.UseComparerOf(a) && state.Expected is TSetItem typedExpected
							? ContainsBySetLookup(a, typedExpected)
							: null,
						appendOptionsContexts: state.Options.AppendContexts).InvertIf(state.Negated);
				}),
			subject,
			quantifier,
			options);
	}

	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true, Priority = -2,
		Summary = ContainsMatchingItem, NegatedSummary = DoesNotContainMatchingItem,
		Remarks = BelowValuePriorityRemarks)]
	internal static CountResult<IEnumerable, IThat<IEnumerable?>>
		ContainsMatchingItemForEnumerableCore(
			IThat<IEnumerable?> subject,
			Func<object?, bool> predicate,
			string predicateExpression,
			bool negated)
	{
		predicate.ThrowIfNull();
		Quantifier quantifier = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CountResult<IEnumerable, IThat<IEnumerable?>>(
			expectationBuilder.AddConstraint(
				(PredicateExpression: predicateExpression, Predicate: predicate, Quantifier: quantifier,
					Negated: negated),
				static (state, it, grammars) =>
					new ContainConstraint<IEnumerable?, object?>(it, grammars,
						(q, g) => q.ToContainsExpectation(g,
							$"an item matching {state.PredicateExpression.TrimCommonWhiteSpace()}"),
						state.Predicate,
						state.Quantifier).InvertIf(state.Negated)),
			subject,
			quantifier);
	}

	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", PerSubject = true,
		Summary = ContainsValue, NegatedSummary = DoesNotContainValue)]
	internal static ObjectCountResult<TCollection, IThat<TCollection>, TItem>
		ContainsItemForCollectionCore<TCollection, TItem>(
			IThat<TCollection> subject,
			TItem expected,
			bool negated)
		where TCollection : IEnumerable
	{
		Quantifier quantifier = new();
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCountResult<TCollection, IThat<TCollection>, TItem>(
			expectationBuilder.AddConstraint(
				(Options: options, Expected: expected, Quantifier: quantifier, Negated: negated),
				static (state, it, grammars) =>
					new ContainConstraint<TCollection?, TItem>(it, grammars,
						(q, g) => q.ToContainsExpectation(g, ContainedItemExpectation(state.Options, state.Expected)),
						state.Expected,
						a => state.Options.AreConsideredEqual(a, state.Expected),
						state.Quantifier,
						appendOptionsContexts: state.Options.AppendContexts).InvertIf(state.Negated)),
			subject,
			quantifier,
			options);
	}

	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", PerSubject = true,
		Summary = ContainsValue, NegatedSummary = DoesNotContainValue)]
	internal static StringEqualityTypeCountResult<TCollection, IThat<TCollection>>
		ContainsItemForCollectionStringsCore<TCollection>(
			IThat<TCollection> subject,
			string? expected,
			bool negated)
		where TCollection : IEnumerable
	{
		Quantifier quantifier = new();
		StringEqualityOptions options = new(negated ? "unexpected" : "expected");
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringEqualityTypeCountResult<TCollection, IThat<TCollection>>(
			expectationBuilder.AddConstraint(
				(Options: options, Expected: expected, Quantifier: quantifier, Negated: negated),
				static (state, it, grammars) =>
					new ContainConstraint<TCollection?, string?>(it, grammars,
						(q, g) => q.ToContainsExpectation(g, ContainedStringExpectation(state.Options, state.Expected)),
						state.Expected,
						a => state.Options.AreConsideredEqual(a, state.Expected),
						state.Quantifier).InvertIf(state.Negated)),
			subject,
			quantifier,
			options);
	}

	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", PerSubject = true,
		Factory = typeof(ObjectEqualityWithToleranceOptionsFactory),
		Summary = ContainsValue, NegatedSummary = DoesNotContainValue)]
	internal static ObjectCountWithToleranceResult<TCollection, IThat<TCollection>, TItem, TTolerance>
		ContainsItemWithToleranceForCollectionCore<TCollection, TItem, TTolerance>(
			IThat<TCollection> subject,
			TItem expected,
			ObjectEqualityWithToleranceOptions<TItem, TTolerance> options,
			bool negated)
		where TCollection : IEnumerable
	{
		Quantifier quantifier = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCountWithToleranceResult<TCollection, IThat<TCollection>, TItem, TTolerance>(
			expectationBuilder.AddConstraint(
				(Options: options, Expected: expected, Quantifier: quantifier, Negated: negated),
				static (state, it, grammars) =>
					new ContainConstraint<TCollection?, TItem>(it, grammars,
						(q, g) => q.ToContainsExpectation(g, ContainedItemExpectation(state.Options, state.Expected)),
						state.Expected,
						a => state.Options.AreConsideredEqual(a, state.Expected),
						state.Quantifier,
						appendOptionsContexts: state.Options.AppendContexts).InvertIf(state.Negated)),
			subject,
			quantifier,
			options);
	}

	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", PerSubject = true,
		Summary = ContainsMatchingItem, NegatedSummary = DoesNotContainMatchingItem)]
	internal static CountResult<TCollection, IThat<TCollection>>
		ContainsMatchingItemForCollectionCore<TCollection, TItem>(
			IThat<TCollection> subject,
			Func<TItem, bool> predicate,
			string predicateExpression,
			bool negated)
		where TCollection : IEnumerable
	{
		predicate.ThrowIfNull();
		Quantifier quantifier = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CountResult<TCollection, IThat<TCollection>>(
			expectationBuilder.AddConstraint(
				(PredicateExpression: predicateExpression, Predicate: predicate, Quantifier: quantifier,
					Negated: negated),
				static (state, it, grammars) =>
					new ContainConstraint<TCollection?, TItem>(it, grammars,
						(q, g) => q.ToContainsExpectation(g,
							$"an item matching {state.PredicateExpression.TrimCommonWhiteSpace()}"),
						state.Predicate,
						state.Quantifier).InvertIf(state.Negated)),
			subject,
			quantifier);
	}

	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true,
		Summary = ContainsCollection, NegatedSummary = DoesNotContainCollection,
		Remarks = ContainsRemarks + "\n" + SetComparerRemarks,
		NegatedRemarks = DoesNotContainRemarks + "\n" + SetComparerRemarks)]
	internal static ObjectProperCollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		ContainsCore<TItem>(
			IThat<IEnumerable<TItem>?> subject,
			IEnumerable<TItem> expected,
			string expectedExpression,
			bool negated)
	{
		IEnumerable<TItem> expectedValues = expected.ToNonEmptyValues(negated);
		ItemEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectProperCollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint(
				(ExpectedExpression: expectedExpression, ExpectedValues: expectedValues, Options: options,
					MatchOptions: matchOptions, Negated: negated),
				static (state, it, grammars) =>
					new IsEqualToConstraint<IEnumerable<TItem>, TItem, TItem>(it, grammars,
						state.ExpectedExpression.TrimCommonWhiteSpace(), state.ExpectedValues, state.Options,
						state.MatchOptions,
						failsForNullSubject: true,
						usesDefaultEquality: () => state.Options.HasDefaultMatchType).InvertIf(state.Negated)),
			subject,
			options,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.ContainsProperly);
	}

	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true,
		Summary = ContainsCollection, NegatedSummary = DoesNotContainCollection,
		Remarks = ContainsRemarks + "\n" + SetComparerRemarks,
		NegatedRemarks = DoesNotContainRemarks + "\n" + SetComparerRemarks)]
	internal static StringProperCollectionMatchResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>
		ContainsForStringsCore(
			IThat<IEnumerable<string?>?> subject,
			IEnumerable<string?> expected,
			string expectedExpression,
			bool negated)
	{
		IEnumerable<string?> expectedValues = expected.ToNonEmptyValues(negated);
		StringEqualityOptions options = new(negated ? "unexpected" : "expected");
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringProperCollectionMatchResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>(
			expectationBuilder.AddConstraint(
				(ExpectedExpression: expectedExpression, ExpectedValues: expectedValues, Options: options,
					MatchOptions: matchOptions, Negated: negated),
				static (state, it, grammars) =>
					new IsEqualToConstraint<IEnumerable<string?>, string?, string?>(it, grammars,
						state.ExpectedExpression.TrimCommonWhiteSpace(), state.ExpectedValues, state.Options,
						state.MatchOptions,
						failsForNullSubject: true,
						usesDefaultEquality: () => state.Options.ComparesByOrdinalEquality).InvertIf(state.Negated)),
			subject,
			options,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.ContainsProperly);
	}

	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true,
		Summary = ContainsCollection, NegatedSummary = DoesNotContainCollection,
		Remarks = ContainsRemarks, NegatedRemarks = DoesNotContainRemarks)]
	internal static StringProperCollectionMatchResult<string?[], IThat<string?[]?>>
		ContainsForStringArrayCore(
			IThat<string?[]?> subject,
			IEnumerable<string?> expected,
			string expectedExpression,
			bool negated)
	{
		IEnumerable<string?> expectedValues = expected.ToNonEmptyValues(negated);
		StringEqualityOptions options = new(negated ? "unexpected" : "expected");
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringProperCollectionMatchResult<string?[], IThat<string?[]?>>(
			expectationBuilder.AddConstraint(
				(ExpectedExpression: expectedExpression, ExpectedValues: expectedValues, Options: options,
					MatchOptions: matchOptions, Negated: negated),
				static (state, it, grammars) =>
					new IsEqualToConstraint<IEnumerable<string?>, string?, string?>(it, grammars,
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
		Remarks = ContainsRemarks + "\n" + SetComparerRemarks,
		NegatedRemarks = DoesNotContainRemarks + "\n" + SetComparerRemarks)]
	internal static ObjectProperCollectionMatchWithToleranceResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>,
			TItem, TTolerance>
		ContainsWithToleranceCore<TItem, TTolerance>(
			IThat<IEnumerable<TItem>?> subject,
			IEnumerable<TItem> expected,
			ObjectEqualityWithToleranceOptions<TItem, TTolerance> options,
			string expectedExpression,
			bool negated)
	{
		IEnumerable<TItem> expectedValues = expected.ToNonEmptyValues(negated);
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectProperCollectionMatchWithToleranceResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>,
			TItem, TTolerance>(
			expectationBuilder.AddConstraint(
				(ExpectedExpression: expectedExpression, ExpectedValues: expectedValues, Options: options,
					MatchOptions: matchOptions, Negated: negated),
				static (state, it, grammars) =>
					new IsEqualToConstraint<IEnumerable<TItem>, TItem, TItem>(it, grammars,
						state.ExpectedExpression.TrimCommonWhiteSpace(), state.ExpectedValues, state.Options,
						state.MatchOptions,
						failsForNullSubject: true,
						usesDefaultEquality: () =>
							ObjectEqualityWithToleranceOptionsFactory.HasDefaultMatchType(state.Options))
						.InvertIf(state.Negated)),
			subject,
			options,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.ContainsProperly);
	}

	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true, Priority = -1,
		Summary = ContainsCollection, NegatedSummary = DoesNotContainCollection,
		Remarks = ContainsRemarks + "\n" + UntypedSetComparerRemarks,
		NegatedRemarks = DoesNotContainRemarks + "\n" + UntypedSetComparerRemarks)]
	internal static ObjectProperCollectionMatchResult<IEnumerable, IThat<IEnumerable?>, TItem>
		ContainsForEnumerableCore<TItem>(
			IThat<IEnumerable?> subject,
			IEnumerable<TItem> expected,
			string expectedExpression,
			bool negated)
	{
		IEnumerable<TItem> expectedValues = expected.ToNonEmptyValues(negated);
		ItemEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectProperCollectionMatchResult<IEnumerable, IThat<IEnumerable?>, TItem>(
			expectationBuilder.AddConstraint(
				(ExpectedExpression: expectedExpression, ExpectedValues: expectedValues, Options: options,
					MatchOptions: matchOptions, Negated: negated),
				static (state, it, grammars) =>
					new IsEqualToConstraint<IEnumerable, TItem, TItem>(it, grammars,
						state.ExpectedExpression.TrimCommonWhiteSpace(), state.ExpectedValues, state.Options,
						state.MatchOptions,
						failsForNullSubject: true,
						usesDefaultEquality: () => state.Options.HasDefaultMatchType).InvertIf(state.Negated)),
			subject,
			options,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.ContainsProperly);
	}

	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true, Priority = -1,
		Summary = ContainsCollection, NegatedSummary = DoesNotContainCollection,
		Remarks = ContainsRemarks + "\n" + UntypedCollectionItemRemarks + "\n" + UntypedSetComparerRemarks,
		NegatedRemarks = DoesNotContainRemarks + "\n" + UntypedCollectionItemRemarks + "\n" +
		                 UntypedSetComparerRemarks)]
	internal static ObjectProperCollectionMatchResult<IEnumerable, IThat<IEnumerable?>, object?>
		ContainsForObjectsCore(
			IThat<IEnumerable?> subject,
			IEnumerable expected,
			string expectedExpression,
			bool negated)
		=> ContainsForEnumerableCore<object?>(subject, expected?.Cast<object?>()!, expectedExpression, negated);

	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", PerSubject = true,
		Summary = ContainsCollection, NegatedSummary = DoesNotContainCollection,
		Remarks = ContainsRemarks, NegatedRemarks = DoesNotContainRemarks)]
	internal static ObjectProperCollectionMatchResult<TCollection, IThat<TCollection>, TItem>
		ContainsForCollectionCore<TCollection, TItem>(
			IThat<TCollection> subject,
			IEnumerable<TItem> expected,
			string expectedExpression,
			bool negated)
		where TCollection : IEnumerable
	{
		IEnumerable<TItem> expectedValues = expected.ToNonEmptyValues(negated);
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectProperCollectionMatchResult<TCollection, IThat<TCollection>, TItem>(
			expectationBuilder.AddConstraint(
				(ExpectedExpression: expectedExpression, ExpectedValues: expectedValues, Options: options,
					MatchOptions: matchOptions, Negated: negated),
				static (state, it, grammars) =>
					new IsEqualToConstraint<TCollection, TItem, TItem>(it, grammars,
						state.ExpectedExpression.TrimCommonWhiteSpace(), state.ExpectedValues, state.Options,
						state.MatchOptions, failsForNullSubject: true).InvertIf(state.Negated)),
			subject,
			options,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.ContainsProperly);
	}

	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", PerSubject = true,
		Summary = ContainsCollection, NegatedSummary = DoesNotContainCollection,
		Remarks = ContainsRemarks, NegatedRemarks = DoesNotContainRemarks)]
	internal static StringProperCollectionMatchResult<TCollection, IThat<TCollection>>
		ContainsForCollectionStringsCore<TCollection>(
			IThat<TCollection> subject,
			IEnumerable<string?> expected,
			string expectedExpression,
			bool negated)
		where TCollection : IEnumerable
	{
		IEnumerable<string?> expectedValues = expected.ToNonEmptyValues(negated);
		StringEqualityOptions options = new(negated ? "unexpected" : "expected");
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringProperCollectionMatchResult<TCollection, IThat<TCollection>>(
			expectationBuilder.AddConstraint(
				(ExpectedExpression: expectedExpression, ExpectedValues: expectedValues, Options: options,
					MatchOptions: matchOptions, Negated: negated),
				static (state, it, grammars) =>
					new IsEqualToConstraint<TCollection, string?, string?>(it, grammars,
						state.ExpectedExpression.TrimCommonWhiteSpace(), state.ExpectedValues, state.Options,
						state.MatchOptions, failsForNullSubject: true).InvertIf(state.Negated)),
			subject,
			options,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.ContainsProperly);
	}

	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", PerSubject = true,
		Factory = typeof(ObjectEqualityWithToleranceOptionsFactory),
		Summary = ContainsCollection, NegatedSummary = DoesNotContainCollection,
		Remarks = ContainsRemarks, NegatedRemarks = DoesNotContainRemarks)]
	internal static ObjectProperCollectionMatchWithToleranceResult<TCollection, IThat<TCollection>, TItem, TTolerance>
		ContainsWithToleranceForCollectionCore<TCollection, TItem, TTolerance>(
			IThat<TCollection> subject,
			IEnumerable<TItem> expected,
			ObjectEqualityWithToleranceOptions<TItem, TTolerance> options,
			string expectedExpression,
			bool negated)
		where TCollection : IEnumerable
	{
		IEnumerable<TItem> expectedValues = expected.ToNonEmptyValues(negated);
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectProperCollectionMatchWithToleranceResult<TCollection, IThat<TCollection>, TItem, TTolerance>(
			expectationBuilder.AddConstraint(
				(ExpectedExpression: expectedExpression, ExpectedValues: expectedValues, Options: options,
					MatchOptions: matchOptions, Negated: negated),
				static (state, it, grammars) =>
					new IsEqualToConstraint<TCollection, TItem, TItem>(it, grammars,
						state.ExpectedExpression.TrimCommonWhiteSpace(), state.ExpectedValues, state.Options,
						state.MatchOptions, failsForNullSubject: true).InvertIf(state.Negated)),
			subject,
			options,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.ContainsProperly);
	}

	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true,
		Summary = ContainsPredicates, NegatedSummary = DoesNotContainPredicates,
		Remarks = ContainsPredicatesRemarks, NegatedRemarks = DoesNotContainPredicatesRemarks)]
	internal static ProperCollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		ContainsFromPredicatesCore<TItem>(
			IThat<IEnumerable<TItem>?> subject,
			IEnumerable<Expression<Func<TItem, bool>>> expected,
			string expectedExpression,
			bool negated)
	{
		IEnumerable<Expression<Func<TItem, bool>>> expectedValues = expected.ToNonEmptyValues(negated);
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ProperCollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint(
				(ExpectedExpression: expectedExpression, ExpectedValues: expectedValues, MatchOptions: matchOptions,
					Negated: negated),
				static (state, it, grammars)
					=> new IsEqualToFromPredicateConstraint<IEnumerable<TItem>, TItem, TItem>(it, grammars,
						state.ExpectedExpression.TrimCommonWhiteSpace(), state.ExpectedValues, state.MatchOptions,
						failsForNullSubject: true).InvertIf(state.Negated)),
			subject,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.ContainsProperly);
	}

	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true,
		Summary = ContainsExpectations, NegatedSummary = DoesNotContainExpectations,
		Remarks = ContainsExpectationsRemarks, NegatedRemarks = DoesNotContainExpectationsRemarks)]
	internal static ProperCollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		ContainsFromExpectationsCore<TItem>(
			IThat<IEnumerable<TItem>?> subject,
			IEnumerable<Action<IThatSubject<TItem?>>> expected,
			string expectedExpression,
			bool negated)
	{
		IEnumerable<Action<IThatSubject<TItem?>>> expectedValues = expected.ToNonEmptyValues(negated);
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ProperCollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint(
				(ExpectedExpression: expectedExpression, ExpectedValues: expectedValues, MatchOptions: matchOptions,
					Negated: negated),
				static (state, it, grammars)
					=> new IsEqualToFromExpectationsConstraint<IEnumerable<TItem>, TItem, TItem>(it,
						grammars,
						state.ExpectedExpression.TrimCommonWhiteSpace(), state.ExpectedValues, state.MatchOptions,
						failsForNullSubject: true).InvertIf(state.Negated)),
			subject,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.ContainsProperly);
	}

	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", PerSubject = true, Priority = -1,
		Summary = ContainsPredicates, NegatedSummary = DoesNotContainPredicates,
		Remarks = ContainsPredicatesRemarks + "\n" + BelowCollectionPriorityRemarks,
		NegatedRemarks = DoesNotContainPredicatesRemarks + "\n" + BelowCollectionPriorityRemarks)]
	internal static ProperCollectionMatchResult<TCollection, IThat<TCollection>, TItem>
		ContainsFromPredicatesForCollectionCore<TCollection, TItem>(
			IThat<TCollection> subject,
			IEnumerable<Expression<Func<TItem, bool>>> expected,
			string expectedExpression,
			bool negated)
		where TCollection : IEnumerable<TItem>
	{
		IEnumerable<Expression<Func<TItem, bool>>> expectedValues = expected.ToNonEmptyValues(negated);
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ProperCollectionMatchResult<TCollection, IThat<TCollection>, TItem>(
			expectationBuilder.AddConstraint(
				(ExpectedExpression: expectedExpression, ExpectedValues: expectedValues, MatchOptions: matchOptions,
					Negated: negated),
				static (state, it, grammars)
					=> new IsEqualToFromPredicateConstraint<TCollection, TItem, TItem>(it, grammars,
						state.ExpectedExpression.TrimCommonWhiteSpace(), state.ExpectedValues, state.MatchOptions,
						failsForNullSubject: true).InvertIf(state.Negated)),
			subject,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.ContainsProperly);
	}

	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", PerSubject = true, Priority = -1,
		Summary = ContainsExpectations, NegatedSummary = DoesNotContainExpectations,
		Remarks = ContainsExpectationsRemarks + "\n" + BelowCollectionPriorityRemarks,
		NegatedRemarks = DoesNotContainExpectationsRemarks + "\n" + BelowCollectionPriorityRemarks)]
	internal static ProperCollectionMatchResult<TCollection, IThat<TCollection>, TItem>
		ContainsFromExpectationsForCollectionCore<TCollection, TItem>(
			IThat<TCollection> subject,
			IEnumerable<Action<IThatSubject<TItem?>>> expected,
			string expectedExpression,
			bool negated)
		where TCollection : IEnumerable<TItem>
	{
		IEnumerable<Action<IThatSubject<TItem?>>> expectedValues = expected.ToNonEmptyValues(negated);
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ProperCollectionMatchResult<TCollection, IThat<TCollection>, TItem>(
			expectationBuilder.AddConstraint(
				(ExpectedExpression: expectedExpression, ExpectedValues: expectedValues, MatchOptions: matchOptions,
					Negated: negated),
				static (state, it, grammars)
					=> new IsEqualToFromExpectationsConstraint<TCollection, TItem, TItem>(it,
						grammars,
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

	/// <summary>
	///     Asks a set <paramref name="collection" /> whose comparer decides itself whether it contains the
	///     <paramref name="expected" /> item, or returns <see langword="null" /> when the items have to be enumerated
	///     instead.
	/// </summary>
	/// <remarks>
	///     A <see langword="null" /> item is enumerated, because a set whose comparer rejects <see langword="null" />
	///     throws when asked for it.
	/// </remarks>
	private static bool? ContainsBySetLookup<TItem>(object collection, TItem expected)
		=> expected is not null
			? UserCode.Invoke(static values => values.Collection.Contains(values.Expected),
				(Collection: (ICollection<TItem>)collection, Expected: expected), "the comparer")
			: null;
}
