using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

#pragma warning disable S110 // The result hierarchy is intentionally deep, so that each continuation inherits the complete vocabulary of its base
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
