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

	[CreateCollectionExpectation("AreEqualTo", Factory = typeof(ObjectEqualityWithToleranceOptionsFactory),
		Summary = ElementsAreEqualTo)]
	internal static ToleranceEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem, TTolerance>
		AreEqualToWithToleranceCore<TItem, TTolerance>(
			Elements<TItem> elements,
			TItem expected,
			ObjectEqualityWithToleranceOptions<TItem, TTolerance> options)
	{
		IElements<TItem> iElements = elements;
		ExpectationBuilder expectationBuilder = iElements.Subject.Get().ExpectationBuilder;
		return new ToleranceEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem, TTolerance>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new AsyncCollectionConstraint<TItem>(
					expectationBuilder,
					it, grammars,
					iElements.Quantifier,
					g => ElementExpectations.IsEqualTo(g, Formatter.Format(expected), options),
					a => options.AreConsideredEqual(a, expected),
					"were")),
			iElements.Subject,
			options);
	}

	[CreateCollectionExpectation("AreEqualTo", Summary = ElementsAreEqualTo)]
	internal static ObjectEqualityResult<IEnumerable<TItem>?, IThat<IEnumerable<TItem>?>, TItem>
		AreEqualToCore<TItem>(
			Elements<TItem> elements,
			TItem expected)
	{
		IElements<TItem> iElements = elements;
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = iElements.Subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IEnumerable<TItem>?, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new AsyncCollectionConstraint<TItem>(
					expectationBuilder,
					it, grammars,
					iElements.Quantifier,
					g => ElementExpectations.IsEqualTo(g, Formatter.Format(expected), options),
					a => options.AreConsideredEqual(a, expected),
					"were")),
			iElements.Subject,
			options);
	}

	[CreateCollectionExpectation("AreEqualTo", Priority = -1, Summary = ElementsAreEqualTo)]
	internal static ObjectEqualityResult<IEnumerable, IThat<IEnumerable?>, object?>
		AreEqualToForEnumerableCore(
			ElementsForEnumerable<IEnumerable?> elements,
			object? expected)
	{
		IElementsForEnumerable<IEnumerable?> iElements = elements;
		ObjectEqualityOptions<object?> options = new();
		ExpectationBuilder expectationBuilder = iElements.Subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IEnumerable, IThat<IEnumerable?>, object?>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new AsyncCollectionForEnumerableConstraint<IEnumerable>(
					expectationBuilder,
					it, grammars,
					iElements.Quantifier,
					g => ElementExpectations.IsEqualTo(g, Formatter.Format(expected), options),
					a => options.AreConsideredEqual(a, expected),
					"were")),
			iElements.Subject,
			options);
	}

	[CreateCollectionExpectation("AreEqualTo", Summary = ElementsAreEqualTo)]
	internal static ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, object?>
		AreEqualToForStructCore<TEnumerable, TItem>(
			ElementsForStructEnumerable<TEnumerable, TItem> elements,
			object? expected)
		where TEnumerable : struct, IEnumerable<TItem>
	{
		IElementsForStructEnumerable<TEnumerable, TItem> iElements = elements;
		ObjectEqualityOptions<object?> options = new();
		ExpectationBuilder expectationBuilder = iElements.Subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, object?>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new AsyncCollectionForEnumerableConstraint<TEnumerable>(
					expectationBuilder,
					it, grammars,
					iElements.Quantifier,
					g => ElementExpectations.IsEqualTo(g, Formatter.Format(expected), options),
					a => options.AreConsideredEqual(a, expected),
					"were")),
			iElements.Subject,
			options);
	}

	[CreateCollectionExpectation("AreEqualTo", Factory = typeof(ObjectEqualityWithToleranceOptionsFactory),
		Summary = ElementsAreEqualTo)]
	internal static ToleranceEqualityResult<TEnumerable, IThat<TEnumerable>, TItem, TTolerance>
		AreEqualToWithToleranceForStructCore<TEnumerable, TItem, TTolerance>(
			ElementsForStructEnumerable<TEnumerable, TItem> elements,
			TItem expected,
			ObjectEqualityWithToleranceOptions<TItem, TTolerance> options)
		where TEnumerable : struct, IEnumerable<TItem>
	{
		IElementsForStructEnumerable<TEnumerable, TItem> iElements = elements;
		ExpectationBuilder expectationBuilder = iElements.Subject.Get().ExpectationBuilder;
		return new ToleranceEqualityResult<TEnumerable, IThat<TEnumerable>, TItem, TTolerance>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new AsyncCollectionForEnumerableConstraint<TEnumerable>(
					expectationBuilder,
					it, grammars,
					iElements.Quantifier,
					g => ElementExpectations.IsEqualTo(g, Formatter.Format(expected), options),
					a => options.AreConsideredEqual((TItem)a!, expected),
					"were")),
			iElements.Subject,
			options);
	}

	[CreateCollectionExpectation("AreEqualTo", Summary = ElementsAreEqualTo)]
	internal static StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>
		AreEqualToForStringsCore(
			Elements elements,
			string? expected)
	{
		IElements iElements = elements;
		StringEqualityOptions options = new(nameof(expected));
		ExpectationBuilder expectationBuilder = iElements.Subject.Get().ExpectationBuilder;
		return new StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new AsyncCollectionConstraint<string?>(
					expectationBuilder,
					it, grammars,
					iElements.Quantifier,
					g => ElementExpectations.IsEqualTo(g, Formatter.Format(expected), options),
					a => options.AreConsideredEqual(a, expected),
					"were")),
			iElements.Subject,
			options);
	}

	[CreateCollectionExpectation("AreEqualTo", Summary = ElementsAreEqualTo)]
	internal static StringEqualityResult<TEnumerable, IThat<TEnumerable>>
		AreEqualToForStructStringsCore<TEnumerable>(
			ElementsForStructEnumerable<TEnumerable> elements,
			string? expected)
		where TEnumerable : struct, IEnumerable<string?>
	{
		IElementsForStructEnumerable<TEnumerable> iElements = elements;
		StringEqualityOptions options = new(nameof(expected));
		ExpectationBuilder expectationBuilder = iElements.Subject.Get().ExpectationBuilder;
		return new StringEqualityResult<TEnumerable, IThat<TEnumerable>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new AsyncCollectionForEnumerableConstraint<TEnumerable>(
					expectationBuilder,
					it, grammars,
					iElements.Quantifier,
					g => ElementExpectations.IsEqualTo(g, Formatter.Format(expected), options),
					a => options.AreConsideredEqual((string?)a, expected),
					"were")),
			iElements.Subject,
			options);
	}
}
