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
public class ObjectCollectionMatchWithToleranceResult<TType, TThat, TElement, TTolerance>
	: AndOrResult<TType, TThat, ObjectCollectionMatchWithToleranceResult<TType, TThat, TElement, TTolerance>>,
		IOptionsProvider<CollectionMatchOptions>,
		IObjectEqualityWithToleranceResult<ObjectCollectionMatchWithToleranceResult<TType, TThat, TElement, TTolerance>,
			TElement, TTolerance>
{
	private readonly CollectionMatchOptions _collectionMatchOptions;
	private readonly ObjectEqualityWithToleranceOptions<TElement, TTolerance> _options;
	private readonly TThat _returnValue;

	internal ObjectCollectionMatchWithToleranceResult(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		ObjectEqualityWithToleranceOptions<TElement, TTolerance> options,
		CollectionMatchOptions collectionMatchOptions)
		: base(expectationBuilder, returnValue)
	{
		_returnValue = returnValue;
		_options = options;
		_collectionMatchOptions = collectionMatchOptions;
	}

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	ObjectEqualityOptions<TElement> IOptionsProvider<ObjectEqualityOptions<TElement>>.Options => _options;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	ObjectEqualityWithToleranceOptions<TElement, TTolerance>
		IOptionsProvider<ObjectEqualityWithToleranceOptions<TElement, TTolerance>>.Options => _options;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	CollectionMatchOptions IOptionsProvider<CollectionMatchOptions>.Options => _collectionMatchOptions;

	/// <inheritdoc cref="ObjectEqualityOptionsExtensions.Within{TSelf, TElement, TTolerance}(IObjectEqualityWithToleranceResult{TSelf, TElement, TTolerance}, TTolerance)" />
	public WithinResult Within(TTolerance tolerance)
	{
		_options.Within(tolerance);
		return new WithinResult(ExpectationBuilder, _returnValue, _options, _collectionMatchOptions);
	}

	/// <summary>
	///     The result directly after <see cref="Within(TTolerance)" />, whose tolerance can still be made relative to
	///     the expected value, e.g. via <c>Percent()</c>.
	/// </summary>
	public sealed class WithinResult
		: ObjectCollectionMatchWithToleranceResult<TType, TThat, TElement, TTolerance>,
			IWithinToleranceResult<ObjectCollectionMatchWithToleranceResult<TType, TThat, TElement, TTolerance>,
				TTolerance>
	{
		internal WithinResult(
			ExpectationBuilder expectationBuilder,
			TThat returnValue,
			ObjectEqualityWithToleranceOptions<TElement, TTolerance> options,
			CollectionMatchOptions collectionMatchOptions)
			: base(expectationBuilder, returnValue, options, collectionMatchOptions)
		{
		}

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
