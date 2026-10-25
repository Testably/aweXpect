using System;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a collection has a matching item, optionally at a given index, allowing a
///     <typeparamref name="TTolerance" /> on the comparison.
/// </summary>
/// <remarks>
///     The options are specified via <see cref="CollectionIndexOptionsExtensions" /> and
///     <see cref="ObjectMatchTypeExtensions" />.
/// </remarks>
public class ObjectHasItemWithToleranceResult<TCollection, TThat, TItem, TTolerance>
	: AndOrResult<TCollection, TThat, ObjectHasItemWithToleranceResult<TCollection, TThat, TItem, TTolerance>>,
		IOptionsProvider<CollectionIndexOptions>,
		IObjectEqualityWithToleranceResult<ObjectHasItemWithToleranceResult<TCollection, TThat, TItem, TTolerance>, TItem,
			TTolerance>
{
	private readonly TThat _returnValue;
	private readonly IOptionsHolderSource<CollectionIndexOptions> _collectionIndexOptions;
	private readonly IOptionsHolderSource<ObjectMatchType<TItem>> _options;

	internal ObjectHasItemWithToleranceResult(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		IOptionsHolderSource<CollectionIndexOptions> collectionIndexOptions,
		IOptionsHolderSource<ObjectMatchType<TItem>> options)
		: base(expectationBuilder, returnValue)
	{
		_returnValue = returnValue;
		_collectionIndexOptions = collectionIndexOptions;
		_options = options;
	}

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsHolder<ObjectMatchType<TItem>> IOptionsProvider<ObjectMatchType<TItem>>.Options => _options.Holder;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsHolder<CollectionIndexOptions> IOptionsProvider<CollectionIndexOptions>.Options => _collectionIndexOptions.Holder;

	/// <inheritdoc cref="ObjectMatchTypeExtensions.Within{TSelf, TElement, TTolerance}(IObjectEqualityWithToleranceResult{TSelf, TElement, TTolerance}, TTolerance)" />
	public WithinResult Within(TTolerance tolerance)
	{
		_options.Holder.Within(tolerance);
		return new WithinResult(ExpectationBuilder, _returnValue, _collectionIndexOptions, _options);
	}

	/// <summary>
	///     The result directly after <see cref="Within(TTolerance)" />, whose tolerance can still be made relative to
	///     the expected value, e.g. via <c>Percent()</c>.
	/// </summary>
	public sealed class WithinResult
		: ObjectHasItemWithToleranceResult<TCollection, TThat, TItem, TTolerance>,
			IWithinToleranceResult<ObjectHasItemWithToleranceResult<TCollection, TThat, TItem, TTolerance>, TTolerance>
	{
		internal WithinResult(
			ExpectationBuilder expectationBuilder,
			TThat returnValue,
			IOptionsHolderSource<CollectionIndexOptions> collectionIndexOptions,
			IOptionsHolderSource<ObjectMatchType<TItem>> options)
			: base(expectationBuilder, returnValue, collectionIndexOptions, options)
		{
		}

		/// <inheritdoc cref="IWithinToleranceResult{TResult,TTolerance}.SetRelativeTolerance(Func{TTolerance, TTolerance, TTolerance}, string)" />
		ObjectHasItemWithToleranceResult<TCollection, TThat, TItem, TTolerance>
			IWithinToleranceResult<ObjectHasItemWithToleranceResult<TCollection, TThat, TItem, TTolerance>, TTolerance>.SetRelativeTolerance(
				Func<TTolerance, TTolerance, TTolerance> absoluteTolerance, string unit)
		{
			((IOptionsProvider<ObjectMatchType<TItem>>)this).Options
				.SetRelativeTolerance<TItem, TTolerance>(absoluteTolerance, unit);
			return this;
		}
	}
}
