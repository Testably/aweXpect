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
public class StringBlockHasItemResult<TCollection, TThat>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	IOptionsHolderSource<CollectionIndexOptions> collectionIndexOptions,
	IOptionsHolderSource<StringEqualityOptions> options)
	: AndOrResult<TCollection, TThat, StringBlockHasItemResult<TCollection, TThat>>(expectationBuilder, returnValue),
		IOptionsProvider<CollectionIndexOptions>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsHolder<CollectionIndexOptions> IOptionsProvider<CollectionIndexOptions>.Options => collectionIndexOptions.Holder;

	/// <summary>
	///     Ignores casing when comparing the <see langword="string" />s.
	/// </summary>
	public StringBlockHasItemResult<TCollection, TThat> IgnoringCase(bool ignoreCase = true)
	{
		options.Holder.IgnoringCase(ignoreCase);
		return this;
	}

	/// <summary>
	///     Uses the provided <paramref name="comparer" /> for comparing <see langword="string" />s.
	/// </summary>
	public StringBlockHasItemResult<TCollection, TThat> Using(
		IEqualityComparer<string> comparer)
	{
		options.Holder.Using(comparer);
		return this;
	}
}
