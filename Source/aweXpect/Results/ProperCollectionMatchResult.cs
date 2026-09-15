using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a collection contains or is contained in another collection.
/// </summary>
/// <remarks>
///     <seealso cref="CollectionMatchResult{TType,TThat,TItem}" />
/// </remarks>
public class ProperCollectionMatchResult<TType, TThat, TItem>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	CollectionMatchOptions collectionMatchOptions,
	CollectionMatchOptions.EquivalenceRelations properEquivalenceRelation)
	: CollectionMatchResult<TType, TThat, TItem>(expectationBuilder, returnValue, collectionMatchOptions)
{
	private readonly CollectionMatchOptions _collectionMatchOptions = collectionMatchOptions;

	/// <summary>
	///     Verifies that the two collections differ by at least one additional item.
	/// </summary>
	/// <remarks>
	///     This means, that the expected collection is a proper subset of the subject for <c>Contains</c> and a proper
	///     superset for <c>IsContainedIn</c>.
	/// </remarks>
	public CollectionMatchResult<TType, TThat, TItem> Properly()
	{
		_collectionMatchOptions.SetEquivalenceRelation(properEquivalenceRelation);
		return this;
	}
}
