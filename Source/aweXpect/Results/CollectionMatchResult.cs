using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a collection matches another collection.
/// </summary>
/// <remarks>
///     <seealso cref="ObjectEqualityResult{TType,TThat,TSelf}" />
/// </remarks>
public class CollectionMatchResult<TType, TThat, TElement>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	CollectionMatchOptions collectionMatchOptions)
	: CollectionMatchResult<TType, TThat, TElement,
		CollectionMatchResult<TType, TThat, TElement>>(
		expectationBuilder,
		returnValue,
		collectionMatchOptions);

/// <summary>
///     The result for verifying that a collection matches another collection.
/// </summary>
/// <remarks>
///     <seealso cref="ObjectEqualityResult{TType,TThat,TSelf}" />
/// </remarks>
public class CollectionMatchResult<TType, TThat, TElement, TSelf>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	CollectionMatchOptions collectionMatchOptions)
	: AndOrResult<TType, TThat>(expectationBuilder, returnValue),
		IOptionsProvider<CollectionMatchOptions>
	where TSelf : CollectionMatchResult<TType, TThat, TElement, TSelf>
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
	public TSelf IgnoringDuplicates()
	{
		collectionMatchOptions.IgnoringDuplicates();
		return (TSelf)this;
	}

	/// <summary>
	///     Ignores items that appear in between the matched items.
	/// </summary>
	/// <remarks>
	///     This option has no effect when <see cref="InAnyOrder()" /> is used.
	/// </remarks>
	public TSelf IgnoringInterspersedItems()
	{
		collectionMatchOptions.IgnoringInterspersedItems();
		return (TSelf)this;
	}
}
