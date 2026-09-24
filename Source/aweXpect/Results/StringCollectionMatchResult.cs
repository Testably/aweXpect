using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a string collection matches another collection.
/// </summary>
/// <remarks>
///     <seealso cref="StringEqualityTypeResult{TType,TThat,TSelf}" />
/// </remarks>
public class StringCollectionMatchResult<TType, TThat>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	StringEqualityOptions options,
	CollectionMatchOptions collectionMatchOptions)
	: StringCollectionMatchResult<TType, TThat,
		StringCollectionMatchResult<TType, TThat>>(
		expectationBuilder,
		returnValue,
		options,
		collectionMatchOptions);

/// <summary>
///     The result for verifying that a string collection matches another collection.
/// </summary>
/// <remarks>
///     <seealso cref="StringEqualityTypeResult{TType,TThat,TSelf}" />
/// </remarks>
public class StringCollectionMatchResult<TType, TThat, TSelf>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	StringEqualityOptions options,
	CollectionMatchOptions collectionMatchOptions)
	: StringEqualityTypeResult<TType, TThat, TSelf>(expectationBuilder, returnValue, options),
		IOptionsProvider<CollectionMatchOptions>
	where TSelf : StringCollectionMatchResult<TType, TThat, TSelf>
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
	///     Each distinct item is compared once, so <c>[1, 1, 2]</c> matches <c>[1, 2]</c>.
	/// </remarks>
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
