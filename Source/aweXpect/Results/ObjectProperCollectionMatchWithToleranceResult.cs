using System;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a collection contains or is contained in another collection, optionally
///     properly, allowing a <typeparamref name="TTolerance" /> on the comparison.
/// </summary>
/// <remarks>
///     The options are specified via <see cref="CollectionMatchOptionsExtensions" /> and
///     <see cref="ObjectMatchTypeExtensions" />.
/// </remarks>
public class ObjectProperCollectionMatchWithToleranceResult<TCollection, TThat, TItem, TTolerance>
	: AndOrResult<TCollection, TThat, ObjectProperCollectionMatchWithToleranceResult<TCollection, TThat, TItem, TTolerance>>,
		IOptionsProvider<CollectionMatchOptions>,
		ICollectionContainmentOptions,
		IProperContainmentOptions,
		IObjectEqualityWithToleranceResult<
			ObjectProperCollectionMatchWithToleranceResult<TCollection, TThat, TItem, TTolerance>, TItem, TTolerance>
{
	private readonly IOptionsHolderSource<CollectionMatchOptions> _collectionMatchOptions;
	private readonly IOptionsHolderSource<ObjectMatchType<TItem>> _options;
	private readonly TThat _returnValue;

	internal ObjectProperCollectionMatchWithToleranceResult(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		IOptionsHolderSource<ObjectMatchType<TItem>> options,
		IOptionsHolderSource<CollectionMatchOptions> collectionMatchOptions)
		: base(expectationBuilder, returnValue)
	{
		_returnValue = returnValue;
		_options = options;
		_collectionMatchOptions = collectionMatchOptions;
	}

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsHolder<ObjectMatchType<TItem>> IOptionsProvider<ObjectMatchType<TItem>>.Options => _options.Holder;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsHolder<CollectionMatchOptions> IOptionsProvider<CollectionMatchOptions>.Options => _collectionMatchOptions.Holder;

	/// <inheritdoc cref="ObjectMatchTypeExtensions.Within{TSelf, TElement, TTolerance}(IObjectEqualityWithToleranceResult{TSelf, TElement, TTolerance}, TTolerance)" />
	public WithinResult Within(TTolerance tolerance)
	{
		_options.Holder.Within(tolerance);
		return new WithinResult(ExpectationBuilder, _returnValue, _options, _collectionMatchOptions);
	}

	/// <summary>
	///     The result directly after <see cref="Within(TTolerance)" />, whose tolerance can still be made relative to
	///     the expected value, e.g. via <c>Percent()</c>.
	/// </summary>
	public sealed class WithinResult
		: ObjectProperCollectionMatchWithToleranceResult<TCollection, TThat, TItem, TTolerance>,
			IWithinToleranceResult<ObjectProperCollectionMatchWithToleranceResult<TCollection, TThat, TItem, TTolerance>,
				TTolerance>
	{
		internal WithinResult(
			ExpectationBuilder expectationBuilder,
			TThat returnValue,
			IOptionsHolderSource<ObjectMatchType<TItem>> options,
			IOptionsHolderSource<CollectionMatchOptions> collectionMatchOptions)
			: base(expectationBuilder, returnValue, options, collectionMatchOptions)
		{
		}

		/// <inheritdoc cref="IWithinToleranceResult{TResult,TTolerance}.SetRelativeTolerance(Func{TTolerance, TTolerance, TTolerance}, string)" />
		ObjectProperCollectionMatchWithToleranceResult<TCollection, TThat, TItem, TTolerance>
			IWithinToleranceResult<ObjectProperCollectionMatchWithToleranceResult<TCollection, TThat, TItem, TTolerance>,
				TTolerance>.SetRelativeTolerance(
				Func<TTolerance, TTolerance, TTolerance> absoluteTolerance, string unit)
		{
			((IOptionsProvider<ObjectMatchType<TItem>>)this).Options
				.SetRelativeTolerance<TItem, TTolerance>(absoluteTolerance, unit);
			return this;
		}
	}
}
