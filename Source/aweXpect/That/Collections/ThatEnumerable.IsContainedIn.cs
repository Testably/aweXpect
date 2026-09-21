using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.SourceGenerators;

namespace aweXpect;

public static partial class ThatEnumerable
{
	private const string ContainedIn =
		"Verifies that the collection is contained in the provided <paramref name=\"expected\" /> collection.";

	private const string NotContainedIn =
		"Verifies that the collection is not contained in the provided <paramref name=\"unexpected\" /> collection.";

	private const string ContainedInRemarks =
		"The subject items must appear in the expected collection in the same order and contiguous, i.e. without\n" +
		"other items in between. Use <c>IgnoringInterspersedItems()</c> to allow other items in between or\n" +
		"<c>InAnyOrder()</c> to also ignore the order.";

	private const string NotContainedInRemarks =
		"The subject is only considered contained when its items appear in the unexpected collection in the same order\n" +
		"and contiguous, i.e. without other items in between. Use <c>IgnoringInterspersedItems()</c> to also consider it\n" +
		"contained with other items in between or <c>InAnyOrder()</c> to also ignore the order.";

	[CreateCollectionExpectation("Is{Not}ContainedIn", GuaranteesNotNull = true,
		Summary = ContainedIn, NegatedSummary = NotContainedIn,
		Remarks = ContainedInRemarks, NegatedRemarks = NotContainedInRemarks)]
	internal static ObjectProperCollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		IsContainedInCore<TItem>(
			IThat<IEnumerable<TItem>?> subject,
			IEnumerable<TItem> expected,
			string expectedExpression,
			bool negated)
	{
		expected.ThrowIfNull(negated ? "unexpected" : "expected");
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.IsContainedIn);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectProperCollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint<IEnumerable<TItem>?>((it, grammars) =>
			{
				IsEqualToConstraint<TItem, TItem> constraint = new(expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected, options, matchOptions,
					failsForNullSubject: true);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.IsContainedInProperly);
	}

	[CreateCollectionExpectation("Is{Not}ContainedIn", GuaranteesNotNull = true,
		Summary = ContainedIn, NegatedSummary = NotContainedIn,
		Remarks = ContainedInRemarks, NegatedRemarks = NotContainedInRemarks)]
	internal static StringProperCollectionMatchResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>
		IsContainedInForStringsCore(
			IThat<IEnumerable<string?>?> subject,
			IEnumerable<string?> expected,
			string expectedExpression,
			bool negated)
	{
		expected.ThrowIfNull(negated ? "unexpected" : "expected");
		StringEqualityOptions options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.IsContainedIn);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringProperCollectionMatchResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>(
			expectationBuilder.AddConstraint<IEnumerable<string?>?>((it, grammars) =>
			{
				IsEqualToConstraint<string?, string?> constraint = new(expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected, options, matchOptions,
					failsForNullSubject: true);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.IsContainedInProperly);
	}

	[CreateCollectionExpectation("Is{Not}ContainedIn", GuaranteesNotNull = true, Priority = -1,
		Summary = ContainedIn, NegatedSummary = NotContainedIn,
		Remarks = ContainedInRemarks, NegatedRemarks = NotContainedInRemarks)]
	internal static ObjectProperCollectionMatchResult<IEnumerable, IThat<IEnumerable?>, TItem>
		IsContainedInForEnumerableCore<TItem>(
			IThat<IEnumerable?> subject,
			IEnumerable<TItem> expected,
			string expectedExpression,
			bool negated)
	{
		expected.ThrowIfNull(negated ? "unexpected" : "expected");
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.IsContainedIn);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectProperCollectionMatchResult<IEnumerable, IThat<IEnumerable?>, TItem>(
			expectationBuilder.AddConstraint<IEnumerable?>((it, grammars) =>
			{
				IsEqualToForEnumerableConstraint<IEnumerable, TItem, TItem> constraint = new(
					expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected, options, matchOptions,
					failsForNullSubject: true);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.IsContainedInProperly);
	}

	[CreateCollectionExpectation("Is{Not}ContainedIn", GuaranteesNotNull = true,
		Summary =
			"Verifies that the collection is contained in the provided <paramref name=\"expected\" /> collection of predicates.",
		NegatedSummary =
			"Verifies that the collection is not contained in the provided <paramref name=\"unexpected\" /> collection of predicates.",
		Remarks = ContainedInRemarks, NegatedRemarks = NotContainedInRemarks)]
	internal static ProperCollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		IsContainedInFromPredicatesCore<TItem>(
			IThat<IEnumerable<TItem>?> subject,
			IEnumerable<Expression<Func<TItem, bool>>> expected,
			string expectedExpression,
			bool negated)
	{
		expected.ThrowIfNull(negated ? "unexpected" : "expected");
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.IsContainedIn);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ProperCollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint<IEnumerable<TItem>?>((it, grammars) =>
			{
				IsEqualToFromPredicateConstraint<TItem, TItem> constraint = new(expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected, matchOptions, failsForNullSubject: true);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.IsContainedInProperly);
	}

	[CreateCollectionExpectation("Is{Not}ContainedIn", GuaranteesNotNull = true,
		Summary =
			"Verifies that the collection is contained in the provided <paramref name=\"expected\" /> collection of expectations.",
		NegatedSummary =
			"Verifies that the collection is not contained in the provided <paramref name=\"unexpected\" /> collection of expectations.",
		Remarks = ContainedInRemarks, NegatedRemarks = NotContainedInRemarks)]
	internal static ProperCollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		IsContainedInFromExpectationsCore<TItem>(
			IThat<IEnumerable<TItem>?> subject,
			IEnumerable<Action<IThatSubject<TItem?>>> expected,
			string expectedExpression,
			bool negated)
	{
		expected.ThrowIfNull(negated ? "unexpected" : "expected");
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.IsContainedIn);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ProperCollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint<IEnumerable<TItem>?>((it, grammars) =>
			{
				IsEqualToFromExpectationsConstraint<TItem, TItem> constraint = new(expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected, matchOptions, failsForNullSubject: true);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.IsContainedInProperly);
	}

	[CreateCollectionExpectation("Is{Not}ContainedIn", PerSubject = true,
		Summary = ContainedIn, NegatedSummary = NotContainedIn,
		Remarks = ContainedInRemarks, NegatedRemarks = NotContainedInRemarks)]
	internal static ObjectProperCollectionMatchResult<TCollection, IThat<TCollection>, TItem>
		IsContainedInForCollectionCore<TCollection, TItem>(
			IThat<TCollection> subject,
			IEnumerable<TItem> expected,
			string expectedExpression,
			bool negated)
		where TCollection : IEnumerable
	{
		expected.ThrowIfNull(negated ? "unexpected" : "expected");
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.IsContainedIn);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectProperCollectionMatchResult<TCollection, IThat<TCollection>, TItem>(
			expectationBuilder.AddConstraint<TCollection?>((it, grammars) =>
			{
				IsEqualToForEnumerableConstraint<TCollection, TItem, TItem> constraint = new(
					expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected, options, matchOptions);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.IsContainedInProperly);
	}

	[CreateCollectionExpectation("Is{Not}ContainedIn", PerSubject = true,
		Summary = ContainedIn, NegatedSummary = NotContainedIn,
		Remarks = ContainedInRemarks, NegatedRemarks = NotContainedInRemarks)]
	internal static StringProperCollectionMatchResult<TCollection, IThat<TCollection>>
		IsContainedInForCollectionStringsCore<TCollection>(
			IThat<TCollection> subject,
			IEnumerable<string?> expected,
			string expectedExpression,
			bool negated)
		where TCollection : IEnumerable
	{
		expected.ThrowIfNull(negated ? "unexpected" : "expected");
		StringEqualityOptions options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.IsContainedIn);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringProperCollectionMatchResult<TCollection, IThat<TCollection>>(
			expectationBuilder.AddConstraint<TCollection?>((it, grammars) =>
			{
				IsEqualToForEnumerableConstraint<TCollection, string?, string?> constraint = new(
					expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected, options, matchOptions);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.IsContainedInProperly);
	}
}
