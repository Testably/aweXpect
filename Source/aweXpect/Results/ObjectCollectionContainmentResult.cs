using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

#pragma warning disable S110 // The result hierarchy is intentionally deep, so that each continuation inherits the complete vocabulary of its base
/// <summary>
///     The result for verifying that a collection contains or is contained in another collection.
/// </summary>
/// <remarks>
///     <seealso cref="ObjectCollectionMatchResult{TType,TThat,TElement}" />
/// </remarks>
public class ObjectCollectionContainmentResult<TType, TThat, TElement>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	ObjectEqualityOptions<TElement> options,
	CollectionMatchOptions collectionMatchOptions)
	: ObjectCollectionContainmentResult<TType, TThat, TElement,
		ObjectCollectionContainmentResult<TType, TThat, TElement>>(
		expectationBuilder,
		returnValue,
		options,
		collectionMatchOptions);

/// <summary>
///     The result for verifying that a collection contains or is contained in another collection.
/// </summary>
/// <remarks>
///     <seealso cref="ObjectCollectionMatchResult{TType,TThat,TElement,TSelf}" />
/// </remarks>
public class ObjectCollectionContainmentResult<TType, TThat, TElement, TSelf>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	ObjectEqualityOptions<TElement> options,
	CollectionMatchOptions collectionMatchOptions)
	: ObjectCollectionMatchResult<TType, TThat, TElement, TSelf>(expectationBuilder, returnValue, options,
		collectionMatchOptions)
	where TSelf : ObjectCollectionContainmentResult<TType, TThat, TElement, TSelf>
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
