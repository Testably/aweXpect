using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

	[CreateCollectionExpectation("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true,
		Summary = ContainsValue, NegatedSummary = DoesNotContainValue)]
	internal static ObjectCountResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		ContainsItemCore<TItem>(
			IThat<IEnumerable<TItem>?> subject,
			TItem expected,
			bool negated)
	{
		Quantifier quantifier = new();
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCountResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new AsyncContainConstraint<TItem>(expectationBuilder, it, grammars,
					(q, g) => q.ToContainsExpectation(g, ContainedItemExpectation(options, expected), negated),
					a => options.AreConsideredEqual(a, expected),
					quantifier).InvertIf(negated)),
			subject,
			quantifier,
			options);
	}

	[CreateCollectionExpectation("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true, Priority = 1,
		Summary = ContainsValue, NegatedSummary = DoesNotContainValue, Remarks = NullLiteralRemarks)]
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
			expectationBuilder.AddConstraint((it, grammars) =>
				new AsyncContainConstraint<string?>(expectationBuilder, it, grammars,
					(q, g) => q.ToContainsExpectation(g, $"{Formatter.Format(expected)}{options}", negated),
					a => options.AreConsideredEqual(a, expected),
					quantifier).InvertIf(negated)),
			subject,
			quantifier,
			options);
	}

	[CreateCollectionExpectation("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true,
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
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainConstraint<TItem>(expectationBuilder, it, grammars,
					(q, g) => q.ToContainsExpectation(g,
						$"an item matching {predicateExpression.TrimCommonWhiteSpace()}", negated),
					predicate,
					quantifier).InvertIf(negated)),
			subject,
			quantifier);
	}

	[CreateCollectionExpectation("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true, Priority = -1,
		Summary = ContainsValue, NegatedSummary = DoesNotContainValue)]
	internal static ObjectCountResult<IEnumerable, IThat<IEnumerable?>, object?>
		ContainsItemForEnumerableCore(
			IThat<IEnumerable?> subject,
			object? expected,
			bool negated)
	{
		Quantifier quantifier = new();
		ObjectEqualityOptions<object?> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCountResult<IEnumerable, IThat<IEnumerable?>, object?>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new AsyncContainForEnumerableConstraint<IEnumerable, object?>(expectationBuilder, it, grammars,
					(q, g) => q.ToContainsExpectation(g, ContainedItemExpectation(options, expected), negated),
					a => options.AreConsideredEqual(a, expected),
					quantifier).InvertIf(negated)),
			subject,
			quantifier,
			options);
	}

	[CreateCollectionExpectation("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true, Priority = -1,
		Summary = ContainsValue, NegatedSummary = DoesNotContainValue, Remarks = SingleValueRemarks)]
	internal static ObjectCountResult<IEnumerable, IThat<IEnumerable?>, object?>
		ContainsSingleStringForEnumerableCore(
			IThat<IEnumerable?> subject,
			string? expected,
			bool negated)
		=> ContainsItemForEnumerableCore(subject, expected, negated);

	[CreateCollectionExpectation("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true, Priority = -2,
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
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainForEnumerableConstraint<IEnumerable, object?>(expectationBuilder, it, grammars,
					(q, g) => q.ToContainsExpectation(g,
						$"an item matching {predicateExpression.TrimCommonWhiteSpace()}", negated),
					predicate,
					quantifier).InvertIf(negated)),
			subject,
			quantifier);
	}

	[CreateCollectionExpectation("Contains", NegatedName = "DoesNotContain", PerSubject = true,
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
			expectationBuilder.AddConstraint((it, grammars) =>
				new AsyncContainForEnumerableConstraint<TCollection, TItem>(expectationBuilder, it, grammars,
					(q, g) => q.ToContainsExpectation(g, ContainedItemExpectation(options, expected), negated),
					a => options.AreConsideredEqual(a, expected),
					quantifier).InvertIf(negated)),
			subject,
			quantifier,
			options);
	}

	[CreateCollectionExpectation("Contains", NegatedName = "DoesNotContain", PerSubject = true,
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
			expectationBuilder.AddConstraint((it, grammars) =>
				new AsyncContainForEnumerableConstraint<TCollection, string?>(expectationBuilder, it, grammars,
					(q, g) => q.ToContainsExpectation(g, $"{Formatter.Format(expected)}{options}", negated),
					a => options.AreConsideredEqual(a, expected),
					quantifier).InvertIf(negated)),
			subject,
			quantifier,
			options);
	}

	[CreateCollectionExpectation("Contains", NegatedName = "DoesNotContain", PerSubject = true,
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
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainForEnumerableConstraint<TCollection, TItem>(expectationBuilder, it, grammars,
					(q, g) => q.ToContainsExpectation(g,
						$"an item matching {predicateExpression.TrimCommonWhiteSpace()}", negated),
					predicate,
					quantifier).InvertIf(negated)),
			subject,
			quantifier);
	}

	[CreateCollectionExpectation("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true,
		Summary = ContainsCollection, NegatedSummary = DoesNotContainCollection,
		Remarks = ContainsRemarks, NegatedRemarks = DoesNotContainRemarks)]
	internal static ObjectProperCollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		ContainsCore<TItem>(
			IThat<IEnumerable<TItem>?> subject,
			IEnumerable<TItem> expected,
			string expectedExpression,
			bool negated)
	{
		expected.ThrowIfNullOrEmpty(negated);
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectProperCollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
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
	internal static StringProperCollectionMatchResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>
		ContainsForStringsCore(
			IThat<IEnumerable<string?>?> subject,
			IEnumerable<string?> expected,
			string expectedExpression,
			bool negated)
	{
		expected.ThrowIfNullOrEmpty(negated);
		StringEqualityOptions options = new(negated ? "unexpected" : "expected");
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringProperCollectionMatchResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>(
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
		Summary = ContainsCollection, NegatedSummary = DoesNotContainCollection,
		Remarks = ContainsRemarks, NegatedRemarks = DoesNotContainRemarks)]
	internal static StringProperCollectionMatchResult<string?[], IThat<string?[]?>>
		ContainsForStringArrayCore(
			IThat<string?[]?> subject,
			IEnumerable<string?> expected,
			string expectedExpression,
			bool negated)
	{
		expected.ThrowIfNullOrEmpty(negated);
		StringEqualityOptions options = new(negated ? "unexpected" : "expected");
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringProperCollectionMatchResult<string?[], IThat<string?[]?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<string?, string?>(expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected, options, matchOptions,
					failsForNullSubject: true).InvertIf(negated)),
			subject,
			options,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.ContainsProperly);
	}

	[CreateCollectionExpectation("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true, Priority = -1,
		Summary = ContainsCollection, NegatedSummary = DoesNotContainCollection,
		Remarks = ContainsRemarks, NegatedRemarks = DoesNotContainRemarks)]
	internal static ObjectProperCollectionMatchResult<IEnumerable, IThat<IEnumerable?>, TItem>
		ContainsForEnumerableCore<TItem>(
			IThat<IEnumerable?> subject,
			IEnumerable<TItem> expected,
			string expectedExpression,
			bool negated)
	{
		expected.ThrowIfNullOrEmpty(negated);
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectProperCollectionMatchResult<IEnumerable, IThat<IEnumerable?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToForEnumerableConstraint<IEnumerable, TItem, TItem>(expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected, options, matchOptions,
					failsForNullSubject: true).InvertIf(negated)),
			subject,
			options,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.ContainsProperly);
	}

	[CreateCollectionExpectation("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true, Priority = -1,
		Summary = ContainsCollection, NegatedSummary = DoesNotContainCollection,
		Remarks = ContainsRemarks + "\n" + UntypedCollectionItemRemarks,
		NegatedRemarks = DoesNotContainRemarks + "\n" + UntypedCollectionItemRemarks)]
	internal static ObjectProperCollectionMatchResult<IEnumerable, IThat<IEnumerable?>, object?>
		ContainsForObjectsCore(
			IThat<IEnumerable?> subject,
			IEnumerable expected,
			string expectedExpression,
			bool negated)
		=> ContainsForEnumerableCore<object?>(subject, expected?.Cast<object?>()!, expectedExpression, negated);

	[CreateCollectionExpectation("Contains", NegatedName = "DoesNotContain", PerSubject = true,
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
		expected.ThrowIfNullOrEmpty(negated);
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectProperCollectionMatchResult<TCollection, IThat<TCollection>, TItem>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToForEnumerableConstraint<TCollection, TItem, TItem>(expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected, options, matchOptions,
					failsForNullSubject: true).InvertIf(negated)),
			subject,
			options,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.ContainsProperly);
	}

	[CreateCollectionExpectation("Contains", NegatedName = "DoesNotContain", PerSubject = true,
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
		expected.ThrowIfNullOrEmpty(negated);
		StringEqualityOptions options = new(negated ? "unexpected" : "expected");
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringProperCollectionMatchResult<TCollection, IThat<TCollection>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToForEnumerableConstraint<TCollection, string?, string?>(expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected, options, matchOptions,
					failsForNullSubject: true).InvertIf(negated)),
			subject,
			options,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.ContainsProperly);
	}

	[CreateCollectionExpectation("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true,
		Summary = ContainsPredicates, NegatedSummary = DoesNotContainPredicates,
		Remarks = ContainsPredicatesRemarks, NegatedRemarks = DoesNotContainPredicatesRemarks)]
	internal static ProperCollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		ContainsFromPredicatesCore<TItem>(
			IThat<IEnumerable<TItem>?> subject,
			IEnumerable<Expression<Func<TItem, bool>>> expected,
			string expectedExpression,
			bool negated)
	{
		expected.ThrowIfNullOrEmpty(negated);
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ProperCollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToFromPredicateConstraint<IEnumerable<TItem>, TItem, TItem>(expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected, matchOptions,
					failsForNullSubject: true).InvertIf(negated)),
			subject,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.ContainsProperly);
	}

	[CreateCollectionExpectation("Contains", NegatedName = "DoesNotContain", GuaranteesNotNull = true,
		Summary = ContainsExpectations, NegatedSummary = DoesNotContainExpectations,
		Remarks = ContainsExpectationsRemarks, NegatedRemarks = DoesNotContainExpectationsRemarks)]
	internal static ProperCollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		ContainsFromExpectationsCore<TItem>(
			IThat<IEnumerable<TItem>?> subject,
			IEnumerable<Action<IThatSubject<TItem?>>> expected,
			string expectedExpression,
			bool negated)
	{
		expected.ThrowIfNullOrEmpty(negated);
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ProperCollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToFromExpectationsConstraint<IEnumerable<TItem>, TItem, TItem>(expectationBuilder, it,
					grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected, matchOptions,
					failsForNullSubject: true).InvertIf(negated)),
			subject,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.ContainsProperly);
	}

	[CreateCollectionExpectation("Contains", NegatedName = "DoesNotContain", PerSubject = true, Priority = -1,
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
		expected.ThrowIfNullOrEmpty(negated);
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ProperCollectionMatchResult<TCollection, IThat<TCollection>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToFromPredicateConstraint<TCollection, TItem, TItem>(expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected, matchOptions,
					failsForNullSubject: true).InvertIf(negated)),
			subject,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.ContainsProperly);
	}

	[CreateCollectionExpectation("Contains", NegatedName = "DoesNotContain", PerSubject = true, Priority = -1,
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
		expected.ThrowIfNullOrEmpty(negated);
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ProperCollectionMatchResult<TCollection, IThat<TCollection>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToFromExpectationsConstraint<TCollection, TItem, TItem>(expectationBuilder, it,
					grammars,
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
	///     Casts the <paramref name="item" /> of an untyped enumerable to <typeparamref name="TItem" />.
	/// </summary>
	/// <remarks>
	///     A <see langword="null" /> item is not matched by a type pattern, but is a valid value whenever
	///     <typeparamref name="TItem" /> admits it.
	/// </remarks>
	private static bool TryCastItem<TItem>(object? item, out TItem typedItem)
	{
		if (item is TItem typed)
		{
			typedItem = typed;
			return true;
		}

		typedItem = default!;
		return item is null && default(TItem) is null;
	}

	private sealed class ContainConstraint<TItem>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		Func<Quantifier, ExpectationGrammars, string> expectationText,
		Func<TItem, bool> predicate,
		Quantifier quantifier)
		: ConstraintResult(grammars),
			IContextConstraint<IEnumerable<TItem>?>
	{
		private IEnumerable<TItem>? _actual;
		private int _count;
		private bool _isFinished;
		private bool _isNegated;
		private IEnumerable<TItem>? _materializedEnumerable;

		public ConstraintResult IsMetBy(IEnumerable<TItem>? actual, IEvaluationContext context)
		{
			_actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			_materializedEnumerable =
				context.UseMaterializedEnumerable<TItem, IEnumerable<TItem>>(actual);
			_count = 0;
			_isFinished = false;
			foreach (TItem _ in _materializedEnumerable.Where(predicate))
			{
				_count++;
				bool? check = quantifier.Check(_count, false);
				switch (check)
				{
					case false:
						Outcome = Outcome.Failure;
						expectationBuilder.AddCollectionContext(_materializedEnumerable);
						return this;
					case true:
						Outcome = Outcome.Success;
						return this;
				}
			}

			expectationBuilder.AddCollectionContext(_materializedEnumerable);
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
			return typeof(TValue).IsAssignableFrom(typeof(IEnumerable<TItem>));
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
			IAsyncContextConstraint<IEnumerable<TItem>?>
	{
		private IEnumerable<TItem>? _actual;
		private int _count;
		private bool _isFinished;
		private bool _isNegated;
		private IEnumerable<TItem>? _materializedEnumerable;

		public async Task<ConstraintResult> IsMetBy(IEnumerable<TItem>? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			_materializedEnumerable =
				context.UseMaterializedEnumerable<TItem, IEnumerable<TItem>>(actual);
			_count = 0;
			_isFinished = false;
			foreach (TItem item in _materializedEnumerable)
			{
				if (!await predicate(item))
				{
					continue;
				}

				_count++;
				bool? check = quantifier.Check(_count, false);
				switch (check)
				{
					case false:
						Outcome = Outcome.Failure;
						expectationBuilder.AddCollectionContext(_materializedEnumerable);
						return this;
					case true:
						Outcome = Outcome.Success;
						return this;
				}
			}

			expectationBuilder.AddCollectionContext(_materializedEnumerable);
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
			return typeof(TValue).IsAssignableFrom(typeof(IEnumerable<TItem>));
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

	private sealed class ContainForEnumerableConstraint<TEnumerable, TItem>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		Func<Quantifier, ExpectationGrammars, string> expectationText,
		Func<TItem, bool> predicate,
		Quantifier quantifier)
		: ConstraintResult(grammars),
			IContextConstraint<TEnumerable?>
		where TEnumerable : IEnumerable
	{
		private IEnumerable? _actual;
		private int _count;
		private bool _isFinished;
		private bool _isNegated;
		private IEnumerable? _materializedEnumerable;

		public ConstraintResult IsMetBy(TEnumerable? actual, IEvaluationContext context)
		{
			_actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			_materializedEnumerable = context.UseMaterializedEnumerable(actual);
			_count = 0;
			_isFinished = false;
			foreach (object? item in _materializedEnumerable)
			{
				if (TryCastItem(item, out TItem typedItem) && predicate(typedItem))
				{
					_count++;
					bool? check = quantifier.Check(_count, false);
					switch (check)
					{
						case false:
							Outcome = Outcome.Failure;
							expectationBuilder.AddCollectionContext(_materializedEnumerable);
							return this;
						case true:
							Outcome = Outcome.Success;
							return this;
					}
				}
			}

			expectationBuilder.AddCollectionContext(_materializedEnumerable);
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
			return typeof(TValue).IsAssignableFrom(typeof(IEnumerable<TItem>));
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

	private sealed class AsyncContainForEnumerableConstraint<TEnumerable, TItem>(
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
			IAsyncContextConstraint<TEnumerable?>
		where TEnumerable : IEnumerable
	{
		private IEnumerable? _actual;
		private int _count;
		private bool _isFinished;
		private bool _isNegated;
		private IEnumerable? _materializedEnumerable;

		public async Task<ConstraintResult> IsMetBy(TEnumerable? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			_materializedEnumerable = context.UseMaterializedEnumerable(actual);
			_count = 0;
			_isFinished = false;
			foreach (object? item in _materializedEnumerable)
			{
				if (TryCastItem(item, out TItem typedItem) && await predicate(typedItem))
				{
					_count++;
					bool? check = quantifier.Check(_count, false);
					switch (check)
					{
						case false:
							Outcome = Outcome.Failure;
							expectationBuilder.AddCollectionContext(_materializedEnumerable);
							return this;
						case true:
							Outcome = Outcome.Success;
							return this;
					}
				}
			}

			expectationBuilder.AddCollectionContext(_materializedEnumerable);
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
			return typeof(TValue).IsAssignableFrom(typeof(IEnumerable<TItem>));
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
