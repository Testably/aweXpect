using System.Collections;
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

public static partial class ThatEnumerable
{
	private const string EndsWithSummary =
		"Verifies that the collection ends with the provided <paramref name=\"expected\" /> collection.";

	private const string DoesNotEndWithSummary =
		"Verifies that the collection does not end with the provided <paramref name=\"unexpected\" /> collection.";

	[CreateExpectationFamily("EndsWith", NegatedName = "DoesNotEndWith", GuaranteesNotNull = true,
		Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary, Remarks = SetComparerRemarks)]
	[CreateExpectationFamily("EndsWith", NegatedName = "DoesNotEndWith", GuaranteesNotNull = true,
		Params = true, Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary,
		Remarks = SetComparerRemarks)]
	internal static ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		EndsWithCore<TItem>(
			IThat<IEnumerable<TItem>?> subject,
			IEnumerable<TItem> expected,
			string? expectedExpression,
			bool negated)
	{
		IEnumerable<TItem> expectedValues = expected.ToNonEmptyValues(negated);
		ItemEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((Options: options, ExpectedExpression: expectedExpression,
					ExpectedValues: expectedValues, Negated: negated),
				static (state, it, grammars) =>
				{
					SubjectEqualityOptions<TItem, TItem> itemOptions =
						new(state.Options, () => state.Options.HasDefaultMatchType);
					EndsWithConstraint<IEnumerable<TItem>?, TItem, TItem> constraint = new(it, grammars,
						state.ExpectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(state.ExpectedValues),
						state.ExpectedValues.ToArray(), itemOptions, itemOptions.UseComparerOf);
					return state.Negated ? constraint.Invert() : constraint;
				}),
			subject,
			options);
	}

	[CreateExpectationFamily("EndsWith", NegatedName = "DoesNotEndWith", GuaranteesNotNull = true,
		Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary, Remarks = SetComparerRemarks)]
	[CreateExpectationFamily("EndsWith", NegatedName = "DoesNotEndWith", GuaranteesNotNull = true,
		Params = true, ExpectedType = "string?",
		Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary, Remarks = SetComparerRemarks)]
	internal static StringEqualityTypeResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>
		EndsWithForStringsCore(
			IThat<IEnumerable<string?>?> subject,
			IEnumerable<string?> expected,
			string? expectedExpression,
			bool negated)
	{
		IEnumerable<string?> expectedValues = expected.ToNonEmptyValues(negated);
		StringEqualityOptions options = new(negated ? "unexpected" : "expected");
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringEqualityTypeResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>(
			expectationBuilder.AddConstraint((Options: options, ExpectedExpression: expectedExpression,
					ExpectedValues: expectedValues, Negated: negated),
				static (state, it, grammars) =>
				{
					SubjectEqualityOptions<string?, string?> itemOptions =
						new(state.Options, () => state.Options.ComparesByOrdinalEquality);
					EndsWithConstraint<IEnumerable<string?>?, string?, string?> constraint = new(it, grammars,
						state.ExpectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(state.ExpectedValues),
						state.ExpectedValues.ToArray(), itemOptions, itemOptions.UseComparerOf);
					return state.Negated ? constraint.Invert() : constraint;
				}),
			subject,
			options);
	}

	[CreateExpectationFamily("EndsWith", NegatedName = "DoesNotEndWith", GuaranteesNotNull = true,
		Factory = typeof(ObjectEqualityWithToleranceOptionsFactory),
		Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary, Remarks = SetComparerRemarks)]
	[CreateExpectationFamily("EndsWith", NegatedName = "DoesNotEndWith", GuaranteesNotNull = true,
		Factory = typeof(ObjectEqualityWithToleranceOptionsFactory), Params = true,
		Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary, Remarks = SetComparerRemarks)]
	internal static ObjectEqualityWithToleranceResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem, TTolerance>
		EndsWithWithToleranceCore<TItem, TTolerance>(
			IThat<IEnumerable<TItem>?> subject,
			IEnumerable<TItem> expected,
			ObjectEqualityWithToleranceOptions<TItem, TTolerance> options,
			string? expectedExpression,
			bool negated)
	{
		IEnumerable<TItem> expectedValues = expected.ToNonEmptyValues(negated);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityWithToleranceResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem, TTolerance>(
			expectationBuilder.AddConstraint((Options: options, ExpectedExpression: expectedExpression,
					ExpectedValues: expectedValues, Negated: negated),
				static (state, it, grammars) =>
				{
					SubjectEqualityOptions<TItem, TItem> itemOptions = new(state.Options,
						() => ObjectEqualityWithToleranceOptionsFactory.HasDefaultMatchType(state.Options));
					EndsWithConstraint<IEnumerable<TItem>?, TItem, TItem> constraint = new(it, grammars,
						state.ExpectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(state.ExpectedValues),
						state.ExpectedValues.ToArray(), itemOptions, itemOptions.UseComparerOf);
					return state.Negated ? constraint.Invert() : constraint;
				}),
			subject,
			options);
	}

	[CreateExpectationFamily("EndsWith", NegatedName = "DoesNotEndWith", GuaranteesNotNull = true,
		Priority = -1, Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary,
		Remarks = UntypedSetComparerRemarks)]
	[CreateExpectationFamily("EndsWith", NegatedName = "DoesNotEndWith", GuaranteesNotNull = true,
		Params = true, Priority = -2, Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary,
		Remarks = LowerPriorityRemarks + "\n" + UntypedSetComparerRemarks)]
	internal static ObjectEqualityResult<IEnumerable, IThat<IEnumerable?>, TItem>
		EndsWithForEnumerableCore<TItem>(
			IThat<IEnumerable?> subject,
			IEnumerable<TItem> expected,
			string? expectedExpression,
			bool negated)
	{
		IEnumerable<TItem> expectedValues = expected.ToNonEmptyValues(negated);
		ItemEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IEnumerable, IThat<IEnumerable?>, TItem>(
			expectationBuilder.AddConstraint((Options: options, ExpectedExpression: expectedExpression,
					ExpectedValues: expectedValues, Negated: negated),
				static (state, it, grammars) =>
				{
					SubjectEqualityOptions<TItem, TItem> itemOptions =
						new(state.Options, () => state.Options.HasDefaultMatchType);
					EndsWithConstraint<IEnumerable?, object?, TItem> constraint = new(
						it, grammars,
						state.ExpectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(state.ExpectedValues),
						state.ExpectedValues.ToArray(), itemOptions, itemOptions.UseComparerOf);
					return state.Negated ? constraint.Invert() : constraint;
				}),
			subject,
			options);
	}

	[CreateExpectationFamily("EndsWith", NegatedName = "DoesNotEndWith", GuaranteesNotNull = true,
		Priority = -1, Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary,
		Remarks = UntypedCollectionRemarks + "\n" + UntypedSetComparerRemarks)]
	internal static ObjectEqualityResult<IEnumerable, IThat<IEnumerable?>, object?>
		EndsWithForObjectsCore(
			IThat<IEnumerable?> subject,
			IEnumerable expected,
			string? expectedExpression,
			bool negated)
		=> EndsWithForEnumerableCore<object?>(subject, expected?.Cast<object?>()!, expectedExpression, negated);

	[CreateExpectationFamily("EndsWith", NegatedName = "DoesNotEndWith", GuaranteesNotNull = true,
		Priority = -1, ExpectedType = "string?",
		Summary = "Verifies that the collection ends with the provided <paramref name=\"expected\" /> value.",
		NegatedSummary =
			"Verifies that the collection does not end with the provided <paramref name=\"unexpected\" /> value.",
		Remarks = SingleValueRemarks + "\n" + UntypedSetComparerRemarks)]
	internal static ObjectEqualityResult<IEnumerable, IThat<IEnumerable?>, string?>
		EndsWithSingleStringCore(
			IThat<IEnumerable?> subject,
			string? expected,
			bool negated)
	{
		string?[] expectedItems = [expected,];
		ItemEqualityOptions<string?> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IEnumerable, IThat<IEnumerable?>, string?>(
			expectationBuilder.AddConstraint((Options: options, ExpectedItems: expectedItems, Negated: negated),
				static (state, it, grammars) =>
				{
					SubjectEqualityOptions<string?, string?> itemOptions =
						new(state.Options, () => state.Options.HasDefaultMatchType);
					EndsWithConstraint<IEnumerable?, object?, string?> constraint = new(
						it, grammars,
						Formatter.Format(state.ExpectedItems), state.ExpectedItems, itemOptions,
						itemOptions.UseComparerOf);
					return state.Negated ? constraint.Invert() : constraint;
				}),
			subject,
			options);
	}

	[CreateExpectationFamily("EndsWith", NegatedName = "DoesNotEndWith", PerSubject = true,
		Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary)]
	[CreateExpectationFamily("EndsWith", NegatedName = "DoesNotEndWith", PerSubject = true, Params = true,
		Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary)]
	internal static ObjectEqualityResult<TCollection, IThat<TCollection>, TItem>
		EndsWithForCollectionCore<TCollection, TItem>(
			IThat<TCollection> subject,
			IEnumerable<TItem> expected,
			string? expectedExpression,
			bool negated)
		where TCollection : IEnumerable
	{
		IEnumerable<TItem> expectedValues = expected.ToNonEmptyValues(negated);
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<TCollection, IThat<TCollection>, TItem>(
			expectationBuilder.AddConstraint((Options: options, ExpectedExpression: expectedExpression,
					ExpectedValues: expectedValues, Negated: negated),
				static (state, it, grammars) =>
				{
					EndsWithConstraint<TCollection?, object?, TItem> constraint = new(
						it, grammars,
						state.ExpectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(state.ExpectedValues),
						state.ExpectedValues.ToArray(), state.Options);
					return state.Negated ? constraint.Invert() : constraint;
				}),
			subject,
			options);
	}

	[CreateExpectationFamily("EndsWith", NegatedName = "DoesNotEndWith", PerSubject = true,
		Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary)]
	[CreateExpectationFamily("EndsWith", NegatedName = "DoesNotEndWith", PerSubject = true, Params = true,
		Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary)]
	internal static StringEqualityTypeResult<TCollection, IThat<TCollection>>
		EndsWithForCollectionStringsCore<TCollection>(
			IThat<TCollection> subject,
			IEnumerable<string?> expected,
			string? expectedExpression,
			bool negated)
		where TCollection : IEnumerable
	{
		IEnumerable<string?> expectedValues = expected.ToNonEmptyValues(negated);
		StringEqualityOptions options = new(negated ? "unexpected" : "expected");
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringEqualityTypeResult<TCollection, IThat<TCollection>>(
			expectationBuilder.AddConstraint((Options: options, ExpectedExpression: expectedExpression,
					ExpectedValues: expectedValues, Negated: negated),
				static (state, it, grammars) =>
				{
					EndsWithConstraint<TCollection?, object?, string?> constraint = new(
						it, grammars,
						state.ExpectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(state.ExpectedValues),
						state.ExpectedValues.ToArray(), state.Options);
					return state.Negated ? constraint.Invert() : constraint;
				}),
			subject,
			options);
	}

	[CreateExpectationFamily("EndsWith", NegatedName = "DoesNotEndWith", PerSubject = true,
		Factory = typeof(ObjectEqualityWithToleranceOptionsFactory),
		Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary)]
	[CreateExpectationFamily("EndsWith", NegatedName = "DoesNotEndWith", PerSubject = true,
		Factory = typeof(ObjectEqualityWithToleranceOptionsFactory), Params = true,
		Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary)]
	internal static ObjectEqualityWithToleranceResult<TCollection, IThat<TCollection>, TItem, TTolerance>
		EndsWithWithToleranceForCollectionCore<TCollection, TItem, TTolerance>(
			IThat<TCollection> subject,
			IEnumerable<TItem> expected,
			ObjectEqualityWithToleranceOptions<TItem, TTolerance> options,
			string? expectedExpression,
			bool negated)
		where TCollection : IEnumerable
	{
		IEnumerable<TItem> expectedValues = expected.ToNonEmptyValues(negated);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityWithToleranceResult<TCollection, IThat<TCollection>, TItem, TTolerance>(
			expectationBuilder.AddConstraint((Options: options, ExpectedExpression: expectedExpression,
					ExpectedValues: expectedValues, Negated: negated),
				static (state, it, grammars) =>
				{
					EndsWithConstraint<TCollection?, object?, TItem> constraint = new(
						it, grammars,
						state.ExpectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(state.ExpectedValues),
						state.ExpectedValues.ToArray(), state.Options);
					return state.Negated ? constraint.Invert() : constraint;
				}),
			subject,
			options);
	}
}
