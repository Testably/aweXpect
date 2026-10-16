using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a collection contains or is contained in another collection, optionally
///     properly, allowing a <typeparamref name="TTolerance" /> on the comparison.
/// </summary>
/// <remarks>
///     The options are specified via <see cref="CollectionMatchOptionsExtensions" /> and
///     <see cref="ObjectEqualityOptionsExtensions" />.
/// </remarks>
public class ObjectProperCollectionMatchWithToleranceResult<TType, TThat, TItem, TTolerance>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	ObjectEqualityWithToleranceOptions<TItem, TTolerance> options,
	CollectionMatchOptions collectionMatchOptions)
	: AndOrResult<TType, TThat, ObjectProperCollectionMatchWithToleranceResult<TType, TThat, TItem, TTolerance>>(
			expectationBuilder, returnValue),
		IOptionsProvider<CollectionMatchOptions>,
		ICollectionContainmentOptions,
		IProperContainmentOptions,
		IObjectEqualityWithToleranceResult<
			ObjectProperCollectionMatchWithToleranceResult<TType, TThat, TItem, TTolerance>, TItem, TTolerance>
{
	private readonly TThat _returnValue = returnValue;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	ObjectEqualityOptions<TItem> IOptionsProvider<ObjectEqualityOptions<TItem>>.Options => options;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	ObjectEqualityWithToleranceOptions<TItem, TTolerance>
		IOptionsProvider<ObjectEqualityWithToleranceOptions<TItem, TTolerance>>.Options => options;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	CollectionMatchOptions IOptionsProvider<CollectionMatchOptions>.Options => collectionMatchOptions;

	/// <inheritdoc cref="ObjectEqualityOptionsExtensions.Within{TSelf, TElement, TTolerance}(IObjectEqualityWithToleranceResult{TSelf, TElement, TTolerance}, TTolerance)" />
	public WithinResult Within(TTolerance tolerance)
	{
		options.Within(tolerance);
		return new WithinResult(ExpectationBuilder, _returnValue, options, collectionMatchOptions);
	}

	/// <summary>
	///     The result directly after <see cref="Within(TTolerance)" />, whose tolerance can still be made relative to
	///     the expected value, e.g. via <c>Percent()</c>.
	/// </summary>
	public sealed class WithinResult(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		ObjectEqualityWithToleranceOptions<TItem, TTolerance> options,
		CollectionMatchOptions collectionMatchOptions)
		: ObjectProperCollectionMatchWithToleranceResult<TType, TThat, TItem, TTolerance>(expectationBuilder,
				returnValue, options, collectionMatchOptions),
			IWithinToleranceResult<ObjectProperCollectionMatchWithToleranceResult<TType, TThat, TItem, TTolerance>,
				TTolerance>
	{
		/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
		IRelativeToleranceOptions<TTolerance> IOptionsProvider<IRelativeToleranceOptions<TTolerance>>.Options
			=> ((IOptionsProvider<ObjectEqualityWithToleranceOptions<TItem, TTolerance>>)this).Options;

		/// <inheritdoc cref="IWithinToleranceResult{TResult,TTolerance}.Result" />
		ObjectProperCollectionMatchWithToleranceResult<TType, TThat, TItem, TTolerance>
			IWithinToleranceResult<ObjectProperCollectionMatchWithToleranceResult<TType, TThat, TItem, TTolerance>,
				TTolerance>.Result
			=> this;
	}
}
