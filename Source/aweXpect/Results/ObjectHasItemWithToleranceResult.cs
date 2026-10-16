using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a collection has a matching item, optionally at a given index, allowing a
///     <typeparamref name="TTolerance" /> on the comparison.
/// </summary>
/// <remarks>
///     The options are specified via <see cref="CollectionIndexOptionsExtensions" /> and
///     <see cref="ObjectEqualityOptionsExtensions" />.
/// </remarks>
public class ObjectHasItemWithToleranceResult<TCollection, TItem, TTolerance>(
	ExpectationBuilder expectationBuilder,
	IThat<TCollection?> collection,
	CollectionIndexOptions collectionIndexOptions,
	ObjectEqualityWithToleranceOptions<TItem, TTolerance> options)
	: AndOrResult<TCollection, IThat<TCollection?>, ObjectHasItemWithToleranceResult<TCollection, TItem, TTolerance>>(
			expectationBuilder, collection),
		IOptionsProvider<CollectionIndexOptions>,
		IObjectEqualityWithToleranceResult<ObjectHasItemWithToleranceResult<TCollection, TItem, TTolerance>, TItem,
			TTolerance>
{
	private readonly IThat<TCollection?> _collection = collection;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	ObjectEqualityOptions<TItem> IOptionsProvider<ObjectEqualityOptions<TItem>>.Options => options;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	ObjectEqualityWithToleranceOptions<TItem, TTolerance>
		IOptionsProvider<ObjectEqualityWithToleranceOptions<TItem, TTolerance>>.Options => options;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	CollectionIndexOptions IOptionsProvider<CollectionIndexOptions>.Options => collectionIndexOptions;

	/// <inheritdoc cref="ObjectEqualityOptionsExtensions.Within{TSelf, TElement, TTolerance}(IObjectEqualityWithToleranceResult{TSelf, TElement, TTolerance}, TTolerance)" />
	public WithinResult Within(TTolerance tolerance)
	{
		options.Within(tolerance);
		return new WithinResult(ExpectationBuilder, _collection, collectionIndexOptions, options);
	}

	/// <summary>
	///     The result directly after <see cref="Within(TTolerance)" />, whose tolerance can still be made relative to
	///     the expected value, e.g. via <c>Percent()</c>.
	/// </summary>
	public sealed class WithinResult(
		ExpectationBuilder expectationBuilder,
		IThat<TCollection?> collection,
		CollectionIndexOptions collectionIndexOptions,
		ObjectEqualityWithToleranceOptions<TItem, TTolerance> options)
		: ObjectHasItemWithToleranceResult<TCollection, TItem, TTolerance>(expectationBuilder, collection,
				collectionIndexOptions, options),
			IWithinToleranceResult<ObjectHasItemWithToleranceResult<TCollection, TItem, TTolerance>, TTolerance>
	{
		/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
		IRelativeToleranceOptions<TTolerance> IOptionsProvider<IRelativeToleranceOptions<TTolerance>>.Options
			=> ((IOptionsProvider<ObjectEqualityWithToleranceOptions<TItem, TTolerance>>)this).Options;

		/// <inheritdoc cref="IWithinToleranceResult{TResult,TTolerance}.Result" />
		ObjectHasItemWithToleranceResult<TCollection, TItem, TTolerance>
			IWithinToleranceResult<ObjectHasItemWithToleranceResult<TCollection, TItem, TTolerance>, TTolerance>.Result
			=> this;
	}
}
