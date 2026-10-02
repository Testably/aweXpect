using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a collection matches another collection.
/// </summary>
/// <remarks>
///     <seealso cref="ObjectEqualityResult{TType,TThat,TSelf}" />
/// </remarks>
public class ObjectCollectionMatchResult<TType, TThat, TElement>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	ObjectEqualityOptions<TElement> options,
	CollectionMatchOptions collectionMatchOptions)
	: ObjectCollectionMatchResult<TType, TThat, TElement,
		ObjectCollectionMatchResult<TType, TThat, TElement>>(
		expectationBuilder,
		returnValue,
		options,
		collectionMatchOptions);

/// <summary>
///     The result for verifying that a collection matches another collection.
/// </summary>
/// <remarks>
///     <seealso cref="ObjectEqualityResult{TType,TThat,TSelf}" />
/// </remarks>
public class ObjectCollectionMatchResult<TType, TThat, TElement, TSelf>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	ObjectEqualityOptions<TElement> options,
	CollectionMatchOptions collectionMatchOptions)
	: ObjectEqualityResult<TType, TThat, TElement, TSelf>(expectationBuilder, returnValue, options),
		IOptionsProvider<CollectionMatchOptions>
	where TSelf : ObjectCollectionMatchResult<TType, TThat, TElement, TSelf>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	CollectionMatchOptions IOptionsProvider<CollectionMatchOptions>.Options => collectionMatchOptions;

	/// <summary>
	///     Ignores the order in the subject and expected values.
	/// </summary>
	public TSelf InAnyOrder()
	{
		collectionMatchOptions.InAnyOrder();
		return (TSelf)this;
	}

	/// <summary>
	///     Ignores duplicates in both collections.
	/// </summary>
	/// <remarks>
	///     Only which items occur matters, not how often: every expected item has to be matched by an item, and every
	///     item has to match an expected item, as far as the relation requires it, so <c>[1, 1, 2]</c> matches
	///     <c>[1, 2]</c>.
	/// </remarks>
	public TSelf IgnoringDuplicates()
	{
		collectionMatchOptions.IgnoringDuplicates();
		return (TSelf)this;
	}
}
