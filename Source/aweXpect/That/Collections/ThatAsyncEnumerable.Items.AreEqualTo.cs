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

	[CreateExpectationFamily("AreEqualTo", Factory = typeof(ObjectEqualityWithToleranceOptionsFactory),
		Summary = ItemsAreEqualTo)]
	internal static ObjectEqualityWithToleranceResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem, TTolerance>
		AreEqualToWithToleranceCore<TItem, TTolerance>(
			IAsyncEnumerableItems<TItem> items,
			TItem expected, Options.ObjectEqualityWithToleranceOptions<TItem, TTolerance> initialOptions)
	{
		OptionsSlot<Options.ObjectEqualityWithToleranceOptions<TItem, TTolerance>> options = new(initialOptions);
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		return new ObjectEqualityWithToleranceResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem,
			TTolerance>(
			expectationBuilder.AddConstraint((items.Quantifier, Expected: expected, Options: options),
				static (state, it, grammars)
					=> new AsyncCollectionConstraint<TItem>(
						it, grammars,
						state.Quantifier,
						new ElementEqualTo<TItem, TItem>(state.Options.Value, state.Expected),
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
		OptionsSlot<Options.ObjectEqualityOptions<TItem>> options = new(new Options.ObjectEqualityOptions<TItem>());
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((items.Quantifier, Expected: expected, Options: options),
				static (state, it, grammars)
					=> new AsyncCollectionConstraint<TItem>(
						it, grammars,
						state.Quantifier,
						new ElementEqualTo<TItem, TItem>(state.Options.Value, state.Expected, state.Options.Value),
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
		OptionsSlot<StringEqualityOptions> options = new(new StringEqualityOptions(nameof(expected)));
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		return new StringEqualityTypeResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>(
			expectationBuilder.AddConstraint((items.Quantifier, Expected: expected, Options: options),
				static (state, it, grammars)
					=> new AsyncCollectionConstraint<string?>(
						it, grammars,
						state.Quantifier,
						new ElementEqualToString<string?>(state.Options.Value, state.Expected),
						"were")),
			items.Subject,
			options);
	}
}
#endif
