using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

#pragma warning disable S110 // The result hierarchy is intentionally deep, so that each continuation inherits the complete vocabulary of its base
/// <summary>
///     The result for verifying that a collection contains or is contained in another collection.
/// </summary>
/// <remarks>
///     <seealso cref="ObjectCollectionContainmentResult{TType,TThat,TElement}" />
/// </remarks>
public class ObjectProperCollectionMatchResult<TType, TThat, TItem>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	ObjectEqualityOptions<TItem> options,
	CollectionMatchOptions collectionMatchOptions,
	CollectionMatchOptions.EquivalenceRelations properEquivalenceRelation)
	: ObjectCollectionContainmentResult<TType, TThat, TItem>(expectationBuilder, returnValue, options,
		collectionMatchOptions)
{
	private readonly CollectionMatchOptions _collectionMatchOptions = collectionMatchOptions;

	/// <summary>
	///     Verifies that the two collections differ by at least one additional item.
	/// </summary>
	/// <remarks>
	///     This means that the expected collection is a proper subset of the subject for <c>Contains</c> and a proper
	///     superset for <c>IsContainedIn</c>.
	/// </remarks>
	public ObjectCollectionContainmentResult<TType, TThat, TItem> Properly()
	{
		_collectionMatchOptions.SetEquivalenceRelation(properEquivalenceRelation);
		return this;
	}
}
