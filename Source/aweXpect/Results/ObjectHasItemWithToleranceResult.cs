using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

#pragma warning disable S110 // The result hierarchy is intentionally deep, so that each continuation inherits the complete vocabulary of its base
/// <summary>
///     The result for verifying that a collection has a matching item, optionally at a given index, allowing a
///     <typeparamref name="TTolerance" /> on the comparison.
/// </summary>
/// <remarks>
///     <seealso cref="ObjectHasItemResult{TCollection,TItem}" />
/// </remarks>
public class ObjectHasItemWithToleranceResult<TCollection, TItem, TTolerance>(
	ExpectationBuilder expectationBuilder,
	IThat<TCollection?> collection,
	CollectionIndexOptions collectionIndexOptions,
	ObjectEqualityWithToleranceOptions<TItem, TTolerance> options)
	: ObjectHasItemResult<TCollection, TItem>(expectationBuilder, collection, collectionIndexOptions, options)
{
	/// <summary>
	///     Specifies a <paramref name="tolerance" /> to apply on the comparison.
	/// </summary>
	public ObjectHasItemResult<TCollection, TItem> Within(TTolerance tolerance)
	{
		options.Within(tolerance);
		return this;
	}
}
