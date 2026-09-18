using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a collection has a matching item at a given index.
/// </summary>
/// <remarks>
///     <seealso cref="ExpectationResult{TType,TSelf}" />
/// </remarks>
public class HasItemAtIndexResult<TCollection>(
	ExpectationBuilder expectationBuilder,
	IThat<TCollection> collection,
	CollectionIndexOptions collectionIndexOptions)
	: AndOrResult<TCollection, IThat<TCollection>>(expectationBuilder, collection),
		IOptionsProvider<CollectionIndexOptions>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	CollectionIndexOptions IOptionsProvider<CollectionIndexOptions>.Options => collectionIndexOptions;

	/// <summary>
	///     …from end.
	/// </summary>
	public AndOrResult<TCollection, IThat<TCollection>> FromEnd()
	{
		if (collectionIndexOptions.Match is CollectionIndexOptions.IMatchFromBeginning match)
		{
			collectionIndexOptions.SetMatch(match.FromEnd());
		}

		return this;
	}
}
