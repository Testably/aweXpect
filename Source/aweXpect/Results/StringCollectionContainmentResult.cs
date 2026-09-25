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
public class StringCollectionContainmentResult<TType, TThat>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	StringEqualityOptions options,
	CollectionMatchOptions collectionMatchOptions)
	: StringCollectionContainmentResult<TType, TThat,
		StringCollectionContainmentResult<TType, TThat>>(
		expectationBuilder,
		returnValue,
		options,
		collectionMatchOptions);

/// <summary>
///     The result for verifying that a string collection contains or is contained in another collection.
/// </summary>
/// <remarks>
///     <seealso cref="StringCollectionMatchResult{TType,TThat,TSelf}" />
/// </remarks>
public class StringCollectionContainmentResult<TType, TThat, TSelf>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	StringEqualityOptions options,
	CollectionMatchOptions collectionMatchOptions)
	: StringCollectionMatchResult<TType, TThat, TSelf>(expectationBuilder, returnValue, options,
		collectionMatchOptions)
	where TSelf : StringCollectionContainmentResult<TType, TThat, TSelf>
{
	private readonly CollectionMatchOptions _collectionMatchOptions = collectionMatchOptions;

	/// <summary>
	///     Ignores items that appear in between the matched items.
	/// </summary>
	/// <exception cref="System.InvalidOperationException">
	///     The order is already ignored via <c>InAnyOrder()</c>.
	/// </exception>
	public TSelf IgnoringInterspersedItems()
	{
		_collectionMatchOptions.IgnoringInterspersedItems();
		return (TSelf)this;
	}
}
