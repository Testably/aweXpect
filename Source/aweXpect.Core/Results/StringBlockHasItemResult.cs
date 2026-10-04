using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a collection has an item that matches a block of lines, optionally at a given index.
/// </summary>
/// <remarks>
///     A block compares the lines on its own, so of the string options only the casing and a comparer can be
///     specified; the index is specified via <see cref="CollectionIndexOptionsExtensions" />.
/// </remarks>
public class StringBlockHasItemResult<TCollection>(
	ExpectationBuilder expectationBuilder,
	IThat<TCollection?> collection,
	CollectionIndexOptions collectionIndexOptions,
	StringEqualityOptions options)
	: AndOrResult<TCollection, IThat<TCollection?>, StringBlockHasItemResult<TCollection>>(expectationBuilder,
			collection),
		IOptionsProvider<CollectionIndexOptions>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	CollectionIndexOptions IOptionsProvider<CollectionIndexOptions>.Options => collectionIndexOptions;

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
