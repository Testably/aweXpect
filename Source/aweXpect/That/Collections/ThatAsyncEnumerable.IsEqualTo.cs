#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.SourceGenerators;

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	private const string Matches =
		"Verifies that the collection matches the <paramref name=\"expected\" /> collection.";

	private const string DoesNotMatch =
		"Verifies that the collection does not match the <paramref name=\"unexpected\" /> collection.";

	[CreateExpectationFamily("Is{Not}EqualTo", Summary = Matches, NegatedSummary = DoesNotMatch)]
	internal static ObjectCollectionMatchResult<IAsyncEnumerable<TItem>?, IThat<IAsyncEnumerable<TItem>?>, TItem>
		IsEqualToCore<TItem>(
			IThat<IAsyncEnumerable<TItem>?> subject,
			IEnumerable<TItem>? expected,
			string expectedExpression,
			bool negated)
	{
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchResult<IAsyncEnumerable<TItem>?, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint(
				(ExpectedExpression: expectedExpression, Expected: expected, Options: options,
					MatchOptions: matchOptions, Negated: negated),
				static (state, it, grammars) =>
				{
					AsyncIsEqualToConstraint<TItem, TItem> constraint = new(it, grammars,
						state.ExpectedExpression.TrimCommonWhiteSpace(), state.Expected, state.Options,
						state.MatchOptions);
					return state.Negated ? constraint.Invert() : constraint;
				}),
			subject,
			options,
			matchOptions);
	}

	[CreateExpectationFamily("Is{Not}EqualTo", Summary = Matches, NegatedSummary = DoesNotMatch)]
	internal static StringCollectionMatchResult<IAsyncEnumerable<string?>?, IThat<IAsyncEnumerable<string?>?>>
		IsEqualToForStringsCore(
			IThat<IAsyncEnumerable<string?>?> subject,
			IEnumerable<string?>? expected,
			string expectedExpression,
			bool negated)
	{
		StringEqualityOptions options = new(negated ? "unexpected" : "expected");
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringCollectionMatchResult<IAsyncEnumerable<string?>?, IThat<IAsyncEnumerable<string?>?>>(
			expectationBuilder.AddConstraint(
				(ExpectedExpression: expectedExpression, Expected: expected, Options: options,
					MatchOptions: matchOptions, Negated: negated),
				static (state, it, grammars) =>
				{
					AsyncIsEqualToConstraint<string?, string?> constraint = new(it, grammars,
						state.ExpectedExpression.TrimCommonWhiteSpace(), state.Expected, state.Options,
						state.MatchOptions);
					return state.Negated ? constraint.Invert() : constraint;
				}),
			subject,
			options,
			matchOptions);
	}

	[CreateExpectationFamily("Is{Not}EqualTo", Factory = typeof(ObjectEqualityWithToleranceOptionsFactory),
		Summary = Matches, NegatedSummary = DoesNotMatch)]
	internal static ObjectCollectionMatchWithToleranceResult<IAsyncEnumerable<TItem>?,
			IThat<IAsyncEnumerable<TItem>?>, TItem, TTolerance>
		IsEqualToWithToleranceCore<TItem, TTolerance>(
			IThat<IAsyncEnumerable<TItem>?> subject,
			IEnumerable<TItem>? expected,
			ObjectEqualityWithToleranceOptions<TItem, TTolerance> options,
			string expectedExpression,
			bool negated)
	{
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IAsyncEnumerable<TItem>?, IThat<IAsyncEnumerable<TItem>?>,
			TItem, TTolerance>(
			expectationBuilder.AddConstraint(
				(ExpectedExpression: expectedExpression, Expected: expected, Options: options,
					MatchOptions: matchOptions, Negated: negated),
				static (state, it, grammars) =>
				{
					AsyncIsEqualToConstraint<TItem, TItem> constraint = new(it, grammars,
						state.ExpectedExpression.TrimCommonWhiteSpace(), state.Expected, state.Options,
						state.MatchOptions);
					return state.Negated ? constraint.Invert() : constraint;
				}),
			subject,
			options,
			matchOptions);
	}

	[CreateExpectationFamily("Is{Not}EqualTo",
		Summary = "Verifies that the collection matches the <paramref name=\"expected\" /> collection of predicates.",
		NegatedSummary =
			"Verifies that the collection does not match the <paramref name=\"unexpected\" /> collection of predicates.")]
	internal static CollectionMatchResult<IAsyncEnumerable<TItem>?, IThat<IAsyncEnumerable<TItem>?>, TItem>
		IsEqualToFromPredicatesCore<TItem>(
			IThat<IAsyncEnumerable<TItem>?> subject,
			IEnumerable<Expression<Func<TItem, bool>>>? expected,
			string expectedExpression,
			bool negated)
	{
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionMatchResult<IAsyncEnumerable<TItem>?, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint(
				(ExpectedExpression: expectedExpression, Expected: expected, MatchOptions: matchOptions,
					Negated: negated),
				static (state, it, grammars) =>
				{
					AsyncIsEqualToFromPredicateConstraint<TItem, TItem> constraint = new(it, grammars,
						state.ExpectedExpression.TrimCommonWhiteSpace(), state.Expected, state.MatchOptions);
					return state.Negated ? constraint.Invert() : constraint;
				}),
			subject,
			matchOptions);
	}

	[CreateExpectationFamily("Is{Not}EqualTo",
		Summary = "Verifies that the collection matches the <paramref name=\"expected\" /> collection of expectations.",
		NegatedSummary =
			"Verifies that the collection does not match the <paramref name=\"unexpected\" /> collection of expectations.")]
	internal static CollectionMatchResult<IAsyncEnumerable<TItem>?, IThat<IAsyncEnumerable<TItem>?>, TItem>
		IsEqualToFromExpectationsCore<TItem>(
			IThat<IAsyncEnumerable<TItem>?> subject,
			IEnumerable<Action<IThatSubject<TItem?>>>? expected,
			string expectedExpression,
			bool negated)
	{
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionMatchResult<IAsyncEnumerable<TItem>?, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint(
				(ExpectedExpression: expectedExpression, Expected: expected, MatchOptions: matchOptions,
					Negated: negated),
				static (state, it, grammars) =>
				{
					AsyncIsEqualToFromExpectationsConstraint<TItem, TItem> constraint = new(it, grammars,
						state.ExpectedExpression.TrimCommonWhiteSpace(), state.Expected, state.MatchOptions);
					return state.Negated ? constraint.Invert() : constraint;
				}),
			subject,
			matchOptions);
	}
}
#endif
