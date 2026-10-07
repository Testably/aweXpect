#if NET8_0_OR_GREATER
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;
using aweXpect.SourceGenerators;
using StringEqualityOptions = aweXpect.Options.StringEqualityOptions;

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	private const string ElementsAreEqualTo = "…are equal to the <paramref name=\"expected\" /> value.";

	[CreateExpectationFamily("AreEqualTo", Factory = typeof(ObjectEqualityWithToleranceOptionsFactory),
		Summary = ElementsAreEqualTo)]
	internal static ObjectEqualityWithToleranceResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem, TTolerance>
		AreEqualToWithToleranceCore<TItem, TTolerance>(
			Elements<TItem> elements,
			TItem expected, Options.ObjectEqualityWithToleranceOptions<TItem, TTolerance> options)
	{
		IAsyncEnumerableElements<TItem> iElements = elements;
		ExpectationBuilder expectationBuilder = iElements.Subject.Get().ExpectationBuilder;
		return new ObjectEqualityWithToleranceResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem,
			TTolerance>(
			expectationBuilder.AddConstraint((iElements.Quantifier, Expected: expected, Options: options),
				static (state, it, grammars)
					=> new AsyncCollectionConstraint<TItem>(
						it, grammars,
						state.Quantifier,
						new ElementEqualTo<TItem, TItem>(state.Options, state.Expected),
						"were")),
			iElements.Subject,
			options);
	}

	[CreateExpectationFamily("AreEqualTo", Summary = ElementsAreEqualTo)]
	internal static ObjectEqualityResult<IAsyncEnumerable<TItem>?, IThat<IAsyncEnumerable<TItem>?>, TItem>
		AreEqualToCore<TItem>(
			Elements<TItem> elements,
			TItem expected)
	{
		IAsyncEnumerableElements<TItem> iElements = elements;
		Options.ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = iElements.Subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IAsyncEnumerable<TItem>?, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((iElements.Quantifier, Expected: expected, Options: options),
				static (state, it, grammars)
					=> new AsyncCollectionConstraint<TItem>(
						it, grammars,
						state.Quantifier,
						new ElementEqualTo<TItem, TItem>(state.Options, state.Expected, state.Options),
						"were")),
			iElements.Subject,
			options);
	}

	[CreateExpectationFamily("AreEqualTo", Summary = ElementsAreEqualTo)]
	internal static StringEqualityTypeResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>
		AreEqualToForStringsCore(
			Elements elements,
			string? expected)
	{
		IAsyncEnumerableStringElements iElements = elements;
		StringEqualityOptions options = new(nameof(expected));
		ExpectationBuilder expectationBuilder = iElements.Subject.Get().ExpectationBuilder;
		return new StringEqualityTypeResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>(
			expectationBuilder.AddConstraint((iElements.Quantifier, Expected: expected, Options: options),
				static (state, it, grammars)
					=> new AsyncCollectionConstraint<string?>(
						it, grammars,
						state.Quantifier,
						new ElementEqualToString<string?>(state.Options, state.Expected),
						"were")),
			iElements.Subject,
			options);
	}
}
#endif
