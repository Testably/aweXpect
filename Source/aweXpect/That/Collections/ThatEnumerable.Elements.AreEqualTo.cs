using System.Collections;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.SourceGenerators;

namespace aweXpect;

public static partial class ThatEnumerable
{
	private const string ElementsAreEqualTo = "…are equal to the <paramref name=\"expected\" /> value.";

	[CreateExpectationFamily("AreEqualTo", Factory = typeof(ObjectEqualityWithToleranceOptionsFactory),
		Summary = ElementsAreEqualTo, Remarks = SetItemComparerRemarks)]
	internal static ObjectEqualityWithToleranceResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem, TTolerance>
		AreEqualToWithToleranceCore<TItem, TTolerance>(
			Elements<TItem> elements,
			TItem expected,
			ObjectEqualityWithToleranceOptions<TItem, TTolerance> options)
	{
		IEnumerableElements<TItem> iElements = elements;
		ExpectationBuilder expectationBuilder = iElements.Subject.Get().ExpectationBuilder;
		return new ObjectEqualityWithToleranceResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem, TTolerance>(
			expectationBuilder.AddConstraint((Options: options, Expected: expected, Quantifier: iElements.Quantifier),
				static (state, it, grammars) =>
				{
					SubjectEqualityOptions<TItem, TItem> itemOptions = new(state.Options, state.Expected is not null);
					return new CollectionConstraint<IEnumerable<TItem>?, TItem>(
						it, grammars,
						state.Quantifier,
						new ElementEqualTo<TItem, TItem>(itemOptions, state.Expected),
						"were");
				}),
			iElements.Subject,
			options);
	}

	[CreateExpectationFamily("AreEqualTo", Summary = ElementsAreEqualTo, Remarks = SetItemComparerRemarks)]
	internal static ObjectEqualityResult<IEnumerable<TItem>?, IThat<IEnumerable<TItem>?>, TItem>
		AreEqualToCore<TItem>(
			Elements<TItem> elements,
			TItem expected)
	{
		IEnumerableElements<TItem> iElements = elements;
		ItemEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = iElements.Subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IEnumerable<TItem>?, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((Options: options, Expected: expected, Quantifier: iElements.Quantifier),
				static (state, it, grammars) =>
				{
					SubjectEqualityOptions<TItem, TItem> itemOptions =
						new(state.Options, state.Expected is not null);
					return new CollectionConstraint<IEnumerable<TItem>?, TItem>(
						it, grammars,
						state.Quantifier,
						new ElementEqualTo<TItem, TItem>(itemOptions, state.Expected, state.Options),
						"were");
				}),
			iElements.Subject,
			options);
	}

	[CreateExpectationFamily("AreEqualTo", Priority = -1, Summary = ElementsAreEqualTo,
		Remarks = UntypedSetComparerRemarks)]
	internal static ObjectEqualityResult<IEnumerable, IThat<IEnumerable?>, object?>
		AreEqualToForEnumerableCore(
			ElementsForEnumerable<IEnumerable> elements,
			object? expected)
	{
		INonGenericEnumerableElements<IEnumerable> iElements = elements;
		ItemEqualityOptions<object?> options = new();
		ExpectationBuilder expectationBuilder = iElements.Subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IEnumerable, IThat<IEnumerable?>, object?>(
			expectationBuilder.AddConstraint((Options: options, Expected: expected, Quantifier: iElements.Quantifier),
				static (state, it, grammars) =>
				{
					SubjectEqualityOptions<object?, object?> itemOptions =
						new(state.Options, state.Expected is not null);
					return new CollectionConstraint<IEnumerable, object?>(
						it, grammars,
						state.Quantifier,
						new ElementEqualTo<object?, object?>(itemOptions, state.Expected, state.Options),
						"were");
				}),
			iElements.Subject,
			options);
	}

	[CreateExpectationFamily("AreEqualTo", Summary = ElementsAreEqualTo)]
	internal static ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, object?>
		AreEqualToForStructCore<TEnumerable, TItem>(
			ElementsForStructEnumerable<TEnumerable, TItem> elements,
			object? expected)
		where TEnumerable : struct, IEnumerable<TItem>
	{
		IStructEnumerableElements<TEnumerable, TItem> iElements = elements;
		ObjectEqualityOptions<object?> options = new();
		ExpectationBuilder expectationBuilder = iElements.Subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, object?>(
			expectationBuilder.AddConstraint((Quantifier: iElements.Quantifier, Expected: expected, Options: options),
				static (state, it, grammars)
					=> new CollectionConstraint<TEnumerable, object?>(
						it, grammars,
						state.Quantifier,
						new ElementEqualTo<object?, object?>(state.Options, state.Expected, state.Options),
						"were")),
			iElements.Subject,
			options);
	}

	[CreateExpectationFamily("AreEqualTo", Factory = typeof(ObjectEqualityWithToleranceOptionsFactory),
		Summary = ElementsAreEqualTo)]
	internal static ObjectEqualityWithToleranceResult<TEnumerable, IThat<TEnumerable>, TItem, TTolerance>
		AreEqualToWithToleranceForStructCore<TEnumerable, TItem, TTolerance>(
			ElementsForStructEnumerable<TEnumerable, TItem> elements,
			TItem expected,
			ObjectEqualityWithToleranceOptions<TItem, TTolerance> options)
		where TEnumerable : struct, IEnumerable<TItem>
	{
		IStructEnumerableElements<TEnumerable, TItem> iElements = elements;
		ExpectationBuilder expectationBuilder = iElements.Subject.Get().ExpectationBuilder;
		return new ObjectEqualityWithToleranceResult<TEnumerable, IThat<TEnumerable>, TItem, TTolerance>(
			expectationBuilder.AddConstraint((Quantifier: iElements.Quantifier, Expected: expected, Options: options),
				static (state, it, grammars)
					=> new CollectionConstraint<TEnumerable, object?>(
						it, grammars,
						state.Quantifier,
						new ElementEqualTo<object?, TItem>(state.Options, state.Expected),
						"were")),
			iElements.Subject,
			options);
	}

	[CreateExpectationFamily("AreEqualTo", Summary = ElementsAreEqualTo, Remarks = SetItemComparerRemarks)]
	internal static StringEqualityTypeResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>
		AreEqualToForStringsCore(
			Elements elements,
			string? expected)
	{
		IEnumerableStringElements iElements = elements;
		StringEqualityOptions options = new(nameof(expected));
		ExpectationBuilder expectationBuilder = iElements.Subject.Get().ExpectationBuilder;
		return new StringEqualityTypeResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>(
			expectationBuilder.AddConstraint((Options: options, Expected: expected, Quantifier: iElements.Quantifier),
				static (state, it, grammars) =>
				{
					SubjectEqualityOptions<string?, string?> itemOptions =
						new(state.Options, state.Expected is not null);
					return new CollectionConstraint<IEnumerable<string?>?, string?>(
						it, grammars,
						state.Quantifier,
						new ElementEqualToString<string?>(state.Options, state.Expected, itemOptions),
						"were");
				}),
			iElements.Subject,
			options);
	}

	[CreateExpectationFamily("AreEqualTo", Summary = ElementsAreEqualTo)]
	internal static StringEqualityTypeResult<TEnumerable, IThat<TEnumerable>>
		AreEqualToForStructStringsCore<TEnumerable>(
			ElementsForStructEnumerable<TEnumerable> elements,
			string? expected)
		where TEnumerable : struct, IEnumerable<string?>
	{
		IStructEnumerableStringElements<TEnumerable> iElements = elements;
		StringEqualityOptions options = new(nameof(expected));
		ExpectationBuilder expectationBuilder = iElements.Subject.Get().ExpectationBuilder;
		return new StringEqualityTypeResult<TEnumerable, IThat<TEnumerable>>(
			expectationBuilder.AddConstraint((Quantifier: iElements.Quantifier, Expected: expected, Options: options),
				static (state, it, grammars)
					=> new CollectionConstraint<TEnumerable, object?>(
						it, grammars,
						state.Quantifier,
						new ElementEqualToString<object?>(state.Options, state.Expected),
						"were")),
			iElements.Subject,
			options);
	}
}
