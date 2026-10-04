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
	private const string StartsWithSummary =
		"Verifies that the collection starts with the provided <paramref name=\"expected\" /> collection.";

	private const string DoesNotStartWithSummary =
		"Verifies that the collection does not start with the provided <paramref name=\"unexpected\" /> collection.";

	private const string SingleValueRemarks =
		"Without this overload a <see cref=\"string\" /> argument would bind to the collection overload and be\n" +
		"expected as a sequence of characters.";

	private const string LowerPriorityRemarks =
		"The priority is below the one of the collection overload, so that a collection argument binds as the\n" +
		"expected sequence instead of as a single expected item.";

	private const string UntypedCollectionRemarks =
		"Without this overload a collection argument without an item type would bind to the <c>params</c> overload\n" +
		"and be expected as a single item. The priority only takes effect with C# 13 or later.";

	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", GuaranteesNotNull = true,
		Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary, Remarks = SetComparerRemarks)]
	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", GuaranteesNotNull = true,
		Params = true, Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary,
		Remarks = SetComparerRemarks)]
	internal static ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		StartsWithCore<TItem>(
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
						new(state.Options);
					StartsWithConstraint<IEnumerable<TItem>?, TItem, TItem> constraint = new(it, grammars,
						state.ExpectedExpression?.TrimCommonWhiteSpace(), state.ExpectedValues,
						state.ExpectedValues.ToArray(), itemOptions, itemOptions);
					return state.Negated ? constraint.Invert() : constraint;
				}),
			subject,
			options);
	}

	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", GuaranteesNotNull = true,
		Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary, Remarks = SetComparerRemarks)]
	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", GuaranteesNotNull = true,
		Params = true, ExpectedType = "string?",
		Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary, Remarks = SetComparerRemarks)]
	internal static StringEqualityTypeResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>
		StartsWithForStringsCore(
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
						new(state.Options);
					StartsWithConstraint<IEnumerable<string?>?, string?, string?> constraint = new(it, grammars,
						state.ExpectedExpression?.TrimCommonWhiteSpace(), state.ExpectedValues,
						state.ExpectedValues.ToArray(), itemOptions, itemOptions);
					return state.Negated ? constraint.Invert() : constraint;
				}),
			subject,
			options);
	}

	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", GuaranteesNotNull = true,
		Factory = typeof(ObjectEqualityWithToleranceOptionsFactory),
		Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary, Remarks = SetComparerRemarks)]
	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", GuaranteesNotNull = true,
		Factory = typeof(ObjectEqualityWithToleranceOptionsFactory), Params = true,
		Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary, Remarks = SetComparerRemarks)]
	internal static ObjectEqualityWithToleranceResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem, TTolerance>
		StartsWithWithToleranceCore<TItem, TTolerance>(
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
					SubjectEqualityOptions<TItem, TItem> itemOptions = new(state.Options);
					StartsWithConstraint<IEnumerable<TItem>?, TItem, TItem> constraint = new(it, grammars,
						state.ExpectedExpression?.TrimCommonWhiteSpace(), state.ExpectedValues,
						state.ExpectedValues.ToArray(), itemOptions, itemOptions);
					return state.Negated ? constraint.Invert() : constraint;
				}),
			subject,
			options);
	}

	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", GuaranteesNotNull = true,
		Priority = -1, Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary,
		Remarks = UntypedSetComparerRemarks)]
	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", GuaranteesNotNull = true,
		Params = true, Priority = -2, Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary,
		Remarks = LowerPriorityRemarks + "\n" + UntypedSetComparerRemarks)]
	internal static ObjectEqualityResult<IEnumerable, IThat<IEnumerable?>, TItem>
		StartsWithForEnumerableCore<TItem>(
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
						new(state.Options);
					StartsWithConstraint<IEnumerable?, object?, TItem> constraint = new(
						it, grammars,
						state.ExpectedExpression?.TrimCommonWhiteSpace(), state.ExpectedValues,
						state.ExpectedValues.ToArray(), itemOptions, itemOptions);
					return state.Negated ? constraint.Invert() : constraint;
				}),
			subject,
			options);
	}

	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", GuaranteesNotNull = true,
		Priority = -1, Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary,
		Remarks = UntypedCollectionRemarks + "\n" + UntypedSetComparerRemarks)]
	internal static ObjectEqualityResult<IEnumerable, IThat<IEnumerable?>, object?>
		StartsWithForObjectsCore(
			IThat<IEnumerable?> subject,
			IEnumerable expected,
			string? expectedExpression,
			bool negated)
		=> StartsWithForEnumerableCore<object?>(subject, expected?.Cast<object?>()!, expectedExpression, negated);

	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", GuaranteesNotNull = true,
		Priority = -1, ExpectedType = "string?",
		Summary = "Verifies that the collection starts with the provided <paramref name=\"expected\" /> value.",
		NegatedSummary =
			"Verifies that the collection does not start with the provided <paramref name=\"unexpected\" /> value.",
		Remarks = SingleValueRemarks + "\n" + UntypedSetComparerRemarks)]
	internal static ObjectEqualityResult<IEnumerable, IThat<IEnumerable?>, string?>
		StartsWithSingleStringCore(
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
						new(state.Options);
					StartsWithConstraint<IEnumerable?, object?, string?> constraint = new(
						it, grammars,
						null, state.ExpectedItems, state.ExpectedItems, itemOptions,
						itemOptions);
					return state.Negated ? constraint.Invert() : constraint;
				}),
			subject,
			options);
	}

	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", PerSubject = true,
		Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary)]
	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", PerSubject = true, Params = true,
		Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary)]
	internal static ObjectEqualityResult<TCollection, IThat<TCollection>, TItem>
		StartsWithForCollectionCore<TCollection, TItem>(
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
					StartsWithConstraint<TCollection?, object?, TItem> constraint = new(
						it, grammars,
						state.ExpectedExpression?.TrimCommonWhiteSpace(), state.ExpectedValues,
						state.ExpectedValues.ToArray(), state.Options);
					return state.Negated ? constraint.Invert() : constraint;
				}),
			subject,
			options);
	}

	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", PerSubject = true,
		Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary)]
	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", PerSubject = true, Params = true,
		Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary)]
	internal static StringEqualityTypeResult<TCollection, IThat<TCollection>>
		StartsWithForCollectionStringsCore<TCollection>(
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
					StartsWithConstraint<TCollection?, object?, string?> constraint = new(
						it, grammars,
						state.ExpectedExpression?.TrimCommonWhiteSpace(), state.ExpectedValues,
						state.ExpectedValues.ToArray(), state.Options);
					return state.Negated ? constraint.Invert() : constraint;
				}),
			subject,
			options);
	}


	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", PerSubject = true,
		Factory = typeof(ObjectEqualityWithToleranceOptionsFactory),
		Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary)]
	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", PerSubject = true,
		Factory = typeof(ObjectEqualityWithToleranceOptionsFactory), Params = true,
		Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary)]
	internal static ObjectEqualityWithToleranceResult<TCollection, IThat<TCollection>, TItem, TTolerance>
		StartsWithWithToleranceForCollectionCore<TCollection, TItem, TTolerance>(
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
					StartsWithConstraint<TCollection?, object?, TItem> constraint = new(
						it, grammars,
						state.ExpectedExpression?.TrimCommonWhiteSpace(), state.ExpectedValues,
						state.ExpectedValues.ToArray(), state.Options);
					return state.Negated ? constraint.Invert() : constraint;
				}),
			subject,
			options);
	}
}
