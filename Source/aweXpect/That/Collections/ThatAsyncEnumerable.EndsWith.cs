#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.SourceGenerators;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	private const string EndsWithSummary =
		"Verifies that the collection ends with the provided <paramref name=\"expected\" /> collection.";

	private const string DoesNotEndWithSummary =
		"Verifies that the collection does not end with the provided <paramref name=\"unexpected\" /> collection.";

	[CreateExpectationFamily("EndsWith", NegatedName = "DoesNotEndWith", GuaranteesNotNull = true,
		Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary)]
	[CreateExpectationFamily("EndsWith", NegatedName = "DoesNotEndWith", GuaranteesNotNull = true,
		Params = true, Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary)]
	internal static ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		EndsWithCore<TItem>(
			IThat<IAsyncEnumerable<TItem>?> subject,
			IEnumerable<TItem> expected,
			string? expectedExpression,
			bool negated)
	{
		IEnumerable<TItem> expectedValues = expected.ToNonEmptyValues(negated);
		OptionsHolder<ObjectMatchType<TItem>> options = new(ObjectMatchType<TItem>.Default);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint(
				(Options: options, ExpectedExpression: expectedExpression, ExpectedValues: expectedValues,
					Negated: negated),
				static (state, it, grammars) =>
				{
					AsyncEndsWithConstraint<TItem, TItem> constraint = new(it, grammars, state.Negated ? "unexpected" : "expected",
						state.ExpectedExpression?.TrimCommonWhiteSpace(), state.ExpectedValues,
						state.ExpectedValues.ToArray(), state.Options);
					return state.Negated ? constraint.Invert() : constraint;
				}),
			subject,
			options);
	}

	[CreateExpectationFamily("EndsWith", NegatedName = "DoesNotEndWith", GuaranteesNotNull = true,
		Factory = typeof(ObjectToleranceMatchTypeFactory),
		Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary)]
	[CreateExpectationFamily("EndsWith", NegatedName = "DoesNotEndWith", GuaranteesNotNull = true,
		Factory = typeof(ObjectToleranceMatchTypeFactory), Params = true,
		Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary)]
	internal static ObjectEqualityWithToleranceResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem,
			TTolerance>
		EndsWithWithToleranceCore<TItem, TTolerance>(
			IThat<IAsyncEnumerable<TItem>?> subject,
			IEnumerable<TItem> expected,
			ObjectToleranceMatchType<TItem, TTolerance> initialOptions,
			string? expectedExpression,
			bool negated)
	{
		OptionsHolder<ObjectMatchType<TItem>> options = new(initialOptions);
		IEnumerable<TItem> expectedValues = expected.ToNonEmptyValues(negated);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityWithToleranceResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem,
			TTolerance>(
			expectationBuilder.AddConstraint(
				(Options: options, ExpectedExpression: expectedExpression, ExpectedValues: expectedValues,
					Negated: negated),
				static (state, it, grammars) =>
				{
					AsyncEndsWithConstraint<TItem, TItem> constraint = new(it, grammars, state.Negated ? "unexpected" : "expected",
						state.ExpectedExpression?.TrimCommonWhiteSpace(), state.ExpectedValues,
						state.ExpectedValues.ToArray(), state.Options);
					return state.Negated ? constraint.Invert() : constraint;
				}),
			subject,
			options);
	}

	[CreateExpectationFamily("EndsWith", NegatedName = "DoesNotEndWith", GuaranteesNotNull = true,
		Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary)]
	[CreateExpectationFamily("EndsWith", NegatedName = "DoesNotEndWith", GuaranteesNotNull = true,
		Params = true, ExpectedType = "string?",
		Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary)]
	internal static StringEqualityTypeResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>
		EndsWithForStringsCore(
			IThat<IAsyncEnumerable<string?>?> subject,
			IEnumerable<string?> expected,
			string? expectedExpression,
			bool negated)
	{
		IEnumerable<string?> expectedValues = expected.ToNonEmptyValues(negated);
		OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringEqualityTypeResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>(
			expectationBuilder.AddConstraint(
				(Options: options, ExpectedExpression: expectedExpression, ExpectedValues: expectedValues,
					Negated: negated),
				static (state, it, grammars) =>
				{
					AsyncEndsWithConstraint<string?, string?> constraint = new(it, grammars, state.Negated ? "unexpected" : "expected",
						state.ExpectedExpression?.TrimCommonWhiteSpace(), state.ExpectedValues,
						state.ExpectedValues.ToArray(), state.Options);
					return state.Negated ? constraint.Invert() : constraint;
				}),
			subject,
			options);
	}
}
#endif
