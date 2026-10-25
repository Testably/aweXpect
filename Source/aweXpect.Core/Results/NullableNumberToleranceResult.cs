using System;
#if NET8_0_OR_GREATER
using System.Numerics;
#endif
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result of an expectation with an underlying value of type <typeparamref name="TType" />?.
///     <para />
///     In addition to the combinations from <see cref="AndOrResult{TType,TThat}" />, allows specifying a
///     tolerance via <see cref="ToleranceExtensions" />.
/// </summary>
public class NullableNumberToleranceResult<TType, TThat>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	IOptionsHolderSource<NumberTolerance<TType>> options)
	: AndOrResult<TType?, TThat, NullableNumberToleranceResult<TType, TThat>>(expectationBuilder, returnValue),
		INumberToleranceResult<NullableNumberToleranceResult<TType, TThat>, TType>
#if NET8_0_OR_GREATER
	where TType : struct, INumber<TType>
#else
	where TType : struct, IComparable<TType>
#endif
{
	private readonly TThat _returnValue = returnValue;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsHolder<NumberTolerance<TType>> IOptionsProvider<NumberTolerance<TType>>.Options => options.Holder;

	/// <inheritdoc cref="ToleranceExtensions.Within{TSelf, TNumber}(INumberToleranceResult{TSelf, TNumber}, TNumber)" />
	public WithinResult Within(TType tolerance)
	{
		options.Holder.Within(tolerance);
		return new WithinResult(ExpectationBuilder, _returnValue, options);
	}

	/// <summary>
	///     The result directly after <see cref="NullableNumberToleranceResult{TType,TThat}.Within" />, whose tolerance can still be made relative to the
	///     expected value, e.g. via <c>Percent()</c>.
	/// </summary>
	public sealed class WithinResult(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		IOptionsHolderSource<NumberTolerance<TType>> options)
		: NullableNumberToleranceResult<TType, TThat>(expectationBuilder, returnValue, options),
			IWithinToleranceResult<NullableNumberToleranceResult<TType, TThat>, TType>
	{
		/// <inheritdoc cref="IWithinToleranceResult{TResult,TTolerance}.SetRelativeTolerance(Func{TTolerance, TTolerance, TTolerance}, string)" />
		NullableNumberToleranceResult<TType, TThat> IWithinToleranceResult<NullableNumberToleranceResult<TType, TThat>, TType>.SetRelativeTolerance(
			Func<TType, TType, TType> absoluteTolerance, string unit)
		{
			OptionsHolder<NumberTolerance<TType>> holder = ((IOptionsProvider<NumberTolerance<TType>>)this).Options;
			holder.Value = holder.Value.SetRelativeTolerance(absoluteTolerance, unit);
			return this;
		}
	}
}
