using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

#pragma warning disable S110 // The result hierarchy is intentionally deep, so that each continuation inherits the complete vocabulary of its base
/// <summary>
///     The result for verifying that a collection contains or is contained in another collection, allowing a
///     <typeparamref name="TTolerance" /> on the comparison.
/// </summary>
/// <remarks>
///     <seealso cref="ObjectProperCollectionMatchResult{TType,TThat,TItem}" />
/// </remarks>
public class ObjectProperCollectionMatchWithToleranceResult<TType, TThat, TItem, TTolerance>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	ObjectEqualityWithToleranceOptions<TItem, TTolerance> options,
	CollectionMatchOptions collectionMatchOptions,
	CollectionMatchOptions.EquivalenceRelations properEquivalenceRelation)
	: ObjectProperCollectionMatchResult<TType, TThat, TItem>(expectationBuilder, returnValue, options,
		collectionMatchOptions, properEquivalenceRelation)
{
	/// <summary>
	///     Specifies a <paramref name="tolerance" /> to apply on the comparison.
	/// </summary>
	public ObjectProperCollectionMatchResult<TType, TThat, TItem> Within(TTolerance tolerance)
	{
		options.Within(tolerance);
		return this;
	}
}
