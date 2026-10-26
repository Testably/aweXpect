using System;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying how often an item occurs in a collection, allowing a
///     <typeparamref name="TTolerance" /> on the comparison.
/// </summary>
/// <remarks>
///     The options are specified via <see cref="QuantifierExtensions" /> and
///     <see cref="ObjectMatchTypeExtensions" />.
/// </remarks>
public class ObjectCountWithToleranceResult<TCollection, TThat, TItem, TTolerance>
	: AndOrResult<TCollection, TThat, ObjectCountWithToleranceResult<TCollection, TThat, TItem, TTolerance>>,
		IOptionsProvider<Quantifier>,
		IObjectEqualityWithToleranceResult<ObjectCountWithToleranceResult<TCollection, TThat, TItem, TTolerance>,
			TItem, TTolerance>
{
	private readonly OptionsHolder<ObjectMatchType<TItem>> _options;
	private readonly OptionsHolder<Quantifier> _quantifier;
	private readonly TThat _returnValue;

	internal ObjectCountWithToleranceResult(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		OptionsHolder<Quantifier> quantifier,
		OptionsHolder<ObjectMatchType<TItem>> options)
		: base(expectationBuilder, returnValue)
	{
		_returnValue = returnValue;
		_quantifier = quantifier;
		_options = options;
	}

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsHolder<ObjectMatchType<TItem>> IOptionsProvider<ObjectMatchType<TItem>>.Options => _options;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsHolder<Quantifier> IOptionsProvider<Quantifier>.Options => _quantifier;

	/// <inheritdoc cref="ObjectMatchTypeExtensions.Within{TSelf, TItem, TTolerance}(IObjectEqualityWithToleranceResult{TSelf, TItem, TTolerance}, TTolerance)" />
	public WithinResult Within(TTolerance tolerance)
	{
		_options.Within(tolerance);
		return new WithinResult(ExpectationBuilder, _returnValue, _quantifier, _options);
	}

	/// <summary>
	///     The result directly after <see cref="Within(TTolerance)" />, whose tolerance can still be made relative to
	///     the expected value, e.g. via <c>Percent()</c>.
	/// </summary>
	public sealed class WithinResult
		: ObjectCountWithToleranceResult<TCollection, TThat, TItem, TTolerance>,
			IWithinToleranceResult<ObjectCountWithToleranceResult<TCollection, TThat, TItem, TTolerance>, TTolerance>
	{
		internal WithinResult(
			ExpectationBuilder expectationBuilder,
			TThat returnValue,
			OptionsHolder<Quantifier> quantifier,
			OptionsHolder<ObjectMatchType<TItem>> options)
			: base(expectationBuilder, returnValue, quantifier, options)
		{
		}

		/// <inheritdoc cref="IWithinToleranceResult{TResult,TTolerance}.SetRelativeTolerance(Func{TTolerance, TTolerance, TTolerance}, string)" />
		ObjectCountWithToleranceResult<TCollection, TThat, TItem, TTolerance>
			IWithinToleranceResult<ObjectCountWithToleranceResult<TCollection, TThat, TItem, TTolerance>, TTolerance>.SetRelativeTolerance(
				Func<TTolerance, TTolerance, TTolerance> absoluteTolerance, string unit)
		{
			((IOptionsProvider<ObjectMatchType<TItem>>)this).Options
				.SetRelativeTolerance<TItem, TTolerance>(absoluteTolerance, unit);
			return this;
		}
	}
}
