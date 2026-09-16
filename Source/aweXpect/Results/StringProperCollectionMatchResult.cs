using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

#pragma warning disable S110 // The result hierarchy is intentionally deep, so that each continuation inherits the complete vocabulary of its base
/// <summary>
///     The result for verifying that a string collection contains or is contained in another collection.
/// </summary>
/// <remarks>
///     <seealso cref="StringCollectionMatchResult{TType,TThat}" />
/// </remarks>
public class StringProperCollectionMatchResult<TType, TThat>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	StringEqualityOptions options,
	CollectionMatchOptions collectionMatchOptions,
	CollectionMatchOptions.EquivalenceRelations properEquivalenceRelation)
	: StringCollectionMatchResult<TType, TThat>(expectationBuilder, returnValue, options, collectionMatchOptions)
{
	private readonly CollectionMatchOptions _collectionMatchOptions = collectionMatchOptions;

	/// <summary>
	///     Verifies that the two collections differ by at least one additional item.
	/// </summary>
	/// <remarks>
	///     This means, that the expected collection is a proper subset of the subject for <c>Contains</c> and a proper
	///     superset for <c>IsContainedIn</c>.
	/// </remarks>
	public StringCollectionMatchResult<TType, TThat> Properly()
	{
		_collectionMatchOptions.SetEquivalenceRelation(properEquivalenceRelation);
		return this;
	}
}
