#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Linq;
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
	private const string StartsWithSummary =
		"Verifies that the collection starts with the provided <paramref name=\"expected\" /> collection.";

	private const string DoesNotStartWithSummary =
		"Verifies that the collection does not start with the provided <paramref name=\"unexpected\" /> collection.";

	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", GuaranteesNotNull = true,
		Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary)]
	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", GuaranteesNotNull = true,
		Params = true, Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary)]
	internal static ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		StartsWithCore<TItem>(
			IThat<IAsyncEnumerable<TItem>?> subject,
			IEnumerable<TItem> expected,
			string? expectedExpression,
			bool negated)
	{
		IEnumerable<TItem> expectedValues = expected.ToNonEmptyValues(negated);
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint(
				(Options: options, ExpectedExpression: expectedExpression, ExpectedValues: expectedValues,
					Negated: negated),
				static (state, it, grammars) =>
				{
					AsyncStartsWithConstraint<TItem, TItem> constraint = new(it, grammars,
						state.ExpectedExpression?.TrimCommonWhiteSpace(), state.ExpectedValues,
						state.ExpectedValues.ToArray(), state.Options);
					return state.Negated ? constraint.Invert() : constraint;
				}),
			subject,
			options);
	}

	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", GuaranteesNotNull = true,
		Factory = typeof(ObjectEqualityWithToleranceOptionsFactory),
		Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary)]
	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", GuaranteesNotNull = true,
		Factory = typeof(ObjectEqualityWithToleranceOptionsFactory), Params = true,
		Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary)]
	internal static ObjectEqualityWithToleranceResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem,
			TTolerance>
		StartsWithWithToleranceCore<TItem, TTolerance>(
			IThat<IAsyncEnumerable<TItem>?> subject,
			IEnumerable<TItem> expected,
			ObjectEqualityWithToleranceOptions<TItem, TTolerance> options,
			string? expectedExpression,
			bool negated)
	{
		IEnumerable<TItem> expectedValues = expected.ToNonEmptyValues(negated);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityWithToleranceResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem,
			TTolerance>(
			expectationBuilder.AddConstraint(
				(Options: options, ExpectedExpression: expectedExpression, ExpectedValues: expectedValues,
					Negated: negated),
				static (state, it, grammars) =>
				{
					AsyncStartsWithConstraint<TItem, TItem> constraint = new(it, grammars,
						state.ExpectedExpression?.TrimCommonWhiteSpace(), state.ExpectedValues,
						state.ExpectedValues.ToArray(), state.Options);
					return state.Negated ? constraint.Invert() : constraint;
				}),
			subject,
			options);
	}

	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", GuaranteesNotNull = true,
		Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary)]
	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", GuaranteesNotNull = true,
		Params = true, ExpectedType = "string?",
		Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary)]
	internal static StringEqualityTypeResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>
		StartsWithForStringsCore(
			IThat<IAsyncEnumerable<string?>?> subject,
			IEnumerable<string?> expected,
			string? expectedExpression,
			bool negated)
	{
		IEnumerable<string?> expectedValues = expected.ToNonEmptyValues(negated);
		StringEqualityOptions options = new(negated ? "unexpected" : "expected");
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringEqualityTypeResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>(
			expectationBuilder.AddConstraint(
				(Options: options, ExpectedExpression: expectedExpression, ExpectedValues: expectedValues,
					Negated: negated),
				static (state, it, grammars) =>
				{
					AsyncStartsWithConstraint<string?, string?> constraint = new(it, grammars,
						state.ExpectedExpression?.TrimCommonWhiteSpace(), state.ExpectedValues,
						state.ExpectedValues.ToArray(), state.Options);
					return state.Negated ? constraint.Invert() : constraint;
				}),
			subject,
			options);
	}
}
#endif
