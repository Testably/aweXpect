using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a collection has an item that matches a block of lines, optionally at a given index.
/// </summary>
/// <remarks>
///     <seealso cref="StringBlockCountResult{TType,TThat}" />
/// </remarks>
public class StringBlockHasItemResult<TCollection>(
	ExpectationBuilder expectationBuilder,
	IThat<TCollection?> collection,
	CollectionIndexOptions collectionIndexOptions,
	StringEqualityOptions options)
	: HasItemResult<TCollection>(expectationBuilder, collection, collectionIndexOptions),
		IOptionsProvider<StringEqualityOptions>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	StringEqualityOptions IOptionsProvider<StringEqualityOptions>.Options => options;

	/// <summary>
	///     Ignores casing when comparing the <see langword="string" />s.
	/// </summary>
	public StringBlockHasItemResult<TCollection> IgnoringCase(bool ignoreCase = true)
	{
		options.IgnoringCase(ignoreCase);
		return this;
	}

	/// <summary>
	///     Uses the provided <paramref name="comparer" /> for comparing <see langword="string" />s.
	/// </summary>
	public StringBlockHasItemResult<TCollection> Using(
		IEqualityComparer<string> comparer)
	{
		options.Using(comparer);
		return this;
	}
}
