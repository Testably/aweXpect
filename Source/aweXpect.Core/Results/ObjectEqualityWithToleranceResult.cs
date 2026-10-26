using System;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result of an expectation with an underlying value of type <typeparamref name="TType" />, allowing a
///     <typeparamref name="TTolerance" /> on the comparison.
/// </summary>
/// <remarks>
///     The options are specified via <see cref="ObjectMatchTypeExtensions" />.
/// </remarks>
public class ObjectEqualityWithToleranceResult<TType, TThat, TElement, TTolerance>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	OptionsSlot<ObjectMatchType<TElement>> options)
	: AndOrResult<TType, TThat, ObjectEqualityWithToleranceResult<TType, TThat, TElement, TTolerance>>(
			expectationBuilder, returnValue),
		IObjectEqualityWithToleranceResult<ObjectEqualityWithToleranceResult<TType, TThat, TElement, TTolerance>,
			TElement, TTolerance>
{
	private readonly TThat _returnValue = returnValue;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsSlot<ObjectMatchType<TElement>> IOptionsProvider<ObjectMatchType<TElement>>.Options => options;

	/// <inheritdoc cref="ObjectMatchTypeExtensions.Within{TSelf, TElement, TTolerance}(IObjectEqualityWithToleranceResult{TSelf, TElement, TTolerance}, TTolerance)" />
	public WithinResult Within(TTolerance tolerance)
	{
		options.Within(tolerance);
		return new WithinResult(ExpectationBuilder, _returnValue, options);
	}

	/// <summary>
	///     The result directly after <see cref="Within(TTolerance)" />, whose tolerance can still be made relative to
	///     the expected value, e.g. via <c>Percent()</c>.
	/// </summary>
	public sealed class WithinResult(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		OptionsSlot<ObjectMatchType<TElement>> options)
		: ObjectEqualityWithToleranceResult<TType, TThat, TElement, TTolerance>(expectationBuilder, returnValue,
				options),
			IWithinToleranceResult<ObjectEqualityWithToleranceResult<TType, TThat, TElement, TTolerance>, TTolerance>
	{
		/// <inheritdoc cref="IWithinToleranceResult{TResult,TTolerance}.WithRelativeTolerance(Func{TTolerance, TTolerance, TTolerance}, string)" />
		ObjectEqualityWithToleranceResult<TType, TThat, TElement, TTolerance>
			IWithinToleranceResult<ObjectEqualityWithToleranceResult<TType, TThat, TElement, TTolerance>, TTolerance>.WithRelativeTolerance(
				Func<TTolerance, TTolerance, TTolerance> absoluteTolerance, string unit)
		{
			((IOptionsProvider<ObjectMatchType<TElement>>)this).Options
				.WithRelativeTolerance<TElement, TTolerance>(absoluteTolerance, unit);
			return this;
		}
	}
}
