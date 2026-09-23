using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
	private const string Matches =
		"Verifies that the collection matches the <paramref name=\"expected\" /> collection.";

	private const string DoesNotMatch =
		"Verifies that the collection does not match the <paramref name=\"unexpected\" /> collection.";

	private const string MatchesPredicates =
		"Verifies that the collection matches the <paramref name=\"expected\" /> collection of predicates.";

	private const string DoesNotMatchPredicates =
		"Verifies that the collection does not match the <paramref name=\"unexpected\" /> collection of predicates.";

	private const string MatchesExpectations =
		"Verifies that the collection matches the <paramref name=\"expected\" /> collection of expectations.";

	private const string DoesNotMatchExpectations =
		"Verifies that the collection does not match the <paramref name=\"unexpected\" /> collection of expectations.";

	private const string SameCollectionTypeRemarks =
		"Without this overload an <see cref=\"System.Collections.Immutable.ImmutableArray{T}\" /> or a collection\n" +
		"expression would bind to the equality expectation for structs, which compares the backing arrays by\n" +
		"reference.";

	private const string BelowCollectionPriorityRemarks =
		"The priority is below the one of the value overloads, so that an empty collection expression binds to them\n" +
		"instead of to this one.";

	[CreateCollectionExpectation("Is{Not}EqualTo", Summary = Matches, NegatedSummary = DoesNotMatch)]
	internal static ObjectCollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		IsEqualToCore<TItem>(
			IThat<IEnumerable<TItem>?> subject,
			IEnumerable<TItem> expected,
			string expectedExpression,
			bool negated)
	{
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint<IEnumerable<TItem>?>((it, grammars) =>
			{
				IsEqualToConstraint<TItem, TItem> constraint = new(expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected, options, matchOptions);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options,
			matchOptions);
	}

	[CreateCollectionExpectation("Is{Not}EqualTo", Summary = Matches, NegatedSummary = DoesNotMatch)]
	internal static StringCollectionMatchResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>
		IsEqualToForStringsCore(
			IThat<IEnumerable<string?>?> subject,
			IEnumerable<string?> expected,
			string expectedExpression,
			bool negated)
	{
		StringEqualityOptions options = new();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringCollectionMatchResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>(
			expectationBuilder.AddConstraint<IEnumerable<string?>?>((it, grammars) =>
			{
				IsEqualToConstraint<string?, string?> constraint = new(expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected, options, matchOptions);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options,
			matchOptions);
	}

	[CreateCollectionExpectation("Is{Not}EqualTo", Priority = -1,
		Summary = Matches, NegatedSummary = DoesNotMatch)]
	internal static ObjectCollectionMatchResult<IEnumerable, IThat<IEnumerable?>, TItem>
		IsEqualToForEnumerableCore<TItem>(
			IThat<IEnumerable?> subject,
			IEnumerable<TItem> expected,
			string expectedExpression,
			bool negated)
	{
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchResult<IEnumerable, IThat<IEnumerable?>, TItem>(
			expectationBuilder.AddConstraint<IEnumerable?>((it, grammars) =>
			{
				IsEqualToForEnumerableConstraint<IEnumerable, TItem, TItem> constraint = new(
					expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected, options, matchOptions);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options,
			matchOptions);
	}

	[CreateCollectionExpectation("Is{Not}EqualTo", Priority = -2,
		Summary = Matches, NegatedSummary = DoesNotMatch,
		Remarks =
			"Without this overload an untyped <see cref=\"System.Collections.IEnumerable\" /> would bind to the\n" +
			"equality expectation for objects, which compares the instances by reference. A multi-dimensional array\n" +
			"has no shape as an <see cref=\"System.Collections.IEnumerable\" />, so it is compared by its flattened\n" +
			"content.")]
	internal static ObjectCollectionMatchResult<IEnumerable, IThat<IEnumerable?>, object?>
		IsEqualToForObjectsCore(
			IThat<IEnumerable?> subject,
			IEnumerable expected,
			string expectedExpression,
			bool negated)
	{
		ObjectEqualityOptions<object?> options = new();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchResult<IEnumerable, IThat<IEnumerable?>, object?>(
			expectationBuilder.AddConstraint<IEnumerable?>((it, grammars) =>
			{
				IsEqualToForEnumerableConstraint<IEnumerable, object?, object?> constraint = new(
					expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected?.Cast<object?>(), options, matchOptions);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options,
			matchOptions);
	}

	[CreateCollectionExpectation("Is{Not}EqualTo", Factory = typeof(ObjectEqualityWithToleranceOptionsFactory),
		Summary = Matches, NegatedSummary = DoesNotMatch)]
	internal static ObjectCollectionMatchWithToleranceResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>,
			TItem, TTolerance>
		IsEqualToWithToleranceCore<TItem, TTolerance>(
			IThat<IEnumerable<TItem>?> subject,
			IEnumerable<TItem> expected,
			ObjectEqualityWithToleranceOptions<TItem, TTolerance> options,
			string expectedExpression,
			bool negated)
	{
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>,
			TItem, TTolerance>(
			expectationBuilder.AddConstraint<IEnumerable<TItem>?>((it, grammars) =>
			{
				IsEqualToConstraint<TItem, TItem> constraint = new(expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected, options, matchOptions);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options,
			matchOptions);
	}

	[CreateCollectionExpectation("Is{Not}EqualTo",
		Summary = MatchesPredicates, NegatedSummary = DoesNotMatchPredicates)]
	internal static CollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		IsEqualToFromPredicatesCore<TItem>(
			IThat<IEnumerable<TItem>?> subject,
			IEnumerable<Expression<Func<TItem, bool>>> expected,
			string expectedExpression,
			bool negated)
	{
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint<IEnumerable<TItem>?>((it, grammars) =>
			{
				IsEqualToFromPredicateConstraint<IEnumerable<TItem>, TItem, TItem> constraint = new(
					expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected, matchOptions);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			matchOptions);
	}

	[CreateCollectionExpectation("Is{Not}EqualTo",
		Summary = MatchesExpectations, NegatedSummary = DoesNotMatchExpectations)]
	internal static CollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		IsEqualToFromExpectationsCore<TItem>(
			IThat<IEnumerable<TItem>?> subject,
			IEnumerable<Action<IThatSubject<TItem?>>> expected,
			string expectedExpression,
			bool negated)
	{
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint<IEnumerable<TItem>?>((it, grammars) =>
			{
				IsEqualToFromExpectationsConstraint<IEnumerable<TItem>, TItem, TItem> constraint = new(
					expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected, matchOptions);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			matchOptions);
	}

	[CreateCollectionExpectation("Is{Not}EqualTo", PerSubject = true,
		Summary = Matches, NegatedSummary = DoesNotMatch)]
	[CreateCollectionExpectation("Is{Not}EqualTo", PerSubject = true, ExpectedType = "{subject}",
		Summary = Matches, NegatedSummary = DoesNotMatch, Remarks = SameCollectionTypeRemarks)]
	internal static ObjectCollectionMatchResult<TCollection, IThat<TCollection>, TItem>
		IsEqualToForCollectionCore<TCollection, TItem>(
			IThat<TCollection> subject,
			IEnumerable<TItem> expected,
			string expectedExpression,
			bool negated)
		where TCollection : IEnumerable
	{
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchResult<TCollection, IThat<TCollection>, TItem>(
			expectationBuilder.AddConstraint<TCollection?>((it, grammars) =>
			{
				IsEqualToForEnumerableConstraint<TCollection, TItem, TItem> constraint = new(
					expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected, options, matchOptions);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options,
			matchOptions);
	}

	[CreateCollectionExpectation("Is{Not}EqualTo", PerSubject = true,
		Summary = Matches, NegatedSummary = DoesNotMatch)]
	[CreateCollectionExpectation("Is{Not}EqualTo", PerSubject = true, ExpectedType = "{subject}",
		Summary = Matches, NegatedSummary = DoesNotMatch, Remarks = SameCollectionTypeRemarks)]
	internal static StringCollectionMatchResult<TCollection, IThat<TCollection>>
		IsEqualToForCollectionStringsCore<TCollection>(
			IThat<TCollection> subject,
			IEnumerable<string?> expected,
			string expectedExpression,
			bool negated)
		where TCollection : IEnumerable
	{
		StringEqualityOptions options = new();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringCollectionMatchResult<TCollection, IThat<TCollection>>(
			expectationBuilder.AddConstraint<TCollection?>((it, grammars) =>
			{
				IsEqualToForEnumerableConstraint<TCollection, string?, string?> constraint = new(
					expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected, options, matchOptions);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options,
			matchOptions);
	}

	[CreateCollectionExpectation("Is{Not}EqualTo", PerSubject = true,
		Factory = typeof(ObjectEqualityWithToleranceOptionsFactory),
		Summary = Matches, NegatedSummary = DoesNotMatch)]
	internal static ObjectCollectionMatchWithToleranceResult<TCollection, IThat<TCollection>, TItem, TTolerance>
		IsEqualToWithToleranceForCollectionCore<TCollection, TItem, TTolerance>(
			IThat<TCollection> subject,
			IEnumerable<TItem> expected,
			ObjectEqualityWithToleranceOptions<TItem, TTolerance> options,
			string expectedExpression,
			bool negated)
		where TCollection : IEnumerable
	{
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<TCollection, IThat<TCollection>, TItem, TTolerance>(
			expectationBuilder.AddConstraint<TCollection?>((it, grammars) =>
			{
				IsEqualToForEnumerableConstraint<TCollection, TItem, TItem> constraint = new(
					expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected, options, matchOptions);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options,
			matchOptions);
	}

	[CreateCollectionExpectation("Is{Not}EqualTo", PerSubject = true, Priority = -1,
		Summary = MatchesPredicates, NegatedSummary = DoesNotMatchPredicates,
		Remarks = BelowCollectionPriorityRemarks)]
	internal static CollectionMatchResult<TCollection, IThat<TCollection>, TItem>
		IsEqualToFromPredicatesForCollectionCore<TCollection, TItem>(
			IThat<TCollection> subject,
			IEnumerable<Expression<Func<TItem, bool>>> expected,
			string expectedExpression,
			bool negated)
		where TCollection : IEnumerable<TItem>
	{
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionMatchResult<TCollection, IThat<TCollection>, TItem>(
			expectationBuilder.AddConstraint<TCollection?>((it, grammars) =>
			{
				IsEqualToFromPredicateConstraint<TCollection, TItem, TItem> constraint = new(
					expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected, matchOptions);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			matchOptions);
	}

	[CreateCollectionExpectation("Is{Not}EqualTo", PerSubject = true, Priority = -1,
		Summary = MatchesExpectations, NegatedSummary = DoesNotMatchExpectations,
		Remarks = BelowCollectionPriorityRemarks)]
	internal static CollectionMatchResult<TCollection, IThat<TCollection>, TItem>
		IsEqualToFromExpectationsForCollectionCore<TCollection, TItem>(
			IThat<TCollection> subject,
			IEnumerable<Action<IThatSubject<TItem?>>> expected,
			string expectedExpression,
			bool negated)
		where TCollection : IEnumerable<TItem>
	{
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionMatchResult<TCollection, IThat<TCollection>, TItem>(
			expectationBuilder.AddConstraint<TCollection?>((it, grammars) =>
			{
				IsEqualToFromExpectationsConstraint<TCollection, TItem, TItem> constraint = new(
					expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected, matchOptions);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			matchOptions);
	}
}
