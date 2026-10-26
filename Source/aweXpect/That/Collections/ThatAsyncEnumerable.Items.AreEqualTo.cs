#if NET8_0_OR_GREATER
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Results;
using aweXpect.SourceGenerators;
using StringEqualityOptions = aweXpect.Options.StringEqualityOptions;

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	private const string ItemsAreEqualTo = "…are equal to the <paramref name=\"expected\" /> value.";

	[CreateExpectationFamily("AreEqualTo", Factory = typeof(ObjectToleranceMatchTypeFactory),
		Summary = ItemsAreEqualTo)]
	internal static ObjectEqualityWithToleranceResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem, TTolerance>
		AreEqualToWithToleranceCore<TItem, TTolerance>(
			IAsyncEnumerableItems<TItem> items,
			TItem expected, Options.ObjectToleranceMatchType<TItem, TTolerance> initialOptions)
	{
		OptionsHolder<ObjectMatchType<TItem>> options = new(initialOptions);
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		return new ObjectEqualityWithToleranceResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem,
			TTolerance>(
			expectationBuilder.AddConstraint((items.Quantifier, Expected: expected, Options: options),
				static (state, it, grammars)
					=> new AsyncCollectionConstraint<TItem>(
						it, grammars,
						state.Quantifier,
						new ElementEqualTo<TItem, TItem>(state.Options, state.Expected),
						"were")),
			items.Subject,
			options);
	}

	[CreateExpectationFamily("AreEqualTo", Summary = ItemsAreEqualTo)]
	internal static ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		AreEqualToCore<TItem>(
			IAsyncEnumerableItems<TItem> items,
			TItem expected)
	{
		OptionsHolder<ObjectMatchType<TItem>> options = new(ObjectMatchType<TItem>.Default);
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((items.Quantifier, Expected: expected, Options: options),
				static (state, it, grammars)
					=> new AsyncCollectionConstraint<TItem>(
						it, grammars,
						state.Quantifier,
						new ElementEqualTo<TItem, TItem>(state.Options, state.Expected, true),
						"were")),
			items.Subject,
			options);
	}

	[CreateExpectationFamily("AreEqualTo", Summary = ItemsAreEqualTo)]
	internal static StringEqualityTypeResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>
		AreEqualToForStringsCore(
			IAsyncEnumerableItems<string?> items,
			string? expected)
	{
		OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		return new StringEqualityTypeResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>(
			expectationBuilder.AddConstraint((items.Quantifier, Expected: expected, Options: options),
				static (state, it, grammars)
					=> new AsyncCollectionConstraint<string?>(
						it, grammars,
						state.Quantifier,
						new ElementEqualToString<string?>(state.Options, state.Expected),
						"were")),
			items.Subject,
			options);
	}
}
#endif
