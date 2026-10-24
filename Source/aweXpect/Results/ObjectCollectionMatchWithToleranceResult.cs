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
public class ObjectCollectionMatchWithToleranceResult<TType, TThat, TItem, TTolerance>
	: AndOrResult<TType, TThat, ObjectCollectionMatchWithToleranceResult<TType, TThat, TItem, TTolerance>>,
		IOptionsProvider<CollectionMatchOptions>,
		IObjectEqualityWithToleranceResult<ObjectCollectionMatchWithToleranceResult<TType, TThat, TItem, TTolerance>,
			TItem, TTolerance>
{
	private readonly CollectionMatchOptions _collectionMatchOptions;
	private readonly ObjectEqualityWithToleranceOptions<TItem, TTolerance> _options;
	private readonly TThat _returnValue;

	internal ObjectCollectionMatchWithToleranceResult(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		ObjectEqualityWithToleranceOptions<TItem, TTolerance> options,
		CollectionMatchOptions collectionMatchOptions)
		: base(expectationBuilder, returnValue)
	{
		_returnValue = returnValue;
		_options = options;
		_collectionMatchOptions = collectionMatchOptions;
	}

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	ObjectEqualityOptions<TItem> IOptionsProvider<ObjectEqualityOptions<TItem>>.Options => _options;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	ObjectEqualityWithToleranceOptions<TItem, TTolerance>
		IOptionsProvider<ObjectEqualityWithToleranceOptions<TItem, TTolerance>>.Options => _options;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	CollectionMatchOptions IOptionsProvider<CollectionMatchOptions>.Options => _collectionMatchOptions;

	/// <inheritdoc cref="ObjectEqualityOptionsExtensions.Within{TSelf, TItem, TTolerance}(IObjectEqualityWithToleranceResult{TSelf, TItem, TTolerance}, TTolerance)" />
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
		: ObjectCollectionMatchWithToleranceResult<TType, TThat, TItem, TTolerance>,
			IWithinToleranceResult<ObjectCollectionMatchWithToleranceResult<TType, TThat, TItem, TTolerance>,
				TTolerance>
	{
		internal WithinResult(
			ExpectationBuilder expectationBuilder,
			TThat returnValue,
			ObjectEqualityWithToleranceOptions<TItem, TTolerance> options,
			CollectionMatchOptions collectionMatchOptions)
			: base(expectationBuilder, returnValue, options, collectionMatchOptions)
		{
		}

		/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
		IRelativeToleranceOptions<TTolerance> IOptionsProvider<IRelativeToleranceOptions<TTolerance>>.Options
			=> ((IOptionsProvider<ObjectEqualityWithToleranceOptions<TItem, TTolerance>>)this).Options;

		/// <inheritdoc cref="IWithinToleranceResult{TResult,TTolerance}.Result" />
		ObjectCollectionMatchWithToleranceResult<TType, TThat, TItem, TTolerance>
			IWithinToleranceResult<ObjectCollectionMatchWithToleranceResult<TType, TThat, TItem, TTolerance>,
				TTolerance>.Result
			=> this;
	}
}
