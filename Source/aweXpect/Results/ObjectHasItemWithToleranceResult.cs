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
///     <see cref="ObjectEqualityOptionsExtensions" />.
/// </remarks>
public class ObjectHasItemWithToleranceResult<TCollection, TItem, TTolerance>
	: AndOrResult<TCollection, IThat<TCollection?>, ObjectHasItemWithToleranceResult<TCollection, TItem, TTolerance>>,
		IOptionsProvider<CollectionIndexOptions>,
		IObjectEqualityWithToleranceResult<ObjectHasItemWithToleranceResult<TCollection, TItem, TTolerance>, TItem,
			TTolerance>
{
	private readonly IThat<TCollection?> _collection;
	private readonly OptionsSlot<CollectionIndexOptions> _collectionIndexOptions;
	private readonly OptionsSlot<ObjectEqualityWithToleranceOptions<TItem, TTolerance>> _options;

	internal ObjectHasItemWithToleranceResult(
		ExpectationBuilder expectationBuilder,
		IThat<TCollection?> collection,
		OptionsSlot<CollectionIndexOptions> collectionIndexOptions,
		OptionsSlot<ObjectEqualityWithToleranceOptions<TItem, TTolerance>> options)
		: base(expectationBuilder, collection)
	{
		_collection = collection;
		_collectionIndexOptions = collectionIndexOptions;
		_options = options;
	}

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	/// <remarks>
	///     A view on the options of the expectation, so that the options of every object comparison can change them.
	/// </remarks>
	OptionsSlot<ObjectEqualityOptions<TItem>> IOptionsProvider<ObjectEqualityOptions<TItem>>.Options
		=> new(_options.Value);

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsSlot<ObjectEqualityWithToleranceOptions<TItem, TTolerance>>
		IOptionsProvider<ObjectEqualityWithToleranceOptions<TItem, TTolerance>>.Options => _options;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsSlot<CollectionIndexOptions> IOptionsProvider<CollectionIndexOptions>.Options => _collectionIndexOptions;

	/// <inheritdoc cref="ObjectEqualityOptionsExtensions.Within{TSelf, TElement, TTolerance}(IObjectEqualityWithToleranceResult{TSelf, TElement, TTolerance}, TTolerance)" />
	public WithinResult Within(TTolerance tolerance)
	{
		_options.Within(tolerance);
		return new WithinResult(ExpectationBuilder, _collection, _collectionIndexOptions, _options);
	}

	/// <summary>
	///     The result directly after <see cref="Within(TTolerance)" />, whose tolerance can still be made relative to
	///     the expected value, e.g. via <c>Percent()</c>.
	/// </summary>
	public sealed class WithinResult
		: ObjectHasItemWithToleranceResult<TCollection, TItem, TTolerance>,
			IWithinToleranceResult<ObjectHasItemWithToleranceResult<TCollection, TItem, TTolerance>, TTolerance>
	{
		internal WithinResult(
			ExpectationBuilder expectationBuilder,
			IThat<TCollection?> collection,
			OptionsSlot<CollectionIndexOptions> collectionIndexOptions,
			OptionsSlot<ObjectEqualityWithToleranceOptions<TItem, TTolerance>> options)
			: base(expectationBuilder, collection, collectionIndexOptions, options)
		{
		}

		/// <inheritdoc cref="IWithinToleranceResult{TResult,TTolerance}.WithRelativeTolerance(Func{TTolerance, TTolerance, TTolerance}, string)" />
		ObjectHasItemWithToleranceResult<TCollection, TItem, TTolerance>
			IWithinToleranceResult<ObjectHasItemWithToleranceResult<TCollection, TItem, TTolerance>, TTolerance>.WithRelativeTolerance(
				Func<TTolerance, TTolerance, TTolerance> absoluteTolerance, string unit)
		{
			((IOptionsProvider<ObjectEqualityWithToleranceOptions<TItem, TTolerance>>)this).Options.Value
				.UseRelativeTolerance(absoluteTolerance, unit);
			return this;
		}
	}
}
