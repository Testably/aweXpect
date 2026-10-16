using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a collection matches another collection, allowing a
///     <typeparamref name="TTolerance" /> on the comparison.
/// </summary>
/// <remarks>
///     The options are specified via <see cref="CollectionMatchOptionsExtensions" /> and
///     <see cref="ObjectEqualityOptionsExtensions" />.
/// </remarks>
public class ObjectCollectionMatchWithToleranceResult<TType, TThat, TElement, TTolerance>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	ObjectEqualityWithToleranceOptions<TElement, TTolerance> options,
	CollectionMatchOptions collectionMatchOptions)
	: AndOrResult<TType, TThat, ObjectCollectionMatchWithToleranceResult<TType, TThat, TElement, TTolerance>>(
			expectationBuilder, returnValue),
		IOptionsProvider<CollectionMatchOptions>,
		IObjectEqualityWithToleranceResult<ObjectCollectionMatchWithToleranceResult<TType, TThat, TElement, TTolerance>,
			TElement, TTolerance>
{
	private readonly TThat _returnValue = returnValue;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	ObjectEqualityOptions<TElement> IOptionsProvider<ObjectEqualityOptions<TElement>>.Options => options;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	ObjectEqualityWithToleranceOptions<TElement, TTolerance>
		IOptionsProvider<ObjectEqualityWithToleranceOptions<TElement, TTolerance>>.Options => options;

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
		ObjectEqualityWithToleranceOptions<TElement, TTolerance> options,
		CollectionMatchOptions collectionMatchOptions)
		: ObjectCollectionMatchWithToleranceResult<TType, TThat, TElement, TTolerance>(expectationBuilder,
				returnValue, options, collectionMatchOptions),
			IWithinToleranceResult<ObjectCollectionMatchWithToleranceResult<TType, TThat, TElement, TTolerance>,
				TTolerance>
	{
		/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
		IRelativeToleranceOptions<TTolerance> IOptionsProvider<IRelativeToleranceOptions<TTolerance>>.Options
			=> ((IOptionsProvider<ObjectEqualityWithToleranceOptions<TElement, TTolerance>>)this).Options;

		/// <inheritdoc cref="IWithinToleranceResult{TResult,TTolerance}.Result" />
		ObjectCollectionMatchWithToleranceResult<TType, TThat, TElement, TTolerance>
			IWithinToleranceResult<ObjectCollectionMatchWithToleranceResult<TType, TThat, TElement, TTolerance>,
				TTolerance>.Result
			=> this;
	}
}
